using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Dashboard;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkflowMgmt.Application.Features.Dashboard
{
    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, ApiResponse<RoleDashboardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDashboardDataQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RoleDashboardDto>> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Guid.TryParse(request.UserId, out var userId))
                {
                    return ApiResponse<RoleDashboardDto>.ErrorResponse("Invalid user ID format");
                }

                // Get user details
                var user = await _unitOfWork.UserRepository.GetUserById(userId);
                if (user == null)
                {
                    return ApiResponse<RoleDashboardDto>.ErrorResponse("User not found");
                }

                var role = await _unitOfWork.UserRepository.GetRoleByRoleId(user.role_id);
                var department = await _unitOfWork.UserRepository.GetDepartmentByDepartmentId(user.department_id);

                if (role == null)
                {
                    return ApiResponse<RoleDashboardDto>.ErrorResponse("User role not found");
                }

                // Get dashboard data based on role
                var dashboardData = await _unitOfWork.DashboardRepository.GetDashboardDataByRole(
                    userId, 
                    role.code, 
                    user.department_id
                );

                dashboardData.UserRole = role.name;
                dashboardData.UserName = $"{user.first_name} {user.last_name}";
                dashboardData.Department = department?.name ?? string.Empty;

                return ApiResponse<RoleDashboardDto>.SuccessResponse(dashboardData);
            }
            catch (Exception ex)
            {
                return ApiResponse<RoleDashboardDto>.ErrorResponse($"Error retrieving dashboard data: {ex.Message}");
            }
        }
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.User
{
    public class GetActiveUsersCommandHandler : IRequestHandler<GetActiveUsersCommand, ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveUsersCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>> Handle(GetActiveUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetActiveUsers();
                return ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>.SuccessResponse(users, "Active users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>.ErrorResponse($"Error during fetching active users: {ex.Message}");
            }
        }
    }

    public class GetActiveUsersByDepartmentCommandHandler : IRequestHandler<GetActiveUsersByDepartmentCommand, ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveUsersByDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>> Handle(GetActiveUsersByDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetActiveUsersByDepartment(request.departmentId);
                return ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>.SuccessResponse(users, "Active users by department retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>.ErrorResponse($"Error during fetching active users by department: {ex.Message}");
            }
        }
    }

    public class GetActiveUsersByAllowedDepartmentCommandHandler : IRequestHandler<GetActiveUsersByAllowedDepartmentCommand, ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveUsersByAllowedDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>> Handle(GetActiveUsersByAllowedDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetActiveUsersByAllowedDepartment(request.departmentId);
                return ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>.SuccessResponse(users, "Active users by allowed department retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>.ErrorResponse($"Error during fetching active users by allowed department: {ex.Message}");
            }
        }
    }

    public class GetActiveUsersByAllowedDepartmentAndRoleCommandHandler : IRequestHandler<GetActiveUsersByAllowedDepartmentAndRoleCommand, ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveUsersByAllowedDepartmentAndRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>> Handle(GetActiveUsersByAllowedDepartmentAndRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _unitOfWork.UserRepository.GetActiveUsersByAllowedDepartmentAndRole(request.departmentId, request.roleId);
                return ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>.SuccessResponse(users, "Active users by allowed department and role retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>.ErrorResponse($"Error during fetching active users by allowed department and role: {ex.Message}");
            }
        }
    }
}

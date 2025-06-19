using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Auth;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Auth
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ApiResponse<UserProfileDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProfileQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Guid.TryParse(request.userId, out var userId))
                {
                    return ApiResponse<UserProfileDto>.ErrorResponse("Invalid user ID format");
                }

                // Get user by ID
                var user = await _unitOfWork.UserRepository.GetUserById(userId);
                
                if (user == null)
                {
                    return ApiResponse<UserProfileDto>.ErrorResponse("User not found");
                }

                var role = await _unitOfWork.UserRepository.GetRoleByRoleId(user.role_id);
                var department = await _unitOfWork.UserRepository.GetDepartmentByDepartmentId(user.department_id);

                var profile = new UserProfileDto
                {
                    Id = user.id.ToString(),
                    Username = user.username,
                    Email = user.email,
                    FirstName = user.first_name,
                    LastName = user.last_name,
                    FullName = $"{user.first_name} {user.last_name}",
                    Role = role?.name ?? string.Empty,
                    RoleCode = role?.code ?? string.Empty,
                    Department = department?.name ?? string.Empty,
                    DepartmentCode = department?.code ?? string.Empty,
                    ProfileImageUrl = user.profile_image_url,
                    LastLogin = user.last_login,
                    Permissions = role?.permissions ?? Array.Empty<string>()
                };

                return ApiResponse<UserProfileDto>.SuccessResponse(profile, "Profile retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserProfileDto>.ErrorResponse($"Error retrieving profile: {ex.Message}");
            }
        }
    }
}

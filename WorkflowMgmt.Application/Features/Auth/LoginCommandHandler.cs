using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Interface.JwtToken;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork)
        {
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Support login by username or email
                var user = await _unitOfWork.UserRepository.GetUserByUserName(request.Username);


                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.password_hash))
                {
                    return ApiResponse<LoginResponse>.ErrorResponse("Invalid credentials");
                }
                Role? role = new Role();
                DepartmentDTO? department = new DepartmentDTO();
                string token = string.Empty;
                bool userExists = user != null;
                LoginResponse response = new LoginResponse();

                // Update last login
                if (user != null)
                {
                    await _unitOfWork.UserRepository.UpdateLastLoginAsync(user.id);
                    role = await _unitOfWork.UserRepository.GetRoleByRoleId(user.role_id);
                    department = await _unitOfWork.UserRepository.GetDepartmentByDepartmentId(user.department_id);
                    token = _jwtTokenService.GenerateToken(user, role, department);
                    _unitOfWork.Commit();

                    response = new LoginResponse
                    {
                        Token = token,
                        Username = user.username,
                        Email = user.email,
                        FirstName = user.first_name,
                        LastName = user.last_name,
                        FullName = $"{user.first_name} {user.last_name}",
                        Role = role?.name ?? string.Empty,
                        RoleCode = role?.code ?? string.Empty,
                        Department = department?.name ?? string.Empty,
                        DepartmentCode = department?.code ?? string.Empty,
                        UserId = user.id.ToString(),
                        ProfileImageUrl = user.profile_image_url,
                        LastLogin = user.last_login,
                        Permissions = role?.permissions ?? Array.Empty<string>()
                    };
                }



                return ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponse>.ErrorResponse($"Error during login: {ex.Message}");
            }
        }
    }
}

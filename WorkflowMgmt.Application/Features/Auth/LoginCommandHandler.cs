using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Auth;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Interface.JwtToken;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<LoginResponse>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public LoginCommandHandler(IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork ,IConfiguration configuration)
        {
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
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

                // Update last login
                await _unitOfWork.UserRepository.UpdateLastLoginAsync(user.id);

                // Get user role and department
                var role = await _unitOfWork.UserRepository.GetRoleByRoleId(user.role_id);
                var department = await _unitOfWork.UserRepository.GetDepartmentByDepartmentId(user.department_id);

                // Generate tokens
                var tokens = _jwtTokenService.GenerateTokens(user, role, department);

                // Create new refresh token record for each login (multi-device support)
                var refreshTokenExpiryMinutes = int.Parse(_configuration["Jwt:RefreshTokenExpiresInMinutes"] ?? "10080");
                var refreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = tokens.RefreshToken,
                    UserId = user.id,
                    ExpiresAt = DateTime.Now.AddMinutes(refreshTokenExpiryMinutes),
                    CreatedAt = DateTime.Now,
                    IsRevoked = false
                };

                await _unitOfWork.RefreshTokenRepository.CreateAsync(refreshToken);

                _unitOfWork.Commit();

                var response = new LoginResponse
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = tokens.ExpiresAt
                };

                return ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<LoginResponse>.ErrorResponse($"Error during login: {ex.Message}");
            }
        }
    }
}

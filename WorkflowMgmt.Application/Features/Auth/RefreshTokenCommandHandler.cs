using MediatR;
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
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthTokenResponse>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork)
        {
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AuthTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate refresh token
                var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request.refreshToken);
                
                if (refreshToken == null)
                {
                    return ApiResponse<AuthTokenResponse>.ErrorResponse("Invalid refresh token");
                }

                if (refreshToken.IsRevoked)
                {
                    return ApiResponse<AuthTokenResponse>.ErrorResponse("Refresh token has been revoked");
                }

                if (refreshToken.ExpiresAt <= DateTime.Now)
                {
                    return ApiResponse<AuthTokenResponse>.ErrorResponse("Refresh token has expired");
                }

                // Get user details
                var user = await _unitOfWork.UserRepository.GetUserById(refreshToken.UserId);
                if (user == null)
                {
                    return ApiResponse<AuthTokenResponse>.ErrorResponse("User not found");
                }

                var role = await _unitOfWork.UserRepository.GetRoleByRoleId(user.role_id);
                var department = await _unitOfWork.UserRepository.GetDepartmentByDepartmentId(user.department_id);

                // Generate new tokens
                var newTokens = _jwtTokenService.GenerateTokens(user, role, department);

                // Revoke old refresh token
                await _unitOfWork.RefreshTokenRepository.RevokeTokenAsync(request.refreshToken, "Replaced by new token");

                // Create new refresh token record
                var newRefreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    Token = newTokens.RefreshToken,
                    UserId = user.id,
                    ExpiresAt = DateTime.UtcNow.AddDays(7), // From configuration
                    CreatedAt = DateTime.UtcNow,
                    IsRevoked = false
                };

                await _unitOfWork.RefreshTokenRepository.CreateAsync(newRefreshToken);
                _unitOfWork.Commit();

                return ApiResponse<AuthTokenResponse>.SuccessResponse(newTokens, "Tokens refreshed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<AuthTokenResponse>.ErrorResponse($"Error refreshing token: {ex.Message}");
            }
        }
    }
}

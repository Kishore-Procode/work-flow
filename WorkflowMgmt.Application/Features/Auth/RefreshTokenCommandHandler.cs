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
using Microsoft.Extensions.Configuration;

namespace WorkflowMgmt.Application.Features.Auth
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthTokenResponse>>
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public RefreshTokenCommandHandler(IJwtTokenService jwtTokenService, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<ApiResponse<AuthTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Clean up expired tokens (7 days old) to prevent database bloat
                await CleanupExpiredTokensAsync();

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

                // Update existing refresh token with new token (rotation)
                refreshToken.Token = newTokens.RefreshToken;
                refreshToken.ExpiresAt = DateTime.Now.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpiresInDays"])); // 7 days expiry
                refreshToken.CreatedAt = DateTime.Now;
                refreshToken.IsRevoked = false;
                refreshToken.RevokedReason = null;
                refreshToken.RevokedAt = null;

                await _unitOfWork.RefreshTokenRepository.UpdateAsync(refreshToken);
                _unitOfWork.Commit();

                // Return response with updated token details from database
                var response = new AuthTokenResponse
                {
                    AccessToken = newTokens.AccessToken,
                    RefreshToken = refreshToken.Token,
                    ExpiresAt = newTokens.ExpiresAt
                };

                return ApiResponse<AuthTokenResponse>.SuccessResponse(response, "Tokens refreshed successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<AuthTokenResponse>.ErrorResponse($"Error refreshing token: {ex.Message}");
            }
        }

        private async Task CleanupExpiredTokensAsync()
        {
            try
            {
                // Delete tokens older than 7 days to prevent database bloat
                await _unitOfWork.RefreshTokenRepository.DeleteExpiredTokensAsync();
            }
            catch (Exception)
            {
                // Ignore cleanup errors, don't fail the refresh operation
            }
        }
    }
}

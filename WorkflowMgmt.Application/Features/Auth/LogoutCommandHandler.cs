using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Auth
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Guid.TryParse(request.userId, out var userId))
                {
                    return ApiResponse<string>.ErrorResponse("Invalid user ID");
                }

                // If specific refresh token is provided, revoke only that token (single device logout)
                if (!string.IsNullOrWhiteSpace(request.refreshToken))
                {
                    var refreshToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request.refreshToken);
                    if (refreshToken != null && refreshToken.UserId == userId && !refreshToken.IsRevoked)
                    {
                        // Revoke the specific refresh token (logout from this device only)
                        await _unitOfWork.RefreshTokenRepository.RevokeTokenAsync(request.refreshToken, "User logout - single device");
                    }
                }
                else
                {
                    // Revoke all refresh tokens for the user (logout from all devices)
                    await _unitOfWork.RefreshTokenRepository.RevokeAllUserTokensAsync(userId, "User logout - all devices");
                }

                _unitOfWork.Commit();

                return ApiResponse<string>.SuccessResponse("Logged out successfully", "User logged out successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<string>.ErrorResponse($"Error during logout: {ex.Message}");
            }
        }
    }
}

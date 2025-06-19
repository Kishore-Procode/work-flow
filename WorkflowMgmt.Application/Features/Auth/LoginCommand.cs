using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Auth;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Auth
{
    public record LoginCommand(string Username, string Password) : IRequest<ApiResponse<LoginResponse>>;
    public record RefreshTokenCommand(string refreshToken) : IRequest<ApiResponse<AuthTokenResponse>>;
    public record GetProfileQuery(string userId) : IRequest<ApiResponse<UserProfileDto>>;
    public record ChangePasswordCommand(string userId, string currentPassword, string newPassword) : IRequest<ApiResponse<string>>;
    public record ForgotPasswordCommand(string email) : IRequest<ApiResponse<string>>;
    public record ResetPasswordCommand(string token, string email, string newPassword) : IRequest<ApiResponse<string>>;
}

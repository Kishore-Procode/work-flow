using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.UserManagement
{
    public record GetUserManagementCommand() : IRequest<ApiResponse<List<UserDTO>>>;
    public record GetUserManagementById(Guid id) : IRequest<ApiResponse<UserDTO>>;

    public record CreateUserCommand(UserDTO User) : IRequest<ApiResponse<Guid>>;

    public record UpdateUserCommand(UserDTO user) : IRequest<ApiResponse<bool>>;

    public record DeleteOrRestoreUserCommand(Guid id, string modifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;

    public record UpdatePasswordCommand(UpdatePasswordRequest user) : IRequest<ApiResponse<bool>>;
}

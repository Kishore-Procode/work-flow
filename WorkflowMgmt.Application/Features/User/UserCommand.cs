using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.User
{
    public record GetActiveUsersCommand() : IRequest<ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>>;
    public record GetActiveUsersByDepartmentCommand(int departmentId) : IRequest<ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>>;
    public record GetActiveUsersByAllowedDepartmentCommand(int departmentId) : IRequest<ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>>;
    public record GetActiveUsersByAllowedDepartmentAndRoleCommand(int departmentId, int roleId) : IRequest<ApiResponse<List<WorkflowMgmt.Domain.Entities.UserDto>>>;
}

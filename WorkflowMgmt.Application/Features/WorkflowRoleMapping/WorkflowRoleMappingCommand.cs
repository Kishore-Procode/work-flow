using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowRoleMapping
{
    public record GetDepartmentRoleUsersCommand() : IRequest<ApiResponse<List<WorkflowRoleMappingDto>>>;
    public record GetDepartmentRoleUsersByDepartmentCommand(int departmentId) : IRequest<ApiResponse<List<WorkflowRoleMappingDto>>>;
    public record UpdateDepartmentRoleUsersCommand(UpdateDepartmentRoleUsersRequest request) : IRequest<ApiResponse<bool>>;
}

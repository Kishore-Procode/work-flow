using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Application.Features.Auth;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Department
{
    public record GetDepartmentCommand() : IRequest<ApiResponse<List<DepartmentDTO>>>;
    public record GetDepartmentByIdCommand(int id) : IRequest<ApiResponse<DepartmentDTO>>;
    public record CreateDepartmentCommand(DepartmentDTO Department) : IRequest<ApiResponse<int>>;
    public record UpdateDepartmentCommand(DepartmentDTO Department) : IRequest<ApiResponse<bool>>;
    public record DeleteOrRestoreDepartmentCommand(int id, string modifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;
    public record GetDepartmentStatsQuery() : IRequest<ApiResponse<DepartmentStatsDto>>;
}

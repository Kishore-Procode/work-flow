using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Stats;

namespace WorkflowMgmt.Application.Features.Stats
{
    public record GetDepartmentStatsQuery() : IRequest<ApiResponse<DepartmentStatsDto>>;
    public record GetCourseStatsQuery() : IRequest<ApiResponse<CourseStatsDto>>;
    public record GetSemesterStatsQuery() : IRequest<ApiResponse<SemesterStatsDto>>;
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Semesters;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Semester
{
    public record GetSemesterCommand() : IRequest<ApiResponse<List<SemesterDTO>>>;

    public record GetSemesterByIdCommand(int Id) : IRequest<ApiResponse<SemesterDTO>>;

    public record GetSemestersByDepartmentCommand(int DepartmentId) : IRequest<ApiResponse<List<SemesterDTO>>>;

    public record GetSemestersByDepartmentAndCourseCommand(int DepartmentId, int CourseId) : IRequest<ApiResponse<List<SemesterDTO>>>;

    public record CreateSemesterCommand(SemesterDTO Semester) : IRequest<ApiResponse<int>>;

    public record UpdateSemesterCommand(SemesterDTO Semester) : IRequest<ApiResponse<bool>>;

    public record DeleteOrRestoreSemesterCommand(int Id, string ModifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;
}

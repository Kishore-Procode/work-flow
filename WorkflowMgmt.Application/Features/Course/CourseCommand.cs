using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Courses;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Course
{
    public record GetCourseCommand() : IRequest<ApiResponse<List<CourseDTO>>>;
    public record GetCourseByIdCommand(int id) : IRequest<ApiResponse<CourseDTO>>;
    public record CreateCourseCommand(CourseDTO Course) : IRequest<ApiResponse<int>>;
    public record UpdateCourseCommand(CourseDTO Course) : IRequest<ApiResponse<bool>>;
    public record DeleteOrRestoreCourseCommand(int id, string modifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;

}

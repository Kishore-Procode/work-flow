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
    
}

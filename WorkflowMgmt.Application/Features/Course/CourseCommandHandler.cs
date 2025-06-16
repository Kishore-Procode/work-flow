using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Application.Features.Department;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Courses;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.IRepository;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Course
{
    public class CourseCommandHandler : IRequestHandler<GetCourseCommand, ApiResponse<List<CourseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<List<CourseDTO>>> Handle(GetCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var courses = await _unitOfWork.CourseRepository.GetAllCourses();
                return ApiResponse<List<CourseDTO>>.SuccessResponse(courses, "Courses retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseDTO>>.ErrorResponse($"Error retrieving courses: {ex.Message}");
            }
        }

    }
}

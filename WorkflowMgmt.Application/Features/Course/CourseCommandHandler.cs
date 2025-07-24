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
                return ApiResponse<List<CourseDTO>>.ErrorResponse($"Error during fetching courses: {ex.Message}");
            }
        }

    }

    public class GetCoursesByDepartmentCommandHandler : IRequestHandler<GetCoursesByDepartmentCommand, ApiResponse<List<CourseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCoursesByDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<List<CourseDTO>>> Handle(GetCoursesByDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var courses = await _unitOfWork.CourseRepository.GetCoursesByDepartmentAsync(request.departmentId);
                return ApiResponse<List<CourseDTO>>.SuccessResponse(courses, "Courses retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CourseDTO>>.ErrorResponse($"Error during fetching courses by department: {ex.Message}");
            }
        }
    }

    public class GetCourseByIdCommandHandler : IRequestHandler<GetCourseByIdCommand, ApiResponse<CourseDTO>>
    {         
        private readonly IUnitOfWork _unitOfWork;
        public GetCourseByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<CourseDTO>> Handle(GetCourseByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var course = await _unitOfWork.CourseRepository.GetCourseById(request.id);
                if (course == null)
                    return ApiResponse<CourseDTO>.ErrorResponse($"Course with ID {request.id} not found");
                return ApiResponse<CourseDTO>.SuccessResponse(course, "Course retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<CourseDTO>.ErrorResponse($"Error fetching course: {ex.Message}");
            }
        }
    }
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ApiResponse<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var courseId = await _unitOfWork.CourseRepository.InsertCourse(request.Course);
                if (courseId > 0)
                    _unitOfWork.Commit();
                return ApiResponse<int>.SuccessResponse(courseId, "Department created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating department: {ex.Message}");
            }
        }
    }
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<bool>> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isUpdated = await _unitOfWork.CourseRepository.UpdateCourse(request.Course);
                if (isUpdated)
                    _unitOfWork.Commit();
                return ApiResponse<bool>.SuccessResponse(isUpdated, "Course updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating course: {ex.Message}");
            }
        }
    }
    public class DeleteOrRestoreCourseCommandHandler : IRequestHandler<DeleteOrRestoreCourseCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteOrRestoreCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var isDeleted = await _unitOfWork.CourseRepository.DeleteOrRestoreCourse(request.id, request.modifiedBy, request.isRestore);
                if (isDeleted)
                    _unitOfWork.Commit();
                return ApiResponse<bool>.SuccessResponse(isDeleted, request.isRestore ? "Course restored successfully" : "Course deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error deleting/restoring course: {ex.Message}");
            }
        }
    }

}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Semesters;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.Semester
{
    public class GetSemesterCommandHandler : IRequestHandler<GetSemesterCommand, ApiResponse<List<SemesterDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSemesterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<SemesterDTO>>> Handle(GetSemesterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var semesters = await _unitOfWork.SemesterRepository.GetAllSemesters();
                return ApiResponse<List<SemesterDTO>>.SuccessResponse(semesters, "Semesters retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SemesterDTO>>.ErrorResponse($"Error fetching semesters: {ex.Message}");
            }
        }
    }

    public class GetSemesterByIdCommandHandler : IRequestHandler<GetSemesterByIdCommand, ApiResponse<SemesterDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSemesterByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<SemesterDTO>> Handle(GetSemesterByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var semester = await _unitOfWork.SemesterRepository.GetSemesterById(request.Id); // ✅ Fixed: request.Id

                if (semester == null)
                    return ApiResponse<SemesterDTO>.ErrorResponse($"Semester with ID {request.Id} not found");

                return ApiResponse<SemesterDTO>.SuccessResponse(semester, "Semester retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<SemesterDTO>.ErrorResponse($"Error fetching semester: {ex.Message}");
            }
        }
    }

    public class GetSemestersByDepartmentCommandHandler : IRequestHandler<GetSemestersByDepartmentCommand, ApiResponse<List<SemesterDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSemestersByDepartmentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<SemesterDTO>>> Handle(GetSemestersByDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var semesters = await _unitOfWork.SemesterRepository.GetSemestersByDepartmentAsync(request.DepartmentId);
                return ApiResponse<List<SemesterDTO>>.SuccessResponse(semesters, "Semesters retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SemesterDTO>>.ErrorResponse($"Error fetching semesters by department: {ex.Message}");
            }
        }
    }

    public class GetSemestersByDepartmentAndCourseCommandHandler : IRequestHandler<GetSemestersByDepartmentAndCourseCommand, ApiResponse<List<SemesterDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSemestersByDepartmentAndCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<SemesterDTO>>> Handle(GetSemestersByDepartmentAndCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var semesters = await _unitOfWork.SemesterRepository.GetSemestersByDepartmentAndCourseAsync(request.DepartmentId, request.CourseId);
                return ApiResponse<List<SemesterDTO>>.SuccessResponse(semesters, "Semesters retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SemesterDTO>>.ErrorResponse($"Error fetching semesters by department and course: {ex.Message}");
            }
        }
    }


    public class CreateSemesterCommandHandler : IRequestHandler<CreateSemesterCommand, ApiResponse<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSemesterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> Handle(CreateSemesterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var semesterId = await _unitOfWork.SemesterRepository.InsertSemester(request.Semester);
                if (semesterId > 0)
                    _unitOfWork.Commit();

                return ApiResponse<int>.SuccessResponse(semesterId, "Semester created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<int>.ErrorResponse($"Error creating semester: {ex.Message}");
            }
        }
    }

    public class UpdateSemesterCommandHandler : IRequestHandler<UpdateSemesterCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSemesterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateSemesterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.SemesterRepository.UpdateSemester(request.Semester);
                if (success)
                    _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(success, "Semester updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating semester: {ex.Message}");
            }
        }
    }

    public class DeleteOrRestoreSemesterCommandHandler : IRequestHandler<DeleteOrRestoreSemesterCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrRestoreSemesterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreSemesterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.SemesterRepository
                    .DeleteOrRestoreSemester(request.Id, request.ModifiedBy, request.isRestore);

                if (result)
                    _unitOfWork.Commit();

                var message = request.isRestore ? "Semester restored successfully" : "Semester deleted successfully";
                return ApiResponse<bool>.SuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Operation failed: {ex.Message}");
            }
        }
    }
}

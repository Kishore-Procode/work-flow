using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.AcademicYear
{
    public class AcademicYearCommandHandler : IRequestHandler<GetAcademicYearCommand, ApiResponse<List<AcademicYearDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AcademicYearCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<AcademicYearDTO>>> Handle(GetAcademicYearCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var academicYears = await _unitOfWork.AcademicYearRepository.GetAllAcademicYears();

                return ApiResponse<List<AcademicYearDTO>>.SuccessResponse(academicYears, "Academic years retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AcademicYearDTO>>.ErrorResponse($"Error during fetching academic years: {ex.Message}");
            }
        }
    }

    public class GetAcademicYearsByLevelCommandHandler : IRequestHandler<GetAcademicYearsByLevelCommand, ApiResponse<List<AcademicYearDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAcademicYearsByLevelCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<AcademicYearDTO>>> Handle(GetAcademicYearsByLevelCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var academicYears = await _unitOfWork.AcademicYearRepository.GetAcademicYearsByLevelId(request.levelId);

                return ApiResponse<List<AcademicYearDTO>>.SuccessResponse(academicYears, "Academic years retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<AcademicYearDTO>>.ErrorResponse($"Error during fetching academic years: {ex.Message}");
            }
        }
    }

    public class GetAcademicYearByIdCommandHandler : IRequestHandler<GetAcademicYearByIdCommand, ApiResponse<AcademicYearDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAcademicYearByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<AcademicYearDTO>> Handle(GetAcademicYearByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var academicYear = await _unitOfWork.AcademicYearRepository.GetAcademicYearById(request.id);

                if (academicYear == null)
                {
                    return ApiResponse<AcademicYearDTO>.ErrorResponse("Academic year not found");
                }

                return ApiResponse<AcademicYearDTO>.SuccessResponse(academicYear, "Academic year retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AcademicYearDTO>.ErrorResponse($"Error during fetching academic year: {ex.Message}");
            }
        }
    }

    public class CreateAcademicYearCommandHandler : IRequestHandler<CreateAcademicYearCommand, ApiResponse<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateAcademicYearCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> Handle(CreateAcademicYearCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var academicYearId = await _unitOfWork.AcademicYearRepository.InsertAcademicYear(request.AcademicYear);
                _unitOfWork.Commit();

                return ApiResponse<int>.SuccessResponse(academicYearId, "Academic year created successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<int>.ErrorResponse($"Error during creating academic year: {ex.Message}");
            }
        }
    }

    public class UpdateAcademicYearCommandHandler : IRequestHandler<UpdateAcademicYearCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAcademicYearCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateAcademicYearCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.AcademicYearRepository.UpdateAcademicYear(request.AcademicYear);
                _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(result, "Academic year updated successfully");
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return ApiResponse<bool>.ErrorResponse($"Error during updating academic year: {ex.Message}");
            }
        }
    }

    public class DeleteOrRestoreAcademicYearCommandHandler : IRequestHandler<DeleteOrRestoreAcademicYearCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrRestoreAcademicYearCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreAcademicYearCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.AcademicYearRepository.DeleteOrRestoreAcademicYear(request.id, request.modifiedBy, request.isRestore);
                _unitOfWork.Commit();

                var message = request.isRestore ? "Academic year restored successfully" : "Academic year deleted successfully";
                return ApiResponse<bool>.SuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                var action = request.isRestore ? "restoring" : "deleting";
                return ApiResponse<bool>.ErrorResponse($"Error during {action} academic year: {ex.Message}");
            }
        }
    }
}

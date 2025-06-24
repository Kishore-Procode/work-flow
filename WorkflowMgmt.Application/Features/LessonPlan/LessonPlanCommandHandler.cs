using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Entities.LessonPlan;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.LessonPlan
{
    public class GetLessonPlansCommandHandler : IRequestHandler<GetLessonPlansCommand, ApiResponse<List<LessonPlanDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlansCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<LessonPlanDto>>> Handle(GetLessonPlansCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlans = await _unitOfWork.LessonPlanRepository.GetAllLessonPlans();
                return ApiResponse<List<LessonPlanDto>>.SuccessResponse(lessonPlans, "Lesson plans retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<LessonPlanDto>>.ErrorResponse($"Error fetching lesson plans: {ex.Message}");
            }
        }
    }

    public class GetLessonPlanByIdCommandHandler : IRequestHandler<GetLessonPlanByIdCommand, ApiResponse<LessonPlanDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlanByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<LessonPlanDto>> Handle(GetLessonPlanByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lessonPlan = await _unitOfWork.LessonPlanRepository.GetLessonPlanById(request.Id);
                if (lessonPlan == null)
                    return ApiResponse<LessonPlanDto>.ErrorResponse($"Lesson plan with ID {request.Id} not found");

                return ApiResponse<LessonPlanDto>.SuccessResponse(lessonPlan, "Lesson plan retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<LessonPlanDto>.ErrorResponse($"Error fetching lesson plan: {ex.Message}");
            }
        }
    }

    public class CreateLessonPlanCommandHandler : IRequestHandler<CreateLessonPlanCommand, ApiResponse<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLessonPlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateLessonPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var id = await _unitOfWork.LessonPlanRepository.InsertLessonPlan(request.LessonPlan);
                if (id != Guid.Empty)
                    _unitOfWork.Commit();

                return ApiResponse<Guid>.SuccessResponse(id, "Lesson plan created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.ErrorResponse($"Error creating lesson plan: {ex.Message}");
            }
        }
    }

    public class UpdateLessonPlanCommandHandler : IRequestHandler<UpdateLessonPlanCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLessonPlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateLessonPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.LessonPlanRepository.UpdateLessonPlan(request.LessonPlan);
                if (success)
                    _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(success, "Lesson plan updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating lesson plan: {ex.Message}");
            }
        }
    }

    public class DeleteOrRestoreLessonPlanCommandHandler : IRequestHandler<DeleteOrRestoreLessonPlanCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrRestoreLessonPlanCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreLessonPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.LessonPlanRepository
                    .DeleteOrRestoreLessonPlan(request.Id, request.ModifiedBy, request.isRestore);

                if (result)
                    _unitOfWork.Commit();

                var message = request.isRestore ? "Lesson plan restored successfully" : "Lesson plan deleted successfully";
                return ApiResponse<bool>.SuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Operation failed: {ex.Message}");
            }
        }
    }
}

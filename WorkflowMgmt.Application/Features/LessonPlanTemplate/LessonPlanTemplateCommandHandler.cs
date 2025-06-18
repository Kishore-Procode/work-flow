using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Entities.LessonPlanTemplate;

using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.LessonPlanTemplate
{
    public class GetLessonPlanTemplatesCommandHandler : IRequestHandler<GetLessonPlanTemplatesCommand, ApiResponse<List<LessonPlanTemplateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlanTemplatesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<LessonPlanTemplateDto>>> Handle(GetLessonPlanTemplatesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var templates = await _unitOfWork.LessonPlanTemplateRepository.GetAllLessonPlanTemplates();
                return ApiResponse<List<LessonPlanTemplateDto>>.SuccessResponse(templates, "Lesson plan templates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<LessonPlanTemplateDto>>.ErrorResponse($"Error fetching lesson plan templates: {ex.Message}");
            }
        }
    }

    public class GetLessonPlanTemplateByIdCommandHandler : IRequestHandler<GetLessonPlanTemplateByIdCommand, ApiResponse<LessonPlanTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLessonPlanTemplateByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<LessonPlanTemplateDto>> Handle(GetLessonPlanTemplateByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var template = await _unitOfWork.LessonPlanTemplateRepository.GetLessonPlanTemplateById(request.Id);
                if (template == null)
                    return ApiResponse<LessonPlanTemplateDto>.ErrorResponse($"Template with ID {request.Id} not found");

                return ApiResponse<LessonPlanTemplateDto>.SuccessResponse(template, "Lesson plan template retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<LessonPlanTemplateDto>.ErrorResponse($"Error fetching lesson plan template: {ex.Message}");
            }
        }
    }

    public class CreateLessonPlanTemplateCommandHandler : IRequestHandler<CreateLessonPlanTemplateCommand, ApiResponse<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLessonPlanTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateLessonPlanTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var id = await _unitOfWork.LessonPlanTemplateRepository.InsertLessonPlanTemplate(request.Template);
                if (id != Guid.Empty)
                    _unitOfWork.Commit();

                return ApiResponse<Guid>.SuccessResponse(id, "Lesson plan template created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.ErrorResponse($"Error creating lesson plan template: {ex.Message}");
            }
        }
    }

    public class UpdateLessonPlanTemplateCommandHandler : IRequestHandler<UpdateLessonPlanTemplateCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLessonPlanTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateLessonPlanTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.LessonPlanTemplateRepository.UpdateLessonPlanTemplate(request.Template);
                if (success)
                    _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(success, "Lesson plan template updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating lesson plan template: {ex.Message}");
            }
        }
    }

    public class DeleteOrRestoreLessonPlanTemplateCommandHandler : IRequestHandler<DeleteOrRestoreLessonPlanTemplateCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrRestoreLessonPlanTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreLessonPlanTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.LessonPlanTemplateRepository
                    .DeleteOrRestoreLessonPlanTemplate(request.Id, request.ModifiedBy, request.isRestore);

                if (result)
                    _unitOfWork.Commit();

                var message = request.isRestore ? "Lesson plan template restored successfully" : "Lesson plan template deleted successfully";
                return ApiResponse<bool>.SuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Operation failed: {ex.Message}");
            }
        }
    }
}

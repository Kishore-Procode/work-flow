using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.SyllabusTemplate;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.SyllabusTemplate
{
    public class GetSyllabusTemplatesCommandHandler : IRequestHandler<GetSyllabusTemplatesCommand, ApiResponse<List<SyllabusTemplateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabusTemplatesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<SyllabusTemplateDto>>> Handle(GetSyllabusTemplatesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var templates = await _unitOfWork.SyllabusTemplateRepository.GetAllSyllabusTemplates();
                return ApiResponse<List<SyllabusTemplateDto>>.SuccessResponse(templates, "Syllabus templates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SyllabusTemplateDto>>.ErrorResponse($"Error fetching syllabus templates: {ex.Message}");
            }
        }
    }

    public class GetSyllabusTemplateByIdCommandHandler : IRequestHandler<GetSyllabusTemplateByIdCommand, ApiResponse<SyllabusTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSyllabusTemplateByIdCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<SyllabusTemplateDto>> Handle(GetSyllabusTemplateByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var template = await _unitOfWork.SyllabusTemplateRepository.GetSyllabusTemplateById(request.Id);
                if (template == null)
                    return ApiResponse<SyllabusTemplateDto>.ErrorResponse($"Template with ID {request.Id} not found");

                return ApiResponse<SyllabusTemplateDto>.SuccessResponse(template, "Syllabus template retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<SyllabusTemplateDto>.ErrorResponse($"Error fetching syllabus template: {ex.Message}");
            }
        }
    }

    public class CreateSyllabusTemplateCommandHandler : IRequestHandler<CreateSyllabusTemplateCommand, ApiResponse<Guid>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSyllabusTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateSyllabusTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var id = await _unitOfWork.SyllabusTemplateRepository.InsertSyllabusTemplate(request.Template);
                if (id != Guid.Empty)
                    _unitOfWork.Commit();

                return ApiResponse<Guid>.SuccessResponse(id, "Syllabus template created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.ErrorResponse($"Error creating syllabus template: {ex.Message}");
            }
        }
    }

    public class UpdateSyllabusTemplateCommandHandler : IRequestHandler<UpdateSyllabusTemplateCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSyllabusTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateSyllabusTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.SyllabusTemplateRepository.UpdateSyllabusTemplate(request.Template);
                if (success)
                    _unitOfWork.Commit();

                return ApiResponse<bool>.SuccessResponse(success, "Syllabus template updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error updating syllabus template: {ex.Message}");
            }
        }
    }

    public class DeleteOrRestoreSyllabusTemplateCommandHandler : IRequestHandler<DeleteOrRestoreSyllabusTemplateCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrRestoreSyllabusTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteOrRestoreSyllabusTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.SyllabusTemplateRepository
                    .DeleteOrRestoreSyllabusTemplate(request.Id, request.ModifiedBy, request.isRestore);

                if (result)
                    _unitOfWork.Commit();

                var message = request.isRestore ? "Syllabus template restored successfully" : "Syllabus template deleted successfully";
                return ApiResponse<bool>.SuccessResponse(result, message);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Operation failed: {ex.Message}");
            }
        }
    }
}

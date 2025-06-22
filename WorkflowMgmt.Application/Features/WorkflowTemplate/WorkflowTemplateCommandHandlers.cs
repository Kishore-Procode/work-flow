using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.WorkflowTemplate
{
    public class GetAllWorkflowTemplatesQueryHandler : IRequestHandler<GetAllWorkflowTemplatesQuery, ApiResponse<List<WorkflowTemplateWithStagesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllWorkflowTemplatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowTemplateWithStagesDto>>> Handle(GetAllWorkflowTemplatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var templates = await _unitOfWork.WorkflowTemplateRepository.GetAllAsync();
                return ApiResponse<List<WorkflowTemplateWithStagesDto>>.SuccessResponse(templates, "Workflow templates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowTemplateWithStagesDto>>.ErrorResponse($"Error retrieving workflow templates: {ex.Message}");
            }
        }
    }

    public class GetWorkflowTemplatesByDocumentTypeQueryHandler : IRequestHandler<GetWorkflowTemplatesByDocumentTypeQuery, ApiResponse<List<WorkflowTemplateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowTemplatesByDocumentTypeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowTemplateDto>>> Handle(GetWorkflowTemplatesByDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var templates = await _unitOfWork.WorkflowTemplateRepository.GetByDocumentTypeAsync(request.DocumentType);
                return ApiResponse<List<WorkflowTemplateDto>>.SuccessResponse(templates, "Workflow templates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowTemplateDto>>.ErrorResponse($"Error retrieving workflow templates: {ex.Message}");
            }
        }
    }

    public class GetActiveWorkflowTemplatesQueryHandler : IRequestHandler<GetActiveWorkflowTemplatesQuery, ApiResponse<List<WorkflowTemplateDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveWorkflowTemplatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<WorkflowTemplateDto>>> Handle(GetActiveWorkflowTemplatesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var templates = await _unitOfWork.WorkflowTemplateRepository.GetActiveAsync();
                return ApiResponse<List<WorkflowTemplateDto>>.SuccessResponse(templates, "Active workflow templates retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<WorkflowTemplateDto>>.ErrorResponse($"Error retrieving active workflow templates: {ex.Message}");
            }
        }
    }

    public class GetWorkflowTemplateByIdQueryHandler : IRequestHandler<GetWorkflowTemplateByIdQuery, ApiResponse<WorkflowTemplateWithStagesDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowTemplateByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<WorkflowTemplateWithStagesDto?>> Handle(GetWorkflowTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var template = await _unitOfWork.WorkflowTemplateRepository.GetByIdAsync(request.Id);
                if (template == null)
                {
                    return ApiResponse<WorkflowTemplateWithStagesDto?>.ErrorResponse("Workflow template not found");
                }
                return ApiResponse<WorkflowTemplateWithStagesDto?>.SuccessResponse(template, "Workflow template retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<WorkflowTemplateWithStagesDto?>.ErrorResponse($"Error retrieving workflow template: {ex.Message}");
            }
        }
    }

    public class CreateWorkflowTemplateCommandHandler : IRequestHandler<CreateWorkflowTemplateCommand, ApiResponse<WorkflowTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<WorkflowTemplateDto>> Handle(CreateWorkflowTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if template with same name and document type already exists
                var exists = await _unitOfWork.WorkflowTemplateRepository.ExistsByNameAndDocumentTypeAsync(request.Name, request.DocumentType);
                if (exists)
                {
                    return ApiResponse<WorkflowTemplateDto>.ErrorResponse($"A workflow template with name '{request.Name}' for document type '{request.DocumentType}' already exists.");
                }

                var createDto = new CreateWorkflowTemplateDto
                {
                    Name = request.Name,
                    Description = request.Description,
                    DocumentType = request.DocumentType,
                    Stages = request.Stages
                };

                var templateId = await _unitOfWork.WorkflowTemplateRepository.CreateAsync(createDto);

                // Create stages if provided
                if (request.Stages?.Count > 0)
                {
                    foreach (var stage in request.Stages)
                    {
                        var stageId = await _unitOfWork.WorkflowStageRepository.CreateAsync(templateId, stage);

                        // Create stage roles if provided
                        if (stage.RequiredRoles?.Count > 0)
                        {
                            foreach (var role in stage.RequiredRoles)
                            {
                                await _unitOfWork.WorkflowStageRoleRepository.CreateAsync(stageId, role);
                            }
                        }

                        // Create stage actions if provided
                        if (stage.Actions?.Count > 0)
                        {
                            foreach (var action in stage.Actions)
                            {
                                await _unitOfWork.WorkflowStageActionRepository.CreateAsync(stageId, action);
                            }
                        }
                    }
                }
                var result = await _unitOfWork.WorkflowTemplateRepository.GetByIdSimpleAsync(templateId);
                _unitOfWork.Commit();
                return ApiResponse<WorkflowTemplateDto>.SuccessResponse(result!, "Workflow template created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<WorkflowTemplateDto>.ErrorResponse($"Failed to create workflow template: {ex.Message}");
            }
        }
    }

    public class UpdateWorkflowTemplateCommandHandler : IRequestHandler<UpdateWorkflowTemplateCommand, ApiResponse<WorkflowTemplateDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<WorkflowTemplateDto?>> Handle(UpdateWorkflowTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if template exists
                var exists = await _unitOfWork.WorkflowTemplateRepository.ExistsAsync(request.Id);
                if (!exists)
                {
                    return ApiResponse<WorkflowTemplateDto?>.ErrorResponse("Workflow template not found");
                }

                // Check if another template with same name and document type already exists
                var nameExists = await _unitOfWork.WorkflowTemplateRepository.ExistsByNameAndDocumentTypeAsync(request.Name, request.DocumentType, request.Id);
                if (nameExists)
                {
                    return ApiResponse<WorkflowTemplateDto?>.ErrorResponse($"A workflow template with name '{request.Name}' for document type '{request.DocumentType}' already exists.");
                }

                var updateDto = new UpdateWorkflowTemplateDto
                {
                    Name = request.Name,
                    Description = request.Description,
                    DocumentType = request.DocumentType,
                    Stages = request.Stages
                };

                var success = await _unitOfWork.WorkflowTemplateRepository.UpdateAsync(request.Id, updateDto);
                if (!success)
                {
                    return ApiResponse<WorkflowTemplateDto?>.ErrorResponse("Failed to update workflow template");
                }

                // Update stages if provided
                if (request.Stages?.Count > 0)
                {
                    // First, deactivate existing stages
                    await _unitOfWork.WorkflowStageRepository.DeactivateStagesByTemplateIdAsync(request.Id);

                    // Create new stages
                    foreach (var stage in request.Stages)
                    {
                        var stageId = await _unitOfWork.WorkflowStageRepository.CreateAsync(request.Id, stage);

                        // Create stage roles if provided
                        if (stage.RequiredRoles?.Count > 0)
                        {
                            foreach (var role in stage.RequiredRoles)
                            {
                                await _unitOfWork.WorkflowStageRoleRepository.CreateAsync(stageId, role);
                            }
                        }

                        // Create stage actions if provided
                        if (stage.Actions?.Count > 0)
                        {
                            foreach (var action in stage.Actions)
                            {
                                await _unitOfWork.WorkflowStageActionRepository.CreateAsync(stageId, action);
                            }
                        }
                    }
                }

                var result = await _unitOfWork.WorkflowTemplateRepository.GetByIdSimpleAsync(request.Id);
                _unitOfWork.Commit();
                return ApiResponse<WorkflowTemplateDto?>.SuccessResponse(result, "Workflow template updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<WorkflowTemplateDto?>.ErrorResponse($"Failed to update workflow template: {ex.Message}");
            }
        }
    }

    public class DeleteWorkflowTemplateCommandHandler : IRequestHandler<DeleteWorkflowTemplateCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteWorkflowTemplateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.WorkflowTemplateRepository.DeleteAsync(request.Id);
                if (success)
                {
                    _unitOfWork.Commit();
                }
                return ApiResponse<bool>.SuccessResponse(success, "Workflow template deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to delete workflow template: {ex.Message}");
            }
        }
    }

    public class ToggleWorkflowTemplateActiveCommandHandler : IRequestHandler<ToggleWorkflowTemplateActiveCommand, ApiResponse<WorkflowTemplateDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleWorkflowTemplateActiveCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<WorkflowTemplateDto?>> Handle(ToggleWorkflowTemplateActiveCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var success = await _unitOfWork.WorkflowTemplateRepository.ToggleActiveAsync(request.Id);
                if (!success)
                {
                    return ApiResponse<WorkflowTemplateDto?>.ErrorResponse("Workflow template not found");
                }

                var result = await _unitOfWork.WorkflowTemplateRepository.GetByIdSimpleAsync(request.Id);
                _unitOfWork.Commit();

                return ApiResponse<WorkflowTemplateDto?>.SuccessResponse(result, "Workflow template status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<WorkflowTemplateDto?>.ErrorResponse($"Failed to toggle workflow template status: {ex.Message}");
            }
        }
    }
}

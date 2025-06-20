using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowTemplate
{
    public class GetAllWorkflowTemplatesQueryHandler : IRequestHandler<GetAllWorkflowTemplatesQuery, IEnumerable<WorkflowTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllWorkflowTemplatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowTemplateDto>> Handle(GetAllWorkflowTemplatesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowTemplateRepository.GetAllAsync();
        }
    }

    public class GetWorkflowTemplatesByDocumentTypeQueryHandler : IRequestHandler<GetWorkflowTemplatesByDocumentTypeQuery, IEnumerable<WorkflowTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowTemplatesByDocumentTypeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowTemplateDto>> Handle(GetWorkflowTemplatesByDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowTemplateRepository.GetByDocumentTypeAsync(request.DocumentType);
        }
    }

    public class GetActiveWorkflowTemplatesQueryHandler : IRequestHandler<GetActiveWorkflowTemplatesQuery, IEnumerable<WorkflowTemplateDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetActiveWorkflowTemplatesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowTemplateDto>> Handle(GetActiveWorkflowTemplatesQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowTemplateRepository.GetActiveAsync();
        }
    }

    public class GetWorkflowTemplateByIdQueryHandler : IRequestHandler<GetWorkflowTemplateByIdQuery, WorkflowTemplateWithStagesDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowTemplateByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowTemplateWithStagesDto?> Handle(GetWorkflowTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowTemplateRepository.GetByIdAsync(request.Id);
        }
    }

    public class CreateWorkflowTemplateCommandHandler : IRequestHandler<CreateWorkflowTemplateCommand, WorkflowTemplateDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowTemplateDto> Handle(CreateWorkflowTemplateCommand request, CancellationToken cancellationToken)
        {
            // Check if template with same name and document type already exists
            var exists = await _unitOfWork.WorkflowTemplateRepository.ExistsByNameAndDocumentTypeAsync(request.Name, request.DocumentType);
            if (exists)
            {
                throw new InvalidOperationException($"A workflow template with name '{request.Name}' for document type '{request.DocumentType}' already exists.");
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

            _unitOfWork.Commit();

            var result = await _unitOfWork.WorkflowTemplateRepository.GetByIdSimpleAsync(templateId);
            return result ?? throw new InvalidOperationException("Failed to retrieve created workflow template.");
        }
    }

    public class UpdateWorkflowTemplateCommandHandler : IRequestHandler<UpdateWorkflowTemplateCommand, WorkflowTemplateDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowTemplateDto?> Handle(UpdateWorkflowTemplateCommand request, CancellationToken cancellationToken)
        {
            // Check if template exists
            var exists = await _unitOfWork.WorkflowTemplateRepository.ExistsAsync(request.Id);
            if (!exists)
            {
                return null;
            }

            // Check if another template with same name and document type already exists
            var nameExists = await _unitOfWork.WorkflowTemplateRepository.ExistsByNameAndDocumentTypeAsync(request.Name, request.DocumentType, request.Id);
            if (nameExists)
            {
                throw new InvalidOperationException($"A workflow template with name '{request.Name}' for document type '{request.DocumentType}' already exists.");
            }

            var updateDto = new UpdateWorkflowTemplateDto
            {
                Name = request.Name,
                Description = request.Description,
                DocumentType = request.DocumentType
            };

            var success = await _unitOfWork.WorkflowTemplateRepository.UpdateAsync(request.Id, updateDto);
            if (!success)
            {
                return null;
            }

            _unitOfWork.Commit();

            return await _unitOfWork.WorkflowTemplateRepository.GetByIdSimpleAsync(request.Id);
        }
    }

    public class DeleteWorkflowTemplateCommandHandler : IRequestHandler<DeleteWorkflowTemplateCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWorkflowTemplateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteWorkflowTemplateCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.WorkflowTemplateRepository.DeleteAsync(request.Id);
            if (success)
            {
                _unitOfWork.Commit();
            }
            return success;
        }
    }

    public class ToggleWorkflowTemplateActiveCommandHandler : IRequestHandler<ToggleWorkflowTemplateActiveCommand, WorkflowTemplateDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ToggleWorkflowTemplateActiveCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowTemplateDto?> Handle(ToggleWorkflowTemplateActiveCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.WorkflowTemplateRepository.ToggleActiveAsync(request.Id);
            if (!success)
            {
                return null;
            }

            _unitOfWork.Commit();

            return await _unitOfWork.WorkflowTemplateRepository.GetByIdSimpleAsync(request.Id);
        }
    }
}

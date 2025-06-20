using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentWorkflow
{
    public class GetAllDocumentWorkflowsQueryHandler : IRequestHandler<GetAllDocumentWorkflowsQuery, IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllDocumentWorkflowsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> Handle(GetAllDocumentWorkflowsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DocumentWorkflowRepository.GetAllAsync();
        }
    }

    public class GetDocumentWorkflowsByDocumentTypeQueryHandler : IRequestHandler<GetDocumentWorkflowsByDocumentTypeQuery, IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentWorkflowsByDocumentTypeQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> Handle(GetDocumentWorkflowsByDocumentTypeQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DocumentWorkflowRepository.GetByDocumentTypeAsync(request.DocumentType);
        }
    }

    public class GetDocumentWorkflowsByStatusQueryHandler : IRequestHandler<GetDocumentWorkflowsByStatusQuery, IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentWorkflowsByStatusQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> Handle(GetDocumentWorkflowsByStatusQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DocumentWorkflowRepository.GetByStatusAsync(request.Status);
        }
    }

    public class GetDocumentWorkflowsByInitiatedByQueryHandler : IRequestHandler<GetDocumentWorkflowsByInitiatedByQuery, IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentWorkflowsByInitiatedByQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> Handle(GetDocumentWorkflowsByInitiatedByQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DocumentWorkflowRepository.GetByInitiatedByAsync(request.UserId);
        }
    }

    public class GetDocumentWorkflowsByRoleQueryHandler : IRequestHandler<GetDocumentWorkflowsByRoleQuery, IEnumerable<DocumentWorkflowWithDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentWorkflowsByRoleQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DocumentWorkflowWithDetailsDto>> Handle(GetDocumentWorkflowsByRoleQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DocumentWorkflowRepository.GetByRoleAsync(request.Role);
        }
    }

    public class GetDocumentWorkflowByIdQueryHandler : IRequestHandler<GetDocumentWorkflowByIdQuery, DocumentWorkflowWithDetailsDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentWorkflowByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentWorkflowWithDetailsDto?> Handle(GetDocumentWorkflowByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
        }
    }

    public class GetDocumentWorkflowByDocumentIdQueryHandler : IRequestHandler<GetDocumentWorkflowByDocumentIdQuery, DocumentWorkflowWithDetailsDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentWorkflowByDocumentIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentWorkflowWithDetailsDto?> Handle(GetDocumentWorkflowByDocumentIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(request.DocumentId);
        }
    }

    public class GetWorkflowStatsQueryHandler : IRequestHandler<GetWorkflowStatsQuery, WorkflowStatsDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStatsDto> Handle(GetWorkflowStatsQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.DocumentWorkflowRepository.GetWorkflowStatsAsync();
        }
    }

    public class CreateDocumentWorkflowCommandHandler : IRequestHandler<CreateDocumentWorkflowCommand, DocumentWorkflowDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDocumentWorkflowCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentWorkflowDto> Handle(CreateDocumentWorkflowCommand request, CancellationToken cancellationToken)
        {
            // Check if workflow template exists and is active
            var template = await _unitOfWork.WorkflowTemplateRepository.GetByIdSimpleAsync(request.WorkflowTemplateId);
            if (template == null || !template.IsActive)
            {
                throw new InvalidOperationException("Workflow template not found or is not active.");
            }

            // Check if document already has an active workflow
            var existingWorkflow = await _unitOfWork.DocumentWorkflowRepository.ExistsByDocumentIdAsync(request.DocumentId);
            if (existingWorkflow)
            {
                throw new InvalidOperationException("Document already has an active workflow.");
            }

            var createDto = new CreateDocumentWorkflowDto
            {
                DocumentId = request.DocumentId,
                DocumentType = request.DocumentType,
                WorkflowTemplateId = request.WorkflowTemplateId,
                InitiatedBy = request.InitiatedBy
            };

            var workflowId = await _unitOfWork.DocumentWorkflowRepository.CreateAsync(createDto);

            await _unitOfWork.SaveAsync();

            var result = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(workflowId);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to retrieve created document workflow.");
            }

            return new DocumentWorkflowDto
            {
                Id = result.Id,
                DocumentId = result.DocumentId,
                DocumentType = result.DocumentType,
                WorkflowTemplateId = result.WorkflowTemplateId,
                CurrentStageId = result.CurrentStageId,
                Status = result.Status,
                InitiatedBy = result.InitiatedBy,
                InitiatedDate = result.InitiatedDate,
                CompletedDate = result.CompletedDate,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                ModifiedDate = result.ModifiedDate
            };
        }
    }

    public class UpdateDocumentWorkflowCommandHandler : IRequestHandler<UpdateDocumentWorkflowCommand, DocumentWorkflowDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDocumentWorkflowCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentWorkflowDto?> Handle(UpdateDocumentWorkflowCommand request, CancellationToken cancellationToken)
        {
            // Check if workflow exists
            var workflow = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
            if (workflow == null)
            {
                return null;
            }

            // If CurrentStageId is provided, validate it exists
            if (request.CurrentStageId.HasValue)
            {
                var stageExists = await _unitOfWork.WorkflowStageRepository.ExistsAsync(request.CurrentStageId.Value);
                if (!stageExists)
                {
                    throw new InvalidOperationException("Stage not found.");
                }
            }

            var updateDto = new UpdateDocumentWorkflowDto
            {
                CurrentStageId = request.CurrentStageId,
                Status = request.Status,
                CompletedDate = request.CompletedDate
            };

            var success = await _unitOfWork.DocumentWorkflowRepository.UpdateAsync(request.Id, updateDto);
            if (!success)
            {
                return null;
            }

            await _unitOfWork.SaveAsync();

            var result = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
            if (result == null)
            {
                return null;
            }

            return new DocumentWorkflowDto
            {
                Id = result.Id,
                DocumentId = result.DocumentId,
                DocumentType = result.DocumentType,
                WorkflowTemplateId = result.WorkflowTemplateId,
                CurrentStageId = result.CurrentStageId,
                Status = result.Status,
                InitiatedBy = result.InitiatedBy,
                InitiatedDate = result.InitiatedDate,
                CompletedDate = result.CompletedDate,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                ModifiedDate = result.ModifiedDate
            };
        }
    }

    public class AdvanceDocumentWorkflowCommandHandler : IRequestHandler<AdvanceDocumentWorkflowCommand, DocumentWorkflowDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdvanceDocumentWorkflowCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentWorkflowDto?> Handle(AdvanceDocumentWorkflowCommand request, CancellationToken cancellationToken)
        {
            // Check if workflow exists and is active
            var workflow = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
            if (workflow == null || !workflow.IsActive || workflow.Status != "In Progress")
            {
                return null;
            }

            if (!workflow.CurrentStageId.HasValue)
            {
                throw new InvalidOperationException("Workflow has no current stage.");
            }

            // Get the next stage based on the action type
            var nextStage = await _unitOfWork.WorkflowStageRepository.GetNextStageAsync(workflow.CurrentStageId.Value, request.ActionType);

            // Advance the workflow
            var success = await _unitOfWork.DocumentWorkflowRepository.AdvanceStageAsync(request.Id, nextStage?.Id, request.Comments);
            if (!success)
            {
                return null;
            }

            await _unitOfWork.SaveAsync();

            var result = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
            if (result == null)
            {
                return null;
            }

            return new DocumentWorkflowDto
            {
                Id = result.Id,
                DocumentId = result.DocumentId,
                DocumentType = result.DocumentType,
                WorkflowTemplateId = result.WorkflowTemplateId,
                CurrentStageId = result.CurrentStageId,
                Status = result.Status,
                InitiatedBy = result.InitiatedBy,
                InitiatedDate = result.InitiatedDate,
                CompletedDate = result.CompletedDate,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                ModifiedDate = result.ModifiedDate
            };
        }
    }

    public class CompleteDocumentWorkflowCommandHandler : IRequestHandler<CompleteDocumentWorkflowCommand, DocumentWorkflowDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompleteDocumentWorkflowCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentWorkflowDto?> Handle(CompleteDocumentWorkflowCommand request, CancellationToken cancellationToken)
        {
            // Check if workflow exists and is active
            var workflow = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
            if (workflow == null || !workflow.IsActive)
            {
                return null;
            }

            var success = await _unitOfWork.DocumentWorkflowRepository.CompleteWorkflowAsync(request.Id);
            if (!success)
            {
                return null;
            }

            await _unitOfWork.SaveAsync();

            var result = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
            if (result == null)
            {
                return null;
            }

            return new DocumentWorkflowDto
            {
                Id = result.Id,
                DocumentId = result.DocumentId,
                DocumentType = result.DocumentType,
                WorkflowTemplateId = result.WorkflowTemplateId,
                CurrentStageId = result.CurrentStageId,
                Status = result.Status,
                InitiatedBy = result.InitiatedBy,
                InitiatedDate = result.InitiatedDate,
                CompletedDate = result.CompletedDate,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                ModifiedDate = result.ModifiedDate
            };
        }
    }

    public class CancelDocumentWorkflowCommandHandler : IRequestHandler<CancelDocumentWorkflowCommand, DocumentWorkflowDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelDocumentWorkflowCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentWorkflowDto?> Handle(CancelDocumentWorkflowCommand request, CancellationToken cancellationToken)
        {
            // Check if workflow exists and is active
            var workflow = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
            if (workflow == null || !workflow.IsActive)
            {
                return null;
            }

            var success = await _unitOfWork.DocumentWorkflowRepository.CancelWorkflowAsync(request.Id, request.Reason);
            if (!success)
            {
                return null;
            }

            await _unitOfWork.SaveAsync();

            var result = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.Id);
            if (result == null)
            {
                return null;
            }

            return new DocumentWorkflowDto
            {
                Id = result.Id,
                DocumentId = result.DocumentId,
                DocumentType = result.DocumentType,
                WorkflowTemplateId = result.WorkflowTemplateId,
                CurrentStageId = result.CurrentStageId,
                Status = result.Status,
                InitiatedBy = result.InitiatedBy,
                InitiatedDate = result.InitiatedDate,
                CompletedDate = result.CompletedDate,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                ModifiedDate = result.ModifiedDate
            };
        }
    }
}

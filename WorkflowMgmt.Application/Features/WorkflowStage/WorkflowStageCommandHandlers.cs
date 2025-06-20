using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowStage
{
    public class GetWorkflowStagesByTemplateIdQueryHandler : IRequestHandler<GetWorkflowStagesByTemplateIdQuery, IEnumerable<WorkflowStageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStagesByTemplateIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowStageDto>> Handle(GetWorkflowStagesByTemplateIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageRepository.GetByTemplateIdAsync(request.TemplateId);
        }
    }

    public class GetWorkflowStageByIdQueryHandler : IRequestHandler<GetWorkflowStageByIdQuery, WorkflowStageDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStageDto?> Handle(GetWorkflowStageByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageRepository.GetByIdAsync(request.Id);
        }
    }

    public class GetWorkflowStagesByRoleQueryHandler : IRequestHandler<GetWorkflowStagesByRoleQuery, IEnumerable<WorkflowStageDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStagesByRoleQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowStageDto>> Handle(GetWorkflowStagesByRoleQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageRepository.GetByRoleAsync(request.Role);
        }
    }

    public class CreateWorkflowStageCommandHandler : IRequestHandler<CreateWorkflowStageCommand, WorkflowStageDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkflowStageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStageDto> Handle(CreateWorkflowStageCommand request, CancellationToken cancellationToken)
        {
            // Check if template exists
            var templateExists = await _unitOfWork.WorkflowTemplateRepository.ExistsAsync(request.TemplateId);
            if (!templateExists)
            {
                throw new InvalidOperationException("Workflow template not found.");
            }

            // Check if stage order already exists in template
            var orderExists = await _unitOfWork.WorkflowStageRepository.ExistsInTemplateAsync(request.TemplateId, request.StageOrder);
            if (orderExists)
            {
                throw new InvalidOperationException($"A stage with order {request.StageOrder} already exists in this template.");
            }

            var createDto = new CreateWorkflowStageDto
            {
                StageName = request.StageName,
                StageOrder = request.StageOrder,
                AssignedRole = request.AssignedRole,
                Description = request.Description,
                IsRequired = request.IsRequired,
                AutoApprove = request.AutoApprove,
                TimeoutDays = request.TimeoutDays,
                Actions = request.Actions
            };

            var stageId = await _unitOfWork.WorkflowStageRepository.CreateAsync(request.TemplateId, createDto);

            // Create stage actions if provided
            if (request.Actions?.Count > 0)
            {
                foreach (var action in request.Actions)
                {
                    await _unitOfWork.WorkflowStageActionRepository.CreateAsync(stageId, action);
                }
            }

            _unitOfWork.Commit();

            var result = await _unitOfWork.WorkflowStageRepository.GetByIdAsync(stageId);
            return result ?? throw new InvalidOperationException("Failed to retrieve created workflow stage.");
        }
    }

    public class UpdateWorkflowStageCommandHandler : IRequestHandler<UpdateWorkflowStageCommand, WorkflowStageDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkflowStageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStageDto?> Handle(UpdateWorkflowStageCommand request, CancellationToken cancellationToken)
        {
            // Check if stage exists
            var stage = await _unitOfWork.WorkflowStageRepository.GetByIdAsync(request.Id);
            if (stage == null)
            {
                return null;
            }

            // Check if stage order already exists in template (excluding current stage)
            var orderExists = await _unitOfWork.WorkflowStageRepository.ExistsInTemplateAsync(stage.WorkflowTemplateId, request.StageOrder, request.Id);
            if (orderExists)
            {
                throw new InvalidOperationException($"A stage with order {request.StageOrder} already exists in this template.");
            }

            var updateDto = new UpdateWorkflowStageDto
            {
                StageName = request.StageName,
                StageOrder = request.StageOrder,
                AssignedRole = request.AssignedRole,
                Description = request.Description,
                IsRequired = request.IsRequired,
                AutoApprove = request.AutoApprove,
                TimeoutDays = request.TimeoutDays
            };

            var success = await _unitOfWork.WorkflowStageRepository.UpdateAsync(request.Id, updateDto);
            if (!success)
            {
                return null;
            }

            _unitOfWork.Commit();

            return await _unitOfWork.WorkflowStageRepository.GetByIdAsync(request.Id);
        }
    }

    public class DeleteWorkflowStageCommandHandler : IRequestHandler<DeleteWorkflowStageCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWorkflowStageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteWorkflowStageCommand request, CancellationToken cancellationToken)
        {
            // First delete all stage actions
            await _unitOfWork.WorkflowStageActionRepository.DeleteByStageIdAsync(request.Id);

            // Then delete the stage
            var success = await _unitOfWork.WorkflowStageRepository.DeleteAsync(request.Id);
            if (success)
            {
                _unitOfWork.Commit();
            }
            return success;
        }
    }

    public class ReorderWorkflowStagesCommandHandler : IRequestHandler<ReorderWorkflowStagesCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReorderWorkflowStagesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ReorderWorkflowStagesCommand request, CancellationToken cancellationToken)
        {
            // Check if template exists
            var templateExists = await _unitOfWork.WorkflowTemplateRepository.ExistsAsync(request.TemplateId);
            if (!templateExists)
            {
                throw new InvalidOperationException("Workflow template not found.");
            }

            // Validate that all stages belong to the template
            var stages = await _unitOfWork.WorkflowStageRepository.GetByTemplateIdAsync(request.TemplateId);
            var stageIds = stages.Select(s => s.Id).ToHashSet();

            foreach (var stageOrder in request.StageOrders)
            {
                if (!stageIds.Contains(stageOrder.StageId))
                {
                    throw new InvalidOperationException($"Stage {stageOrder.StageId} does not belong to template {request.TemplateId}.");
                }
            }

            var stageOrderTuples = request.StageOrders.Select(so => (so.StageId, so.NewOrder)).ToList();
            var success = await _unitOfWork.WorkflowStageRepository.ReorderStagesAsync(request.TemplateId, stageOrderTuples);

            if (success)
            {
                _unitOfWork.Commit();
            }

            return success;
        }
    }
}

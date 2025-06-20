using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowStageAction
{
    public class GetWorkflowStageActionsByStageIdQueryHandler : IRequestHandler<GetWorkflowStageActionsByStageIdQuery, IEnumerable<WorkflowStageActionDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageActionsByStageIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WorkflowStageActionDto>> Handle(GetWorkflowStageActionsByStageIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageActionRepository.GetByStageIdAsync(request.StageId);
        }
    }

    public class GetWorkflowStageActionByIdQueryHandler : IRequestHandler<GetWorkflowStageActionByIdQuery, WorkflowStageActionDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowStageActionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStageActionDto?> Handle(GetWorkflowStageActionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(request.Id);
        }
    }

    public class CreateWorkflowStageActionCommandHandler : IRequestHandler<CreateWorkflowStageActionCommand, WorkflowStageActionDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateWorkflowStageActionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStageActionDto> Handle(CreateWorkflowStageActionCommand request, CancellationToken cancellationToken)
        {
            // Check if stage exists
            var stageExists = await _unitOfWork.WorkflowStageRepository.ExistsAsync(request.StageId);
            if (!stageExists)
            {
                throw new InvalidOperationException("Workflow stage not found.");
            }

            // Check if action with same type already exists for this stage
            var actionExists = await _unitOfWork.WorkflowStageActionRepository.ExistsByStageAndActionTypeAsync(request.StageId, request.ActionType);
            if (actionExists)
            {
                throw new InvalidOperationException($"An action with type '{request.ActionType}' already exists for this stage.");
            }

            // If NextStageId is provided, validate it exists
            if (request.NextStageId.HasValue)
            {
                var nextStageExists = await _unitOfWork.WorkflowStageRepository.ExistsAsync(request.NextStageId.Value);
                if (!nextStageExists)
                {
                    throw new InvalidOperationException("Next stage not found.");
                }
            }

            var createDto = new CreateWorkflowStageActionDto
            {
                ActionName = request.ActionName,
                ActionType = request.ActionType,
                NextStageId = request.NextStageId
            };

            var actionId = await _unitOfWork.WorkflowStageActionRepository.CreateAsync(request.StageId, createDto);

            _unitOfWork.Commit();

            var result = await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(actionId);
            return result ?? throw new InvalidOperationException("Failed to retrieve created workflow stage action.");
        }
    }

    public class UpdateWorkflowStageActionCommandHandler : IRequestHandler<UpdateWorkflowStageActionCommand, WorkflowStageActionDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkflowStageActionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStageActionDto?> Handle(UpdateWorkflowStageActionCommand request, CancellationToken cancellationToken)
        {
            // Check if action exists
            var action = await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(request.Id);
            if (action == null)
            {
                return null;
            }

            // Check if another action with same type already exists for this stage (excluding current action)
            var actionExists = await _unitOfWork.WorkflowStageActionRepository.ExistsByStageAndActionTypeAsync(action.WorkflowStageId, request.ActionType, request.Id);
            if (actionExists)
            {
                throw new InvalidOperationException($"An action with type '{request.ActionType}' already exists for this stage.");
            }

            // If NextStageId is provided, validate it exists
            if (request.NextStageId.HasValue)
            {
                var nextStageExists = await _unitOfWork.WorkflowStageRepository.ExistsAsync(request.NextStageId.Value);
                if (!nextStageExists)
                {
                    throw new InvalidOperationException("Next stage not found.");
                }
            }

            var updateDto = new UpdateWorkflowStageActionDto
            {
                ActionName = request.ActionName,
                ActionType = request.ActionType,
                NextStageId = request.NextStageId
            };

            var success = await _unitOfWork.WorkflowStageActionRepository.UpdateAsync(request.Id, updateDto);
            if (!success)
            {
                return null;
            }

            _unitOfWork.Commit();

            return await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(request.Id);
        }
    }

    public class DeleteWorkflowStageActionCommandHandler : IRequestHandler<DeleteWorkflowStageActionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWorkflowStageActionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteWorkflowStageActionCommand request, CancellationToken cancellationToken)
        {
            var success = await _unitOfWork.WorkflowStageActionRepository.DeleteAsync(request.Id);
            if (success)
            {
                _unitOfWork.Commit();
            }
            return success;
        }
    }
}

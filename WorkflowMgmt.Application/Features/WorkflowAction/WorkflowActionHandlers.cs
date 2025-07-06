using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.WorkflowAction
{
    public class ProcessWorkflowActionHandler : IRequestHandler<ProcessWorkflowActionCommand, DocumentWorkflowDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProcessWorkflowActionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DocumentWorkflowDto?> Handle(ProcessWorkflowActionCommand request, CancellationToken cancellationToken)
        {
            // Get the document workflow
            var documentWorkflow = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.DocumentWorkflowId);
            if (documentWorkflow == null)
            {
                throw new InvalidOperationException("Document workflow not found.");
            }

            // Get the workflow stage action
            var action = await _unitOfWork.WorkflowStageActionRepository.GetByIdAsync(request.ActionId);
            if (action == null)
            {
                throw new InvalidOperationException("Workflow stage action not found.");
            }

            // Verify the action belongs to the current stage
            if (action.WorkflowStageId != documentWorkflow.CurrentStageId)
            {
                throw new InvalidOperationException("Action does not belong to the current workflow stage.");
            }

            // Determine the next stage
            Guid? nextStageId = null;
            if (action.NextStageId.HasValue)
            {
                // Use the specified next stage
                nextStageId = action.NextStageId.Value;
            }
            else
            {
                // If no next stage specified, get the next stage in order
                var currentStage = await _unitOfWork.WorkflowStageRepository.GetByIdAsync(documentWorkflow.CurrentStageId.Value);
                if (currentStage != null)
                {
                    var nextStage = await _unitOfWork.WorkflowStageRepository.GetNextStageAsync(
                        currentStage.WorkflowTemplateId, currentStage.StageOrder);
                    nextStageId = nextStage?.Id;
                }
            }

            // Determine who should be assigned to the next stage
            Guid? assignedTo = null;
            if (nextStageId.HasValue)
            {
                // Get users assigned to the next stage based on workflow_stage_roles
                var stageRoles = await _unitOfWork.WorkflowStageRoleRepository.GetByStageIdAsync(nextStageId.Value);
                if (stageRoles.Any())
                {
                    // For now, assign to the first user found with the required role
                    // In a real implementation, you might want more sophisticated assignment logic
                    var firstRole = stageRoles.First();
                    var usersWithRole = await _unitOfWork.WorkflowUserRepository.GetByRoleCodeAsync(firstRole.RoleCode);
                    assignedTo = usersWithRole.FirstOrDefault()?.Id;
                }
            }

            // Create workflow stage history record
            var stageHistory = new CreateWorkflowStageHistoryDto
            {
                DocumentWorkflowId = request.DocumentWorkflowId,
                StageId = documentWorkflow.CurrentStageId.Value,
                ActionTaken = action.ActionName,
                ProcessedBy = request.ProcessedBy,
                AssignedTo = assignedTo,
                Comments = request.Comments,
                Attachments = request.Attachments
            };

            await _unitOfWork.WorkflowStageHistoryRepository.CreateAsync(stageHistory);

            // Update document workflow
            var updateDto = new UpdateDocumentWorkflowDto
            {
                CurrentStageId = nextStageId,
                Status = nextStageId.HasValue ? "In Progress" : "Completed"
            };

            if (!nextStageId.HasValue)
            {
                updateDto.CompletedDate = DateTime.Now;
            }

            await _unitOfWork.DocumentWorkflowRepository.UpdateAsync(request.DocumentWorkflowId, updateDto);

            // If moving to next stage, create a new history record for the assignment
            if (nextStageId.HasValue && assignedTo.HasValue)
            {
                var assignmentHistory = new CreateWorkflowStageHistoryDto
                {
                    DocumentWorkflowId = request.DocumentWorkflowId,
                    StageId = nextStageId.Value,
                    ActionTaken = "assigned",
                    ProcessedBy = request.ProcessedBy,
                    AssignedTo = assignedTo.Value,
                    Comments = $"Automatically assigned to next stage after {action.ActionName}"
                };

                await _unitOfWork.WorkflowStageHistoryRepository.CreateAsync(assignmentHistory);
            }

            await _unitOfWork.SaveAsync();

            // Return updated workflow
            return await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.DocumentWorkflowId);
        }
    }

    public class GetWorkflowActionsHandler : IRequestHandler<GetWorkflowActionsQuery, WorkflowStageActionDto[]>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetWorkflowActionsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowStageActionDto[]> Handle(GetWorkflowActionsQuery request, CancellationToken cancellationToken)
        {
            // Get the document workflow
            var documentWorkflow = await _unitOfWork.DocumentWorkflowRepository.GetByIdAsync(request.DocumentWorkflowId);
            if (documentWorkflow?.CurrentStageId == null)
            {
                return Array.Empty<WorkflowStageActionDto>();
            }

            // Get actions for the current stage
            var actions = await _unitOfWork.WorkflowStageActionRepository.GetActiveByStageIdAsync(documentWorkflow.CurrentStageId.Value);
            return actions.ToArray();
        }
    }
}

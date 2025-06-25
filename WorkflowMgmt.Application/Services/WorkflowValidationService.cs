using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IServices;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Services
{
    public class WorkflowValidationService : IWorkflowValidationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkflowValidationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WorkflowValidationResult> ValidateUserActionAsync(Guid userId, Guid documentId, 
            string documentType, Guid actionId)
        {
            try
            {
                // Check if user can perform this action using existing repository method
                var canPerform = await _unitOfWork.DocumentLifecycleRepository.CanUserPerformActionAsync(
                    userId, documentId, documentType, actionId);

                if (!canPerform)
                {
                    return WorkflowValidationResult.Failure(
                        "User is not authorized to perform this action", "UNAUTHORIZED_ACTION");
                }

                return WorkflowValidationResult.Success();
            }
            catch (Exception ex)
            {
                return WorkflowValidationResult.Failure($"Validation error: {ex.Message}", "VALIDATION_ERROR");
            }
        }

        public async Task<WorkflowValidationResult> ValidateStageTransitionAsync(Guid currentStageId, 
            Guid? nextStageId, string actionType)
        {
            try
            {
                // Get current stage details
                var currentStage = await _unitOfWork.WorkflowStageRepository.GetByIdAsync(currentStageId);
                if (currentStage == null)
                {
                    return WorkflowValidationResult.Failure("Current stage not found", "STAGE_NOT_FOUND");
                }

                // If no next stage, this is a terminal action (approve/reject)
                if (!nextStageId.HasValue)
                {
                    if (actionType.ToLower() == "approve" || actionType.ToLower() == "reject")
                    {
                        return WorkflowValidationResult.Success();
                    }
                    return WorkflowValidationResult.Failure("Invalid terminal action", "INVALID_TERMINAL_ACTION");
                }

                // Validate next stage exists and belongs to same template
                var nextStage = await _unitOfWork.WorkflowStageRepository.GetByIdAsync(nextStageId.Value);
                if (nextStage == null)
                {
                    return WorkflowValidationResult.Failure("Next stage not found", "NEXT_STAGE_NOT_FOUND");
                }

                // Check if stages belong to same workflow template
                if (currentStage.WorkflowTemplateId != nextStage.WorkflowTemplateId)
                {
                    return WorkflowValidationResult.Failure(
                        "Stages belong to different workflow templates", "TEMPLATE_MISMATCH");
                }

                // Validate stage order progression (optional business rule)
                if (nextStage.StageOrder <= currentStage.StageOrder && actionType.ToLower() != "escalate")
                {
                    return WorkflowValidationResult.Warning(
                        "Moving to a previous stage - this may require special permissions");
                }

                return WorkflowValidationResult.Success();
            }
            catch (Exception ex)
            {
                return WorkflowValidationResult.Failure($"Stage transition validation error: {ex.Message}", 
                    "TRANSITION_VALIDATION_ERROR");
            }
        }

        public async Task<WorkflowValidationResult> ValidateDocumentStateAsync(Guid documentId, string documentType)
        {
            try
            {
                // Get document workflow
                var workflow = await _unitOfWork.DocumentWorkflowRepository.GetByDocumentIdAsync(documentId.ToString());
                if (workflow == null)
                {
                    return WorkflowValidationResult.Failure("Document workflow not found", "WORKFLOW_NOT_FOUND");
                }

                // Check if workflow is in a valid state for actions
                if (workflow.Status == "Completed")
                {
                    return WorkflowValidationResult.Failure("Cannot perform actions on completed workflow", 
                        "WORKFLOW_COMPLETED");
                }

                if (workflow.Status == "Cancelled")
                {
                    return WorkflowValidationResult.Failure("Cannot perform actions on cancelled workflow", 
                        "WORKFLOW_CANCELLED");
                }

                // Validate document exists and is active
                if (documentType.ToLower() == "syllabus")
                {
                    // Add syllabus-specific validation if needed
                    // For now, assume document exists if workflow exists
                }

                return WorkflowValidationResult.Success();
            }
            catch (Exception ex)
            {
                return WorkflowValidationResult.Failure($"Document state validation error: {ex.Message}", 
                    "DOCUMENT_STATE_ERROR");
            }
        }

        public async Task<WorkflowValidationResult> ValidateRequiredApprovalsAsync(Guid stageId, Guid documentId)
        {
            try
            {
                // Get stage details
                var stage = await _unitOfWork.WorkflowStageRepository.GetByIdAsync(stageId);
                if (stage == null)
                {
                    return WorkflowValidationResult.Failure("Stage not found", "STAGE_NOT_FOUND");
                }

                // Check if stage requires approvals
                if (!stage.IsRequired)
                {
                    return WorkflowValidationResult.Success();
                }

                // Get required roles for this stage
                var requiredRoles = await _unitOfWork.WorkflowStageRoleRepository.GetByStageIdAsync(stageId);
                if (requiredRoles.Count == 0)
                {
                    return WorkflowValidationResult.Warning("No required roles defined for this stage");
                }

                // For now, assume approvals are met if we reach this point
                // In a full implementation, you'd check approval history
                return WorkflowValidationResult.Success();
            }
            catch (Exception ex)
            {
                return WorkflowValidationResult.Failure($"Required approvals validation error: {ex.Message}", 
                    "APPROVALS_VALIDATION_ERROR");
            }
        }

        public async Task<WorkflowValidationResult> ValidateUserRoleAsync(Guid userId, Guid stageId, Guid documentId)
        {
            try
            {
                // This validation is already handled by CanUserPerformActionAsync
                // But we can add additional role-specific validations here
                return WorkflowValidationResult.Success();
            }
            catch (Exception ex)
            {
                return WorkflowValidationResult.Failure($"User role validation error: {ex.Message}", 
                    "ROLE_VALIDATION_ERROR");
            }
        }

        public async Task<WorkflowValidationResult> ValidateWorkflowTemplateAsync(Guid templateId)
        {
            try
            {
                var template = await _unitOfWork.WorkflowTemplateRepository.GetByIdAsync(templateId);
                if (template == null)
                {
                    return WorkflowValidationResult.Failure("Workflow template not found", "TEMPLATE_NOT_FOUND");
                }

                if (!template.IsActive)
                {
                    return WorkflowValidationResult.Failure("Workflow template is inactive", "TEMPLATE_INACTIVE");
                }

                // Validate template has stages
                if (template.Stages.Count == 0)
                {
                    return WorkflowValidationResult.Failure("Workflow template has no stages", "NO_STAGES");
                }

                // Validate stage order sequence
                var expectedOrder = 1;
                foreach (var stage in template.Stages.OrderBy(s => s.StageOrder))
                {
                    if (stage.StageOrder != expectedOrder)
                    {
                        return WorkflowValidationResult.Failure(
                            $"Invalid stage order sequence at stage {stage.StageName}", "INVALID_STAGE_ORDER");
                    }
                    expectedOrder++;
                }

                return WorkflowValidationResult.Success();
            }
            catch (Exception ex)
            {
                return WorkflowValidationResult.Failure($"Template validation error: {ex.Message}", 
                    "TEMPLATE_VALIDATION_ERROR");
            }
        }

        public async Task<List<WorkflowValidationRule>> GetValidationRulesAsync(string documentType)
        {
            // Return predefined validation rules for document types
            var rules = new List<WorkflowValidationRule>();

            if (documentType.ToLower() == "syllabus")
            {
                rules.AddRange(new[]
                {
                    new WorkflowValidationRule
                    {
                        RuleId = "SYLLABUS_REQUIRED_FIELDS",
                        RuleName = "Required Fields Validation",
                        Description = "Syllabus must have all required fields filled",
                        RuleType = "required"
                    },
                    new WorkflowValidationRule
                    {
                        RuleId = "SYLLABUS_DEPARTMENT_APPROVAL",
                        RuleName = "Department Approval Required",
                        Description = "Syllabus must be approved by department head",
                        RuleType = "conditional"
                    }
                });
            }

            return await Task.FromResult(rules);
        }
    }
}

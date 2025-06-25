using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Models.Workflow
{
    // DTOs for API responses
    public class WorkflowTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public int StageCount { get; set; }
        public int ActionCount { get; set; }
        public int TimedCount { get; set; }
    }

    public class WorkflowTemplateWithStagesDto : WorkflowTemplateDto
    {
        public List<WorkflowStageDto> Stages { get; set; } = new();
    }

    public class WorkflowStageDto
    {
        public Guid Id { get; set; }
        public Guid WorkflowTemplateId { get; set; }
        public string StageName { get; set; } = string.Empty;
        public int StageOrder { get; set; }
        public string AssignedRole { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsRequired { get; set; }
        public bool AutoApprove { get; set; }
        public int? TimeoutDays { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public List<WorkflowStageActionDto> Actions { get; set; } = new();
        public List<WorkflowStageRoleDto> RequiredRoles { get; set; } = new();
    }

    public class WorkflowStageActionDto
    {
        public Guid Id { get; set; }
        public Guid WorkflowStageId { get; set; }
        public string ActionName { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public Guid? NextStageId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class DocumentWorkflowDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public Guid WorkflowTemplateId { get; set; }
        public Guid? CurrentStageId { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid InitiatedBy { get; set; }
        public Guid? AssignedTo { get; set; }
        public DateTime InitiatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class DocumentWorkflowWithDetailsDto : DocumentWorkflowDto
    {
        public string WorkflowTemplateName { get; set; } = string.Empty;
        public string? CurrentStageName { get; set; }
        public string? CurrentStageAssignedRole { get; set; }
        public int? CurrentStageOrder { get; set; }
    }

    // DTOs for API requests
    public class CreateWorkflowTemplateDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        public List<CreateWorkflowStageDto> Stages { get; set; } = new();
    }

    public class UpdateWorkflowTemplateDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        public List<CreateWorkflowStageDto> Stages { get; set; } = new();
    }

    public class CreateWorkflowStageDto
    {
        [Required]
        [MaxLength(255)]
        public string StageName { get; set; } = string.Empty;

        [Required]
        public int StageOrder { get; set; }

        [Required]
        [MaxLength(100)]
        public string AssignedRole { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; } = true;

        public bool AutoApprove { get; set; } = false;

        public int? TimeoutDays { get; set; }

        public List<CreateWorkflowStageActionDto> Actions { get; set; } = new();

        public List<CreateWorkflowStageRoleDto> RequiredRoles { get; set; } = new();
    }

    public class UpdateWorkflowStageDto
    {
        [Required]
        [MaxLength(255)]
        public string StageName { get; set; } = string.Empty;

        [Required]
        public int StageOrder { get; set; }

        [Required]
        [MaxLength(100)]
        public string AssignedRole { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsRequired { get; set; } = true;

        public bool AutoApprove { get; set; } = false;

        public int? TimeoutDays { get; set; }
    }

    public class CreateWorkflowStageActionDto
    {
        [Required]
        [MaxLength(255)]
        public string ActionName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ActionType { get; set; } = string.Empty;

        public Guid? NextStageId { get; set; }

        /// <summary>
        /// Stage order for next stage (will be converted to NextStageId during processing)
        /// -1 = Complete workflow, null/undefined = Stay in current stage or auto-advance
        /// </summary>
        public int? NextStageOrder { get; set; }
    }

    public class UpdateWorkflowStageActionDto
    {
        [Required]
        [MaxLength(255)]
        public string ActionName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ActionType { get; set; } = string.Empty;

        public Guid? NextStageId { get; set; }
    }

    public class CreateDocumentWorkflowDto
    {
        [Required]
        [MaxLength(255)]
        public string DocumentId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public Guid WorkflowTemplateId { get; set; }

        [Required]
        public Guid InitiatedBy { get; set; }

        public Guid? AssignedTo { get; set; }
    }

    public class UpdateDocumentWorkflowDto
    {
        public Guid? CurrentStageId { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        public DateTime? CompletedDate { get; set; }
    }

    public class AdvanceWorkflowDto
    {
        [Required]
        [MaxLength(100)]
        public string ActionType { get; set; } = string.Empty;

        public string? Comments { get; set; }
    }

    public class CancelWorkflowDto
    {
        public string? Reason { get; set; }
    }

    // Statistics DTOs
    public class WorkflowStatsDto
    {
        public int TotalTemplates { get; set; }
        public int ActiveTemplates { get; set; }
        public int TotalWorkflows { get; set; }
        public int ActiveWorkflows { get; set; }
        public int CompletedWorkflows { get; set; }
        public int PendingWorkflows { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Workflow
{
    public class WorkflowStageHistory : BaseEntityGuid
    {
        [Required]
        public Guid DocumentWorkflowId { get; set; }

        [Required]
        public Guid StageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ActionTaken { get; set; } = string.Empty;

        [Required]
        public Guid ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; } = DateTime.UtcNow;

        public string? Comments { get; set; }

        public string[]? Attachments { get; set; }

        // Navigation properties
        public virtual DocumentWorkflow DocumentWorkflow { get; set; } = null!;
        public virtual WorkflowStage Stage { get; set; } = null!;
        public virtual WorkflowManagement.WorkflowUser ProcessedByUser { get; set; } = null!;
    }
}

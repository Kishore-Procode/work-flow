using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Workflow
{
    public class WorkflowStageAction : BaseEntityGuid
    {
        [Required]
        public Guid WorkflowStageId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ActionName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ActionType { get; set; } = string.Empty;

        public Guid? NextStageId { get; set; }

        // Navigation properties
        public virtual WorkflowStage WorkflowStage { get; set; } = null!;
        public virtual WorkflowStage? NextStage { get; set; }
    }
}

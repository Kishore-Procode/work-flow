using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Workflow
{
    public class WorkflowStage : BaseEntityGuid
    {
        [Required]
        public Guid WorkflowTemplateId { get; set; }

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

        // Navigation properties
        public virtual WorkflowTemplate WorkflowTemplate { get; set; } = null!;
        public virtual ICollection<WorkflowStageAction> Actions { get; set; } = new List<WorkflowStageAction>();
        public virtual ICollection<DocumentWorkflow> CurrentStageWorkflows { get; set; } = new List<DocumentWorkflow>();
    }
}

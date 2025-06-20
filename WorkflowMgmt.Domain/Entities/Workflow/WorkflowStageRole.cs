using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Workflow
{
    public class WorkflowStageRole : BaseEntityGuid
    {
        [Required]
        public Guid WorkflowStageId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleCode { get; set; } = string.Empty;

        [Required]
        public bool IsRequired { get; set; } = true;

        // Navigation properties
        public virtual WorkflowStage WorkflowStage { get; set; } = null!;
    }
}

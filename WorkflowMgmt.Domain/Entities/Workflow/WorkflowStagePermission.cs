using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Workflow
{
    public class WorkflowStagePermission : BaseEntityGuid
    {
        [Required]
        public Guid WorkflowStageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string PermissionName { get; set; } = string.Empty;

        [Required]
        public bool IsRequired { get; set; } = false;

        // Navigation properties
        public virtual WorkflowStage WorkflowStage { get; set; } = null!;
    }
}

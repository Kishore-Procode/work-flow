using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.WorkflowManagement
{
    public class StageAssignment : BaseEntityGuid
    {
        [Required]
        public Guid WorkflowStageId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public int? DepartmentId { get; set; }

        public bool IsDefault { get; set; } = false;

        // Navigation properties
        public virtual Workflow.WorkflowStage WorkflowStage { get; set; } = null!;
        public virtual WorkflowUser User { get; set; } = null!;
        public virtual WorkflowRole Role { get; set; } = null!;
    }
}

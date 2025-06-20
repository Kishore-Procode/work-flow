using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Workflow
{
    public class DocumentWorkflow : BaseEntityGuid
    {
        [Required]
        [MaxLength(255)]
        public string DocumentId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public Guid WorkflowTemplateId { get; set; }

        public Guid? CurrentStageId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "In Progress";

        [Required]
        public Guid InitiatedBy { get; set; }

        public DateTime InitiatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedDate { get; set; }

        // Navigation properties
        public virtual WorkflowTemplate WorkflowTemplate { get; set; } = null!;
        public virtual WorkflowStage? CurrentStage { get; set; }
    }
}

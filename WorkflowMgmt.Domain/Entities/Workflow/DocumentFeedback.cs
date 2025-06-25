using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Workflow
{
    public class DocumentFeedback : BaseEntityGuid
    {
        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        public Guid? WorkflowStageId { get; set; }

        public Guid? FeedbackProvider { get; set; }

        [Required]
        public string FeedbackText { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FeedbackType { get; set; } = "general";

        public bool IsAddressed { get; set; } = false;

        public Guid? AddressedBy { get; set; }

        public DateTime? AddressedDate { get; set; }

        // Navigation properties
        public virtual WorkflowStage? WorkflowStage { get; set; }
        public virtual User? FeedbackProviderUser { get; set; }
        public virtual User? AddressedByUser { get; set; }
    }
}

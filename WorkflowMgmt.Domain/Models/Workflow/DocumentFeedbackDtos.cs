using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Models.Workflow
{
    // DTOs for API responses
    public class DocumentFeedbackDto
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public Guid? WorkflowStageId { get; set; }
        public Guid? FeedbackProvider { get; set; }
        public string FeedbackText { get; set; } = string.Empty;
        public string FeedbackType { get; set; } = "general";
        public bool IsAddressed { get; set; }
        public Guid? AddressedBy { get; set; }
        public DateTime? AddressedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        // Additional properties for display
        public string? FeedbackProviderName { get; set; }
        public string? AddressedByName { get; set; }
        public string? StageName { get; set; }
    }

    // DTOs for API requests
    public class CreateDocumentFeedbackDto
    {
        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        public Guid? WorkflowStageId { get; set; }

        [Required]
        public string FeedbackText { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FeedbackType { get; set; } = "general";
    }

    public class UpdateDocumentFeedbackDto
    {
        public string? FeedbackText { get; set; }
        public string? FeedbackType { get; set; }
        public bool? IsAddressed { get; set; }
    }

    // DTO for document lifecycle view
    public class DocumentLifecycleDto
    {
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        // Workflow information
        public Guid? WorkflowId { get; set; }
        public string? WorkflowTemplateName { get; set; }
        public Guid? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public int? CurrentStageOrder { get; set; }
        public Guid? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        
        // Available actions for current user
        public List<WorkflowStageActionDto> AvailableActions { get; set; } = new();
        
        // Recent feedback
        public List<DocumentFeedbackDto> RecentFeedback { get; set; } = new();
    }

    // DTO for processing workflow actions
    public class ProcessDocumentActionDto
    {
        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public Guid ActionId { get; set; }

        public string? Comments { get; set; }

        public string? FeedbackType { get; set; } = "general";
    }
}

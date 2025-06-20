using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Models.Workflow
{
    public class WorkflowStageHistoryDto
    {
        public Guid Id { get; set; }
        public Guid DocumentWorkflowId { get; set; }
        public Guid StageId { get; set; }
        public string ActionTaken { get; set; } = string.Empty;
        public Guid ProcessedBy { get; set; }
        public DateTime ProcessedDate { get; set; }
        public string? Comments { get; set; }
        public string[]? Attachments { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class WorkflowStageHistoryWithDetailsDto : WorkflowStageHistoryDto
    {
        public string StageName { get; set; } = string.Empty;
        public string ProcessedByName { get; set; } = string.Empty;
        public string ProcessedByEmail { get; set; } = string.Empty;
        public string ProcessedByRole { get; set; } = string.Empty;
        public string DocumentTitle { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
    }

    public class CreateWorkflowStageHistoryDto
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

        public string? Comments { get; set; }

        public string[]? Attachments { get; set; }
    }
}

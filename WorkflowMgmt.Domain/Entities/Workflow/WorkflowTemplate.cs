using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Workflow
{
    public class WorkflowTemplate : BaseEntityGuid
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string DocumentType { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<WorkflowStage> Stages { get; set; } = new List<WorkflowStage>();
        public virtual ICollection<DocumentWorkflow> DocumentWorkflows { get; set; } = new List<DocumentWorkflow>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.WorkflowManagement
{
    public class WorkflowRole : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int HierarchyLevel { get; set; }

        public string? Permissions { get; set; } // JSON string of permissions

        // Navigation properties
        public virtual ICollection<WorkflowUser> Users { get; set; } = new List<WorkflowUser>();
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Syllabus
{
    public class SyllabusTemplate : BaseEntityGuid
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string TemplateType { get; set; } = string.Empty;

        [Required]
        public string Sections { get; set; } = "[]"; // JSON string of SyllabusSection[]
    }
}

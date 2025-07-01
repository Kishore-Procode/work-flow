using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Entities.Syllabus
{
    public class Syllabus : BaseEntityGuid
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        public int? CourseId { get; set; }

        public int? SemesterId { get; set; }

        [Required]
        public Guid TemplateId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FacultyName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? FacultyEmail { get; set; }

        [Required]
        public int Credits { get; set; }

        [Required]
        public int DurationWeeks { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContentCreationMethod { get; set; } = "form_entry";

        public string? CourseDescription { get; set; }

        public string? LearningObjectives { get; set; }

        public string? LearningOutcomes { get; set; }

        public string? CourseTopics { get; set; }

        public string? AssessmentMethods { get; set; }

        public string? DetailedContent { get; set; }

        public string? ReferenceMaterials { get; set; }

        public string? HtmlFormData { get; set; }

        [MaxLength(500)]
        public string? DocumentUrl { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Draft";

        public Guid? WorkflowId { get; set; }

        [MaxLength(50)]
        public string? FileProcessingStatus { get; set; }

        public string? FileProcessingNotes { get; set; }

        [MaxLength(255)]
        public string? OriginalFilename { get; set; }

        // Navigation properties
        public virtual SyllabusTemplate Template { get; set; } = null!;
    }
}

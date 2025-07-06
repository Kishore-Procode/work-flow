using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorkflowMgmt.Domain.Models.Workflow;
using Microsoft.AspNetCore.Http;

namespace WorkflowMgmt.Domain.Models.Syllabus
{
    // Base Syllabus DTOs
    public class SyllabusDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int? CourseId { get; set; }
        public int? SemesterId { get; set; }
        public Guid TemplateId { get; set; }
        public Guid FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int DurationWeeks { get; set; }
        public string ContentCreationMethod { get; set; } = "form_entry";
        public string? CourseDescription { get; set; }
        public string? LearningObjectives { get; set; }
        public string? LearningOutcomes { get; set; }
        public string? CourseTopics { get; set; }
        public string? AssessmentMethods { get; set; }
        public string? DetailedContent { get; set; }
        public string? ReferenceMaterials { get; set; }
        public string? HtmlFormData { get; set; }
        public string? DocumentUrl { get; set; }
        public string Status { get; set; } = "Draft";
        public Guid? WorkflowId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? FileProcessingStatus { get; set; }
        public string? FileProcessingNotes { get; set; }
        public string? OriginalFilename { get; set; }
    }

    public class SyllabusWithDetailsDto : SyllabusDto
    {
        public SyllabusTemplateDto Template { get; set; } = null!;
        public DepartmentDto Department { get; set; } = null!;
        public CourseDto? Course { get; set; }
        public SemesterDto? Semester { get; set; }
        public WorkflowDto? Workflow { get; set; }
    }

    public class CreateSyllabusDto
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
        public Guid FacultyId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FacultyName { get; set; } = string.Empty;

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

        public IFormFile? DocumentFile { get; set; }

        [MaxLength(500)]
        public string? DocumentUrl { get; set; }

        [MaxLength(255)]
        public string? OriginalFilename { get; set; }

        public bool AutoCreateWorkflow { get; set; } = true;
    }

    public class UpdateSyllabusDto
    {
        [MaxLength(255)]
        public string? Title { get; set; }

        public int? CourseId { get; set; }

        public int? SemesterId { get; set; }

        public Guid? FacultyId { get; set; }

        [MaxLength(255)]
        public string? FacultyName { get; set; }

        public int? Credits { get; set; }

        public int? DurationWeeks { get; set; }

        public string? CourseDescription { get; set; }

        public string? LearningObjectives { get; set; }

        public string? LearningOutcomes { get; set; }

        public string? CourseTopics { get; set; }

        public string? AssessmentMethods { get; set; }

        public string? DetailedContent { get; set; }

        public string? ReferenceMaterials { get; set; }

        public string? HtmlFormData { get; set; }

        [MaxLength(50)]
        public string? ContentCreationMethod { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        [MaxLength(500)]
        public string? DocumentUrl { get; set; }

        [MaxLength(255)]
        public string? OriginalFilename { get; set; }

        [MaxLength(50)]
        public string? FileProcessingStatus { get; set; }

        [MaxLength(1000)]
        public string? FileProcessingNotes { get; set; }
    }

    // Supporting DTOs
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class CourseDto
    {
        public int Id { get; set; }
        public string Course_Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class SemesterDto
    {
        public int Id { get; set; }
        public string Semester_Name { get; set; } = string.Empty;
        public int Year { get; set; }
    }

    public class WorkflowDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public CurrentStageDto? CurrentStage { get; set; }
    }

    public class CurrentStageDto
    {
        public Guid Id { get; set; }
        public string StageName { get; set; } = string.Empty;
        public string AssignedRole { get; set; } = string.Empty;
        public WorkflowStageActionDto[] Actions { get; set; } = Array.Empty<WorkflowStageActionDto>();
    }

    // Workflow Action DTOs
    public class SyllabusWorkflowActionDto
    {
        public string? Comments { get; set; }
        public string? Reason { get; set; }
    }

    // Document Download DTO
    public class SyllabusDocumentDto
    {
        public byte[] Content { get; set; } = Array.Empty<byte>();
        public string ContentType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }

    // Statistics DTOs
    public class SyllabusStatsDto
    {
        public int Total { get; set; }
        public int Draft { get; set; }
        public int UnderReview { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Published { get; set; }
        public SyllabusByDepartmentDto[] ByDepartment { get; set; } = Array.Empty<SyllabusByDepartmentDto>();
        public SyllabusByStatusDto[] ByStatus { get; set; } = Array.Empty<SyllabusByStatusDto>();
    }

    public class SyllabusByDepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class SyllabusByStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    // Syllabus Template DTOs
    public class SyllabusTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TemplateType { get; set; } = string.Empty;
        public string Sections { get; set; } = "[]"; // JSON string
        public string? HtmlFormTemplate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CreateSyllabusTemplateDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string TemplateType { get; set; } = string.Empty;

        [Required]
        public string Sections { get; set; } = "[]";

        public string? HtmlFormTemplate { get; set; }
    }

    public class UpdateSyllabusTemplateDto
    {
        [MaxLength(255)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? TemplateType { get; set; }

        public string? Sections { get; set; }

        public string? HtmlFormTemplate { get; set; }
    }
}

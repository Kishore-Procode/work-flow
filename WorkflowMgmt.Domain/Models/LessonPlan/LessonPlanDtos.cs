using System;
using System.ComponentModel.DataAnnotations;

namespace WorkflowMgmt.Domain.Models.LessonPlan
{
    // Base Lesson Plan DTOs
    public class LessonPlanDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid? SyllabusId { get; set; }
        public Guid TemplateId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; } = 60;
        public int NumberOfSessions { get; set; } = 1;
        public DateTime? ScheduledDate { get; set; }
        public Guid FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string ContentCreationMethod { get; set; } = "form_entry";
        public string? LessonDescription { get; set; }
        public string? LearningObjectives { get; set; }
        public string? TeachingMethods { get; set; }
        public string? LearningActivities { get; set; }
        public string? DetailedContent { get; set; }
        public string? Resources { get; set; }
        public string? AssessmentMethods { get; set; }
        public string? Prerequisites { get; set; }
        public string? DocumentUrl { get; set; }
        public string Status { get; set; } = "Draft";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string? FileProcessingStatus { get; set; }
        public string? FileProcessingNotes { get; set; }
        public string? OriginalFilename { get; set; }
    }

    public class LessonPlanWithDetailsDto : LessonPlanDto
    {
        public LessonPlanTemplateDto? Template { get; set; }
        public SyllabusBasicDto? Syllabus { get; set; }
    }

    public class LessonPlanTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TemplateType { get; set; } = string.Empty;
        public int DurationMinutes { get; set; } = 60;
        public string Sections { get; set; } = "[]"; // JSON string
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class SyllabusBasicDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public int? SemesterId { get; set; }
        public string? SemesterName { get; set; }
        public string FacultyName { get; set; } = string.Empty;
    }

    // Create/Update DTOs
    public class CreateLessonPlanDto
    {
        public string Title { get; set; } = string.Empty;
        public Guid? SyllabusId { get; set; }
        public Guid TemplateId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int DurationMinutes { get; set; } = 60;
        public int NumberOfSessions { get; set; } = 1;
        public DateTime? ScheduledDate { get; set; }
        public Guid FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string ContentCreationMethod { get; set; } = "form_entry";
        public string? LessonDescription { get; set; }
        public string? LearningObjectives { get; set; }
        public string? TeachingMethods { get; set; }
        public string? LearningActivities { get; set; }
        public string? DetailedContent { get; set; }
        public string? Resources { get; set; }
        public string? AssessmentMethods { get; set; }
        public string? Prerequisites { get; set; }
        public string? DocumentUrl { get; set; }
        public string? OriginalFilename { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateLessonPlanDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public Guid? SyllabusId { get; set; }
        public string? ModuleName { get; set; }
        public int? DurationMinutes { get; set; }
        public int? NumberOfSessions { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public Guid? FacultyId { get; set; }
        public string? FacultyName { get; set; }
        public string? LessonDescription { get; set; }
        public string? LearningObjectives { get; set; }
        public string? TeachingMethods { get; set; }
        public string? LearningActivities { get; set; }
        public string? DetailedContent { get; set; }
        public string? Resources { get; set; }
        public string? AssessmentMethods { get; set; }
        public string? Prerequisites { get; set; }
        public string? Status { get; set; }
        public string? DocumentUrl { get; set; }
        public string? OriginalFilename { get; set; }
        public string? ModifiedBy { get; set; }
    }

    // Stats DTO
    public class LessonPlanStatsDto
    {
        public int TotalLessonPlans { get; set; }
        public int DraftLessonPlans { get; set; }
        public int PublishedLessonPlans { get; set; }
        public int UnderReviewLessonPlans { get; set; }
        public int ApprovedLessonPlans { get; set; }
        public int RejectedLessonPlans { get; set; }
        public int TotalTemplates { get; set; }
        public int ActiveTemplates { get; set; }
        public double AverageDuration { get; set; }
        public int TotalSessions { get; set; }
    }

    // Template Create/Update DTOs
    public class CreateLessonPlanTemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TemplateType { get; set; } = string.Empty;
        public int DurationMinutes { get; set; } = 60;
        public string Sections { get; set; } = "[]"; // JSON string
        public string? CreatedBy { get; set; }
    }

    public class UpdateLessonPlanTemplateDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? TemplateType { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Sections { get; set; } // JSON string
        public string? ModifiedBy { get; set; }
    }
}

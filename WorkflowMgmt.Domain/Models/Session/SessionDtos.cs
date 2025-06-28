using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WorkflowMgmt.Domain.Models.Session
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid? LessonPlanId { get; set; }
        public Guid FacultyId { get; set; }
        public string TeachingMethod { get; set; } = "Lecture";
        public DateTime? SessionDate { get; set; }
        public TimeSpan? SessionTime { get; set; }
        public int DurationMinutes { get; set; } = 90;
        public string Instructor { get; set; } = string.Empty;
        public string ContentCreationMethod { get; set; } = "form_entry";
        public string? SessionDescription { get; set; }
        public string? SessionObjectives { get; set; }
        public string? SessionActivities { get; set; }
        public string? MaterialsEquipment { get; set; }
        public string? DetailedContent { get; set; }
        public string? ContentResources { get; set; }
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

    public class SessionWithDetailsDto : SessionDto
    {
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? LessonPlanTitle { get; set; }
        public string? SyllabusTitle { get; set; }
    }

    public class CreateSessionDto
    {
        public string Title { get; set; } = string.Empty;
        public Guid? LessonPlanId { get; set; }
        public Guid FacultyId { get; set; }
        public string TeachingMethod { get; set; } = "Lecture";
        public DateTime? SessionDate { get; set; }
        public TimeSpan? SessionTime { get; set; }
        public int DurationMinutes { get; set; } = 90;
        public string Instructor { get; set; } = string.Empty;
        public string ContentCreationMethod { get; set; } = "form_entry";
        public string? SessionDescription { get; set; }
        public string? SessionObjectives { get; set; }
        public string? SessionActivities { get; set; }
        public string? MaterialsEquipment { get; set; }
        public string? DetailedContent { get; set; }
        public string? ContentResources { get; set; }
        public string? DocumentUrl { get; set; }
        public string? OriginalFilename { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateSessionDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public Guid? LessonPlanId { get; set; }
        public Guid? FacultyId { get; set; }
        public string? TeachingMethod { get; set; }
        public DateTime? SessionDate { get; set; }
        public TimeSpan? SessionTime { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Instructor { get; set; }
        public string? SessionDescription { get; set; }
        public string? SessionObjectives { get; set; }
        public string? SessionActivities { get; set; }
        public string? MaterialsEquipment { get; set; }
        public string? DetailedContent { get; set; }
        public string? ContentResources { get; set; }
        public string? Status { get; set; }
        public string? DocumentUrl { get; set; }
        public string? OriginalFilename { get; set; }
        public string? FileProcessingStatus { get; set; }
        public string? FileProcessingNotes { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class SessionStatsDto
    {
        public int TotalSessions { get; set; }
        public int DraftSessions { get; set; }
        public int PublishedSessions { get; set; }
        public int UnderReviewSessions { get; set; }
        public int RejectedSessions { get; set; }
        public int TodaySessions { get; set; }
        public int UpcomingSessions { get; set; }
    }

    // Supporting DTOs
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class LessonPlanDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
    }
}

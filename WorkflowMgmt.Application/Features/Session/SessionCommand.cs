using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using WorkflowMgmt.Domain.Models.Session;

namespace WorkflowMgmt.Application.Features.Session
{
    // Queries
    public record GetSessionsQuery() : IRequest<IEnumerable<SessionDto>>;

    public record GetSessionByIdQuery(Guid Id) : IRequest<SessionWithDetailsDto?>;

    public record GetSessionsByStatusQuery(string Status) : IRequest<IEnumerable<SessionDto>>;

    public record GetSessionsByInstructorQuery(string Instructor) : IRequest<IEnumerable<SessionDto>>;

    public record GetSessionsByDepartmentQuery(int DepartmentId) : IRequest<IEnumerable<SessionDto>>;

    public record GetSessionsByLessonPlanQuery(Guid LessonPlanId) : IRequest<IEnumerable<SessionDto>>;

    public record GetSessionsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<IEnumerable<SessionDto>>;

    public record GetSessionsStatsQuery() : IRequest<SessionStatsDto>;

    // Commands
    public class CreateSessionCommand : IRequest<SessionDto>
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public Guid LessonPlanId { get; set; }

        // Faculty ID and Instructor will be set from logged-in user, not from form
        public Guid FacultyId { get; set; }
        public string Instructor { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string TeachingMethod { get; set; } = "Lecture";

        public DateTime? SessionDate { get; set; }

        public TimeSpan? SessionTime { get; set; }

        [Required]
        public int DurationMinutes { get; set; } = 90;

        [Required]
        [MaxLength(50)]
        public string ContentCreationMethod { get; set; } = "form_entry";

        public string? SessionDescription { get; set; }

        public string? SessionObjectives { get; set; }

        public string? SessionActivities { get; set; }

        public string? MaterialsEquipment { get; set; }

        public string? DetailedContent { get; set; }

        public string? ContentResources { get; set; }

        public IFormFile? DocumentFile { get; set; }

        public bool AutoCreateWorkflow { get; set; } = true;
    }

    public class UpdateSessionCommand : IRequest<SessionDto>
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string? Title { get; set; }

        public Guid? LessonPlanId { get; set; }

        // Faculty ID and Instructor will be set from logged-in user
        public Guid? FacultyId { get; set; }
        public string? Instructor { get; set; }

        [MaxLength(50)]
        public string? TeachingMethod { get; set; }

        public DateTime? SessionDate { get; set; }

        public TimeSpan? SessionTime { get; set; }

        public int? DurationMinutes { get; set; }

        public string? SessionDescription { get; set; }

        public string? SessionObjectives { get; set; }

        public string? SessionActivities { get; set; }

        public string? MaterialsEquipment { get; set; }

        public string? DetailedContent { get; set; }

        public string? ContentResources { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        public IFormFile? DocumentFile { get; set; }
    }

    public record DeleteSessionCommand(Guid Id) : IRequest<bool>;

    public record ToggleSessionActiveCommand(Guid Id) : IRequest<bool>;
}

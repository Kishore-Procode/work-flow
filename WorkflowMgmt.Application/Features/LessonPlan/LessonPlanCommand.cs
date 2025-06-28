using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Http;
using WorkflowMgmt.Domain.Models.LessonPlan;

namespace WorkflowMgmt.Application.Features.LessonPlan
{
    // Queries
    public record GetLessonPlansQuery() : IRequest<IEnumerable<LessonPlanDto>>;

    public record GetLessonPlanByIdQuery(Guid Id) : IRequest<LessonPlanWithDetailsDto?>;

    public record GetLessonPlansByStatusQuery(string Status) : IRequest<IEnumerable<LessonPlanDto>>;

    public record GetLessonPlansByFacultyQuery(string FacultyName) : IRequest<IEnumerable<LessonPlanDto>>;

    public record GetLessonPlansByTemplateQuery(Guid TemplateId) : IRequest<IEnumerable<LessonPlanDto>>;

    public record GetLessonPlansBySyllabusQuery(Guid SyllabusId) : IRequest<IEnumerable<LessonPlanDto>>;

    public record GetLessonPlansStatsQuery() : IRequest<LessonPlanStatsDto>;

    // Commands
    public class CreateLessonPlanCommand : IRequest<LessonPlanDto>
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        public Guid? SyllabusId { get; set; }

        [Required]
        public Guid TemplateId { get; set; }

        [Required]
        [MaxLength(255)]
        public string ModuleName { get; set; } = string.Empty;

        [Required]
        public int DurationMinutes { get; set; } = 60;

        [Required]
        public int NumberOfSessions { get; set; } = 1;

        public DateTime? ScheduledDate { get; set; }

        // Faculty ID and Name will be set from logged-in user, not from form
        public Guid FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string ContentCreationMethod { get; set; } = "form_entry";

        public string? LessonDescription { get; set; }

        public string? LearningObjectives { get; set; }

        public string? TeachingMethods { get; set; }

        public string? LearningActivities { get; set; }

        public string? DetailedContent { get; set; }

        public string? Resources { get; set; }

        public string? AssessmentMethods { get; set; }

        public string? Prerequisites { get; set; }

        public IFormFile? DocumentFile { get; set; }
    }

    public class UpdateLessonPlanCommand : IRequest<LessonPlanDto>
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string? Title { get; set; }

        public Guid? SyllabusId { get; set; }

        [MaxLength(255)]
        public string? ModuleName { get; set; }

        public int? DurationMinutes { get; set; }

        public int? NumberOfSessions { get; set; }

        public DateTime? ScheduledDate { get; set; }

        // Faculty ID and Name will be set from logged-in user
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

        [MaxLength(50)]
        public string? Status { get; set; }

        public IFormFile? DocumentFile { get; set; }
    }

    public record DeleteLessonPlanCommand(Guid Id) : IRequest<bool>;

    public record ToggleLessonPlanActiveCommand(Guid Id) : IRequest<bool>;

    // Template Queries
    public record GetLessonPlanTemplatesQuery() : IRequest<IEnumerable<LessonPlanTemplateDto>>;

    public record GetLessonPlanTemplateByIdQuery(Guid Id) : IRequest<LessonPlanTemplateDto?>;

    public record GetActiveLessonPlanTemplatesQuery() : IRequest<IEnumerable<LessonPlanTemplateDto>>;

    public record GetLessonPlanTemplatesByTypeQuery(string TemplateType) : IRequest<IEnumerable<LessonPlanTemplateDto>>;

    // Template Commands
    public class CreateLessonPlanTemplateCommand : IRequest<LessonPlanTemplateDto>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(100)]
        public string TemplateType { get; set; } = string.Empty;

        [Required]
        public int DurationMinutes { get; set; } = 60;

        [Required]
        public string Sections { get; set; } = "[]"; // JSON string
    }

    public class UpdateLessonPlanTemplateCommand : IRequest<LessonPlanTemplateDto>
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? TemplateType { get; set; }

        public int? DurationMinutes { get; set; }

        public string? Sections { get; set; } // JSON string
    }

    public record DeleteLessonPlanTemplateCommand(Guid Id) : IRequest<bool>;

    public record ToggleLessonPlanTemplateActiveCommand(Guid Id) : IRequest<bool>;
}

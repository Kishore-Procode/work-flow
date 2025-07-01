using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Syllabus;

namespace WorkflowMgmt.Application.Features.Syllabus
{
    // Queries
    public class GetAllSyllabiQuery : IRequest<IEnumerable<SyllabusWithDetailsDto>>
    {
    }

    public class GetSyllabiByDepartmentQuery : IRequest<IEnumerable<SyllabusWithDetailsDto>>
    {
        public int DepartmentId { get; set; }
    }

    public class GetSyllabiByStatusQuery : IRequest<IEnumerable<SyllabusWithDetailsDto>>
    {
        public string Status { get; set; } = string.Empty;
    }

    public class GetSyllabiByFacultyQuery : IRequest<IEnumerable<SyllabusWithDetailsDto>>
    {
        public string FacultyName { get; set; } = string.Empty;
    }

    public class GetSyllabiByTemplateQuery : IRequest<IEnumerable<SyllabusWithDetailsDto>>
    {
        public Guid TemplateId { get; set; }
    }

    public class GetSyllabusByLessonPlanQuery : IRequest<SyllabusWithDetailsDto?>
    {
        public Guid LessonPlanId { get; set; }
    }

    public class GetSyllabusByIdQuery : IRequest<SyllabusWithDetailsDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetSyllabusDocumentQuery : IRequest<SyllabusDocumentDto?>
    {
        public Guid Id { get; set; }
    }

    public class GetSyllabusStatsQuery : IRequest<SyllabusStatsDto>
    {
    }

    // Commands
    public class CreateSyllabusCommand : IRequest<SyllabusDto>
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

        // Faculty ID will be set from logged-in user, not from form
        public Guid FacultyId { get; set; }

        // Faculty Name will be set from logged-in user, not from form
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

        public bool AutoCreateWorkflow { get; set; } = true;
    }

    public class UpdateSyllabusCommand : IRequest<SyllabusDto?>
    {
        public Guid Id { get; set; }

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
        public string? Status { get; set; }

        public IFormFile? DocumentFile { get; set; }
    }

    public class DeleteSyllabusCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ToggleSyllabusActiveCommand : IRequest<SyllabusDto?>
    {
        public Guid Id { get; set; }
    }

    // Workflow Commands
    public class SubmitSyllabusForReviewCommand : IRequest<SyllabusDto?>
    {
        public Guid Id { get; set; }
    }

    public class ApproveSyllabusCommand : IRequest<SyllabusDto?>
    {
        public Guid Id { get; set; }
        public string? Comments { get; set; }
    }

    public class RejectSyllabusCommand : IRequest<SyllabusDto?>
    {
        public Guid Id { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class PublishSyllabusCommand : IRequest<SyllabusDto?>
    {
        public Guid Id { get; set; }
    }

    public class ReprocessSyllabusFileCommand : IRequest<SyllabusDto?>
    {
        public Guid Id { get; set; }
    }
}

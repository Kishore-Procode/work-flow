using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WorkflowMgmt.Domain.Models.Syllabus;

namespace WorkflowMgmt.Application.Features.SyllabusTemplate
{
    // Queries
    public class GetAllSyllabusTemplatesQuery : IRequest<IEnumerable<SyllabusTemplateDto>>
    {
    }

    public class GetSyllabusTemplatesByTypeQuery : IRequest<IEnumerable<SyllabusTemplateDto>>
    {
        public string TemplateType { get; set; } = string.Empty;
    }

    public class GetActiveSyllabusTemplatesQuery : IRequest<IEnumerable<SyllabusTemplateDto>>
    {
    }

    public class GetSyllabusTemplateByIdQuery : IRequest<SyllabusTemplateDto?>
    {
        public Guid Id { get; set; }
    }

    // Commands
    public class CreateSyllabusTemplateCommand : IRequest<SyllabusTemplateDto>
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
    }

    public class UpdateSyllabusTemplateCommand : IRequest<SyllabusTemplateDto?>
    {
        public Guid Id { get; set; }

        [MaxLength(255)]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [MaxLength(100)]
        public string? TemplateType { get; set; }

        public string? Sections { get; set; }
    }

    public class DeleteSyllabusTemplateCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class ToggleSyllabusTemplateActiveCommand : IRequest<SyllabusTemplateDto?>
    {
        public Guid Id { get; set; }
    }
}

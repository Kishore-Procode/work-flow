using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.SyllabusTemplate;
using WorkflowMgmt.Domain.Models;

namespace WorkflowMgmt.Application.Features.SyllabusTemplate
{
    public record GetSyllabusTemplatesCommand() : IRequest<ApiResponse<List<SyllabusTemplateDto>>>;

    public record GetSyllabusTemplateByIdCommand(Guid Id) : IRequest<ApiResponse<SyllabusTemplateDto>>;

    public record CreateSyllabusTemplateCommand(SyllabusTemplateDto Template) : IRequest<ApiResponse<Guid>>;

    public record UpdateSyllabusTemplateCommand(SyllabusTemplateDto Template) : IRequest<ApiResponse<bool>>;

    public record DeleteOrRestoreSyllabusTemplateCommand(Guid Id, string ModifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;
}

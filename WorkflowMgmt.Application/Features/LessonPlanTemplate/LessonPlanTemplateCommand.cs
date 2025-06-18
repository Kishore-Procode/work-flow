using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Entities.LessonPlanTemplate;



namespace WorkflowMgmt.Application.Features.LessonPlanTemplate
{
    public record GetLessonPlanTemplatesCommand() : IRequest<ApiResponse<List<LessonPlanTemplateDto>>>;

    public record GetLessonPlanTemplateByIdCommand(Guid Id) : IRequest<ApiResponse<LessonPlanTemplateDto>>;

    public record CreateLessonPlanTemplateCommand(LessonPlanTemplateDto Template) : IRequest<ApiResponse<Guid>>;

    public record UpdateLessonPlanTemplateCommand(LessonPlanTemplateDto Template) : IRequest<ApiResponse<bool>>;

    public record DeleteOrRestoreLessonPlanTemplateCommand(Guid Id, string ModifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;
}

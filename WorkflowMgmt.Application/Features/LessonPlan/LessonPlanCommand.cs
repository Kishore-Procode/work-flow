using System;
using System.Collections.Generic;
using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Entities.LessonPlan;

namespace WorkflowMgmt.Application.Features.LessonPlan
{
    public record GetLessonPlansCommand() : IRequest<ApiResponse<List<LessonPlanDto>>>;

    public record GetLessonPlanByIdCommand(Guid Id) : IRequest<ApiResponse<LessonPlanDto>>;

    public record CreateLessonPlanCommand(LessonPlanDto LessonPlan) : IRequest<ApiResponse<Guid>>;

    public record UpdateLessonPlanCommand(LessonPlanDto LessonPlan) : IRequest<ApiResponse<bool>>;

    public record DeleteOrRestoreLessonPlanCommand(Guid Id, string ModifiedBy, bool isRestore) : IRequest<ApiResponse<bool>>;
}

using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentStatus
{
    // Queries
    public record GetUserDocumentStatusCommand(Guid UserId, string? DocumentType = null) : IRequest<ApiResponse<List<DocumentStatusDto>>>;

    public record GetDocumentStatusDetailCommand(Guid DocumentId, string DocumentType, Guid UserId) : IRequest<ApiResponse<DocumentStatusDetailDto?>>;

    public record GetWorkflowRoadmapCommand(Guid WorkflowTemplateId) : IRequest<ApiResponse<List<WorkflowRoadmapDto>>>;

    public record GetUserDocumentStatsCommand(Guid UserId) : IRequest<ApiResponse<DocumentStatusStatsDto>>;
}



using MediatR;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentStatus
{
    public record GetDocumentWorkflowHistoryCommand(
        string DocumentId,
        string DocumentType,
        Guid UserId
    ) : IRequest<ApiResponse<List<DocumentWorkflowHistoryDto>>>;
}

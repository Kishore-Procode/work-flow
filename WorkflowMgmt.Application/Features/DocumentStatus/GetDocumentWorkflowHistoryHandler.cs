using MediatR;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.Models;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Application.Features.DocumentStatus
{
    public class GetDocumentWorkflowHistoryHandler : IRequestHandler<GetDocumentWorkflowHistoryCommand, ApiResponse<List<DocumentWorkflowHistoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDocumentWorkflowHistoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<DocumentWorkflowHistoryDto>>> Handle(
            GetDocumentWorkflowHistoryCommand request, 
            CancellationToken cancellationToken)
        {
            try
            {
                var history = await _unitOfWork.DocumentStatusRepository
                    .GetDocumentWorkflowHistoryAsync(request.DocumentId, request.DocumentType, request.UserId);

                return new ApiResponse<List<DocumentWorkflowHistoryDto>>
                {
                    Success = true,
                    Data = history.ToList(),
                    Message = "Document workflow history retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<DocumentWorkflowHistoryDto>>
                {
                    Success = false,
                    Data = new List<DocumentWorkflowHistoryDto>(),
                    Message = $"Error retrieving document workflow history: {ex.Message}"
                };
            }
        }
    }
}

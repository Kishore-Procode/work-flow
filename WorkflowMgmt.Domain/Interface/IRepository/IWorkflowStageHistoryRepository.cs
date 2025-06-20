using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowStageHistoryRepository
    {
        Task<IEnumerable<WorkflowStageHistoryWithDetailsDto>> GetByDocumentWorkflowIdAsync(Guid documentWorkflowId);
        Task<IEnumerable<WorkflowStageHistoryWithDetailsDto>> GetByStageIdAsync(Guid stageId);
        Task<IEnumerable<WorkflowStageHistoryWithDetailsDto>> GetByProcessedByAsync(Guid processedBy);
        Task<IEnumerable<WorkflowStageHistoryWithDetailsDto>> GetByActionTypeAsync(string actionType);
        Task<WorkflowStageHistoryWithDetailsDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateWorkflowStageHistoryDto history);
        Task<bool> ExistsAsync(Guid id);
        Task<WorkflowStageHistoryWithDetailsDto?> GetLatestByDocumentWorkflowAsync(Guid documentWorkflowId);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Workflow;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowStageActionRepository
    {
        Task<IEnumerable<WorkflowStageActionDto>> GetByStageIdAsync(Guid stageId);
        Task<IEnumerable<WorkflowStageActionDto>> GetActiveByStageIdAsync(Guid stageId);
        Task<WorkflowStageActionDto?> GetByIdAsync(Guid id);
        Task<WorkflowStageActionDto?> GetByStageAndActionTypeAsync(Guid stageId, string actionType);
        Task<Guid> CreateAsync(Guid stageId, CreateWorkflowStageActionDto action);
        Task<bool> UpdateAsync(Guid id, UpdateWorkflowStageActionDto action);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByStageAndActionTypeAsync(Guid stageId, string actionType, Guid? excludeId = null);
        Task<bool> DeleteByStageIdAsync(Guid stageId);
        Task<bool> DeleteByTemplateIdAsync(Guid templateId);
    }
}

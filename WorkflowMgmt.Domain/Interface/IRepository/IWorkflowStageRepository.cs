using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Workflow;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowStageRepository
    {
        Task<IEnumerable<WorkflowStageDto>> GetByTemplateIdAsync(Guid templateId);
        Task<WorkflowStageDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<WorkflowStageDto>> GetByRoleAsync(string role);
        Task<WorkflowStageDto?> GetFirstStageByTemplateIdAsync(Guid templateId);
        Task<WorkflowStageDto?> GetNextStageAsync(Guid currentStageId, string actionType);
        Task<WorkflowStageDto?> GetNextStageAsync(Guid templateId, int currentStageOrder);
        Task<Guid> CreateAsync(Guid templateId, CreateWorkflowStageDto stage);
        Task<bool> UpdateAsync(Guid id, UpdateWorkflowStageDto stage);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeactivateStagesByTemplateIdAsync(Guid templateId);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsInTemplateAsync(Guid templateId, int stageOrder, Guid? excludeId = null);
        Task<int> GetMaxStageOrderAsync(Guid templateId);
        Task<bool> ReorderStagesAsync(Guid templateId, List<(Guid stageId, int newOrder)> stageOrders);
    }
}

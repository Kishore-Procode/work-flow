using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Workflow;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowTemplateRepository
    {
        Task<List<WorkflowTemplateWithStagesDto>> GetAllAsync();
        Task<List<WorkflowTemplateDto>> GetByDocumentTypeAsync(string documentType);
        Task<List<WorkflowTemplateDto>> GetActiveAsync();
        Task<WorkflowTemplateWithStagesDto?> GetByIdAsync(Guid id);
        Task<WorkflowTemplateDto?> GetByIdSimpleAsync(Guid id);
        Task<Guid> CreateAsync(CreateWorkflowTemplateDto template);
        Task<bool> UpdateAsync(Guid id, UpdateWorkflowTemplateDto template);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ToggleActiveAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByNameAndDocumentTypeAsync(string name, string documentType, Guid? excludeId = null);
    }
}

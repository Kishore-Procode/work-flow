using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowStageRoleRepository
    {
        // Basic CRUD operations
        Task<List<WorkflowStageRoleDto>> GetByStageIdAsync(Guid stageId);
        Task<bool> CreateAsync(Guid stageId, CreateWorkflowStageRoleDto role);
        Task<bool> DeleteByStageIdAsync(Guid stageId);
        Task<bool> DeleteByTemplateIdAsync(Guid templateId);
        Task<bool> UpdateStageRolesAsync(Guid stageId, List<UpdateRoleDto> roles);

        // Query operations
        Task<List<WorkflowStageRoleDto>> GetByRoleCodeAsync(string roleCode);
        Task<bool> ExistsAsync(Guid stageId, string roleCode);

        // Bulk operations
        Task<bool> CreateMultipleAsync(Guid stageId, List<CreateWorkflowStageRoleDto> roles);
    }
}

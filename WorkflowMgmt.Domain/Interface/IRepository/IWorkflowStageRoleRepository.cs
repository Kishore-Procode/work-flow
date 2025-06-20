using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowStageRoleRepository
    {
        // Basic CRUD operations
        Task<IEnumerable<WorkflowStageRoleDto>> GetByStageIdAsync(Guid stageId);
        Task<bool> CreateAsync(Guid stageId, CreateWorkflowStageRoleDto role);
        Task<bool> DeleteByStageIdAsync(Guid stageId);
        Task<bool> UpdateStageRolesAsync(Guid stageId, IEnumerable<UpdateRoleDto> roles);

        // Query operations
        Task<IEnumerable<WorkflowStageRoleDto>> GetByRoleCodeAsync(string roleCode);
        Task<bool> ExistsAsync(Guid stageId, string roleCode);

        // Bulk operations
        Task<bool> CreateMultipleAsync(Guid stageId, IEnumerable<CreateWorkflowStageRoleDto> roles);
    }
}

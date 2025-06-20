using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Workflow;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowStagePermissionRepository
    {
        // Basic CRUD operations
        Task<IEnumerable<WorkflowStagePermissionDto>> GetByStageIdAsync(Guid stageId);
        Task<bool> CreateAsync(Guid stageId, CreateWorkflowStagePermissionDto permission);
        Task<bool> DeleteByStageIdAsync(Guid stageId);
        Task<bool> UpdateStagePermissionsAsync(Guid stageId, IEnumerable<UpdatePermissionDto> permissions);

        // Query operations
        Task<IEnumerable<WorkflowStagePermissionDto>> GetByPermissionNameAsync(string permissionName);
        Task<bool> ExistsAsync(Guid stageId, string permissionName);

        // Bulk operations
        Task<bool> CreateMultipleAsync(Guid stageId, IEnumerable<CreateWorkflowStagePermissionDto> permissions);

        // Available permissions
        Task<IEnumerable<AvailablePermissionDto>> GetAvailablePermissionsAsync();
    }
}

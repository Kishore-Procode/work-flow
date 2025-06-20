using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.WorkflowManagement;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowRoleRepository
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<IEnumerable<RoleDto>> GetActiveAsync();
        Task<RoleDto?> GetByIdAsync(int id);
        Task<RoleDto?> GetByCodeAsync(string code);
        Task<int> CreateAsync(CreateRoleDto role);
        Task<bool> UpdateAsync(int id, UpdateRoleDto role);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleActiveAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}

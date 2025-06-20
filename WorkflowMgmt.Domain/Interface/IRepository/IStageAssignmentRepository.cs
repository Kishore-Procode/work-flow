using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.WorkflowManagement;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IStageAssignmentRepository
    {
        Task<IEnumerable<StageAssigneeDto>> GetByStageIdAsync(Guid stageId);
        Task<IEnumerable<StageAssigneeDto>> GetByUserIdAsync(Guid userId);
        Task<StageAssigneeDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(Guid stageId, AssignUserToStageDto assignment);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteByStageAndUserAsync(Guid stageId, Guid userId);
        Task<bool> ExistsAsync(Guid stageId, Guid userId);
        Task<bool> SetDefaultAssigneeAsync(Guid stageId, Guid userId, int? departmentId = null);
        Task<StageAssigneeDto?> GetDefaultAssigneeAsync(Guid stageId, int? departmentId = null);
    }
}

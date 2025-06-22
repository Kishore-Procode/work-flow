using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IWorkflowRoleMappingRepository
    {
        Task<List<WorkflowRoleMappingDto>> GetDepartmentRoleUsers();
        Task<List<WorkflowRoleMappingDto>> GetDepartmentRoleUsersByDepartment(int departmentId);
        Task<bool> UpdateDepartmentRoleUsers(int departmentId, List<DepartmentRoleUserAssignmentDto> assignments);
        Task<bool> ValidateUniqueRolePrimary(int departmentId, int roleId, Guid userId);
    }
}

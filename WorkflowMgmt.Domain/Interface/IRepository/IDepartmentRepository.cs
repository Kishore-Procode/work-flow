using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IDepartmentRepository
    {
        Task<List<DepartmentDTO>> GetAllDepartments();
        Task<DepartmentDTO?> GetDepartmentById(int id);
        Task<int> InsertDepartment(DepartmentDTO department);
        Task<bool> UpdateDepartment(DepartmentDTO department);
        Task<bool> DeleteOrRestoreDepartment(int id, string modifiedBy, bool isRestore);
        Task<DepartmentStatsDto> GetDepartmentStatsAsync();
    }
}

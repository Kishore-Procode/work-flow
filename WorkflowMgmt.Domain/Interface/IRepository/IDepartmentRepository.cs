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
    }
}

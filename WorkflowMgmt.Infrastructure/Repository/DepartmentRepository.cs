using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class DepartmentRepository : RepositoryTranBase, IDepartmentRepository
    {
        public DepartmentRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<DepartmentDTO>> GetAllDepartments()
        {
            var departments = await Connection.QueryAsync<DepartmentDTO>(
                "SELECT * FROM public.departments", Transaction);

            return departments.ToList();
        }
    }
}

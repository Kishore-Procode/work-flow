using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class UserManagementRepository : RepositoryTranBase, IUserManagementRepository
    {
        public UserManagementRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var users = await Connection.QueryAsync<UserDTO>(
                "SELECT * FROM workflowmgmt.users", Transaction);

            return users.ToList();
        }
    }
}

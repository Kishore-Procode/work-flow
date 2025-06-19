using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class UserRepository : RepositoryTranBase, IUserRepository
    {
        public UserRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await Connection.QueryAsync<User>(
                "SELECT * FROM workflowmgmt.users", Transaction);

            return users.ToList();
        }
        public async Task<User?> GetUserByUserName(string userName)
        {
            return await Connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM workflowmgmt.users where (username = @UserName or email = @UserName) AND is_active IS TRUE",
                new { UserName = userName },
                Transaction);
        }

        public async Task<User?> GetUserById(Guid userId)
        {
            return await Connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM workflowmgmt.users WHERE id = @UserId AND is_active IS TRUE",
                new { UserId = userId },
                Transaction);
        }
        public async Task UpdateLastLoginAsync(Guid userId)
        {
            await Connection.ExecuteAsync(
                "UPDATE workflowmgmt.users SET last_login = @Now WHERE id = @UserId",
                new { Now = DateTime.UtcNow, UserId = userId },
                transaction: Transaction
            );
        }
        public async Task<Role?> GetRoleByRoleId(int roleId)
        {
            return await Connection.QueryFirstOrDefaultAsync<Role>(
                "SELECT * FROM workflowmgmt.roles WHERE id = @RoleId",
                new { RoleId = roleId },
                transaction: Transaction
                );
        }
        public async Task<DepartmentDTO?> GetDepartmentByDepartmentId(int? departmentId)
        {
            return await Connection.QueryFirstOrDefaultAsync<DepartmentDTO>(
                "SELECT * FROM workflowmgmt.departments WHERE id = @DepartmentId",
                new { DepartmentId = departmentId },
                transaction: Transaction
                );
        }
    }
}

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
using WorkflowMgmt.Domain.Models.WorkflowManagement;
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
                "SELECT * FROM workflowmgmt.users " +
                "where (lower(username) = lower(@UserName) or " +
                "lower(email) = lower(@UserName)) AND is_active IS TRUE",
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
                new { Now = DateTime.Now, UserId = userId },
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

        public async Task<List<WorkflowMgmt.Domain.Entities.UserDto>> GetActiveUsers()
        {
            var sql = @"
                SELECT
                    u.id,
                    u.username,
                    u.email,
                    u.first_name,
                    u.last_name,
                    u.role_id,
                    u.department_id,
                    u.phone,
                    u.profile_image_url,
                    u.allowed_departments,
                    u.allowed_roles,
                    r.name as role_name,
                    r.code as role_code,
                    d.name as department_name,
                    d.code as department_code
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON u.department_id = d.id
                WHERE u.is_active = true AND r.is_active = true
                ORDER BY u.first_name, u.last_name";

            var users = await Connection.QueryAsync<WorkflowMgmt.Domain.Entities.UserDto>(sql, Transaction);
            return users.ToList();
        }

        public async Task<List<WorkflowMgmt.Domain.Entities.UserDto>> GetActiveUsersByDepartment(int departmentId)
        {
            var sql = @"
                SELECT
                    u.id,
                    u.username,
                    u.email,
                    u.first_name,
                    u.last_name,
                    u.role_id,
                    u.department_id,
                    u.phone,
                    u.profile_image_url,
                    u.allowed_departments,
                    u.allowed_roles,
                    r.name as role_name,
                    r.code as role_code,
                    d.name as department_name,
                    d.code as department_code
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON u.department_id = d.id
                WHERE u.is_active = true AND r.is_active = true
                AND u.department_id = @DepartmentId
                ORDER BY u.first_name, u.last_name";

            var users = await Connection.QueryAsync<WorkflowMgmt.Domain.Entities.UserDto>(sql, new { DepartmentId = departmentId }, Transaction);
            return users.ToList();
        }

        public async Task<List<WorkflowMgmt.Domain.Entities.UserDto>> GetActiveUsersByAllowedDepartment(int departmentId)
        {
            var sql = @"
                SELECT
                    u.id,
                    u.username,
                    u.email,
                    u.first_name,
                    u.last_name,
                    u.role_id,
                    u.department_id,
                    u.phone,
                    u.profile_image_url,
                    u.allowed_departments,
                    u.allowed_roles,
                    r.name as role_name,
                    r.code as role_code,
                    d.name as department_name,
                    d.code as department_code
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON u.department_id = d.id
                WHERE u.is_active = true AND r.is_active = true
                AND @DepartmentId = ANY(u.allowed_departments)
                ORDER BY u.first_name, u.last_name";

            var users = await Connection.QueryAsync<WorkflowMgmt.Domain.Entities.UserDto>(sql, new { DepartmentId = departmentId }, Transaction);
            return users.ToList();
        }

        public async Task<List<WorkflowMgmt.Domain.Entities.UserDto>> GetActiveUsersByAllowedDepartmentAndRole(int departmentId, int roleId)
        {
            var sql = @"
                SELECT
                    u.id,
                    u.username,
                    u.email,
                    u.first_name,
                    u.last_name,
                    u.role_id,
                    u.department_id,
                    u.phone,
                    u.profile_image_url,
                    u.allowed_departments,
                    u.allowed_roles,
                    r.name as role_name,
                    r.code as role_code,
                    d.name as department_name,
                    d.code as department_code
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON u.department_id = d.id
                WHERE u.is_active = true AND r.is_active = true
                AND @DepartmentId = ANY(u.allowed_departments)
                AND @RoleId = ANY(u.allowed_roles)
                ORDER BY u.first_name, u.last_name";

            var users = await Connection.QueryAsync<WorkflowMgmt.Domain.Entities.UserDto>(sql, new { DepartmentId = departmentId, RoleId = roleId }, Transaction);
            return users.ToList();
        }
    }
}

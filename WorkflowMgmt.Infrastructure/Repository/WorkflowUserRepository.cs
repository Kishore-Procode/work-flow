using Dapper;
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
    public class WorkflowUserRepository : RepositoryTranBase, IWorkflowUserRepository
    {
        public WorkflowUserRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<UserWithDetailsDto>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                ORDER BY u.last_name, u.first_name";

            var result = await Connection.QueryAsync<UserWithDetailsDto>(sql, transaction: Transaction);
            return result.ToList();
        }

        public async Task<List<UserWithDetailsDto>> GetByDepartmentAsync(int departmentId)
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                WHERE u.department_id = @DepartmentId
                ORDER BY u.last_name, u.first_name";

            var result = await Connection.QueryAsync<UserWithDetailsDto>(sql, new { DepartmentId = departmentId }, transaction: Transaction);
            return result.ToList();
        }

        public async Task<List<UserWithDetailsDto>> GetByRoleAsync(int roleId)
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                WHERE u.role_id = @RoleId
                ORDER BY u.last_name, u.first_name";

            var result = await Connection.QueryAsync<UserWithDetailsDto>(sql, new { RoleId = roleId }, transaction: Transaction);
            return result.ToList();
        }

        public async Task<List<UserWithDetailsDto>> GetByRoleCodeAsync(string roleCode)
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                WHERE r.code = @RoleCode
                ORDER BY u.last_name, u.first_name";

            var result = await Connection.QueryAsync<UserWithDetailsDto>(sql, new { RoleCode = roleCode }, transaction: Transaction);
            return result.ToList();
        }

        public async Task<List<UserWithDetailsDto>> GetActiveAsync()
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                WHERE u.is_active = true
                ORDER BY u.last_name, u.first_name";

            var result = await Connection.QueryAsync<UserWithDetailsDto>(sql, transaction: Transaction);
            return result.ToList();
        }

        public async Task<UserWithDetailsDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                WHERE u.id = @Id";

            return await Connection.QuerySingleOrDefaultAsync<UserWithDetailsDto>(sql, new { Id = id }, transaction: Transaction);
        }

        public async Task<UserWithDetailsDto?> GetByUsernameAsync(string username)
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                WHERE u.username = @Username";

            return await Connection.QuerySingleOrDefaultAsync<UserWithDetailsDto>(sql, new { Username = username }, transaction: Transaction);
        }

        public async Task<UserWithDetailsDto?> GetByEmailAsync(string email)
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                WHERE u.email = @Email";

            return await Connection.QuerySingleOrDefaultAsync<UserWithDetailsDto>(sql, new { Email = email }, transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(CreateUserDto user)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.users
                (id, first_name, last_name, email, username, password_hash, department_id, role_id,
                 phone_number, email_verified, is_active, created_date, created_by)
                VALUES (@Id, @FirstName, @LastName, @Email, @Username, @PasswordHash, @DepartmentId, @RoleId,
                        @PhoneNumber, false, true, @CreatedDate, @CreatedBy)";

            // Hash the password (you should use a proper password hashing library like BCrypt)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Username,
                PasswordHash = passwordHash,
                user.DepartmentId,
                user.RoleId,
                user.PhoneNumber,
                CreatedDate = DateTime.Now,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateUserDto user)
        {
            var sql = @"
                UPDATE workflowmgmt.users
                SET first_name = @FirstName,
                    last_name = @LastName,
                    email = @Email,
                    department_id = @DepartmentId,
                    role_id = @RoleId,
                    phone_number = @PhoneNumber,
                    email_verified = COALESCE(@EmailVerified, email_verified),
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.DepartmentId,
                user.RoleId,
                user.PhoneNumber,
                user.EmailVerified,
                ModifiedDate = DateTime.Now,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM workflowmgmt.users WHERE id = @Id";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.users
                SET is_active = NOT is_active,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedDate = DateTime.Now,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.users WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByUsernameAsync(string username, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM workflowmgmt.users
                WHERE username = @Username";

            object parameters = new { Username = username };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Username = username, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM workflowmgmt.users
                WHERE email = @Email";

            object parameters = new { Email = email };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Email = email, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> UpdatePasswordAsync(Guid id, string passwordHash)
        {
            var sql = @"
                UPDATE workflowmgmt.users
                SET password_hash = @PasswordHash,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                PasswordHash = passwordHash,
                ModifiedDate = DateTime.Now,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<UserStatsDto> GetUserStatsAsync()
        {
            var sql = @"
                SELECT
                    COUNT(*) as TotalUsers,
                    COUNT(CASE WHEN is_active = true THEN 1 END) as ActiveUsers,
                    COUNT(CASE WHEN last_login_date >= @RecentDate THEN 1 END) as RecentLogins
                FROM workflowmgmt.users";

            var recentDate = DateTime.Now.AddDays(-30); // Last 30 days
            var stats = await Connection.QuerySingleAsync<UserStatsDto>(sql, new { RecentDate = recentDate }, transaction: Transaction);

            // Get users by role
            var rolesSql = @"
                SELECT
                    r.name as RoleName,
                    COUNT(u.id) as Count
                FROM workflowmgmt.roles r
                LEFT JOIN workflowmgmt.users u ON r.id = u.role_id AND u.is_active = true
                GROUP BY r.id, r.name
                ORDER BY r.name";

            var usersByRole = await Connection.QueryAsync<UsersByRoleDto>(rolesSql, transaction: Transaction);
            stats.UsersByRole = usersByRole.ToArray();

            // Get users by department
            var departmentsSql = @"
                SELECT
                    d.name as DepartmentName,
                    COUNT(u.id) as Count
                FROM workflowmgmt.departments d
                LEFT JOIN workflowmgmt.users u ON d.id = u.department_id AND u.is_active = true
                GROUP BY d.id, d.name
                ORDER BY d.name";

            var usersByDepartment = await Connection.QueryAsync<UsersByDepartmentDto>(departmentsSql, transaction: Transaction);
            stats.UsersByDepartment = usersByDepartment.ToArray();

            return stats;
        }

        public async Task<List<UserWithDetailsDto>> GetEligibleUsersForStageAsync(Guid stageId, int? departmentId = null)
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                INNER JOIN workflowmgmt.workflow_stages ws ON ws.id = @StageId
                WHERE u.is_active = true
                AND r.code = ws.assigned_role";

            object parameters = new { StageId = stageId };

            if (departmentId.HasValue)
            {
                sql += " AND u.department_id = @DepartmentId";
                parameters = new { StageId = stageId, DepartmentId = departmentId.Value };
            }

            sql += " ORDER BY u.last_name, u.first_name";

            var result = await Connection.QueryAsync<UserWithDetailsDto>(sql, parameters, transaction: Transaction);
            return result.ToList();
        }

        public async Task<UserWithDetailsDto?> GetDefaultAssigneeForStageAsync(Guid stageId, int departmentId)
        {
            var sql = @"
                SELECT
                    u.id as Id,
                    u.first_name as FirstName,
                    u.last_name as LastName,
                    u.email as Email,
                    u.username as Username,
                    u.department_id as DepartmentId,
                    u.role_id as RoleId,
                    u.phone_number as PhoneNumber,
                    u.profile_picture as ProfilePicture,
                    u.last_login_date as LastLoginDate,
                    u.email_verified as EmailVerified,
                    u.is_active as IsActive,
                    u.created_date as CreatedDate,
                    u.modified_date as ModifiedDate,
                    u.created_by as CreatedBy,
                    u.modified_by as ModifiedBy,
                    d.name as DepartmentName,
                    r.name as RoleName,
                    r.code as RoleCode,
                    r.hierarchy_level as RoleHierarchyLevel
                FROM workflowmgmt.users u
                INNER JOIN workflowmgmt.departments d ON u.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON u.role_id = r.id
                INNER JOIN workflowmgmt.stage_assignments sa ON u.id = sa.user_id
                WHERE sa.workflow_stage_id = @StageId
                AND sa.department_id = @DepartmentId
                AND sa.is_default = true
                AND u.is_active = true
                LIMIT 1";

            return await Connection.QuerySingleOrDefaultAsync<UserWithDetailsDto>(sql,
                new { StageId = stageId, DepartmentId = departmentId },
                transaction: Transaction);
        }
    }
}

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
                    u.is_active,
                    u.last_login,
                    u.created_date,
                    u.modified_date,
                    u.created_by,
                    u.modified_by,
                    u.allowed_departments,
                    u.allowed_roles,
                    r.name as role_name,
                    r.code as role_code,
                    r.description as role_description,
                    d.name as department_name,
                    d.code as department_code
                FROM workflowmgmt.users u
                LEFT JOIN workflowmgmt.roles r ON u.role_id = r.id
                LEFT JOIN workflowmgmt.departments d ON u.department_id = d.id
                ORDER BY u.first_name, u.last_name";

            var users = await Connection.QueryAsync<UserDTO>(sql, transaction: Transaction);
            return users.ToList();
        }
        public async Task<UserDTO?> GetUserManagementById(Guid id)
        {
            var sql = "SELECT id, username, email, first_name, last_name, role_id, department_id, " +
                "phone, profile_image_url, is_active, last_login, created_date, modified_date, " +
                "created_by, modified_by FROM workflowmgmt.users WHERE id = @Id" +
                " AND is_active = true";
            var user = await Connection.QueryFirstOrDefaultAsync<UserDTO>(sql, new { Id = id }, Transaction);
            return user;
        }

        public async Task<Guid> InsertUser(UserDTO user)
        {
            // Only hash the password if it's not empty and not already hashed
            if (!string.IsNullOrEmpty(user.password_hash) && !user.password_hash.StartsWith("$2"))
            {
                user.password_hash = BCrypt.Net.BCrypt.HashPassword(user.password_hash);
            }

            var sql = @"
                INSERT INTO workflowmgmt.users (
                    username,
                    email,
                    password_hash,
                    first_name,
                    last_name,
                    role_id,
                    department_id,
                    phone,
                    profile_image_url,
                    is_active,
                    last_login,
                    created_date,
                    created_by,
                    allowed_departments,
                    allowed_roles
                ) VALUES (
                    @username,
                    @email,
                    @password_hash,
                    @first_name,
                    @last_name,
                    @role_id,
                    @department_id,
                    @phone,
                    @profile_image_url,
                    @is_active,
                    @last_login,
                    NOW(),
                    @created_by,
                    @allowed_departments,
                    @allowed_roles
                )
                RETURNING id;
            ";

            var userId = await Connection.ExecuteScalarAsync<Guid>(sql, user, Transaction);
            return userId;
        }

        public async Task<bool> UpdateUser(UserDTO user)
        {
            string sql;

            // Check if password should be updated
            if (!string.IsNullOrEmpty(user.password_hash))
            {
                // Only hash the password if it's not already hashed
                if (!user.password_hash.StartsWith("$2"))
                {
                    user.password_hash = BCrypt.Net.BCrypt.HashPassword(user.password_hash);
                }

                sql = @"
                    UPDATE workflowmgmt.users SET
                    username = @username,
                    email = @email,
                    password_hash = @password_hash,
                    first_name = @first_name,
                    last_name = @last_name,
                    role_id = @role_id,
                    department_id = @department_id,
                    phone = @phone,
                    profile_image_url = @profile_image_url,
                    is_active = @is_active,
                    allowed_departments = @allowed_departments,
                    allowed_roles = @allowed_roles,
                    modified_date = NOW(),
                    modified_by = @modified_by
                    WHERE id = @id;
               ";
            }
            else
            {
                // Update without changing password
                sql = @"
                    UPDATE workflowmgmt.users SET
                    username = @username,
                    email = @email,
                    first_name = @first_name,
                    last_name = @last_name,
                    role_id = @role_id,
                    department_id = @department_id,
                    phone = @phone,
                    profile_image_url = @profile_image_url,
                    is_active = @is_active,
                    allowed_departments = @allowed_departments,
                    allowed_roles = @allowed_roles,
                    modified_date = NOW(),
                    modified_by = @modified_by
                    WHERE id = @id;
               ";
            }

            var rowsAffected = await Connection.ExecuteAsync(sql, user, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteOrRestoreUser(Guid id, string modifiedBy, bool isRestore)
        {
            var sql = @"
        UPDATE workflowmgmt.users
        SET is_active = @IsActive,
            modified_by = @ModifiedBy,
            modified_date = NOW()
        WHERE id = @id AND is_active != @IsActive";

            var result = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedBy = modifiedBy,
                IsActive = isRestore
               
            }, Transaction);

            return result > 0;
        }

        public async Task<bool> UpdatePassword(UpdatePasswordRequest updateUser)
        {
            var existingPassword = await Connection.QueryFirstOrDefaultAsync<string>(
                "SELECT password_hash FROM workflowmgmt.users WHERE id = @Id",
                new { Id = updateUser.UserId },
                Transaction
            );

            if (existingPassword == null)
                return false;

           
            if (!BCrypt.Net.BCrypt.Verify(updateUser.OldPassword, existingPassword))
                return false;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(updateUser.NewPassword);
            
            var rows = await Connection.ExecuteAsync(@"
                UPDATE workflowmgmt.users
                SET password_hash = @HashedPassword,
                    modified_date = NOW()
                WHERE id = @Id",
                new { Id = updateUser.UserId, HashedPassword = hashedPassword },
                Transaction
            );

            return rows > 0;
        }

        public async Task<bool> UpdateProfile(UpdateProfileRequest profile)
        {
            var sql = @"
                UPDATE workflowmgmt.users SET
                first_name = @first_name,
                last_name = @last_name,
                email = @email,
                phone = @phone,
                modified_date = NOW()
                WHERE id = @id;
            ";

            var rowsAffected = await Connection.ExecuteAsync(sql, profile, Transaction);
            return rowsAffected > 0;
        }

    }
}

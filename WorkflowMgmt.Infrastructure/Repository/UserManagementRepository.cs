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
                "SELECT id, username, email, first_name, last_name, " +
                "role_id, department_id, phone, profile_image_url, is_active, last_login, " +
                "created_date, modified_date, created_by, modified_by FROM workflowmgmt.users",
                Transaction);

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
            user.password_hash = BCrypt.Net.BCrypt.HashPassword(user.password_hash);

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
                    created_by                                    
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
                    @created_by
                )
                RETURNING id;
            ";

            var userId = await Connection.ExecuteScalarAsync<Guid>(sql, user, Transaction);
            return userId;
        }

        public async Task<bool> UpdateUser(UserDTO user)
        {
            user.password_hash = BCrypt.Net.BCrypt.HashPassword(user.password_hash);

            var sql = @"
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
                modified_date = NOW(),
                modified_by = @modified_by
                WHERE id = @id;
           ";

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
                new { Id = updateUser.id },
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
                new { Id = updateUser.id, HashedPassword = hashedPassword },
                Transaction
            );

            return rows > 0;
        }

    }
}

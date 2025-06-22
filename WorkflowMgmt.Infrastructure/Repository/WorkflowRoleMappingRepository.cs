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
    public class WorkflowRoleMappingRepository : RepositoryTranBase, IWorkflowRoleMappingRepository
    {
        public WorkflowRoleMappingRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<WorkflowRoleMappingDto>> GetDepartmentRoleUsers()
        {
            var sql = @"
                SELECT 
                    wrm.id,
                    wrm.department_id,
                    wrm.role_id,
                    wrm.user_id,
                    wrm.isprimary,
                    d.name as department_name,
                    r.name as role_name,
                    r.code as role_code,
                    CONCAT(u.first_name, ' ', u.last_name) as user_name,
                    u.email as user_email
                FROM workflowmgmt.workflow_role_mapping wrm
                INNER JOIN workflowmgmt.departments d ON wrm.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON wrm.role_id = r.id
                INNER JOIN workflowmgmt.users u ON wrm.user_id = u.id
                WHERE d.is_active = true AND r.is_active = true AND u.is_active = true
                ORDER BY d.name, r.name, wrm.isprimary DESC, u.first_name";

            var mappings = await Connection.QueryAsync<WorkflowRoleMappingDto>(sql, Transaction);
            return mappings.ToList();
        }

        public async Task<List<WorkflowRoleMappingDto>> GetDepartmentRoleUsersByDepartment(int departmentId)
        {
            var sql = @"
                SELECT 
                    wrm.id,
                    wrm.department_id,
                    wrm.role_id,
                    wrm.user_id,
                    wrm.isprimary,
                    d.name as department_name,
                    r.name as role_name,
                    r.code as role_code,
                    CONCAT(u.first_name, ' ', u.last_name) as user_name,
                    u.email as user_email
                FROM workflowmgmt.workflow_role_mapping wrm
                INNER JOIN workflowmgmt.departments d ON wrm.department_id = d.id
                INNER JOIN workflowmgmt.roles r ON wrm.role_id = r.id
                INNER JOIN workflowmgmt.users u ON wrm.user_id = u.id
                WHERE wrm.department_id = @DepartmentId 
                AND d.is_active = true AND r.is_active = true AND u.is_active = true
                ORDER BY r.name, wrm.isprimary DESC, u.first_name";

            var mappings = await Connection.QueryAsync<WorkflowRoleMappingDto>(sql, new { DepartmentId = departmentId }, Transaction);
            return mappings.ToList();
        }

        public async Task<bool> UpdateDepartmentRoleUsers(int departmentId, List<DepartmentRoleUserAssignmentDto> assignments)
        {
            try
            {
                // First, delete existing mappings for this department
                var deleteSql = "DELETE FROM workflowmgmt.workflow_role_mapping WHERE department_id = @DepartmentId";
                await Connection.ExecuteAsync(deleteSql, new { DepartmentId = departmentId }, Transaction);

                if (assignments.Any())
                {
                    // Validate that each role has only one primary user
                    var roleGroups = assignments.GroupBy(a => a.role_id);
                    foreach (var roleGroup in roleGroups)
                    {
                        var primaryCount = roleGroup.Count(a => a.is_primary);
                        if (primaryCount > 1)
                        {
                            throw new InvalidOperationException($"Role ID {roleGroup.Key} cannot have more than one primary user.");
                        }
                    }

                    // Insert new mappings
                    var insertSql = @"
                        INSERT INTO workflowmgmt.workflow_role_mapping 
                        (department_id, role_id, user_id, isprimary)
                        VALUES (@DepartmentId, @RoleId, @UserId, @IsPrimary)";

                    var parameters = assignments.Select(a => new
                    {
                        DepartmentId = departmentId,
                        RoleId = a.role_id,
                        UserId = a.user_id,
                        IsPrimary = a.is_primary
                    });

                    var rowsAffected = await Connection.ExecuteAsync(insertSql, parameters, Transaction);
                    return rowsAffected > 0;
                }

                return true; // Successfully deleted all mappings
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ValidateUniqueRolePrimary(int departmentId, int roleId, Guid userId)
        {
            var sql = @"
                SELECT COUNT(*) 
                FROM workflowmgmt.workflow_role_mapping 
                WHERE department_id = @DepartmentId 
                AND role_id = @RoleId 
                AND isprimary = true 
                AND user_id != @UserId";

            var count = await Connection.ExecuteScalarAsync<int>(sql, new 
            { 
                DepartmentId = departmentId, 
                RoleId = roleId, 
                UserId = userId 
            }, Transaction);

            return count == 0; // Returns true if no other primary user exists for this role
        }
    }
}

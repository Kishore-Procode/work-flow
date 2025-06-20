using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class WorkflowStageDetailsRepository : RepositoryTranBase, IWorkflowStageDetailsRepository
    {
        public WorkflowStageDetailsRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<WorkflowStageDetailsDto>> GetByWorkflowTemplateIdAsync(Guid workflowTemplateId)
        {
            var sql = @"
                SELECT 
                    stage_id as StageId,
                    workflow_template_id as WorkflowTemplateId,
                    stage_name as StageName,
                    stage_order as StageOrder,
                    assigned_role as AssignedRole,
                    description as Description,
                    is_required as IsRequired,
                    auto_approve as AutoApprove,
                    timeout_days as TimeoutDays,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    required_roles as RequiredRolesJson,
                    stage_permissions as StagePermissionsJson,
                    actions as ActionsJson
                FROM workflowmgmt.workflow_stage_details
                WHERE workflow_template_id = @WorkflowTemplateId
                ORDER BY stage_order";

            var results = await Connection.QueryAsync<dynamic>(sql, new { WorkflowTemplateId = workflowTemplateId }, transaction: Transaction);

            return results.Select(result => new WorkflowStageDetailsDto
            {
                StageId = result.StageId,
                WorkflowTemplateId = result.WorkflowTemplateId,
                StageName = result.StageName,
                StageOrder = result.StageOrder,
                AssignedRole = result.AssignedRole,
                Description = result.Description,
                IsRequired = result.IsRequired,
                AutoApprove = result.AutoApprove,
                TimeoutDays = result.TimeoutDays,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                ModifiedDate = result.ModifiedDate,
                RequiredRoles = ParseJsonArray<WorkflowStageRoleDto>(result.RequiredRolesJson),
                StagePermissions = ParseJsonArray<WorkflowStagePermissionDto>(result.StagePermissionsJson),
                Actions = ParseJsonArray<WorkflowStageActionDto>(result.ActionsJson)
            });
        }

        public async Task<WorkflowStageDetailsDto?> GetByStageIdAsync(Guid stageId)
        {
            var sql = @"
                SELECT 
                    stage_id as StageId,
                    workflow_template_id as WorkflowTemplateId,
                    stage_name as StageName,
                    stage_order as StageOrder,
                    assigned_role as AssignedRole,
                    description as Description,
                    is_required as IsRequired,
                    auto_approve as AutoApprove,
                    timeout_days as TimeoutDays,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    required_roles as RequiredRolesJson,
                    stage_permissions as StagePermissionsJson,
                    actions as ActionsJson
                FROM workflowmgmt.workflow_stage_details
                WHERE stage_id = @StageId";

            var result = await Connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { StageId = stageId }, transaction: Transaction);

            if (result == null)
                return null;

            return new WorkflowStageDetailsDto
            {
                StageId = result.StageId,
                WorkflowTemplateId = result.WorkflowTemplateId,
                StageName = result.StageName,
                StageOrder = result.StageOrder,
                AssignedRole = result.AssignedRole,
                Description = result.Description,
                IsRequired = result.IsRequired,
                AutoApprove = result.AutoApprove,
                TimeoutDays = result.TimeoutDays,
                IsActive = result.IsActive,
                CreatedDate = result.CreatedDate,
                ModifiedDate = result.ModifiedDate,
                RequiredRoles = ParseJsonArray<WorkflowStageRoleDto>(result.RequiredRolesJson),
                StagePermissions = ParseJsonArray<WorkflowStagePermissionDto>(result.StagePermissionsJson),
                Actions = ParseJsonArray<WorkflowStageActionDto>(result.ActionsJson)
            };
        }

        public async Task<IEnumerable<RoleOptionDto>> GetActiveRolesAsync()
        {
            var sql = @"
                SELECT 
                    code as Code,
                    name as Name,
                    description as Description
                FROM workflowmgmt.roles
                WHERE is_active = true
                ORDER BY name";

            return await Connection.QueryAsync<RoleOptionDto>(sql, transaction: Transaction);
        }

        private List<T> ParseJsonArray<T>(string? jsonString)
        {
            if (string.IsNullOrEmpty(jsonString) || jsonString == "[]")
                return new List<T>();

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(jsonString) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }
    }
}

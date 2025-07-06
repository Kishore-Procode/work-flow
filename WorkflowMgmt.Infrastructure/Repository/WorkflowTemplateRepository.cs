using Dapper;
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
    public class WorkflowTemplateRepository : RepositoryTranBase, IWorkflowTemplateRepository
    {
        public WorkflowTemplateRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<WorkflowTemplateWithStagesDto>> GetAllAsync()
        {
            // First get all templates with counts
            var templateSql = @"
                SELECT
                    wt.id as Id,
                    wt.name as Name,
                    wt.description as Description,
                    wt.document_type as DocumentType,
                    wt.is_active as IsActive,
                    wt.created_date as CreatedDate,
                    wt.modified_date as ModifiedDate,
                    wt.created_by as CreatedBy,
                    wt.modified_by as ModifiedBy,
                    COALESCE(stage_counts.stage_count, 0) as StageCount,
                    COALESCE(action_counts.action_count, 0) as ActionCount,
                    COALESCE(timed_counts.timed_count, 0) as TimedCount
                FROM workflowmgmt.workflow_templates wt
                LEFT JOIN (
                    SELECT workflow_template_id, COUNT(*) as stage_count
                    FROM workflowmgmt.workflow_stages
                    WHERE is_active = true
                    GROUP BY workflow_template_id
                ) stage_counts ON wt.id = stage_counts.workflow_template_id
                LEFT JOIN (
                    SELECT ws.workflow_template_id, COUNT(wsa.id) as action_count
                    FROM workflowmgmt.workflow_stages ws
                    LEFT JOIN workflowmgmt.workflow_stage_actions wsa ON ws.id = wsa.workflow_stage_id AND wsa.is_active = true
                    WHERE ws.is_active = true
                    GROUP BY ws.workflow_template_id
                ) action_counts ON wt.id = action_counts.workflow_template_id
                LEFT JOIN (
                    SELECT workflow_template_id, COUNT(*) as timed_count
                    FROM workflowmgmt.workflow_stages
                    WHERE is_active = true AND timeout_days IS NOT NULL AND timeout_days > 0
                    GROUP BY workflow_template_id
                ) timed_counts ON wt.id = timed_counts.workflow_template_id
                ORDER BY wt.name";

            var templates = await Connection.QueryAsync<WorkflowTemplateWithStagesDto>(templateSql, transaction: Transaction);
            var templateList = templates.ToList();

            // Get stages for all templates
            if (templateList.Any())
            {
                var templateIds = templateList.Select(t => t.Id).ToArray();
                var stagesSql = @"
                    SELECT
                        ws.workflow_template_id as TemplateId,
                        ws.id as Id,
                        ws.stage_name as StageName,
                        ws.stage_order as StageOrder,
                        ws.assigned_role as AssignedRole,
                        ws.timeout_days as TimeoutDays,
                        ws.is_required as IsRequired
                    FROM workflowmgmt.workflow_stages ws
                    WHERE ws.workflow_template_id = ANY(@TemplateIds) AND ws.is_active = true
                    ORDER BY ws.workflow_template_id, ws.stage_order";

                var stages = await Connection.QueryAsync<dynamic>(stagesSql, new { TemplateIds = templateIds }, transaction: Transaction);

                // Group stages by template
                var stagesByTemplate = stages.GroupBy(s => (Guid)s.templateid).ToDictionary(g => g.Key, g => g.ToList());

                // Assign stages to templates
                foreach (var template in templateList)
                {
                    template.Stages = new List<WorkflowStageDto>();
                    if (stagesByTemplate.TryGetValue(template.Id, out var templateStages))
                    {
                        foreach (var stage in templateStages)
                        {
                            template.Stages.Add(new WorkflowStageDto
                            {
                                Id = stage.id,
                                StageName = stage.stagename,
                                StageOrder = stage.stageorder,
                                AssignedRole = stage.assignedrole,
                                TimeoutDays = stage.timeoutdays,
                                IsRequired = stage.isrequired,
                                Actions = new List<WorkflowStageActionDto>(),
                                RequiredRoles = new List<WorkflowStageRoleDto>()
                            });
                        }
                    }
                }
            }

            return templateList;
        }

        public async Task<List<WorkflowTemplateDto>> GetByDocumentTypeAsync(string documentType)
        {
            var sql = @"
                SELECT
                    id as Id,
                    name as Name,
                    description as Description,
                    document_type as DocumentType,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.workflow_templates
                WHERE document_type = @DocumentType
                ORDER BY name";

            var result = await Connection.QueryAsync<WorkflowTemplateDto>(sql, new { DocumentType = documentType }, transaction: Transaction);
            return result.ToList();
        }

        public async Task<List<WorkflowTemplateDto>> GetActiveAsync()
        {
            var sql = @"
                SELECT
                    id as Id,
                    name as Name,
                    description as Description,
                    document_type as DocumentType,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.workflow_templates
                WHERE is_active = true
                ORDER BY name";

            var result = await Connection.QueryAsync<WorkflowTemplateDto>(sql, transaction: Transaction);
            return result.ToList();
        }

        public async Task<WorkflowTemplateWithStagesDto?> GetByIdAsync(Guid id)
        {
            // First get the template with stages and actions
            var templateSql = @"
                SELECT
                    wt.id as Id,
                    wt.name as Name,
                    wt.description as Description,
                    wt.document_type as DocumentType,
                    wt.is_active as IsActive,
                    wt.created_date as CreatedDate,
                    wt.modified_date as ModifiedDate,
                    wt.created_by as CreatedBy,
                    wt.modified_by as ModifiedBy,

                    ws.id as Id,
                    ws.workflow_template_id as WorkflowTemplateId,
                    ws.stage_name as StageName,
                    ws.stage_order as StageOrder,
                    ws.assigned_role as AssignedRole,
                    ws.description as Description,
                    ws.is_required as IsRequired,
                    ws.auto_approve as AutoApprove,
                    ws.timeout_days as TimeoutDays,
                    ws.is_active as IsActive,
                    ws.created_date as CreatedDate,
                    ws.modified_date as ModifiedDate,
                    ws.created_by as CreatedBy,
                    ws.modified_by as ModifiedBy,

                    wsa.id as Id,
                    wsa.workflow_stage_id as WorkflowStageId,
                    wsa.action_name as ActionName,
                    wsa.action_type as ActionType,
                    wsa.next_stage_id as NextStageId,
                    wsa.is_active as IsActive,
                    wsa.created_date as CreatedDate
                FROM workflowmgmt.workflow_templates wt
                LEFT JOIN workflowmgmt.workflow_stages ws ON wt.id = ws.workflow_template_id AND ws.is_active = true
                LEFT JOIN workflowmgmt.workflow_stage_actions wsa ON ws.id = wsa.workflow_stage_id AND wsa.is_active = true
                WHERE wt.id = @Id
                ORDER BY ws.stage_order, wsa.action_name";

            var templateDict = new Dictionary<Guid, WorkflowTemplateWithStagesDto>();
            var stageDict = new Dictionary<Guid, WorkflowStageDto>();

            await Connection.QueryAsync<WorkflowTemplateWithStagesDto, WorkflowStageDto, WorkflowStageActionDto, WorkflowTemplateWithStagesDto>(
                templateSql,
                (template, stage, action) =>
                {
                    if (!templateDict.TryGetValue(template.Id, out var templateEntry))
                    {
                        templateEntry = template;
                        templateEntry.Stages = new List<WorkflowStageDto>();
                        templateDict.Add(template.Id, templateEntry);
                    }

                    if (stage != null)
                    {
                        if (!stageDict.TryGetValue(stage.Id, out var stageEntry))
                        {
                            stageEntry = stage;
                            stageEntry.Actions = new List<WorkflowStageActionDto>();
                            stageEntry.RequiredRoles = new List<WorkflowStageRoleDto>();
                            stageDict.Add(stage.Id, stageEntry);
                            templateEntry.Stages.Add(stageEntry);
                        }

                        if (action != null)
                        {
                            stageEntry.Actions.Add(action);
                        }
                    }

                    return templateEntry;
                },
                new { Id = id },
                transaction: Transaction,
                splitOn: "Id,Id");

            var result = templateDict.Values.FirstOrDefault();

            // Now fetch required roles for each stage
            if (result != null && result.Stages.Any())
            {
                var stageIds = result.Stages.Select(s => s.Id).ToArray();
                var rolesSql = @"
                    SELECT
                        wsr.workflow_stage_id as StageId,
                        wsr.role_code as RoleCode,
                        r.name as RoleName,
                        wsr.is_required as IsRequired
                    FROM workflowmgmt.workflow_stage_roles wsr
                    LEFT JOIN workflowmgmt.roles r ON wsr.role_code = r.code
                    WHERE wsr.workflow_stage_id = ANY(@StageIds)
                    ORDER BY wsr.role_code";

                var stageRoles = await Connection.QueryAsync<dynamic>(rolesSql, new { StageIds = stageIds }, transaction: Transaction);

                foreach (var stageRole in stageRoles)
                {
                    var stage = result.Stages.FirstOrDefault(s => s.Id == (Guid)stageRole.stageid);
                    if (stage != null)
                    {
                        stage.RequiredRoles.Add(new WorkflowStageRoleDto
                        {
                            RoleCode = stageRole.rolecode,
                            RoleName = stageRole.rolename,
                            IsRequired = stageRole.isrequired
                        });
                    }
                }
            }

            return result;
        }

        public async Task<WorkflowTemplateDto?> GetByIdSimpleAsync(Guid id)
        {
            var sql = @"
                SELECT
                    wt.id as Id,
                    wt.name as Name,
                    wt.description as Description,
                    wt.document_type as DocumentType,
                    wt.is_active as IsActive,
                    wt.created_date as CreatedDate,
                    wt.modified_date as ModifiedDate,
                    wt.created_by as CreatedBy,
                    wt.modified_by as ModifiedBy,
                    COALESCE(stage_counts.stage_count, 0) as StageCount,
                    COALESCE(action_counts.action_count, 0) as ActionCount,
                    COALESCE(timed_counts.timed_count, 0) as TimedCount
                FROM workflowmgmt.workflow_templates wt
                LEFT JOIN (
                    SELECT workflow_template_id, COUNT(*) as stage_count
                    FROM workflowmgmt.workflow_stages
                    WHERE is_active = true
                    GROUP BY workflow_template_id
                ) stage_counts ON wt.id = stage_counts.workflow_template_id
                LEFT JOIN (
                    SELECT ws.workflow_template_id, COUNT(wsa.id) as action_count
                    FROM workflowmgmt.workflow_stages ws
                    LEFT JOIN workflowmgmt.workflow_stage_actions wsa ON ws.id = wsa.workflow_stage_id AND wsa.is_active = true
                    WHERE ws.is_active = true
                    GROUP BY ws.workflow_template_id
                ) action_counts ON wt.id = action_counts.workflow_template_id
                LEFT JOIN (
                    SELECT workflow_template_id, COUNT(*) as timed_count
                    FROM workflowmgmt.workflow_stages
                    WHERE is_active = true AND timeout_days IS NOT NULL AND timeout_days > 0
                    GROUP BY workflow_template_id
                ) timed_counts ON wt.id = timed_counts.workflow_template_id
                WHERE wt.id = @Id";

            return await Connection.QuerySingleOrDefaultAsync<WorkflowTemplateDto>(sql, new { Id = id }, transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(CreateWorkflowTemplateDto template)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.workflow_templates 
                (id, name, description, document_type, is_active, created_date, created_by)
                VALUES (@Id, @Name, @Description, @DocumentType, true, @CreatedDate, @CreatedBy)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                template.Name,
                template.Description,
                template.DocumentType,
                CreatedDate = DateTime.Now,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateWorkflowTemplateDto template)
        {
            var sql = @"
                UPDATE workflowmgmt.workflow_templates 
                SET name = @Name,
                    description = @Description,
                    document_type = @DocumentType,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                template.Name,
                template.Description,
                template.DocumentType,
                ModifiedDate = DateTime.Now,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM workflowmgmt.workflow_templates WHERE id = @Id";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.workflow_templates 
                SET is_active = NOT is_active,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedDate = DateTime.Now,
                ModifiedBy = "system" // TODO: Get from current user context
            }, Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.workflow_templates WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByNameAndDocumentTypeAsync(string name, string documentType, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.workflow_templates 
                WHERE name = @Name 
                AND document_type = @DocumentType";

            object parameters = new { Name = name, DocumentType = documentType };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Name = name, DocumentType = documentType, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }
    }
}

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

        public async Task<IEnumerable<WorkflowTemplateDto>> GetAllAsync()
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
                ORDER BY name";

            return await Connection.QueryAsync<WorkflowTemplateDto>(sql, transaction: Transaction);
        }

        public async Task<IEnumerable<WorkflowTemplateDto>> GetByDocumentTypeAsync(string documentType)
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

            return await Connection.QueryAsync<WorkflowTemplateDto>(sql, new { DocumentType = documentType }, transaction: Transaction);
        }

        public async Task<IEnumerable<WorkflowTemplateDto>> GetActiveAsync()
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

            return await Connection.QueryAsync<WorkflowTemplateDto>(sql, transaction: Transaction);
        }

        public async Task<WorkflowTemplateWithStagesDto?> GetByIdAsync(Guid id)
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
                sql,
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

            return templateDict.Values.FirstOrDefault();
        }

        public async Task<WorkflowTemplateDto?> GetByIdSimpleAsync(Guid id)
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
                WHERE id = @Id";

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
                CreatedDate = DateTime.UtcNow,
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
                ModifiedDate = DateTime.UtcNow,
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
                ModifiedDate = DateTime.UtcNow,
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

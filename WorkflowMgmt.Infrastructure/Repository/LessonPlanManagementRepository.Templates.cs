using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WorkflowMgmt.Domain.Models.LessonPlan;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public partial class LessonPlanManagementRepository
    {
        // Template operations
        public async Task<IEnumerable<LessonPlanTemplateDto>> GetAllTemplatesAsync()
        {
            var sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    description as Description,
                    template_type as TemplateType,
                    duration_minutes as DurationMinutes,
                    sections as Sections,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.lesson_plan_templates 
                ORDER BY name";

            return await Connection.QueryAsync<LessonPlanTemplateDto>(sql, transaction: Transaction);
        }

        public async Task<LessonPlanTemplateDto?> GetTemplateByIdAsync(Guid id)
        {
            var sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    description as Description,
                    template_type as TemplateType,
                    duration_minutes as DurationMinutes,
                    sections as Sections,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.lesson_plan_templates 
                WHERE id = @Id";

            return await Connection.QueryFirstOrDefaultAsync<LessonPlanTemplateDto>(sql, new { Id = id }, Transaction);
        }

        public async Task<IEnumerable<LessonPlanTemplateDto>> GetActiveTemplatesAsync()
        {
            var sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    description as Description,
                    template_type as TemplateType,
                    duration_minutes as DurationMinutes,
                    sections as Sections,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.lesson_plan_templates 
                WHERE is_active = true 
                ORDER BY name";

            return await Connection.QueryAsync<LessonPlanTemplateDto>(sql, transaction: Transaction);
        }

        public async Task<IEnumerable<LessonPlanTemplateDto>> GetTemplatesByTypeAsync(string templateType)
        {
            var sql = @"
                SELECT 
                    id as Id,
                    name as Name,
                    description as Description,
                    template_type as TemplateType,
                    duration_minutes as DurationMinutes,
                    sections as Sections,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.lesson_plan_templates 
                WHERE template_type = @TemplateType AND is_active = true 
                ORDER BY name";

            return await Connection.QueryAsync<LessonPlanTemplateDto>(sql, new { TemplateType = templateType }, Transaction);
        }

        public async Task<Guid> CreateTemplateAsync(CreateLessonPlanTemplateDto template)
        {
            var sql = @"
                INSERT INTO workflowmgmt.lesson_plan_templates (
                    name,
                    description,
                    template_type,
                    duration_minutes,
                    sections,
                    is_active,
                    created_date,
                    created_by
                ) VALUES (
                    @Name,
                    @Description,
                    @TemplateType,
                    @DurationMinutes,
                    @Sections::jsonb,
                    true,
                    NOW(),
                    @CreatedBy
                )
                RETURNING id;
            ";

            return await Connection.ExecuteScalarAsync<Guid>(sql, template, Transaction);
        }

        public async Task<bool> UpdateTemplateAsync(Guid id, UpdateLessonPlanTemplateDto template)
        {
            var setParts = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("ModifiedDate", DateTime.UtcNow);
            parameters.Add("ModifiedBy", template.ModifiedBy);

            if (!string.IsNullOrEmpty(template.Name))
            {
                setParts.Add("name = @Name");
                parameters.Add("Name", template.Name);
            }

            if (template.Description != null)
            {
                setParts.Add("description = @Description");
                parameters.Add("Description", template.Description);
            }

            if (!string.IsNullOrEmpty(template.TemplateType))
            {
                setParts.Add("template_type = @TemplateType");
                parameters.Add("TemplateType", template.TemplateType);
            }

            if (template.DurationMinutes.HasValue)
            {
                setParts.Add("duration_minutes = @DurationMinutes");
                parameters.Add("DurationMinutes", template.DurationMinutes);
            }

            if (template.Sections != null)
            {
                setParts.Add("sections = @Sections::jsonb");
                parameters.Add("Sections", template.Sections);
            }

            if (!setParts.Any())
            {
                return false; // Nothing to update
            }

            setParts.Add("modified_date = @ModifiedDate");
            setParts.Add("modified_by = @ModifiedBy");

            var sql = $@"
                UPDATE workflowmgmt.lesson_plan_templates 
                SET {string.Join(", ", setParts)}
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, parameters, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteTemplateAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.lesson_plan_templates 
                SET is_active = false, modified_date = NOW()
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleTemplateActiveAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.lesson_plan_templates 
                SET is_active = NOT is_active, modified_date = NOW()
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> TemplateExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM workflowmgmt.lesson_plan_templates
                WHERE name = @Name AND is_active = true";

            object parameters;

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Name = name, ExcludeId = excludeId.Value };
            }
            else
            {
                parameters = new { Name = name };
            }

            var count = await Connection.ExecuteScalarAsync<int>(sql, parameters, Transaction);
            return count > 0;
        }
    }
}

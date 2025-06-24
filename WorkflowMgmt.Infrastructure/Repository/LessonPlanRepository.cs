using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using WorkflowMgmt.Domain.Entities.LessonPlanTemplate;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class LessonPlanTemplateRepository : RepositoryTranBase, ILessonPlanTemplateRepository
    {
        public LessonPlanTemplateRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<LessonPlanTemplateDto>> GetAllLessonPlanTemplates()
        {
            var templates = await Connection.QueryAsync<LessonPlanTemplateDto>(
                "SELECT * FROM workflowmgmt.lesson_plan_templates ORDER BY name", Transaction);

            return templates.ToList();
        }

        public async Task<LessonPlanTemplateDto?> GetLessonPlanTemplateById(Guid id)
        {
            var sql = "SELECT * FROM workflowmgmt.lesson_plan_templates WHERE id = @Id";
            var template = await Connection.QueryFirstOrDefaultAsync<LessonPlanTemplateDto>(sql, new { Id = id }, Transaction);
            return template;
        }

        public async Task<Guid> InsertLessonPlanTemplate(LessonPlanTemplateDto template)
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
                    @name,
                    @description,
                    @template_type,
                    @duration_minutes,
                    @sections::jsonb,
                    @is_active,
                    NOW(),
                    @CreatedBy
                )
                RETURNING id;
            ";

            var templateId = await Connection.ExecuteScalarAsync<Guid>(sql, template, Transaction);
            return templateId;
        }

        public async Task<bool> UpdateLessonPlanTemplate(LessonPlanTemplateDto template)
        {
            var sql = @"
                UPDATE workflowmgmt.lesson_plan_templates SET
                name = @name,
                description = @description,
                template_type = @template_type,
                duration_minutes = @duration_minutes,
                sections = @sections::jsonb,
                is_active = @is_active,
                modified_date = NOW(),
                modified_by = @ModifiedBy
                WHERE id = @id;
           ";

            var rowsAffected = await Connection.ExecuteAsync(sql, template, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteOrRestoreLessonPlanTemplate(Guid id, string modifiedBy, bool isRestore)
        {
            var sql = @"
        UPDATE workflowmgmt.lesson_plan_templates
        SET is_active = @IsActive,
            modified_by = @ModifiedBy,
            modified_date = NOW()
        WHERE id = @Id AND is_active != @IsActive";

            var result = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedBy = modifiedBy,
                IsActive = isRestore
            }, Transaction);

            return result > 0;
        }
    }
}

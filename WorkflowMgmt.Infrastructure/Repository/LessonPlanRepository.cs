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
            var sql = "SELECT * FROM workflowmgmt.lesson_plan_templates WHERE is_active = true";
            var templates = await Connection.QueryAsync<LessonPlanTemplateDto>(sql, transaction: Transaction);
            return templates.ToList();
        }

        public async Task<LessonPlanTemplateDto?> GetLessonPlanTemplateById(Guid id)
        {
            var sql = "SELECT * FROM workflowmgmt.lesson_plan_templates WHERE id = @Id AND is_active = true";
            return await Connection.QueryFirstOrDefaultAsync<LessonPlanTemplateDto>(sql, new { Id = id }, Transaction);
        }

        public async Task<Guid> InsertLessonPlanTemplate(LessonPlanTemplateDto template)
        {
            var sql = @"
                INSERT INTO workflowmgmt.lesson_plan_templates (
                    id,
                    name,
                    description,
                    template_type,
                    duration_minutes,
                    sections,
                    is_active,
                    created_date,
                    created_by
                ) VALUES (
                    @Id,
                    @Name,
                    @Description,
                    @TemplateType,
                    @DurationMinutes,
                    @Sections::jsonb,
                    @IsActive,
                    NOW(),
                    @CreatedBy
                ) RETURNING id;
            ";

            template.Id = Guid.NewGuid();
            var parameters = new
            {
                template.Id,
                template.Name,
                template.Description,
                template.TemplateType,
                template.DurationMinutes,
                Sections = template.Sections?.ToJsonString(),
                template.IsActive,
                template.CreatedBy
            };

            return await Connection.ExecuteScalarAsync<Guid>(sql, parameters, Transaction);
        }

        public async Task<bool> UpdateLessonPlanTemplate(LessonPlanTemplateDto template)
        {
            var sql = @"
                UPDATE workflowmgmt.lesson_plan_templates SET
                    name = @Name,
                    description = @Description,
                    template_type = @TemplateType,
                    duration_minutes = @DurationMinutes,
                    sections = @Sections::jsonb,
                    is_active = @IsActive,
                    modified_date = NOW(),
                    modified_by = @ModifiedBy
                WHERE id = @Id;
            ";

            var parameters = new
            {
                template.Id,
                template.Name,
                template.Description,
                template.TemplateType,
                template.DurationMinutes,
                Sections = template.Sections?.ToJsonString(),
                template.IsActive,
                template.ModifiedBy
            };

            var affectedRows = await Connection.ExecuteAsync(sql, parameters, Transaction);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteOrRestoreLessonPlanTemplate(Guid id, string modifiedBy, bool isRestore)
        {
            var sql = @"
                UPDATE workflowmgmt.lesson_plan_templates
                SET is_active = @IsActive,
                    modified_by = @ModifiedBy,
                    modified_date = NOW()
                WHERE id = @Id AND is_active != @IsActive;
            ";

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

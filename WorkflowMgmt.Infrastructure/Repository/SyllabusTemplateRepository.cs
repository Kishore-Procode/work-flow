using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WorkflowMgmt.Domain.Entities.SyllabusTemplate;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class SyllabusTemplateRepository : RepositoryTranBase, ISyllabusTemplateRepository
    {
        public SyllabusTemplateRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<SyllabusTemplateDto>> GetAllSyllabusTemplates()
        {
            var sql = "SELECT * FROM workflowmgmt.syllabus_templates WHERE is_active = true";
            var templates = await Connection.QueryAsync<SyllabusTemplateDto>(sql, transaction: Transaction);
            return templates.ToList();
        }

        public async Task<SyllabusTemplateDto?> GetSyllabusTemplateById(Guid id)
        {
            var sql = "SELECT * FROM workflowmgmt.syllabus_templates WHERE id = @Id AND is_active = true";
            return await Connection.QueryFirstOrDefaultAsync<SyllabusTemplateDto>(sql, new { Id = id }, Transaction);
        }

        public async Task<Guid> InsertSyllabusTemplate(SyllabusTemplateDto template)
        {
            var sql = @"
                INSERT INTO workflowmgmt.syllabus_templates (
                    id,
                    name,
                    description,
                    template_type,
                    sections,
                    is_active,
                    created_date,
                    created_by
                ) VALUES (
                    @Id,
                    @Name,
                    @Description,
                    @TemplateType,
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
                Sections = template.Sections?.ToJsonString(),
                template.IsActive,
                template.CreatedBy
            };

            return await Connection.ExecuteScalarAsync<Guid>(sql, parameters, Transaction);
        }

        public async Task<bool> UpdateSyllabusTemplate(SyllabusTemplateDto template)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabus_templates SET
                    name = @Name,
                    description = @Description,
                    template_type = @TemplateType,
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
                Sections = template.Sections?.ToJsonString(),
                template.IsActive,
                template.ModifiedBy
            };

            var affectedRows = await Connection.ExecuteAsync(sql, parameters, Transaction);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteOrRestoreSyllabusTemplate(Guid id, string modifiedBy, bool isRestore)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabus_templates
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

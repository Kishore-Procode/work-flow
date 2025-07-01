using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Syllabus;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class SyllabusTemplateManagementRepository : RepositoryTranBase, ISyllabusTemplateRepository
    {
        public SyllabusTemplateManagementRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<SyllabusTemplateDto>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    id as Id,
                    name as Name,
                    description as Description,
                    template_type as TemplateType,
                    sections as Sections,
                    html_form_template as HtmlFormTemplate,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.syllabus_templates
                ORDER BY name";

            return await Connection.QueryAsync<SyllabusTemplateDto>(sql, transaction: Transaction);
        }

        public async Task<SyllabusTemplateDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT
                    id as Id,
                    name as Name,
                    description as Description,
                    template_type as TemplateType,
                    sections as Sections,
                    html_form_template as HtmlFormTemplate,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.syllabus_templates
                WHERE id = @Id";

            return await Connection.QuerySingleOrDefaultAsync<SyllabusTemplateDto>(sql, new { Id = id }, transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(CreateSyllabusTemplateDto template)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.syllabus_templates
                (id, name, description, template_type, sections, html_form_template, is_active, created_date, created_by)
                VALUES (@Id, @Name, @Description, @TemplateType, cast( @Sections as json), @HtmlFormTemplate, true, @CreatedDate, @CreatedBy)";

            var res = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                template.Name,
                template.Description,
                template.TemplateType,
                template.Sections,
                template.HtmlFormTemplate,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateSyllabusTemplateDto template)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabus_templates
                SET name = CASE WHEN @Name IS NOT NULL THEN @Name ELSE name END,
                    description = CASE WHEN @Description IS NOT NULL THEN @Description ELSE description END,
                    template_type = CASE WHEN @TemplateType IS NOT NULL THEN @TemplateType ELSE template_type END,
                    sections = CASE WHEN @Sections IS NOT NULL THEN cast(@Sections as jsonb) ELSE sections END,
                    html_form_template = CASE WHEN @HtmlFormTemplate IS NOT NULL THEN @HtmlFormTemplate ELSE html_form_template END,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                template.Name,
                template.Description,
                template.TemplateType,
                template.Sections,
                template.HtmlFormTemplate,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM workflowmgmt.syllabus_templates WHERE id = @Id";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabus_templates 
                SET is_active = NOT is_active,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<SyllabusTemplateDto>> GetActiveAsync()
        {
            var sql = @"
                SELECT
                    id as Id,
                    name as Name,
                    description as Description,
                    template_type as TemplateType,
                    sections as Sections,
                    html_form_template as HtmlFormTemplate,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.syllabus_templates
                WHERE is_active = true
                ORDER BY name";

            return await Connection.QueryAsync<SyllabusTemplateDto>(sql, transaction: Transaction);
        }

        public async Task<IEnumerable<SyllabusTemplateDto>> GetByTypeAsync(string templateType)
        {
            var sql = @"
                SELECT
                    id as Id,
                    name as Name,
                    description as Description,
                    template_type as TemplateType,
                    sections as Sections,
                    html_form_template as HtmlFormTemplate,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy
                FROM workflowmgmt.syllabus_templates
                WHERE template_type = @TemplateType
                ORDER BY name";

            return await Connection.QueryAsync<SyllabusTemplateDto>(sql, new { TemplateType = templateType }, transaction: Transaction);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.syllabus_templates WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.syllabus_templates 
                WHERE name = @Name";

            object parameters = new { Name = name };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Name = name, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByNameAndTypeAsync(string name, string templateType, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.syllabus_templates 
                WHERE name = @Name AND template_type = @TemplateType";

            object parameters = new { Name = name, TemplateType = templateType };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Name = name, TemplateType = templateType, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }
    }
}

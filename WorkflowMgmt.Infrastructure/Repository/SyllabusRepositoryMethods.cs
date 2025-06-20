using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Models.Syllabus;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public partial class SyllabusManagementRepository
    {
        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabi 
                SET status = @Status,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                Status = status,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateWorkflowIdAsync(Guid id, Guid workflowId)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabi 
                SET workflow_id = @WorkflowId,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                WorkflowId = workflowId,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateDocumentUrlAsync(Guid id, string documentUrl)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabi 
                SET document_url = @DocumentUrl,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                DocumentUrl = documentUrl,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateFileProcessingStatusAsync(Guid id, string status, string? notes = null)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabi 
                SET file_processing_status = @Status,
                    file_processing_notes = @Notes,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                Status = status,
                Notes = notes,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<SyllabusDocumentDto?> GetDocumentAsync(Guid id)
        {
            var sql = @"
                SELECT 
                    document_url as DocumentUrl,
                    original_filename as OriginalFilename
                FROM workflowmgmt.syllabi
                WHERE id = @Id AND document_url IS NOT NULL";

            var result = await Connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Id = id }, transaction: Transaction);
            
            if (result == null)
            {
                return null;
            }

            // TODO: Implement actual file retrieval from storage
            // For now, return a placeholder
            return new SyllabusDocumentDto
            {
                Content = System.Text.Encoding.UTF8.GetBytes("Document content placeholder"),
                ContentType = "application/pdf",
                FileName = result.OriginalFilename ?? "syllabus.pdf"
            };
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.syllabi WHERE id = @Id";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByTitleAndDepartmentAsync(string title, int departmentId, Guid? excludeId = null)
        {
            var sql = @"
                SELECT COUNT(1) 
                FROM workflowmgmt.syllabi 
                WHERE title = @Title AND department_id = @DepartmentId";

            object parameters = new { Title = title, DepartmentId = departmentId };

            if (excludeId.HasValue)
            {
                sql += " AND id != @ExcludeId";
                parameters = new { Title = title, DepartmentId = departmentId, ExcludeId = excludeId.Value };
            }

            var count = await Connection.QuerySingleAsync<int>(sql, parameters, transaction: Transaction);
            return count > 0;
        }

        public async Task<SyllabusStatsDto> GetStatsAsync()
        {
            var sql = @"
                SELECT 
                    COUNT(*) as Total,
                    COUNT(CASE WHEN status = 'Draft' THEN 1 END) as Draft,
                    COUNT(CASE WHEN status = 'Under Review' THEN 1 END) as UnderReview,
                    COUNT(CASE WHEN status = 'Approved' THEN 1 END) as Approved,
                    COUNT(CASE WHEN status = 'Rejected' THEN 1 END) as Rejected,
                    COUNT(CASE WHEN status = 'Published' THEN 1 END) as Published
                FROM workflowmgmt.syllabi
                WHERE is_active = true";

            var stats = await Connection.QuerySingleAsync<SyllabusStatsDto>(sql, transaction: Transaction);

            // Get syllabi by department
            var departmentsSql = @"
                SELECT 
                    d.id as DepartmentId,
                    d.name as DepartmentName,
                    COUNT(s.id) as Count
                FROM workflowmgmt.departments d
                LEFT JOIN workflowmgmt.syllabi s ON d.id = s.department_id AND s.is_active = true
                GROUP BY d.id, d.name
                ORDER BY d.name";

            var byDepartment = await Connection.QueryAsync<SyllabusByDepartmentDto>(departmentsSql, transaction: Transaction);
            stats.ByDepartment = byDepartment.ToArray();

            // Get syllabi by status
            var statusSql = @"
                SELECT 
                    status as Status,
                    COUNT(*) as Count
                FROM workflowmgmt.syllabi
                WHERE is_active = true
                GROUP BY status
                ORDER BY status";

            var byStatus = await Connection.QueryAsync<SyllabusByStatusDto>(statusSql, transaction: Transaction);
            stats.ByStatus = byStatus.ToArray();

            return stats;
        }
    }
}

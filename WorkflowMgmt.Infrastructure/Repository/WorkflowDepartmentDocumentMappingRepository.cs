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
    public class WorkflowDepartmentDocumentMappingRepository : RepositoryTranBase, IWorkflowDepartmentDocumentMappingRepository
    {
        public WorkflowDepartmentDocumentMappingRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<DocumentTypeWorkflowMappingDto>> GetDepartmentDocumentMappings(int departmentId)
        {
            // Define all possible document types
            var allDocumentTypes = new[] { "syllabus", "lesson", "session" };
            
            // Get existing mappings for the department
            var existingMappingsSql = @"
                SELECT 
                    wddm.document_type,
                    wddm.workflow_template_id,
                    wt.name as workflow_template_name
                FROM workflowmgmt.workflow_department_document_mapping wddm
                INNER JOIN workflowmgmt.workflow_templates wt ON wddm.workflow_template_id = wt.id
                WHERE wddm.department_id = @DepartmentId";

            var existingMappings = await Connection.QueryAsync<DocumentTypeWorkflowMappingDto>(
                existingMappingsSql, 
                new { DepartmentId = departmentId }, 
                Transaction);

            var existingMappingsList = existingMappings.ToList();
            var result = new List<DocumentTypeWorkflowMappingDto>();

            // Create result list with all document types
            foreach (var documentType in allDocumentTypes)
            {
                var existingMapping = existingMappingsList.FirstOrDefault(m => m.document_type == documentType);
                
                if (existingMapping != null)
                {
                    // Use existing mapping
                    result.Add(existingMapping);
                }
                else
                {
                    // Create entry with null workflow template
                    result.Add(new DocumentTypeWorkflowMappingDto
                    {
                        document_type = documentType,
                        workflow_template_id = null,
                        workflow_template_name = null
                    });
                }
            }

            return result.OrderBy(r => r.document_type).ToList();
        }

        public async Task<bool> UpdateDepartmentDocumentMappings(int departmentId, List<DepartmentDocumentMappingAssignmentDto> mappings)
        {
            try
            {
                // First, delete existing mappings for this department
                var deleteSql = "DELETE FROM workflowmgmt.workflow_department_document_mapping WHERE department_id = @DepartmentId";
                await Connection.ExecuteAsync(deleteSql, new { DepartmentId = departmentId }, Transaction);

                if (mappings.Any())
                {
                    // Validate document types
                    var validDocumentTypes = new[] { "syllabus", "lesson", "session" };
                    var invalidDocumentTypes = mappings.Where(m => !validDocumentTypes.Contains(m.document_type.ToLower())).ToList();
                    
                    if (invalidDocumentTypes.Any())
                    {
                        throw new ArgumentException($"Invalid document types: {string.Join(", ", invalidDocumentTypes.Select(m => m.document_type))}. Valid types are: syllabus, lesson, session");
                    }

                    // Validate that workflow templates exist and are active
                    var templateIds = mappings.Select(m => m.workflow_template_id).Distinct().ToList();
                    var templateCheckSql = @"
                        SELECT id FROM workflowmgmt.workflow_templates 
                        WHERE id = ANY(@TemplateIds)";
                    
                    var existingTemplateIds = await Connection.QueryAsync<Guid>(
                        templateCheckSql, 
                        new { TemplateIds = templateIds.ToArray() }, 
                        Transaction);

                    var missingTemplateIds = templateIds.Except(existingTemplateIds).ToList();
                    if (missingTemplateIds.Any())
                    {
                        throw new ArgumentException($"Invalid or inactive workflow template IDs: {string.Join(", ", missingTemplateIds)}");
                    }

                    // Insert new mappings
                    var insertSql = @"
                        INSERT INTO workflowmgmt.workflow_department_document_mapping 
                        (department_id, document_type, workflow_template_id)
                        VALUES (@DepartmentId, @DocumentType, @WorkflowTemplateId)";

                    var parameters = mappings.Select(m => new
                    {
                        DepartmentId = departmentId,
                        DocumentType = m.document_type.ToLower(),
                        WorkflowTemplateId = m.workflow_template_id
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
    }
}

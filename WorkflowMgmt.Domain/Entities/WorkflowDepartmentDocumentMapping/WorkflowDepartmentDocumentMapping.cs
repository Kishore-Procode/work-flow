using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities
{
    public class WorkflowDepartmentDocumentMappingDto
    {
        public int department_id { get; set; }
        public string document_type { get; set; } = string.Empty;
        public Guid workflow_template_id { get; set; }
        
        // Joined data
        public string? department_name { get; set; }
        public string? workflow_template_name { get; set; }
    }

    public class DocumentTypeWorkflowMappingDto
    {
        public string document_type { get; set; } = string.Empty;
        public Guid? workflow_template_id { get; set; }
        public string? workflow_template_name { get; set; }
    }

    public class DepartmentDocumentMappingAssignmentDto
    {
        public string document_type { get; set; } = string.Empty;
        public Guid workflow_template_id { get; set; }
    }

    public class UpdateDepartmentDocumentMappingRequest
    {
        public int department_id { get; set; }
        public List<DepartmentDocumentMappingAssignmentDto> mappings { get; set; } = new List<DepartmentDocumentMappingAssignmentDto>();
    }
}

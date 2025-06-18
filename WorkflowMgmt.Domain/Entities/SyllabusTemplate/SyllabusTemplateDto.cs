using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities.SyllabusTemplate
{
    public class SyllabusTemplateDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();              
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string TemplateType { get; set; } = string.Empty;

       
        public JsonNode Sections { get; set; } = JsonNode.Parse("[]")!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}

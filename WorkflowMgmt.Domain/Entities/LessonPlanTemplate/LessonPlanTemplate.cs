using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities.LessonPlanTemplate
{
    public class LessonPlanTemplateDto : BaseEntityGuid
    {
        public string name { get; set; } = string.Empty;
        public string? description { get; set; }
        public string template_type { get; set; } = string.Empty;
        public int duration_minutes { get; set; } = 60;
        public string sections { get; set; } = "[]"; // Store as JSON string to match database
        public bool is_active { get; set; } = true;
    }
}

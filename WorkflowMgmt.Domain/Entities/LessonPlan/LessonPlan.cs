using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities.LessonPlan
{
    public class LessonPlanDto : BaseEntityGuid
    {
        public string title { get; set; } = string.Empty;
        public Guid? syllabus_id { get; set; }
        public Guid template_id { get; set; }
        public string module_name { get; set; } = string.Empty;
        public int duration_minutes { get; set; } = 60;
        public int number_of_sessions { get; set; } = 1;
        public DateTime? scheduled_date { get; set; }
        public string faculty_name { get; set; } = string.Empty;
        public string content_creation_method { get; set; } = "form_entry";
        public string? lesson_description { get; set; }
        public string? learning_objectives { get; set; }
        public string? teaching_methods { get; set; }
        public string? learning_activities { get; set; }
        public string? detailed_content { get; set; }
        public string? resources { get; set; }
        public string? assessment_methods { get; set; }
        public string? prerequisites { get; set; }
        public string? document_url { get; set; }
        public string status { get; set; } = "Draft";
        public bool is_active { get; set; } = true;
        public string? file_processing_status { get; set; } = "not_applicable";
        public string? file_processing_notes { get; set; }
        public string? original_filename { get; set; }
    }
}

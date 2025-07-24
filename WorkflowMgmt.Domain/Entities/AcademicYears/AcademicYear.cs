using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities
{
    public class AcademicYearDTO : BaseEntity
    {
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public int start_year { get; set; }
        public int end_year { get; set; }
        public int level_id { get; set; }
        public string status { get; set; } = "Active";
        public string? description { get; set; }
        public bool is_active { get; set; } = true;
        
        // Navigation properties for display
        public string? level_name { get; set; }
        public string? level_code { get; set; }
    }

    public class AcademicYearStatsDto
    {
        public int TotalAcademicYears { get; set; }
        public int ActiveAcademicYears { get; set; }
        public int CurrentYearCount { get; set; }
    }

    public class AcademicYearByLevelDto
    {
        public int level_id { get; set; }
        public string level_name { get; set; } = string.Empty;
        public List<AcademicYearDTO> academic_years { get; set; } = new List<AcademicYearDTO>();
    }
}

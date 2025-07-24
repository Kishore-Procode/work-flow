using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities.Semesters
{
    public class SemesterDTO : BaseEntity
    {
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
        public int academic_year_id { get; set; }
        public int department_id { get; set; }
        public int[] course_id { get; set; } = new int[0];

        // Helper property for database mapping
        public string course_id_string
        {
            get => course_id != null && course_id.Length > 0 ? string.Join(",", course_id) : "";
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    course_id = new int[0];
                }
                else
                {
                    course_id = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(int.Parse)
                                    .ToArray();
                }
            }
        }

        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int duration_weeks { get; set; }
        public int level_id { get; set; }
        public int total_students { get; set; } = 0;
        public string status { get; set; } = "Upcoming";
        public string? description { get; set; }
        public bool exam_scheduled { get; set; } = false;
        public bool is_active { get; set; } = true;

        // Navigation properties for display
        public string? department_name { get; set; }
        public string? department_code { get; set; }
        public string? level_name { get; set; }
        public string? level_code { get; set; }
        public string? academic_year_name { get; set; }
        public string? academic_year_code { get; set; }
        public List<CourseInfo>? courses { get; set; } = new List<CourseInfo>();
    }

    public class CourseInfo
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
    }

    public class SemesterStatsDto
    {
        public int TotalSemesters { get; set; }
        public int ActiveSemesters { get; set; }
        public int UpcomingSemesters { get; set; }
        public int OngoingSemesters { get; set; }
        public int CompletedSemesters { get; set; }
    }
}

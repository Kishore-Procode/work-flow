using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Models.Stats
{
    public class CourseStatsDto
    {
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }
        public int InactiveCourses { get; set; }
        public int DraftCourses { get; set; }
        public int TotalCredits { get; set; }
        public decimal AverageCredits { get; set; }
        public Dictionary<string, int> CoursesByType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CoursesByLevel { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CoursesByDepartment { get; set; } = new Dictionary<string, int>();
    }
    public class CourseStatsQueryResultDto
    {
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }
        public int InactiveCourses { get; set; }
        public int DraftCourses { get; set; }
        public int TotalCredits { get; set; }
        public decimal AverageCredits { get; set; }
        public string? courses_by_type_json { get; set; }
        public string? courses_by_level_json { get; set; }
        public string? courses_by_department_json { get; set; }
    }
}

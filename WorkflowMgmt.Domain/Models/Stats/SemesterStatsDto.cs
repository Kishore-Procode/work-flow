using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Models.Stats
{
    public class SemesterStatsDto
    {
        public int TotalSemesters { get; set; }
        public int ActiveSemesters { get; set; }
        public int UpcomingSemesters { get; set; }
        public int OngoingSemesters { get; set; }
        public int CompletedSemesters { get; set; }
        public int TotalStudents { get; set; }
        public decimal AverageDurationWeeks { get; set; }
        public int SemestersWithExams { get; set; }
        public Dictionary<string, int> SemestersByStatus { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SemestersByLevel { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SemestersByDepartment { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> SemestersByAcademicYear { get; set; } = new Dictionary<string, int>();
    }

    public class SemesterStatsQueryResultDto
    {
        public int TotalSemesters { get; set; }
        public int ActiveSemesters { get; set; }
        public int UpcomingSemesters { get; set; }
        public int OngoingSemesters { get; set; }
        public int CompletedSemesters { get; set; }
        public int TotalStudents { get; set; }
        public decimal AverageDurationWeeks { get; set; }
        public int SemestersWithExams { get; set; }
        public string? semesters_by_status_json { get; set; }
        public string? semesters_by_level_json { get; set; }
        public string? semesters_by_department_json { get; set; }
        public string? semesters_by_academic_year_json { get; set; }
    }
}

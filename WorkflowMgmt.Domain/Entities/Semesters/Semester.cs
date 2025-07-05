using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities.Semesters
{
    public class SemesterDTO : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public int CourseId { get; set; }
        // Note: Semesters have CourseId - each semester belongs to a specific course

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int DurationWeeks { get; set; }

        public string Level { get; set; } = "Undergraduate";

        public int TotalStudents { get; set; } = 0;

        public string Status { get; set; } = "Upcoming";

        public string? Description { get; set; }

        public bool ExamScheduled { get; set; } = false;
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowMgmt.Domain.Entities.Courses
{
    public class CourseDTO : BaseEntity
    {
        [Column("name")]
        public string CourseName { get; set; } = string.Empty;

        [Column("code")]
        public string CourseCode { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("credits")]
        public int Credits { get; set; }

        [Column("department_id")]
        public int DepartmentId { get; set; }

        [Column("course_type")]
        public string CourseType { get; set; } = "Core";

        [Column("level")]
        public string Level { get; set; } = "Undergraduate";

        [Column("duration_weeks")]
        public int DurationWeeks { get; set; }

        [Column("max_capacity")]
        public int MaxCapacity { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Active";

        [Column("prerequisites")]
        public string? Prerequisites { get; set; }

        [Column("learning_objectives")]
        public string? LearningObjectives { get; set; }

        [Column("learning_outcomes")]
        public string? LearningOutcomes { get; set; }
    }
}

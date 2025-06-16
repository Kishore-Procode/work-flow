using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Domain.Entities.Courses
{
    public class CourseDTO : BaseEntity
    {
        public string ?CourseName { get; set; }
        public string ?CourseCode { get; set; }
        public string ?Description { get; set; }
        public int Credits { get; set; }
        public string ?Course_Type { get; set; }
        public string ?Level { get; set; }
        public int Semester_Id { get; set; }
        public string ?Duration_Work { get;set; }
        public string ?Status { get; set; }
        public string ?Prerequisites { get; set; }
        public string ?Learning_Objectives { get; set; }
        public string ?Learning_Outcomes { get; set; }
        public DateTime Created_Date { get; set; }


    }
}

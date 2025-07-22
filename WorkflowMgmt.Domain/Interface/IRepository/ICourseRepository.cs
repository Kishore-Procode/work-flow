using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Courses;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ICourseRepository
    {
        Task<List<CourseDTO>> GetAllCourses();
        Task<CourseDTO?> GetCourseById(int id);
        Task<int> InsertCourse(CourseDTO course);
        Task<bool> UpdateCourse(CourseDTO course);
        Task<bool> DeleteOrRestoreCourse(int id, string modifiedBy, bool isRestore);
    }
}

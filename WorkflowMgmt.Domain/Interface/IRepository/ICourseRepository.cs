using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Courses;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ICourseRepository
    {
        Task<List<CourseDTO>> GetAllCourses();
    }
}

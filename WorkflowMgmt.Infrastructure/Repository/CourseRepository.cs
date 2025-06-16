using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Entities.Courses;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class CourseRepository : RepositoryTranBase, ICourseRepository
    {
        public CourseRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<CourseDTO>> GetAllCourses()
        {
            var courses = await Connection.QueryAsync<CourseDTO>(
                "SELECT * FROM workflowmgmt.courses", Transaction);

            return courses.ToList();
        }

    }
}

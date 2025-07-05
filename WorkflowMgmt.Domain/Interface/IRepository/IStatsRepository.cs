using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models.Stats;
using WorkflowMgmt.Domain.Models.User;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IStatsRepository
    {
        Task<DepartmentStatsDto> GetDepartmentStatsAsync();
        Task<CourseStatsDto> GetCourseStatsAsync();
        Task<SemesterStatsDto> GetSemesterStatsAsync();
        Task<UserStatsDto> GetUserStatsAsync();
        Task<int> GetTotalDepartmentsAsync();
        Task<int> GetActiveDepartmentsAsync();
        Task<int> GetTotalProgramsAsync();
        Task<int> GetNbaAccreditedDepartmentsAsync();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Models.Stats;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IStatsRepository
    {
        Task<DepartmentStatsDto> GetDepartmentStatsAsync();
        Task<CourseStatsDto> GetCourseStatsAsync();
        Task<SemesterStatsDto> GetSemesterStatsAsync();
        Task<int> GetTotalDepartmentsAsync();
        Task<int> GetActiveDepartmentsAsync();
        Task<int> GetTotalProgramsAsync();
        Task<int> GetNbaAccreditedDepartmentsAsync();
    }
}

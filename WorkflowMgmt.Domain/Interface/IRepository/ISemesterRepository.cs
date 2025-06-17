using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities.Semesters;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface ISemesterRepository
    {
        Task<List<SemesterDTO>> GetAllSemesters();
        Task<SemesterDTO?> GetSemesterById(int id);
        Task<int> InsertSemester(SemesterDTO semester);
        Task<bool> UpdateSemester(SemesterDTO semester);
        Task<bool> DeleteOrRestoreSemester(int id, string modifiedBy, bool isRestore);
    }
}

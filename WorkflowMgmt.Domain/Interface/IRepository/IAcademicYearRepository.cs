using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;

namespace WorkflowMgmt.Domain.Interface.IRepository
{
    public interface IAcademicYearRepository
    {
        Task<List<AcademicYearDTO>> GetAllAcademicYears();
        Task<List<AcademicYearDTO>> GetAcademicYearsByLevelId(int levelId);
        Task<AcademicYearDTO?> GetAcademicYearById(int id);
        Task<int> InsertAcademicYear(AcademicYearDTO academicYear);
        Task<bool> UpdateAcademicYear(AcademicYearDTO academicYear);
        Task<bool> DeleteOrRestoreAcademicYear(int id, string modifiedBy, bool isRestore);
    }
}

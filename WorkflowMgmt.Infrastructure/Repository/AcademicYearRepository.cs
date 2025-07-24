using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Entities;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class AcademicYearRepository : RepositoryTranBase, IAcademicYearRepository
    {
        public AcademicYearRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<AcademicYearDTO>> GetAllAcademicYears()
        {
            var sql = @"
                SELECT ay.*, l.name as level_name, l.code as level_code
                FROM workflowmgmt.academic_years ay
                LEFT JOIN workflowmgmt.levels l ON ay.level_id = l.id
                WHERE ay.is_active = true 
                ORDER BY l.sort_order, ay.start_year DESC";

            var academicYears = await Connection.QueryAsync<AcademicYearDTO>(sql, Transaction);
            return academicYears.ToList();
        }

        public async Task<List<AcademicYearDTO>> GetAcademicYearsByLevelId(int levelId)
        {
            var sql = @"
                SELECT ay.*, l.name as level_name, l.code as level_code
                FROM workflowmgmt.academic_years ay
                LEFT JOIN workflowmgmt.levels l ON ay.level_id = l.id
                WHERE ay.level_id = @LevelId AND ay.is_active = true 
                ORDER BY ay.start_year DESC";

            var academicYears = await Connection.QueryAsync<AcademicYearDTO>(sql, new { LevelId = levelId }, Transaction);
            return academicYears.ToList();
        }

        public async Task<AcademicYearDTO?> GetAcademicYearById(int id)
        {
            var sql = @"
                SELECT ay.*, l.name as level_name, l.code as level_code
                FROM workflowmgmt.academic_years ay
                LEFT JOIN workflowmgmt.levels l ON ay.level_id = l.id
                WHERE ay.id = @Id";

            var academicYear = await Connection.QueryFirstOrDefaultAsync<AcademicYearDTO>(sql, new { Id = id }, Transaction);
            return academicYear;
        }

        public async Task<int> InsertAcademicYear(AcademicYearDTO academicYear)
        {
            var sql = @"
                INSERT INTO workflowmgmt.academic_years (
                    name,
                    code,
                    start_year,
                    end_year,
                    level_id,
                    status,
                    description,
                    is_active,
                    created_date,
                    created_by
                ) VALUES (
                    @name,
                    @code,
                    @start_year,
                    @end_year,
                    @level_id,
                    @status,
                    @description,
                    @is_active,
                    NOW(),
                    @CreatedBy
                )
                RETURNING id;
            ";

            var academicYearId = await Connection.ExecuteScalarAsync<int>(sql, academicYear, Transaction);
            return academicYearId;
        }

        public async Task<bool> UpdateAcademicYear(AcademicYearDTO academicYear)
        {
            var sql = @"
                UPDATE workflowmgmt.academic_years SET
                name = @name,
                code = @code,
                start_year = @start_year,
                end_year = @end_year,
                level_id = @level_id,
                status = @status,
                description = @description,
                is_active = @is_active,
                modified_date = NOW(),
                modified_by = @ModifiedBy
                WHERE id = @id;
           ";

            var rowsAffected = await Connection.ExecuteAsync(sql, academicYear, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteOrRestoreAcademicYear(int id, string modifiedBy, bool isRestore)
        {
            var sql = @"
        UPDATE workflowmgmt.academic_years
        SET is_active = @IsActive,
            status = @Status,
            modified_by = @ModifiedBy,
            modified_date = NOW()
        WHERE id = @Id AND is_active != @IsActive";

            var result = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedBy = modifiedBy,
                IsActive = isRestore,
                Status = isRestore ? "Active" : "Inactive"
            }, Transaction);

            return result > 0;
        }
    }
}

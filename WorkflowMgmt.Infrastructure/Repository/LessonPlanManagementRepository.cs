using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WorkflowMgmt.Domain.Entities.LessonPlan;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class LessonPlanRepository : RepositoryTranBase, ILessonPlanRepository
    {
        public LessonPlanRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<List<LessonPlanDto>> GetAllLessonPlans()
        {
            var lessonPlans = await Connection.QueryAsync<LessonPlanDto>(
                "SELECT * FROM workflowmgmt.lesson_plans ORDER BY title", Transaction);

            return lessonPlans.ToList();
        }

        public async Task<LessonPlanDto?> GetLessonPlanById(Guid id)
        {
            var sql = "SELECT * FROM workflowmgmt.lesson_plans WHERE id = @Id";
            var lessonPlan = await Connection.QueryFirstOrDefaultAsync<LessonPlanDto>(sql, new { Id = id }, Transaction);
            return lessonPlan;
        }

        public async Task<Guid> InsertLessonPlan(LessonPlanDto lessonPlan)
        {
            var sql = @"
                INSERT INTO workflowmgmt.lesson_plans (
                    title,
                    syllabus_id,
                    template_id,
                    module_name,
                    duration_minutes,
                    number_of_sessions,
                    scheduled_date,
                    faculty_name,
                    content_creation_method,
                    lesson_description,
                    learning_objectives,
                    teaching_methods,
                    learning_activities,
                    detailed_content,
                    resources,
                    assessment_methods,
                    prerequisites,
                    document_url,
                    status,
                    is_active,
                    file_processing_status,
                    file_processing_notes,
                    original_filename,
                    created_date,
                    created_by
                ) VALUES (
                    @title,
                    @syllabus_id,
                    @template_id,
                    @module_name,
                    @duration_minutes,
                    @number_of_sessions,
                    @scheduled_date,
                    @faculty_name,
                    @content_creation_method,
                    @lesson_description,
                    @learning_objectives,
                    @teaching_methods,
                    @learning_activities,
                    @detailed_content,
                    @resources,
                    @assessment_methods,
                    @prerequisites,
                    @document_url,
                    @status,
                    @is_active,
                    @file_processing_status,
                    @file_processing_notes,
                    @original_filename,
                    NOW(),
                    @CreatedBy
                )
                RETURNING id;
            ";

            var lessonPlanId = await Connection.ExecuteScalarAsync<Guid>(sql, lessonPlan, Transaction);
            return lessonPlanId;
        }

        public async Task<bool> UpdateLessonPlan(LessonPlanDto lessonPlan)
        {
            var sql = @"
                UPDATE workflowmgmt.lesson_plans SET
                title = @title,
                syllabus_id = @syllabus_id,
                template_id = @template_id,
                module_name = @module_name,
                duration_minutes = @duration_minutes,
                number_of_sessions = @number_of_sessions,
                scheduled_date = @scheduled_date,
                faculty_name = @faculty_name,
                content_creation_method = @content_creation_method,
                lesson_description = @lesson_description,
                learning_objectives = @learning_objectives,
                teaching_methods = @teaching_methods,
                learning_activities = @learning_activities,
                detailed_content = @detailed_content,
                resources = @resources,
                assessment_methods = @assessment_methods,
                prerequisites = @prerequisites,
                document_url = @document_url,
                status = @status,
                is_active = @is_active,
                file_processing_status = @file_processing_status,
                file_processing_notes = @file_processing_notes,
                original_filename = @original_filename,
                modified_date = NOW(),
                modified_by = @ModifiedBy
                WHERE id = @id;
           ";

            var rowsAffected = await Connection.ExecuteAsync(sql, lessonPlan, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteOrRestoreLessonPlan(Guid id, string modifiedBy, bool isRestore)
        {
            var sql = @"
        UPDATE workflowmgmt.lesson_plans
        SET is_active = @IsActive,
            modified_by = @ModifiedBy,
            modified_date = NOW()
        WHERE id = @Id AND is_active != @IsActive";

            var result = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedBy = modifiedBy,
                IsActive = isRestore
            }, Transaction);

            return result > 0;
        }
    }
}

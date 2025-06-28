using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Session;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public class SessionRepository : RepositoryTranBase, ISessionRepository
    {
        public SessionRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<SessionDto>> GetAllAsync()
        {
            var sql = @"
                SELECT id, title, lesson_plan_id, faculty_id, teaching_method,
                       session_date, session_time, duration_minutes, instructor, content_creation_method,
                       session_description, session_objectives, session_activities, materials_equipment,
                       detailed_content, content_resources, document_url, status, is_active,
                       created_date, modified_date, created_by, modified_by, file_processing_status,
                       file_processing_notes, original_filename
                FROM workflowmgmt.sessions
                WHERE is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<SessionDto>(sql, transaction: Transaction);
        }

        public async Task<SessionWithDetailsDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT s.id, s.title, s.lesson_plan_id, s.faculty_id, s.teaching_method,
                       s.session_date, s.session_time, s.duration_minutes, s.instructor, s.content_creation_method,
                       s.session_description, s.session_objectives, s.session_activities, s.materials_equipment,
                       s.detailed_content, s.content_resources, s.document_url, s.status, s.is_active,
                       s.created_date, s.modified_date, s.created_by, s.modified_by, s.file_processing_status,
                       s.file_processing_notes, s.original_filename,
                       syl.department_id as DepartmentId,
                       d.name as DepartmentName,
                       lp.title as LessonPlanTitle,
                       syl.title as SyllabusTitle
                FROM workflowmgmt.sessions s
                LEFT JOIN workflowmgmt.lesson_plans lp ON s.lesson_plan_id = lp.id
                LEFT JOIN workflowmgmt.syllabi syl ON lp.syllabus_id = syl.id
                LEFT JOIN workflowmgmt.departments d ON syl.department_id = d.id
                WHERE s.id = @Id AND s.is_active = true";

            return await Connection.QuerySingleOrDefaultAsync<SessionWithDetailsDto>(sql, new { Id = id }, transaction: Transaction);
        }

        public async Task<IEnumerable<SessionDto>> GetByStatusAsync(string status)
        {
            var sql = @"
                SELECT id, title, lesson_plan_id, faculty_id, teaching_method,
                       session_date, session_time, duration_minutes, instructor, content_creation_method,
                       session_description, session_objectives, session_activities, materials_equipment,
                       detailed_content, content_resources, document_url, status, is_active,
                       created_date, modified_date, created_by, modified_by, file_processing_status,
                       file_processing_notes, original_filename
                FROM workflowmgmt.sessions
                WHERE status = @Status AND is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<SessionDto>(sql, new { Status = status }, transaction: Transaction);
        }

        public async Task<IEnumerable<SessionDto>> GetByInstructorAsync(string instructor)
        {
            var sql = @"
                SELECT id, title, lesson_plan_id, faculty_id, teaching_method,
                       session_date, session_time, duration_minutes, instructor, content_creation_method,
                       session_description, session_objectives, session_activities, materials_equipment,
                       detailed_content, content_resources, document_url, status, is_active,
                       created_date, modified_date, created_by, modified_by, file_processing_status,
                       file_processing_notes, original_filename
                FROM workflowmgmt.sessions
                WHERE instructor = @Instructor AND is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<SessionDto>(sql, new { Instructor = instructor }, transaction: Transaction);
        }

        public async Task<IEnumerable<SessionDto>> GetByDepartmentAsync(int departmentId)
        {
            var sql = @"
                SELECT s.id, s.title, s.lesson_plan_id, s.faculty_id, s.teaching_method,
                       s.session_date, s.session_time, s.duration_minutes, s.instructor, s.content_creation_method,
                       s.session_description, s.session_objectives, s.session_activities, s.materials_equipment,
                       s.detailed_content, s.content_resources, s.document_url, s.status, s.is_active,
                       s.created_date, s.modified_date, s.created_by, s.modified_by, s.file_processing_status,
                       s.file_processing_notes, s.original_filename
                FROM workflowmgmt.sessions s
                INNER JOIN workflowmgmt.lesson_plans lp ON s.lesson_plan_id = lp.id
                INNER JOIN workflowmgmt.syllabi syl ON lp.syllabus_id = syl.id
                WHERE syl.department_id = @DepartmentId AND s.is_active = true
                ORDER BY s.created_date DESC";

            return await Connection.QueryAsync<SessionDto>(sql, new { DepartmentId = departmentId }, transaction: Transaction);
        }

        public async Task<IEnumerable<SessionDto>> GetByLessonPlanAsync(Guid lessonPlanId)
        {
            var sql = @"
                SELECT id, title, lesson_plan_id, faculty_id, teaching_method,
                       session_date, session_time, duration_minutes, instructor, content_creation_method,
                       session_description, session_objectives, session_activities, materials_equipment,
                       detailed_content, content_resources, document_url, status, is_active,
                       created_date, modified_date, created_by, modified_by, file_processing_status,
                       file_processing_notes, original_filename
                FROM workflowmgmt.sessions
                WHERE lesson_plan_id = @LessonPlanId AND is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<SessionDto>(sql, new { LessonPlanId = lessonPlanId }, transaction: Transaction);
        }

        public async Task<IEnumerable<SessionDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var sql = @"
                SELECT id, title, lesson_plan_id, faculty_id, teaching_method,
                       session_date, session_time, duration_minutes, instructor, content_creation_method,
                       session_description, session_objectives, session_activities, materials_equipment,
                       detailed_content, content_resources, document_url, status, is_active,
                       created_date, modified_date, created_by, modified_by, file_processing_status,
                       file_processing_notes, original_filename
                FROM workflowmgmt.sessions
                WHERE session_date BETWEEN @StartDate AND @EndDate AND is_active = true
                ORDER BY session_date ASC, session_time ASC";

            return await Connection.QueryAsync<SessionDto>(sql, new { StartDate = startDate, EndDate = endDate }, transaction: Transaction);
        }

        public async Task<SessionStatsDto> GetStatsAsync()
        {
            var sql = @"
                SELECT 
                    COUNT(*) as TotalSessions,
                    COUNT(CASE WHEN status = 'Draft' THEN 1 END) as DraftSessions,
                    COUNT(CASE WHEN status = 'Published' THEN 1 END) as PublishedSessions,
                    COUNT(CASE WHEN status = 'Under Review' THEN 1 END) as UnderReviewSessions,
                    COUNT(CASE WHEN status = 'Rejected' THEN 1 END) as RejectedSessions,
                    COUNT(CASE WHEN session_date = CURRENT_DATE THEN 1 END) as TodaySessions,
                    COUNT(CASE WHEN session_date > CURRENT_DATE THEN 1 END) as UpcomingSessions
                FROM workflowmgmt.sessions 
                WHERE is_active = true";

            return await Connection.QuerySingleAsync<SessionStatsDto>(sql, transaction: Transaction);
        }

        public async Task<Guid> CreateAsync(CreateSessionDto session)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.sessions
                (id, title, lesson_plan_id, faculty_id, teaching_method, session_date, session_time,
                 duration_minutes, instructor, content_creation_method, session_description, session_objectives,
                 session_activities, materials_equipment, detailed_content, content_resources, document_url,
                 status, is_active, created_date, created_by, file_processing_status, file_processing_notes, original_filename)
                VALUES
                (@Id, @Title, @LessonPlanId, @FacultyId, @TeachingMethod, @SessionDate, @SessionTime,
                 @DurationMinutes, @Instructor, @ContentCreationMethod, @SessionDescription, @SessionObjectives,
                 @SessionActivities, @MaterialsEquipment, @DetailedContent, @ContentResources, @DocumentUrl,
                 'Draft', true, NOW(), @CreatedBy, 'not_applicable', null, @OriginalFilename)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                session.Title,
                session.LessonPlanId,
                session.FacultyId,
                session.TeachingMethod,
                session.SessionDate,
                session.SessionTime,
                session.DurationMinutes,
                session.Instructor,
                session.ContentCreationMethod,
                session.SessionDescription,
                session.SessionObjectives,
                session.SessionActivities,
                session.MaterialsEquipment,
                session.DetailedContent,
                session.ContentResources,
                session.DocumentUrl,
                session.OriginalFilename,
                session.CreatedBy
            }, Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateSessionDto session)
        {
            var setParts = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("ModifiedDate", DateTime.UtcNow);
            parameters.Add("ModifiedBy", session.ModifiedBy);

            if (!string.IsNullOrEmpty(session.Title))
            {
                setParts.Add("title = @Title");
                parameters.Add("Title", session.Title);
            }

            if (session.LessonPlanId.HasValue)
            {
                setParts.Add("lesson_plan_id = @LessonPlanId");
                parameters.Add("LessonPlanId", session.LessonPlanId);
            }



            if (session.FacultyId.HasValue)
            {
                setParts.Add("faculty_id = @FacultyId");
                parameters.Add("FacultyId", session.FacultyId);
            }

            if (!string.IsNullOrEmpty(session.TeachingMethod))
            {
                setParts.Add("teaching_method = @TeachingMethod");
                parameters.Add("TeachingMethod", session.TeachingMethod);
            }

            if (session.SessionDate.HasValue)
            {
                setParts.Add("session_date = @SessionDate");
                parameters.Add("SessionDate", session.SessionDate);
            }

            if (session.SessionTime.HasValue)
            {
                setParts.Add("session_time = @SessionTime");
                parameters.Add("SessionTime", session.SessionTime);
            }

            if (session.DurationMinutes.HasValue)
            {
                setParts.Add("duration_minutes = @DurationMinutes");
                parameters.Add("DurationMinutes", session.DurationMinutes);
            }

            if (!string.IsNullOrEmpty(session.Instructor))
            {
                setParts.Add("instructor = @Instructor");
                parameters.Add("Instructor", session.Instructor);
            }

            if (session.SessionDescription != null)
            {
                setParts.Add("session_description = @SessionDescription");
                parameters.Add("SessionDescription", session.SessionDescription);
            }

            if (session.SessionObjectives != null)
            {
                setParts.Add("session_objectives = @SessionObjectives");
                parameters.Add("SessionObjectives", session.SessionObjectives);
            }

            if (session.SessionActivities != null)
            {
                setParts.Add("session_activities = @SessionActivities");
                parameters.Add("SessionActivities", session.SessionActivities);
            }

            if (session.MaterialsEquipment != null)
            {
                setParts.Add("materials_equipment = @MaterialsEquipment");
                parameters.Add("MaterialsEquipment", session.MaterialsEquipment);
            }

            if (session.DetailedContent != null)
            {
                setParts.Add("detailed_content = @DetailedContent");
                parameters.Add("DetailedContent", session.DetailedContent);
            }

            if (session.ContentResources != null)
            {
                setParts.Add("content_resources = @ContentResources");
                parameters.Add("ContentResources", session.ContentResources);
            }

            if (!string.IsNullOrEmpty(session.Status))
            {
                setParts.Add("status = @Status");
                parameters.Add("Status", session.Status);
            }

            if (session.DocumentUrl != null)
            {
                setParts.Add("document_url = @DocumentUrl");
                parameters.Add("DocumentUrl", session.DocumentUrl);
            }

            if (session.OriginalFilename != null)
            {
                setParts.Add("original_filename = @OriginalFilename");
                parameters.Add("OriginalFilename", session.OriginalFilename);
            }

            if (session.FileProcessingStatus != null)
            {
                setParts.Add("file_processing_status = @FileProcessingStatus");
                parameters.Add("FileProcessingStatus", session.FileProcessingStatus);
            }

            if (session.FileProcessingNotes != null)
            {
                setParts.Add("file_processing_notes = @FileProcessingNotes");
                parameters.Add("FileProcessingNotes", session.FileProcessingNotes);
            }

            if (!setParts.Any())
            {
                return false; // Nothing to update
            }

            setParts.Add("modified_date = @ModifiedDate");
            setParts.Add("modified_by = @ModifiedBy");

            var sql = $@"
                UPDATE workflowmgmt.sessions
                SET {string.Join(", ", setParts)}
                WHERE id = @Id AND is_active = true";

            var rowsAffected = await Connection.ExecuteAsync(sql, parameters, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.sessions
                SET is_active = false, modified_date = NOW()
                WHERE id = @Id AND is_active = true";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.sessions
                SET is_active = NOT is_active, modified_date = NOW()
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            var sql = "SELECT COUNT(1) FROM workflowmgmt.sessions WHERE id = @Id AND is_active = true";
            var count = await Connection.QuerySingleAsync<int>(sql, new { Id = id }, transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> ExistsByTitleAndDepartmentAsync(string title, int departmentId)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM workflowmgmt.sessions s
                INNER JOIN workflowmgmt.lesson_plans lp ON s.lesson_plan_id = lp.id
                INNER JOIN workflowmgmt.syllabi syl ON lp.syllabus_id = syl.id
                WHERE s.title = @Title AND syl.department_id = @DepartmentId AND s.is_active = true";

            var count = await Connection.QuerySingleAsync<int>(sql,
                new { Title = title, DepartmentId = departmentId },
                transaction: Transaction);
            return count > 0;
        }

        public async Task<bool> UpdateDocumentUrlAsync(Guid id, string documentUrl)
        {
            var sql = @"
                UPDATE workflowmgmt.sessions
                SET document_url = @DocumentUrl, modified_date = NOW()
                WHERE id = @Id AND is_active = true";

            var rowsAffected = await Connection.ExecuteAsync(sql,
                new { Id = id, DocumentUrl = documentUrl },
                Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> UpdateStatusAsync(Guid id, string status)
        {
            var sql = @"
                UPDATE workflowmgmt.sessions
                SET status = @Status, modified_date = NOW()
                WHERE id = @Id AND is_active = true";

            var rowsAffected = await Connection.ExecuteAsync(sql,
                new { Id = id, Status = status },
                Transaction);
            return rowsAffected > 0;
        }
    }
}

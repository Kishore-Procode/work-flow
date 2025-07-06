using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using WorkflowMgmt.Domain.Models.LessonPlan;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public partial class LessonPlanManagementRepository : RepositoryTranBase, ILessonPlanRepository
    {
        public LessonPlanManagementRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        // Basic CRUD operations
        public async Task<IEnumerable<LessonPlanDto>> GetAllAsync()
        {
            var sql = @"
                SELECT
                    id as Id,
                    title as Title,
                    syllabus_id as SyllabusId,
                    template_id as TemplateId,
                    module_name as ModuleName,
                    duration_minutes as DurationMinutes,
                    number_of_sessions as NumberOfSessions,
                    scheduled_date as ScheduledDate,
                    faculty_id as FacultyId,
                    faculty_name as FacultyName,
                    content_creation_method as ContentCreationMethod,
                    lesson_description as LessonDescription,
                    learning_objectives as LearningObjectives,
                    teaching_methods as TeachingMethods,
                    learning_activities as LearningActivities,
                    detailed_content as DetailedContent,
                    resources as Resources,
                    assessment_methods as AssessmentMethods,
                    prerequisites as Prerequisites,
                    document_url as DocumentUrl,
                    status as Status,
                    is_active as IsActive,
                    created_date as CreatedDate,
                    modified_date as ModifiedDate,
                    created_by as CreatedBy,
                    modified_by as ModifiedBy,
                    file_processing_status as FileProcessingStatus,
                    file_processing_notes as FileProcessingNotes,
                    original_filename as OriginalFilename
                FROM workflowmgmt.lesson_plans
                WHERE is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<LessonPlanDto>(sql, transaction: Transaction);
        }

        public async Task<LessonPlanWithDetailsDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT
                    lp.id as Id,
                    lp.title as Title,
                    lp.syllabus_id as SyllabusId,
                    lp.template_id as TemplateId,
                    lp.module_name as ModuleName,
                    lp.duration_minutes as DurationMinutes,
                    lp.number_of_sessions as NumberOfSessions,
                    lp.scheduled_date as ScheduledDate,
                    lp.faculty_id as FacultyId,
                    lp.faculty_name as FacultyName,
                    lp.content_creation_method as ContentCreationMethod,
                    lp.lesson_description as LessonDescription,
                    lp.learning_objectives as LearningObjectives,
                    lp.teaching_methods as TeachingMethods,
                    lp.learning_activities as LearningActivities,
                    lp.detailed_content as DetailedContent,
                    lp.resources as Resources,
                    lp.assessment_methods as AssessmentMethods,
                    lp.prerequisites as Prerequisites,
                    lp.document_url as DocumentUrl,
                    lp.status as Status,
                    lp.is_active as IsActive,
                    lp.created_date as CreatedDate,
                    lp.modified_date as ModifiedDate,
                    lp.created_by as CreatedBy,
                    lp.modified_by as ModifiedBy,
                    lp.file_processing_status as FileProcessingStatus,
                    lp.file_processing_notes as FileProcessingNotes,
                    lp.original_filename as OriginalFilename,

                    -- Template details
                    lpt.id as Id,
                    lpt.name as Name,
                    lpt.description as Description,
                    lpt.template_type as TemplateType,
                    lpt.duration_minutes as DurationMinutes,
                    lpt.sections as Sections,
                    lpt.is_active as IsActive,
                    lpt.created_date as CreatedDate,
                    lpt.modified_date as ModifiedDate,
                    lpt.created_by as CreatedBy,
                    lpt.modified_by as ModifiedBy,

                    -- Syllabus details (if exists)
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    d.name as DepartmentName,
                    s.course_id as CourseId,
                    c.name as CourseName,
                    s.semester_id as SemesterId,
                    sem.name as SemesterName,
                    s.faculty_name as FacultyName
                FROM workflowmgmt.lesson_plans lp
                LEFT JOIN workflowmgmt.lesson_plan_templates lpt ON lp.template_id = lpt.id
                LEFT JOIN workflowmgmt.syllabi s ON lp.syllabus_id = s.id
                LEFT JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                WHERE lp.id = @Id AND lp.is_active = true";

            var lessonPlanDict = new Dictionary<Guid, LessonPlanWithDetailsDto>();

            await Connection.QueryAsync<LessonPlanWithDetailsDto, LessonPlanTemplateDto, SyllabusBasicDto, LessonPlanWithDetailsDto>(
                sql,
                (lessonPlan, template, syllabus) =>
                {
                    if (!lessonPlanDict.TryGetValue(lessonPlan.Id, out var lessonPlanEntry))
                    {
                        lessonPlanEntry = lessonPlan;
                        lessonPlanDict.Add(lessonPlan.Id, lessonPlanEntry);
                    }

                    if (template != null)
                    {
                        lessonPlanEntry.Template = template;
                    }

                    if (syllabus != null)
                    {
                        lessonPlanEntry.Syllabus = syllabus;
                    }

                    return lessonPlanEntry;
                },
                new { Id = id },
                transaction: Transaction,
                splitOn: "Id,Id,Id"
            );

            return lessonPlanDict.Values.FirstOrDefault();
        }

        public async Task<Guid> CreateAsync(CreateLessonPlanDto lessonPlan)
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
                    faculty_id,
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
                    @Title,
                    @SyllabusId,
                    @TemplateId,
                    @ModuleName,
                    @DurationMinutes,
                    @NumberOfSessions,
                    @ScheduledDate,
                    @FacultyId,
                    @FacultyName,
                    @ContentCreationMethod,
                    @LessonDescription,
                    @LearningObjectives,
                    @TeachingMethods,
                    @LearningActivities,
                    @DetailedContent,
                    @Resources,
                    @AssessmentMethods,
                    @Prerequisites,
                    @DocumentUrl,
                    'Draft',
                    true,
                    'not_applicable',
                    null,
                    @OriginalFilename,
                    NOW(),
                    @CreatedBy
                )
                RETURNING id;
            ";

            return await Connection.ExecuteScalarAsync<Guid>(sql, lessonPlan, Transaction);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateLessonPlanDto lessonPlan)
        {
            var setParts = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);
            parameters.Add("ModifiedDate", DateTime.Now);
            parameters.Add("ModifiedBy", lessonPlan.ModifiedBy);

            if (!string.IsNullOrEmpty(lessonPlan.Title))
            {
                setParts.Add("title = @Title");
                parameters.Add("Title", lessonPlan.Title);
            }

            if (lessonPlan.SyllabusId.HasValue)
            {
                setParts.Add("syllabus_id = @SyllabusId");
                parameters.Add("SyllabusId", lessonPlan.SyllabusId);
            }

            if (!string.IsNullOrEmpty(lessonPlan.ModuleName))
            {
                setParts.Add("module_name = @ModuleName");
                parameters.Add("ModuleName", lessonPlan.ModuleName);
            }

            if (lessonPlan.DurationMinutes.HasValue)
            {
                setParts.Add("duration_minutes = @DurationMinutes");
                parameters.Add("DurationMinutes", lessonPlan.DurationMinutes);
            }

            if (lessonPlan.NumberOfSessions.HasValue)
            {
                setParts.Add("number_of_sessions = @NumberOfSessions");
                parameters.Add("NumberOfSessions", lessonPlan.NumberOfSessions);
            }

            if (lessonPlan.ScheduledDate.HasValue)
            {
                setParts.Add("scheduled_date = @ScheduledDate");
                parameters.Add("ScheduledDate", lessonPlan.ScheduledDate);
            }

            if (lessonPlan.FacultyId.HasValue)
            {
                setParts.Add("faculty_id = @FacultyId");
                parameters.Add("FacultyId", lessonPlan.FacultyId);
            }

            if (!string.IsNullOrEmpty(lessonPlan.FacultyName))
            {
                setParts.Add("faculty_name = @FacultyName");
                parameters.Add("FacultyName", lessonPlan.FacultyName);
            }

            if (lessonPlan.LessonDescription != null)
            {
                setParts.Add("lesson_description = @LessonDescription");
                parameters.Add("LessonDescription", lessonPlan.LessonDescription);
            }

            if (lessonPlan.LearningObjectives != null)
            {
                setParts.Add("learning_objectives = @LearningObjectives");
                parameters.Add("LearningObjectives", lessonPlan.LearningObjectives);
            }

            if (lessonPlan.TeachingMethods != null)
            {
                setParts.Add("teaching_methods = @TeachingMethods");
                parameters.Add("TeachingMethods", lessonPlan.TeachingMethods);
            }

            if (lessonPlan.LearningActivities != null)
            {
                setParts.Add("learning_activities = @LearningActivities");
                parameters.Add("LearningActivities", lessonPlan.LearningActivities);
            }

            if (lessonPlan.DetailedContent != null)
            {
                setParts.Add("detailed_content = @DetailedContent");
                parameters.Add("DetailedContent", lessonPlan.DetailedContent);
            }

            if (lessonPlan.Resources != null)
            {
                setParts.Add("resources = @Resources");
                parameters.Add("Resources", lessonPlan.Resources);
            }

            if (lessonPlan.AssessmentMethods != null)
            {
                setParts.Add("assessment_methods = @AssessmentMethods");
                parameters.Add("AssessmentMethods", lessonPlan.AssessmentMethods);
            }

            if (lessonPlan.Prerequisites != null)
            {
                setParts.Add("prerequisites = @Prerequisites");
                parameters.Add("Prerequisites", lessonPlan.Prerequisites);
            }

            if (!string.IsNullOrEmpty(lessonPlan.Status))
            {
                setParts.Add("status = @Status");
                parameters.Add("Status", lessonPlan.Status);
            }

            if (lessonPlan.DocumentUrl != null)
            {
                setParts.Add("document_url = @DocumentUrl");
                parameters.Add("DocumentUrl", lessonPlan.DocumentUrl);
            }

            if (lessonPlan.OriginalFilename != null)
            {
                setParts.Add("original_filename = @OriginalFilename");
                parameters.Add("OriginalFilename", lessonPlan.OriginalFilename);
            }

            if (!setParts.Any())
            {
                return false; // Nothing to update
            }

            setParts.Add("modified_date = @ModifiedDate");
            setParts.Add("modified_by = @ModifiedBy");

            var sql = $@"
                UPDATE workflowmgmt.lesson_plans
                SET {string.Join(", ", setParts)}
                WHERE id = @Id AND is_active = true";

            var rowsAffected = await Connection.ExecuteAsync(sql, parameters, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.lesson_plans
                SET is_active = false, modified_date = NOW()
                WHERE id = @Id AND is_active = true";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.lesson_plans
                SET is_active = NOT is_active, modified_date = NOW()
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, Transaction);
            return rowsAffected > 0;
        }

        // Query operations
        public async Task<IEnumerable<LessonPlanDto>> GetByStatusAsync(string status)
        {
            var sql = @"
                SELECT
                    id as Id, title as Title, syllabus_id as SyllabusId, template_id as TemplateId,
                    module_name as ModuleName, duration_minutes as DurationMinutes,
                    number_of_sessions as NumberOfSessions, scheduled_date as ScheduledDate,
                    faculty_name as FacultyName, content_creation_method as ContentCreationMethod,
                    lesson_description as LessonDescription, learning_objectives as LearningObjectives,
                    teaching_methods as TeachingMethods, learning_activities as LearningActivities,
                    detailed_content as DetailedContent, resources as Resources,
                    assessment_methods as AssessmentMethods, prerequisites as Prerequisites,
                    document_url as DocumentUrl, status as Status, is_active as IsActive,
                    created_date as CreatedDate, modified_date as ModifiedDate,
                    created_by as CreatedBy, modified_by as ModifiedBy,
                    file_processing_status as FileProcessingStatus,
                    file_processing_notes as FileProcessingNotes,
                    original_filename as OriginalFilename
                FROM workflowmgmt.lesson_plans
                WHERE status = @Status AND is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<LessonPlanDto>(sql, new { Status = status }, Transaction);
        }

        public async Task<IEnumerable<LessonPlanDto>> GetByFacultyAsync(string facultyName)
        {
            var sql = @"
                SELECT
                    id as Id, title as Title, syllabus_id as SyllabusId, template_id as TemplateId,
                    module_name as ModuleName, duration_minutes as DurationMinutes,
                    number_of_sessions as NumberOfSessions, scheduled_date as ScheduledDate,
                    faculty_name as FacultyName, content_creation_method as ContentCreationMethod,
                    lesson_description as LessonDescription, learning_objectives as LearningObjectives,
                    teaching_methods as TeachingMethods, learning_activities as LearningActivities,
                    detailed_content as DetailedContent, resources as Resources,
                    assessment_methods as AssessmentMethods, prerequisites as Prerequisites,
                    document_url as DocumentUrl, status as Status, is_active as IsActive,
                    created_date as CreatedDate, modified_date as ModifiedDate,
                    created_by as CreatedBy, modified_by as ModifiedBy,
                    file_processing_status as FileProcessingStatus,
                    file_processing_notes as FileProcessingNotes,
                    original_filename as OriginalFilename
                FROM workflowmgmt.lesson_plans
                WHERE faculty_name = @FacultyName AND is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<LessonPlanDto>(sql, new { FacultyName = facultyName }, Transaction);
        }

        public async Task<IEnumerable<LessonPlanDto>> GetByTemplateAsync(Guid templateId)
        {
            var sql = @"
                SELECT
                    id as Id, title as Title, syllabus_id as SyllabusId, template_id as TemplateId,
                    module_name as ModuleName, duration_minutes as DurationMinutes,
                    number_of_sessions as NumberOfSessions, scheduled_date as ScheduledDate,
                    faculty_name as FacultyName, content_creation_method as ContentCreationMethod,
                    lesson_description as LessonDescription, learning_objectives as LearningObjectives,
                    teaching_methods as TeachingMethods, learning_activities as LearningActivities,
                    detailed_content as DetailedContent, resources as Resources,
                    assessment_methods as AssessmentMethods, prerequisites as Prerequisites,
                    document_url as DocumentUrl, status as Status, is_active as IsActive,
                    created_date as CreatedDate, modified_date as ModifiedDate,
                    created_by as CreatedBy, modified_by as ModifiedBy,
                    file_processing_status as FileProcessingStatus,
                    file_processing_notes as FileProcessingNotes,
                    original_filename as OriginalFilename
                FROM workflowmgmt.lesson_plans
                WHERE template_id = @TemplateId AND is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<LessonPlanDto>(sql, new { TemplateId = templateId }, Transaction);
        }

        public async Task<IEnumerable<LessonPlanDto>> GetBySyllabusAsync(Guid syllabusId)
        {
            var sql = @"
                SELECT
                    id as Id, title as Title, syllabus_id as SyllabusId, template_id as TemplateId,
                    module_name as ModuleName, duration_minutes as DurationMinutes,
                    number_of_sessions as NumberOfSessions, scheduled_date as ScheduledDate,
                    faculty_name as FacultyName, content_creation_method as ContentCreationMethod,
                    lesson_description as LessonDescription, learning_objectives as LearningObjectives,
                    teaching_methods as TeachingMethods, learning_activities as LearningActivities,
                    detailed_content as DetailedContent, resources as Resources,
                    assessment_methods as AssessmentMethods, prerequisites as Prerequisites,
                    document_url as DocumentUrl, status as Status, is_active as IsActive,
                    created_date as CreatedDate, modified_date as ModifiedDate,
                    created_by as CreatedBy, modified_by as ModifiedBy,
                    file_processing_status as FileProcessingStatus,
                    file_processing_notes as FileProcessingNotes,
                    original_filename as OriginalFilename
                FROM workflowmgmt.lesson_plans
                WHERE syllabus_id = @SyllabusId AND is_active = true
                ORDER BY created_date DESC";

            return await Connection.QueryAsync<LessonPlanDto>(sql, new { SyllabusId = syllabusId }, Transaction);
        }

        // Validation operations
        public async Task<bool> ExistsByTitleAndTemplateAsync(string title, Guid templateId)
        {
            var sql = @"
                SELECT COUNT(1)
                FROM workflowmgmt.lesson_plans
                WHERE title = @Title AND template_id = @TemplateId AND is_active = true";

            var count = await Connection.ExecuteScalarAsync<int>(sql, new { Title = title, TemplateId = templateId }, Transaction);
            return count > 0;
        }

        // Stats operations
        public async Task<LessonPlanStatsDto> GetStatsAsync()
        {
            try
            {
                // First check if tables exist and get basic counts
                var basicSql = @"
                    SELECT
                        CAST(COALESCE(COUNT(*), 0) AS INTEGER) as TotalLessonPlans,
                        CAST(COALESCE(COUNT(CASE WHEN status = 'Draft' THEN 1 END), 0) AS INTEGER) as DraftLessonPlans,
                        CAST(COALESCE(COUNT(CASE WHEN status = 'Published' THEN 1 END), 0) AS INTEGER) as PublishedLessonPlans,
                        CAST(COALESCE(COUNT(CASE WHEN status = 'Under Review' THEN 1 END), 0) AS INTEGER) as UnderReviewLessonPlans,
                        CAST(COALESCE(COUNT(CASE WHEN status = 'Approved' THEN 1 END), 0) AS INTEGER) as ApprovedLessonPlans,
                        CAST(COALESCE(COUNT(CASE WHEN status = 'Rejected' THEN 1 END), 0) AS INTEGER) as RejectedLessonPlans,
                        CAST(COALESCE(SUM(number_of_sessions), 0) AS INTEGER) as TotalSessions
                    FROM workflowmgmt.lesson_plans
                    WHERE is_active = true";

                var avgSql = @"
                    SELECT CAST(COALESCE(AVG(duration_minutes), 0) AS DOUBLE PRECISION) as AverageDuration
                    FROM workflowmgmt.lesson_plans
                    WHERE is_active = true AND duration_minutes IS NOT NULL";

                var templateSql = @"
                    SELECT
                        CAST(COALESCE(COUNT(*), 0) AS INTEGER) as TotalTemplates,
                        CAST(COALESCE(COUNT(CASE WHEN is_active = true THEN 1 END), 0) AS INTEGER) as ActiveTemplates
                    FROM workflowmgmt.lesson_plan_templates";

                // Execute queries separately to handle potential null issues
                var basicStats = await Connection.QueryFirstOrDefaultAsync(basicSql, transaction: Transaction);
                var avgStats = await Connection.QueryFirstOrDefaultAsync(avgSql, transaction: Transaction);
                var templateStats = await Connection.QueryFirstOrDefaultAsync(templateSql, transaction: Transaction);

                // Create stats object with safe defaults
                var stats = new LessonPlanStatsDto
                {
                    TotalLessonPlans = basicStats?.TotalLessonPlans ?? 0,
                    DraftLessonPlans = basicStats?.DraftLessonPlans ?? 0,
                    PublishedLessonPlans = basicStats?.PublishedLessonPlans ?? 0,
                    UnderReviewLessonPlans = basicStats?.UnderReviewLessonPlans ?? 0,
                    ApprovedLessonPlans = basicStats?.ApprovedLessonPlans ?? 0,
                    RejectedLessonPlans = basicStats?.RejectedLessonPlans ?? 0,
                    TotalSessions = basicStats?.TotalSessions ?? 0,
                    AverageDuration = avgStats?.AverageDuration ?? 0.0,
                    TotalTemplates = templateStats?.TotalTemplates ?? 0,
                    ActiveTemplates = templateStats?.ActiveTemplates ?? 0
                };

                return stats;
            }
            catch (Exception)
            {
                // Return empty stats if there's any database error
                return new LessonPlanStatsDto
                {
                    TotalLessonPlans = 0,
                    DraftLessonPlans = 0,
                    PublishedLessonPlans = 0,
                    UnderReviewLessonPlans = 0,
                    ApprovedLessonPlans = 0,
                    RejectedLessonPlans = 0,
                    AverageDuration = 0,
                    TotalSessions = 0,
                    TotalTemplates = 0,
                    ActiveTemplates = 0
                };
            }
        }
    }
}

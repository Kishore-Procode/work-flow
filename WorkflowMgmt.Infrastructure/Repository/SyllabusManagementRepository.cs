using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Models.Syllabus;
using WorkflowMgmt.Domain.Models.Workflow;
using WorkflowMgmt.Infrastructure.RepositoryBase;

namespace WorkflowMgmt.Infrastructure.Repository
{
    public partial class SyllabusManagementRepository : RepositoryTranBase, ISyllabusRepository
    {
        public SyllabusManagementRepository(IDbTransaction transaction) : base(transaction)
        {
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetAllAsync()
        {
            var sql = @"
                SELECT 
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    s.course_id as CourseId,
                    s.semester_id as SemesterId,
                    s.template_id as TemplateId,
                    s.faculty_id as FacultyId,
                    s.faculty_name as FacultyName,
                    s.credits as Credits,
                    s.duration_weeks as DurationWeeks,
                    s.content_creation_method as ContentCreationMethod,
                    s.course_description as CourseDescription,
                    s.learning_objectives as LearningObjectives,
                    s.learning_outcomes as LearningOutcomes,
                    s.course_topics as CourseTopics,
                    s.assessment_methods as AssessmentMethods,
                    s.detailed_content as DetailedContent,
                    s.reference_materials as ReferenceMaterials,
                    s.document_url as DocumentUrl,
                    s.status as Status,
                    s.is_active as IsActive,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.file_processing_status as FileProcessingStatus,
                    s.file_processing_notes as FileProcessingNotes,
                    s.original_filename as OriginalFilename,
                    -- Template
                    st.id as Template_Id,
                    st.name as Template_Name,
                    st.description as Template_Description,
                    st.template_type as Template_TemplateType,
                    st.sections as Template_Sections,
                    st.is_active as Template_IsActive,
                    st.created_date as Template_CreatedDate,
                    st.modified_date as Template_ModifiedDate,
                    st.created_by as Template_CreatedBy,
                    st.modified_by as Template_ModifiedBy,
                    -- Department
                    d.id as Department_Id,
                    d.name as Department_Name,
                    d.code as Department_Code,
                    -- Course
                    c.id as Course_Id,
                    c.name as Course_Name,
                    c.code as Course_Code,
                    -- Semester
                    sem.id as Semester_Id,
                    sem.name as Semester_Name,
                    sem.academic_year as Semester_Year,
                    -- Workflow
                    dw.id as Workflow_Id,
                    dw.status as Workflow_Status,
                    ws.id as CurrentStage_Id,
                    ws.stage_name as CurrentStage_StageName,
                    ws.assigned_role as CurrentStage_AssignedRole
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                LEFT JOIN workflowmgmt.document_workflows dw ON dw.document_id = s.id and dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                ORDER BY s.created_date DESC";

            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
                sql,
                (syllabus, template, department, course, semester, workflow, currentStage) =>
                {
                    syllabus.Template = template;
                    syllabus.Department = department;
                    syllabus.Course = course;
                    syllabus.Semester = semester;
                    if (workflow != null)
                    {
                        workflow.CurrentStage = currentStage;
                        syllabus.Workflow = workflow;
                    }
                    return syllabus;
                },
                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
                transaction: Transaction);

            return syllabi;
        }

        public async Task<SyllabusWithDetailsDto?> GetByIdAsync(Guid id)
        {
            var sql = @"
                SELECT 
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    s.course_id as CourseId,
                    s.semester_id as SemesterId,
                    s.template_id as TemplateId,
                    s.faculty_id as FacultyId,
                    s.faculty_name as FacultyName,
                    s.credits as Credits,
                    s.duration_weeks as DurationWeeks,
                    s.content_creation_method as ContentCreationMethod,
                    s.course_description as CourseDescription,
                    s.learning_objectives as LearningObjectives,
                    s.learning_outcomes as LearningOutcomes,
                    s.course_topics as CourseTopics,
                    s.assessment_methods as AssessmentMethods,
                    s.detailed_content as DetailedContent,
                    s.reference_materials as ReferenceMaterials,
                    s.document_url as DocumentUrl,
                    s.status as Status,
                    s.is_active as IsActive,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.file_processing_status as FileProcessingStatus,
                    s.file_processing_notes as FileProcessingNotes,
                    s.original_filename as OriginalFilename,
                    -- Template
                    st.id as Template_Id,
                    st.name as Template_Name,
                    st.description as Template_Description,
                    st.template_type as Template_TemplateType,
                    st.sections as Template_Sections,
                    st.is_active as Template_IsActive,
                    st.created_date as Template_CreatedDate,
                    st.modified_date as Template_ModifiedDate,
                    st.created_by as Template_CreatedBy,
                    st.modified_by as Template_ModifiedBy,
                    -- Department
                    d.id as Department_Id,
                    d.name as Department_Name,
                    d.code as Department_Code,
                    -- Course
                    c.id as Course_Id,
                    c.name as Course_Name,
                    c.code as Course_Code,
                    -- Semester
                    sem.id as Semester_Id,
                    sem.name as Semester_Name,
                    sem.academic_year as Semester_Year,
                    -- Workflow
                    dw.id as Workflow_Id,
                    dw.status as Workflow_Status,
                    ws.id as CurrentStage_Id,
                    ws.stage_name as CurrentStage_StageName,
                    ws.assigned_role as CurrentStage_AssignedRole
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                LEFT JOIN workflowmgmt.document_workflows dw ON dw.document_id = s.id and dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE s.id = @Id";

            var result = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
                sql,
                (syllabus, template, department, course, semester, workflow, currentStage) =>
                {
                    syllabus.Template = template;
                    syllabus.Department = department;
                    syllabus.Course = course;
                    syllabus.Semester = semester;
                    if (workflow != null)
                    {
                        workflow.CurrentStage = currentStage;
                        syllabus.Workflow = workflow;
                    }
                    return syllabus;
                },
                new { Id = id },
                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
                transaction: Transaction);

            var syllabusResult = result.FirstOrDefault();

            // If syllabus has a current stage, fetch the stage actions
            if (syllabusResult?.Workflow?.CurrentStage != null)
            {
                var actionsSql = @"
                    SELECT
                        id as Id,
                        workflow_stage_id as WorkflowStageId,
                        action_name as ActionName,
                        action_type as ActionType,
                        next_stage_id as NextStageId,
                        is_active as IsActive,
                        created_date as CreatedDate
                    FROM workflowmgmt.workflow_stage_actions
                    WHERE workflow_stage_id = @StageId AND is_active = true
                    ORDER BY action_name";

                var actions = await Connection.QueryAsync<WorkflowStageActionDto>(
                    actionsSql,
                    new { StageId = syllabusResult.Workflow.CurrentStage.Id },
                    transaction: Transaction);

                syllabusResult.Workflow.CurrentStage.Actions = actions.ToArray();
            }

            return syllabusResult;
        }

        public async Task<Guid> CreateAsync(CreateSyllabusDto syllabus)
        {
            var id = Guid.NewGuid();
            var sql = @"
                INSERT INTO workflowmgmt.syllabi
                (id, title, department_id, course_id, semester_id, template_id, faculty_id, faculty_name,
                 credits, duration_weeks, content_creation_method, course_description, learning_objectives,
                 learning_outcomes, course_topics, assessment_methods, detailed_content, reference_materials,
                 document_url, original_filename, status, is_active, created_date, created_by)
                VALUES (@Id, @Title, @DepartmentId, @CourseId, @SemesterId, @TemplateId, @FacultyId, @FacultyName,
                        @Credits, @DurationWeeks, @ContentCreationMethod, @CourseDescription, @LearningObjectives,
                        @LearningOutcomes, @CourseTopics, @AssessmentMethods, @DetailedContent, @ReferenceMaterials,
                        @DocumentUrl, @OriginalFilename, 'Draft', true, @CreatedDate, @CreatedBy)";

            await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                syllabus.Title,
                syllabus.DepartmentId,
                syllabus.CourseId,
                syllabus.SemesterId,
                syllabus.TemplateId,
                syllabus.FacultyId,
                syllabus.FacultyName,
                syllabus.Credits,
                syllabus.DurationWeeks,
                syllabus.ContentCreationMethod,
                syllabus.CourseDescription,
                syllabus.LearningObjectives,
                syllabus.LearningOutcomes,
                syllabus.CourseTopics,
                syllabus.AssessmentMethods,
                syllabus.DetailedContent,
                syllabus.ReferenceMaterials,
                syllabus.DocumentUrl,
                syllabus.OriginalFilename,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return id;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateSyllabusDto syllabus)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabi
                SET title = COALESCE(@Title, title),
                    course_id = COALESCE(@CourseId, course_id),
                    semester_id = COALESCE(@SemesterId, semester_id),
                    faculty_id = COALESCE(@FacultyId, faculty_id),
                    faculty_name = COALESCE(@FacultyName, faculty_name),
                    credits = COALESCE(@Credits, credits),
                    duration_weeks = COALESCE(@DurationWeeks, duration_weeks),
                    course_description = COALESCE(@CourseDescription, course_description),
                    learning_objectives = COALESCE(@LearningObjectives, learning_objectives),
                    learning_outcomes = COALESCE(@LearningOutcomes, learning_outcomes),
                    course_topics = COALESCE(@CourseTopics, course_topics),
                    assessment_methods = COALESCE(@AssessmentMethods, assessment_methods),
                    detailed_content = COALESCE(@DetailedContent, detailed_content),
                    reference_materials = COALESCE(@ReferenceMaterials, reference_materials),
                    document_url = COALESCE(@DocumentUrl, document_url),
                    original_filename = COALESCE(@OriginalFilename, original_filename),
                    file_processing_status = COALESCE(@FileProcessingStatus, file_processing_status),
                    file_processing_notes = COALESCE(@FileProcessingNotes, file_processing_notes),
                    status = COALESCE(@Status, status),
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                syllabus.Title,
                syllabus.CourseId,
                syllabus.SemesterId,
                syllabus.FacultyId,
                syllabus.FacultyName,
                syllabus.Credits,
                syllabus.DurationWeeks,
                syllabus.CourseDescription,
                syllabus.LearningObjectives,
                syllabus.LearningOutcomes,
                syllabus.CourseTopics,
                syllabus.AssessmentMethods,
                syllabus.DetailedContent,
                syllabus.ReferenceMaterials,
                syllabus.DocumentUrl,
                syllabus.OriginalFilename,
                syllabus.FileProcessingStatus,
                syllabus.FileProcessingNotes,
                syllabus.Status,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var sql = "DELETE FROM workflowmgmt.syllabi WHERE id = @Id";
            var rowsAffected = await Connection.ExecuteAsync(sql, new { Id = id }, transaction: Transaction);
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleActiveAsync(Guid id)
        {
            var sql = @"
                UPDATE workflowmgmt.syllabi
                SET is_active = NOT is_active,
                    modified_date = @ModifiedDate,
                    modified_by = @ModifiedBy
                WHERE id = @Id";

            var rowsAffected = await Connection.ExecuteAsync(sql, new
            {
                Id = id,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = "system" // TODO: Get from current user context
            }, transaction: Transaction);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetByDepartmentAsync(int departmentId)
        {
            var sql = @"
                SELECT
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    s.course_id as CourseId,
                    s.semester_id as SemesterId,
                    s.template_id as TemplateId,
                    s.faculty_name as FacultyName,
                    s.credits as Credits,
                    s.duration_weeks as DurationWeeks,
                    s.content_creation_method as ContentCreationMethod,
                    s.course_description as CourseDescription,
                    s.learning_objectives as LearningObjectives,
                    s.learning_outcomes as LearningOutcomes,
                    s.course_topics as CourseTopics,
                    s.assessment_methods as AssessmentMethods,
                    s.detailed_content as DetailedContent,
                    s.reference_materials as ReferenceMaterials,
                    s.document_url as DocumentUrl,
                    s.status as Status,
                    s.is_active as IsActive,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.file_processing_status as FileProcessingStatus,
                    s.file_processing_notes as FileProcessingNotes,
                    s.original_filename as OriginalFilename,
                    -- Template
                    st.id as Template_Id,
                    st.name as Template_Name,
                    st.description as Template_Description,
                    st.template_type as Template_TemplateType,
                    st.sections as Template_Sections,
                    st.is_active as Template_IsActive,
                    st.created_date as Template_CreatedDate,
                    st.modified_date as Template_ModifiedDate,
                    st.created_by as Template_CreatedBy,
                    st.modified_by as Template_ModifiedBy,
                    -- Department
                    d.id as Department_Id,
                    d.name as Department_Name,
                    d.code as Department_Code,
                    -- Course
                    c.id as Course_Id,
                    c.name as Course_Name,
                    c.code as Course_Code,
                    -- Semester
                    sem.id as Semester_Id,
                    sem.name as Semester_Name,
                    sem.academic_year as Semester_Year,
                    -- Workflow
                    dw.id as Workflow_Id,
                    dw.status as Workflow_Status,
                    ws.id as CurrentStage_Id,
                    ws.stage_name as CurrentStage_StageName,
                    ws.assigned_role as CurrentStage_AssignedRole
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                LEFT JOIN workflowmgmt.document_workflows dw ON s.id = dw.document_id AND dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE s.department_id = @DepartmentId
                ORDER BY s.created_date DESC";

            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
                sql,
                (syllabus, template, department, course, semester, workflow, currentStage) =>
                {
                    syllabus.Template = template;
                    syllabus.Department = department;
                    syllabus.Course = course;
                    syllabus.Semester = semester;
                    if (workflow != null)
                    {
                        workflow.CurrentStage = currentStage;
                        syllabus.Workflow = workflow;
                    }
                    return syllabus;
                },
                new { DepartmentId = departmentId },
                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
                transaction: Transaction);

            return syllabi;
        }
        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetByStatusAsync(string status)
        {
            var sql = @"
                SELECT 
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    s.course_id as CourseId,
                    s.semester_id as SemesterId,
                    s.template_id as TemplateId,
                    s.faculty_id as FacultyId,
                    s.faculty_name as FacultyName,
                    s.credits as Credits,
                    s.duration_weeks as DurationWeeks,
                    s.content_creation_method as ContentCreationMethod,
                    s.course_description as CourseDescription,
                    s.learning_objectives as LearningObjectives,
                    s.learning_outcomes as LearningOutcomes,
                    s.course_topics as CourseTopics,
                    s.assessment_methods as AssessmentMethods,
                    s.detailed_content as DetailedContent,
                    s.reference_materials as ReferenceMaterials,
                    s.document_url as DocumentUrl,
                    s.status as Status,
                    s.is_active as IsActive,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.file_processing_status as FileProcessingStatus,
                    s.file_processing_notes as FileProcessingNotes,
                    s.original_filename as OriginalFilename,
                    -- Template
                    st.id as Template_Id,
                    st.name as Template_Name,
                    st.description as Template_Description,
                    st.template_type as Template_TemplateType,
                    st.sections as Template_Sections,
                    st.is_active as Template_IsActive,
                    st.created_date as Template_CreatedDate,
                    st.modified_date as Template_ModifiedDate,
                    st.created_by as Template_CreatedBy,
                    st.modified_by as Template_ModifiedBy,
                    -- Department
                    d.id as Department_Id,
                    d.name as Department_Name,
                    d.code as Department_Code,
                    -- Course
                    c.id as Course_Id,
                    c.name as Course_Name,
                    c.code as Course_Code,
                    -- Semester
                    sem.id as Semester_Id,
                    sem.name as Semester_Name,
                    sem.academic_year as Semester_Year,
                    -- Workflow
                    dw.id as Workflow_Id,
                    dw.status as Workflow_Status,
                    ws.id as CurrentStage_Id,
                    ws.stage_name as CurrentStage_StageName,
                    ws.assigned_role as CurrentStage_AssignedRole
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                LEFT JOIN workflowmgmt.document_workflows dw ON s.id = dw.document_id AND dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE s.status = @Status
                ORDER BY s.created_date DESC";

            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
                sql,
                (syllabus, template, department, course, semester, workflow, currentStage) =>
                {
                    syllabus.Template = template;
                    syllabus.Department = department;
                    syllabus.Course = course;
                    syllabus.Semester = semester;
                    if (workflow != null)
                    {
                        workflow.CurrentStage = currentStage;
                        syllabus.Workflow = workflow;
                    }
                    return syllabus;
                },
                new { Status = status },
                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
                transaction: Transaction);

            return syllabi;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetByFacultyAsync(string facultyName)
        {
            var sql = @"
                SELECT 
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    s.course_id as CourseId,
                    s.semester_id as SemesterId,
                    s.template_id as TemplateId,
                    s.faculty_name as FacultyName,
                    s.credits as Credits,
                    s.duration_weeks as DurationWeeks,
                    s.content_creation_method as ContentCreationMethod,
                    s.course_description as CourseDescription,
                    s.learning_objectives as LearningObjectives,
                    s.learning_outcomes as LearningOutcomes,
                    s.course_topics as CourseTopics,
                    s.assessment_methods as AssessmentMethods,
                    s.detailed_content as DetailedContent,
                    s.reference_materials as ReferenceMaterials,
                    s.document_url as DocumentUrl,
                    s.status as Status,
                    s.is_active as IsActive,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.file_processing_status as FileProcessingStatus,
                    s.file_processing_notes as FileProcessingNotes,
                    s.original_filename as OriginalFilename,
                    -- Template
                    st.id as Template_Id,
                    st.name as Template_Name,
                    st.description as Template_Description,
                    st.template_type as Template_TemplateType,
                    st.sections as Template_Sections,
                    st.is_active as Template_IsActive,
                    st.created_date as Template_CreatedDate,
                    st.modified_date as Template_ModifiedDate,
                    st.created_by as Template_CreatedBy,
                    st.modified_by as Template_ModifiedBy,
                    -- Department
                    d.id as Department_Id,
                    d.name as Department_Name,
                    d.code as Department_Code,
                    -- Course
                    c.id as Course_Id,
                    c.name as Course_Name,
                    c.code as Course_Code,
                    -- Semester
                    sem.id as Semester_Id,
                    sem.name as Semester_Name,
                    sem.academic_year as Semester_Year,
                    -- Workflow
                    dw.id as Workflow_Id,
                    dw.status as Workflow_Status,
                    ws.id as CurrentStage_Id,
                    ws.stage_name as CurrentStage_StageName,
                    ws.assigned_role as CurrentStage_AssignedRole
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                LEFT JOIN workflowmgmt.document_workflows dw ON s.id = dw.document_id AND dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE s.faculty_name ILIKE @FacultyName
                ORDER BY s.created_date DESC";

            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
                sql,
                (syllabus, template, department, course, semester, workflow, currentStage) =>
                {
                    syllabus.Template = template;
                    syllabus.Department = department;
                    syllabus.Course = course;
                    syllabus.Semester = semester;
                    if (workflow != null)
                    {
                        workflow.CurrentStage = currentStage;
                        syllabus.Workflow = workflow;
                    }
                    return syllabus;
                },
                new { FacultyName = $"%{facultyName}%" },
                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
                transaction: Transaction);

            return syllabi;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetByTemplateAsync(Guid templateId)
        {
            var sql = @"
                SELECT 
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    s.course_id as CourseId,
                    s.semester_id as SemesterId,
                    s.template_id as TemplateId,
                    s.faculty_id as FacultyId,
                    s.faculty_name as FacultyName,
                    s.credits as Credits,
                    s.duration_weeks as DurationWeeks,
                    s.content_creation_method as ContentCreationMethod,
                    s.course_description as CourseDescription,
                    s.learning_objectives as LearningObjectives,
                    s.learning_outcomes as LearningOutcomes,
                    s.course_topics as CourseTopics,
                    s.assessment_methods as AssessmentMethods,
                    s.detailed_content as DetailedContent,
                    s.reference_materials as ReferenceMaterials,
                    s.document_url as DocumentUrl,
                    s.status as Status,
                    s.is_active as IsActive,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.file_processing_status as FileProcessingStatus,
                    s.file_processing_notes as FileProcessingNotes,
                    s.original_filename as OriginalFilename,
                    -- Template
                    st.id as Template_Id,
                    st.name as Template_Name,
                    st.description as Template_Description,
                    st.template_type as Template_TemplateType,
                    st.sections as Template_Sections,
                    st.is_active as Template_IsActive,
                    st.created_date as Template_CreatedDate,
                    st.modified_date as Template_ModifiedDate,
                    st.created_by as Template_CreatedBy,
                    st.modified_by as Template_ModifiedBy,
                    -- Department
                    d.id as Department_Id,
                    d.name as Department_Name,
                    d.code as Department_Code,
                    -- Course
                    c.id as Course_Id,
                    c.name as Course_Name,
                    c.code as Course_Code,
                    -- Semester
                    sem.id as Semester_Id,
                    sem.name as Semester_Name,
                    sem.academic_year as Semester_Year,
                    -- Workflow
                    dw.id as Workflow_Id,
                    dw.status as Workflow_Status,
                    ws.id as CurrentStage_Id,
                    ws.stage_name as CurrentStage_StageName,
                    ws.assigned_role as CurrentStage_AssignedRole
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                LEFT JOIN workflowmgmt.document_workflows dw ON s.id = dw.document_id AND dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE s.template_id = @TemplateId
                ORDER BY s.created_date DESC";

            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
                sql,
                (syllabus, template, department, course, semester, workflow, currentStage) =>
                {
                    syllabus.Template = template;
                    syllabus.Department = department;
                    syllabus.Course = course;
                    syllabus.Semester = semester;
                    if (workflow != null)
                    {
                        workflow.CurrentStage = currentStage;
                        syllabus.Workflow = workflow;
                    }
                    return syllabus;
                },
                new { TemplateId = templateId },
                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
                transaction: Transaction);

            return syllabi;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetByCourseAsync(int courseId)
        {
            var sql = @"
                SELECT
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    s.course_id as CourseId,
                    s.semester_id as SemesterId,
                    s.template_id as TemplateId,
                    s.faculty_id as FacultyId,
                    s.faculty_name as FacultyName,
                    s.credits as Credits,
                    s.duration_weeks as DurationWeeks,
                    s.content_creation_method as ContentCreationMethod,
                    s.course_description as CourseDescription,
                    s.learning_objectives as LearningObjectives,
                    s.learning_outcomes as LearningOutcomes,
                    s.course_topics as CourseTopics,
                    s.assessment_methods as AssessmentMethods,
                    s.detailed_content as DetailedContent,
                    s.reference_materials as ReferenceMaterials,
                    s.document_url as DocumentUrl,
                    s.status as Status,
                    s.is_active as IsActive,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.file_processing_status as FileProcessingStatus,
                    s.file_processing_notes as FileProcessingNotes,
                    s.original_filename as OriginalFilename,
                    -- Template
                    st.id as Template_Id,
                    st.name as Template_Name,
                    st.description as Template_Description,
                    st.template_type as Template_TemplateType,
                    st.sections as Template_Sections,
                    st.is_active as Template_IsActive,
                    st.created_date as Template_CreatedDate,
                    st.modified_date as Template_ModifiedDate,
                    st.created_by as Template_CreatedBy,
                    st.modified_by as Template_ModifiedBy,
                    -- Department
                    d.id as Department_Id,
                    d.name as Department_Name,
                    d.code as Department_Code,
                    -- Course
                    c.id as Course_Id,
                    c.name as Course_Name,
                    c.code as Course_Code,
                    -- Semester
                    sem.id as Semester_Id,
                    sem.name as Semester_Name,
                    sem.academic_year as Semester_Year,
                    -- Workflow
                    dw.id as Workflow_Id,
                    dw.status as Workflow_Status,
                    ws.id as CurrentStage_Id,
                    ws.stage_name as CurrentStage_StageName,
                    ws.assigned_role as CurrentStage_AssignedRole
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                LEFT JOIN workflowmgmt.document_workflows dw ON s.id = dw.document_id AND dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE s.course_id = @CourseId
                ORDER BY s.created_date DESC";

            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
                sql,
                (syllabus, template, department, course, semester, workflow, currentStage) =>
                {
                    syllabus.Template = template;
                    syllabus.Department = department;
                    syllabus.Course = course;
                    syllabus.Semester = semester;
                    if (workflow != null)
                    {
                        workflow.CurrentStage = currentStage;
                        syllabus.Workflow = workflow;
                    }
                    return syllabus;
                },
                new { CourseId = courseId },
                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
                transaction: Transaction);

            return syllabi;
        }

        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetBySemesterAsync(int semesterId)
        {
            var sql = @"
                SELECT
                    s.id as Id,
                    s.title as Title,
                    s.department_id as DepartmentId,
                    s.course_id as CourseId,
                    s.semester_id as SemesterId,
                    s.template_id as TemplateId,
                    s.faculty_id as FacultyId,
                    s.faculty_name as FacultyName,
                    s.credits as Credits,
                    s.duration_weeks as DurationWeeks,
                    s.content_creation_method as ContentCreationMethod,
                    s.course_description as CourseDescription,
                    s.learning_objectives as LearningObjectives,
                    s.learning_outcomes as LearningOutcomes,
                    s.course_topics as CourseTopics,
                    s.assessment_methods as AssessmentMethods,
                    s.detailed_content as DetailedContent,
                    s.reference_materials as ReferenceMaterials,
                    s.document_url as DocumentUrl,
                    s.status as Status,
                    s.is_active as IsActive,
                    s.created_date as CreatedDate,
                    s.modified_date as ModifiedDate,
                    s.created_by as CreatedBy,
                    s.modified_by as ModifiedBy,
                    s.file_processing_status as FileProcessingStatus,
                    s.file_processing_notes as FileProcessingNotes,
                    s.original_filename as OriginalFilename,
                    -- Template
                    st.id as Template_Id,
                    st.name as Template_Name,
                    st.description as Template_Description,
                    st.template_type as Template_TemplateType,
                    st.sections as Template_Sections,
                    st.is_active as Template_IsActive,
                    st.created_date as Template_CreatedDate,
                    st.modified_date as Template_ModifiedDate,
                    st.created_by as Template_CreatedBy,
                    st.modified_by as Template_ModifiedBy,
                    -- Department
                    d.id as Department_Id,
                    d.name as Department_Name,
                    d.code as Department_Code,
                    -- Course
                    c.id as Course_Id,
                    c.name as Course_Name,
                    c.code as Course_Code,
                    -- Semester
                    sem.id as Semester_Id,
                    sem.name as Semester_Name,
                    sem.academic_year as Semester_Year,
                    -- Workflow
                    dw.id as Workflow_Id,
                    dw.status as Workflow_Status,
                    ws.id as CurrentStage_Id,
                    ws.stage_name as CurrentStage_StageName,
                    ws.assigned_role as CurrentStage_AssignedRole
                FROM workflowmgmt.syllabi s
                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
                LEFT JOIN workflowmgmt.document_workflows dw ON s.id = dw.document_id AND dw.document_type = 'syllabus'
                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
                WHERE s.semester_id = @SemesterId
                ORDER BY s.created_date DESC";

            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
                sql,
                (syllabus, template, department, course, semester, workflow, currentStage) =>
                {
                    syllabus.Template = template;
                    syllabus.Department = department;
                    syllabus.Course = course;
                    syllabus.Semester = semester;
                    if (workflow != null)
                    {
                        workflow.CurrentStage = currentStage;
                        syllabus.Workflow = workflow;
                    }
                    return syllabus;
                },
                new { SemesterId = semesterId },
                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
                transaction: Transaction);

            return syllabi;
        }
    }
}

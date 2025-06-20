//using Dapper;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using WorkflowMgmt.Domain.Models.Syllabus;
//using WorkflowMgmt.Infrastructure.RepositoryBase;

//namespace WorkflowMgmt.Infrastructure.Repository
//{
//    public partial class SyllabusManagementRepository
//    {
//        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetByStatusAsync(string status)
//        {
//            var sql = @"
//                SELECT 
//                    s.id as Id,
//                    s.title as Title,
//                    s.department_id as DepartmentId,
//                    s.course_id as CourseId,
//                    s.semester_id as SemesterId,
//                    s.template_id as TemplateId,
//                    s.faculty_name as FacultyName,
//                    s.faculty_email as FacultyEmail,
//                    s.credits as Credits,
//                    s.duration_weeks as DurationWeeks,
//                    s.content_creation_method as ContentCreationMethod,
//                    s.course_description as CourseDescription,
//                    s.learning_objectives as LearningObjectives,
//                    s.learning_outcomes as LearningOutcomes,
//                    s.course_topics as CourseTopics,
//                    s.assessment_methods as AssessmentMethods,
//                    s.detailed_content as DetailedContent,
//                    s.reference_materials as ReferenceMaterials,
//                    s.document_url as DocumentUrl,
//                    s.status as Status,
//                    s.workflow_id as WorkflowId,
//                    s.is_active as IsActive,
//                    s.created_date as CreatedDate,
//                    s.modified_date as ModifiedDate,
//                    s.created_by as CreatedBy,
//                    s.modified_by as ModifiedBy,
//                    s.file_processing_status as FileProcessingStatus,
//                    s.file_processing_notes as FileProcessingNotes,
//                    s.original_filename as OriginalFilename,
//                    -- Template
//                    st.id as Template_Id,
//                    st.name as Template_Name,
//                    st.description as Template_Description,
//                    st.template_type as Template_TemplateType,
//                    st.sections as Template_Sections,
//                    st.is_active as Template_IsActive,
//                    st.created_date as Template_CreatedDate,
//                    st.modified_date as Template_ModifiedDate,
//                    st.created_by as Template_CreatedBy,
//                    st.modified_by as Template_ModifiedBy,
//                    -- Department
//                    d.id as Department_Id,
//                    d.name as Department_Name,
//                    d.code as Department_Code,
//                    -- Course
//                    c.id as Course_Id,
//                    c.name as Course_Name,
//                    c.code as Course_Code,
//                    -- Semester
//                    sem.id as Semester_Id,
//                    sem.name as Semester_Name,
//                    sem.year as Semester_Year,
//                    -- Workflow
//                    dw.id as Workflow_Id,
//                    dw.status as Workflow_Status,
//                    ws.id as CurrentStage_Id,
//                    ws.stage_name as CurrentStage_StageName,
//                    ws.assigned_role as CurrentStage_AssignedRole
//                FROM workflowmgmt.syllabi s
//                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
//                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
//                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
//                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
//                LEFT JOIN workflowmgmt.document_workflows dw ON s.workflow_id = dw.id
//                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
//                WHERE s.status = @Status
//                ORDER BY s.created_date DESC";

//            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
//                sql,
//                (syllabus, template, department, course, semester, workflow, currentStage) =>
//                {
//                    syllabus.Template = template;
//                    syllabus.Department = department;
//                    syllabus.Course = course;
//                    syllabus.Semester = semester;
//                    if (workflow != null)
//                    {
//                        workflow.CurrentStage = currentStage;
//                        syllabus.Workflow = workflow;
//                    }
//                    return syllabus;
//                },
//                new { Status = status },
//                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
//                transaction: Transaction);

//            return syllabi;
//        }

//        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetByFacultyAsync(string facultyName)
//        {
//            var sql = @"
//                SELECT 
//                    s.id as Id,
//                    s.title as Title,
//                    s.department_id as DepartmentId,
//                    s.course_id as CourseId,
//                    s.semester_id as SemesterId,
//                    s.template_id as TemplateId,
//                    s.faculty_name as FacultyName,
//                    s.faculty_email as FacultyEmail,
//                    s.credits as Credits,
//                    s.duration_weeks as DurationWeeks,
//                    s.content_creation_method as ContentCreationMethod,
//                    s.course_description as CourseDescription,
//                    s.learning_objectives as LearningObjectives,
//                    s.learning_outcomes as LearningOutcomes,
//                    s.course_topics as CourseTopics,
//                    s.assessment_methods as AssessmentMethods,
//                    s.detailed_content as DetailedContent,
//                    s.reference_materials as ReferenceMaterials,
//                    s.document_url as DocumentUrl,
//                    s.status as Status,
//                    s.workflow_id as WorkflowId,
//                    s.is_active as IsActive,
//                    s.created_date as CreatedDate,
//                    s.modified_date as ModifiedDate,
//                    s.created_by as CreatedBy,
//                    s.modified_by as ModifiedBy,
//                    s.file_processing_status as FileProcessingStatus,
//                    s.file_processing_notes as FileProcessingNotes,
//                    s.original_filename as OriginalFilename,
//                    -- Template
//                    st.id as Template_Id,
//                    st.name as Template_Name,
//                    st.description as Template_Description,
//                    st.template_type as Template_TemplateType,
//                    st.sections as Template_Sections,
//                    st.is_active as Template_IsActive,
//                    st.created_date as Template_CreatedDate,
//                    st.modified_date as Template_ModifiedDate,
//                    st.created_by as Template_CreatedBy,
//                    st.modified_by as Template_ModifiedBy,
//                    -- Department
//                    d.id as Department_Id,
//                    d.name as Department_Name,
//                    d.code as Department_Code,
//                    -- Course
//                    c.id as Course_Id,
//                    c.name as Course_Name,
//                    c.code as Course_Code,
//                    -- Semester
//                    sem.id as Semester_Id,
//                    sem.name as Semester_Name,
//                    sem.year as Semester_Year,
//                    -- Workflow
//                    dw.id as Workflow_Id,
//                    dw.status as Workflow_Status,
//                    ws.id as CurrentStage_Id,
//                    ws.stage_name as CurrentStage_StageName,
//                    ws.assigned_role as CurrentStage_AssignedRole
//                FROM workflowmgmt.syllabi s
//                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
//                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
//                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
//                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
//                LEFT JOIN workflowmgmt.document_workflows dw ON s.workflow_id = dw.id
//                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
//                WHERE s.faculty_name ILIKE @FacultyName
//                ORDER BY s.created_date DESC";

//            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
//                sql,
//                (syllabus, template, department, course, semester, workflow, currentStage) =>
//                {
//                    syllabus.Template = template;
//                    syllabus.Department = department;
//                    syllabus.Course = course;
//                    syllabus.Semester = semester;
//                    if (workflow != null)
//                    {
//                        workflow.CurrentStage = currentStage;
//                        syllabus.Workflow = workflow;
//                    }
//                    return syllabus;
//                },
//                new { FacultyName = $"%{facultyName}%" },
//                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
//                transaction: Transaction);

//            return syllabi;
//        }

//        public async Task<IEnumerable<SyllabusWithDetailsDto>> GetByTemplateAsync(Guid templateId)
//        {
//            var sql = @"
//                SELECT 
//                    s.id as Id,
//                    s.title as Title,
//                    s.department_id as DepartmentId,
//                    s.course_id as CourseId,
//                    s.semester_id as SemesterId,
//                    s.template_id as TemplateId,
//                    s.faculty_name as FacultyName,
//                    s.faculty_email as FacultyEmail,
//                    s.credits as Credits,
//                    s.duration_weeks as DurationWeeks,
//                    s.content_creation_method as ContentCreationMethod,
//                    s.course_description as CourseDescription,
//                    s.learning_objectives as LearningObjectives,
//                    s.learning_outcomes as LearningOutcomes,
//                    s.course_topics as CourseTopics,
//                    s.assessment_methods as AssessmentMethods,
//                    s.detailed_content as DetailedContent,
//                    s.reference_materials as ReferenceMaterials,
//                    s.document_url as DocumentUrl,
//                    s.status as Status,
//                    s.workflow_id as WorkflowId,
//                    s.is_active as IsActive,
//                    s.created_date as CreatedDate,
//                    s.modified_date as ModifiedDate,
//                    s.created_by as CreatedBy,
//                    s.modified_by as ModifiedBy,
//                    s.file_processing_status as FileProcessingStatus,
//                    s.file_processing_notes as FileProcessingNotes,
//                    s.original_filename as OriginalFilename,
//                    -- Template
//                    st.id as Template_Id,
//                    st.name as Template_Name,
//                    st.description as Template_Description,
//                    st.template_type as Template_TemplateType,
//                    st.sections as Template_Sections,
//                    st.is_active as Template_IsActive,
//                    st.created_date as Template_CreatedDate,
//                    st.modified_date as Template_ModifiedDate,
//                    st.created_by as Template_CreatedBy,
//                    st.modified_by as Template_ModifiedBy,
//                    -- Department
//                    d.id as Department_Id,
//                    d.name as Department_Name,
//                    d.code as Department_Code,
//                    -- Course
//                    c.id as Course_Id,
//                    c.name as Course_Name,
//                    c.code as Course_Code,
//                    -- Semester
//                    sem.id as Semester_Id,
//                    sem.name as Semester_Name,
//                    sem.year as Semester_Year,
//                    -- Workflow
//                    dw.id as Workflow_Id,
//                    dw.status as Workflow_Status,
//                    ws.id as CurrentStage_Id,
//                    ws.stage_name as CurrentStage_StageName,
//                    ws.assigned_role as CurrentStage_AssignedRole
//                FROM workflowmgmt.syllabi s
//                INNER JOIN workflowmgmt.syllabus_templates st ON s.template_id = st.id
//                INNER JOIN workflowmgmt.departments d ON s.department_id = d.id
//                LEFT JOIN workflowmgmt.courses c ON s.course_id = c.id
//                LEFT JOIN workflowmgmt.semesters sem ON s.semester_id = sem.id
//                LEFT JOIN workflowmgmt.document_workflows dw ON s.workflow_id = dw.id
//                LEFT JOIN workflowmgmt.workflow_stages ws ON dw.current_stage_id = ws.id
//                WHERE s.template_id = @TemplateId
//                ORDER BY s.created_date DESC";

//            var syllabi = await Connection.QueryAsync<SyllabusWithDetailsDto, SyllabusTemplateDto, DepartmentDto, CourseDto, SemesterDto, WorkflowDto, CurrentStageDto, SyllabusWithDetailsDto>(
//                sql,
//                (syllabus, template, department, course, semester, workflow, currentStage) =>
//                {
//                    syllabus.Template = template;
//                    syllabus.Department = department;
//                    syllabus.Course = course;
//                    syllabus.Semester = semester;
//                    if (workflow != null)
//                    {
//                        workflow.CurrentStage = currentStage;
//                        syllabus.Workflow = workflow;
//                    }
//                    return syllabus;
//                },
//                new { TemplateId = templateId },
//                splitOn: "Template_Id,Department_Id,Course_Id,Semester_Id,Workflow_Id,CurrentStage_Id",
//                transaction: Transaction);

//            return syllabi;
//        }
//    }
//}

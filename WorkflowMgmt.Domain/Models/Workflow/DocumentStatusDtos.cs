using System;
using System.Collections.Generic;

namespace WorkflowMgmt.Domain.Models.Workflow
{
    public class DocumentStatusDto
    {
        public Guid DocumentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        // Workflow information
        public Guid? WorkflowId { get; set; }
        public string? WorkflowTemplateName { get; set; }
        public Guid? WorkflowTemplateId { get; set; }
        public Guid? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public int? CurrentStageOrder { get; set; }
        
        // User involvement
        public bool IsAssignedTo { get; set; }
        public bool IsProcessedBy { get; set; }
        public DateTime? LastActionDate { get; set; }
        public string? LastActionTaken { get; set; }
        
        // Progress information
        public int Progress { get; set; }
        public int TotalStages { get; set; }
        public int CompletedStages { get; set; }
        
        // Additional metadata
        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Version { get; set; }
        public int? Comments { get; set; }
    }

    public class DocumentStatusDetailDto
    {
        // Basic document info
        public string DocumentId { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        
        // Document-specific details (joined from respective tables)
        public SyllabusDetailsDto? SyllabusDetails { get; set; }
        public LessonDetailsDto? LessonDetails { get; set; }
        public SessionDetailsDto? SessionDetails { get; set; }
        
        // Workflow information
        public string WorkflowId { get; set; } = string.Empty;
        public string WorkflowTemplateName { get; set; } = string.Empty;
        public string WorkflowTemplateId { get; set; } = string.Empty;
        public string? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public int? CurrentStageOrder { get; set; }
        
        // Progress and timeline
        public int Progress { get; set; }
        public int TotalStages { get; set; }
        public int CompletedStages { get; set; }
        public DateTime InitiatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        
        // User involvement history
        public List<DocumentUserHistoryDto> UserHistory { get; set; } = new();
        
        // Workflow roadmap
        public List<WorkflowRoadmapDto> WorkflowRoadmap { get; set; } = new();
    }

    public class SyllabusDetailsDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Semester { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public string? OriginalFilename { get; set; }
    }

    public class LessonDetailsDto
    {
        public string LessonTitle { get; set; } = string.Empty;
        public string SyllabusId { get; set; } = string.Empty;
        public string SyllabusTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public string? OriginalFilename { get; set; }
    }

    public class SessionDetailsDto
    {
        public string SessionTitle { get; set; } = string.Empty;
        public string LessonPlanId { get; set; } = string.Empty;
        public string LessonPlanTitle { get; set; } = string.Empty;
        public string SyllabusId { get; set; } = string.Empty;
        public string SyllabusTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string DepartmentId { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string? DocumentUrl { get; set; }
        public string? OriginalFilename { get; set; }
    }

    public class DocumentUserHistoryDto
    {
        public string Id { get; set; } = string.Empty;
        public string StageId { get; set; } = string.Empty;
        public string StageName { get; set; } = string.Empty;
        public int StageOrder { get; set; }
        public string ActionTaken { get; set; } = string.Empty;
        public string ProcessedBy { get; set; } = string.Empty;
        public string ProcessedByName { get; set; } = string.Empty;
        public string? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime ProcessedDate { get; set; }
        public string? Comments { get; set; }
        public string[]? Attachments { get; set; }
    }

    public class WorkflowRoadmapDto
    {
        public string StageId { get; set; } = string.Empty;
        public string StageName { get; set; } = string.Empty;
        public int StageOrder { get; set; }
        public string? Description { get; set; }
        public string AssignedRole { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool AutoApprove { get; set; }
        public int? TimeoutDays { get; set; }
        public bool IsActive { get; set; }
        
        // Status for this document
        public string Status { get; set; } = "pending"; // completed, current, pending
        public DateTime? CompletedDate { get; set; }
        public string? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        
        // Available actions (if current stage)
        public List<WorkflowStageActionDto> AvailableActions { get; set; } = new();
    }

    public class DocumentStatusStatsDto
    {
        public int TotalDocuments { get; set; }
        public int UnderReview { get; set; }
        public int Approved { get; set; }
        public int RevisionRequired { get; set; }
        public int Drafts { get; set; }
    }
}

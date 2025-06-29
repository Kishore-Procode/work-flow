using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.IRepository;

namespace WorkflowMgmt.Domain.Interface.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }
        ICourseRepository CourseRepository { get; }
        ISemesterRepository SemesterRepository { get; }
        IUserManagementRepository UserManagementRepository { get; }
        ILessonPlanTemplateRepository LessonPlanTemplateRepository { get; }
        ILessonPlanRepository LessonPlanRepository { get; }
        ISessionRepository SessionRepository { get; }
        IDocumentUploadRepository DocumentUploadRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IStatsRepository StatsRepository { get; }
        IWorkflowTemplateRepository WorkflowTemplateRepository { get; }
        IWorkflowStageRepository WorkflowStageRepository { get; }
        IWorkflowStageActionRepository WorkflowStageActionRepository { get; }
        IDocumentWorkflowRepository DocumentWorkflowRepository { get; }
        IWorkflowUserRepository WorkflowUserRepository { get; }
        IWorkflowRoleRepository WorkflowRoleRepository { get; }
        IStageAssignmentRepository StageAssignmentRepository { get; }
        IWorkflowStageHistoryRepository WorkflowStageHistoryRepository { get; }
        IWorkflowStageRoleRepository WorkflowStageRoleRepository { get; }
        IDocumentFeedbackRepository DocumentFeedbackRepository { get; }
        IDocumentLifecycleRepository DocumentLifecycleRepository { get; }
        IWorkflowStageDetailsRepository WorkflowStageDetailsRepository { get; }
        ISyllabusRepository SyllabusRepository { get; }
        ISyllabusTemplateRepository SyllabusTemplateRepository { get; }
        IWorkflowRoleMappingRepository WorkflowRoleMappingRepository { get; }
        IWorkflowDepartmentDocumentMappingRepository WorkflowDepartmentDocumentMappingRepository { get; }
        INotificationRepository NotificationRepository { get; }
        void Begin();
        void Commit();
        void Rollback();
        Task SaveAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }

}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using WorkflowMgmt.Domain.Interface.IRepository;
using WorkflowMgmt.Domain.Interface.IUnitOfWork;
using WorkflowMgmt.Domain.IRepository;
using WorkflowMgmt.Infrastructure.ConnectionFactory;
using WorkflowMgmt.Infrastructure.Repository;

namespace WorkflowMgmt.Infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        private IUserRepository? userRepository;
        private IDepartmentRepository? departmentRepository;
        private ICourseRepository? courseRepository;
        private ISemesterRepository? semesterRepository;
        private ILessonPlanTemplateRepository lessonPlanTemplateRepository;
        private IRefreshTokenRepository? refreshTokenRepository;

        private IUserManagementRepository? userManagementRepository;
        private IStatsRepository statsRepository;
        private IWorkflowTemplateRepository? workflowTemplateRepository;
        private IWorkflowStageRepository? workflowStageRepository;
        private IWorkflowStageActionRepository? workflowStageActionRepository;
        private IDocumentWorkflowRepository? documentWorkflowRepository;
        private IWorkflowUserRepository? workflowUserRepository;
        private IWorkflowRoleRepository? workflowRoleRepository;
        private IStageAssignmentRepository? stageAssignmentRepository;
        private IWorkflowStageHistoryRepository? workflowStageHistoryRepository;
        private ISyllabusRepository? syllabusRepository;
        private ISyllabusTemplateRepository? syllabusTemplateRepository;

        public UnitOfWork(IDbConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }
        public IUserRepository UserRepository
        {
            get { return userRepository ?? (userRepository = new UserRepository(_transaction)); }
        }
        public IDepartmentRepository DepartmentRepository
        {
            get { return departmentRepository ?? (departmentRepository = new DepartmentRepository(_transaction)); }
        }
        public ICourseRepository CourseRepository
        {
            get { return courseRepository ?? (courseRepository = new CourseRepository(_transaction)); }
        }
        
        public ISemesterRepository SemesterRepository
        {
            get { return semesterRepository ?? (semesterRepository = new SemesterRepository(_transaction)); }
        }

        public IUserManagementRepository UserManagementRepository
        {
            get { return userManagementRepository ?? (userManagementRepository = new UserManagementRepository(_transaction)); }
        }

        public ILessonPlanTemplateRepository LessonPlanTemplateRepository
        {
            get
            {
                return lessonPlanTemplateRepository ??
                       (lessonPlanTemplateRepository = new LessonPlanTemplateRepository(_transaction));
            }
        }

        public IRefreshTokenRepository RefreshTokenRepository
        {
            get { return refreshTokenRepository ?? (refreshTokenRepository = new RefreshTokenRepository(_transaction)); }
        }

        public IStatsRepository StatsRepository
        {
            get { return statsRepository ?? (statsRepository = new StatsRepository(_transaction)); }
        }

        public IWorkflowTemplateRepository WorkflowTemplateRepository
        {
            get { return workflowTemplateRepository ?? (workflowTemplateRepository = new WorkflowTemplateRepository(_transaction)); }
        }

        public IWorkflowStageRepository WorkflowStageRepository
        {
            get { return workflowStageRepository ?? (workflowStageRepository = new WorkflowStageRepository(_transaction)); }
        }

        public IWorkflowStageActionRepository WorkflowStageActionRepository
        {
            get { return workflowStageActionRepository ?? (workflowStageActionRepository = new WorkflowStageActionRepository(_transaction)); }
        }

        public IDocumentWorkflowRepository DocumentWorkflowRepository
        {
            get { return documentWorkflowRepository ?? (documentWorkflowRepository = new DocumentWorkflowRepository(_transaction)); }
        }

        public ISyllabusRepository SyllabusRepository
        {
            get { return syllabusRepository ?? (syllabusRepository = new SyllabusManagementRepository(_transaction)); }
        }

        public IWorkflowUserRepository WorkflowUserRepository
        {
            get { return workflowUserRepository ?? (workflowUserRepository = new WorkflowUserRepository(_transaction)); }
        }

        public IWorkflowRoleRepository WorkflowRoleRepository
        {
            get { return workflowRoleRepository ?? (workflowRoleRepository = new WorkflowRoleRepository(_transaction)); }
        }

        public IStageAssignmentRepository StageAssignmentRepository
        {
            get { return stageAssignmentRepository ?? (stageAssignmentRepository = new StageAssignmentRepository(_transaction)); }
        }

        public IWorkflowStageHistoryRepository WorkflowStageHistoryRepository
        {
            get { return workflowStageHistoryRepository ?? (workflowStageHistoryRepository = new WorkflowStageHistoryRepository(_transaction)); }
        }

        public ISyllabusTemplateRepository SyllabusTemplateRepository
        {
            get { return syllabusTemplateRepository ?? (syllabusTemplateRepository = new SyllabusTemplateManagementRepository(_transaction)); }
        }

        public void Begin()
        {
            if (_transaction == null)
                _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            _transaction?.Commit();
            _connection?.Close();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _connection?.Close();
        }

        public async Task SaveAsync()
        {
            // For now, just commit the transaction
            // In the future, this could be extended to handle async operations
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }

}

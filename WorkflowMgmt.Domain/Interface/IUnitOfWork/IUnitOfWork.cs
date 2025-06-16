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

        void Begin();
        void Commit();
        void Rollback();
    }

}

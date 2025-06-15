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

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }

}

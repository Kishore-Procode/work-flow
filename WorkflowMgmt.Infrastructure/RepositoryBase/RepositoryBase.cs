using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowMgmt.Infrastructure.RepositoryBase
{
    public class RepositoryTranBase
    {
        protected IDbTransaction Transaction { get; }
        protected IDbConnection Connection =>
            Transaction?.Connection ?? throw new InvalidOperationException("Transaction is null.");

        protected RepositoryTranBase(IDbTransaction transaction)
        {
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }
    }
}

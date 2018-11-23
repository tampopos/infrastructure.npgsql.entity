using System.Data;
using System.Threading;
using Tmpps.Infrastructure.Data.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity.Wrapper
{
    public class TransactionWrapper : ITransaction
    {
        private IDbTransaction dbTransaction;
        private CancellationTokenSource tokenSource;

        public TransactionWrapper(IDbTransaction dbTransaction, CancellationTokenSource tokenSource)
        {
            this.dbTransaction = dbTransaction;
            this.tokenSource = tokenSource;
        }

        private CancellationToken CancellationToken => this.tokenSource.Token;

        public void Commit()
        {
            this.CancellationToken.ThrowIfCancellationRequested();
            this.dbTransaction.Commit();
        }

        public void Dispose()
        {
            this.dbTransaction.Dispose();
        }

        public void Rollback()
        {
            this.dbTransaction.Rollback();
        }
    }
}
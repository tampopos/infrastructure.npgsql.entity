using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tmpps.Infrastructure.Data.Entity.Interfaces;
using Tmpps.Infrastructure.Data.Interfaces;
using Tmpps.Infrastructure.Npgsql.Entity.Extensions;

namespace Tmpps.Infrastructure.Npgsql.Entity.Wrapper
{
    public class DbContextWrapper<TContext> : IDbContext where TContext : DbContext
    {
        private TContext context;
        private CancellationTokenSource tokenSource;
        private IDbQueryCache queryPool;

        public DbContextWrapper(
            TContext context,
            CancellationTokenSource tokenSource,
            IDbQueryCache queryPool)
        {
            this.context = context;
            this.tokenSource = tokenSource;
            this.queryPool = queryPool;
        }

        private CancellationToken CancellationToken => this.tokenSource.Token;

        public async Task<ITransaction> BeginTransactionAsync()
        {
            var trn = await this.context.Database.BeginTransactionAsync(this.CancellationToken);
            return new TransactionWrapper(trn.GetDbTransaction(), this.tokenSource);
        }

        public IDbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return new DbSetWrapper<TEntity>(this.context.Set<TEntity>());
        }

        public async Task<int> ExecuteAsync(IDbQuery query)
        {
            var connection = this.GetDbConnection();
            var transaction = this.context.Database.CurrentTransaction?.GetDbTransaction();
            using(var command = connection.CreateCommand())
            {
                command.CommandText = query.ToString();
                command.Transaction = transaction;
                this.CancellationToken.ThrowIfCancellationRequested();
                return await Task.FromResult(command.ExecuteNonQuery());
            }
        }

        public IDbQuery CreateDbQuery(string sql)
        {
            return new NpgsqlDbQuery(sql);
        }

        public IDbQuery CreateDbQueryById(string sqlId = null)
        {
            return new NpgsqlDbQuery(this.queryPool.GetSqlById(sqlId));
        }

        public IDbConnection GetDbConnection()
        {
            return this.context.Database.GetDbConnection();
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbQuery query)
        {
            var connection = this.GetDbConnection();
            var transaction = this.context.Database.CurrentTransaction?.GetDbTransaction();
            this.CancellationToken.ThrowIfCancellationRequested();
            return await connection.QueryAsync<T>(query.ToString(), query.GetDapperParameters(), transaction);
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(IDbQuery query)
        {
            var connection = this.GetDbConnection();
            var transaction = this.context.Database.CurrentTransaction?.GetDbTransaction();
            this.CancellationToken.ThrowIfCancellationRequested();
            return await connection.QuerySingleOrDefaultAsync<T>(query.ToString(), query.GetDapperParameters(), transaction);
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(IDbQuery query)
        {
            var connection = this.GetDbConnection();
            var transaction = this.context.Database.CurrentTransaction?.GetDbTransaction();
            this.CancellationToken.ThrowIfCancellationRequested();
            return await connection.QueryFirstOrDefaultAsync<T>(query.ToString(), query.GetDapperParameters(), transaction);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.context.SaveChangesAsync(this.CancellationToken);
        }
    }
}
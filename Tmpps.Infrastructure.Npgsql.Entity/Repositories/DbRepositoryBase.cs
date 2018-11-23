using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tmpps.Infrastructure.Data.Entity.Interfaces;
using Tmpps.Infrastructure.Data.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity.Repositories
{
    public abstract class DbRepositoryBase<TKey, TEntity> : IRepository<TKey, TEntity> where TEntity : class
    {
        private Lazy<IDbContext> dbContextLazy;
        private IDbContext DbContext => this.dbContextLazy.Value;
        protected IDbSet<TEntity> DbSet => this.DbContext.Set<TEntity>();
        protected IQueryable<TEntity> Query => this.DbSet.Query;

        public DbRepositoryBase(Lazy<IDbContext> dbContextLazy)
        {
            this.dbContextLazy = dbContextLazy;
        }

        protected virtual async Task AddAsync(TEntity entity)
        {
            this.DbSet.Add(entity);
            await this.DbContext.SaveChangesAsync();
        }

        protected virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            this.DbSet.AddRange(entities);
            await this.DbContext.SaveChangesAsync();
        }

        protected virtual async Task UpdateAsync(TEntity entity)
        {
            this.DbSet.Update(entity);
            await this.DbContext.SaveChangesAsync();
        }

        protected virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            this.DbSet.UpdateRange(entities);
            await this.DbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(TKey id)
        {
            return await this.Query.AnyAsync(this.GetKeyExpression(id));
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await this.Query.SingleOrDefaultAsync(this.GetKeyExpression(id));
        }

        protected abstract Expression<Func<TEntity, bool>> GetKeyExpression(TKey id);

        protected virtual async Task RemoveAsync(TEntity entity)
        {
            this.DbSet.Remove(entity);
            await this.DbContext.SaveChangesAsync();
        }

        protected virtual async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            this.DbSet.RemoveRange(entities);
            await this.DbContext.SaveChangesAsync();
        }
    }
    public abstract class DbRepositoryBase<TEntity> : DbRepositoryBase<string, TEntity> where TEntity : class
    {
        public DbRepositoryBase(Lazy<IDbContext> dbContextLazy) : base(dbContextLazy) { }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tmpps.Infrastructure.Data.Entity;
using Tmpps.Infrastructure.Data.Entity.Interfaces;
using Tmpps.Infrastructure.Data.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity
{
    public class DbSetWrapper<TEntity> : IDbSet<TEntity> where TEntity : class
    {
        private DbSet<TEntity> dbSet;

        public DbSetWrapper(DbSet<TEntity> dbSet)
        {
            this.dbSet = dbSet;
        }

        public IQueryable<TEntity> Query => this.dbSet;

        public void Add(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            this.dbSet.AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            this.dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            this.dbSet.UpdateRange(entities);
        }

        public TEntity Find(int id)
        {
            return this.dbSet.Find(id);
        }

        public async Task<TEntity> FindAsync(int id)
        {
            return await this.dbSet.FindAsync(id);
        }

        public IQueryable<TEntity> FromSql(IDbQuery query)
        {
            return this.dbSet.FromSql(query.ToString(), query.GetParameters());
        }

        public void Remove(TEntity entity)
        {
            this.dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            this.dbSet.RemoveRange(entities);
        }

        public async Task<List<TEntity>> ToListAsync()
        {
            return await this.dbSet.ToListAsync();
        }
    }
}
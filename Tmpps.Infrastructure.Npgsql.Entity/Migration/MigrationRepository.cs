using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tmpps.Infrastructure.Data.Entity.Interfaces;
using Tmpps.Infrastructure.Data.Migration.Interfaces;
using Tmpps.Infrastructure.Npgsql.Entity.Repositories;

namespace Tmpps.Infrastructure.Npgsql.Entity.Migration
{
    public class MigrationRepository : DbRepositoryBase<MigrationHistory>, IMigrationRepository<MigrationHistory>
    {
        public MigrationRepository(Lazy<IMigrationDbContext> dbContextLazy) : base(new Lazy<IDbContext>(() => dbContextLazy.Value)) { }

        protected override Expression<Func<MigrationHistory, bool>> GetKeyExpression(string id)
        {
            return e => e.MigrationHistoryId == id;
        }

        async Task IMigrationRepository<MigrationHistory>.AddAsync(MigrationHistory entity)
        {
            await this.AddAsync(entity);
        }
    }
}
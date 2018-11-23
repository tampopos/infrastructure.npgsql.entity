using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Tmpps.Infrastructure.Data;
using Tmpps.Infrastructure.Data.Entity;
using Tmpps.Infrastructure.Data.Entity.Interfaces;
using Tmpps.Infrastructure.Data.Interfaces;
using Tmpps.Infrastructure.Npgsql.Entity.Extensions;

namespace Tmpps.Infrastructure.Npgsql.Entity
{
    public abstract class NpgsqlDbContext : DbContext
    {
        private IDbQueryCache queryPool;

        public NpgsqlDbContext(DbContextOptions options) : base(options) { }
        public NpgsqlDbContext(DbContextOptions<NpgsqlDbContext> options) : this(options as DbContextOptions) { }
        public NpgsqlDbContext(DbContextOptions options, IDbQueryCache queryPool) : this(options)
        {
            this.queryPool = queryPool;
        }
        public NpgsqlDbContext(DbContextOptions<NpgsqlDbContext> options, IDbQueryCache queryPool) : this(options as DbContextOptions, queryPool) { }

        public override int SaveChanges()
        {
            this.PreSaveChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.PreSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected virtual void PreSaveChanges()
        {
            foreach (var item in this.ChangeTracker.Entries())
            {
                this.PreSaveChanges(item);
            }
        }

        protected virtual void PreSaveChanges(EntityEntry item) { }
    }
}
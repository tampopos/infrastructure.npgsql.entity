using System.Threading;
using Tmpps.Infrastructure.Data.Interfaces;
using Tmpps.Infrastructure.Data.Migration.Interfaces;
using Tmpps.Infrastructure.Npgsql.Entity.Wrapper;

namespace Tmpps.Infrastructure.Npgsql.Entity.Migration
{
    public class MigrationDbContextWrapper : DbContextWrapper<MigrationDbContext>, IMigrationDbContext
    {
        public MigrationDbContextWrapper(MigrationDbContext context, CancellationTokenSource tokenSource, IDbQueryCache queryPool) : base(context, tokenSource, queryPool) { }
    }
}
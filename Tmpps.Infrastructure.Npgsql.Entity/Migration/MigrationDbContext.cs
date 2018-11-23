using Microsoft.EntityFrameworkCore;
using Tmpps.Infrastructure.Data.Interfaces;
using Tmpps.Infrastructure.Data.Migration.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity.Migration
{
    public class MigrationDbContext : NpgsqlDbContext
    {
        public MigrationDbContext(DbContextOptions<MigrationDbContext> options) : base(options) { }
        public DbSet<MigrationHistory> MigrationHistories { get; set; }
    }
}
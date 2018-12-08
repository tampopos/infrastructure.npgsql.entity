using Microsoft.EntityFrameworkCore;
using Tmpps.Infrastructure.Common.DependencyInjection.Builder.Interfaces;
using Tmpps.Infrastructure.Data;
using Tmpps.Infrastructure.Data.Interfaces;
using Tmpps.Infrastructure.Data.Migration.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity.Migration
{
    public class MigrationDIModule : IDIModule
    {
        public void DefineModule(IDIBuilder builder)
        {
            builder.RegisterType<MigrationDbContext>(x => x.InstancePerLifetimeScope());
            builder.RegisterType<MigrationDbContextWrapper>(x => x.As<IMigrationDbContext>());
            builder.RegisterType<MigrationRepository>(x => x.As<IMigrationRepository<MigrationHistory>>());
            builder.RegisterType<MigrationHelper>(x => x.As<IMigrationHelper>());
            builder.RegisterType<MigrationUseCase>(x => x.As<IMigrationUseCase>());
            builder.RegisterType<DbQueryCache>(x => x.As<IDbQueryCache>());
        }
    }
}
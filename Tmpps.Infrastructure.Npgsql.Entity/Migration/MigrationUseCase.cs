using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Tmpps.Infrastructure.Common.DependencyInjection.Interfaces;
using Tmpps.Infrastructure.Common.Foundation.Exceptions;
using Tmpps.Infrastructure.Common.IO.Interfaces;
using Tmpps.Infrastructure.Common.ValueObjects;
using Tmpps.Infrastructure.Data.Migration.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity.Migration
{
    internal class MigrationUseCase : IMigrationUseCase
    {
        private IMigrationConfig config;
        private IPathResolver pathResolver;
        private IScopeProvider scopeProvider;

        public MigrationUseCase(
            IMigrationConfig config,
            IPathResolver pathResolver,
            IScopeProvider scopeProvider)
        {
            this.config = config;
            this.pathResolver = pathResolver;
            this.scopeProvider = scopeProvider;
        }

        public async Task<int> ExecuteAsync()
        {
            var dir = this.pathResolver.ResolveDirectoryPath(this.config.Path);
            if (string.IsNullOrEmpty(dir))
            {
                throw new BizLogicException($"指定のディレクトリーは存在しません。(path:{this.config.Path})");
            }
            var databases = Directory.GetDirectories(dir).Where(x => string.IsNullOrEmpty(this.config.Database) || this.config.Database == Path.GetDirectoryName(x));
            foreach (var database in databases)
            {
                var files = Directory.GetFiles(Path.Combine(dir, database), "*.sql", SearchOption.AllDirectories)
                    .OrderBy(x => x)
                    .GroupBy(x => Path.GetFileNameWithoutExtension(x))
                    .ToArray();
                var duplicate = files.Where(x => x.Count() > 1).SelectMany(x => x).ToArray();
                if (duplicate.Length > 0)
                {
                    throw new BizLogicException($"ファイル名に重複があります。{Environment.NewLine}{string.Join(Environment.NewLine, duplicate)}");
                }

                var optionsBuilder = new DbContextOptionsBuilder<MigrationDbContext>();
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder(this.config.RootConnectionString);
                connectionStringBuilder.Database = database;
                optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
                var options = new TypeValuePair<DbContextOptions<MigrationDbContext>>(optionsBuilder.Options);

                using(var scope = this.scopeProvider.BeginLifetimeScope(options))
                {
                    var migrationHelper = scope.Resolve<IMigrationHelper>();
                    await migrationHelper.InitializeDatabaseAsync(database);
                    await migrationHelper.InitializeAsync();
                }
                foreach (var file in files.Select(x => new { Id = x.Key, Path = x.First() }))
                {
                    using(var scope = this.scopeProvider.BeginLifetimeScope(options))
                    {
                        await scope.Resolve<IMigrationHelper>().MigrationAsync(file.Id, file.Path);
                    }
                }
            }

            return 0;
        }
    }
}
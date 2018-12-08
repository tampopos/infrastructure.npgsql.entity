using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using Tmpps.Infrastructure.Common.Foundation.Interfaces;
using Tmpps.Infrastructure.Common.IO.Interfaces;
using Tmpps.Infrastructure.Data.Configuration.Interfaces;
using Tmpps.Infrastructure.Data.Entity.Interfaces;
using Tmpps.Infrastructure.Data.Interfaces;
using Tmpps.Infrastructure.Data.Migration.Interfaces;
using Tmpps.Infrastructure.Npgsql.Entity.Extensions;

namespace Tmpps.Infrastructure.Npgsql.Entity.Migration
{
    public class MigrationHelper : IMigrationHelper
    {
        private Lazy<IMigrationDbContext> dbContextLazy;
        private IMigrationRepository<MigrationHistory> migrationRepository;
        private ISystemClock systemClock;
        private IPathResolver pathResolver;
        private ILogger logger;
        private IMigrationConfig config;
        private IMigrationDbContext DbContext => this.dbContextLazy.Value;

        public MigrationHelper(
            Lazy<IMigrationDbContext> dbContextLazy,
            IMigrationRepository<MigrationHistory> migrationRepository,
            ISystemClock systemClock,
            IPathResolver pathResolver,
            IMigrationConfig config,
            ILogger logger)
        {
            this.dbContextLazy = dbContextLazy;
            this.migrationRepository = migrationRepository;
            this.systemClock = systemClock;
            this.pathResolver = pathResolver;
            this.config = config;
            this.logger = logger;
        }

        public async Task InitializeDatabaseAsync(string databaseName)
        {
            this.logger.LogInformation($"Start create database");
            using(var connection = new NpgsqlConnection(this.config.RootConnectionString))
            {
                await connection.OpenAsync();
                var exists = await this.ExistsMigrationDatabaseAsync(connection, databaseName);
                if (exists)
                {
                    this.logger.LogInformation("Skip create database");
                    return;
                }
                this.CreateMigrationDatabaseAsync(connection, databaseName);
                connection.Close();
            }
            this.logger.LogInformation("End create database");
        }

        public async Task InitializeAsync()
        {
            this.logger.LogInformation("Start create table");
            var exists = await this.ExistsMigrationTableAsync();
            if (exists)
            {
                this.logger.LogInformation("Skip create table");
                return;
            }
            var transaction = await this.DbContext.BeginTransactionAsync();
            try
            {
                await this.CreateMigrationHistoryTableAsync();
                transaction.Commit();
                this.logger.LogInformation("End create table");
            }
            catch (Exception)
            {
                transaction.Rollback();
                this.logger.LogInformation("Error create table");
                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public async Task MigrationAsync(string id, string path)
        {
            this.logger.LogInformation($"Start execute {path}");
            var exists = await this.migrationRepository.ExistsAsync(id);
            if (exists)
            {
                this.logger.LogInformation($"Skip execute {path}");
                return;
            }
            var sql = File.ReadAllText(this.pathResolver.ResolveFilePath(path));
            var query = this.DbContext.CreateDbQuery(sql);
            var history = new MigrationHistory();
            history.MigrationHistoryId = id;
            history.CreatedAt = this.systemClock.Now;
            var transaction = await this.DbContext.BeginTransactionAsync();
            try
            {
                await this.DbContext.ExecuteAsync(query);
                await this.migrationRepository.AddAsync(history);
                transaction.Commit();
                this.logger.LogInformation($"End execute {path}");
            }
            catch (Exception)
            {
                transaction.Rollback();
                this.logger.LogInformation($"Error execute {path}");
                throw;
            }
            finally
            {
                transaction.Dispose();
            }
        }

        private async Task<bool> ExistsMigrationTableAsync()
        {
            var query = this.DbContext.CreateDbQuery("select count(*) from pg_stat_user_tables where relname  = @table_name").AddParameters(("@table_name", MigrationConstants.TableName));
            return await this.DbContext.QuerySingleOrDefaultAsync<int>(query) > 0;
        }

        private async Task<bool> ExistsMigrationDatabaseAsync(IDbConnection connection, string databaseName)
        {
            return await connection.QuerySingleOrDefaultAsync<int>("select count(*) from pg_database where datname = @database_name", new { database_name = databaseName }) > 0;
        }

        private void CreateMigrationDatabaseAsync(IDbConnection connection, string databaseName)
        {
            using(var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"create database {databaseName}";
                cmd.ExecuteNonQuery();
            }
        }

        private async Task CreateMigrationHistoryTableAsync()
        {
            var sql = $@"
create table {MigrationConstants.TableName}(
  {MigrationConstants.MigrationHistoryIdName} varchar(128) primary key
  ,{MigrationConstants.CreatedAtName} timestamp not null
)";
            var query = this.DbContext.CreateDbQuery(sql);
            await this.DbContext.ExecuteAsync(query);
        }
    }
}
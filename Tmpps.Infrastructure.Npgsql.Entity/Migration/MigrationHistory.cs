using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Tmpps.Infrastructure.Data.Migration.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity.Migration
{
    [Table(MigrationConstants.TableName)]
    public class MigrationHistory : IMigrationHistory
    {
        [Column(MigrationConstants.MigrationHistoryIdName)]
        [Key]
        public string MigrationHistoryId { get; set; }

        [Column(MigrationConstants.CreatedAtName)]
        public DateTime CreatedAt { get; set; }
    }
}
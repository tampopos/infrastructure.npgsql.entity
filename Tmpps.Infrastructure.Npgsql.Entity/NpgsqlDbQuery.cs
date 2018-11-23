using System.Data;
using Npgsql;
using Tmpps.Infrastructure.Data;
using Tmpps.Infrastructure.Data.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity
{
    public class NpgsqlDbQuery : DbQuery
    {
        public NpgsqlDbQuery(string sql = null) : base(sql) { }

        public override IDataParameter CreateParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value);
        }
    }
}
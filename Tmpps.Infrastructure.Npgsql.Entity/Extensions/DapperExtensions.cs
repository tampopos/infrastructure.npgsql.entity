using Dapper;
using Tmpps.Infrastructure.Data.Interfaces;

namespace Tmpps.Infrastructure.Npgsql.Entity.Extensions
{
    public static class DapperExtensions
    {
        public static DynamicParameters GetDapperParameters(this IDbQuery dbQuery)
        {
            var parameters = new DynamicParameters();
            foreach (var parameter in dbQuery.GetParameters())
            {
                parameters.Add(parameter.ParameterName, parameter.Value);
            }
            return parameters;
        }
    }
}
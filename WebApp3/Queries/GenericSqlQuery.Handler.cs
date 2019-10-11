using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;

namespace WebApp3.Queries
{
    public class GenericSqlQueryHandler : IRequestHandler<GenericSqlQuery, GenericSqlQueryResponse>
    {
        public async Task<GenericSqlQueryResponse> Handle(GenericSqlQuery request, CancellationToken cancellationToken)
        {
            string connectionString = "Server=sqlserver.local;Database=BBUsers;Trusted_Connection=False;User Id=bbuser;Password=123;MultipleActiveResultSets=true";
            using (var cnx = new SqlConnection(connectionString))
            {
                var sw = new Stopwatch();
                sw.Start();

                var results = await cnx.QueryAsync<dynamic>(request.SqlCommand);
                sw.Stop();

                var result = new GenericSqlQueryResponse
                {
                    Data = results,
                    ElapsedTime = sw.ElapsedMilliseconds
                };
                return result;
            }
        }
    }

    public static class DapperHelpers
    {
        public static dynamic ToExpandoObject(this object value)
        {
            IDictionary<string, object> dapperRowProperties = value as IDictionary<string, object>;

            IDictionary<string, object> expando = new ExpandoObject();

            foreach (KeyValuePair<string, object> property in dapperRowProperties)
                expando.Add(property.Key, property.Value);

            return expando as ExpandoObject;
        }
    }
}

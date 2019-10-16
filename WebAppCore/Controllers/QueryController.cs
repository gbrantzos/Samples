using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApp3.Queries;

namespace WebAppCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly IMediator mediator;

        public QueryController(IMediator mediator) => this.mediator = mediator;

        [HttpPost]
        public async Task<IEnumerable<dynamic>> ExecuteQuery([FromBody] GenericSqlQuery sqlQuery)
        {
            string connectionString = "Server=sqlserver.local;Database=BBUsers;Trusted_Connection=False;User Id=bbuser;Password=123;MultipleActiveResultSets=true";
            using (var cnx = new SqlConnection(connectionString))
            {
                var results = await cnx.QueryAsync<dynamic>(sqlQuery.SqlCommand);
                return results;
            }

            //GenericSqlQueryResponse result = await this.mediator.Send(sqlQuery);
            //return result.Data;
        }
    }
}
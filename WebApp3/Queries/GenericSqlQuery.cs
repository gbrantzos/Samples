using System.Collections.Generic;
using MediatR;

namespace WebApp3.Queries
{
    public class GenericSqlQuery : IRequest<GenericSqlQueryResponse>
    {
        public string SqlCommand { get; set; }
    }

    public class GenericSqlQueryResponse
    {
        public IEnumerable<dynamic> Data { get; set; }
        public long ElapsedTime { get; set; }
    }
}

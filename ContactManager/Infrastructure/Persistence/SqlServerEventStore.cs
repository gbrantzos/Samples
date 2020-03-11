using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Domain.Core;
using Dapper;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace ContactManager.Infrastructure.Persistence
{
    public class SqlServerEventStore : IEventStore
    {
        private class EventDTO
        {
            public Guid EventID       { get; set; }
            public long AggregateID   { get; set; }
            public int Version        { get; set; }
            public DateTime CreatedAt { get; set; }
            public string EventType   { get; set; }
            public string Payload     { get; set; }
        }

        private readonly string connectionString;

        public SqlServerEventStore(string connectionString)
            => this.connectionString = connectionString;

        public async Task<IEnumerable<Event>> GetEventStream(long aggregateID)
        {
            const string sql = "select * from Events where AggregateID = @aggregateID order by Version";
            using var db = new SqlConnection(connectionString);
            var events = await db.QueryAsync<EventDTO>(sql, new { aggregateID });

            return events
                .Select(e =>
                {
                    var type = Type.GetType(e.EventType);
                    return JsonConvert.DeserializeObject(e.Payload, type) as Event;
                })
                .ToList();
        }

        public async Task SaveEventStream(IEnumerable<Event> eventStream)
        {
            // Make checks...
            var events = eventStream
                .Select(e => new EventDTO
                {
                    EventID     = e.EventID,
                    AggregateID = e.AggregateID,
                    Version     = e.Version,
                    CreatedAt   = e.CreatedAt,
                    EventType   = e.GetType().FullName,
                    Payload     = JsonConvert.SerializeObject(e)
                });
            const string sql = "insert into Events (EventID, AggregateID, Version, CreatedAt, EventType, Payload) " +
                               "values (@EventID, @AggregateID, @Version, @CreatedAt, @EventType, @Payload)";

            using var db = new SqlConnection(connectionString);
            await db.ExecuteAsync(sql, events);
        }
    }
}

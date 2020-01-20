using System.Collections.Generic;
using Dapper;
using Microsoft.Data.SqlClient;

namespace PharmexTargets.Persistence
{
    public class TargetUpdater
    {
        private readonly string connectionString;

        public TargetUpdater(string connectionString)
            => this.connectionString = connectionString;

        public void UpdateTargets(IEnumerable<TargetRow> rows)
        {
            using (var cnx = new SqlConnection(connectionString))
            {
                cnx.Open();
                cnx.Execute(Queries.DeleteTargets);
                cnx.Execute(Queries.InsertTargets, rows);
                // Merge command
            }
        }
    }
}

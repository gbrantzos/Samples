using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace Workbench
{
    public class TasksWhenAny
    {
        // private const string ConnectionString = "Server=WORKBOX;Database=glxSJA;Integrated Security=true";
        private const string ConnectionString = "user id=S01001;password=S01001;data source=//senserver.gbworks.lan:1521/SEN";

        public async Task<int> ExecuteQuery(string delayString, CancellationToken cancellationToken = default)
        {
            if (delayString == "0:00:30")
            {
                Console.WriteLine(" >> Failed!");
                await Task.Delay(Timeout.Infinite, cancellationToken);

                return -1;
            }

            // using var cnx = new SqlConnection(ConnectionString);
            Console.WriteLine($"Starting query {delayString} ....");
            using var cnx = new OracleConnection(ConnectionString);
            cnx.Open();

            var cmd = cnx.CreateCommand();
            // cmd.CommandText = $"WAITFOR DELAY '{delayString}';";
            cmd.CommandText = "begin dbms_lock.sleep(30); end;";
            cmd.CommandTimeout = 300;

            await cmd.ExecuteScalarAsync(cancellationToken);
            return -1;
        }
    }
}

using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

var connectionString = "server = sqlserver.gbworks.lan; database = glxPraxitelis; user = sa; password = Devel12#$; TrustServerCertificate = true";
var cnx = new SqlConnection(connectionString);
await cnx.OpenAsync();
try
{
    Console.WriteLine("Start execution");
    await cnx.ExecuteAsync("WAITFOR DELAY '00:01:30'", commandTimeout: 10);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    Console.WriteLine("Check");
    Console.ReadKey();
    cnx.Close();
}

Console.WriteLine("WAITFOR DELAY '00:00:30'");
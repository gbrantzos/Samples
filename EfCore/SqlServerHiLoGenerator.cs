using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EfCore
{
    public interface IHiLoGenerator
    {
        // Steps:
        //
        // 1. Select nextHi, if null then 0
        // 2. Increase nextHi by 1
        // 3. Update HiLo table with new nextHi value
        // 4. If update fails then insert new value
        //
        // All these should be enclosed in a transaction

        /// <summary>
        /// Get next ID for requested table
        /// <list type="table">
        /// <item>
        /// <description>Item 1.</description>
        /// </item>
        /// <item>
        /// <description>Item 2.</description>
        /// </item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        int NextID(string table);
    }

    public class SqlServerHiLoGenerator : IHiLoGenerator
    {
        private static object _lock = new object();

        private class HiLo
        {
            public int Hi { get; set; } = -1;
            public int Lo { get; set; }
        }

        private readonly int range;
        private readonly string connectionString;
        private readonly Dictionary<string, HiLo> hiloCache = new Dictionary<string, HiLo>();

        public SqlServerHiLoGenerator(string connectionString, int range)
        {
            this.connectionString = connectionString;
            this.range = range;
        }

        public int NextID(string table)
        {
            lock (_lock)
            {
                if (!hiloCache.TryGetValue(table, out var hiLo))
                {
                    hiLo = new HiLo();
                    hiloCache.Add(table, hiLo);
                }

                if (hiLo.Hi == -1 || hiLo.Lo >= range)
                {
                    hiLo.Hi = GetNextHi(table);
                    hiLo.Lo = 0;
                }

                hiLo.Lo++;
                return (hiLo.Hi * range) + hiLo.Lo;
            }
        }

        private int GetNextHi(string table)
        {
            /*
            -- Create table
            CREATE TABLE HiLoDetails (
              TableName nvarchar(100) NOT NULL,
              NextHi    int NOT null,

              CONSTRAINT PK_HiLoDetails PRIMARY KEY (TableName)
            )
            */

            using (var cnx = new SqlConnection(connectionString))
            {
                cnx.Open();
                using (var trx = cnx.BeginTransaction())
                {
                    var select = "SELECT COALESCE(MAX(NextHi) ,0) from HiLoDetails WHERE TableName = @tableName";
                    var selectCmd = PrepareCommand(
                        cnx,
                        trx,
                        select,
                        new Dictionary<string, object> { { "@tableName", table } });
                    int nextHi = (int)selectCmd.ExecuteScalar();

                    var merge = "MERGE INTO dbo.HiLoDetails t " +
                                "  USING (SELECT @tableName TableName) s " +
                                "  ON (s.TableName = t.TableName) " +
                                "WHEN MATCHED THEN " +
                                "  UPDATE SET t.NextHi = t.NextHi + 1 " +
                                "WHEN NOT MATCHED THEN " +
                                "  INSERT VALUES (@tableName, 1);";
                    var mergeCmd = PrepareCommand(
                        cnx,
                        trx,
                        merge,
                        new Dictionary<string, object> { { "@tableName", table } });
                    var affectedRows = mergeCmd.ExecuteNonQuery();
                    trx.Commit();

                    return nextHi;
                }
            }
        }

        private IDbCommand PrepareCommand(
            IDbConnection cnx,
            IDbTransaction trx,
            string sqlCommand,
            Dictionary<string, object> parameters = null)
        {
            var cmd = cnx.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sqlCommand;
            cmd.Transaction = trx;

            if (parameters != null)
                foreach (var item in parameters)
                {
                    var prm = cmd.CreateParameter();
                    prm.ParameterName = item.Key;
                    prm.Value = item.Value;

                    cmd.Parameters.Add(prm);
                }

            return cmd;
        }
    }
}

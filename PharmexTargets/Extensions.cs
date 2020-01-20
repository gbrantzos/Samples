using System;
using System.Data;

namespace PharmexTargets
{
    public static class Extensions
    {
        public static T SafeField<T>(this DataRow row, string columnName)
        {
            if (row[columnName] == null || row[columnName] == DBNull.Value)
                return default(T);
            return row.Field<T>(columnName);
        }
    }
}

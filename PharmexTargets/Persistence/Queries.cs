using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace PharmexTargets.Persistence
{
    public static class Queries
    {
        private static readonly Assembly assembly = typeof(Queries).Assembly;
        private static readonly string prefix = $"{typeof(Queries).Namespace}.Queries.";

        private static string GetResourceFile(string fileName)
        {
            string extention = fileName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase) ?
                String.Empty : ".sql";
            string fullName = $"{prefix}{fileName}{extention}";

            var resourceStream = assembly.GetManifestResourceStream(fullName);
            using var reader = new StreamReader(resourceStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }

        public static string InsertTargets => GetResourceFile("InsertTargets");
        public static string DeleteTargets => GetResourceFile("DeleteTargets");
    }
}

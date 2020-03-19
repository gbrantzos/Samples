using System;
using System.Collections;
using System.Collections.Generic;

namespace Workbench
{
    public static class ConfigurationExtensions
    {
        public static Dictionary<string,string> ToConfigData(this object source, string root = null)
        {
            var result = new Dictionary<string, string>();
            ConvertInternal(result, source, root ?? String.Empty);

            return result;
        }

        private static void ConvertInternal(Dictionary<string, string> result, object source, string rootKey)
        {
            var properties = source.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var keyName = String.IsNullOrEmpty(rootKey) ? prop.Name : rootKey + ':' + prop.Name;
                var value = prop.GetValue(source, null);

                if (value == null) return;

                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                {
                    result[keyName] = value.ToString();
                }
                else if (prop.PropertyType == typeof(DateTime))
                {
                    result[keyName] = ((DateTime)value).ToString("s");
                }
                else if (value is IEnumerable enumerableValue)
                {
                    var index = 0;
                    foreach(object child in enumerableValue)
                    {
                        ConvertInternal(result, child, $"{keyName}:{index}");
                        index++;
                    }
                }
                else
                {
                    ConvertInternal(result, value, keyName);
                }
            }
        }
    }
}

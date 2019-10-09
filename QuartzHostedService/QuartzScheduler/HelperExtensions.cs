using System;
using System.Collections.Generic;

namespace QuartzHostedService.QuartzScheduler
{
    public static class Extensions
    {
        // https://stackoverflow.com/a/9314733

        /// <summary>
        /// Get hierarchical structure (parent - child)
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="nextItem"></param>
        /// <param name="canContinue"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
                yield return current;
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
            => FromHierarchy(source, nextItem, s => s != null);

        /// <summary>
        /// Handling the null check and throw a ArgumentNullException if null
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <param name="parameter">the actual parameter</param>
        /// <param name="parameterName">the name of the parameter</param>
        /// <param name="message">an optional message instead of the default</param>
        public static T ThrowIfNull<T>(this T parameter, string parameterName, string message = null) where T : class
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName, message ?? $"Parameter '{parameterName}' is null!");
            }
            return parameter;
        }

        /// <summary>
        /// Handling string empty check and throw a ArgumentNullException if null
        /// </summary>
        /// <param name="parameter">the actual string parameter</param>
        /// <param name="parameterName">the name of the parameter</param>
        /// <param name="message">an optional message instead of the default</param>
        public static string ThrowIfEmpty(this string parameter, string parameterName, string message = null)
        {
            if (String.IsNullOrEmpty(parameter))
            {
                throw new ArgumentNullException(parameterName, message ?? $"Parameter '{parameterName}' is null or empty!");
            }
            return parameter;
        }

        /// <summary>
        /// Get string truncated to max length.
        /// </summary>
        /// <param name="value">String to truncate</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns></returns>
        public static string WithMaxLength(this string value, int maxLength)
            => value?.Substring(0, Math.Min(value.Length, maxLength));
    }
}

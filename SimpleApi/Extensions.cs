using System.Runtime.CompilerServices;

namespace SimpleApi;

public static class Extensions
{
    public static T ThrowIfNull<T>(this T parameter, 
        [CallerArgumentExpression("parameter")] string parameterName = null!, 
        string? message = null)
    {
        if (parameter == null)
        {
            throw new ArgumentNullException(parameterName, message ?? $"Parameter '{parameterName}' is null!");
        }
        return parameter;
    }
}

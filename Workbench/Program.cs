using System;
using System.Collections.Generic;

IErrorMapperRegistry mapperRegistry = new ErrorMapperRegistry();
mapperRegistry.Register(new NotFoundErrorMapper());
mapperRegistry.Register(new InvalidRequestErrorMapper());

var notFoundMapper = mapperRegistry.GetMapper<NotFound>();
var notFound = new NotFound();
var notFoundDetails = notFoundMapper(notFound);
Console.WriteLine(notFoundDetails);

var invalidRequestMapper = mapperRegistry.GetMapper<InvalidRequest>();
var invalidRequest = new InvalidRequest();
var invalidRequestDetails = invalidRequestMapper(invalidRequest);
Console.WriteLine(invalidRequestDetails);

Console.ReadKey();


public class Error { }

public class ProblemDetails
{
    public string Type { get; init; } = String.Empty;
    public long Code { get; init; } = -1;

    public override string ToString() => $"{Type}, #{Code}";
}

public interface IErrorMapper<in TError> where TError : Error
{
    ProblemDetails Map(TError error);
}

public class NotFound : Error { }
public class InvalidRequest : Error { }

public class NotFoundErrorMapper : IErrorMapper<NotFound>
{
    public ProblemDetails Map(NotFound err)
    {
        return new ProblemDetails
        {
            Type = err.GetType().Name,
            Code = err.GetHashCode()
        };
    }
}

public class InvalidRequestErrorMapper : IErrorMapper<InvalidRequest>
{
    public ProblemDetails Map(InvalidRequest err)
    {
        return new ProblemDetails
        {
            Type = err.GetType().Name,
            Code = err.GetHashCode()
        };
    }
}

public interface IErrorMapperRegistry
{
    void Register<TError>(IErrorMapper<TError> provider) where TError : Error;
    Func<TError, ProblemDetails> GetMapper<TError>() where TError : Error;
}

public class ErrorMapperRegistry : IErrorMapperRegistry
{
    private class DelegateHelper<T> where T : Error
    {
        public DelegateHelper(IErrorMapper<T> errorMapper)
        {
            Func<T, ProblemDetails> mapper = (r) => errorMapper.Map(r);
            MapperDelegate = error => mapper(error);
        }

        public Func<T, ProblemDetails> MapperDelegate { get; }
    }

    private readonly Dictionary<Type, Func<Error, ProblemDetails>> _cache = new(); 

    public void Register<TError>(IErrorMapper<TError> provider) where TError : Error
    {
        var delegateHelper = new DelegateHelper<TError>(provider);
        _cache.Add(typeof(TError), err => delegateHelper.MapperDelegate.Invoke((TError) err));
    }

    public Func<TError, ProblemDetails> GetMapper<TError>() where TError : Error
    {
        var mapper = _cache[typeof(TError)];
        return error => mapper(error);
    }
}


// using System;
// using System.Diagnostics;
// using System.Threading;
// using System.Threading.Tasks;
//
//
// var task = Task.Run(() => ConsumeCPU(50));
//
// while (true)
// {
//     await Task.Delay(2000);
//     var cpuUsage = await GetCpuUsageForProcess();
//
//     Console.WriteLine(cpuUsage);
// }
//
//
// static void ConsumeCPU(int percentage)
// {
//     Stopwatch watch = new Stopwatch();
//     watch.Start();
//     while (true)
//     {
//         if (watch.ElapsedMilliseconds > percentage)
//         {
//             Thread.Sleep(100 - percentage);
//             watch.Reset();
//             watch.Start();
//         }
//     }
// }
//
// static async Task<double> GetCpuUsageForProcess()
// {
//     var startTime = DateTime.UtcNow;
//     var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
//
//     await Task.Delay(1000);
//
//     var endTime = DateTime.UtcNow;
//     var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
//
//     var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
//     var totalMsPassed = (endTime - startTime).TotalMilliseconds;
//
//     var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
//
//     return cpuUsageTotal * 100;
// }
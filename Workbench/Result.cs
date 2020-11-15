using System;

namespace Workbench
{
    public class Result
    {
        public bool HasErrors { get; }
        public string ErrorMessage { get; }

        protected Result(string errorMessage, bool hasErrors)
        {
            HasErrors    = hasErrors;
            ErrorMessage = errorMessage;
        }

        public static Result Fail(string errorMessage) => new Result(errorMessage, true);
        public static Result Ok() => new Result(String.Empty, false);
        public static Result Ok<TData>(TData data) => new Result<TData>(data);
    }

    public class Result<TData> : Result
    {
        public TData Data { get; set; }

        public Result(TData data) : base(String.Empty, false) { Data = data; }

        public static implicit operator Result<TData>(TData data) { return new Result<TData>(data); }
    }

    public static class ResultExtensions
    {
        public static TReturn When<TReturn>(this Result result, Func<TReturn> onSuccess, Func<TReturn> onFail)
        {
            if (result.HasErrors)
                return onFail();

            return onSuccess();
        }

        public static TReturn When<TReturn, TResult>(this Result<TResult> result, Func<TResult, TReturn> onSuccess, Func<string, TReturn> onFail)
        {
            if (result.HasErrors)
                return onFail(result.ErrorMessage);

            return onSuccess(result.Data);
        }
    }
}

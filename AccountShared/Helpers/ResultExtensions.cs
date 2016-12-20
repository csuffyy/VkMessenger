using System;
using VkData.Interface;

namespace VkData.Helpers
{
    public static class ResultExtensions
    {
        public static Result<T> ToGeneric<T>(this Result result, T value, bool completed)
        {
            return new Result<T>(result, value, completed);
        }

        public static Result Try<TException>(this Result result, Action execute,
            Action<TException> onException = null, ILogger logger = null) where TException : Exception
        {
            return new TryAction<TException>(execute, onException, logger);
        }

        public static Result Try(this Result result, Action execute,
            Action<Exception> onException = null, ILogger logger = null)
        {
            return new TryAction<Exception>(execute, onException, logger);
        }

        public static Result Try(this Result result, Action execute, ILogger logger = null)
        {
            return new TryAction<Exception>(execute, null, logger);
        }
    }
}
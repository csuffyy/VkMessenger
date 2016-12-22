using System;
using VkData.Interface;

namespace VkData.Helpers
{
    public static class TryExtensions
    {
        public static Result Try(this Action action, ILogger logger)
        {
            return Try(action, null, logger);
        }

        public static Result Try(this Action action, Action<Exception> onException = null, ILogger logger = null)
        {
            return new TryAction(action, onException, logger);
        }

        public static Result Try<T>(this Action action, Action<T> onException = null, ILogger logger = null)
            where T : Exception
        {
            return new TryAction<T>(action, onException, logger);
        }

        public static Result<T> Try<T>(this Func<T> execute, ILogger logger = null)
        {
            return new Try<T>(execute, logger);
        }

        public static Result<T> Try<T>(this Func<T> execute, Action<Exception> onException, ILogger logger = null)
        {
            return new Try<T>(execute, onException, logger);
        }

        public static Result<T> Try<T, TException>(this Func<T> execute, Action<TException> onException,
            ILogger logger = null) where TException : Exception
        {
            return new Try<T, TException>(execute, onException, logger);
        }

        public static Result<T> Try<T, TException>(this Func<T> execute,
            ILogger logger = null) where TException : Exception
        {
            return new Try<T, TException>(execute, e => { }, logger);
        }

        public static Result<T> TryOr<T, TException>(this Func<T> execute, Func<TException, T> onException,
            ILogger logger = null) where TException : Exception
        {
            return new Try<T, TException>(execute, onException, logger);
        }

        public static Result<T> TryOr<T>(this Func<T> execute, Func<Exception, T> onException,
            ILogger logger = null)
        {
            return new Try<T, Exception>(execute, onException, logger);
        }

        public static Result<T> Try<T, TParam>(this Func<TParam, T> execute, TParam parameter, ILogger logger = null)
        {
            return new Try<T>(() => execute(parameter), logger);
        }

        public static Result<T> Try<T, TParam>(this Func<TParam, T> execute, TParam parameter, Action<Exception> onException, ILogger logger = null)
        {
            return new Try<T>(() => execute(parameter), onException, logger);
        }

        public static Result<T> Try<TParam, T, TException>(this Func<TParam, T> execute, TParam parameter, Action<TException> onException,
            ILogger logger = null) where TException : Exception
        {
            return new Try<T, TException>(() => execute(parameter), onException, logger);
        }

        public static Result<T> Try<TParam, T, TException>(this Func<TParam, T> execute, TParam parameter, 
            ILogger logger = null) where TException : Exception
        {
            return new Try<T, TException>(() => execute(parameter), e => { }, logger);
        }

        public static Result<T> TryOr<T, TException, TParam>(this Func<TParam, T>execute, TParam parameter, Func<TException, T> onException,
            ILogger logger = null) where TException : Exception
        {
            return new Try<T, TException>(() => execute(parameter), onException, logger);
        }

        public static Result<T> TryOr<T, TParam>(this Func<TParam, T>execute, TParam parameter, Func<Exception, T> onException,
            ILogger logger = null)
        {
            return new Try<T, Exception>(() => execute(parameter), onException, logger);
        }
    }
}
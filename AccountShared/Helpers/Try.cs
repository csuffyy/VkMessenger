using System;
using VkData.Interface;

namespace VkData.Helpers
{
    /// <summary>
    ///     uses generic Result to retrieve a value from operation
    /// </summary>
    /// <typeparam name="TSource">Type of a value to return</typeparam>
    public class Try<TSource> : Try<TSource, Exception>
    {
        public Try(Func<TSource> execute, ILogger logger = null) : base(execute, e => { }, logger)
        {
        }

        public Try(Func<TSource> execute, Action<Exception> onException, ILogger logger = null)
            : base(execute, onException, logger)
        {
        }

        public static implicit operator Result<TSource>(Try<TSource> target)
            => target.Result;
    }

    /// <summary>
    ///     uses generic Result to retrieve a value from operation which might throw a custom Exception
    /// </summary>
    /// <typeparam name="TSource">Type of a value to return</typeparam>
    /// <typeparam name="TException">Custom exception type</typeparam>
    public class Try<TSource, TException> : IQuery<TSource> where TException : Exception
    {
        public Try(Func<TSource> execute, Action<TException> onException, ILogger logger = null)
        {
            try
            {
                var source = execute();
                Result = new Result<TSource>(source);
            }
            catch (TException e)
            {
                onException?.Invoke(e);

                #region Try<TSource> support

                if (typeof (TException) == typeof (Exception))
                {
                    logger?.Log(e);
                    Result = new Result<TSource>(GetEmpty(), false);
                }

                #endregion
            }
            catch (Exception e)
            {
                logger?.Log(e);
                Result = new Result<TSource>(GetEmpty(), false);
            }
        }

        public Try(Func<TSource> execute, Func<TException, TSource> onException, ILogger logger = null)
        {
            try
            {
                Result = new Result<TSource>(execute());
            }
            catch (TException e)
            {
                var source = onException == null
                    ? GetEmpty()
                    : onException.Invoke(e);
                Result = new Result<TSource>(source);

                #region Try<TSource> support

                if (typeof (TException) == typeof (Exception))
                {
                    logger?.Log(e);
                }

                #endregion
            }
            catch (Exception e)
            {
                logger?.Log(e);
                if (e is TException) return;
                Result = new Result<TSource>(GetEmpty(), false);
            }
        }

        public Result<TSource> Result { get; }

        private static TSource GetEmpty()
        {
            var type = typeof (TSource);
            if (type.IsClass)
                return (TSource) Activator.CreateInstance(typeof (TSource));

            return default(TSource);
        }

        public static implicit operator Result<TSource>(Try<TSource, TException> target)
            => target.Result;
    }
}
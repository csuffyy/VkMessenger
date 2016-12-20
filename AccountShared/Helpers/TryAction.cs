using System;
using VkData.Interface;

namespace VkData.Helpers
{
    /// <summary>
    ///     uses non-generic Result
    /// </summary>
    /// <typeparam name="TException">Custom exception type</typeparam>
    public class TryAction<TException> where TException : Exception
    {
        public TryAction(Action execute, Action<TException> onException, ILogger logger = null)
        {
            try
            {
                execute();
                Result = new Result(true);
            }
            catch (TException e)
            {
                Result = new Result(false);
                logger?.Log(e);
                onException?.Invoke(e);
            }
        }

        public Result Result { get; }

        public static implicit operator Result(TryAction<TException> tryAction)
            => tryAction.Result;
    }

    /// <summary>
    ///     uses non-generic result
    /// </summary>
    public class TryAction : TryAction<Exception>
    {
        public TryAction(Action execute, Action<Exception> onException, ILogger logger = null)
            : base(execute, onException, logger)
        {
        }

        public static implicit operator Result(TryAction tryAction)
            => tryAction.Result;
    }
}
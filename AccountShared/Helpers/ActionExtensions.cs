using System;
using System.Threading.Tasks;

namespace VkData.Helpers
{
    public static class ActionExtensions
    {
        public static void CallSafe(this Action action) => action?.Invoke();

        /// <summary>
        ///     Creates new action made by executing this action after specified action with delay
        /// </summary>
        /// <param name="action">current action</param>
        /// <param name="exec">specified action</param>
        /// <param name="interval">time interval to wait between execution</param>
        public static Action After(this Action action, Action exec, int interval = 0)
        {
            if (interval == default(int))
                return () =>
                {
                    exec();
                    action();
                };

            return async () =>
            {
                exec();
                await Task.Delay(interval);
                action();
            };
        }

        public static Action Delayed(this Action action, int interval)
        {
            if (interval == default(int))
                return action;

            return async () =>
            {
                await Task.Delay(interval);
                action();
            };
        }

        public static Func<T> Delayed<T>(this Func<T> func, int interval)
        {
            if (interval == default(int))
                return func;

            return () =>
            {
                Task.Delay(interval);
                return func();
            };
        }
    }
}
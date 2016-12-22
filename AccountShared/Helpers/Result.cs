using System;

namespace VkData.Helpers
{
    public class Result
    {
        public Result(bool success)
        {
            Success = success;
        }

        public bool Success { get; protected set; }

        private Result OnCondition(Action onCondition, bool condition, int millisecondsDelay = 0)
        {
            if (!condition) return this;
            onCondition?.Delayed(millisecondsDelay).Invoke();
            return this;
        }

        public virtual Result OnSuccess(Action onSuccess, int millisecondsDelay = 0)
            => OnCondition(onSuccess, Success, millisecondsDelay);

        public virtual Result OnFailure(Action onFailure, int millisecondsDelay = 0)
            => OnCondition(onFailure, !Success, millisecondsDelay);

        public virtual Result OnBoth(Action onBoth, int millisecondsDelay = 0)
        {
            onBoth?.Delayed(millisecondsDelay).Invoke();
            return this;
        }

        public virtual Result OnBoth(Action onSuccess, Action OnFailure, int millisecondsDelay = 0)
        {
            if (Success)
                onSuccess?.Delayed(millisecondsDelay).Invoke();
            else OnFailure?.Delayed(millisecondsDelay).Invoke();
            return this;
        }
    }

    public class Result<T> : Result
    {
        public Result(Result result, T value, bool completed = true) : base(result.Success)
        {
            Value = value;
            OperationCompleted = completed;
        }

        public Result(T value, bool success = true, bool completed = true) : base(success)
        {
            Value = value;
            OperationCompleted = completed;
        }

        public T Value { get; private set; }
        public bool OperationCompleted { get; }

        private Result<T> OnConditionFunc(Func<T> onConditionFunc, bool condition, int millisecondsDelay = 0)
        {
            if (!condition) return this;
            Value = onConditionFunc.Delayed(millisecondsDelay).Invoke();
            return this;
        }

        public Result<T> OnSuccess(Func<T> onSuccessFunc) => OnConditionFunc(onSuccessFunc, Success);

        public Result<T> OnFailure(Func<T> onFailureFunc) => OnConditionFunc(onFailureFunc, !Success);

        public Result<T> OnBoth(Func<T> onSuccess, Func<T> OnFailure)
        {
            Value = Success ? onSuccess() : OnFailure();
            return this;
        }

        public Result<T> OnBoth(Action onSuccess, Func<T> OnFailure)
        {
            var result = base.OnBoth(onSuccess, () => Value = OnFailure());
            return new Result<T>(result, Value, OperationCompleted);
        }

        public Result<T> OnBoth(Func<T> onSuccess, Action OnFailure)
        {
            var result = base.OnBoth(() => Value = onSuccess(), OnFailure);
            return new Result<T>(result, Value, OperationCompleted);
        }

        public static implicit operator
            T(Result<T> result)
        {
            return result == null ? default(T) : result.Value;
        }
    }
}
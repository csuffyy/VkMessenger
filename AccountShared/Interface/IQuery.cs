using VkData.Helpers;

namespace VkData.Interface
{
    public interface IQuery
    {
        Result Result { get; }
    }

    public interface IQuery<TResult>
    {
        Result<TResult> Result { get; }
    }
}
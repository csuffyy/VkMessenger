using System;
using System.Net;
using VkData.Account.Extension;
using VkData.Helpers;
using VkData.Interface;

namespace VkData.Account.Types
{
    public class HttpQuery<T> : IQuery<T>
    {
        public HttpQuery(string url, Func<string, T> getResult, int timeout = 10000, ILogger logger = null)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.ReadWriteTimeout = timeout;
            var response = (string) ((Func<string>) (() => request.GetResponseString())).Try(logger);
            Result = ((Func<T>) (() => getResult(response))).Try(logger);
        }

        public Result<T> Result { get; }
    }
}
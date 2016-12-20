using System;
using Newtonsoft.Json;
using VkNet.Model;

namespace VkData.Account.Types
{
    public class LongPollServerSettings
    {
        public LongPollServerSettings(ulong localPts, ulong localTs, long? maxMsgId)
        {
            MaxMsgId = maxMsgId;
            LocalPts = localPts;
            LocalTs = localTs;
            LocalTsLastModified = DateTime.Now;
        }

        public LongPollServerSettings()
        {
            LocalTsLastModified = DateTime.Now;
        }

        public long? MaxMsgId { get;  set; }
        public ulong? LocalPts { get;  set; }
        public ulong LocalTs { get;  set; }
        public DateTime LocalTsLastModified { get; private set; }

        [JsonIgnore]
        public bool TsOutdated => DateTime.Now - LocalTsLastModified > TimeSpan.FromDays(1);

        [JsonIgnore]
        public bool Empty =>
            (LocalPts == 0 || LocalPts == null) && LocalTs == 0;

        /// <summary>
        /// Creates new LongPollServerSettings from 
        /// Vk.Net LongPollServerResponse
        /// </summary>
        /// <param name="response"></param>
        public void FromResponse(LongPollServerResponse response)
        {
            LocalPts = response.Pts;
            LocalTs = response.Ts;
            LocalTsLastModified = DateTime.Now;
        }
    }
}
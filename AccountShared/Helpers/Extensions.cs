using System;

namespace VkData.Helpers
{
    public static class Extensions
    {
        public static T ToNotNullable<T>(this T? value)
            where T : struct => value ?? default(T);


        public static DateTime ToDateTime(this long unixTime)
            => new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unixTime + 10800);
    }
}
using System;

namespace VkData.Helpers
{
    public static class ObjectExtensions
    {
        /// <summary>
        ///     throw an ArgumentNullException if object is null
        /// </summary>
        /// <param name="obj"></param>
        public static void ThrowIfNull(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), $"{nameof(obj)} must not be null");
        }
    }
}
using System;
using System.Text;

namespace VkData.Helpers
{
    public static class ExceptionExtensions
    {
        public static string GetInnerExceptions(this Exception e)
        {
            if (e == null)
                return string.Empty;

            var inner = e.InnerException;
            var build = new StringBuilder(inner?.Message);
            do
            {
                build.Append($"\n{inner?.InnerException?.Message}");
                inner = inner?.InnerException;
            } while (inner?.InnerException != null);

            return build.ToString();
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace VkData.Helpers
{
    public static class StringExtensions
    {
        public static string RenameDirectoryPath(this string Path, string newName)
        {
            var parts = Path.Split('\\');
            return JoinPath(parts.Where((s, i) => i < parts.Length - 1).JoinStrings("\\"), newName);
        }

        public static string JoinPath(this string path, string subPath) => $"{path}\\{subPath}";

        public static IEnumerable<string> JoinPath(this string path, string[] items)
            => items.Select(s => JoinPath(path, s));

        public static string GetText(this string body, string name)
            => $"{name}:\n {body}\n";
    }
}
using System.IO;

namespace VkData.Helpers
{
    public static class FileUtils
    {
        public static string JoinAndCreate(string directory, string fileName)
        {
            CreateIfNotExist(directory);
            return directory.JoinPath(fileName);
        }

        public static void CreateIfNotExist(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        public static bool IsDirectoryEmpty(string path)
            => Directory.Exists(path) && (Directory.GetFiles(path).Length > 0);

        public static void MergeDirectory(string oldPath, string newPath)
        {
            if (Directory.Exists(newPath))
                Directory.Delete(oldPath, true);
            else
                Directory.Move(oldPath, newPath);
        }
    }
}
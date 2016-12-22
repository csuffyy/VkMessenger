using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using VkData.Interface;

namespace VkData.Helpers
{
    public class Downloader : IFileService
    {
        public Downloader(string path)
        {
            Path = path;
            Client = new WebClient
            {
                Proxy = null
            };
        }

        private WebClient Client { get; }

        public bool Busy { get { return Client.IsBusy; } }

        public string Path { get; set; }

        public Task<string> DownloadAsync(Uri uri, string subPath, PathOptions options = PathOptions.Partial,
            bool rewrite = false)
            => DownloadAsync(uri.AbsoluteUri, subPath, options, rewrite);

        public async Task<string> DownloadAsync(string absoluteUri, string subPath, PathOptions options = PathOptions.Partial,
            bool rewrite = false)
        {
            var uri = new Uri(absoluteUri);
            var path = GetAbsolutePath(options, subPath);

            if (!rewrite)
            {
                if (File.Exists(path))
                    return path;
            }
            else
            {
                File.Delete(path);
            }

            FileUtils.CreateIfNotExist(System.IO.Path.GetDirectoryName(path));

            while (Client.IsBusy)
            {
                await Task.Delay(100);
            }

            await Client.DownloadFileTaskAsync(uri, path);

            return path;
        }

        public string DownloadSyncronously(Uri uri, string subPath, PathOptions options = PathOptions.Partial,
            bool rewrite = false)
            => DownloadSyncronously(uri.AbsoluteUri, subPath, options, rewrite);

        public string DownloadSyncronously(string absoluteUri, string subPath, PathOptions options = PathOptions.Partial,
            bool rewrite = false)
        {
            var uri = new Uri(absoluteUri);
            var path = GetAbsolutePath(options, subPath);

            if (!rewrite)
            {
                if (File.Exists(path))
                    return path;
            }
            else
            {
               // File.Delete(path);
            }

            FileUtils.CreateIfNotExist(System.IO.Path.GetDirectoryName(path));

            while (Client.IsBusy)
            {
                Thread.Sleep(100);
            }

            Client.DownloadFileAsync(uri, path);

            return path;
        }

        private string GetAbsolutePath(PathOptions options, string subPath)
            => options == PathOptions.Partial ? Path.JoinPath(subPath) : subPath;
    }
}
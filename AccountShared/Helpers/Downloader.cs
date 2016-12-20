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

        public string Path { get; set; }

        public string DownloadAsync(Uri uri, string subPath, PathOptions options = PathOptions.Partial, bool rewrite = false)
            => DownloadAsync(uri.AbsoluteUri, subPath, options, rewrite);

        public string DownloadAsync(string absoluteUri, string subPath, PathOptions options = PathOptions.Partial, bool rewrite = false)
        {
            return 
                InternalDownload(new Uri(absoluteUri), subPath, async (u, p) => await Client.DownloadFileTaskAsync(u, p), async t => await Task.Delay(t), rewrite, options);
        }

        private string InternalDownload(Uri uri, string subPath, Action<Uri, string> download, Action<int> wait, bool rewrite = false, PathOptions options = PathOptions.Partial)
        {
            if (uri == null) return null;
            var path = GetAbsolutePath(options, subPath);

            if (!rewrite)
            {
                if (File.Exists(path))
                    return path;
            }
            else File.Delete(path);

            FileUtils.CreateIfNotExist(System.IO.Path.GetDirectoryName(path));

            while (Client.IsBusy)
            {
                wait(50);
            }

            download(uri, path);

            return path;
        }

        public string DownloadSyncronously(Uri uri, string subPath, PathOptions options = PathOptions.Partial, bool rewrite = false)
           => DownloadSyncronously(uri.AbsoluteUri, subPath, options, rewrite);

        public  string DownloadSyncronously(string absoluteUri, string subPath, PathOptions options = PathOptions.Partial, bool rewrite = false)
        {
            return InternalDownload(new Uri(absoluteUri), subPath, Client.DownloadFile, Thread.Sleep, rewrite, options);
        }

        private string GetAbsolutePath(PathOptions options, string subPath)
            => options == PathOptions.Partial ? Path.JoinPath(subPath) : subPath;
    }
}
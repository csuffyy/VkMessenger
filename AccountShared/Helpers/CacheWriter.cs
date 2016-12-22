using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using VkData.Interface;

namespace VkData.Helpers
{
    public class CacheWriter : IFileService
    {
        public CacheWriter(ISerializer serializer, ILogger logger)
        {
            Serializer = serializer;
            Logger = logger;
        }

        public CacheWriter(string path, ISerializer serializer, ILogger logger)
        {
            Path = path;
            Serializer = serializer;
            Logger = logger;
        }

        private ISerializer Serializer { get; }
        public ILogger Logger { get; set; }

        public string Path { get; set; }

        public async void Write(object o, string fileName)
        {
            var json = Serializer.SerializeObject(o);
            var path = FileUtils.JoinAndCreate(Path, fileName);

            if (!File.Exists(path))
                File.Create(path).Dispose();

            using (var reader = new StreamReader(path))
            {
                if (json == await reader.ReadToEndAsync())
                    return;
            }

            File.WriteAllText(path, string.Empty);
            using (var writer = new StreamWriter(path, true))
            {
                await writer.WriteAsync(json);
            }
        }

        public void WriteCollection<TSource>(IEnumerable<TSource> collection, Func<TSource, string> nameSelector,
            string folderName)
        {
            collection.ThrowIfNull();
            var path = FileUtils.JoinAndCreate(Path, folderName);

            FileUtils.CreateIfNotExist(path);
            foreach (var item in collection)
            {
                var subPath = folderName.JoinPath(nameSelector(item));
                Write(item, subPath);
            }
        }


        public TSource Load<TSource>(string subPath) where TSource : new()
        {
            return Load((TSource)(object)null, subPath);
        }

        public TSource Load<TSource>(TSource item, string subPath) where TSource : new()
        {
            var filePath = FileUtils.JoinAndCreate(Path, subPath);

            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
                return new TSource();

            string content;
            using (var reader = new StreamReader(filePath))
            {
                content = reader.ReadToEnd();
            }

            var deserializedObject =
                new Try<TSource>(
                    () => Serializer.DeserializeObject<TSource>(content),
                    _ => Delete(subPath),
                    Logger);

            var value = deserializedObject.Result.Value;
            Debug.Assert(value != null, "value != null");
            return value;
        }


        public TCollection LoadCollection<TCollection, TSource>(TSource sentinel,
            TCollection sentinelCollection,
            string folderName) where TCollection : ICollection<TSource>, new() where TSource : new()
        {
            var collection = new TCollection();
            var path = FileUtils.JoinAndCreate(Path, folderName);
            if (!Directory.Exists(path))
                return collection;
            foreach (var subPath in Directory.GetFiles(path))
            {
                var item = default(TSource);
                var name = System.IO.Path.GetFileName(subPath);
                collection.Add(Load(item, folderName.JoinPath(name)));
            }
            return collection;
        }

        public void DeleteAllCache()
        {
            if (Directory.Exists(Path))
                Directory.Delete(Path);
        }

        internal void Delete(params string[] files)
        {
            foreach (var path in files.Select(item => FileUtils.JoinAndCreate(Path, item)).Where(File.Exists))
            {
                File.Delete(path);
            }
        }
    }
}
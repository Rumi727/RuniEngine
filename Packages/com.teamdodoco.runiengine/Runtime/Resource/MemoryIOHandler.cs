#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Resource;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace RuniEngine
{
    /// <summary>
    /// Thread-safe
    /// </summary>
    public sealed class MemoryIOHandler : IOHandler
    {
        class VirtualDirectory : IDisposable
        {
            public readonly ConcurrentDictionary<string, VirtualDirectory> directories = new();
            public readonly ConcurrentDictionary<string, VirtualFile> files = new();

            public VirtualDirectory? GetDirectory(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return this;

                string[] paths = path.Split(PathUtility.directorySeparatorChars);
                VirtualDirectory directory = this;

                for (int i = 0; i < paths.Length; i++)
                {
                    path = paths[i];
                    if (!directory.directories.TryGetValue(path, out directory))
                        return null;
                }

                return directory;
            }

            public VirtualDirectory GetNestDirectory(string path, out string childPath)
            {
                childPath = path;

                if (string.IsNullOrEmpty(path))
                    return this;

                string[] paths = path.Split(PathUtility.directorySeparatorChars);
                VirtualDirectory result = this;

                for (int i = 0; i < paths.Length; i++)
                {
                    path = paths[i];
                    if (!result.directories.TryGetValue(path, out VirtualDirectory directory))
                        break;

                    result = directory;
                    childPath = PathUtility.GetParentPath(childPath);

                    continue;
                }

                return result;
            }

            public VirtualFile? GetFile(string path)
            {
                string parentPath = PathUtility.GetParentPath(path);
                string fileName = PathUtility.GetFileName(path);

                VirtualDirectory? directory = GetDirectory(parentPath);
                if (directory == null)
                    return null;

                return directory.files.GetValueOrDefault(fileName);
            }

            public void Dispose()
            {
                directories.Clear();
                files.Clear();
            }
        }

        class VirtualFile
        {
            readonly IOHandler ioHandler = empty;
            readonly string? shortcutPath = null;

            readonly MemoryStream content = (MemoryStream)Stream.Null;

            VirtualFile(IOHandler ioHandler, string? shortcutPath)
            {
                this.ioHandler = ioHandler;
                this.shortcutPath = shortcutPath;
            }

            VirtualFile(byte[] content) => this.content = new MemoryStream(content.Copy());

            public byte[] ReadAllBytes()
            {
                if (shortcutPath != null)
                    return ioHandler.ReadAllBytes(shortcutPath);

                return content.ToArray();
            }

            public UniTask<byte[]> ReadAllBytesAsync()
            {
                if (shortcutPath != null)
                    return ioHandler.ReadAllBytesAsync(shortcutPath);

                return UniTask.RunOnThreadPool(() => content.ToArray());
            }

            public string ReadAllText()
            {
                if (shortcutPath != null)
                    return ioHandler.ReadAllText(shortcutPath);
                
                return Encoding.UTF8.GetString(content.ToArray());
            }

            public UniTask<string> ReadAllTextAsync()
            {
                if (shortcutPath != null)
                    return ioHandler.ReadAllTextAsync(shortcutPath);

                return UniTask.RunOnThreadPool(() => Encoding.UTF8.GetString(content.ToArray()));
            }

            public IEnumerable<string> ReadLines()
            {
                if (shortcutPath != null)
                    return ioHandler.ReadLines(shortcutPath);

                return Encoding.UTF8.GetString(content.ToArray()).ReadLines();
            }

            public Stream OpenRead()
            {
                if (shortcutPath != null)
                    return ioHandler.OpenRead(shortcutPath);

                return new MemoryStream(content.ToArray(), false);
            }

            public static VirtualFile CreateShortcut(IOHandler ioHandler) => new VirtualFile(ioHandler, string.Empty);
            public static VirtualFile CreateShortcut(IOHandler ioHandler, string path) => new VirtualFile(ioHandler, path);

            public static VirtualFile Create(byte[] content) => new VirtualFile(content);
            public static VirtualFile Create(string content) => new VirtualFile(Encoding.UTF8.GetBytes(content));
            public static VirtualFile Create(Stream stream) => new VirtualFile(stream.ReadFully());
        }

        public MemoryIOHandler() => directory = new VirtualDirectory();
        MemoryIOHandler(VirtualDirectory directory, MemoryIOHandler parent, string childPath) : base(parent, childPath) => this.directory = directory;



        public new MemoryIOHandler? parent => (MemoryIOHandler?)base.parent;

        /// <returns><see cref="MemoryIOHandler"/><br/>유니티 qt이 공변 반환 타입 지원 안하네 tllllllllqkf</returns>
        public override IOHandler CreateChild(string path)
        {
            /*GetNearestDirectory(path, out string childPath);
            return new MemoryIOHandler(directory?.GetDirectory(path), this, path);*/
            throw new NotImplementedException();
        }

        public override bool DirectoryExists(string path)
        {
            /*VirtualDirectory? directory = GetNearestDirectory(out string nearPath);
            directory = directory?.GetDirectory(PathUtility.Combine(nearPath, path));

            return directory != null;*/
            throw new NotImplementedException();
        }

        public override bool FileExists(string path, string extension = "") => throw new NotImplementedException();
        public override bool FileExists(string path, out string outPath, ExtensionFilter extensionFilter) => throw new NotImplementedException();

        public override IEnumerable<string> GetDirectories(string path) => throw new NotImplementedException();

        public override IEnumerable<string> GetAllDirectories(string path) => throw new NotImplementedException();

        public override IEnumerable<string> GetFiles(string path) => throw new NotImplementedException();
        public override IEnumerable<string> GetFiles(string path, ExtensionFilter extensionFilter) => throw new NotImplementedException();

        public override IEnumerable<string> GetAllFiles(string path) => throw new NotImplementedException();
        public override IEnumerable<string> GetAllFiles(string path, ExtensionFilter extensionFilter) => throw new NotImplementedException();

        public override byte[] ReadAllBytes(string path, string extension = "") => throw new NotImplementedException();

        public override UniTask<byte[]> ReadAllBytesAsync(string path, string extension = "") => throw new NotImplementedException();

        public override string ReadAllText(string path, string extension = "") => throw new NotImplementedException();

        public override UniTask<string> ReadAllTextAsync(string path, string extension = "") => throw new NotImplementedException();

        public override IEnumerable<string> ReadLines(string path, string extension = "") => throw new NotImplementedException();

        public override Stream OpenRead(string path, string extension = "") => throw new NotImplementedException();

        public override void Dispose() => directory?.Dispose();



        readonly VirtualDirectory directory;

        /*VirtualDirectory GetNearestDirectory(string path, out string childPath)
        {
            MemoryIOHandler? ioHandler = this;
            VirtualDirectory? directory = this.directory;
            childPath = string.Empty;

            while (directory == null)
            {
                string childPath = ioHandler.childPath;
                ioHandler = parent;

                if (ioHandler == null)
                    break;

                directory = ioHandler.directory;
                childPath = PathUtility.Combine(childPath, childPath);
            }

            return directory;
        }

        VirtualDirectory? GetDirectory(string path)
        {
            VirtualDirectory? directory = GetNearestDirectory(out string nearPath);
            return directory?.GetDirectory(PathUtility.Combine(nearPath, path));
        }*/
    }
}

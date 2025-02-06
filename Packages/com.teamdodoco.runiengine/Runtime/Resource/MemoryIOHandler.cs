#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Resource;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections.Concurrent;
using System.Text;
using RuniEngine.Spans;
using System.Linq;
using System.Text.RegularExpressions;

namespace RuniEngine
{
    /// <summary>
    /// Thread-safe
    /// </summary>
    public sealed class MemoryIOHandler : IOHandler
    {
        class VirtualDirectory
        {
            public readonly ConcurrentDictionary<string, VirtualDirectory> directories = new();
            public readonly ConcurrentDictionary<string, VirtualFile> files = new();

            public VirtualDirectory? GetDirectory(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return this;

                VirtualDirectory directory = this;
                foreach (var item in path.AsSpan().SplitAny(PathUtility.directorySeparatorChars))
                {
                    path = new string(item);
                    if (!directory.directories.TryGetValue(path, out directory))
                        return null;
                }

                return directory;
            }

            public VirtualFile? GetFile(string path)
            {
                string parentPath = PathUtility.GetParentPath(path);
                string fileName = PathUtility.GetFileName(path);

                return GetDirectory(parentPath)?.files.GetValueOrDefault(fileName);
            }
        }

        class VirtualFile
        {
            readonly IOHandler? ioHandler = empty;
            readonly Stream content = Stream.Null;

            VirtualFile(IOHandler ioHandler) => this.ioHandler = ioHandler;
            VirtualFile(Stream stream) => content = stream;

            public byte[] ReadAllBytes() => ioHandler?.ReadAllBytes() ?? content.ReadFully();

            public UniTask<byte[]> ReadAllBytesAsync() => ioHandler?.ReadAllBytesAsync() ?? UniTask.RunOnThreadPool(() => content.ReadFully());

            public string ReadAllText() => ioHandler?.ReadAllText() ?? Encoding.UTF8.GetString(content.ReadFully());

            public UniTask<string> ReadAllTextAsync() => ioHandler?.ReadAllTextAsync() ?? UniTask.RunOnThreadPool(() => Encoding.UTF8.GetString(content.ReadFully()));

            public IEnumerable<string> ReadLines() => ioHandler?.ReadLines() ?? Encoding.UTF8.GetString(content.ReadFully()).ReadLines();

            public Stream OpenRead() => ioHandler?.OpenRead() ?? new MemoryStream(content.ReadFully(), false);

            public static VirtualFile CreateShortcut(IOHandler ioHandler) => new VirtualFile(ioHandler);

            public static VirtualFile Create(byte[] content) => Create(new MemoryStream(content, false));
            public static VirtualFile Create(string content) => Create(new MemoryStream(Encoding.UTF8.GetBytes(content), false));
            public static VirtualFile Create(Stream stream) => new VirtualFile(stream);
        }

        public MemoryIOHandler() => rootDirectory = new VirtualDirectory();
        MemoryIOHandler(VirtualDirectory rootDirectory, MemoryIOHandler? parent, string childPath) : base(parent, childPath) => this.rootDirectory = rootDirectory;

        public new MemoryIOHandler? parent => (MemoryIOHandler?)base.parent;



        readonly VirtualDirectory rootDirectory;



        /// <returns><see cref="MemoryIOHandler"/><br/>유니티 qt이 공변 반환 타입 지원 안하네 tllllllllqkf</returns>
        public override IOHandler CreateChild(string path)
        {
            MemoryIOHandler handler = this;
            if (string.IsNullOrEmpty(path))
                return handler;

            foreach (var item in path.AsSpan().SplitAny(PathUtility.directorySeparatorChars))
            {
                string childPath = new string(item);
                handler = new MemoryIOHandler(rootDirectory, handler, childPath);
            }

            return handler;
        }

        public override IOHandler AddExtension(string extension) => new MemoryIOHandler(rootDirectory, parent, childPath + extension);



        public override bool DirectoryExists() => rootDirectory.GetDirectory(childFullPath) != null;

        public override bool FileExists() => rootDirectory.GetFile(childFullPath) != null;
        public override bool FileExists(out IOHandler outHandler, ExtensionFilter extensionFilter) => ResourceManager.FileExtensionExists(this, out outHandler, extensionFilter);


        public override IEnumerable<string> GetDirectories()
        {
            VirtualDirectory? directory = rootDirectory.GetDirectory(childFullPath) ?? throw new DirectoryNotFoundException(childFullPath);
            foreach (var item in directory.directories)
                yield return item.Key;
        }

        public override IEnumerable<string> GetAllDirectories() => InternalGetAllDirectories().Select(x => x.Key);

        IEnumerable<KeyValuePair<string, VirtualDirectory>> InternalGetAllDirectories()
        {
            return Recurse(this, rootDirectory.GetDirectory(childFullPath) ?? throw new DirectoryNotFoundException(childFullPath), string.Empty);

            static IEnumerable<KeyValuePair<string, VirtualDirectory>> Recurse(MemoryIOHandler handler, VirtualDirectory directory, string parentPath)
            {
                foreach (var item in directory.directories)
                {
                    string path = parentPath + item.Key;
                    yield return new KeyValuePair<string, VirtualDirectory>(PathUtility.GetRelativePath(handler.parent?.childFullPath ?? string.Empty, path), item.Value);

                    foreach (var subItem in Recurse(handler, item.Value, path))
                        yield return subItem;
                }
            }
        }


        public override IEnumerable<string> GetFiles()
        {
            VirtualDirectory? directory = rootDirectory.GetDirectory(childFullPath) ?? throw new DirectoryNotFoundException(childFullPath);
            return directory.files.Select(item => item.Key);
        }

        public override IEnumerable<string> GetFiles(ExtensionFilter extensionFilter)
        {
            VirtualDirectory? directory = rootDirectory.GetDirectory(childFullPath) ?? throw new DirectoryNotFoundException(childFullPath);
            return FilterFiles(directory.files.Keys, extensionFilter);
        }

        public override IEnumerable<string> GetAllFiles()
        {
            var directories = InternalGetAllDirectories();
            return directories.SelectMany(directoryItem => directoryItem.Value.files.Select(fileItem => PathUtility.Combine(directoryItem.Key, fileItem.Key)));
        }

        public override IEnumerable<string> GetAllFiles(ExtensionFilter extensionFilter) => FilterFiles(GetAllFiles(), extensionFilter);

        static IEnumerable<string> FilterFiles(IEnumerable<string> files, ExtensionFilter extensionFilter)
        {
            IEnumerable<string> patterns = extensionFilter.ToString().Split('|').Select(ConvertPatternToRegex);

            // `*` 패턴이 포함되어 있다면 바로 모든 파일 반환
            if (patterns.Contains(".*"))
                return files;

            return files.Where(file => patterns.Any(pattern => Regex.IsMatch(file, pattern, RegexOptions.IgnoreCase))).ToList();
        }

        static string ConvertPatternToRegex(string pattern)
        {
            if (pattern == "*" || pattern == "*.*")
                return ".*"; // 모든 파일을 허용하는 패턴

            string escaped = Regex.Escape(pattern).Replace(@"\*", ".*"); // '*'를 '.*'로 변환
            return $"^{escaped}$";
        }

        public override byte[] ReadAllBytes() => rootDirectory.GetFile(childFullPath)?.ReadAllBytes() ?? throw new FileNotFoundException();

        public override UniTask<byte[]> ReadAllBytesAsync() => rootDirectory.GetFile(childFullPath)?.ReadAllBytesAsync() ?? throw new FileNotFoundException();

        public override string ReadAllText() => rootDirectory.GetFile(childFullPath)?.ReadAllText() ?? throw new FileNotFoundException();

        public override UniTask<string> ReadAllTextAsync() => rootDirectory.GetFile(childFullPath)?.ReadAllTextAsync() ?? throw new FileNotFoundException();

        public override IEnumerable<string> ReadLines() => rootDirectory.GetFile(childFullPath)?.ReadLines() ?? throw new FileNotFoundException();

        public override Stream OpenRead() => rootDirectory.GetFile(childFullPath)?.OpenRead() ?? throw new FileNotFoundException();



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

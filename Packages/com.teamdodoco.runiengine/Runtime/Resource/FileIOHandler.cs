#nullable enable
using Cysharp.Threading.Tasks;
using RuniEngine.Spans;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Resource
{
    public class FileIOHandler : IOHandler
    {
        public FileIOHandler(string path) : base() => this.path = path;
        FileIOHandler(FileIOHandler? parent, string childPath) : base(parent, childPath) => path = PathUtility.Combine((parent?.path ?? string.Empty), childPath);

        public new FileIOHandler? parent => (FileIOHandler?)base.parent;



        public string path { get; } = string.Empty;



        /// <returns><see cref="FileIOHandler"/><br/>유니티 qt이 공변 반환 타입 지원 안하네 tllllllllqkf</returns>
        public override IOHandler CreateChild(string path)
        {
            FileIOHandler handler = this;
            if (string.IsNullOrEmpty(path))
                return handler;

            foreach (var item in path.AsSpan().SplitAny(PathUtility.directorySeparatorChars))
            {
                string childPath = new string(item);
                handler = new FileIOHandler(handler, childPath);
            }
            
            return handler;
        }

        public override IOHandler AddExtension(string extension) => new FileIOHandler(parent, childPath + extension);



        public override bool DirectoryExists() => Directory.Exists(path);

        public override bool FileExists() => File.Exists(path);
        public override bool FileExists(out IOHandler handler, ExtensionFilter extensionFilter) => ResourceManager.FileExtensionExists(this, out handler, extensionFilter);

        public override IEnumerable<string> GetDirectories() => Directory.EnumerateDirectories(path).Select(x => PathUtility.GetRelativePath(path, x));

        public override IEnumerable<string> GetAllDirectories() => Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories).Select(x => PathUtility.GetRelativePath(path, x));

        public override IEnumerable<string> GetFiles() => Directory.EnumerateFiles(path).Select(x => PathUtility.GetRelativePath(path, x));
        public override IEnumerable<string> GetFiles(ExtensionFilter extensionFilter) => DirectoryUtility.EnumerateFiles(path, extensionFilter).Select(x => PathUtility.GetRelativePath(path, x));

        public override IEnumerable<string> GetAllFiles() => Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Select(x => PathUtility.GetRelativePath(path, x));
        public override IEnumerable<string> GetAllFiles(ExtensionFilter extensionFilter) => DirectoryUtility.EnumerateFiles(path, extensionFilter, SearchOption.AllDirectories).Select(x => PathUtility.GetRelativePath(path, x));

        public override byte[] ReadAllBytes() => File.ReadAllBytes(path);

        public override UniTask<byte[]> ReadAllBytesAsync() => File.ReadAllBytesAsync(path).AsUniTask();

        public override string ReadAllText() => File.ReadAllText(path);

        public override UniTask<string> ReadAllTextAsync() => File.ReadAllTextAsync(path).AsUniTask();

        public override IEnumerable<string> ReadLines() => File.ReadLines(path);

        /// <returns><see cref="FileStream"/><br/>유니티 qt이 공변 반환 타입 지원 안하네 tllllllllqkf</returns>
        public override Stream OpenRead() => File.OpenRead(path);
    }
}

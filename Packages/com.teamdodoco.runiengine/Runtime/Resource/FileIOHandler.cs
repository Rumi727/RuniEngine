#nullable enable
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Resource
{
    public class FileIOHandler : IOHandler
    {
        public FileIOHandler(string path) => this.path = path.UniformDirectorySeparatorCharacter();
        protected FileIOHandler(string path, FileIOHandler parent, string childPath) : base(parent, childPath) => this.path = path.UniformDirectorySeparatorCharacter();

        public string path { get; }
        public string fullPath => Path.GetFullPath(path);



        public new FileIOHandler? parent => (FileIOHandler?)base.parent;

        /// <returns><see cref="FileIOHandler"/><br/>유니티 qt이 공변 반환 타입 지원 안하네 tllllllllqkf</returns>
        public override IOHandler CreateChild(string path) => new FileIOHandler(Path.Combine(this.path, path), this, path);



        public override bool DirectoryExists(string path) => Directory.Exists(Path.Combine(this.path, path));

        public override bool FileExists(string path, string extension = "") => File.Exists(Path.Combine(this.path, path) + extension);
        public override bool FileExists(string path, out string outPath, ExtensionFilter extensionFilter) => ResourceManager.FileExtensionExists(this, Path.Combine(this.path, path), out outPath, extensionFilter);

        public override IEnumerable<string> GetDirectories(string path) => Directory.EnumerateDirectories(Path.Combine(this.path, path)).Select(x => PathUtility.GetRelativePath(this.path, x));

        public override IEnumerable<string> GetAllDirectories(string path) => Directory.EnumerateDirectories(Path.Combine(this.path, path), "*", SearchOption.AllDirectories).Select(x => PathUtility.GetRelativePath(this.path, x));

        public override IEnumerable<string> GetFiles(string path) => Directory.EnumerateFiles(Path.Combine(this.path, path)).Select(x => PathUtility.GetRelativePath(this.path, x));
        public override IEnumerable<string> GetFiles(string path, ExtensionFilter extensionFilter) => DirectoryUtility.EnumerateFiles(Path.Combine(this.path, path), extensionFilter).Select(x => PathUtility.GetRelativePath(this.path, x));

        public override IEnumerable<string> GetAllFiles(string path) => Directory.EnumerateFiles(Path.Combine(this.path, path), "*", SearchOption.AllDirectories).Select(x => PathUtility.GetRelativePath(this.path, x));
        public override IEnumerable<string> GetAllFiles(string path, ExtensionFilter extensionFilter) => DirectoryUtility.EnumerateFiles(Path.Combine(this.path, path), extensionFilter, SearchOption.AllDirectories).Select(x => PathUtility.GetRelativePath(this.path, x));

        public override byte[] ReadAllBytes(string path, string extension = "") => File.ReadAllBytes(Path.Combine(this.path, path) + extension);

        public override UniTask<byte[]> ReadAllBytesAsync(string path, string extension = "") => File.ReadAllBytesAsync(Path.Combine(this.path, path) + extension).AsUniTask();

        public override string ReadAllText(string path, string extension = "") => File.ReadAllText(Path.Combine(this.path, path) + extension);

        public override UniTask<string> ReadAllTextAsync(string path, string extension = "") => File.ReadAllTextAsync(Path.Combine(this.path, path) + extension).AsUniTask();

        public override IEnumerable<string> ReadLines(string path, string extension = "") => File.ReadLines(Path.Combine(this.path, path) + extension);

        /// <returns><see cref="FileStream"/><br/>유니티 qt이 공변 반환 타입 지원 안하네 tllllllllqkf</returns>
        public override Stream OpenRead(string path, string extension = "") => File.OpenRead(Path.Combine(this.path, path) + extension);
    }
}

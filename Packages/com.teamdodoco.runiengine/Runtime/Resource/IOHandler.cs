#nullable enable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Resource
{
    public abstract class IOHandler : IDisposable
    {
        public static readonly IOHandler empty = new EmptyIOHandler();



        public IOHandler() { }
        protected IOHandler(IOHandler parent, string childPath)
        {
            this.parent = parent;
            this.childPath = childPath.UniformDirectorySeparatorCharacter();
        }



        public IOHandler? parent { get; }

        public string childPath { get; } = string.Empty;
        public string childFullPath => PathUtility.Combine(parent?.childPath, childPath);

        public abstract IOHandler CreateChild(string path);

        public bool DirectoryExists() => DirectoryExists(string.Empty);
        public abstract bool DirectoryExists(string path);

        public bool FileExists() => FileExists(string.Empty);
        public bool FileExists(out string outPath, ExtensionFilter extensionFilter) => FileExists(out outPath, extensionFilter);
        public abstract bool FileExists(string path, string extension = "");
        public abstract bool FileExists(string path, out string outPath, ExtensionFilter extensionFilter);

        public IEnumerable<string> GetDirectories() => GetDirectories(string.Empty);
        public abstract IEnumerable<string> GetDirectories(string path);

        public IEnumerable<string> GetAllDirectories() => GetAllDirectories(string.Empty);
        public abstract IEnumerable<string> GetAllDirectories(string path);

        public IEnumerable<string> GetFiles() => GetFiles(string.Empty);
        public IEnumerable<string> GetFiles(ExtensionFilter extensionFilter) => GetFiles(string.Empty, extensionFilter);
        public abstract IEnumerable<string> GetFiles(string path);
        public abstract IEnumerable<string> GetFiles(string path, ExtensionFilter extensionFilter);

        public IEnumerable<string> GetAllFiles() => GetAllFiles(string.Empty);
        public IEnumerable<string> GetAllFiles(ExtensionFilter extensionFilter) => GetAllFiles(string.Empty, extensionFilter);
        public abstract IEnumerable<string> GetAllFiles(string path);
        public abstract IEnumerable<string> GetAllFiles(string path, ExtensionFilter extensionFilter);

        public byte[] ReadAllBytes() => ReadAllBytes(string.Empty);
        public abstract byte[] ReadAllBytes(string path, string extension = "");

        public UniTask<byte[]> ReadAllBytesAsync() => ReadAllBytesAsync(string.Empty);
        public abstract UniTask<byte[]> ReadAllBytesAsync(string path, string extension = "");

        public string ReadAllText() => ReadAllText(string.Empty);
        public abstract string ReadAllText(string path, string extension = "");

        public UniTask<string> ReadAllTextAsync() => ReadAllTextAsync(string.Empty);
        public abstract UniTask<string> ReadAllTextAsync(string path, string extension = "");

        public IEnumerable<string> ReadLines() => ReadLines(string.Empty);
        public abstract IEnumerable<string> ReadLines(string path, string extension = "");

        public Stream OpenRead() => OpenRead(string.Empty);
        public abstract Stream OpenRead(string path, string extension = "");

        public virtual void Dispose() { }

        sealed class EmptyIOHandler : IOHandler
        {
            public EmptyIOHandler() { }
            EmptyIOHandler(IOHandler parent, string childPath) : base(parent, childPath) { }

            public override IOHandler CreateChild(string path) => new EmptyIOHandler(this, path);

            public override bool DirectoryExists(string path) => false;

            public override bool FileExists(string path, string extension = "") => false;
            public override bool FileExists(string path, out string outPath, ExtensionFilter extensionFilter)
            {
                outPath = string.Empty;
                return false;
            }

            public override IEnumerable<string> GetDirectories(string path) => Enumerable.Empty<string>();

            public override IEnumerable<string> GetAllDirectories(string path) => Enumerable.Empty<string>();

            public override IEnumerable<string> GetFiles(string path) => Enumerable.Empty<string>();
            public override IEnumerable<string> GetFiles(string path, ExtensionFilter extensionFilter) => Enumerable.Empty<string>();

            public override IEnumerable<string> GetAllFiles(string path) => Enumerable.Empty<string>();
            public override IEnumerable<string> GetAllFiles(string path, ExtensionFilter extensionFilter) => Enumerable.Empty<string>();

            public override byte[] ReadAllBytes(string path, string extension = "") => Array.Empty<byte>();

            public override UniTask<byte[]> ReadAllBytesAsync(string path, string extension = "") => new UniTask<byte[]>(Array.Empty<byte>());

            public override string ReadAllText(string path, string extension = "") => string.Empty;

            public override UniTask<string> ReadAllTextAsync(string path, string extension = "") => new UniTask<string>(string.Empty);

            public override IEnumerable<string> ReadLines(string path, string extension = "") => Enumerable.Empty<string>();

            public override Stream OpenRead(string path, string extension = "") => Stream.Null;
        }
    }
}

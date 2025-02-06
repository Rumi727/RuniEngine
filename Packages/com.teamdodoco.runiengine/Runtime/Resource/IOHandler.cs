#nullable enable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RuniEngine.Resource
{
    public abstract class IOHandler
    {
        public static readonly IOHandler empty = new EmptyIOHandler();



        protected IOHandler() { }
        protected IOHandler(IOHandler? parent, string childPath)
        {
            this.parent = parent;

            this.childPath = childPath.UniformDirectorySeparatorCharacter();
            childFullPath = PathUtility.Combine(parent?.childFullPath, this.childPath);
        }



        public IOHandler? parent { get; }

        public string childPath { get; } = string.Empty;
        public string childFullPath { get; } = string.Empty;



        public abstract IOHandler CreateChild(string path);
        public abstract IOHandler AddExtension(string extension);

        public abstract bool DirectoryExists();

        public abstract bool FileExists();
        public abstract bool FileExists(out IOHandler handler, ExtensionFilter extensionFilter);

        public abstract IEnumerable<string> GetDirectories();

        public abstract IEnumerable<string> GetAllDirectories();

        public abstract IEnumerable<string> GetFiles();
        public abstract IEnumerable<string> GetFiles(ExtensionFilter extensionFilter);

        public abstract IEnumerable<string> GetAllFiles();
        public abstract IEnumerable<string> GetAllFiles(ExtensionFilter extensionFilter);

        public abstract byte[] ReadAllBytes();

        public abstract UniTask<byte[]> ReadAllBytesAsync();

        public abstract string ReadAllText();

        public abstract UniTask<string> ReadAllTextAsync();

        public abstract IEnumerable<string> ReadLines();

        public abstract Stream OpenRead();

        sealed class EmptyIOHandler : IOHandler
        {
            public EmptyIOHandler() { }
            EmptyIOHandler(IOHandler? parent, string childPath) : base(parent, childPath) { }

            public override IOHandler CreateChild(string path) => new EmptyIOHandler(this, path);
            public override IOHandler AddExtension(string extension) => new EmptyIOHandler(parent, childPath + extension);

            public override bool DirectoryExists() => false;

            public override bool FileExists() => false;
            public override bool FileExists(out IOHandler outPath, ExtensionFilter extensionFilter)
            {
                outPath = empty;
                return false;
            }

            public override IEnumerable<string> GetDirectories() => Enumerable.Empty<string>();

            public override IEnumerable<string> GetAllDirectories() => Enumerable.Empty<string>();

            public override IEnumerable<string> GetFiles() => Enumerable.Empty<string>();
            public override IEnumerable<string> GetFiles(ExtensionFilter extensionFilter) => Enumerable.Empty<string>();

            public override IEnumerable<string> GetAllFiles() => Enumerable.Empty<string>();
            public override IEnumerable<string> GetAllFiles(ExtensionFilter extensionFilter) => Enumerable.Empty<string>();

            public override byte[] ReadAllBytes() => Array.Empty<byte>();

            public override UniTask<byte[]> ReadAllBytesAsync() => new UniTask<byte[]>(Array.Empty<byte>());

            public override string ReadAllText() => string.Empty;

            public override UniTask<string> ReadAllTextAsync() => new UniTask<string>(string.Empty);

            public override IEnumerable<string> ReadLines() => Enumerable.Empty<string>();

            public override Stream OpenRead() => Stream.Null;
        }
    }
}

#nullable enable
namespace RuniEngine.Resource
{
    public readonly struct ExtensionFilter
    {
        readonly static ExtensionFilter _allFileFilter = new ExtensionFilter("*");
        public static ExtensionFilter allFileFilter => _allFileFilter;



        readonly static ExtensionFilter _pictureFileFilter = new ExtensionFilter(".png", ".jpg", ".jif", ".jpeg", ".jpe", ".bmp", ".exr", ".gif", ".hdr", ".iff", ".pict", ".tif", ".tiff", ".psd", ".ico", ".jng", ".koa", ".lbm", ".mng", ".pbm", ".pcd", ".pcx", ".pgm", ".ppm", ".ras", ".tga", ".targa", ".wbpm", ".cut", ".xbm", ".xpm", ".dds", ".g3", ".sgi", ".j2k", ".j2c", ".jp2", ".pfm", ".webp", ".jxr");
        public static ExtensionFilter pictureFileFilter => _pictureFileFilter;

        readonly static ExtensionFilter _textFileFilter = new ExtensionFilter(".txt", ".html", ".htm", ".xml", ".bytes", ".json", ".csv", ".yaml", ".fnt");
        public static ExtensionFilter textFileFilter => _textFileFilter;

        readonly static ExtensionFilter _musicFileFilter = new ExtensionFilter(".ogg", ".mp3", ".mp2", ".wav", ".aif", ".xm", ".mod", ".it", ".vag", ".xma", ".s3m");
        public static ExtensionFilter musicFileFilter => _musicFileFilter;

        readonly static ExtensionFilter _nbsFileFilter = new ExtensionFilter(".nbs");
        public static ExtensionFilter nbsFileFilter => _nbsFileFilter;

        readonly static ExtensionFilter _videoFileFilter = new ExtensionFilter(".asf", ".avi", ".dv", ".m4v", ".mov", ".mp4", ".mpg", ".mpeg", ".ogv", ".vp8", ".webm", ".wmv");
        public static ExtensionFilter videoFileFilter => _videoFileFilter;

        readonly static ExtensionFilter _compressFileFilter = new ExtensionFilter(".zip");
        public static ExtensionFilter compressFileFilter => _compressFileFilter;

        readonly static ExtensionFilter _codeFileFilter = new ExtensionFilter(".java", ".php", ".scss", ".cs", ".css", ".js", ".py", ".c", ".cpp", ".class", ".fs", ".go", ".rb");
        public static ExtensionFilter codeFileFilter => _codeFileFilter;



        //public NameSpacePathPair label { get; }
        public string[] extensions { get; }

        public ExtensionFilter(params string[] extensions)
        {
            //label = new NameSpacePathPair("");
            this.extensions = extensions;
        }

        /*public ExtensionFilter(NameSpacePathPair label, params string[] extensions)
        {
            this.label = label;
            this.extensions = extensions;
        }*/

        public static implicit operator ExtensionFilter(string value) => new ExtensionFilter(value.Split("|"));

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < extensions.Length; i++)
            {
                if (i < extensions.Length - 1)
                    result += extensions[i] + "|";
                else
                    result += extensions[i];
            }

            return result;
        }
    }
}

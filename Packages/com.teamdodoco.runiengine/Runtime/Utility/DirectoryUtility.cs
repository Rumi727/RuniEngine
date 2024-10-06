using RuniEngine.Resource;
using System.IO;
using System.Linq;

namespace RuniEngine
{
    public static class DirectoryUtility
    {
        public static void Copy(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                Copy(folder, dest);
            }
        }

        public static string[] GetFiles(string path, ExtensionFilter extensionFilter) => GetFiles(path, extensionFilter, SearchOption.TopDirectoryOnly);
        
        public static string[] GetFiles(string path, ExtensionFilter extensionFilter, SearchOption searchOption)
        {
            if (extensionFilter.extensions.Length == 1)
                return Directory.GetFiles(path, "*" + extensionFilter.extensions[0], searchOption);

            return Directory.EnumerateFiles(path, "*", searchOption).Where(x =>
            {
                for (int i = 0; i < extensionFilter.extensions.Length; i++)
                {
                    if (x.EndsWith(extensionFilter.extensions[i]))
                        return true;
                }

                return false;
            }).ToArray();
        }
    }
}

#nullable enable
using System.IO;
using UnityEngine.Networking;

namespace RuniEngine
{
    public static class PathUtility
    {
        public const string urlPathPrefix = "file:///";

        public static string RemoveInvalidPathChars(string filename) => string.Concat(filename.Split(Path.GetInvalidPathChars()));
        public static string ReplaceInvalidPathChars(string filename) => string.Join("_", filename.Split(Path.GetInvalidPathChars()));

        public static string RemoveInvalidFileNameChars(string filename) => string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        public static string ReplaceInvalidFileNameChars(string filename) => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        public static string GetPathWithExtension(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension != "")
                return path.Remove(path.Length - extension.Length);
            else
                return path;
        }

        public static string GetPathWithFileName(string path)
        {
            string fileName = Path.GetFileName(path);
            if (fileName != "")
                return path.Remove(path.Length - fileName.Length - 1);
            else
                return path;
        }

        public static string UrlPathPrefix(this string path) => urlPathPrefix + UnityWebRequest.EscapeURL(path);

        public static string UniformDirectorySeparatorCharacter(this string path) => path.UniformDirectorySeparatorCharacter('\\', '/');
        public static string UniformDirectorySeparatorCharacter(this string path, char altSeparatorChar, char separatorChar) => path.Replace(altSeparatorChar, separatorChar);
    }
}

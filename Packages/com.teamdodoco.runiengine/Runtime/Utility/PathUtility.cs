#nullable enable
using System.IO;
using System.Text;
using UnityEngine.Networking;

namespace RuniEngine
{
    public static class PathUtility
    {
        public const char directorySeparatorChar = '/';
        public const string urlPathPrefix = "file:///";

        public static string RemoveInvalidPathChars(string filename) => string.Concat(filename.Split(Path.GetInvalidPathChars()));
        public static string ReplaceInvalidPathChars(string filename) => string.Join("_", filename.Split(Path.GetInvalidPathChars()));

        public static string RemoveInvalidFileNameChars(string filename) => string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        public static string ReplaceInvalidFileNameChars(string filename) => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));

        public static string GetExtension(string path)
        {
            int index = path.LastIndexOf('.');
            if (index < 0)
                return string.Empty;

            return path.Substring(index);
        }

        public static string GetFileName(string path)
        {
            int index = path.LastIndexOfAny(directorySeparatorChars);
            if (index < 0)
                return path;

            return path.Substring(index + 1);
        }

        public static string GetPathWithExtension(string path)
        {
            string extension = GetExtension(path);
            if (extension != string.Empty)
                return path.Remove(path.Length - extension.Length);
            else
                return path;
        }

        public static string GetPathWithFileName(string path)
        {
            string fileName = GetFileName(path);
            if (fileName != string.Empty)
                return path.Remove(path.Length - fileName.Length);
            else
                return path;
        }

        static readonly char[] directorySeparatorChars = new char[] { '/', '\\' };
        public static string GetParentPath(string path)
        {
            int index = path.LastIndexOfAny(directorySeparatorChars);
            if (index < 0)
                return string.Empty;

            return path.Substring(0, index);
        }

        public static string UrlPathPrefix(this string path) => urlPathPrefix + UnityWebRequest.EscapeURL(path);

        public static string UniformDirectorySeparatorCharacter(this string path) => path.UniformDirectorySeparatorCharacter('\\', directorySeparatorChar);
        public static string UniformDirectorySeparatorCharacter(this string path, char altSeparatorChar, char separatorChar) => path.Replace(altSeparatorChar, separatorChar);

        public static string Combine(params string[] paths)
        {
            StringBuilder stringBuilder = StringBuilderCache.Acquire();
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].Length <= 0)
                    continue;

                if (stringBuilder.Length <= 0)
                {
                    stringBuilder.Append(paths[i]);
                    continue;
                }

                char last = stringBuilder[stringBuilder.Length - 1];
                if (last != '/')
                    stringBuilder.Append('/');

                stringBuilder.Append(paths[i]);
            }

            return StringBuilderCache.Release(stringBuilder);
        }

        public static string GetRelativePath(string relativeTo, string path)
        {
            relativeTo = relativeTo.UniformDirectorySeparatorCharacter();
            path = path.UniformDirectorySeparatorCharacter();

            if (relativeTo.Length <= 0)
                return path;

            if (path.Length <= 0)
                return string.Empty;

            if (path.StartsWith(relativeTo))
            {
                path = path.Substring(relativeTo.Length);
                if (path[0] == directorySeparatorChar)
                    path = path.Substring(1);
            }

            return path;
        }

        public static bool StartsWith(string path, string startPath)
        {
            string[] paths = path.Split(directorySeparatorChar);
            string[] startPaths = startPath.Split(directorySeparatorChar);

            if (paths.Length < startPaths.Length)
                return false;

            for (int i = 0; i < startPaths.Length; i++)
            {
                if (paths[i] != startPaths[i])
                    return false;
            }

            return true;
        }
    }
}

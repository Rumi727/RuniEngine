using Cysharp.Threading.Tasks;
using RuniEngine.Resource;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RuniEngine.Accounts
{
    public sealed class UserAccount : IDisposable
    {
        public string profile
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _profile;
            }
        }
        public string _profile;

        public string name
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _name;
            }
        }
        string _name;

        public string hashedPassword
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                return _hashedPassword;
            }
        }
        string _hashedPassword;

        public string path
        {
            get
            {
                if (isDisposed)
                    throw new ObjectDisposedException(GetType().FullName);

                string value = Path.Combine(UserAccountManager.accountsPath, PathUtility.ReplaceInvalidPathChars(name));
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                return value;
            }
        }

        public bool isDisposed { get; private set; } = false;

        UserAccount(string profile, string name, string hashedPassword)
        {
            _profile = profile;
            _name = name;

            _hashedPassword = hashedPassword;
        }

        public static async UniTask<UserAccount?> Create(UserAccountInfo info, string password)
        {
            if (info.hashedPassword == await UniTask.RunOnThreadPool(() => GetHashedPassword(password)))
            {
                string profile = "";
                if (ResourceManager.FileExtensionExists(Path.Combine(info.path, "profile"), out string profilePath, ExtensionFilter.pictureFileFilter))
                    profile = profilePath;

                return new UserAccount(profile, info.name, info.hashedPassword);
            }

            return null;
        }

        public static string GetHashedPassword(string password)
        {
            password = password.Trim();

            if (string.IsNullOrEmpty(password))
                return string.Empty;

            using (SHA512 hash = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashedBytes = hash.ComputeHash(bytes);

                StringBuilder stringBuilder = new StringBuilder(128);
                foreach (byte item in hashedBytes)
                    stringBuilder.Append(item.ToString("X2"));

                return stringBuilder.ToString();
            }
        }

        public void Dispose()
        {
            _name = "";
            _hashedPassword = "";

            isDisposed = true;
        }
    }
}

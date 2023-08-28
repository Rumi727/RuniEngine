#nullable enable
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RuniEngine.Account
{
    public sealed class UserAccount : IDisposable
    {
        public string path => Path.Combine(UserAccountManager.accountsPath, name);

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

        public bool isDisposed { get; private set; } = false;

        UserAccount(string name, string hashedPassword)
        {
            _name = name;
            _hashedPassword = hashedPassword;
        }

        public static bool Create(UserAccountInfo info, string password, out UserAccount? result)
        {
            if (info.hashedPassword == GetHashedPassword(password))
            {
                result = new UserAccount(info.name, info.hashedPassword);
                return true;
            }

            result = null;
            return false;
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

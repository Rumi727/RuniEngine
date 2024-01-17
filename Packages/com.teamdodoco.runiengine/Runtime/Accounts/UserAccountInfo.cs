#nullable enable
using System;
using System.IO;

namespace RuniEngine.Accounts
{
    public readonly struct UserAccountInfo : IEquatable<UserAccountInfo>
    {
        public string name { get; }
        public string hashedPassword { get; }

        public string path
        {
            get
            {
                string value = Path.Combine(UserAccountManager.accountsPath, PathUtility.ReplaceInvalidPathChars(name));
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                return value;
            }
        }

        public UserAccountInfo(string name, string hashedPassword)
        {
            this.name = name;
            this.hashedPassword = hashedPassword;
        }



        public static bool operator ==(UserAccountInfo lhs, UserAccountInfo rhs) => lhs.Equals(rhs);
        public static bool operator !=(UserAccountInfo lhs, UserAccountInfo rhs) => !lhs.Equals(rhs);

        public override bool Equals(object obj)
        {
            if (obj is UserAccountInfo info)
                return name == info.name;

            return false;
        }

        public bool Equals(UserAccountInfo other) => name == other.name;

        public override int GetHashCode() => name.GetHashCode();
    }
}
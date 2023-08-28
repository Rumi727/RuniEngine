#nullable enable
using System;

namespace RuniEngine.Account
{
    public readonly struct UserAccountInfo : IEquatable<UserAccountInfo>
    {
        public string name { get; }
        public string hashedPassword { get; }

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
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace RuniEngine
{
    [Serializable]
    public struct Version : IEquatable<Version>, IComparable, IComparable<Version>
    {
        [JsonIgnore] public static Version all => new Version();
        [JsonIgnore] public static Version zero => new Version(0, 0, 0);
        [JsonIgnore] public static Version one => new Version(1, 0, 0);

        public ulong? major;
        public ulong? minor;
        public ulong? patch;


        public Version(string? value)
        {
            if (value == null)
            {
                major = null;
                minor = null;
                patch = null;

                return;
            }

            string[] versions = value.Split(".");
            if (versions == null || versions.Length <= 0)
            {
                major = 0;
                minor = 0;
                patch = 0;
            }
            else if (versions.Length == 1)
            {
                if (ulong.TryParse(versions[0], out ulong major))
                    this.major = major;
                else
                    this.major = null;

                minor = 0;
                patch = 0;
            }
            else if (versions.Length == 2)
            {
                if (ulong.TryParse(versions[0], out ulong major))
                    this.major = major;
                else
                    this.major = null;

                if (ulong.TryParse(versions[1], out ulong minor))
                    this.minor = minor;
                else
                    this.minor = null;

                patch = 0;
            }
            else
            {
                {
                    if (ulong.TryParse(versions[0], out ulong major))
                        this.major = major;
                    else
                        this.major = null;
                }

                {
                    if (ulong.TryParse(versions[1], out ulong minor))
                        this.minor = minor;
                    else
                        this.minor = null;
                }

                {
                    if (ulong.TryParse(versions[2], out ulong patch))
                        this.patch = patch;
                    else
                        this.patch = null;
                }
            }
        }
        public Version(ulong? major, ulong? minor, ulong? patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        

        public static bool operator <=(Version lhs, Version rhs)
        {
            if ((lhs.major == null || rhs.major == null || lhs.major == rhs.major) && (lhs.minor == null || rhs.minor == null || lhs.minor == rhs.minor) && (lhs.patch == null || rhs.patch == null || lhs.patch <= rhs.patch))
                return true;
            else if ((lhs.major == null || rhs.major == null || lhs.major == rhs.major) && (lhs.minor == null || rhs.minor == null || lhs.minor < rhs.minor))
                return true;
            else if (lhs.major == null || rhs.major == null || lhs.major < rhs.major)
                return true;

            return false;
        }
        public static bool operator >=(Version lhs, Version rhs)
        {
            if ((lhs.major == null || rhs.major == null || lhs.major == rhs.major) && (lhs.minor == null || rhs.minor == null || lhs.minor == rhs.minor) && (lhs.patch == null || rhs.patch == null || lhs.patch >= rhs.patch))
                return true;
            else if ((lhs.major == null || rhs.major == null || lhs.major == rhs.major) && (lhs.minor == null || rhs.minor == null || lhs.minor > rhs.minor))
                return true;
            else if (lhs.major == null || rhs.major == null || lhs.major > rhs.major)
                return true;

            return false;
        }
        public static bool operator <(Version lhs, Version rhs)
        {
            if ((lhs.major == null || rhs.major == null || lhs.major == rhs.major) && (lhs.minor == null || rhs.minor == null || lhs.minor == rhs.minor) && (lhs.patch == null || rhs.patch == null || lhs.patch < rhs.patch))
                return true;
            else if ((lhs.major == null || rhs.major == null || lhs.major == rhs.major) && (lhs.minor == null || rhs.minor == null || lhs.minor < rhs.minor))
                return true;
            else if (lhs.major == null || rhs.major == null || lhs.major < rhs.major)
                return true;

            return false;
        }
        public static bool operator >(Version lhs, Version rhs)
        {
            if ((lhs.major == null || rhs.major == null || lhs.major == rhs.major) && (lhs.minor == null || rhs.minor == null || lhs.minor == rhs.minor) && (lhs.patch == null || rhs.patch == null || lhs.patch > rhs.patch))
                return true;
            else if ((lhs.major == null || rhs.major == null || lhs.major == rhs.major) && (lhs.minor == null || rhs.minor == null || lhs.minor > rhs.minor))
                return true;
            else if (lhs.major == null || rhs.major == null || lhs.major > rhs.major)
                return true;

            return false;
        }
        public static bool operator ==(Version lhs, Version rhs) => lhs.major == rhs.minor && lhs.minor == rhs.minor && lhs.patch == rhs.patch;
        public static bool operator !=(Version lhs, Version rhs) => !(lhs == rhs);



        public static explicit operator string(Version value) => value.ToString();
        public static explicit operator Version(string value) => new Version(value);

        public static explicit operator Vector3Int(Version value) => new Vector3Int((int)(value.major ?? 0), (int)(value.minor ?? 0), (int)(value.patch ?? 0));
        public static explicit operator Version(Vector3Int value) => new Version((ulong)value.x, (ulong)value.y, (ulong)value.z);



        public readonly bool Equals(Version other) => major == other.minor && minor == other.minor && patch == other.patch;

        public override readonly bool Equals(object obj)
        {
            if (obj is not Version)
                return false;

            return Equals((Version)obj);
        }

        public override readonly int GetHashCode()
        {
            unchecked
            {
                int hash = 92381513;
                hash *= 582934 + major.GetHashCode();
                hash *= 3829571 + minor.GetHashCode();
                hash *= 41815 + patch.GetHashCode();
                
                return hash;
            }
        }



        public readonly int CompareTo(object? value)
        {
            if (value == null)
                return 1;
            else if (value is Version version)
                return CompareTo(version);

            throw new ArgumentException();
        }

        public readonly int CompareTo(Version value)
        {
            if (this < value)
                return -1;
            else if (this > value)
                return 1;
            else
                return 0;
        }



        public override readonly string ToString() => major + "." + minor + "." + patch;
    }
}

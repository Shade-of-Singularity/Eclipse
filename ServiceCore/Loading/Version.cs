/// - - -    Copyright (c) 2025     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///         http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// ]]>

using System;
using System.Runtime.InteropServices;

namespace ServiceCore.Loading
{
    /// <summary>
    /// Describes any version.
    /// </summary>
    /// <remarks>
    /// Follows Semantic Versioning 2.0.0, without extensions: https://semver.org/
    /// </remarks>
    /// <param name="major">Major version of a thing.</param>
    /// <param name="minor">Minor version of a thing.</param>
    /// <param name="patch">Patch version of a thing.</param>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Version(ushort major, ushort minor, uint patch) : IComparable<Version>, IEquatable<Version>
    {
        /// <summary>
        /// Version with all zeros.
        /// </summary>
        public static readonly Version Zero = new(0, 0, 0);

        /// <summary>
        /// Major version, according to 2.0.0 version semantics.
        /// </summary>
        [FieldOffset(sizeof(ushort) + sizeof(uint))]
        public readonly ushort Major = major;
        /// <summary>
        /// Minor version, according to 2.0.0 version semantics.
        /// </summary>
        [FieldOffset(sizeof(uint))]
        public readonly ushort Minor = minor;
        /// <summary>
        /// Patch version, according to 2.0.0 version semantics.
        /// </summary>
        [FieldOffset(0)]
        public readonly uint Patch = patch;
        /// <summary>
        /// Entire version packed to one <see cref="ulong"/>.
        /// </summary>
        [FieldOffset(0)]
        public readonly ulong Packed;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Parses version of a format "MAJOR.MINOR.PATCH", following 2.0.0 version semantics, excluding extension: https://semver.org/
        /// </summary>
        /// <param name="raw">Raw version declaration.</param>
        /// <param name="version">Parsed version or <see cref="Zero"/> when returns <c>false</c>.</param>
        /// <returns><c>true</c> when <paramref name="raw"/> was parsed to <paramref name="version"/> successfully. <c>false</c> otherwise.</returns>
        public static bool TryParse(string raw, out Version version)
        {
            if (string.IsNullOrEmpty(raw))
            {
                version = Zero;
                return false;
            }

            ushort major;
            ushort minor;

            int splitA = raw.IndexOf('.');
            if (splitA == -1 || splitA + 1 == raw.Length)
            {
                ushort.TryParse(raw, out major);
                version = new(major, 0, 0);
                return true;
            }

            int splitB = raw.IndexOf('.', splitA + 1);
            if (splitB == -1)
            {
                ushort.TryParse(raw.AsSpan(0, splitA), out major);
                ushort.TryParse(raw.AsSpan(splitA + 1), out minor);

                version = new(major, minor, 0);
                return true;
            }

            if (splitB + 1 == raw.Length)
            {
                ushort.TryParse(raw.AsSpan(0, splitA), out major);
                ushort.TryParse(raw.AsSpan(splitA + 1, splitB - splitA - 1), out minor);

                version = new(major, minor, 0);
                return true;
            }

            ushort.TryParse(raw.AsSpan(0, splitA), out major);
            ushort.TryParse(raw.AsSpan(splitA + 1, splitB - splitA - 1), out minor);
            uint.TryParse(raw.AsSpan(splitB + 1), out uint patch);
            version = new(major, minor, patch);
            return true;
        }

        /// <summary>
        /// Parses version of a format "MAJOR.MINOR.PATCH", following 2.0.0 version semantics, excluding extension: https://semver.org/
        /// </summary>
        /// <param name="raw">Raw version declaration.</param>
        /// <param name="version">Parsed version or <see cref="Zero"/> when returns <c>false</c>.</param>
        /// <returns><c>true</c> when <paramref name="raw"/> was parsed to <paramref name="version"/> successfully. <c>false</c> otherwise.</returns>
        public static bool TryParse(ReadOnlySpan<char> raw, out Version version)
        {
            if (raw.IsEmpty)
            {
                version = Zero;
                return false;
            }

            ushort major;
            ushort minor;

            int splitA = raw.IndexOf('.');
            if (splitA == -1 || splitA + 1 == raw.Length)
            {
                ushort.TryParse(raw, out major);
                version = new(major, 0, 0);
                return true;
            }

            ReadOnlySpan<char> second = raw[(splitA + 1)..];
            int splitB = second.IndexOf('.');
            if (splitB == -1)
            {
                ushort.TryParse(raw[..splitA], out major);
                ushort.TryParse(second, out minor);

                version = new(major, minor, 0);
                return true;
            }

            if (splitB + 1 == raw.Length)
            {
                ushort.TryParse(raw[..splitA], out major);
                ushort.TryParse(raw.Slice(splitA + 1, splitB - splitA - 1), out minor);

                version = new(major, minor, 0);
                return true;
            }

            ushort.TryParse(raw[..splitA], out major);
            ushort.TryParse(raw.Slice(splitA + 1, splitB - splitA - 1), out minor);
            uint.TryParse(raw[(splitB + 1)..], out uint patch);
            version = new(major, minor, patch);
            return true;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        /// <inheritdoc/>
        public int CompareTo(Version other) => Packed.CompareTo(other.Packed);

        /// <inheritdoc/>
        public bool Equals(Version other) => Packed == other.Packed;

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Version version && Packed == version.Packed;

        /// <inheritdoc/>
        public override int GetHashCode() => Packed.GetHashCode();

        /// <inheritdoc cref="Equals(object)"/>
        public static bool operator ==(Version left, Version right) => left.Packed == right.Packed;

        /// <summary>
        /// Inverse of <see cref="Equals(object)"/>
        /// </summary>
        public static bool operator !=(Version left, Version right) => left.Packed != right.Packed;

        /// <summary>
        /// Checks if <paramref name="left"/> is smaller than <paramref name="right"/>.
        /// </summary>
        public static bool operator <(Version left, Version right) => left.Packed < right.Packed;

        /// <summary>
        /// Checks if <paramref name="left"/> is smaller than or equals to <paramref name="right"/>.
        /// </summary>
        public static bool operator <=(Version left, Version right) => left.Packed <= right.Packed;

        /// <summary>
        /// Checks if <paramref name="left"/> is larger than <paramref name="right"/>.
        /// </summary>
        public static bool operator >(Version left, Version right) => left.Packed > right.Packed;

        /// <summary>
        /// Checks if <paramref name="left"/> is larger than or equals to <paramref name="right"/>.
        /// </summary>
        public static bool operator >=(Version left, Version right) => left.Packed >= right.Packed;
    }
}

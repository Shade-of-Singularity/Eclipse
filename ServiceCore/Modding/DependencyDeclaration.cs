using System;
using System.Runtime.CompilerServices;

namespace ServiceCore.Modding
{
    /// <summary>
    /// Describes a specific dependency.
    /// </summary>
    public readonly struct DependencyDeclaration(string target, bool isAssembly, Version version, VersionDependencyType type) : IEquatable<DependencyDeclaration>
    {
        /// <summary>
        /// <para>When <see cref="isAssembly"/> is <c>true</c> - <see cref="target"/> is name of an assembly.</para>
        /// <para>Otherwise - <see cref="target"/> is <see cref="ModificationInfo.Identifier"/>.</para>
        /// </summary>
        public readonly string target = target;
        /// <summary>
        /// Whether <see cref="target"/> is assembly or <see cref="ModificationInfo"/>.
        /// </summary>
        public readonly bool isAssembly = isAssembly;
        /// <summary>
        /// <see cref="Version"/> on which this declaration depends on.
        /// </summary>
        public readonly Version version = version;
        /// <summary>
        /// <see cref="Version"/> dependency type on this declaration depends on.
        /// </summary>
        public readonly VersionDependencyType type = type;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="TryParse(ReadOnlySpan{char}, out DependencyDeclaration)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParse(string raw, out DependencyDeclaration declaration) => TryParse(raw.AsSpan(), out declaration);

        /// <summary>
        /// Parses <paramref name="raw"/> input string of a format "mod ?>= 4.0.0" to a full <see cref="DependencyDeclaration"/>.
        /// </summary>
        /// <remarks>
        /// Never sets <see cref="isAssembly"/> in <paramref name="declaration"/> to <c>true</c>.
        /// </remarks>
        /// <param name="raw">Raw input.</param>
        /// <param name="declaration">Parsed declaration or default value.</param>
        /// <returns><c>true</c> when parsed successfully. <c>false</c> when otherwise.</returns>
        public static bool TryParse(ReadOnlySpan<char> raw, out DependencyDeclaration declaration)
        {
            raw = raw.Trim();
            if (raw.IsEmpty)
            {
                declaration = default;
                return false;
            }

            // Case of "mod" kind of dependency.
            const char Separator = ' ';
            int index = raw.IndexOf(Separator);
            if (index == -1)
            {
                // Only contains name.
                declaration = new(raw.ToString(), isAssembly: false, Version.Zero, VersionDependencyType.Any);
                return true;
            }

            // Case of "mod ?>=" kind of dependency.
            ReadOnlySpan<char> name = raw[..index];
            raw = raw.Slice(index + 1, raw.Length - index - 1);
            index = raw.IndexOf(Separator);
            if (index == -1)
            {
                if (!VersionDependencyHelpers.TryParseType(raw, out VersionDependencyType dependency))
                {
                    declaration = default;
                    return false;
                }

                declaration = new(name.ToString(), isAssembly: false, Version.Zero, dependency);
                return true;
            }

            // Case of "mod ?>= 4.0.0" kind of dependency.
            ReadOnlySpan<char> type = raw[..index];
            raw = raw.Slice(index + 1, raw.Length - index - 1);
            index = raw.IndexOf(Separator);
            if (index == -1)
            {
                if (!VersionDependencyHelpers.TryParseType(type, out VersionDependencyType dependency) || !Version.TryParse(raw, out Version version))
                {
                    declaration = default;
                    return false;
                }

                declaration = new(name.ToString(), isAssembly: false, version, dependency);
                return true;
            }

            // More than 3 arguments are not allowed in declaration.
            declaration = default;
            return false;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override string ToString() => $"{target} {type.GetSymbol()} {version} ({nameof(isAssembly)}? {isAssembly})";

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is DependencyDeclaration declaration && Equals(declaration);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(target, isAssembly, version, type);

        /// <inheritdoc cref="Equals(object)"/>
        public bool Equals(DependencyDeclaration other)
        {
            return type == other.type && isAssembly == other.isAssembly
                && version == other.version
                && string.Equals(target, other.target, StringComparison.Ordinal);
        }
    }
}

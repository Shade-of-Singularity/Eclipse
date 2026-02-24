using System;
using System.Runtime.CompilerServices;

namespace ServiceCore.Modding
{
    /// <summary>
    /// Helpers for <see cref="VersionDependencyType"/> enum.
    /// </summary>
    public static class VersionDependencyHelpers
    {
        public const char SmallerToken = '<';
        public const char EqualToken = '=';
        public const char LargerToken = '>';

        public const char OptionalToken = '?';
        public const char IncompatibleToken = '!';

        /// <inheritdoc cref="VersionDependencyType.Any"/>
        public const string VersionDependencyTypeSymbol_Any = "";
        /// <inheritdoc cref="VersionDependencyType.Smaller"/>
        public const string VersionDependencyTypeSymbol_Smaller = "<";
        /// <inheritdoc cref="VersionDependencyType.SmallerOrEqual"/>
        public const string VersionDependencyTypeSymbol_SmallerOrEqual = "<=";
        /// <inheritdoc cref="VersionDependencyType.Equal"/>
        public const string VersionDependencyTypeSymbol_Equal = "=";
        /// <inheritdoc cref="VersionDependencyType.Equal"/>
        public const string VersionDependencyTypeSymbol_EqualAlt = "==";
        /// <inheritdoc cref="VersionDependencyType.LargerOrEqual"/>
        public const string VersionDependencyTypeSymbol_LargerOrEqual = ">=";
        /// <inheritdoc cref="VersionDependencyType.Larger"/>
        public const string VersionDependencyTypeSymbol_Larger = ">";

        /// <inheritdoc cref="VersionDependencyType.Optional"/>
        public const string VersionDependencyTypeSymbol_Optional = "?";
        /// <inheritdoc cref="VersionDependencyType.Incompatible"/>
        public const string VersionDependencyTypeSymbol_Incompatible = "!";

        /// <summary><inheritdoc cref="VersionDependencyType.Any"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Optional"/></remarks>
        public const string VersionDependencyTypeSymbol_OptionalAny = VersionDependencyTypeSymbol_Optional + VersionDependencyTypeSymbol_Any;
        /// <summary><inheritdoc cref="VersionDependencyType.Smaller"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Optional"/></remarks>
        public const string VersionDependencyTypeSymbol_OptionalSmaller = VersionDependencyTypeSymbol_Optional + VersionDependencyTypeSymbol_Smaller;
        /// <summary><inheritdoc cref="VersionDependencyType.SmallerOrEqual"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Optional"/></remarks>
        public const string VersionDependencyTypeSymbol_OptionalSmallerOrEqual = VersionDependencyTypeSymbol_Optional + VersionDependencyTypeSymbol_SmallerOrEqual;
        /// <summary><inheritdoc cref="VersionDependencyType.Equal"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Optional"/></remarks>
        public const string VersionDependencyTypeSymbol_OptionalEqual = VersionDependencyTypeSymbol_Optional + VersionDependencyTypeSymbol_Equal;
        /// <summary><inheritdoc cref="VersionDependencyType.Equal"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Optional"/></remarks>
        public const string VersionDependencyTypeSymbol_OptionalEqualAlt = VersionDependencyTypeSymbol_Optional + VersionDependencyTypeSymbol_EqualAlt;
        /// <summary><inheritdoc cref="VersionDependencyType.LargerOrEqual"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Optional"/></remarks>
        public const string VersionDependencyTypeSymbol_OptionalLargerOrEqual = VersionDependencyTypeSymbol_Optional + VersionDependencyTypeSymbol_LargerOrEqual;
        /// <summary><inheritdoc cref="VersionDependencyType.Larger"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Optional"/></remarks>
        public const string VersionDependencyTypeSymbol_OptionalLarger = VersionDependencyTypeSymbol_Optional + VersionDependencyTypeSymbol_Larger;

        /// <summary><inheritdoc cref="VersionDependencyType.Any"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Incompatible"/></remarks>
        public const string VersionDependencyTypeSymbol_IncompatibleAny = VersionDependencyTypeSymbol_Incompatible + VersionDependencyTypeSymbol_Any;
        /// <summary><inheritdoc cref="VersionDependencyType.Smaller"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Incompatible"/></remarks>
        public const string VersionDependencyTypeSymbol_IncompatibleSmaller = VersionDependencyTypeSymbol_Incompatible + VersionDependencyTypeSymbol_Smaller;
        /// <summary><inheritdoc cref="VersionDependencyType.SmallerOrEqual"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Incompatible"/></remarks>
        public const string VersionDependencyTypeSymbol_IncompatibleSmallerOrEqual = VersionDependencyTypeSymbol_Incompatible + VersionDependencyTypeSymbol_SmallerOrEqual;
        /// <summary><inheritdoc cref="VersionDependencyType.Equal"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Incompatible"/></remarks>
        public const string VersionDependencyTypeSymbol_IncompatibleEqual = VersionDependencyTypeSymbol_Incompatible + VersionDependencyTypeSymbol_Equal;
        /// <summary><inheritdoc cref="VersionDependencyType.Equal"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Incompatible"/></remarks>
        public const string VersionDependencyTypeSymbol_IncompatibleEqualAlt = VersionDependencyTypeSymbol_Incompatible + VersionDependencyTypeSymbol_EqualAlt;
        /// <summary><inheritdoc cref="VersionDependencyType.LargerOrEqual"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Incompatible"/></remarks>
        public const string VersionDependencyTypeSymbol_IncompatibleLargerOrEqual = VersionDependencyTypeSymbol_Incompatible + VersionDependencyTypeSymbol_LargerOrEqual;
        /// <summary><inheritdoc cref="VersionDependencyType.Larger"/></summary>
        /// <remarks><inheritdoc cref="VersionDependencyType.Incompatible"/></remarks>
        public const string VersionDependencyTypeSymbol_IncompatibleLarger = VersionDependencyTypeSymbol_Incompatible + VersionDependencyTypeSymbol_Larger;

        /// <summary>
        /// Max length an "?!>=" token can take.
        /// </summary>
        private const int MaxDeclarationLength = 4;

        /// <summary>
        /// Retrieves special symbol used to declare a dependency type (e.g. "!", "?", ">", and even "!>=", "?>")
        /// </summary>
        /// <param name="type">Type to version dependency type.</param>
        /// <returns>Special symbol used to declare this dependency <paramref name="type"/>.</returns>
        /// <exception cref="SwitchExpressionException">
        /// Throws when you mess-up comparison declaration (within mask <see cref="VersionDependencyType.TypeMask"/>).
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Throws when you combine "!" and "?".
        /// </exception>
        public static string GetSymbol(this VersionDependencyType type) => (type & VersionDependencyType.ExclusionMask) switch
        {
            VersionDependencyType.Any => (type & VersionDependencyType.TypeMask) switch
            {
                VersionDependencyType.Any => VersionDependencyTypeSymbol_Any,
                VersionDependencyType.Smaller => VersionDependencyTypeSymbol_Smaller,
                VersionDependencyType.SmallerOrEqual => VersionDependencyTypeSymbol_SmallerOrEqual,
                VersionDependencyType.Equal => VersionDependencyTypeSymbol_Equal,
                VersionDependencyType.LargerOrEqual => VersionDependencyTypeSymbol_LargerOrEqual,
                VersionDependencyType.Larger => VersionDependencyTypeSymbol_Larger,
                _ => throw new SwitchExpressionException(type),
            },

            VersionDependencyType.Optional => (type & VersionDependencyType.TypeMask) switch
            {
                VersionDependencyType.Any => VersionDependencyTypeSymbol_OptionalAny,
                VersionDependencyType.Smaller => VersionDependencyTypeSymbol_OptionalSmaller,
                VersionDependencyType.SmallerOrEqual => VersionDependencyTypeSymbol_OptionalSmallerOrEqual,
                VersionDependencyType.Equal => VersionDependencyTypeSymbol_OptionalEqual,
                VersionDependencyType.LargerOrEqual => VersionDependencyTypeSymbol_OptionalLargerOrEqual,
                VersionDependencyType.Larger => VersionDependencyTypeSymbol_OptionalLarger,
                _ => throw new SwitchExpressionException(type),
            },

            VersionDependencyType.Incompatible => (type & VersionDependencyType.TypeMask) switch
            {
                VersionDependencyType.Any => VersionDependencyTypeSymbol_IncompatibleAny,
                VersionDependencyType.Smaller => VersionDependencyTypeSymbol_IncompatibleSmaller,
                VersionDependencyType.SmallerOrEqual => VersionDependencyTypeSymbol_IncompatibleSmallerOrEqual,
                VersionDependencyType.Equal => VersionDependencyTypeSymbol_IncompatibleEqual,
                VersionDependencyType.LargerOrEqual => VersionDependencyTypeSymbol_IncompatibleLargerOrEqual,
                VersionDependencyType.Larger => VersionDependencyTypeSymbol_IncompatibleLarger,
                _ => throw new SwitchExpressionException(type),
            },

            // Indicates both Optional and Incompatible masks (for whatever reason)
            _ => throw new NotSupportedException($"{Engine.LogPrefix} Cannot use '!' and '?' tokens in a dependency declaration."),
        };

        /// <inheritdoc cref="TryGetType(ReadOnlySpan{char}, out VersionDependencyType)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetType(string raw, out VersionDependencyType type) => TryGetType(raw.AsSpan(), out type);

        /// <summary>
        /// Tries to retrieve <see cref="VersionDependencyType"/> from a raw declaration in text (e.g. "?>=", "!==")
        /// </summary>
        /// <param name="raw">Raw declaration <b>without</b> anything extra: spaces (on both sides), other symbols, etc.</param>
        /// <param name="type">Retrieved <see cref="VersionDependencyType"/>.</param>
        /// <returns><c>true</c> when type was successfully retrieved and <paramref name="type"/> was provided. <c>false</c> otherwise.</returns>
        public static bool TryGetType(ReadOnlySpan<char> raw, out VersionDependencyType type)
        {
            /* Learned lessons (GPT):
             * Early on, you were optimizing:
             * - switch ordering
             * - loop removal
             * - instruction counts
             * - bitmask tricks
             * But the real improvement happened when you aligned the code with the grammar:
             * [!][?] ( < | <= | = | == | > | >= )
             * 
             * Once the structure of the code mirrored the structure of the language:
             * - correctness became obvious
             * - edge cases disappeared
             * - micro-optimizations became irrelevant
             * 
             * General Rule #1:
             * Make your code structurally reflect the grammar or domain rules.
             * When structure matches semantics, performance and correctness both improve naturally.
             * 
             * - - -
             * Smaller = 001
             * Equal   = 010
             * Larger  = 100
             * This isn't micro-optimization.
             * This is semantic compression.
             * 
             * General Rule #2
             * Use bitwise composition when the domain represents independent dimensions.
             * 
             * General Rule #3
             * Optimize the hot path, not the initialization path.
             */

            type = VersionDependencyType.Any;

            if (raw.IsEmpty || raw.Length > MaxDeclarationLength)
                return false;

            int src = 0;
            int length = raw.Length;

            // Modifiers:
            if (raw[src] == IncompatibleToken)
            {
                type |= VersionDependencyType.Incompatible;
                src++;

                if (src == length) return true;
            }

            if (raw[src] == OptionalToken)
            {
                type |= VersionDependencyType.Optional;
                src++;
            }

            // Types:
            if (src == length) return true;
            switch (raw[src])
            {
                case SmallerToken: type |= VersionDependencyType.Smaller; break;
                case EqualToken: type |= VersionDependencyType.Equal; break;
                case LargerToken: type |= VersionDependencyType.Larger; break;

                // Declaring more than one '?' or '!' is not allowed.
                case OptionalToken:
                case IncompatibleToken: return false;

                default: return false;
            }

            if (++src == length) return true;
            switch (raw[src])
            {
                case SmallerToken:
                    if ((type & VersionDependencyType.LargerOrEqual) != VersionDependencyType.Any)
                    {
                        // '<' declaration after '=' is not allowed.
                        // '<' after '>' is also not allowed.
                        type = VersionDependencyType.Any;
                        return false;
                    }

                    type |= VersionDependencyType.Smaller;
                    break;

                case EqualToken: type |= VersionDependencyType.Equal; break;
                case LargerToken:
                    if ((type & VersionDependencyType.SmallerOrEqual) != VersionDependencyType.Any)
                    {
                        // '>' declaration after '=' is not allowed.
                        // '>' after '<' is also not allowed.
                        type = VersionDependencyType.Any;
                        return false;
                    }

                    type |= VersionDependencyType.Larger;
                    break;

                // Declaring more than one '?' or '!' is not allowed.
                case OptionalToken:
                case IncompatibleToken: return false;

                default: type = VersionDependencyType.Any; return false;
            }

            return ++src == length;
        }
    }
}

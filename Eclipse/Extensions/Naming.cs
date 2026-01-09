using System.Runtime.CompilerServices;

namespace Eclipse.Extensions
{
    /// <summary>
    /// Gives standardized methods for generating string IDs.
    /// </summary>
    /// Making constants would be better for performance, but it will force all modders to learn one naming standard, adjust if needed.
    /// This is hard to make, so we avoid that entirely by introducing helper methods.
    /// We might change this later if needed.
    public static class Naming
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Used as separator between mod name and an identifier in localization keys.
        /// </summary>
        public const string Separator = ".";

        /// <summary>
        /// Used as prefix in special parameters, like so:
        /// <![CDATA[@eclipse.field-of-view]]>
        /// </summary>
        public const string SettingPrefix = "@";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Constructs unique parameter ID by putting standard <see cref="Separator"/> between <paramref name="mod"/> and <paramref name="id"/>.
        /// </summary>
        /// <remarks>
        /// You can just use constants if you want though.
        /// </remarks>
        public static string Get(string mod, string id)
        {
            if (string.IsNullOrEmpty(mod))
            {
                return id;
            }
            else
            {
                return mod + Separator + id;
            }
        }

        /// <summary>
        /// Construct unique parameter ID for special parameters by putting <see cref="SettingPrefix"/> before <paramref name="id"/>.
        /// </summary>
        /// <remarks>
        /// Doesn't specify a mod! It's not recommended usually, because it is not intuitive.
        /// </remarks>
        /// <param name="id">Identifier of a thing.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetPrefix(string id) => SettingPrefix + id;

        /// <summary>
        /// Construct unique parameter ID for special parameters by putting <see cref="SettingPrefix"/> before <paramref name="mod"/> and <paramref name="id"/>.
        /// 
        /// </summary>
        /// <remarks>
        /// <paramref name="mod"/> and <paramref name="id"/> are combined in the same way as <see cref="Get(string, string)"/> combines them.
        /// </remarks>
        /// <param name="mod">Mod name of identifier.</param>
        /// <param name="id">Identifier of a thing.</param>
        public static string GetPrefix(string mod, string id)
        {
            if (string.IsNullOrEmpty(mod))
            {
                return SettingPrefix + id;
            }
            else
            {
                return SettingPrefix + mod + Separator + id;
            }
        }
    }
}

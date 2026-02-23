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

using System.Runtime.CompilerServices;

namespace ServiceCore.Extensions
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
        /// <param name="mod">Modification name of identifier.</param>
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

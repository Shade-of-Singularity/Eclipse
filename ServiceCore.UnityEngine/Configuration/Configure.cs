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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ServiceCore.Configuration
{
    /// <summary>
    /// Contains extension methods for easier configuration of different fields.
    /// </summary>
    /// <remarks>
    /// Often uses <see cref="DefaultConfigurationService"/> under the hood, but only if necessary.
    /// </remarks>
    public static partial class Configure
    {
        #region Set/Get extensions

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref bool field, bool value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref byte field, byte value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref sbyte field, sbyte value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref short field, short value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref ushort field, ushort value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref char field, char value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref int field, int value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref uint field, uint value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref long field, long value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref ulong field, ulong value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref float field, float value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref double field, double value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref decimal field, decimal value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref string field, string value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(ref object field, object value)
        {
            if (field == value) return false;
            else
            {
                field = value;
                return true;
            }
        }

        /// <summary>
        /// Sets value in a <paramref Identifier="field"/> to <paramref Identifier="value"/>.
        /// </summary>
        /// <param Identifier="field">Field to modify.</param>
        /// <param Identifier="value">Value to apply.</param>
        /// <returns><c>true</c> if <paramref Identifier="field"/> was modified. Otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue<T>(ref T field, T value) where T : IEquatable<T>
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            else
            {
                field = value;
                return true;
            }
        }

        #endregion

        #region Custom 'SetValue/GetValue' Fast-access

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, bool)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref bool field, bool value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, byte)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref byte field, byte value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, sbyte)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref sbyte field, sbyte value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, short)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref short field, short value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, ushort)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref ushort field, ushort value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, char)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref char field, char value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, int)"/> method.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if <paramref Identifier="field"/> changed. <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref int field, int value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, uint)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref uint field, uint value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, long)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref long field, long value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, ulong)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref ulong field, ulong value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, float)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref float field, float value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, double)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>aramref Identifier="field"/> changed. <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref double field, double value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, decimal)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref decimal field, decimal value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        /// <summary>
        /// (Supports <see cref="DefaultConfigurationService.Revert"/> by not serializing the data)
        /// Allows you to set values to raw parameters which require highest performance (i.e. <see cref="Specs.Cache.L1Cache"/>).
        /// </summary>
        /// <remarks>
        /// Sets custom value using <see cref="DefaultConfigurationService.Set(string, string)"/> method.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if <paramref Identifier="field"/> changed. <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref string field, string value)
        {
            if (field == value) return false;
            IConfigurationService.Instance.Set(id, field = value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out bool value, bool def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out byte value, byte def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out sbyte value, sbyte def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out short value, short def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out ushort value, ushort def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out char value, char def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out int value, int def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out uint value, uint def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out long value, long def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out ulong value, ulong def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out float value, float def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out double value, double def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out decimal value, decimal def = default) => IConfigurationService.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out string value, string def = "") => IConfigurationService.Instance.Get(id, out value, def);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBool(string id, bool def = default)
        {
            IConfigurationService.Instance.Get(id, out bool value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte(string id, byte def = default)
        {
            IConfigurationService.Instance.Get(id, out byte value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte GetSByte(string id, sbyte def = default)
        {
            IConfigurationService.Instance.Get(id, out sbyte value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetShort(string id, short def = default)
        {
            IConfigurationService.Instance.Get(id, out short value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetUShort(string id, ushort def = default)
        {
            IConfigurationService.Instance.Get(id, out ushort value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char GetChar(string id, char def = default)
        {
            IConfigurationService.Instance.Get(id, out char value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInt(string id, int def = default)
        {
            IConfigurationService.Instance.Get(id, out int value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetUInt(string id, uint def = default)
        {
            IConfigurationService.Instance.Get(id, out uint value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetLong(string id, long def = default)
        {
            IConfigurationService.Instance.Get(id, out long value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetULong(string id, ulong def = default)
        {
            IConfigurationService.Instance.Get(id, out ulong value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetFloat(string id, float def = default)
        {
            IConfigurationService.Instance.Get(id, out float value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetDouble(string id, double def = default)
        {
            IConfigurationService.Instance.Get(id, out double value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal GetDecimal(string id, decimal def = default)
        {
            IConfigurationService.Instance.Get(id, out decimal value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(string id, string def = "")
        {
            IConfigurationService.Instance.Get(id, out string value, def);
            return value;
        }

        #endregion
    }
}

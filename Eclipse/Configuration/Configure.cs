using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Contains extension methods for easier configuration of different fields.
    /// </summary>
    /// <remarks>
    /// Often uses <see cref="ConfigurationService"/> under the hood, but only if necessary.
    /// </remarks>
    public static partial class Configure
    {
        #region Set/Get extensions

        /// <inheritdoc cref="SetValue{T}(ref T, T)"/>
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
        /// Sets value in a <paramref name="field"/> to <paramref name="value"/>.
        /// </summary>
        /// <param name="field">Field to modify.</param>
        /// <param name="value">Value to apply.</param>
        /// <returns><c>true</c> if <paramref name="field"/> was modified. Otherwise <c>false</c>.</returns>
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
        /// Sets custom value using <see cref="ConfigurationService.Set(string, bool)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref bool field, bool value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, byte)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref byte field, byte value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, sbyte)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref sbyte field, sbyte value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, short)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref short field, short value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, ushort)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref ushort field, ushort value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, char)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref char field, char value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, int)"/> method.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if <paramref name="field"/> changed. <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref int field, int value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, uint)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref uint field, uint value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, long)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref long field, long value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, ulong)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref ulong field, ulong value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, float)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref float field, float value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, double)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>aramref name="field"/> changed. <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref double field, double value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary><inheritdoc cref="SetValue(string, ref string, string)"/></summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, decimal)"/> method.
        /// </remarks>
        /// <returns><inheritdoc cref="SetValue(string, ref string, string)"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref decimal field, decimal value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        /// <summary>
        /// (Supports <see cref="ConfigurationService.Revert"/> by not serializing the data)
        /// Allows you to set values to raw parameters which require highest performance (i.e. <see cref="Specs.Cache.L1Cache"/>).
        /// </summary>
        /// <remarks>
        /// Sets custom value using <see cref="ConfigurationService.Set(string, string)"/> method.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if <paramref name="field"/> changed. <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetValue(string id, ref string field, string value)
        {
            if (field == value) return false;
            EngineService<ConfigurationService>.Instance.Set(id, field = value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out bool value, bool def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out byte value, byte def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out sbyte value, sbyte def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out short value, short def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out ushort value, ushort def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out char value, char def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out int value, int def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out uint value, uint def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out long value, long def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out ulong value, ulong def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out float value, float def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out double value, double def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out decimal value, decimal def = default) => EngineService<ConfigurationService>.Instance.Get(id, out value, def);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetValue(string id, out string value, string def = "") => EngineService<ConfigurationService>.Instance.Get(id, out value, def);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBool(string id, bool def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out bool value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte(string id, byte def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out byte value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte GetSByte(string id, sbyte def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out sbyte value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetShort(string id, short def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out short value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetUShort(string id, ushort def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out ushort value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char GetChar(string id, char def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out char value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInt(string id, int def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out int value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetUInt(string id, uint def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out uint value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetLong(string id, long def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out long value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetULong(string id, ulong def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out ulong value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetFloat(string id, float def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out float value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetDouble(string id, double def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out double value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal GetDecimal(string id, decimal def = default)
        {
            EngineService<ConfigurationService>.Instance.Get(id, out decimal value, def);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(string id, string def = "")
        {
            EngineService<ConfigurationService>.Instance.Get(id, out string value, def);
            return value;
        }

        #endregion
    }
}

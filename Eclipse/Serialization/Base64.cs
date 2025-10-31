namespace Eclipse.Serialization
{
    /// <summary>
    /// Provides ways to compact any numbers into readable chars - compact Base256.
    /// </summary>
    /// <remarks>
    /// A logical continuation of <see cref="Hex"/>, which is Base16.
    /// </remarks>
    public static class Base64
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly char[] toBase = new char[] {
            // 0 - 9:
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',

            // 11 - 60:
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
            'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
            'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',
            'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',

            // 61 - 64.
            'Y', 'Z', '-', '+',
        };

        // TODO: Test if everything works as expected.
        private static readonly ushort[] toUInt = new ushort[] {
            // 0 - 39:
            // Mostly non-readable characters.
            // -1  -2  -3  -4  -5  -6  -7  -8  -9
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,

            // 40 - 79:
            //
            // -1  -2  -3  -4  -5  -6  -7  -8  -9
            0,  0,  0, 63,  0, 62,  0,  0,  0,  1,
            2,  3,  4,  5,  6,  7,  8,  9,  0,  0,
            0,  0,  0,  0,  0, 36, 37, 38, 39, 40,
           41, 42, 43, 44, 45, 46, 47, 48, 49, 50,
            
            // 80 - 119:
            //
            // -1  -2  -3  -4  -5  -6  -7  -8  -9
           51, 52, 53, 54, 55, 56, 57, 58, 59, 60,
            0,  0,  0,  0,  0,  0,  0, 10, 11, 12,
           13, 14, 15, 16, 17, 18, 19, 20, 21, 22,
           23, 24, 25, 26, 27, 28, 29, 30, 31, 32,
            
            // 120 - 159:
            //
            // -1  -2  -3  -4  -5  -6  -7  -8  -9
           33, 34, 35,
        };




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               byte Methods
        /// .                     Note: Order is inverted (from 2 to 1) to make output more readable.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static string SByte(sbyte input) => Byte(unchecked((byte)input));
        public static sbyte ToSByte(string str) => unchecked((sbyte)ToByte(str));
        public static sbyte ToSByte(string str, int at) => unchecked((sbyte)ToByte(str, at));
        public static sbyte ToSByte(char n2, char n1) => unchecked((sbyte)ToByte(n2, n1));
        public static sbyte ToSByte(int n2, int n1) => unchecked((sbyte)ToByte(n2, n1));
        public static string Byte(byte input)
        {
            uint code = input & 0b11_111111u;
            uint value1 = code & 0b00_111111u;
            uint value2 = code & 0b11_000000u;

            value2 >>= 6;

            return new(stackalloc char[2]
            {
                toBase[value2], toBase[value1]
            });
        }

        public static byte ToByte(string str)
        {
            uint value1 = toUInt[str[1]];
            uint value2 = toUInt[str[0]];

            value2 <<= 6;

            return (byte)(value1 | value2);
        }

        public static byte ToByte(string str, int at)
        {
            uint value1 = toUInt[str[at + 1]];
            uint value2 = toUInt[str[at]];

            value2 <<= 6;

            return (byte)(value1 | value2);
        }

        public static byte ToByte(char n2, char n1)
        {
            uint value1 = toUInt[n2];
            uint value2 = toUInt[n1];

            value2 <<= 6;

            return (byte)(value1 | value2);
        }

        // Same as the one above, but with integers instead of chars.
        public static byte ToByte(int n2, int n1)
        {
            uint value1 = toUInt[n2];
            uint value2 = toUInt[n1];

            value2 <<= 6;

            return (byte)(value1 | value2);
        }





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               ushort Methods
        /// .                     Note: Order is inverted (from 4 to 1) to make output more readable.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static string Short(short input) => UShort(unchecked((ushort)input));
        public static short ToShort(string str) => unchecked((short)ToUShort(str));
        public static short ToShort(string str, int at) => unchecked((short)ToUShort(str, at));
        public static short ToShort(char n3, char n2, char n1) => unchecked((short)ToUShort(n3, n2, n1));
        public static short ToShort(int n3, int n2, int n1) => unchecked((short)ToUShort(n3, n2, n1));
        public static string UShort(ushort input)
        {
            uint code = input & 0b111111_111111_111111u;
            uint value1 = code & 0b000000_000000_111111u;
            uint value2 = code & 0b000000_111111_000000u;
            uint value3 = code & 0b111111_000000_000000u;

            value2 >>= 6;
            value3 >>= 12;

            return $"{toBase[value3]}{toBase[value2]}{toBase[value1]}";
        }

        public static ushort ToUShort(string str)
        {
            uint value1 = toUInt[str[2]];
            uint value2 = toUInt[str[1]];
            uint value3 = toUInt[str[0]];

            value2 <<= 6;
            value3 <<= 12;

            return (ushort)(value1 | value2 | value3);
        }

        public static ushort ToUShort(string str, int at)
        {
            uint value1 = toUInt[str[at + 2]];
            uint value2 = toUInt[str[at + 1]];
            uint value3 = toUInt[str[at]];

            value2 <<= 6;
            value3 <<= 12;

            return (ushort)(value1 | value2 | value3);
        }

        public static ushort ToUShort(char n3, char n2, char n1)
        {
            uint value1 = toUInt[n3];
            uint value2 = toUInt[n2];
            uint value3 = toUInt[n1];

            value2 <<= 6;
            value3 <<= 12;

            return (ushort)(value1 | value2 | value3);
        }

        // Same as the one above, but with integers instead of chars.
        public static ushort ToUShort(int n3, int n2, int n1)
        {
            uint value1 = toUInt[n3];
            uint value2 = toUInt[n2];
            uint value3 = toUInt[n1];

            value2 <<= 6;
            value3 <<= 12;

            return (ushort)(value1 | value2 | value3);
        }





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               uint Formatting
        /// .                     Note: Order is inverted (from 8 to 1) to make output more readable.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static string Int(int input) => UInt(unchecked((uint)input));
        public static int ToInt(string str) => unchecked((int)ToUInt(str));
        public static int ToInt(string str, int at) => unchecked((int)ToUInt(str, at));
        public static int ToInt(char n6, char n5, char n4, char n3, char n2, char n1) => unchecked((int)ToUInt(n6, n5, n4, n3, n2, n1));
        public static int ToInt(int n6, int n5, int n4, int n3, int n2, int n1) => unchecked((int)ToUInt(n6, n5, n4, n3, n2, n1));
        public static string UInt(uint input)
        {
            uint code = input & 0b11_111111_111111_111111_111111_111111u;
            uint value1 = code & 0b00_000000_000000_000000_000000_111111u;
            uint value2 = code & 0b00_000000_000000_000000_111111_000000u;
            uint value3 = code & 0b00_000000_000000_111111_000000_000000u;
            uint value4 = code & 0b00_000000_111111_000000_000000_000000u;
            uint value5 = code & 0b00_111111_000000_000000_000000_000000u;
            uint value6 = code & 0b11_000000_000000_000000_000000_000000u;

            value2 >>= 6;
            value3 >>= 12;
            value4 >>= 18;
            value5 >>= 24;
            value6 >>= 30;

            return new(stackalloc char[6]
            {
                toBase[value6], toBase[value5], toBase[value4], toBase[value3], toBase[value2], toBase[value1]
            });
        }

        /// <inheritdoc cref="ToUInt(string, int)"/>
        public static uint ToUInt(string str)
        {
            uint value1 = toUInt[str[5]];
            uint value2 = toUInt[str[4]];
            uint value3 = toUInt[str[3]];
            uint value4 = toUInt[str[2]];
            uint value5 = toUInt[str[1]];
            uint value6 = toUInt[str[0]];

            value2 <<= 6;
            value3 <<= 12;
            value4 <<= 18;
            value5 <<= 24;
            value6 <<= 30;

            return value1 | value2 | value3 | value4 | value5 | value6;
        }

        /// <summary>
        /// Decodes a hex string into a <see cref="uint"/>.
        /// </summary>
        /// <param name="at">Index from which to start.</param>
        public static uint ToUInt(string str, int at)
        {
            uint value1 = toUInt[str[at + 5]];
            uint value2 = toUInt[str[at + 4]];
            uint value3 = toUInt[str[at + 3]];
            uint value4 = toUInt[str[at + 2]];
            uint value5 = toUInt[str[at + 1]];
            uint value6 = toUInt[str[at]];

            value2 <<= 6;
            value3 <<= 12;
            value4 <<= 18;
            value5 <<= 24;
            value6 <<= 30;

            return value1 | value2 | value3 | value4 | value5 | value6;
        }

        public static uint ToUInt(char hex6, char hex5, char hex4,
                                  char hex3, char hex2, char hex1)
        {
            uint value1 = toUInt[hex6];
            uint value2 = toUInt[hex5];
            uint value3 = toUInt[hex4];
            uint value4 = toUInt[hex3];
            uint value5 = toUInt[hex2];
            uint value6 = toUInt[hex1];

            value2 <<= 6;
            value3 <<= 12;
            value4 <<= 18;
            value5 <<= 24;
            value6 <<= 30;

            return value1 | value2 | value3 | value4 | value5 | value6;
        }

        // Same as the one above, but with integers instead of chars.
        public static uint ToUInt(int hex6, int hex5, int hex4,
                                  int hex3, int hex2, int hex1)
        {
            uint value1 = toUInt[hex6];
            uint value2 = toUInt[hex5];
            uint value3 = toUInt[hex4];
            uint value4 = toUInt[hex3];
            uint value5 = toUInt[hex2];
            uint value6 = toUInt[hex1];

            value2 <<= 6;
            value3 <<= 12;
            value4 <<= 18;
            value5 <<= 24;
            value6 <<= 30;

            return value1 | value2 | value3 | value4 | value5 | value6;
        }





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              ulong Formatting
        /// .                     Note: Order is inverted (from 11 to 1) to make output more readable.
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static string Long(long input) => ULong(unchecked((ulong)input));
        public static long ToLong(string str) => unchecked((long)ToULong(str));
        public static long ToLong(string str, int at) => unchecked((long)ToULong(str, at));
        public static long ToLong(char n11, char n10, char n9, char n8, char n7, char n6, char n5, char n4, char n3, char n2, char n1)
        {
            return unchecked((long)ToULong(n11, n10, n9, n8, n7, n6, n5, n4, n3, n2, n1));
        }

        public static long ToLong(int n11, int n10, int n9, int n8, int n7, int n6, int n5, int n4, int n3, int n2, int n1)
        {
            return unchecked((long)ToULong(n11, n10, n9, n8, n7, n6, n5, n4, n3, n2, n1));
        }

        public static string ULong(ulong input)
        {
            ulong code = input & 0b1111_111111_111111_111111_111111_111111_111111_111111_111111_111111_111111uL;
            ulong value1 = code & 0b0000_000000_000000_000000_000000_000000_000000_000000_000000_000000_111111uL;
            ulong value2 = code & 0b0000_000000_000000_000000_000000_000000_000000_000000_000000_111111_000000uL;
            ulong value3 = code & 0b0000_000000_000000_000000_000000_000000_000000_000000_111111_000000_000000uL;
            ulong value4 = code & 0b0000_000000_000000_000000_000000_000000_000000_111111_000000_000000_000000uL;
            ulong value5 = code & 0b0000_000000_000000_000000_000000_000000_111111_000000_000000_000000_000000uL;
            ulong value6 = code & 0b0000_000000_000000_000000_000000_111111_000000_000000_000000_000000_000000uL;
            ulong value7 = code & 0b0000_000000_000000_000000_111111_000000_000000_000000_000000_000000_000000uL;
            ulong value8 = code & 0b0000_000000_000000_111111_000000_000000_000000_000000_000000_000000_000000uL;
            ulong value9 = code & 0b0000_000000_111111_000000_000000_000000_000000_000000_000000_000000_000000uL;
            ulong value10 = code & 0b0000_111111_000000_000000_000000_000000_000000_000000_000000_000000_000000uL;
            ulong value11 = code & 0b1111_000000_000000_000000_000000_000000_000000_000000_000000_000000_000000uL;

            value2 >>= 6;
            value3 >>= 12;
            value4 >>= 18;
            value5 >>= 24;
            value6 >>= 30;
            value7 >>= 36;
            value8 >>= 42;
            value9 >>= 48;
            value10 >>= 54;
            value11 >>= 60;

            return new(stackalloc char[11]
            {
                toBase[value11], toBase[value10], toBase[value9], toBase[value8], toBase[value7], toBase[value6],
                toBase[value5], toBase[value4], toBase[value3], toBase[value2], toBase[value1],
            });
        }

        public static ulong ToULong(string str)
        {
            ulong value1 = toUInt[str[10]];
            ulong value2 = toUInt[str[9]];
            ulong value3 = toUInt[str[8]];
            ulong value4 = toUInt[str[7]];
            ulong value5 = toUInt[str[6]];
            ulong value6 = toUInt[str[5]];
            ulong value7 = toUInt[str[4]];
            ulong value8 = toUInt[str[3]];
            ulong value9 = toUInt[str[2]];
            ulong value10 = toUInt[str[1]];
            ulong value11 = toUInt[str[0]];

            value2 <<= 6;
            value3 <<= 12;
            value4 <<= 18;
            value5 <<= 24;
            value6 <<= 30;
            value7 <<= 36;
            value8 <<= 42;
            value9 <<= 48;
            value10 <<= 54;
            value11 <<= 60;

            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8 | value9 | value10 | value11;
        }

        public static ulong ToULong(string str, int at)
        {
            uint value1 = toUInt[str[at + 10]];
            uint value2 = toUInt[str[at + 9]];
            uint value3 = toUInt[str[at + 8]];
            uint value4 = toUInt[str[at + 7]];
            uint value5 = toUInt[str[at + 6]];
            uint value6 = toUInt[str[at + 5]];
            uint value7 = toUInt[str[at + 4]];
            uint value8 = toUInt[str[at + 3]];
            uint value9 = toUInt[str[at + 2]];
            uint value10 = toUInt[str[at + 1]];
            uint value11 = toUInt[str[at]];

            value2 >>= 6;
            value3 >>= 12;
            value4 >>= 18;
            value5 >>= 24;
            value6 >>= 30;
            value7 >>= 36;
            value8 >>= 42;
            value9 >>= 48;
            value10 >>= 54;
            value11 >>= 60;

            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8 | value9 | value10 | value11;
        }

        public static ulong ToULong(char n11, char n10, char n9, char n8, char n7, char n6, char n5, char n4, char n3, char n2, char n1)
        {
            uint value1 = toUInt[n11];
            uint value2 = toUInt[n10];
            uint value3 = toUInt[n9];
            uint value4 = toUInt[n8];
            uint value5 = toUInt[n7];
            uint value6 = toUInt[n6];
            uint value7 = toUInt[n5];
            uint value8 = toUInt[n4];
            uint value9 = toUInt[n3];
            uint value10 = toUInt[n2];
            uint value11 = toUInt[n1];

            value2 >>= 6;
            value3 >>= 12;
            value4 >>= 18;
            value5 >>= 24;
            value6 >>= 30;
            value7 >>= 36;
            value8 >>= 42;
            value9 >>= 48;
            value10 >>= 54;
            value11 >>= 60;

            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8 | value9 | value10 | value11;
        }

        // Same as the one above, but with integers instead of chars.
        public static ulong ToULong(int n11, int n10, int n9, int n8, int n7, int n6, int n5, int n4, int n3, int n2, int n1)
        {
            uint value1 = toUInt[n11];
            uint value2 = toUInt[n10];
            uint value3 = toUInt[n9];
            uint value4 = toUInt[n8];
            uint value5 = toUInt[n7];
            uint value6 = toUInt[n6];
            uint value7 = toUInt[n5];
            uint value8 = toUInt[n4];
            uint value9 = toUInt[n3];
            uint value10 = toUInt[n2];
            uint value11 = toUInt[n1];

            value2 >>= 6;
            value3 >>= 12;
            value4 >>= 18;
            value5 >>= 24;
            value6 >>= 30;
            value7 >>= 36;
            value8 >>= 42;
            value9 >>= 48;
            value10 >>= 54;
            value11 >>= 60;

            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8 | value9 | value10 | value11;
        }
    }
}

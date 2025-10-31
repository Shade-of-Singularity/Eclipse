//namespace StackControl.Serialization
//{
//    public static class Hex
//    {
//        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
//        /// .
//        /// .                                               Private Fields
//        /// .
//        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
//        private static readonly char[] toHex = new char[] {
//            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
//            'A', 'B', 'C', 'D', 'E', 'F'
//        };

//        // TODO: Test if everything works as expected.
//        private static readonly uint[] toUInt = new uint[] {
//            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
//            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
//            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
//            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
//            0, 0, 0, 0, 0, 0, 0, 0, // First 48 are not HEX - ignored.

//            // Values from '0' to '9'
//            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,

//            0, 0, 0, 0, 0, 0, 0, // Next 7 also not HEX - ignored.

//            // Values from 'A' to 'F'.
//            10, 11, 12, 13, 14, 15,

//            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
//            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
//            0, 0, 0, 0, 0, 0, 0, // Next 27 not HEX either - ignored.

//            // Values from 'a' to 'f'.
//            // We mad them as well, just in case.
//            10, 11, 12, 13, 14, 15
            
//            // Going outside of the bounds will cause an error - intended.
//        };





//        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
//        /// .
//        /// .                                               ushort Methods
//        /// .                     Note: Order is inverted (from 4 to 1) to make output more readable.
//        /// .
//        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
//        public static string UShort(ushort input)
//        {
//            uint code = input & 0xFFFFu;
//            uint value1 = code & 0x000Fu;
//            uint value2 = code & 0x00F0u;
//            uint value3 = code & 0x0F00u;
//            uint value4 = code & 0xF000u;

//            value2 >>= 4;
//            value3 >>= 8;
//            value4 >>= 12;


//            return $"{toHex[value4]}{toHex[value3]}{toHex[value2]}{toHex[value1]}";
//        }

//        /// <inheritdoc cref="ToUShort(string, int)"/>
//        public static ushort ToUShort(string str)
//        {
//            uint value1 = toUInt[str[3]];
//            uint value2 = toUInt[str[2]];
//            uint value3 = toUInt[str[1]];
//            uint value4 = toUInt[str[0]];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;

//            return (ushort)(value1 | value2 | value3 | value4);
//        }

//        /// <summary>
//        /// Decodes a hex string into a <see cref="ushort"/>.
//        /// </summary>
//        /// <param name="at">Index from which to start.</param>
//        public static ushort ToUShort(string str, int at)
//        {
//            uint value1 = toUInt[str[at + 3]];
//            uint value2 = toUInt[str[at + 2]];
//            uint value3 = toUInt[str[at + 1]];
//            uint value4 = toUInt[str[at]];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;

//            return (ushort)(value1 | value2 | value3 | value4);
//        }

//        public static ushort ToUShort(char hex4, char hex3, char hex2, char hex1)
//        {
//            uint value1 = toUInt[hex4];
//            uint value2 = toUInt[hex3];
//            uint value3 = toUInt[hex2];
//            uint value4 = toUInt[hex1];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;

//            return (ushort)(value1 | value2 | value3 | value4);
//        }

//        // Same as the one above, but with integers instead of chars.
//        public static ushort ToUShort(int hex4, int hex3, int hex2, int hex1)
//        {
//            uint value1 = toUInt[hex4];
//            uint value2 = toUInt[hex3];
//            uint value3 = toUInt[hex2];
//            uint value4 = toUInt[hex1];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;

//            return (ushort)(value1 | value2 | value3 | value4);
//        }




//        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
//        /// .
//        /// .                                               uint Formatting
//        /// .                     Note: Order is inverted (from 8 to 1) to make output more readable.
//        /// .
//        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
//        public static string UInt(uint input)
//        {
//            uint code = input & 0xFFFF_FFFF;
//            uint value1 = code & 0x0000000F;
//            uint value2 = code & 0x000000F0;
//            uint value3 = code & 0x00000F00;
//            uint value4 = code & 0x0000F000;
//            uint value5 = code & 0x000F0000;
//            uint value6 = code & 0x00F00000;
//            uint value7 = code & 0x0F000000;
//            uint value8 = code & 0xF0000000;

//            value2 >>= 4;
//            value3 >>= 8;
//            value4 >>= 12;
//            value5 >>= 16;
//            value6 >>= 20;
//            value7 >>= 24;
//            value8 >>= 28;

//            return $"{toHex[value8]}{toHex[value7]}{toHex[value6]}{toHex[value5]}{toHex[value4]}{toHex[value3]}{toHex[value2]}{toHex[value1]}";
//        }

//        /// <inheritdoc cref="ToUInt(string, int)"/>
//        public static uint ToUInt(string str)
//        {
//            uint value1 = toUInt[str[7]];
//            uint value2 = toUInt[str[6]];
//            uint value3 = toUInt[str[5]];
//            uint value4 = toUInt[str[4]];
//            uint value5 = toUInt[str[3]];
//            uint value6 = toUInt[str[2]];
//            uint value7 = toUInt[str[1]];
//            uint value8 = toUInt[str[0]];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;
//            value5 <<= 16;
//            value6 <<= 20;
//            value7 <<= 24;
//            value8 <<= 28;

//            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8;
//        }

//        /// <summary>
//        /// Decodes a hex string into a <see cref="uint"/>.
//        /// </summary>
//        /// <param name="at">Index from which to start.</param>
//        public static uint ToUInt(string str, int at)
//        {
//            uint value1 = toUInt[str[at + 7]];
//            uint value2 = toUInt[str[at + 6]];
//            uint value3 = toUInt[str[at + 5]];
//            uint value4 = toUInt[str[at + 4]];
//            uint value5 = toUInt[str[at + 3]];
//            uint value6 = toUInt[str[at + 2]];
//            uint value7 = toUInt[str[at + 1]];
//            uint value8 = toUInt[str[at]];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;
//            value5 <<= 16;
//            value6 <<= 20;
//            value7 <<= 24;
//            value8 <<= 28;

//            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8;
//        }

//        public static uint ToUInt(char hex8, char hex7, char hex6, char hex5,
//                                  char hex4, char hex3, char hex2, char hex1)
//        {
//            uint value1 = toUInt[hex8];
//            uint value2 = toUInt[hex7];
//            uint value3 = toUInt[hex6];
//            uint value4 = toUInt[hex5];
//            uint value5 = toUInt[hex4];
//            uint value6 = toUInt[hex3];
//            uint value7 = toUInt[hex2];
//            uint value8 = toUInt[hex1];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;
//            value5 <<= 16;
//            value6 <<= 20;
//            value7 <<= 24;
//            value8 <<= 28;

//            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8;
//        }

//        // Same as the one above, but with integers instead of chars.
//        public static uint ToUInt(int hex8, int hex7, int hex6, int hex5,
//                                  int hex4, int hex3, int hex2, int hex1)
//        {
//            uint value1 = toUInt[hex8];
//            uint value2 = toUInt[hex7];
//            uint value3 = toUInt[hex6];
//            uint value4 = toUInt[hex5];
//            uint value5 = toUInt[hex4];
//            uint value6 = toUInt[hex3];
//            uint value7 = toUInt[hex2];
//            uint value8 = toUInt[hex1];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;
//            value5 <<= 16;
//            value6 <<= 20;
//            value7 <<= 24;
//            value8 <<= 28;

//            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8;
//        }




//        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
//        /// .
//        /// .                                              ulong Formatting
//        /// .                     Note: Order is inverted (from 8 to 1) to make output more readable.
//        /// .
//        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
//        public static string UInt(uint input)
//        {
//            uint code = input & 0xFFFF_FFFF;
//            uint value1 = code & 0x0000000F;
//            uint value2 = code & 0x000000F0;
//            uint value3 = code & 0x00000F00;
//            uint value4 = code & 0x0000F000;
//            uint value5 = code & 0x000F0000;
//            uint value6 = code & 0x00F00000;
//            uint value7 = code & 0x0F000000;
//            uint value8 = code & 0xF0000000;

//            value2 >>= 4;
//            value3 >>= 8;
//            value4 >>= 12;
//            value5 >>= 16;
//            value6 >>= 20;
//            value7 >>= 24;
//            value8 >>= 28;

//            return $"{toHex[value8]}{toHex[value7]}{toHex[value6]}{toHex[value5]}{toHex[value4]}{toHex[value3]}{toHex[value2]}{toHex[value1]}";
//        }

//        /// <inheritdoc cref="ToUInt(string, int)"/>
//        public static uint ToUInt(string str)
//        {
//            uint value1 = toUInt[str[7]];
//            uint value2 = toUInt[str[6]];
//            uint value3 = toUInt[str[5]];
//            uint value4 = toUInt[str[4]];
//            uint value5 = toUInt[str[3]];
//            uint value6 = toUInt[str[2]];
//            uint value7 = toUInt[str[1]];
//            uint value8 = toUInt[str[0]];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;
//            value5 <<= 16;
//            value6 <<= 20;
//            value7 <<= 24;
//            value8 <<= 28;

//            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8;
//        }

//        /// <summary>
//        /// Decodes a hex string into a <see cref="uint"/>.
//        /// </summary>
//        /// <param name="at">Index from which to start.</param>
//        public static uint ToUInt(string str, int at)
//        {
//            uint value1 = toUInt[str[at + 7]];
//            uint value2 = toUInt[str[at + 6]];
//            uint value3 = toUInt[str[at + 5]];
//            uint value4 = toUInt[str[at + 4]];
//            uint value5 = toUInt[str[at + 3]];
//            uint value6 = toUInt[str[at + 2]];
//            uint value7 = toUInt[str[at + 1]];
//            uint value8 = toUInt[str[at]];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;
//            value5 <<= 16;
//            value6 <<= 20;
//            value7 <<= 24;
//            value8 <<= 28;

//            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8;
//        }

//        public static uint ToUInt(char hex8, char hex7, char hex6, char hex5,
//                                  char hex4, char hex3, char hex2, char hex1)
//        {
//            uint value1 = toUInt[hex8];
//            uint value2 = toUInt[hex7];
//            uint value3 = toUInt[hex6];
//            uint value4 = toUInt[hex5];
//            uint value5 = toUInt[hex4];
//            uint value6 = toUInt[hex3];
//            uint value7 = toUInt[hex2];
//            uint value8 = toUInt[hex1];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;
//            value5 <<= 16;
//            value6 <<= 20;
//            value7 <<= 24;
//            value8 <<= 28;

//            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8;
//        }

//        // Same as the one above, but with integers instead of chars.
//        public static uint ToUInt(int hex8, int hex7, int hex6, int hex5,
//                                  int hex4, int hex3, int hex2, int hex1)
//        {
//            uint value1 = toUInt[hex8];
//            uint value2 = toUInt[hex7];
//            uint value3 = toUInt[hex6];
//            uint value4 = toUInt[hex5];
//            uint value5 = toUInt[hex4];
//            uint value6 = toUInt[hex3];
//            uint value7 = toUInt[hex2];
//            uint value8 = toUInt[hex1];

//            value2 <<= 4;
//            value3 <<= 8;
//            value4 <<= 12;
//            value5 <<= 16;
//            value6 <<= 20;
//            value7 <<= 24;
//            value8 <<= 28;

//            return value1 | value2 | value3 | value4 | value5 | value6 | value7 | value8;
//        }
//    }
//}

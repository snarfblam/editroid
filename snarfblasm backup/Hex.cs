using System;
using System.Collections.Generic;
using System.Text;

namespace Romulus
{
    public static class Hex
    {
        static char[] hexDigitsU = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        static char[] hexDigitsL = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };

        public static string FormatHex(byte[] hex, HexCasing casing) {
            if (hex == null) throw new ArgumentNullException("hex");

            StringBuilder result = new StringBuilder(hex.Length * 2);
            FormatHex(hex, result, casing);
            return result.ToString();
        }
        public static string FormatHex(byte[] hex) {
            return FormatHex(hex, HexCasing.Upper);
        }
        public static void FormatHex(byte[] hex, StringBuilder output) {
            FormatHex(hex, output, HexCasing.Upper);
        }
        public static void FormatHex(byte[] hex, StringBuilder output, HexCasing casing) {
            if (hex == null) throw new ArgumentNullException("hex");

            char[] digits;
            if (casing == HexCasing.Upper)
                digits = hexDigitsU;
            else if (casing == HexCasing.Lower)
                digits = hexDigitsL;
            else
                throw new ArgumentException("Invalid value for casing.", "casing");


            for (int i = 0; i < hex.Length; i++) {
                int hexVal = hex[i];

                int highNib = hexVal >> 4;
                int lowNib = hexVal & 0xF;

                output.Append(digits[highNib]);
                output.Append(digits[lowNib]);
            }
        }

        /// <summary>
        /// Returns the integer value of the specified hex digit, or -1 if the specified character is not a valid hex digit.
        /// </summary>
        /// <param name="d">The hex digit to parse.</param>
        /// <returns>An integer between -1 and 15.</returns>
        public static int ParseDigit(char d) {
            if (d >= '0' & d <= '9') {
                return (int)(d - '0');
            }
            if (d >= 'A' & d <= 'F') {
                return (int)(d - 'A' + 10);
            }
            if (d >= 'a' & d <= 'f') {
                return (int)(d - 'a' + 10);
            }
            return -1;
        }
        public static byte[] ParseHex(string hex) {
            if (hex == null)
                throw new ArgumentNullException("hex");
            // Require even number of bytes
            if (hex.Length % 2 == 1)
                throw new ArgumentException("Parameter \"hex\" is not a valid hex string.", "hex");

            byte[] result = new byte[hex.Length / 2];
            int iChar = 0;
            for (int iByte = 0; iByte < result.Length; iByte++) {
                int byteValue = 0;

                // Parse first digit of byte
                char digit = hex[iChar];
                if (digit >= '0' & digit <= '9') {
                    byteValue |= (int)(digit - '0');
                } else if (digit >= 'A' & digit <= 'F') {
                    byteValue |= (int)(digit - ('A' - (char)10));
                } else if (digit >= 'a' & digit <= 'f') {
                    byteValue |= (int)(digit - ('a' - (char)10));
                } else {
                    throw new ArgumentException("Parameter \"hex\" is not a valid hex string.", "hex");
                }

                // Move to high nibble
                byteValue <<= 4;
                // Next char
                iChar++;

                // Parse second digit of byte
                digit = hex[iChar];
                if (digit >= '0' & digit <= '9') {
                    byteValue |= (int)(digit - '0');
                } else if (digit >= 'A' & digit <= 'F') {
                    byteValue |= (int)(digit - ('A' - (char)10));
                } else if (digit >= 'a' & digit <= 'f') {
                    byteValue |= (int)(digit - ('a' - (char)10));
                } else {
                    throw new ArgumentException("Parameter \"hex\" is not a valid hex string.", "hex");
                }

                // Next char
                iChar++;

                result[iByte] = (byte)byteValue;
            }

            return result;
        }
    }
    public enum HexCasing
    {
        Upper,
        Lower
    }
}

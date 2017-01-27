using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.Extension
{
    public static class NumberExtensions
    {
        public static int Clamp(this int i, int min, int max) {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
        public static byte Clamp(this byte i, byte min, byte max) {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
        public static long Clamp(this long i, long min, long max) {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
        public static short Clamp(this short i, short min, short max) {
            if (i < min) return min;
            if (i > max) return max;
            return i;
        }
    }
}

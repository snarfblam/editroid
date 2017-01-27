using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    /// <summary>
    /// A pointer-like value representing an offset within a headered ROM.
    /// Can be implicity cast to integer, explicitly cast from integer.
    /// </summary>
    public struct pRom
    {
        int value;

        public static readonly pRom Null;

        public static implicit operator int(pRom o) {
            return o.value;
        }
        public static explicit operator pRom(int i) {
            pRom result;
            result.value = i;
            return result;
        }

        public static pRom operator +(pRom o, int i) {
            return (pRom)(o.value + i);
        }
        public static pRom operator -(pRom o, int i) {
            return (pRom)(o.value - i);
        }
        public static int operator -(pRom o, pRom i) {
            return o.value - i.value;
        }
        public static bool operator ==(pRom o, pRom o2)
        {
            return o.value == o2.value;
        }
        public static bool operator !=(pRom o, pRom o2)
        {
            return o.value != o2.value;
        }

        public static bool operator >(pRom o, pRom o2) {
            return o.value > o2.value;
        }
        public static bool operator >=(pRom o, pRom o2) {
            return o.value >= o2.value;
        }
        public static bool operator <(pRom o, pRom o2) {
            return o.value < o2.value;
        }
        public static bool operator <=(pRom o, pRom o2) {
            return o.value <= o2.value;

        }
        public static pRom operator ++(pRom value) {
            value.value++;
            return value;
        }
        public static pRom operator --(pRom value) {
            value.value++;
            return value;
        }
        public override string ToString() {
            return value.ToString("x");
        }
        public string ToString(string format) {
            return value.ToString(format);
        }

        public bool IsNull { get { return value == 0; } }
    }
}

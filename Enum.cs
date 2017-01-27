using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
    public static class Enum<T> where T:struct, IConvertible
    {
        public static bool IsDefined(T value) {
            return Enum.IsDefined(typeof(T), value);
        }

        static bool isUlong = Enum.GetUnderlyingType(typeof(T)) == typeof(ulong);

        
        
    }

    public struct Flags<T> where T : struct, IConvertible
    {
        T value;
        public Flags(T value) {
            this.value = value;
        }
        public static implicit operator T(Flags<T> val) {
            return val.value;
        }
        public static implicit operator Flags<T>(T val) {
            return new Flags<T>(val);
        }

        public bool CheckFlags(T flags) {
            Int32 v = value.ToInt32(null);
            return (flags.ToInt32(null) & v) == v;
        }

        public void SetFlags(T flags) {
            value = (T)
                ((IConvertible)(value.ToInt32(null) | flags.ToInt32(null))).ToType(typeof(T), null);
        }
        public void SetFlags(T flags, bool state) {
            if (state)
                SetFlags(flags);
            else
                ClearFlags(flags);
        }

        public void ClearFlags(T flags) {
            value = (T)
                ((IConvertible)(value.ToInt32(null) & ~(flags.ToInt32(null)))).ToType(typeof(T), null);
        }
    }
}

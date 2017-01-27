using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    public struct BOOL
    {
        int value;
        private BOOL(int value) {
            this.value = value;
        }

        public static BOOL TRUE = new BOOL(1);
        public static BOOL FALSE = new BOOL(0);

        public static implicit operator bool(BOOL value) {
            return value.value != 0;
        }
        public static implicit operator BOOL(bool value) {
            if (value)
                return TRUE;
            else
                return FALSE;
        }
        public static explicit operator int(BOOL value) {
            return value.value;
        }
        public static explicit operator BOOL(int value) {
            return new BOOL(value);
        }

        public override bool Equals(object obj) {
            if (obj is bool || obj is BOOL || obj is int)
                return (bool)this == (bool)obj;

            return false;
            
        }
    }
}

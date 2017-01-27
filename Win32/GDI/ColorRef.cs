using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Windows
{
    public static partial class Gdi
    {
        [StructLayout( LayoutKind.Explicit)]
        public struct ColorRef
        {
            [FieldOffset(0)]
            public int data;

            [FieldOffset(0)] public byte b;
            [FieldOffset(1)] public byte g;
            [FieldOffset(2)] public byte r;
            [FieldOffset(3)] public byte a;

            public static ColorRef Empty;
            public ColorRef(Int32 value) {
                this = Empty;
                data = value;
            }
            public ColorRef(int r, int g, int b) {
                this = Empty;
                this.r = (byte)r;
                this.g = (byte)g;
                this.b = (byte)b;
            }
            public ColorRef(int a, int r, int g, int b) {
                this = Empty;
                this.a = (byte)a;
                this.r = (byte)r;
                this.g = (byte)g;
                this.b = (byte)b;
            }

        }
    }
}

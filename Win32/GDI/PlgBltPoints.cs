using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    public static partial class Gdi
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct PlgBltPoints
        {
            public int x1, y1, x2, y2, x3, y3;
        }
    }
}

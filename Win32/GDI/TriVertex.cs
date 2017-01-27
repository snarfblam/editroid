using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Windows
{
    public static partial class Gdi
    {
        [StructLayout(LayoutKind.Sequential, Pack=1)]
        public struct TriVertex
        {
            int x;
            int y;

            ushort red;
            ushort green;
            ushort blue;
            ushort alpha;
        }
    }
}

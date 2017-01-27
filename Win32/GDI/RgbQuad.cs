using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Windows
{
    public static partial class Gdi
    {
        [StructLayout( LayoutKind.Sequential, Pack = 1)]
        public struct RgbQuad
        {
            byte red;
            byte green;
            byte blue;
            byte reserved;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    public static partial class Gdi
    {
        public struct Bitmap
        {
            /// <summary>This value should be zero.</summary>
            int type;
            /// <summary>Pixel x of bitmap.</summary>
            int width;
            /// <summary>Pixel height of bitmap.</summary>
            int height;
            /// <summary>Size of scan line in bytes.</summary>
            int scanSizeBytes;
            /// <summary>Number of color planes.</summary>
            ushort planeCount;
            /// <summary>Bit-depth.</summary>
            ushort bitsPerPixel;
            /// <summary>Pointer to pixel data.</summary>
            IntPtr data;
        }
    }
}

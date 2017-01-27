using System;
using System.Collections.Generic;
using System.Text;
using Windows.Enum;

namespace Windows
{
    public static partial class Gdi
    {
        public struct BitmapInfo
        {
            /// <summary>Header.</summary>
            BitmapInfoHeader header;
            /// <summary>First color in table. DotNet can not marshall variable-sized arrays.</summary>
            RgbQuad firstColorTableEntry;

            /// <summary>
            /// A version of BitmapInfo that can be flattened into a byte array to be passed as a 
            /// pointer to the operating system.
            /// </summary>
            public struct Flattenable
            {
                /// <summary>Header.</summary>
                BitmapInfoHeader header;
                /// <summary>An array of color table entries that will be expanded into the structure upon calling Flatten.</summary>
                RgbQuad[] colorTable;

                public byte[] Flatten() {
                    int extraBytes;
                    return Utility.ExpandArraysInStruct(this, out extraBytes);
                }
            }
        }
        public struct BitmapInfo4
        {
            /// <summary>Header.</summary>
            BitmapInfoHeaderV4 header;
            /// <summary>First color in table. DotNet can not marshall variable-sized arrays.</summary>
            RgbQuad firstColorTableEntry;


            /// <summary>
            /// A version of BitmapInfo4 that can be flattened into a byte array to be passed as a 
            /// pointer to the operating system.
            /// </summary>
            public struct Flattenable
            {
                /// <summary>Header.</summary>
                BitmapInfoHeaderV4 header;
                /// <summary>An array of color table entries that will be expanded into the structure upon calling Flatten.</summary>
                RgbQuad[] colorTable;

                public byte[] Flatten() {
                    int extraBytes;
                    return Utility.ExpandArraysInStruct(this, out extraBytes);
                }
            }
        }
        public struct BitmapInfo5
        {
            /// <summary>Header.</summary>
            BitmapInfoHeaderV5 header;
            /// <summary>First color in table. DotNet can not marshall variable-sized arrays.</summary>
            RgbQuad firstColorTableEntry;


            /// <summary>
            /// A version of BitmapInfo5 that can be flattened into a byte array to be passed as a 
            /// pointer to the operating system.
            /// </summary>
            public struct Flattenable
            {
                /// <summary>Header.</summary>
                BitmapInfoHeader header;
                /// <summary>An array of color table entries that will be expanded into the structure upon calling Flatten.</summary>
                RgbQuad[] colorTable;

                public byte[] Flatten() {
                    int extraBytes;
                    return Utility.ExpandArraysInStruct(this, out extraBytes);
                }
            }
        }

        public struct BitmapInfoHeader
        {
            /// <summary>The number of bytes this header fills.</summary>
            uint Size;
            /// <summary>The x of the bitmap in pixels.</summary>
            int Width;
            /// <summary>The height of the bitmap in pixels. If positive, the bitmap is bottom-up. If negative, the bitmap is top-down DIB.</summary>
            int Height;
            /// <summary>The number of planes for the target device. This value must be set to 1.</summary>
            ushort Planes;
            /// <summary>Bit depth, or 0 for JPeg and PNG.</summary>
            ushort BitCount;
            /// <summary>Compression used for the bitmap.</summary>
            BitmapCompression Compression;
            /// <summary>Size of image data in bytes. Can be 0 if Compression is Rgb. Should be size of compressed image stream for Jpegs ane PNGs.</summary>
            uint ImageSize;
            /// <summary>Horizontal resolution.</summary>
            int PixelsPerMeterX;
            /// <summary>Vertical resolution.</summary>
            int PixelsPerMeterY;
            /// <summary>The number of colors used, or zero to use the maximim for the specified compression.</summary>
            uint ColorsUsed;
            /// <summary>The nember of colors needed to display the image, or zero if all colors are needed.</summary>
            uint ColorsNeeded;
        }
        public struct BitmapInfoHeaderV4
        {
            /// <summary>The number of bytes this header fills.</summary>
            uint Size;
            /// <summary>The x of the bitmap in pixels.</summary>
            int Width;
            /// <summary>The height of the bitmap in pixels. If positive, the bitmap is bottom-up. If negative, the bitmap is top-down DIB.</summary>
            int Height;
            /// <summary>The number of planes for the target device. This value must be set to 1.</summary>
            ushort Planes;
            /// <summary>Bit depth, or 0 for JPeg and PNG.</summary>
            ushort BitCount;
            /// <summary>Compression used for the bitmap.</summary>
            BitmapCompression Compression;
            /// <summary>Size of image data in bytes. Can be 0 if Compression is Rgb. Should be size of compressed image stream for Jpegs ane PNGs.</summary>
            uint ImageSize;
            /// <summary>Horizontal resolution.</summary>
            int PixelsPerMeterX;
            /// <summary>Vertical resolution.</summary>
            int PixelsPerMeterY;
            /// <summary>The number of colors used, or zero to use the maximim for the specified compression.</summary>
            uint ColorsUsed;
            /// <summary>The nember of colors needed to display the image, or zero if all colors are needed.</summary>
            uint ColorsNeeded;


            /// <summary>ColorMask for the red component for BitFields bitmaps.</summary>
            uint RedMask;
            /// <summary>ColorMask for the green component for BitFields bitmaps.</summary>
            uint GreenMask;
            /// <summary>ColorMask for the blue component for BitFields bitmaps.</summary>
            uint BlueMask;
            /// <summary>ColorMask for the alpha component for BitFields bitmaps.</summary>
            uint AlphaMask;
            /// <summary>The color space of the DIB. Should be 0.</summary>
            ColorSpace colorSpaceType;
            /// <summary>A CieXyzTriple that specifies the x, y, and z coordinates of the three colors that correspond to the red, green, and blue endpoints for the logical color space associated with the bitmap.</summary>
            CieXyzTriple Endpoints;
            /// <summary>Tone response curve for red.</summary>
            uint GammaRed;
            /// <summary>Tone response curve for green.</summary>
            uint GammaGreen;
            /// <summary>Tone response curve for blue.</summary>
            uint GammaBlue;

        }
        public struct BitmapInfoHeaderV5
        {
            /// <summary>The number of bytes this header fills.</summary>
            uint Size;
            /// <summary>The x of the bitmap in pixels.</summary>
            int Width;
            /// <summary>The height of the bitmap in pixels. If positive, the bitmap is bottom-up. If negative, the bitmap is top-down DIB.</summary>
            int Height;
            /// <summary>The number of planes for the target device. This value must be set to 1.</summary>
            ushort Planes;
            /// <summary>Bit depth, or 0 for JPeg and PNG.</summary>
            ushort BitCount;
            /// <summary>Compression used for the bitmap.</summary>
            BitmapCompression Compression;
            /// <summary>Size of image data in bytes. Can be 0 if Compression is Rgb. Should be size of compressed image stream for Jpegs ane PNGs.</summary>
            uint ImageSize;
            /// <summary>Horizontal resolution.</summary>
            int PixelsPerMeterX;
            /// <summary>Vertical resolution.</summary>
            int PixelsPerMeterY;
            /// <summary>The number of colors used, or zero to use the maximim for the specified compression.</summary>
            uint ColorsUsed;
            /// <summary>The nember of colors needed to display the image, or zero if all colors are needed.</summary>
            uint ColorsNeeded;


            /// <summary>ColorMask for the red component for BitFields bitmaps.</summary>
            uint RedMask;
            /// <summary>ColorMask for the green component for BitFields bitmaps.</summary>
            uint GreenMask;
            /// <summary>ColorMask for the blue component for BitFields bitmaps.</summary>
            uint BlueMask;
            /// <summary>ColorMask for the alpha component for BitFields bitmaps.</summary>
            uint AlphaMask;
            /// <summary>The color space of the DIB. Should be 0.</summary>
            ColorSpace colorSpaceType;
            /// <summary>A CieXyzTriple that specifies the x, y, and z coordinates of the three colors that correspond to the red, green, and blue endpoints for the logical color space associated with the bitmap.</summary>
            CieXyzTriple Endpoints;
            /// <summary>Tone response curve for red.</summary>
            uint GammaRed;
            /// <summary>Tone response curve for green.</summary>
            uint GammaGreen;
            /// <summary>Tone response curve for blue.</summary>
            uint GammaBlue;

            /// <summary>Rendering intent for bitmap.</summary>
            ColorSpaceGammaIntent Intent;
            /// <summary>offset in bytes from the start of this BitmapInfoHeaderV5 to the start of the profile data (which is either a null terminated filename or embedded profile data).</summary>
            ulong ProfileData;
            /// <summary>Size in bytes, of embedded profile data.</summary>
            ulong ProfileSize;
            /// <summary>Reserved. This should be set to 0.</summary>
            ulong Reserved;

        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    public static partial class Gdi
    {
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RegionData
        {

            public RegionDataHeader header;

            public byte[] dataBuffer;

            /// <summary>Creates a RegionData with a buffer of the specified size.</summary>
            /// <param name="bufferSize">The number of bytes</param>
            public RegionData(int bufferSize) {
                header = new RegionDataHeader();
                dataBuffer = new byte[bufferSize];
            }
            /// <summary>Creates a RegionData sized to hold data for the 
            /// specified region, but does not obtain the actual data.</summary>
            /// <param name="region">The region whose data to allocate a buffer for.</param>
            public RegionData(HRgn region) {
                header = new RegionDataHeader();
                int dataSize = (int)Gdi.GetRegionDataSize(region, 0, 0);
                dataBuffer = new byte[dataSize];
            }
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct RegionDataHeader
        {
            /// <summary>Specifies the size, in bytes, of the header. </summary>
            public uint headerSizeInBytes;
            /// <summary>Specifies the type of region. This value must be RegionDataType.Rectangles.</summary>
            public Windows.Enum.RegionDataType type;
            /// <summary>Specifies the number of rectangles that make up the region. </summary>
            public uint rectangleCount;
            /// <summary>Specifies the size of the buffer required to receive the RECT structure that specifies the coordinates of the rectangles that make up the region. If the size is not known, this member can be zero. </summary>
            public uint RECT_Size;
            /// <summary>Specifies a bounding rectangle for the region.</summary>
            public RECT boundingRect;
        }

    }
}

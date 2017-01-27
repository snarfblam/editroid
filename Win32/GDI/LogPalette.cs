using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Windows.Enum;

namespace Windows
{
    public static partial class Gdi
    {
        /// <summary>
        /// Defines a logical palette.
        /// </summary>
        public struct LogPalette
        {
            /// <summary>The version number of the system.</summary>
            public uint PaletteVersion;
            /// <summary>The number of entries in the logical palette.</summary>
            public uint EntryCount;
            /// <summary>Define the color and usage of each entry in the logical palette.
            /// Only the first entry is present because DotNet can not marshal variable-sized arrays.</summary>
            public PaletteEntry firstEntry;

            /// <summary>
            /// A version of LogPalette that can be flattened into a byte array to be passed as a 
            /// pointer to the operating system.
            /// </summary>
            public struct Flattenable
            {
                /// <summary>The version number of the system.</summary>
                public uint PaletteVersion;
                /// <summary>The number of entries in the logical palette.</summary>
                public uint EntryCount;
                /// <summary>An array of palette entries that will be expanded into the structure upon calling Flatten..</summary>
                public PaletteEntry[] entries;

                public byte[] Flatten() {
                    int extraBytes;
                    return Utility.ExpandArraysInStruct(this, out extraBytes);
                }
            }
        }
    }
}

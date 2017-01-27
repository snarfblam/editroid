using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Windows
{
    public static partial class Gdi
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PaletteEntry
        {
            byte Red;
            byte Green;
            byte Blue;
            Windows.Enum.PaletteEntryFlags flags; 
        }
    }
}

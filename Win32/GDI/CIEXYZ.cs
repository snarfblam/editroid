using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Windows
{
    public static partial class Gdi
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct CieXyzTriple
        {
            CieXyz red;
            CieXyz green;
            CieXyz blue;
        }

        [StructLayout( LayoutKind.Sequential)]
        public struct CieXyz
        {
            FxPt2_30 x;
            FxPt2_30 y;
            FxPt2_30 z;
        }

        /// <summary>Defines a structure that holds data of a
        /// fixed-points number with two binary digits for the integer and
        /// thirty digits for the fraction.</summary>
        public struct FxPt2_30
        {
            uint data;

            public static implicit operator FxPt2_30(uint value) {
                FxPt2_30 result;
                result.data = value;
                return result;
            }
            public static implicit operator uint(FxPt2_30 value) {
                return value.data;
            }
        }
    }
}

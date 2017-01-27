using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Windows.Enum;

namespace Windows
{
    public static partial class Gdi
    {
        [StructLayout( LayoutKind.Explicit)]
        public struct LogBrush
        {
            /// <summary>The brush style.</summary>
            [FieldOffset(0)]
            BrushStyle Style;



            /// <summary>The foreground color in which the brush is to be drawn. 
            /// If LogBrush.Style is Hollow, Pattern, DibPatternPtr or DibPattern, Color is ignored.
            /// </summary>
            [FieldOffset(4)]
            ColorRef Color;

            /// <summary>Applies to BrushStyle.DibPatternPtr and BrushStyle.DibPattern. Specifies whether the color table entries of the BitmapInfo structure contain explicit RGB values or indexes into the currently realized logical palette.</summary>
            [FieldOffset(4)]
            BitmapColorUsage ColorUsage;



            /// <summary>Applies to BrushStyle.Hatch. Orientation of the lines used to create the hatch.</summary>
            [FieldOffset(8)]
            HatchStyle style;

            /// <summary>For BrushStyle.DibPattern, a handle to a packed DIB. For BrushStyle.Pattern, a handle to a bitmap that defines the pattern.</summary>
            [FieldOffset(8)]
            HBitmap PatternHandle;

            /// <summary>Far BrushStyle.DibPatternPtr, a pointer to a packed DIB. For BrushStyle.DibPattern, a handle to a packed DIB.</summary>
            [FieldOffset(8)]
            IntPtr PatternDibPointer;
        }
    }
}

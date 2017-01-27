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
        /// Defines the color adjustment values used by the StretchBlt and StretchDIBits functions when the stretch mode is Halftone.
        /// </summary>
        [StructLayout( LayoutKind.Sequential, Pack = 1)]
        public struct ColorAdjustment
        {
            /// <summary>The size, in bytes, of this structure.</summary>
            ushort Size;
            /// <summary>Specifies how the output image should be prepared.</summary>
            ColorAdjustmentFlags Flags;
            /// <summary>The type of standard light source under which the image is viewed.</summary>
            Illuminant IlluminantIndex;
            /// <summary>Specifies the nth power gamma-correction value for the red primary of the source colors. The value must be in the range from 2500 to 65,000. A value of 10,000 means no gamma correction.</summary>
            ushort RedGamma;
            /// <summary>Specifies the nth power gamma-correction value for the green primary of the source colors. The value must be in the range from 2500 to 65,000. A value of 10,000 means no gamma correction.</summary>
            ushort GreenGamma;
            /// <summary>Specifies the nth power gamma-correction value for the blue primary of the source colors. The value must be in the range from 2500 to 65,000. A value of 10,000 means no gamma correction.</summary>
            ushort BlueGamma;
            /// <summary>The black reference for the source colors. Any colors that are darker than this are treated as black. The value must be in the range from 0 to 4000.</summary>
            ushort ReferenceBlack;
            /// <summary>The white reference for the source colors. Any colors that are lighter than this are treated as white. The value must be in the range from 6000 to 10,000.</summary>
            ushort ReferenceWhite;
            /// <summary>The amount of contrast to be applied to the source object. The value must be in the range from -100 to 100. A value of 0 means no contrast adjustment.</summary>
            short Contrast;
            /// <summary>The amount of brightness to be applied to the source object. The value must be in the range from -100 to 100. A value of 0 means no brightness adjustment.</summary>
            short Brightness;
            /// <summary>The amount of colorfulness to be applied to the source object. The value must be in the range from -100 to 100. A value of 0 means no colorfulness adjustment.</summary>
            short Colorfulness;
            /// <summary>The amount of red or green tint adjustment to be applied to the source object. The value must be in the range from -100 to 100. Positive numbers adjust towards red and negative numbers adjust towards green. Zero means no tint adjustment.</summary>
            short RedGreenTint;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Windows
{
    /// <summary>
    /// Defines a recangle.
    /// </summary>
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>Coordinate of the left edge of the rectangle.</summary>
        public int Left;
        /// <summary>Coordinate of the top edge of the rectangle.</summary>
        public int Top;
        /// <summary>Coordinate of the right edge of the rectangle.</summary>
        public int Right;
        /// <summary>Coordinate of the bottom edge of the rectangle.</summary>
        public int Bottom;

        public static readonly RECT Empty;

        /// <summary>Gets/sets the x of this rectangle.</summary>
        public int Width { get { return Right - Left; } set { Right = Left + value; } }
        /// <summary>Gets/sets the height of this rectangle.</summary>
        public int Height { get { return Bottom - Top; } set { Bottom = Top + value; } }

        /// <summary>Provides an implicit conversion from a Windows RECT to a DotNot rectangle.</summary>
        /// <param name="a">The RECT to convert.</param>
        /// <returns>A Rectangle.</returns>
        public static implicit operator Rectangle(RECT a) {
            return new Rectangle(a.Left, a.Top, a.Width, a.Height);
        }
        /// <summary>Provides an implicit conversion from a DotNot rectangle to a Windows RECT.</summary>
        /// <param name="a">The Rectangle to convert.</param>
        /// <returns>A RECT.</returns>
        public static implicit operator RECT(Rectangle a) {
            RECT result;
            result.Left = a.Left;
            result.Top = a.Top;
            result.Right = a.Right;
            result.Bottom = a.Bottom;

            return result;
        }
    }
}

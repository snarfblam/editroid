using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    public static partial class Gdi
    {
        public struct POINT
        {
            public int x, y;

            public static implicit operator System.Drawing.Point(POINT p) {
                return new System.Drawing.Point(p.x, p.y);
            }
            public static implicit operator POINT(System.Drawing.Point p) {
                POINT result;
                result.x = p.X;
                result.y = p.Y;
                return result;
            }
        }
    }
}

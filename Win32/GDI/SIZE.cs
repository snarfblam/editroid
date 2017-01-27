using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    public static partial class Gdi
    {
        public struct SIZE
        {
            public int width, height;

            public static implicit operator System.Drawing.Size(SIZE size) {
                return new System.Drawing.Size(size.width, size.height);
            }
            public static implicit operator SIZE(System.Drawing.Size size) {
                SIZE result;
                result.width = size.Width;
                result.height = size.Height;
                return result;
            }
        }
    }
}

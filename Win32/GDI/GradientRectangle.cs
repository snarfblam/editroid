using System;
using System.Collections.Generic;
using System.Text;

namespace Windows
{
    public static partial class Gdi
    {
        /// <summary>
        /// Specifies the index of three vertices in a TriVertex array that form one triangle.
        /// </summary>
        public struct GradientRectangle
        {
            uint TopLeftVertexIndex;
            uint TopRightVertexIndex;
        }
    }
}

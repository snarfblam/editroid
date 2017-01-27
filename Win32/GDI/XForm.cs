using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Windows
{
    public static partial class Gdi
    {
        /// <summary>
        /// Specifies a world-space to page-space transformation.
        /// </summary>
        public struct XForm
        {
            /// <summary>
            /// Scaling: Horizontal scaling component. Rotation: Cosine of rotation angle. Reflection: Horizontal component
            /// </summary>
            public float eM11;
            /// <summary>
            /// Shear: Horizontal proportionality constant. Rotation: Sine of the rotation angle
            /// </summary>
            public float eM12;
            /// <summary>
            /// Shear: Vertical proportionality constant. Rotation: Negative sine of the rotation angle.
            /// </summary>
            public float eM21;
            /// <summary>
            /// Scaling: Vertical scaling component. Rotation: Cosine of rotation angle. Reflection: Vertical reflection component.
            /// </summary>
            public float eM22;
            /// <summary>
            /// The horizontal translation component, in logical units.
            /// </summary>
            public float eDx;
            /// <summary>
            /// The vertical translation component, in logical units.
            /// </summary>
            public float eDy;

            /// <summary>
            /// Provides a mechanism to convert between XForm types with equal binary representations. Simply assign to one field, then read from another.
            /// </summary>
            [StructLayout( LayoutKind.Explicit)]
            public struct XFormConverter
            {
                [FieldOffset(0)]
                public XForm XForm;
                [FieldOffset(0)]
                public XForm.Reflection Reflection;
                [FieldOffset(0)]
                public XForm.Scale Scale;
                [FieldOffset(0)]
                public XForm.Rotate Rotate;
                [FieldOffset(0)]
                public XForm.Shear Shear;

                public static XFormConverter GetConverter(){
                    XFormConverter result;
                    result.XForm = new XForm();
                    result.Shear = new Shear();
                    result.Scale = new Scale();
                    result.Rotate = new Rotate();
                    result.Reflection = new Reflection();

                    return result;
                }
            }


            /// <summary>
            /// Specifies a world-space to page-space transformation.
            /// </summary>
            public struct Scale
            {
                /// <summary>
                /// Horizontal scaling component.
                /// </summary>
                public float Horizontal;
                /// <summary>
                /// Unused.
                /// </summary>
                public float unused_1;
                /// <summary>
                /// Unused.
                /// </summary>
                public float unused_2;
                /// <summary>
                /// Vertical scaling component.
                /// </summary>
                public float Vertical;
                /// <summary>
                /// The horizontal translation component, in logical units.
                /// </summary>
                public float Dx;
                /// <summary>
                /// The vertical translation component, in logical units.
                /// </summary>
                public float Dy;
            }

            /// <summary>
            /// Specifies a world-space to page-space transformation.
            /// </summary>
            public struct Rotate
            {
                /// <summary>
                /// Cosine of rotation angle.
                /// </summary>
                public float Cosine;
                /// <summary>
                /// Sine of the rotation angle
                /// </summary>
                public float Sine;
                /// <summary>
                /// Negative sine of the rotation angle.
                /// </summary>
                public float NegativeSine;
                /// <summary>
                /// Cosine of rotation angle.
                /// </summary>
                public float Cosine2;
                /// <summary>
                /// The horizontal translation component, in logical units.
                /// </summary>
                public float Dx;
                /// <summary>
                /// The vertical translation component, in logical units.
                /// </summary>
                public float Dy;
            }

            /// <summary>
            /// Specifies a world-space to page-space transformation.
            /// </summary>
            public struct Shear
            {
                /// <summary>
                /// Unused.
                /// </summary>
                public float unused_1;
                /// <summary>
                /// Horizontal proportionality constant.
                /// </summary>
                public float Horizontal;
                /// <summary>
                /// Vertical proportionality constant.
                /// </summary>
                public float Vertical;
                /// <summary>
                /// Unused.
                /// </summary>
                public float unused_2;
                /// <summary>
                /// The horizontal translation component, in logical units.
                /// </summary>
                public float Dx;
                /// <summary>
                /// The vertical translation component, in logical units.
                /// </summary>
                public float Dy;
            }
            /// <summary>
            /// Specifies a world-space to page-space transformation.
            /// </summary>
            public struct Reflection
            {
                /// <summary>
                /// Horizontal component
                /// </summary>
                public float Horizontal;
                /// <summary>
                /// Unused
                /// </summary>
                public float unused_1;
                /// <summary>
                /// Unused
                /// </summary>
                public float unused_2;
                /// <summary>
                /// Vertical reflection component.
                /// </summary>
                public float vertical;
                /// <summary>
                /// The horizontal translation component, in logical units.
                /// </summary>
                public float Dx;
                /// <summary>
                /// The vertical translation component, in logical units.
                /// </summary>
                public float Dy;
            }
        }

    }
}

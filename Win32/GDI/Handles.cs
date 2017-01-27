using System;
using System.Collections.Generic;
using System.Text;
using Windows.Enum;

namespace Windows
{
    public struct HRgn : IHandle
    {
        private IntPtr value;
        public static readonly HRgn Zero;

        public static implicit operator IntPtr(HRgn handle) {
            return handle.value;
        }
        public static explicit operator HRgn(IntPtr handle) {
            HRgn result;
            result.value = handle;
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HRgn A, HRgn B) {
            return A.value == B.value;
        }
        public static bool operator !=(HRgn A, HRgn B) {
            return A.value != B.value;
        }
        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        public static HRgn CreateFromRECT(RECT rect) {
            return Gdi.CreateRectRgn(rect);
        }
        public bool Delete() {
            return Gdi.DeleteObject(this);
        }
    }


    public struct HBitmap : IHandle
    {
        private IntPtr value;
        public static readonly HBitmap Zero;

        public static implicit operator IntPtr(HBitmap handle) {
            return handle.value;
        }
        public static explicit operator HBitmap(IntPtr handle) {
            HBitmap result;
            result.value = handle;
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HBitmap A, HBitmap B) {
            return A.value == B.value;
        }
        public static bool operator !=(HBitmap A, HBitmap B) {
            return A.value != B.value;
        }
        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        public BOOL Delete() {
            return Gdi.DeleteObject(this);
        }

        public BOOL GetDimensions(out Windows.Gdi.SIZE result) {
            return Gdi.GetBitmapDimensionEx(this, out result);
        }

        public BOOL SetDimension(int width, int height, out Windows.Gdi.SIZE size) {
            return Gdi.SetBitmapDimensionEx(this, width, height, out size);
        }


    }

    public struct HBrush : IHandle
    {
        private IntPtr value;
        public static readonly HBrush Zero;

        public static implicit operator IntPtr(HBrush handle) {
            return handle.value;
        }
        public static explicit operator HBrush(IntPtr handle) {
            HBrush result;
            result.value = handle;
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HBrush A, HBrush B) {
            return A.value == B.value;
        }
        public static bool operator !=(HBrush A, HBrush B) {
            return A.value != B.value;
        }
        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        public BOOL Delete() {
            return Gdi.DeleteObject(this);
        }
    }
    public struct HFont : IHandle
    {
        private IntPtr value;
        public static readonly HFont Zero;

        public static implicit operator IntPtr(HFont handle) {
            return handle.value;
        }
        public static explicit operator HFont(IntPtr handle) {
            HFont result;
            result.value = handle;
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HFont A, HFont B) {
            return A.value == B.value;
        }
        public static bool operator !=(HFont A, HFont B) {
            return A.value != B.value;
        }
        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

    }
    public struct HPen : IHandle
    {
        private IntPtr value;
        public static readonly HPen Zero;

        public static implicit operator IntPtr(HPen handle) {
            return handle.value;
        }
        public static explicit operator HPen(IntPtr handle) {
            HPen result;
            result.value = handle;
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HPen A, HPen B) {
            return A.value == B.value;
        }
        public static bool operator !=(HPen A, HPen B) {
            return A.value != B.value;
        }
        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

    }
    public struct HPalette : IHandle
    {
        private IntPtr value;
        public static readonly HPalette Zero;

        public static implicit operator IntPtr(HPalette handle) {
            return handle.value;
        }
        public static explicit operator HPalette(IntPtr handle) {
            HPalette result;
            result.value = handle;
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HPalette A, HPalette B) {
            return A.value == B.value;
        }
        public static bool operator !=(HPalette A, HPalette B) {
            return A.value != B.value;
        }
        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        public BOOL Delete() {
            return Gdi.DeleteObject(this);
        }
    }
    public struct HGdiObject : IHandle
    {
        private IntPtr value;
        public static readonly HGdiObject Zero;

        public static implicit operator IntPtr(HGdiObject handle) {
            return handle.value;
        }
        public static implicit operator HGdiObject(HBitmap handle) {
            return (HGdiObject)(IntPtr)handle;
        }
        public static implicit operator HGdiObject(HFont handle) {
            return (HGdiObject)(IntPtr)handle;
        }
        public static implicit operator HGdiObject(HBrush handle) {
            return (HGdiObject)(IntPtr)handle;
        }
        public static implicit operator HGdiObject(HPen handle) {
            return (HGdiObject)(IntPtr)handle;
        }

        public static explicit operator HGdiObject(IntPtr handle) {
            HGdiObject result;
            result.value = handle;
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HGdiObject A, HGdiObject B) {
            return A.value == B.value;
        }
        public static bool operator !=(HGdiObject A, HGdiObject B) {
            return A.value != B.value;
        }
        public static bool operator ==(HGdiObject A, IHandle B) {
            return A.value == B.Value;
        }
        public static bool operator !=(HGdiObject A, IHandle B) {
            return A.value != B.Value;
        }
        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        public HBitmap AsHBitmap() { return (HBitmap)value; }
        public HFont AsHFont() { return (HFont)value; }
        public HPen AsHPen() { return (HPen)value; }
        public HBrush AsHBrush() { return (HBrush)value; }
    }

}

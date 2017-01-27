using System;
using System.Collections.Generic;
using System.Text;
using Windows.Enum;

namespace Windows
{
    public interface IHandle { IntPtr Value { get; set;} }
    public static class Handle
    {
        public static bool Compare<T>(T handle, object comparison) where T: IHandle {
            IntPtr val1 = IntPtr.Zero;
            IntPtr val2 = val1;

            if (handle != null)
                val1 = handle.Value;
            if (comparison != null) {
                if (comparison is IntPtr)
                    val2 = (IntPtr)comparison;
                else if (!(val2 is T))
                    return false; // Different handle types wont be considered equal, even if they points to the same object
                else
                    val2 = ((IHandle)comparison).Value;
            }


            return val1 == val2;
        }
    }
    public struct HWnd:IHandle 
    {
        private IntPtr value;

        public static implicit operator IntPtr(HWnd handle) {
            return handle.value;
        }
        public static explicit operator HWnd(IntPtr handle) {
            HWnd result;
            result.value = handle;
            return result;
        }

        public RECT GetRect() {
            RECT result;
            User32.GetWindowRect(this, out result);
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HWnd A, HWnd B) {
            return A.value == B.value;
        }
        public static bool operator !=(HWnd A, HWnd B) {
            return A.value != B.value;
        }
        public static readonly HWnd Zero;

        public HIcon GetIcon() {
            HIcon result = (HIcon)SendMessage(WindowsMessage.GetIcon, (int)IconSize.IconSmall2, IntPtr.Zero);
            if (result == HIcon.Zero)
                result = (HIcon)SendMessage( WindowsMessage.GetIcon, (int)IconSize.IconSmall, IntPtr.Zero);
            if (result == HIcon.Zero)
                result = (HIcon)GetClassLongPtr( GetClassLongVal.HIconSmall);
            if (result == HIcon.Zero)
                result = (HIcon)SendMessage( WindowsMessage.GetIcon, (int)IconSize.IconBig, IntPtr.Zero);
            if (result == HIcon.Zero)
                result = (HIcon)GetClassLongPtr(GetClassLongVal.HIcon);

            return result;
        }

        public IntPtr GetClassLongPtr(GetClassLongVal index) {
            return User32.GetClassLongPtr(this, index);
        }
        public IntPtr SendMessage(WindowsMessage message, IntPtr wParam, IntPtr lParam) {
            return User32.SendMessage(this, message, wParam, lParam);
        }
        public IntPtr SendMessage(WindowsMessage message, int wParam, int lParam) {
            return User32.SendMessage(this, message, wParam, lParam);
        }
        public IntPtr SendMessage(WindowsMessage message, int wParam, IntPtr lParam) {
            return User32.SendMessage(this, message, wParam, lParam);
        }
        public IntPtr SendMessage(WindowsMessage message, IntPtr wParam, int lParam) {
            return User32.SendMessage(this, message, wParam, lParam);
        }

        public string GetText() {
            int textLen = User32.GetWindowTextLength(this);
            StringBuilder result = new StringBuilder(textLen + 1);
            User32.GetWindowText(this, result, textLen + 1);
            return result.ToString();
        }

        /// <summary>Scrolls the contents of a window.</summary>
        /// <param name="dx">The distance to scroll horizantally.</param>
        /// <param name="dy">The distance to scroll vertically.</param>
        /// <param name="scroll">A rectangle that specifies the portion of the client area to be scrolled, or null.</param>
        /// <param name="clip">A clipping region for the graphic output, or null.</param>
        /// <param name="update">An HRgn that will be updated to represent the invalid region.</param>
        /// <param name="updateRect">A RECT that will be updated to represent the invalid area.</param>
        /// <param name="flags">Flags to pass to the function.</param>
        /// <returns>The type of region stored to the Update parameter.</returns>
        public RegionType ScrollWindow(int dx, int dy, ref RECT scroll, ref RECT clip, HRgn update, out RECT updateRect, Enum.ShowWindow flags) {
            return User32.ScrollWindowEx(this, dx, dy, ref scroll, ref clip, update, out updateRect, flags);
        }
        /// <summary>Scrolls the contents of a window.</summary>
        /// <param name="dx">The distance to scroll horizantally.</param>
        /// <param name="dy">The distance to scroll vertically.</param>
        /// <param name="update">An HRgn that will be updated to represent the invalid region.</param>
        /// <param name="updateRect">A RECT that will be updated to represent the invalid area.</param>
        /// <param name="flags">Flags to pass to the function.</param>
        /// <returns>The type of region stored to the Update parameter.</returns>
        public RegionType ScrollWindow(int dx, int dy, HRgn update, out RECT updateRect, Enum.ShowWindow flags) {
            return User32.ScrollWindowEx(this, dx, dy, IntPtr.Zero, IntPtr.Zero, update, out updateRect, flags);
        }
        /// <summary>Scrolls the contents of a window.</summary>
        /// <param name="dx">The distance to scroll horizantally.</param>
        /// <param name="dy">The distance to scroll vertically.</param>
        /// <param name="update">An HRgn that will be updated to represent the invalid region.</param>
        /// <param name="flags">Flags to pass to the function.</param>
        /// <returns>The type of region stored to the Update parameter.</returns>
        public RegionType ScrollWindow(int dx, int dy, HRgn update,Enum.ShowWindow flags) {
            return User32.ScrollWindowEx(this, dx, dy, IntPtr.Zero, IntPtr.Zero, update, IntPtr.Zero, flags);
        }

        /// <summary>Scrolls the contents of a window.</summary>
        /// <param name="dx">The distance to scroll horizantally.</param>
        /// <param name="dy">The distance to scroll vertically.</param>
        /// <param name="Update">An HRgn that will be updated to represent the invalid region.</param>
        /// <param name="flags">Flags to pass to the function.</param>
        /// <returns>The type of region stored to the Update parameter.</returns>
        public RegionType ScrollWindow(int dx, int dy, out RECT update, Enum.ShowWindow flags) {
            return User32.ScrollWindowEx(this, dx, dy, IntPtr.Zero, IntPtr.Zero, HRgn.Zero, out update, flags);
        }

        #region IHandle Members

        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        #endregion
    }

    public struct HIcon : IHandle
    {
        private IntPtr value;

        public static implicit operator IntPtr(HIcon handle) {
            return handle.value;
        }
        public static explicit operator HIcon(IntPtr handle) {
            HIcon result;
            result.value = handle;
            return result;
        }

        public static readonly HIcon Zero;

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }
        public static bool operator ==(HIcon A, IntPtr B) {
            return A.value == B;
        }

        public static bool operator !=(HIcon A, IntPtr B) {
            return A.value != B;
        }
        public static bool operator ==(HIcon A, HIcon B) {
            return A.value == B.value;
        }

        public static bool operator !=(HIcon A, HIcon B) {
            return A.value != B.value;
        }

        #region IHandle Members

        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        #endregion
    }
    public struct HInstance : IHandle
    {
        private IntPtr value;

        public static implicit operator IntPtr(HInstance handle) {
            return handle.value;
        }
        public static explicit operator HInstance(IntPtr handle) {
            HInstance result;
            result.value = handle;
            return result;
        }

        public static readonly HInstance Zero;

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }
        public static bool operator ==(HInstance A, HInstance B) {
            return A.value == B.value;
        }

        public static bool operator !=(HInstance A, HInstance B) {
            return A.value != B.value;
        }

        #region IHandle Members

        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        #endregion
    }
}

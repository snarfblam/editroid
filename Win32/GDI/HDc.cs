using System;
using System.Collections.Generic;
using System.Text;
using Windows.Enum;
using POINT = Windows.Gdi.POINT;
using SIZE = Windows.Gdi.SIZE;

namespace Windows
{
    public struct HDc : IHandle
    {
        private IntPtr value;
        public static readonly HDc Zero;

        public static implicit operator IntPtr(HDc handle) {
            return handle.value;
        }
        public static explicit operator HDc(IntPtr handle) {
            HDc result;
            result.value = handle;
            return result;
        }

        public override bool Equals(object obj) {
            return Handle.Compare(this, obj);
        }

        public static bool operator ==(HDc A, HDc B) {
            return A.value == B.value;
        }
        public static bool operator !=(HDc A, HDc B) {
            return A.value != B.value;
        }
        IntPtr IHandle.Value {
            get { return value; }
            set { this.value = value; }
        }

        public void SelectBitmap(HBitmap bitmap, out HBitmap replacedBitmap) {
            replacedBitmap = Gdi.SelectObject(this, bitmap).AsHBitmap();
        }
        public void SelectBrush(HBrush brush, out HBrush replacedBrush) {
            replacedBrush = Gdi.SelectObject(this, brush).AsHBrush();
        }
        public void SelectPen(HPen pen, out HPen replacedPen) {
            replacedPen = Gdi.SelectObject(this, pen).AsHPen();
        }
        public void SelectFont(HFont font, out HFont replacedFont) {
            replacedFont = Gdi.SelectObject(this, font).AsHFont();
        }
        public void SelectObject(HGdiObject obj, out HGdiObject replacedObj) {
            replacedObj = Gdi.SelectObject(this, obj);
            HDc farts;
        }
        public HBitmap CreateCompatibleBitmap(int width, int height) {
            return Gdi.CreateCompatibleBitmap(this, width, height);
        }

        public BOOL BitBlt(RECT destRect, HDc source, int srcX, int srcY, RasterOperation blend) {
            System.Drawing.Rectangle r = destRect;
            return BitBlt(r.X, r.Y, r.Width, r.Height, source, srcX, srcY, blend);
        }
        public BOOL BitBlt(int x, int y, int width, int height, HDc source, int srcX, int srcY, RasterOperation blend) {
            return Gdi.BitBlt(this, x, y, width, height, source, srcX, srcY, blend);
        }
        public BOOL BitBlt(RECT destRect, HDc source, int srcX, int srcY) {
            System.Drawing.Rectangle r = destRect;
            return BitBlt(r.X, r.Y, r.Width, r.Height, source, srcX, srcY, RasterOperation.SourceCopy);
        }
        public BOOL BitBlt(int x, int y, int width, int height, HDc source, int srcX, int srcY) {
            return Gdi.BitBlt(this, x, y, width, height, source, srcX, srcY, RasterOperation.SourceCopy);
        }


        public BOOL StretchBlt(RECT destRect, HDc source, RECT srcRect, RasterOperation blend) {
            System.Drawing.Rectangle d = destRect;
            System.Drawing.Rectangle s = destRect;
            return StretchBlt(d.X, d.Y, d.Width, d.Height, source, s.X, s.Y, s.Width, s.Height, blend);
        }
        public BOOL StretchBlt(int x, int y, int width, int height, HDc source, int srcX, int srcY, int srcWidth, int srcHeight, RasterOperation blend) {
            return Gdi.StretchBlt(this, x, y, width, height, source, srcX, srcY, srcWidth, srcHeight, blend);
        }
        public BOOL StretchBlt(RECT destRect, HDc source, int srcX, int srcY) {
            System.Drawing.Rectangle d = destRect;
            System.Drawing.Rectangle s = destRect;
            return StretchBlt(d.X, d.Y, d.Width, d.Height, source, s.X, s.Y, s.Width, s.Height, RasterOperation.SourceCopy);
        }
        public BOOL StretchBlt(int x, int y, int width, int height, HDc source, int srcX, int srcY, int srcWidth, int srcHeight) {
            return Gdi.StretchBlt(this, x, y, width, height, source, srcX, srcY, srcWidth, srcHeight, RasterOperation.SourceCopy);
        }

        public BOOL MaskBlt(int destX, int destY, int width, int height, HDc source, int srcX, int srcY, HBitmap mask, int maskX, int maskY, Windows.Enum.RasterOperation foreblend, Windows.Enum.RasterOperation backblend) {
            return Gdi.MaskBlt(this, destX, destY, width, height, source, srcX, srcY, mask, maskX, maskY, Gdi.CombineForMskBlt(foreblend, backblend));
        }
        public BOOL MaskBlt(RECT destRect, HDc source, int srcX, int srcY, HBitmap mask, int maskX, int maskY) {
            System.Drawing.Rectangle d = destRect;
            return Gdi.MaskBlt(this, d.X, d.Y, d.Width, d.Height, source, srcX, srcY, mask, maskX, maskY, (RasterOperation)(0xCCAA0020));
        }

        public void SetStretchMode(StretchMode newMode, out StretchMode oldMode) {
            oldMode = Gdi.SetStretchBltMode(this, newMode);
        }
        public StretchMode GetStretchBltMode() {
            return Gdi.GetStretchBltMode(this);
        }

        public BOOL FloodFill(int startX, int startY, Windows.Gdi.ColorRef boundsColor, FloodFill fillType) {
            return Gdi.ExtFloodFill(this, startX, startY, boundsColor, fillType);
        }

        public Windows.Gdi.ColorRef GetPixel(int x, int y) {
            return Gdi.GetPixel(this, x, y);
        }

        public Windows.Gdi.ColorRef SetPixel(int x, int y, Windows.Gdi.ColorRef color) {
            return Gdi.SetPixel(this, x, y, color);
        }
        public BOOL SetPixelV(int x, int y, Windows.Gdi.ColorRef color) {
            return Gdi.SetPixelV(this, x, y, color);
        }

        public BOOL GetBrushOrigin(out POINT result) {
            return Gdi.GetBrushOrgEx(this, out result);
        }

        public BOOL SetBrushOrigin(int x, int y, ref Windows.Gdi.POINT previousOrigin) {
            return Gdi.SetBrushOrgEx(this, x, y, ref previousOrigin);
        }
        public BOOL SetBrushOrigin(int x, int y) {
            Gdi.POINT ignored = new POINT();
            return Gdi.SetBrushOrgEx(this, x, y, ref ignored);
        }
        public Windows.Gdi.ColorRef SetBrushColor(Windows.Gdi.ColorRef color) {
            return Gdi.SetDCBrushColor(this, color);
        }

        public RegionType ExcludeClipRect(int left, int top, int right, int bottom) {
            return Gdi.ExcludeClipRect(this, left, top, right, bottom);
        }
        public RegionType SelectClipRegion(HRgn region, RegionCombination operation) {
            return Gdi.ExtSelectClipRgn(this, region, operation);
        }
        public RegionType GetClipBox(out RECT result) {
            return Gdi.GetClipBox(this, out result);
        }
        public RegionResult GetClipRegoin(HRgn region) {
            return Gdi.GetClipRgn(this, region);
        }
        public RegionResult GetClipRegion(HRgn region) {
            return Gdi.GetClipRgn(this, region);
        }

        public RegionType IntersectClipRect(int left, int top, int right, int bottom) {
            return Gdi.IntersectClipRect(this, left, top, right, bottom);
        }
        public RegionType OffsetClipRegion(int x, int y) {
            return Gdi.OffsetClipRgn(this, x, y);
        }
        public BOOL PointVisible(int x, int y) {
            return Gdi.PtVisible(this, x, y);
        }
        public BOOL RectVisible(ref RECT rect) {
            return Gdi.RectVisible(this, ref rect);
        }
        public BOOL SelectClipPath(RegionCombination mode) {
            return Gdi.SelectClipPath(this, mode);
        }
        public RegionType SelectClipRegion(HRgn region) {
            return Gdi.SelectClipRgn(this, region);
        }
        public HPalette CreateHalftonePalette() {
            return Gdi.CreateHalftonePalette(this);
        }



        public BOOL GetColorAdjustment(out Windows.Gdi.ColorAdjustment output) {
            return Gdi.GetColorAdjustment(this, out output);
        }
        public Windows.Gdi.ColorRef GetNearestColor(Windows.Gdi.ColorRef color) {
            return Gdi.GetNearestColor(this, color);
        }
        public SysPal GetSystemPaletteUse() {
            return Gdi.GetSystemPaletteUse(this);
        }
        public uint RealizePalette() {
            return Gdi.RealizePalette(this);
        }
        public HPalette SelectPalette(HPalette palette, BOOL forceBackground) {
            return Gdi.SelectPalette(this, palette, forceBackground);
        }
        public BOOL UpdateColors() {
            return Gdi.UpdateColors(this);
        }
        public BOOL GetCurrentPosition(out POINT result) {
            return Gdi.GetCurrentPositionEx(this, out result);
        }
        public Windows.Gdi.POINT GetCurrentPosition() {
            Windows.Gdi.POINT result;
            Gdi.GetCurrentPositionEx(this, out result);
            return result;
        }
        public GraphicsMode GetGraphicsMode() {
            return Gdi.GetGraphicsMode(this);
        }
        public BOOL GetViewportExtent(out Windows.Gdi.SIZE result) {
            return Gdi.GetViewportExtEx(this, out result);
        }
        public Windows.Gdi.SIZE GetViewportExtent() {
            Windows.Gdi.SIZE result;
            Gdi.GetViewportExtEx(this, out result);
            return result;
        }
        public BOOL GetViewportOrigin(out Windows.Gdi.POINT result) {
            return Gdi.GetViewportOrgEx(this, out result);
        }
        public Windows.Gdi.POINT GetViewportOrigin() {
            Windows.Gdi.POINT result;
            Gdi.GetViewportOrgEx(this, out result);
            return result;
        }
        public BOOL GetWindowExtent(out Windows.Gdi.SIZE result) {
            return Gdi.GetWindowExtEx(this, out result);
        }
        public SIZE GetWindowExtent() {
            SIZE result;
            Gdi.GetWindowExtEx(this, out result);
            return result;
        }
        public BOOL GetWindowOrigin(out POINT result) {
            return Gdi.GetWindowOrgEx(this, out result);
        }
        public POINT GetWindowOrigin() {
            POINT result;
            Gdi.GetWindowOrgEx(this, out result);
            return result;
        }
        public BOOL OffsetViewportOrigin(int x, int y, out POINT lastPoint) {
            return Gdi.OffsetViewportOrgEx(this, x, y, out lastPoint);
        }
        public BOOL OffsetViewportOrigin(int x, int y) {
            return Gdi.OffsetViewportOrgEx(this, x, y, IntPtr.Zero);
        }
        public BOOL MoveTo(int x, int y, out POINT lastPoint) {
            return Gdi.MoveToEx(this, x, y, out lastPoint);
        }
        public BOOL MoveTo(int x, int y) {
            return Gdi.MoveToEx(this, x, y, IntPtr.Zero);
        }
        public BOOL OffsetWindowOrigin(int x, int y, out POINT lastPoint) {
            return Gdi.OffsetWindowOrgEx(this, x, y, out lastPoint);
        }
        public BOOL OffsetWindowOrigin(int x, int y) {
            return Gdi.OffsetWindowOrgEx(this, x, y, IntPtr.Zero);
        }
        public BOOL ScaleViewportExtent(int xNumerator, int xDenominator, int yNumerator, int yDenominator, out SIZE lastScale) {
            return Gdi.ScaleViewportExtEx(this, xNumerator, xDenominator, yNumerator, yDenominator, out lastScale);
        }
        public BOOL ScaleViewportExtent(int xNumerator, int xDenominator, int yNumerator, int yDenominator) {
            return Gdi.ScaleViewportExtEx(this, xNumerator, xDenominator, yNumerator, yDenominator, IntPtr.Zero);
        }
        public BOOL ScaleViewportExtent(float xScale, float yScale, out SIZE lastScale) {
            int accuracy = 0x4000;
            int xNum = (int)(xScale * accuracy);
            int yNum = (int)(yScale * accuracy);

            return Gdi.ScaleViewportExtEx(this, xNum, accuracy, yNum, accuracy, out lastScale);
        }
        public BOOL ScaleViewportExtent(float xScale, float yScale) {
            int accuracy = 0x4000;
            int xNum = (int)(xScale * accuracy);
            int yNum = (int)(yScale * accuracy);

            return Gdi.ScaleViewportExtEx(this, xNum, accuracy, yNum, accuracy, IntPtr.Zero);
        }
        public GraphicsMode SetGraphicsMode(GraphicsMode iMode) {
            return Gdi.SetGraphicsMode(this, iMode);
        }
        public MappingMode SetMapMode(MappingMode mode) {
            return Gdi.SetMapMode(this, mode);
        }
        public BOOL SetViewportExtent(int x, int y, out SIZE oldSize) {
            return Gdi.SetViewportExtEx(this, x, y, out oldSize);
        }
        public BOOL SetViewportExtent(int x, int y) {
            return Gdi.SetViewportExtEx(this, x, y, IntPtr.Zero);
        }
        public BOOL SetViewportOrigin(int x, int y, out POINT oldSize) {
            return Gdi.SetViewportOrgEx(this, x, y, out oldSize);
        }
        public BOOL SetViewportOrigin(int x, int y) {
            return Gdi.SetViewportOrgEx(this, x, y, IntPtr.Zero);
        }
        public BOOL SetWindowExtent(int x, int y, out SIZE oldSize) {
            return Gdi.SetWindowExtEx(this, x, y, out oldSize);
        }
        public BOOL SetWindowExtent(int x, int y) {
            return Gdi.SetWindowExtEx(this, x, y, IntPtr.Zero);
        }
        public BOOL SetWindowOrigin(int x, int y, out POINT oldPoint) {
            return Gdi.SetWindowOrgEx(this, x, y, out oldPoint);
        }
        public BOOL SetWindowOrigin(int x, int y) {
            return Gdi.SetWindowOrgEx(this, x, y, IntPtr.Zero);
        }





        public BOOL Release(HWnd window) {
            return User32.ReleaseDC(window, this);
        }
        public HDc(HWnd window) {
            value = User32.GetDC(window).value;
        }
        public HDc(HWnd window, bool entireWindow) {
            if (entireWindow) {
                value = User32.GetWindowDC(window).value;
            } else {
                value = User32.GetDC(window).value;
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Windows.Enum;

namespace Windows
{
    public  static partial class Gdi
    {
        /// <summary>
        /// Represents an error code for a GDI color function return value.
        /// </summary>
        public const uint ClrInvalid = 0xFFFF;
        /// <summary>
        /// Represents an error code for a GDI function return value.
        /// </summary>
        public const uint GdiError = 0xFFFF;


        [DllImport("gdi32.dll")]
        public static extern HRgn CreateRectRgn(int left, int top, int right, int bottom);
        public static HRgn CreateRectRgn(RECT r) {
            return CreateRectRgn(r.Left, r.Top, r.Right, r.Bottom);
        }

        [DllImport("gdi32.dll")]
        public static extern BOOL DeleteObject(IntPtr hObject);


        [DllImport("gdi32.dll")]
        public static extern uint GetRegionData(HRgn region, int bufferSize, out RegionData lpRgnData);
        [DllImport("gdi32.dll")]
        public static extern uint GetRegionData(int dataBuferSize, HRgn region, int bufferSize, out Windows.Gdi.RegionData lpRgnData);
        [DllImport("gdi32.dll", EntryPoint = "GetRegionData")]
        public static extern uint GetRegionDataSize(HRgn region, int specifyZero, int specifyZeroAgain);

        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateCompatibleBitmap(HDc hdc, int width, int height);
        [DllImport("gdi32.dll")]
        public static extern HDc CreateCompatibleDC(HDc hdc);
        [DllImport("gdi32.dll")]
        public static extern HGdiObject SelectObject(HDc deviceContext, HGdiObject obj);

        [DllImport("gdi32.dll")]
        public static extern Windows.Enum.RegionType SelectObject(HDc deviceContext, HRgn obj);
        [DllImport("gdi32.dll")]
        public static extern BOOL BitBlt(HDc dest, int x, int y, int nWidth, int nHeight, HDc src, int xSrc, int ySrc, Windows.Enum.RasterOperation dwRop);
        [DllImport("gdi32.dll")]
        public static extern BOOL StretchBlt(HDc dest, int destX, int destY, int destWidth, int destHeight, HDc source, int srcX, int srcY, int srcWidth, int srcHeight, Windows.Enum.RasterOperation dwRop);
        [DllImport("gdi32.dll")]
        public static extern BOOL MaskBlt(HDc dest, int destX, int destY, int width, int height, HDc source, int srcX, int srcY, HBitmap mask, int maskX, int maskY, Windows.Enum.RasterOperation dwRop);
        [DllImport("gdi32.dll")]
        public static extern BOOL PlgBlt(HDc dest, ref Gdi.PlgBltPoints destParallelogram, HDc src, int srcX, int srcY, int width, int height, HBitmap mask, int maskX, int maskY);

        [DllImport("gdi32.dll")]
        public static extern StretchMode SetStretchBltMode(HDc dc, StretchMode nStretchMode);
        [DllImport("gdi32.dll", EntryPoint = "GetObjectA")]
        public static extern StretchMode GetStretchBltMode(HDc dc);

        [DllImport("gdi32.dll")]
        public static extern int SetDIBitsToDevice(HDc dc, int destX, int destY, int width, int height, int srcX, int srcY, int startScan, int scanCount, byte[] data, ref BitmapInfo bitmapInfo, BitmapColorUsage wUsage);
        [DllImport("gdi32.dll")]
        public static extern int SetDIBitsToDevice(HDc dc, int destX, int destY, int width, int height, int srcX, int srcY, int startScan, int scanCount, byte[] data, IntPtr bitmapInfo, BitmapColorUsage wUsage);

        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateBitmapIndirect(ref Bitmap bitmapData);
        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateBitmapIndirect(IntPtr bitmapDataPointer);

        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateDIBitmap(HDc dc, ref BitmapInfoHeader header, ContextBitmap init, [MarshalAs(UnmanagedType.LPArray)] byte[] initialData, ref BitmapInfo lpInitInfo, BitmapColorUsage usage);
        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateDIBitmap(HDc dc, ref BitmapInfoHeaderV4 header, ContextBitmap init, [MarshalAs(UnmanagedType.LPArray)] byte[] initialData, ref BitmapInfo4 lpInitInfo, BitmapColorUsage usage);
        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateDIBitmap(HDc dc, ref BitmapInfoHeaderV5 header, ContextBitmap init, [MarshalAs(UnmanagedType.LPArray)] byte[] initialData, ref BitmapInfo5 lpInitInfo, BitmapColorUsage usage);
        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateDIBitmap(HDc dc, IntPtr header, ContextBitmap init, IntPtr initialData, IntPtr lpInitInfo, BitmapColorUsage usage);
        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateDIBitmap(HDc dc, [MarshalAs(UnmanagedType.LPArray)] byte[] headerBytes, ContextBitmap init, IntPtr initialData, IntPtr lpInitInfo, BitmapColorUsage usage);

        [DllImport("gdi32.dll")]
        public static extern HBitmap CreateDIBSection(HDc dc, ref BitmapInfo gnfo, BitmapColorUsage colorUsage, ref IntPtr pointerToDataPointer, IntPtr hSection, uint bitmapDataOffsetInHSection);


        [DllImport("gdi32.dll")]
        public static extern BOOL ExtFloodFill(HDc dc, int startX, int startY, ColorRef boundsColor, FloodFill fillType);
        [DllImport("gdi32.dll")]
        public static extern BOOL GetBitmapDimensionEx(HBitmap bitmap, out SIZE result);
        [DllImport("gdi32.dll")]
        public static extern uint GetDIBColorTable(HDc dc, uint startIndex, uint entryCount, [MarshalAs(UnmanagedType.LPArray), Out]RgbQuad[] output);
        /// <summary>Retrieves the bits of the specified compatible bitmap and copies them into a buffer as a DIB using the specified format.</summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="bitmap">Handle to the bitmap. This must be a compatible bitmap (DDB).</param>
        /// <param name="startScan">Specifies the first scan line to retrieve.</param>
        /// <param name="scanCount">Specifies the number of scan lines to retrieve.</param>
        /// <param name="outputBuffer">Pointer to a buffer to receive the bitmap data, or null.</param>
        /// <param name="info">Specifies the desired format for the DIB data.</param>
        /// <param name="usage">Specifies the format of the Colors member of the BitmapInfo structure.</param>
        /// <returns>If the outputBuffer parameter is non-NULL and the function succeeds, the return value is the number of scan lines copied from the bitmap.</returns>
        [DllImport("gdi32.dll")]
        public static extern int GetDIBits(HDc dc, HBitmap bitmap, int startScan, int scanCount, [MarshalAs(UnmanagedType.LPArray), Out]byte[] outputBuffer, ref BitmapInfo info, BitmapColorUsage usage);
        /// <summary>Retrieves the bits of the specified compatible bitmap and copies them into a buffer as a DIB using the specified format.</summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="bitmap">Handle to the bitmap. This must be a compatible bitmap (DDB).</param>
        /// <param name="startScan">Specifies the first scan line to retrieve.</param>
        /// <param name="scanCount">Specifies the number of scan lines to retrieve.</param>
        /// <param name="outputBuffer">Pointer to a buffer to receive the bitmap data, or null.</param>
        /// <param name="infoPointer">Specifies the desired format for the DIB data.</param>
        /// <param name="usage">Specifies the format of the Colors member of the BitmapInfo structure.</param>
        /// <returns>If the outputBuffer parameter is non-NULL and the function succeeds, the return value is the number of scan lines copied from the bitmap.</returns>
        [DllImport("gdi32.dll")]
        public static extern int GetDIBits(HDc dc, HBitmap bitmap, int startScan, int scanCount, [MarshalAs(UnmanagedType.LPArray), Out]byte[] outputBuffer, IntPtr infoPointer, BitmapColorUsage usage);
        /// <summary>Retrieves the bits of the specified compatible bitmap and copies them into a buffer as a DIB using the specified format.</summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="bitmap">Handle to the bitmap. This must be a compatible bitmap (DDB).</param>
        /// <param name="startScan">Specifies the first scan line to retrieve.</param>
        /// <param name="scanCount">Specifies the number of scan lines to retrieve.</param>
        /// <param name="outputBufferPointer">Pointer to a buffer to receive the bitmap data, or null.</param>
        /// <param name="infoPointer">Specifies the desired format for the DIB data.</param>
        /// <param name="usage">Specifies the format of the Colors member of the BitmapInfo structure.</param>
        /// <returns>If the outputBufferPointer parameter is non-NULL and the function succeeds, the return value is the number of scan lines copied from the bitmap.</returns>
        [DllImport("gdi32.dll")]
        public static extern int GetDIBits(HDc dc, HBitmap bitmap, int startScan, int scanCount, IntPtr outputBufferPointer, IntPtr infoPointer, BitmapColorUsage usage);

        /// <summary>Retrieves the red, green, blue (RGB) color value of the pixel at the specified coordinates.</summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">The x-coordinate, in logical units, of the pixel.</param>
        /// <param name="y">The y-coordinate, in logical units, of the pixel.</param>
        /// <returns>RGB value of the pixel.</returns>
        [DllImport("gdi32.dll")]
        public static extern ColorRef GetPixel(HDc dc, int x, int y);

        /// <summary>
        /// Assigns preferred dimensions to a bitmap. These dimensions can be used by applications; however, they are not used by the system.
        /// </summary>
        /// <param name="bitmap">Handle to the bitmap. The bitmap cannot be a DIB-section bitmap.</param>
        /// <param name="x">Specifies the x, in 0.1-millimeter units, of the bitmap.</param>
        /// <param name="height">Specifies the height, in 0.1-millimeter units, of the bitmap.</param>
        /// <param name="size">Pointer to a SIZE structure to receive the previous dimensions of the bitmap. This pointer can be NULL.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetBitmapDimensionEx(HBitmap bitmap, int width, int height, out SIZE size);

        
        /// <summary>
        /// Sets RGB color values in a range of entries in the color table of the DIB that is currently selected into a specified device context.
        /// </summary>
        /// <param name="dc">Specifies a device context. A DIB must be selected into this device context.</param>
        /// <param name="startIndex">A zero-based color table index that specifies the first color table entry to set.</param>
        /// <param name="entryCount">Specifies the number of color table entries to set.</param>
        /// <param name="entries">Array of RgbQuad structures containing new color information for the DIB's color table.</param>
        /// <returns>If the function succeeds, the return value is the number of color table entries that the function sets.</returns>
        [DllImport("gdi32.dll")]
        public static extern uint SetDIBColorTable(HDc dc, int startIndex, int entryCount, [MarshalAs(UnmanagedType.LPArray)] RgbQuad[] entries);

        /// <summary>
        /// Sets the pixels in a compatible bitmap (DDB) using the color data found in the specified DIB.
        /// </summary>
        /// <param name="dc">Handle to a device context.</param>
        /// <param name="bitmap">Handle to the compatible bitmap (DDB) that is to be altered using the color data from the specified DIB.</param>
        /// <param name="nStartScan">Specifies the starting scan line for the device-independent color data in the array pointed to by the lpvBits parameter.</param>
        /// <param name="scanCount">Specifies the number of scan lines found in the array containing device-independent color data.</param>
        /// <param name="data">DIB color data, stored as an array of bytes.</param>
        /// <param name="info">Contains information about the DIB.</param>
        /// <param name="usage">Specifies color format.</param>
        /// <returns>If the function succeeds, the return value is the number of scan lines copied.</returns>
        [DllImport("gdi32.dll")]
        public static extern int SetDIBits(HDc dc, HBitmap bitmap, int nStartScan, int scanCount, [MarshalAs(UnmanagedType.LPArray)] byte[] data, ref BitmapInfo info, BitmapColorUsage usage);

        /// <summary>
        /// Sets the pixels in a compatible bitmap (DDB) using the color data found in the specified DIB.
        /// </summary>
        /// <param name="dc">Handle to a device context.</param>
        /// <param name="bitmap">Handle to the compatible bitmap (DDB) that is to be altered using the color data from the specified DIB.</param>
        /// <param name="nStartScan">Specifies the starting scan line for the device-independent color data in the array pointed to by the lpvBits parameter.</param>
        /// <param name="scanCount">Specifies the number of scan lines found in the array containing device-independent color data.</param>
        /// <param name="data">DIB color data, stored as an array of bytes.</param>
        /// <param name="infoPointer">Contains information about the DIB.</param>
        /// <param name="usage">Specifies color format.</param>
        /// <returns>If the function succeeds, the return value is the number of scan lines copied.</returns>
        [DllImport("gdi32.dll")]
        public static extern int SetDIBits(HDc dc, HBitmap bitmap, int nStartScan, int scanCount, [MarshalAs(UnmanagedType.LPArray)] byte[] data, IntPtr infoPointer, BitmapColorUsage usage);

        [DllImport("gdi32.dll")]
        public static extern int SetDIBitsToDevice(HDc dc, int destX, int destY, uint destWidth, uint destHeight, int srcX, int srcY, int startScan, int scanCount, [MarshalAs(UnmanagedType.LPArray)] byte[] data, ref BitmapInfo info, BitmapColorUsage usage);
        [DllImport("gdi32.dll")]
        public static extern int SetDIBitsToDevice(HDc dc, int destX, int destY, uint destWidth, uint destHeight, int srcX, int srcY, int startScan, int scanCount, [MarshalAs(UnmanagedType.LPArray)] byte[] data, IntPtr infoPointer, BitmapColorUsage usage);

        /// <summary>
        /// Sets the pixel at the specified coordinates to the specified color.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the x-coordinate, in logical units, of the points to be set.</param>
        /// <param name="y">Specifies the y-coordinate, in logical units, of the points to be set.</param>
        /// <param name="color">Specifies the color to be used to paint the points. </param>
        /// <returns>If the function succeeds, the return value is the RGB value that the function sets the pixel to. This value may differ from the color specified by crColor; that occurs when an exact match for the specified color cannot be found.
        /// If the function fails, the return value is -1.</returns>
        [DllImport("gdi32.dll")]
        public static extern ColorRef SetPixel(HDc dc, int x, int y, ColorRef color);

        /// <summary>
        /// Sets the pixel at the specified coordinates to the closest approximation of the specified color.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the x-coordinate, in logical units, of the points to be set.</param>
        /// <param name="y">Specifies the y-coordinate, in logical units, of the points to be set.</param>
        /// <param name="color">Specifies the color to be used to paint the points. </param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetPixelV(HDc dc, int x, int y, ColorRef color);

        /// <summary>
        /// Creates a logical brush that has the specified style, color, and pattern.
        /// </summary>
        /// <param name="info">LogBrush structure that contains information about the brush.</param>
        /// <returns>Handle to the brush, or null if the function fails.</returns>
        [DllImport("gdi32.dll")]
        public static extern HBrush CreateBrushIndirect(ref LogBrush info);

        /// <summary>
        /// Creates a logical brush that has the pattern specified by the DIB.
        /// </summary>
        /// <param name="packedDib">Pointer to a packed DIB.</param>
        /// <param name="usage">Specifies whether the color table of the bitmap info structure contains a valid color table and, if so, whether the entries in this color table contain RGB values or palette indecies.</param>
        /// <returns>Handle to the brush, or null if the function fails.</returns>
        [DllImport("gdi32.dll")]
        public static extern HBrush CreateDIBPatternBrushPt(IntPtr packedDib, BitmapColorUsage usage);

        /// <summary>
        /// Creates a logical brush that has the specified hatch pattern and color.
        /// </summary>
        /// <param name="style">Specifies the hatch style of the brush.</param>
        /// <param name="forecolor">Specifies the foreground color of the brush that is used for the hatches.</param>
        /// <returns>Handle to the brush, or null if the function fails.</returns>
        [DllImport("gdi32.dll")]
        public static extern HBrush CreateHatchBrush(HatchStyle style, ColorRef forecolor);

        /// <summary>
        /// Creates a logical brush with the specified bitmap pattern.
        /// </summary>
        /// <param name="pattern">Pattern to use. Can be a DIB section bitmap or a device-dependent bitmap.</param>
        /// <returns>Handle to the brush, or null if the function fails.</returns>
        [DllImport("gdi32.dll")]
        public static extern HBrush CreatePatternBrush(HBitmap pattern);

        /// <summary>
        /// Creates a logical brush that has the specified solid color.
        /// </summary>
        /// <param name="color">Specifies the color of the brush.</param>
        /// <returns>Handle to the brush, or null if the function fails.</returns>
        [DllImport("gdi32.dll")]
        public static extern HBrush CreateSolidBrush(ColorRef color);

        /// <summary>
        /// Retrieves the current brush origin for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="result">Pointer to a POINT structure that receives the brush origin, in device coordinates.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetBrushOrgEx(HDc dc, out POINT result);

        /// <summary>
        /// Retrieves the current color of the specified display element.
        /// </summary>
        /// <param name="id">The display element whose color is to be retrieved.</param>
        /// <returns>RGB color value of the given element.</returns>
        [DllImport("user32.dll")]
        public static extern ColorRef GetSysColor(SystemColor id);

        /// <summary>
        /// Retrieves a handle identifying a logical brush that corresponds to the specified color index.
        /// </summary>
        /// <param name="id">The display element whose color is to be retrieved.</param>
        /// <returns>Handle to the brush, or null if the function fails.</returns>
        [DllImport("user32.dll")]
        public static extern HBrush GetSysColorBrush(SystemColor id);

        /// <summary>
        /// paints the specified rectangle using the brush that is currently selected into the device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the x-coordinate, in logical units, of the upper-left corner of the rectangle to be filled.</param>
        /// <param name="y">Specifies the y-coordinate, in logical units, of the upper-left corner of the rectangle to be filled.</param>
        /// <param name="width">Specifies the width, in logical units, of the rectangle.</param>
        /// <param name="height">Specifies the height, in logical units, of the rectangle.</param>
        /// <param name="op">Specifies the raster operation code.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL PatBlt(HDc dc, int x, int y, int width, int height, RasterOperation op);

        /// <summary>
        /// Sets the brush origin that GDI assigns to the next brush an application selects into the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the x-coordinate, in device units, of the new brush origin.</param>
        /// <param name="y">Specifies the y-coordinate, in device units, of the new brush origin.</param>
        /// <param name="previousOrigin">POINT structure that receives the previous brush origin.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetBrushOrgEx(HDc dc, int x, int y, ref POINT previousOrigin);

        /// <summary>
        /// Sets the brush origin that GDI assigns to the next brush an application selects into the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the x-coordinate, in device units, of the new brush origin.</param>
        /// <param name="y">Specifies the y-coordinate, in device units, of the new brush origin.</param>
        /// <param name="specifyNull">Pointer to a POINT structure that receives the previous brush origin, or null to discard this value.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetBrushOrgEx(HDc dc, int x, int y, IntPtr pointerForResultBuffer);

        /// <summary>
        /// Sets the current device context (DC) brush color to the specified color value.
        /// </summary>
        /// <param name="dc">Handle to the DC.</param>
        /// <param name="color">Specifies the new brush color.</param>
        /// <returns>If the function succeeds, the return value specifies the previous DC brush color as a COLORREF value. If the function fails, the return value is 65535, or 0x0000FFFF.</returns>
        [DllImport("gdi32.dll")]
        public static extern ColorRef SetDCBrushColor(HDc dc, ColorRef color);

        /// <summary>
        /// Creates a new clipping region that consists of the existing clipping region minus the specified rectangle.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="left">Specifies the x-coordinate, in logical units, of the upper-left corner of the rectangle.</param>
        /// <param name="top">Specifies the y-coordinate, in logical units, of the upper-left corner of the rectangle.</param>
        /// <param name="right">Specifies the x-coordinate, in logical units, of the lower-right corner of the rectangle.</param>
        /// <param name="bottom">Specifies the y-coordinate, in logical units, of the lower-right corner of the rectangle.</param>
        /// <returns>The resultant region type, or zero if no region was created.</returns>
        /// <remarks>The lower and right edges of the specified rectangle are not excluded from the clipping region.</remarks>
        [DllImport("gdi32.dll")]
        public static extern RegionType ExcludeClipRect(HDc dc, int left, int top, int right, int bottom);

        /// <summary>
        /// Combines the specified region with the current clipping region using the specified mode.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="region">Handle to the region to be selected. This handle can only be null when RegionCombination.Copy is specified.</param>
        /// <param name="operation">Specifies the operation to be performed.</param>
        /// <returns>The return value specifies the new clipping region's complexity.</returns>
        [DllImport("gdi32.dll")]
        public static extern RegionType ExtSelectClipRgn(HDc dc, HRgn region, RegionCombination operation);

        /// <summary>
        /// Retrieves the dimensions of the tightest bounding rectangle that can be drawn around the current visible area on the device. The visible area is defined by the current clipping region or clip path, as well as any overlapping windows.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="result">RECT structure that is to receive the rectangle dimensions, in logical units.</param>
        /// <returns>The clipping box's complexity, or zero if the function fails.</returns>
        [DllImport("gdi32.dll")]
        public static extern RegionType GetClipBox(HDc dc, out RECT result);

        /// <summary>
        /// Retrieves a handle identifying the current application-defined clipping region for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="region">Handle to an existing region before the function is called. After the function returns, this parameter is a handle to a copy of the current clipping region.</param>
        /// <returns>The result of the operation.</returns>
        [DllImport("gdi32.dll")]
        public static extern RegionResult GetClipRgn(HDc dc, HRgn region);

        /// <summary>
        /// Retrieves the current metaregion for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="region">Handle to an existing region before the function is called. After the function returns, this parameter is a handle to a copy of the current metaregion.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetMetaRgn(HDc dc, HRgn region);

        /// <summary>
        /// Copies the system clipping region of a specified device context to a specific region.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="region">Handle to a region. Before the function is called, this identifies an existing region. After the function returns, this identifies a copy of the current system region . The old region identified by hrgn is overwritten.</param>
        /// <param name="specify4">This parameter should specify SYSRGN, or 4.</param>
        /// <returns>The status of the result.</returns>
        [DllImport("gdi32.dll")]
        public static extern RegionResult GetRandomRgn(HDc dc, HRgn region, int specify4);

        /// <summary>
        /// Creates a new clipping region from the intersection of the current clipping region and the specified rectangle.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="left">Specifies the x-coordinate, in logical units, of the left edge of the rectangle.</param>
        /// <param name="top">Specifies the y-coordinate, in logical units, of the top edge of the rectangle.</param>
        /// <param name="right">Specifies the x-coordinate, in logical units, of the right edge of the rectangle.</param>
        /// <param name="bottom">Specifies the y-coordinate, in logical units, of the bottom edge of the rectangle.</param>
        /// <returns>Specifies the new clipping region's type.</returns>
        [DllImport("gdi32.dll")]
        public static extern RegionType IntersectClipRect(HDc dc, int left, int top, int right, int bottom);

        /// <summary>
        /// Moves the clipping region of a device context by the specified offsets.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Number of logical units to move left or right.</param>
        /// <param name="y">Number of logical units to move up or down.</param>
        /// <returns>The new region's complexity.</returns>
        [DllImport("gdi32.dll")]
        public static extern RegionType OffsetClipRgn(HDc dc, int x, int y);

        /// <summary>
        /// Determines whether the specified points is within the clipping region of a device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">x-coordinate, in logical units, of the points.</param>
        /// <param name="y">y-coordinate, in logical units, of the points.</param>
        /// <returns>Whether the points is visible. If the HDc is not valid the result will be 
        /// equal to -1 when cast to an integer.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL PtVisible(HDc dc, int x, int y);

        /// <summary>
        /// Determines whether any part of the specified rectangle lies within the clipping region of a device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="rect">RECT structure that contains the logical coordinates of the specified rectangle.</param>
        /// <returns>Whether the clipping region contains the RECT. The result will be incremented if the current transform has a rotation.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL RectVisible(HDc dc, ref RECT rect);

        /// <summary>
        /// selects the current path as a clipping region for a device context, combining the new region with any existing clipping region using the specified mode.
        /// </summary>
        /// <param name="dc">Handle to the device context of the path.</param>
        /// <param name="mode">Specifies the way to use the path.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SelectClipPath(HDc dc, RegionCombination mode);

        /// <summary>
        /// selects a region as the current clipping region for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="region">Handle to the region to be selected.</param>
        /// <returns>the region's complexity</returns>
        [DllImport("gdi32.dll")]
        public static extern RegionType SelectClipRgn(HDc dc, HRgn region);

        /// <summary>
        /// Intersects the current clipping region for the specified device context with the current metaregion and saves the combined region as the new metaregion for the specified device context. The clipping region is reset to a null region.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <returns>The return value specifies the new clipping region's complexity.</returns>
        [DllImport("gdi32.dll")]
        public static extern RegionType SetMetaRgn(HDc dc);

        /// <summary>
        /// replaces entries in the specified logical palette.
        /// </summary>
        /// <param name="palette">Handle to the logical palette.</param>
        /// <param name="startIndex">Specifies the first logical palette entry to be replaced.</param>
        /// <param name="count">Specifies the number of entries to be replaced.</param>
        /// <param name="lppePaletteData">Pointer to the first member in an array of PaletteEntry structures used to replace the current entries.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        
        [DllImport("gdi32.dll")]
        public static extern BOOL AnimatePalette(HPalette palette, int startIndex, int count, IntPtr lppePaletteData);
        /// <summary>
        /// replaces entries in the specified logical palette.
        /// </summary>
        /// <param name="palette">Handle to the logical palette.</param>
        /// <param name="startIndex">Specifies the first logical palette entry to be replaced.</param>
        /// <param name="count">Specifies the number of entries to be replaced.</param>
        /// <param name="lppePaletteData">Pointer to the first member in an array of PaletteEntry structures used to replace the current entries.</param>
        /// <param name="singlePaletteEntry">A single PaletteEntry to replace an existing entry. A value of 1 must be specified for count.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
    
        [DllImport("gdi32.dll")]
        public static extern BOOL AnimatePalette(HPalette palette, int startIndex, int count, ref PaletteEntry singlePaletteEntry);
        /// <summary>
        /// replaces entries in the specified logical palette.
        /// </summary>
        /// <param name="palette">Handle to the logical palette.</param>
        /// <param name="startIndex">Specifies the first logical palette entry to be replaced.</param>
        /// <param name="count">Specifies the number of entries to be replaced.</param>
        /// <param name="lppePaletteData">Pointer to the first member in an array of PaletteEntry structures used to replace the current entries.</param>
        /// <param name="paletteData">An array of PaletteEntry structures used to replace the current entries.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL AnimatePalette(HPalette palette, int startIndex, int count, [MarshalAs(UnmanagedType.LPArray)] PaletteEntry[] paletteData);

        /// <summary>
        /// Creates a halftone palette for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <returns>A handle to the palette. The handle will be null if the function fails.</returns>
        [DllImport("gdi32.dll")]
        public static extern HPalette CreateHalftonePalette(HDc dc);


        /// <summary>
        /// Creates a logical palette.
        /// </summary>
        /// <param name="palData">LogPalette structure that contains information about the colors in the logical palette.</param>
        /// <returns>A handle to a palette, or null if the function fails.</returns>
        [DllImport("gdi32.dll")]
        public static extern HPalette CreatePalette(ref LogPalette palData);
        /// <summary>
        /// Creates a logical palette.
        /// </summary>
        /// <param name="palData">Flattened LogPalette structure that contains information about the colors in the logical palette.</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        public static extern HPalette CreatePalette([MarshalAs(UnmanagedType.LPArray)] byte[] palData);
        
        /// <summary>
        /// retrieves the color adjustment values for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="output">Receives the color adjustment values.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetColorAdjustment(HDc dc, out ColorAdjustment output);
        
        /// <summary>
        /// Retrieves a color value identifying a color from the system palette that will be displayed when the specified color value is used.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="color">Specifies a color value that identifies a requested color.</param>
        /// <returns>If the function succeeds, the return value identifies a color from the system palette that corresponds to the given color value.</returns>
        [DllImport("gdi32.dll")]
        public static extern ColorRef GetNearestColor(HDc dc, ColorRef color);

        /// <summary>
        /// Retrieves the index for the entry in the specified logical palette most closely matching a specified color value.
        /// </summary>
        /// <param name="palette">Handle to a logical palette.</param>
        /// <param name="color">Specifies a color to be matched.</param>
        /// <returns>If the function succeeds, the return value is the index of an entry in a logical palette.</returns>
        [DllImport("gdi32.dll")]
        public static extern uint GetNearestPaletteIndex(HPalette palette, ColorRef color);

        /// <summary>
        /// Retrieves a range of palette entries from the system palette that is associated with the specified device context (DC).
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="startIndex">Specifies the first entry to be retrieved from the system palette.</param>
        /// <param name="count">Specifies the number of entries to be retrieved from the system palette.</param>
        /// <param name="entries">An array to receive the palette entries. The array must contain at least as many structures as specified by the nEntries parameter. If this parameter is NULL, the function returns the total number of entries in the palette.</param>
        /// <returns>If the function succeeds, the return value is the number of entries retrieved from the palette. If the function fails, the return value is zero.</returns>
        [DllImport("gdi32.dll")]
        public static extern uint GetSystemPaletteEntries(HDc dc, int startIndex, int count, [MarshalAs(UnmanagedType.LPArray), Out] PaletteEntry[] entries);

        /// <summary>
        /// etrieves a specified range of palette entries from the given logical palette.
        /// </summary>
        /// <param name="palette">Handle to the logical palette.</param>
        /// <param name="startIndex">Specifies the first entry in the logical palette to be retrieved.</param>
        /// <param name="count">Specifies the number of entries in the logical palette to be retrieved.</param>
        /// <param name="entries">PaletteEntry array to receive the palette entries, with at least as many PaletteEntrys as specified by the count parameter.</param>
        /// <returns>If the function succeeds and the handle to the palette is not null, the return value is the number of entries retrieved from the palette. If the function succeeds and handle to the palette is null, the return value is the number of entries in the given palette.</returns>
        [DllImport("gdi32.dll")]
        public static extern uint GetPaletteEntries(HPalette palette, int startIndex, int count, [MarshalAs(UnmanagedType.LPArray), Out] PaletteEntry[] entries);

        /// <summary>
        /// Retrieves the current state of the system (physical) palette for the specified device context (DC).
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <returns>If the function succeeds, the return value is the current state of the system palette. </returns>
        [DllImport("gdi32.dll")]
        public static extern SysPal GetSystemPaletteUse(HDc dc);


        /// <summary>
        /// maps palette entries from the current logical palette to the system palette.
        /// </summary>
        /// <param name="dc">Handle to the device context into which a logical palette has been selected.</param>
        /// <returns>If the function succeeds, the return value is the number of entries in the logical palette mapped to the system palette. If the function fails, the return value is GdiError.</returns>
        [DllImport("gdi32.dll")]
        public static extern uint RealizePalette(HDc dc);

        /// <summary>
        /// increases or decreases the size of a logical palette
        /// </summary>
        /// <param name="palette">Handle to the palette to be changed.</param>
        /// <param name="count">Specifies the number of entries in the palette after it has been resized.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL ResizePalette(HPalette palette, uint count);

        /// <summary>
        /// Selects the specified logical palette into a device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="palette">Handle to the logical palette to be selected.</param>
        /// <param name="forceBackground">Whether the palette is forced to be a background palette: TRUE causes the palette to be mapped to the colors already in the physical palette in the best possible way, FALSE causes the palette to be copied into the device palette when the application is in the foreground.</param>
        /// <returns>If the function succeeds, the return value is a handle to the device context's previous logical palette. If the function fails, the return value is NULL.</returns>
        [DllImport("gdi32.dll")]
        public static extern HPalette SelectPalette(HDc dc, HPalette palette, BOOL forceBackground);

        /// <summary>
        /// Sets the color adjustment values for a device context using the specified values.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="value">ColorAdjustment structure containing the color adjustment values.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetColorAdjustment(HDc dc, ref ColorAdjustment value);

        /// <summary>
        /// Sets a range of entries in a logical palette.
        /// </summary>
        /// <param name="palette">Handle to the logical palette.</param>
        /// <param name="startIndex">Specifies the first logical-palette entry to be set.</param>
        /// <param name="count">Specifies the number of logical-palette entries to be set.</param>
        /// <param name="entries">Array of PaletteEntry structures containing the RGB values and flags.</param>
        /// <returns>If the function succeeds, the return value is the number of entries that were set in the logical palette. If the function fails, the return value is zero.</returns>
        [DllImport("gdi32.dll")]
        public static extern uint SetPaletteEntries(HPalette palette, uint startIndex, uint count, [MarshalAs(UnmanagedType.LPArray)] PaletteEntry[] entries);

        /// <summary>
        /// Allows an application to specify whether the system palette contains 2 or 20 static colors. The default system palette contains 20 static colors.
        /// </summary>
        /// <param name="dc">Handle to the device context. This device context must refer to a device that supports color palettes.</param>
        /// <param name="usage">Specifies the new use of the system palette.</param>
        /// <returns>If the function succeeds, the return value is the previous system palette.</returns>
        [DllImport("gdi32.dll")]
        public static extern SysPal SetSystemPaletteUse(HDc dc, SysPal usage);

        /// <summary>
        /// Resets the origin of a brush or resets a logical palette.
        /// </summary>
        /// <param name="obj">Handle to the logical palette to be reset.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL UnrealizeObject(HGdiObject obj);
        /// <summary>
        /// Resets a logical palette.
        /// </summary>
        /// <param name="obj">Handle to the logical palette to be reset.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL UnrealizeObject(HPalette obj);

        /// <summary>
        /// Updates the client area of the specified device context by remapping the current colors in the client area to the currently realized logical palette.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL UpdateColors(HDc dc);

        /// <summary>
        /// concatenates two world-space to page-space transformations.
        /// </summary>
        /// <param name="result">XForm structure that receives the combined transformation.</param>
        /// <param name="transform1">XForm structure that specifies the first transformation.</param>
        /// <param name="transform2">XForm structure that specifies the second transformation.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL CombineTransform(out XForm result, ref XForm transform1, ref XForm transform2);

        /// <summary>
        /// Converts device coordinates into logical coordinates.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="points">Point structure to convert.</param>
        /// <param name="count">Point count. Should be 1.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL DPtoLP(HDc dc, ref POINT point, int count);        /// <summary>
        /// converts device coordinates into logical coordinates.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="points">Array of Points to convert.</param>
        /// <param name="count">Point count.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL DPtoLP(HDc dc, [MarshalAs(UnmanagedType.LPArray), In, Out] POINT[] points, int count);


        /// <summary>
        /// Retrieves the current position in logical coordinates.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="result">POINT structure that receives the logical coordinates of the current position.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetCurrentPositionEx(HDc dc, out POINT result);

        /// <summary>
        /// Retrieves the current graphics mode for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <returns>If the function succeeds, the return value is the current graphics mode.</returns>
        [DllImport("gdi32.dll")]
        public static extern GraphicsMode GetGraphicsMode(HDc dc);


        /// <summary>
        /// Retrieves the current mapping mode.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <returns>If the function succeeds, the return value specifies the mapping mode.</returns>
        [DllImport("gdi32.dll")]
        public static extern MappingMode GetMapMode(HDc dc);

        /// <summary>
        /// retrieves the x-extent and y-extent of the current viewport for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="result">Receives the x- and y-extents, in device units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetViewportExtEx(HDc dc, out SIZE result);

        /// <summary>
        /// retrieves the x-coordinates and y-coordinates of the viewport origin for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="result">Receives the coordinates of the origin, in device units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetViewportOrgEx(HDc dc, out POINT result);

        /// <summary>
        /// Retrieves the x-extent and y-extent of the window for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="result">Receives the x- and y-extents in page-space units, that is, logical units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetWindowExtEx(HDc dc, out SIZE result);
        
        /// <summary>
        /// Retrieves the x-coordinates and y-coordinates of the window origin for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="result">Receives the coordinates, in logical units, of the window origin.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetWindowOrgEx(HDc dc, out POINT result);

        /// <summary>
        /// Retrieves the current world-space to page-space transformation.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="result">Receives the current world-space to page-space transformation.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL GetWorldTransform(HDc dc, ref XForm result);

        /// <summary>
        /// Converts logical coordinates into device coordinates.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="points">POINT to be converted.</param>
        /// <param name="count">Specifies the number of points to convert. Should be 1.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL LPtoDP(HDc dc, ref POINT point, int count);
        /// <summary>
        /// Converts logical coordinates into device coordinates.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="points">Array of POINTs to be converted.</param>
        /// <param name="count">Specifies the number of points to convert.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL LPtoDP(HDc dc, [MarshalAs(UnmanagedType.LPArray), In, Out] POINT[] points, int count);

        /// <summary>
        /// Converts (maps) a set of points from a coordinate space relative to one window to a coordinate space relative to another window.
        /// </summary>
        /// <param name="wndFrom">Handle to the window from which points are converted, or null for screen coordinates.</param>
        /// <param name="wndTo">Handle to the window to which points are converted, or null for screen coordinates.</param>
        /// <param name="point">POINT structure that contains the point to be converted. The point is in device units.</param>
        /// <param name="pointCount">Should be 1.</param>
        /// <returns>If the function succeeds, the low-order word of the return value is the number of pixels added to the horizontal coordinate of each source points in order to compute the horizontal coordinate of each destination points. (In addition to that, if precisely one of hWndFrom and hWndTo is mirrored, then each resulting horizontal coordinate is multiplied by -1.) The high-order word is the number of pixels added to the vertical coordinate of each source point in order to compute the vertical coordinate of each destination point.</returns>
        [DllImport("user32.dll")]
        public static extern int MapWindowPoints(HWnd wndFrom, HWnd wndTo, ref POINT point, int pointCount);
        /// <summary>
        /// Converts (maps) a set of points from a coordinate space relative to one window to a coordinate space relative to another window.
        /// </summary>
        /// <param name="wndFrom">Handle to the window from which points are converted, or null for screen coordinates.</param>
        /// <param name="wndTo">Handle to the window to which points are converted, or null for screen coordinates.</param>
        /// <param name="points">POINT array that contains 3 or more points to be converted. The points are in device units.</param>
        /// <param name="pointCount">Specifies the number of POINT structures in the array.</param>
        /// <returns>If the function succeeds, the low-order word of the return value is the number of pixels added to the horizontal coordinate of each source points in order to compute the horizontal coordinate of each destination point. (In addition to that, if precisely one of hWndFrom and hWndTo is mirrored, then each resulting horizontal coordinate is multiplied by -1.) The high-order word is the number of pixels added to the vertical coordinate of each source point in order to compute the vertical coordinate of each destination point.</returns>
        [DllImport("user32.dll")]
        public static extern int MapWindowPoints(HWnd wndFrom, HWnd wndTo, [MarshalAs(UnmanagedType.LPArray), In, Out] POINT[] points, int pointCount);
        /// <summary>
        /// Converts (maps) a set of points from a coordinate space relative to one window to a coordinate space relative to another window.
        /// </summary>
        /// <param name="wndFrom">Handle to the window from which points are converted, or null for screen coordinates.</param>
        /// <param name="wndTo">Handle to the window to which points are converted, or null for screen coordinates.</param>
        /// <param name="rect">RECT to convert. The rect is in device coordinates.</param>
        /// <param name="pointCount">Should be 2.</param>
        /// <returns>If the function succeeds, the low-order word of the return value is the number of pixels added to the horizontal coordinate of each source points in order to compute the horizontal coordinate of each destination point. (In addition to that, if precisely one of hWndFrom and hWndTo is mirrored, then each resulting horizontal coordinate is multiplied by -1.) The high-order word is the number of pixels added to the vertical coordinate of each source point in order to compute the vertical coordinate of each destination point.</returns>
        [DllImport("user32.dll")]
        public static extern int MapWindowPoints(HWnd wndFrom, HWnd wndTo, ref RECT rect, int pointCount);

        /// <summary>
        /// Changes the world transformation for a device context using the specified mode.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="xForm">XForm structure used to modify the world transformation for the given device context.</param>
        /// <param name="mode">Specifies how the transformation data modifies the current world transformation.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL ModifyWorldTransform(HDc dc, ref XForm xForm, ModWorldTransform mode);

        /// <summary>
        /// Modifies the viewport origin for a device context using the specified horizontal and vertical offsets.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the horizontal offset, in device units.</param>
        /// <param name="y">Specifies the vertical offset, in device units.</param>
        /// <param name="lastPoint">The previous viewport origin, in device units, is placed in this structure. </param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL OffsetViewportOrgEx(HDc dc, int x, int y, out POINT lastPoint);
        /// <summary>
        /// Modifies the viewport origin for a device context using the specified horizontal and vertical offsets.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the horizontal offset, in device units.</param>
        /// <param name="y">Specifies the vertical offset, in device units.</param>
        /// <param name="lastPoint">The previous viewport origin, in device units, is placed in the structure the pointer references. Specify null to discard this value.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL OffsetViewportOrgEx(HDc dc, int x, int y, IntPtr lastPoint);

        /// <summary>
        /// Updates the current position to the specified point and optionally returns the previous position.
        /// </summary>
        /// <param name="dc">Handle to a device context.</param>
        /// <param name="x">Specifies the x-coordinate, in logical units, of the new position, in logical units.</param>
        /// <param name="y">Specifies the y-coordinate, in logical units, of the new position, in logical units.</param>
        /// <param name="lastPoint">POINT structure that receives the previous current position.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL MoveToEx(HDc dc, int x, int y, out POINT lastPoint);
     
        /// <summary>
        /// Updates the current position to the specified point and optionally returns the previous position.
        /// </summary>
        /// <param name="dc">Handle to a device context.</param>
        /// <param name="x">Specifies the x-coordinate, in logical units, of the new position, in logical units.</param>
        /// <param name="y">Specifies the y-coordinate, in logical units, of the new position, in logical units.</param>
        /// <param name="lastPoint">Pointer to a POINT structure that receives the previous current position. If this parameter is a NULL pointer, the previous position is not returned.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL MoveToEx(HDc dc, int x, int y, IntPtr lastPoint);


        /// <summary>
        /// Modifies the window origin for a device context using the specified horizontal and vertical offsets.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the horizontal offset, in logical units.</param>
        /// <param name="y">Specifies the vertical offset, in logical units.</param>
        /// <param name="lastPoint">The logical coordinates of the previous window origin are placed in this structure. If lpPoint is NULL, the previous origin is not returned.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL OffsetWindowOrgEx(HDc dc, int x, int y, out POINT lastPoint);
        /// <summary>
        /// Modifies the window origin for a device context using the specified horizontal and vertical offsets.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the horizontal offset, in logical units.</param>
        /// <param name="y">Specifies the vertical offset, in logical units.</param>
        /// <param name="lastPoint">The logical coordinates of the previous window origin are placed in the structure this pointer references. Specify null to disregard this value.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL OffsetWindowOrgEx(HDc dc, int x, int y, IntPtr lastPoint);

        /// <summary>
        /// Modifies the viewport for a device context using the ratios formed by the specified multiplicands and divisors.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="xNumerator">Specifies the amount by which to multiply the current horizontal extent.</param>
        /// <param name="xDenominator">Specifies the amount by which to divide the current horizontal extent.</param>
        /// <param name="yNumerator">Specifies the amount by which to multiply the current vertical extent.</param>
        /// <param name="yDenominator">Specifies the amount by which to divide the current vertical extent.</param>
        /// <param name="lastScale">SIZE structure that receives the previous viewport extents, in device units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL ScaleViewportExtEx(HDc dc, int xNumerator, int xDenominator, int yNumerator, int yDenominator, out SIZE lastScale);
        /// <summary>
        /// Modifies the viewport for a device context using the ratios formed by the specified multiplicands and divisors.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="xNumerator">Specifies the amount by which to multiply the current horizontal extent.</param>
        /// <param name="xDenominator">Specifies the amount by which to divide the current horizontal extent.</param>
        /// <param name="yNumerator">Specifies the amount by which to multiply the current vertical extent.</param>
        /// <param name="yDenominator">Specifies the amount by which to divide the current vertical extent.</param>
        /// <param name="lastScale">Pointer to a SIZE structure that receives the previous viewport extents, in device units. Specify null to discard this value.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL ScaleViewportExtEx(HDc dc, int xNumerator, int xDenominator, int yNumerator, int yDenominator, IntPtr lastScale);

        /// <summary>
        /// Modifies the window for a device context using the ratios formed by the specified multiplicands and divisors.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="xNumerator">Specifies the amount by which to multiply the current horizontal extent.</param>
        /// <param name="xDenominator">Specifies the amount by which to divide the current horizontal extent.</param>
        /// <param name="yNumerator">Specifies the amount by which to multiply the current vertical extent.</param>
        /// <param name="yDenominator">Specifies the amount by which to divide the current vertical extent.</param>
        /// <param name="lastScale">SIZE structure that receives the previous viewport extents, in device units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern int ScaleWindowExtEx(HDc dc, int xNumerator, int xDenominator, int yNumerator, int yDenominator, out SIZE lastScale);

        /// <summary>
        /// Modifies the window for a device context using the ratios formed by the specified multiplicands and divisors.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="xNumerator">Specifies the amount by which to multiply the current horizontal extent.</param>
        /// <param name="xDenominator">Specifies the amount by which to divide the current horizontal extent.</param>
        /// <param name="yNumerator">Specifies the amount by which to multiply the current vertical extent.</param>
        /// <param name="yDenominator">Specifies the amount by which to divide the current vertical extent.</param>
        /// <param name="lastScale">Pointer to a SIZE structure that receives the previous window extents, in device units. Specify null to discard this value.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern int ScaleWindowExtEx(HDc dc, int xNumerator, int xDenominator, int yNumerator, int yDenominator, IntPtr lastScale);

        /// <summary>
        /// sets the graphics mode for the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="iMode">Specifies the graphics mode.</param>
        /// <returns>If the function succeeds, the return value is the old graphics mode. If the function fails, the return value is zero. </returns>
        [DllImport("gdi32.dll")]
        public static extern GraphicsMode SetGraphicsMode(HDc dc, GraphicsMode iMode);

        /// <summary>
        /// Sets the mapping mode of the specified device context.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="mode">Specifies the new mapping mode.</param>
        /// <returns>If the function succeeds, the return value identifies the previous mapping mode. If the function fails, the return value is zero.</returns>
        [DllImport("gdi32.dll")]
        public static extern MappingMode SetMapMode(HDc dc, MappingMode mode);

        /// <summary>
        /// Sets the horizontal and vertical extents of the viewport for a device context by using the specified values.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the horizontal extent, in device units, of the viewport.</param>
        /// <param name="y">Specifies the vertical extent, in device units, of the viewport.</param>
        /// <param name="oldSize">Receives the previous viewport extents, in device units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetViewportExtEx(HDc dc, int x, int y, out SIZE oldSize);
        /// <summary>
        /// Sets the horizontal and vertical extents of the viewport for a device context by using the specified values.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="x">Specifies the horizontal extent, in device units, of the viewport.</param>
        /// <param name="y">Specifies the vertical extent, in device units, of the viewport.</param>
        /// <param name="oldSize">Pointer to a SIZE structure to receives the previous viewport extents, or null to discard the value.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetViewportExtEx(HDc dc, int x, int y, IntPtr lpOldSize);

        /// <summary>
        /// Specifies which device point maps to the window origin.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="nX">Specifies the x-coordinate, in device units, of the new viewport origin.</param>
        /// <param name="nY">Specifies the y-coordinate, in device units, of the new viewport origin.</param>
        /// <param name="oldSize">POINT structure that receives the previous viewport origin, in device coordinates.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetViewportOrgEx(HDc dc, int nX, int nY, out POINT oldSize);
        /// <summary>
        /// Specifies which device point maps to the window origin.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="nX">Specifies the x-coordinate, in device units, of the new viewport origin.</param>
        /// <param name="nY">Specifies the y-coordinate, in device units, of the new viewport origin.</param>
        /// <param name="oldSize">Pointer to POINT structure that receives the previous viewport origin, or null to discard the value.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetViewportOrgEx(HDc dc, int nX, int nY, IntPtr oldSize);

        /// <summary>
        /// Sets the horizontal and vertical extents of the window for a device context by using the specified values.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="nX">Specifies the window's horizontal extent in logical units.</param>
        /// <param name="nY">Specifies the window's vertical extent in logical units.</param>
        /// <param name="oldSize">SIZE structure that receives the previous window extents, in logical units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetWindowExtEx(HDc dc, int nX, int nY, out SIZE oldSize);
        /// <summary>
        /// Sets the horizontal and vertical extents of the window for a device context by using the specified values.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="nX">Specifies the window's horizontal extent in logical units.</param>
        /// <param name="nY">Specifies the window's vertical extent in logical units.</param>
        /// <param name="oldSize">SIZE structure that receives the previous window extents, in logical units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetWindowExtEx(HDc dc, int nX, int nY, IntPtr oldSize);

        /// <summary>
        /// Specifies which window point maps to the viewport origin.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="nX">Specifies the x-coordinate, in logical units, of the new window origin.</param>
        /// <param name="nY">Specifies the y-coordinate, in logical units, of the new window origin.</param>
        /// <param name="oldPoint">POINT structure that receives the previous origin of the window, in logical units.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetWindowOrgEx(HDc dc, int nX, int nY, out POINT oldPoint);
        /// <summary>
        /// Specifies which window point maps to the viewport origin.
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="nX">Specifies the x-coordinate, in logical units, of the new window origin.</param>
        /// <param name="nY">Specifies the y-coordinate, in logical units, of the new window origin.</param>
        /// <param name="oldPoint">Pointer to POINT structure that receives the previous origin of the window, or null to discard the value.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetWindowOrgEx(HDc dc, int nX, int nY, IntPtr oldPoint);

        /// <summary>
        /// sets a two-dimensional linear transformation between world space and page space for the specified device context. 
        /// </summary>
        /// <param name="dc">Handle to the device context.</param>
        /// <param name="xForm">XFORM structure that contains the transformation data.</param>
        /// <returns>TRUE for success, FALSE for failure.</returns>
        [DllImport("gdi32.dll")]
        public static extern BOOL SetWorldTransform(HDc dc, ref XForm xForm);


        public static ColorRef RGB(int red, int blue, int green) { return new ColorRef(red, green, blue); }

        public static RasterOperation CombineForMskBlt(RasterOperation whiteOp, RasterOperation blackOp) {

            return (RasterOperation)(
                    (
                        (((uint)blackOp) << 8) & 0xFF000000
                    ) | ((uint)whiteOp)
                );
        }
    }



}

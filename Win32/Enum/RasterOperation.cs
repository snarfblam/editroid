namespace Windows.Enum
{
    public enum RasterOperation:uint 
    {
        /// <summary>Fills the destination rectangle using the color associated with index 0 in the physical palette.</summary>
        Blackness = 0x42,
        /// <summary>Includes any windows that are layered on top of your window in the resulting image. By default, the image only contains your window. Note that this generally cannot be used for printing device contexts.</summary>
        CaptureBlit = 0x40000000,
        /// <summary>Inverts the destination rectangle.</summary>
        DestInvert = 0x550009,
        /// <summary>Merges the colors of the source rectangle with the brush currently selected in hdcDest, by using the Boolean AND operator.</summary>
        MergeCopy = 0xC000CA,
        /// <summary>Merges the colors of the inverted source rectangle with the colors of the destination rectangle by using the Boolean OR operator.</summary>
        MergePaint = 0xBB0226,
        /// <summary>Prevents the bitmap from being mirrored.</summary>
        NoMirrorBitmap = 0x80000000,
        /// <summary>Copies the inverted source rectangle to the destination.</summary>
        NotSourceCopy = 0x330008,
        /// <summary>Combines the colors of the source and destination rectangles by using the Boolean OR operator and then inverts the resultant color.</summary>
        NotSourceErase = 0x1100A6,
        /// <summary>Copies the brush currently selected in hdcDest, into the destination bitmap.</summary>
        PatternCopy = 0xF00021,
        /// <summary>Combines the colors of the brush currently selected in hdcDest, with the colors of the destination rectangle by using the Boolean XOR operator.</summary>
        PatternInvert = 0x5A0049,
        /// <summary>Combines the colors of the brush currently selected in hdcDest, with the colors of the inverted source rectangle by using the Boolean OR operator. The result of this operation is combined with the colors of the destination rectangle by using the Boolean OR operator.</summary>
        PatternPaint = 0xFB0A09,
        /// <summary>Combines the colors of the source and destination rectangles by using the Boolean AND operator.</summary>
        SourceAnd = 0x8800C6,
        /// <summary>Copies the source rectangle directly to the destination rectangle.</summary>
        SourceCopy = 0xCC0020,
        /// <summary>Combines the inverted colors of the destination rectangle with the colors of the source rectangle by using the Boolean AND operator.</summary>
        SourceErase = 0x440328,
        /// <summary>Combines the colors of the source and destination rectangles by using the Boolean XOR operator.</summary>
        SourceInvert = 0x660046,
        /// <summary>Combines the colors of the source and destination rectangles by using the Boolean OR operator.</summary>
        SourcePaint = 0xEE0086,
        /// <summary>Fills the destination rectangle using the color associated with index 1 in the physical palette.</summary>
        Whiteness = 0xFF0062
    }
}

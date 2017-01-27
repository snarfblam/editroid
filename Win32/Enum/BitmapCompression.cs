namespace Windows.Enum
{
    public enum BitmapCompression: uint
    {
        /// <summary>Uncompressed</summary>
        Rgb = 0,
        /// <summary>Runlength encoded 8-bpp.</summary>
        Rle8 = 1,
        /// <summary>Runlength encoded 8-bpp.</summary>
        Rle4 = 2,
        /// <summary>For 16- or 32-bit bitmaps. Same as Rgb, except the color table contains three 32-bit bitmasks, one each for red, green, and blue, that define which bits correspond to the component. This is generally used for "565" 16-bit bitmaps.</summary>
        BitFields = 3,
        /// <summary>JPeg compression.</summary>
        JPeg = 4,
        /// <summary>PNG compression.</summary>
        PNG = 5

    }
} 

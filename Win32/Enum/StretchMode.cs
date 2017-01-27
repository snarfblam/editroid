namespace Windows.Enum
{
    public enum StretchMode
    {
        /// <summary>Performs a Boolean AND operation using the color values for the eliminated and existing pixels. If the bitmap is a monochrome bitmap, this mode preserves black pixels at the expense of white pixels.</summary>
        BlackOnWhite = 1,
        /// <summary>Deletes the pixels. This mode deletes all eliminated lines of pixels without trying to preserve their information.</summary>
        ColorOnColor = 3,
        /// <summary>Maps pixels from the source rectangle into blocks of pixels in the destination rectangle. The average color over the destination block of pixels approximates the color of the source pixels.</summary>
        /// <remarks>
        /// After setting the HALFTONE stretching mode, an application must call the SetBrushOrgEx function to set the brush origin. If it fails to do so, brush misalignment occurs.
        /// This option is not supported on Windows 95/98/Me.
        /// </remarks>
        Halftone = 4,
        /// <summary>Same as BlackOnWhite.</summary>
        AndScans = 1,
        /// <summary>Same as ColorOnColor.</summary>
        DeleteScans = 3,
        /// <summary>Same as WhiteOnBlack.</summary>
        OrScans = 2,
        /// <summary>Performs a Boolean OR operation using the color values for the eliminated and existing pixels. If the bitmap is a monochrome bitmap, this mode preserves white pixels at the expense of black pixels.</summary>
        WhiteOnBlack = 2
    }
}

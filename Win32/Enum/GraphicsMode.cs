namespace Windows.Enum
{
    public enum GraphicsMode: int
    {
        /// <summary>
        /// The current graphics mode is the compatible graphics mode, a mode that is compatible with 16-bit Windows. In this graphics mode, an application cannot set or modify the world transformation for the specified device context.
        /// </summary>
        Advanced = 2,
        /// <summary>
        /// The current graphics mode is the advanced graphics mode, a mode that allows world transformations. In this graphics mode, an application can set or modify the world transformation for the specified device context.
        /// </summary>
        Compatible = 1

    }
}

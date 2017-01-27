namespace Windows.Enum
{
    public enum BrushStyle:uint
    {
        /// <summary>A pattern brush defined by a bitmap (DIB). If DibPattern is used, the LogBrush.PatternDibHandle member contains a handle to a packed DIB.</summary>
        DibPattern = 5,
        /// <summary>Same as DibPattern.</summary>
        DibPattern8x8 = 8,
        /// <summary>A pattern brush defined by a bitmap (DIB). If DibPatternPtr is used, the LogBrush.PatternDibPointer member contains a pointer to a packed DIB.</summary>
        DibPatternPtr = 6,
        /// <summary>Hatched brush.</summary>
        Hatched = 2,
        /// <summary>Hollow brush.</summary>
        Hollow = Null,
        /// <summary>Same as Hollow.</summary>
        Null = 1,
        /// <summary>Pattern brush defined by a memory bitmap.</summary>
        Pattern = 3,
        /// <summary>Same as Pattern.</summary>
        Pattern8x8 = 7,
        /// <summary>Solid brush.</summary>
        Solid = 0

    }
}

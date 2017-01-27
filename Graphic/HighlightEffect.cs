namespace Editroid
{
    /// <summary>Enumerates possible highlight effects for objects in a ScreenControl.</summary>
    public enum HighlightEffect
    {
        /// <summary>The object is inverted.</summary>
        Invert,
        /// <summary>The object is displayed using lighter colors</summary>
        Lighten,
        /// <summary>The object is displayed using lighter colors on an inverted background</summary>
        LightenInvertBack,
        /// <summary>The object is displayed on an inverted background</summary>
        InvertBack,
        /// <summary>The object is displayed with normal colors and has a surrounding rectangle.</summary>
        Rectangle
    }
}

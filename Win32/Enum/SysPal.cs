namespace Windows.Enum
{
    public enum SysPal : uint
    {
        /// <summary>The given device context is invalid or does not support a color palette.</summary>
        Error = 0,
        /// <summary>The system palette contains no static colors except black and white.</summary>
        NoStatic = 2,
        /// <summary></summary>
        NoStatic256 = 3,
        /// <summary>	The system palette contains static colors that will not change when an application realizes its logical palette.</summary>
        Static = 1,
    }
}

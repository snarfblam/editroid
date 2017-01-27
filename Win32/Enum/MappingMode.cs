namespace Windows.Enum
{
    public enum MappingMode: int
    {
        /// <summary>Logical units are mapped to arbitrary units with arbitrarily scaled axes. Use the SetWindowExtEx and SetViewportExtEx functions to specify the units, orientation, and scaling required.</summary>
        Anisotropic = 8,
        /// <summary>Each logical unit is mapped to 0.001 inch. Positive x is to the right; positive y is up.</summary>
        HiEnglish = 5,
        /// <summary>Each logical unit is mapped to 0.01 millimeter. Positive x is to the right; positive y is up.</summary>
        HiMetric = 3,
        /// <summary>Logical units are mapped to arbitrary units with equally scaled axes; that is, one unit along the x-axis is equal to one unit along the y-axis. Graphics device interface makes adjustments as necessary to ensure the x and y units remain the same size.</summary>
        Isotropic = 7,
        /// <summary>Each logical unit is mapped to 0.01 inch. Positive x is to the right; positive y is up.</summary>
        LoEnglish = 4,
        /// <summary>Each logical unit is mapped to 0.1 millimeter. Positive x is to the right; positive y is up.</summary>
        LoMetric = 2,
        /// <summary>Each logical unit is mapped to one device pixel. Positive x is to the right; positive y is down.</summary>
        Text = 1,
        /// <summary>Each logical unit is mapped to one twentieth of a printer's points (1/1440 inch, also called a "twip"). Positive x is to the right; positive y is up.</summary>
        Twips = 6,

    }
}

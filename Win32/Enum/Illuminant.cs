namespace Windows.Enum
{
    public enum Illuminant:ushort
    {
        /// <summary>Device's default. Standard used by output devices.</summary>
        DeviceDefault = 0,
        /// <summary>Tungsten lamp.</summary>
        A = 1,
        /// <summary>Noon sunlight.</summary>
        B = 2,
        /// <summary>NTSC daylight.</summary>
        C = 3,
        /// <summary>Normal print.</summary>
        D50 = 4,
        /// <summary>Bond paper print.</summary>
        D55 = 5,
        /// <summary>Standard daylight. Standard for CRTs and pictures.</summary>
        D65 = 6,
        /// <summary>Northern daylight.</summary>
        D75 = 7,
        /// <summary>Cool white lamp.</summary>
        F2 = 8,
        /// <summary>Same as A.</summary>
        Tungsten = A,
        /// <summary>Same as C.</summary>
        Daylight = C,
        /// <summary>Same as F2.</summary>
        Fluorescent = F2,
        /// <summary>Same as C.</summary>
        Ntsc = C
    }
}

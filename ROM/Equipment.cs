using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    [Flags]
    public enum Equipment: byte
    {
        None = 0,
        Bombs = 1,
        HighJump = 2,
        LongBeam = 4,
        ScrewAttack = 8,
        MaruMari = 0x10,
        Varia = 0x20,
        WaveBeam = 0x40,
        IceBeam = 0x80
    }
}

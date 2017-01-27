using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    class LevelPointerTable<TTargetType>
        where TTargetType : RomObject
    {
        pRom Offset;
        int count;

        public LevelPointerTable(pRom offset) {
            this.Offset = offset;
        }
    }
}

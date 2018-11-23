using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
    
    struct DumpInsertItem
    {
        public LevelIndex Area { get; private set; }
        public DumpInsertType DataType { get; private set; }

        public DumpInsertItem(LevelIndex area, DumpInsertType type) 
        :this(){
            this.Area = area;
            this.DataType = type;
        }
    }

    enum DumpInsertType
    {
        CHR,
        CHRAnimation,
        Item,
        Palette,
        Combo,
        Structure,
        Screen,
        AltMusic,
        Asm,
        TilePhysics,
        Password
    }
}

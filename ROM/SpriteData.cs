using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    class LevelSpriteData
    {
        pCpu FramePointerTable = (pCpu)0x860B;
        pCpu PlacementPointerTable = (pCpu)0x86Df;
        pCpu AnimationTable = (pCpu)0x8572;

        Level level;

        public LevelSpriteData(Level l) {
            this.level = l;
        }
    }

    class SpriteData
    {
        Level l;
        pCpu frameData;
    }
}

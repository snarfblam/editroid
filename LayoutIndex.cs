using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
    /// <summary>
    /// T compact (16-bit) value that identifies a screen layout
    /// </summary>
    struct LayoutIndex
    {
        bool altPal;
        byte level;
        byte screenIndex;

        public LevelIndex Level {
            get {
                return (LevelIndex)level;
            }
            private set {
                level = (byte)((int)value & 0xFF);
            }
        }

        public int ScreenIndex {
            get {
                return screenIndex;
            }
            private set {
                screenIndex = (byte)(value & 0xFF);
            }
        }

        public bool AltPalette { get { return altPal; } }

        public LayoutIndex(LevelIndex level, int screen, bool altPal) {
            this.level = (byte)((int)level & 0xFF);
            this.screenIndex = (byte)(screen & 0xFF);
            this.altPal = altPal;
        }

        static public bool operator == (LayoutIndex a, LayoutIndex b){
            return a.screenIndex == b.screenIndex && a.level == b.level && a.altPal == b.altPal;
        }
        static public bool operator !=(LayoutIndex a, LayoutIndex b) {
            return !(a == b);
        }
    }
}

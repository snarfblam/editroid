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
        byte level;
        byte screenIndex;

        public LevelIndex Level {
            get {
                return (LevelIndex)level;
            }
            set {
                level = (byte)((int)value & 0xFF);
            }
        }

        public int ScreenIndex {
            get {
                return screenIndex;
            }
            set {
                screenIndex = (byte)(value & 0xFF);
            }
        }

        public LayoutIndex(LevelIndex level, int screen) {
            this.level = (byte)((int)level & 0xFF);
            this.screenIndex = (byte)(screen & 0xFF);
        }

        static public bool operator == (LayoutIndex a, LayoutIndex b){
            return a.screenIndex == b.screenIndex && a.level == b.level;
        }
        static public bool operator !=(LayoutIndex a, LayoutIndex b) {
            return !(a == b);
        }
    }
}

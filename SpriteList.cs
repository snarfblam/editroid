using System;
using System.Collections.Generic;
using System.Text;
using Editroid.Graphic;
using System.Drawing;

namespace Editroid
{
    /// <summary>
    /// Stores information needed to render a number of enemies with a ScreenDisplay.
    /// </summary>
    class SpriteList:List<SpriteListItem>
    {

    }

    /// <summary>
    /// Contians the information necessary to render a sprite with a ScreenDisplay.
    /// </summary>
    struct SpriteListItem
    {
        public SpriteListItem(SpriteDefinition sprite, int palIndex, Point location) {
            this.sprite = sprite;
            this.palette = palIndex;
            this.location = location;
        }

        private int palette;
        public int PaletteIndex {
            get { return palette; }
            set { palette = value; }
        }

        private SpriteDefinition sprite;
        public SpriteDefinition SpriteDefinition {
            get { return sprite; }
            set { sprite = value; }
        }

        private Point location;
        public Point Location {
            get { return location; }
            set { location = value; }
        }
    }
}

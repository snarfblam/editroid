using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Editroid.UndoRedo
{
    abstract class MapLocationAction : EditroidAction
    {
        private Point location;
        public MapLocationAction(EditroidUndoRedoQueue q, Point location)
            : base(q) {
            this.location = location;
        }

        public override Point AffectedMapLocation {
            get {
                return location;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.UndoRedo
{
    abstract class ScreenAction:LevelAction
    {
        //int screen;
        public override int AffectedScreenIndex { get { return Screen.Index; } }

        public ScreenAction(EditroidUndoRedoQueue q, Screen screen) 
        :base(q, screen.Owner.Level) {
            this.Screen = screen;
        }

        public Screen Screen { get; private set; }
        //public Screen Screen { get { return Level.Screens[screen]; } }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;

namespace Editroid.Actions
{
    class ChangeColorAttributeTable:ScreenAction
    {
        int oldValue, newValue;
        public ChangeColorAttributeTable(EditroidUndoRedoQueue q, Screen screen, int newValue) 
        :base(q,screen){
            this.newValue = newValue;
            oldValue = Screen.ColorAttributeTable;
        }
        public override void Do() {
            Screen.ColorAttributeTable = (byte)newValue;
        }

        public override void Undo() {
            Screen.ColorAttributeTable = (byte)oldValue;
        }

        public override string GetText() {
            return "Set default color attribute table";
        }

        public override bool TryCombine(EditroidAction newerAction) {
            ChangeColorAttributeTable cat = newerAction as ChangeColorAttributeTable;
            if (cat != null && cat.AffectedLevel == AffectedLevel && cat.AffectedScreenIndex == AffectedScreenIndex) {
                newValue = cat.newValue;
                return true;
            }

            return false;
        }
    }
}

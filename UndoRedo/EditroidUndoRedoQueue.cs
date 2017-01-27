using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.UndoRedo
{
    class EditroidUndoRedoQueue: UndoRedoQueue<EditroidAction>
    {
        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { rom = value; }
        }

        private frmMain editor;

        public frmMain Editor {
            get { return editor; }
            set { editor = value; }
        }

        public override void Do(EditroidAction a) {
            a.Do();
            redoQueue.Clear();
            if (!(undoQueue.Count > 0 && undoQueue.Peek().TryCombine(a))) {
                undoQueue.Push(a);
                CropUndos();
            }
        }
    }
}

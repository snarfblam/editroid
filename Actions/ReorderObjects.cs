using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid.Actions
{
    class ReorderObjects: ScreenItemAction  
    {
        bool toFront;
        //int originalIndex;
        int oldIndex, newIndex;

        public ReorderObjects(EditroidUndoRedoQueue q, Screen screen, ObjectInstance obj, bool toFront)
            : base(q, screen, obj) {
            if (!(obj is StructInstance))
                throw new ArgumentException("Invalid screen item specified for object reorder.", "obj");

            this.toFront = toFront;
            oldIndex = Screen.GetIndex(obj);
            if (toFront)
                newIndex = Screen.Structs.Count - 1;
            else
                newIndex = 0;
        }

        public bool ToFront { get { return toFront; } }
        public override string GetText() {
            return toFront ? "Send to front" : "Send to back";
        }

        public override void Do() {
            Screen.ReorderObject(oldIndex, newIndex);
        }

        public override void Undo() {
            Screen.ReorderObject(newIndex, oldIndex);
        }

        public override bool TryCombine(EditroidAction newerAction) {
            ReorderObjects a = newerAction as ReorderObjects;
            if (a != null && a.GetItem().Equals(GetItem())) {
                newIndex = a.newIndex;
                toFront = a.toFront;
                return true;
            }
            return false;
        }
    }
}

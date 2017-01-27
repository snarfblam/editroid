using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid.Actions
{
   
    /// <summary>
    /// Represents a modification to an EXISTING structure tile
    /// </summary>
    class EditStructTile : StructureAction
    {
        StructTileEdit[] edits;
        public EditStructTile(EditroidUndoRedoQueue q, Level level, int structIndex, StructTileEdit edit)
            : base(q, level, structIndex) {
            edit.oldValue = GetStruct().Data[edit.x, edit.y];
            edits = new StructTileEdit[] { edit };
        }
        public EditStructTile(EditroidUndoRedoQueue q, Level level, int structIndex, StructTileEdit[] edits)
            : base(q, level, structIndex) {
            this.edits = edits;
            for (int i = 0; i < edits.Length; i++) {
                var edit = edits[i];
                edit.oldValue = GetStruct().Data[edit.x, edit.y];
                edits[i] = edit;
            }
        }
        protected override void DoImplementation() {
            var strct = GetStruct();
            var structData = strct.BeginEdit();
            for (int i = 0; i < edits.Length; i++) {
                var edit = edits[i];
                structData[edit.x, edit.y] = edit.newValue;
            }
            structData.EndEdit();
        }

        protected override void UndoImplementation() {
            var strct = GetStruct();
            var structData = strct.BeginEdit();
            for (int i = 0; i < edits.Length; i++) {
                var edit = edits[i];
                structData[edit.x, edit.y] = edit.oldValue;
            }
            structData.EndEdit();
        }

        public override string Description {
            get { return "Edit structure"; }
        }
    }

    class RemoveStructTile : EditStructTile
    {
        public RemoveStructTile(EditroidUndoRedoQueue q, Level level, int structIndex, int x, int y)
            : base(q, level, structIndex, new StructTileEdit(x, y, Structure.EmptyTile)) {
        }

        public override string Description {
            get {
                return "Delete tile from structure";
            }
        }

    }

    public struct StructTileEdit
    {
        public byte x, y, newValue, oldValue;

        public StructTileEdit(int x, int y, int newValue) {
            this.x = (byte)x;
            this.y = (byte)y;
            this.newValue= (byte)newValue;
            this.oldValue = 0;
        }

    }

}

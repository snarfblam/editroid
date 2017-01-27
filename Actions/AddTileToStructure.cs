using System;
using System.Collections.Generic;
using Editroid.UndoRedo;

namespace Editroid.Actions
{
    class AddTileToStructure:StructureAction
    {
        int x, y;
        byte tile;
        byte[,] initialState;

        public AddTileToStructure(EditroidUndoRedoQueue q, Level level, int structIndex, int x, int y, byte tile):base(q,level,structIndex){
            this.x = x;
            this.y =y;
            this.tile = tile;

            initialState = GetStruct().Data.CopyData();
        }
        protected override void DoImplementation() {
            var s = GetStruct();
            var edit = s.BeginEdit();

            edit[x, y] = tile;

            edit.EndEdit();
        }

        protected override void UndoImplementation() {
            var edit = GetStruct().BeginEdit();

            edit.SetData(initialState);

            edit.EndEdit();
        }

        public override string Description {
            get { return "Edit structure"; }
        }
    }
}

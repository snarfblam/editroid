using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;

namespace Editroid.UndoRedo
{
    abstract class ItemAction: LevelAction
    {
        public ItemAction(EditroidUndoRedoQueue q, Level level, ItemScreenData screen, ItemData item) 
        :base(q,level){
            this.Screen = screen;
            this.Item = item;
        }

        public ItemScreenData Screen { get; private set; }
        public ItemData Item { get; private set; }

        ////ItemIndex_DEPRECATED itemIndex;
        ////public ItemIndex_DEPRECATED ItemIndex { get { return itemIndex; } }

        ////public ItemSeeker GetItem() {
        ////    return Level.ItemTable_DEPRECATED.GetItem(itemIndex);
        ////}
        ////public ItemRowEntry GetRow() {
        ////    return Level.ItemTable_DEPRECATED.GetRowByIndex(itemIndex);
        ////}
    }
}

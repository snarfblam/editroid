////using System;
////using System.Collections.Generic;
////using System.Text;
////using Editroid.UndoRedo;
////using Editroid.ROM;

////namespace Editroid.Actions
////{
////    /// <summary>
////    /// Sets the vertical map coordinate of an ItemRowEntry.
////    /// </summary>
////    class SetItemRowPosition:ItemAction
////    {
////        int newY, oldY;
////        bool updatePassData;
////        List<int> affectedPassEntries = new List<int>();

////        public SetItemRowPosition(EditroidUndoRedoQueue q, Level level, ItemIndex_DEPRECATED itemIndex, int newY, bool updatePass) 
////        :base(q,level,itemIndex){
////            this.newY = newY;
////            oldY = Level.ItemTable_DEPRECATED.GetRowByIndex(itemIndex).MapY;
////            this.updatePassData = updatePass;

////            if (updatePassData) {
////                foreach (ScreenItems screen in GetRow()) {
////                    foreach (ItemInstance item in screen) {
////                        int datIndex = Queue.Rom.PasswordData.GetDatumIndex(item);
////                        if (datIndex != -1) {
////                            affectedPassEntries.Add(datIndex);
////                        }
////                    }
////                }
////            }
////        }
////        public override void Do() {
////            SetItemY(oldY, newY);
////        }

////        private void SetItemY(int fromWhat, int toWhat) {
////            foreach (int i in affectedPassEntries) {
////                PasswordDatum dat = Queue.Rom.PasswordData.GetDatum(i);
////                dat.MapY = toWhat;
////            }

////            Level.ItemTable_DEPRECATED.GetRow(fromWhat).MapY = toWhat;

////        }

////        public int NewY { get { return newY; } }
////        public int OldY { get { return oldY; } }

////        public delegate void PasswordEntryDelegate(int entryIndex);
        
////        public void ForEachPasswordEntry(PasswordEntryDelegate action) {

////            foreach (int i in affectedPassEntries) {
////                action(i);
////            }
////        }

////        public override void Undo() {
////            SetItemY(newY, oldY);
////        }

////        public override string GetText() {
////            return "Move item row";
////        }

////        public override bool TryCombine(EditroidAction newerAction) {
////            SetItemRowPosition a = newerAction as SetItemRowPosition;
////            if (a == null || a.Level != Level || a.ItemIndex.Row != ItemIndex.Row || a.updatePassData != updatePassData)
////                return false;

////            newY = a.newY;
////            return true;
////        }
////    }
////}

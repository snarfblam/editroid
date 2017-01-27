////using System;
////using System.Collections.Generic;
////using System.Text;
////using Editroid.UndoRedo;
////using Editroid.ROM;

////namespace Editroid.Actions
////{
////    /// <summary>
////    /// Sets the map location of a ItemScreenData object.
////    /// </summary>
////    class SetItemTilePosition:ItemAction // Todo: this class should be deleted - this kind of editing is no longer supported
        
////    {
////        int passwordDataIndex = -1;
////        public bool UpdatesPassword { get { return passwordDataIndex >= 0; } }
////        public int PasswordDataIndex { get { return passwordDataIndex; } }
////        public SetItemTilePosition(EditroidUndoRedoQueue q, Level level, ItemScreenData screen, int newX, bool updatePass)
////            : base(q, level, screen, null) {
////            this.newX = newX;
////            this.oldX = screen.MapX;
////            this.mapY = screen.MapY;

////            if (updatePass) {
////                passwordDataIndex = q.Rom.PasswordData.GetDatumIndex(new ItemInstance(GetItem(), GetRow().MapY));
////            }

////        }

////        int newX, oldX;
////        int mapY;

////        public override void Do() {
////            SetPosition(oldX, newX);
////        }

////        ////private void SetPosition(int x) {
////        ////    ItemSeeker i = GetItem();
////        ////    i.MapX = x;
////        ////    if (UpdatesPassword) {
////        ////        PasswordDatum d = Queue.Rom.PasswordData.GetDatum(passwordDataIndex);
////        ////        d.MapX = x;
////        ////    }
////        ////}
////        private void SetPosition(int fromWhat, int toWhat) {
////            ////Level.ItemTable_DEPRECATED.GetRow(mapY).GetScreen(fromWhat).MapX = toWhat;
////            Screen.MapX = toWhat;
            
////            if (UpdatesPassword) {
////                PasswordDatum d = Queue.Rom.PasswordData.GetDatum(passwordDataIndex);
////                d.MapX = toWhat;
////            }
////        }

////        public override void Undo() {
////            SetPosition(newX, oldX);
////        }

////        public override string GetText() {
////            return "Move item within row";
////        }

////        public override bool TryCombine(EditroidAction newerAction) {
////            SetItemTilePosition a = newerAction as SetItemTilePosition;
            
////            // If action is same type w/ same screen, combine.
////            if (a == null || a.Screen != Screen || a.passwordDataIndex != passwordDataIndex)
////                return false;
////            this.newX = a.newX;

////            return true;
////        }
////    }
////}

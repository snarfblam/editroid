using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    public class ScreenCollection:iLab.VirtualCollection<Screen>, IRomDataParentObject
    {
        public Level Level { get; private set; }
        PointerTable pointers;

        public Bank PointerBank { get; private set; }
        public Bank DataBank { get; private set; }

        List<int> _invalidScreenIndecies = new List<int>();
        /// <summary>
        /// Gets a list of invalid screen indecies. See remarks.
        /// </summary>
        /// <remarks>
        /// <para>Some screen indecies may be invalid when the ROM is loaded. This is because their pointer
        /// has a problematic value. It may point to another screen's data, or it may have an invalid value
        /// such as FFFF or 0000.</para>
        /// <para>When the screens are saved, the invalid entries are not saved, causing any following screen's
        /// index to be reduced by one (or more if there is more than one invalid screen pointer). For example,
        /// if pointers 5 and 7 are invalid, screen 6 becomes screen 5, and screen 8 becomes screen 6.</para>
        /// <para>The world map should be corrected to account for this issue.</para>
        /// </remarks>
        public IList<int> InvalidScreenIndecies { get; private set; }

        public ScreenCollection(Level level) {
            this.Level = level;
            InvalidScreenIndecies = _invalidScreenIndecies.AsReadOnly();

            PointerBank = level.Format.RoomPtableBank;
            DataBank = level.Format.RoomDataBank;

            var ppPTable = level.Format.pPointerToRoomPTable;
            var pPTable = level.Bank.GetPtr(ppPTable);
            var oPTable = level.Bank.ToOffset(pPTable);

            pointers = new PointerTable(level.Rom, oPTable, 0, level.Format.Uses24bitScreenPointers);

            this.IsReadOnly = false;
            LoadScreens();
            this.IsReadOnly = true;
        }

        public override Screen this[int index] {
            get {
                return base[index];
            }
            set {
                base[index] = value;
            }
        }

        public PointerTable Pointers { get { return pointers; } }

        private void LoadScreens() {
            // **** The original game contains a few duplicate pointers. We have to deal
            // with them a certain way to keep screen, map, and pointer data consistient
            // and avoid overflows. When we find consecutive duplicate pointers, we only load
            // screen data for the last of the duplicates, and the rest are interpreted as
            // empty screens. When the screens are saved back to the ROM, if they are still
            // empty, they are written as zero bytes, which reproduces the doubled pointers.

            // Get the location of the first screen pointer
            var ppRoomTable = Level.Format.pPointerToRoomPTable;
            var pRoomTable = Level.Bank.GetPtr(ppRoomTable);

            int roomPtrCount = Level.Format.CalculateRoomCount();

            ////pCpu pPrevRoom = new pCpu(0);

            for (int i = 0; i < roomPtrCount; i++) {
                pCpu pRoom = Pointers[i];
                pCpu pNextRoom = Pointers[i + 1];
                //pCpu pRoom = PointerBank.GetPtr(pRoomTable + i * 2);
                //pCpu pNextRoom = PointerBank.GetPtr(pRoomTable + (i + 1) * 2);

                bool doubledPointer = pRoom.Value == pNextRoom.Value;

                var newScreen = new Screen(Level.Rom, this);
                Add(newScreen);

                if (doubledPointer) {
                    // Mark doubled pointers, and don't load data for the duplicates
                    _invalidScreenIndecies.Add(i);
                    newScreen.Offset = Level.Bank.ToOffset(pRoom);
                } else {
                    newScreen.LoadFromRom();
                }

                ////pPrevRoom = pRoom;
            }


            ////// Clear out unused pointers
            ////pCpu nullPtr = new pCpu(0xFFFF);
            ////for (int i = Count; i < roomPtrCount; i++) {
            ////    PointerBank.SetPtr(pRoomTable + i * 2, nullPtr);

            ////}
        }

        //public void AddScreen(Screen screen) {
        //    IsReadOnly = false;
        //    Add(screen);
        //    IsReadOnly = true;
        //}
        public Screen AddScreen() {
            IsReadOnly = false;
            try {
                var newScreen = new Screen(Level.Rom, this);
                Add(newScreen);
                return newScreen;
            } finally {
                IsReadOnly = true;
            }
        }

        #region IRomDataParentObject Members

        public IList<IRomDataObject> GetSubItems() {
            return RomDataObjects.EmptyNode;

        }

        public IList<LineDisplayItem> GetListItems() {
            LineDisplayItem[] items = new LineDisplayItem[Count];
            for (int i = 0; i < Count; i++) {
                items[i] = new LineDisplayItem("Screen " + i.ToString("X"), this[i].Offset, this[i].Size, Level.Rom.data);
            }
            return items;
        }

        #endregion

        #region IRomDataObject Members

        public int Offset {
            get { return this[0].Offset; }
        }

        int IRomDataObject.Size { get { return this[Count - 1].Offset + this[Count - 1].Size - this[0].Offset; } }

        bool IRomDataObject.HasListItems { get { return true; } }

        bool IRomDataObject.HasSubItems { get { return false; } }

        string IRomDataObject.DisplayName { get { return "Screens"; } }

        #endregion

        /// <summary>
        /// Removes a screen from the collection. All other screens are updated.
        /// </summary>
        /// <param name="deletedRoomIndex"></param>
        internal void DeleteRoom(int deletedRoomIndex) {
            IsReadOnly = false;
            RemoveAt(deletedRoomIndex);
            ////for (int i = 0; i < Count; i++) {
            ////    this[i].UpdateIndex(i);
            ////    this[i].ReloadData();
            ////}
            IsReadOnly = true;
        }
    }
}

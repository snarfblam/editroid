using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Editroid.ROM
{
    /// <summary>
    /// Used to initialize an ItemTable object. All other uses are deprecated.
    /// </summary>
    public class ItemLoader: IEnumerable<ItemRowEntry>, IRomDataParentObject
    {

        byte[] data;
        MetroidRom rom;
        List<ItemRowEntry> entries = new List<ItemRowEntry>();

        public Level Level { get; private set; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="currentLevelIndex"></param>
        /// <param name="data"></param>
        public ItemLoader(Level level, MetroidRom rom) {
            this.Level = level;
            this.data = rom.data;
            this.rom = rom;

            LoadEntries();
        }

        public int RowCount { get { return entries.Count; } }

        private void LoadEntries()  {
            int offset = Level.Format.ItemDataFirstLinkOffset; 
            entries.Add(new ItemRowEntry(Level.Index, rom, offset,0));

            while(entries[entries.Count - 1].HasNextEntry) {
                entries.Add(entries[entries.Count - 1].LoadNextRow());
                ////entries.Add(new ItemRowEntry(level, data, 
                ////    entries[entries.Count - 1].NextEntryOffset));
                if (entries.Count > 32) throw new Exception("Too many item row entries found. May be a circular reference or erroneous data."); // Todo: approprite exception type
            }
        }

        /////// <summary>
        /////// Re-orders gameItem data entries to prevent parsing problems caused by
        /////// entries being out of logical order.
        /////// </summary>
        ////public void ResortEntries() { // Done: this method should never be used. The type of editing that requires this sorting is no longer supported.
        ////    if(entries.Count < 2) return; // No need for sorting.

        ////    //int romDataOffset = GetItemTableOffset(currentLevelIndex);
        ////    //int workingOffset = entries[0].NextEntryPointer.RealValue - entries[1].NextEntryOffset;

        ////    // Use a bubble sorting method. SIMPLEST
        ////    bool done = false;
        ////    while(!done) {
        ////        done = true; // Assume we are done unless we find we need more sorting

        ////        for(int i = 0; i < entries.Count - 1; i++) { 
        ////            if(entries[i].MapY > entries[i + 1].MapY) { // If two are out of order
        ////                done = false; // Make at least one more sweep

        ////                // Swap pointers
                        
        ////                // go from  xTile -> T -> B -> yTile
        ////                //
        ////                // to        /--------v
        ////                //          xTile    T <- B    yTile
        ////                //                \--------^
        ////                byte swap1, swap2;
        ////                int offsetA = entries[i].Offset;
        ////                int offsetB = entries[i + 1].Offset;
        ////                int offsetX;
        ////                if(i > 0) {// if T is not the first.
        ////                    offsetX = entries[i - 1].Offset; // Gets the offset of previous entry
        ////                } else { // T is the first entry
        ////                    // Use a pointer to the pointer to the gameItem table and treat that like an entry
        ////                    // so we can use the same code
        ////                    pCpu itemTablePointerOffset = Level.Format.pPointerToItemList;
        ////                    offsetX = itemTablePointerOffset.GetDataOffset(Level.Index) - 1;
        ////                }

        ////                // Store xTile.Next(old)
        ////                swap1 = data[offsetX + 1];
        ////                swap2 = data[offsetX + 2];

        ////                // Set xTile.Next(new) to T.Next(old)
        ////                data[offsetX + 1] = data[offsetA + 1];
        ////                data[offsetX + 2] = data[offsetA + 2];

        ////                // Set T.Next(new) to B.Next(old)
        ////                data[offsetA + 1] = data[offsetB + 1];
        ////                data[offsetA + 2] = data[offsetB + 2];

        ////                // Set B.Next(new) to xTile.Next(old);
        ////                data[offsetB + 1] = swap1;
        ////                data[offsetB + 2] = swap2;

        ////                // Reflect resorted entries:
        ////                ItemRowEntry swapE = entries[i];
        ////                entries[i] = entries[i + 1];
        ////                entries[i + 1] = swapE;

        ////            }
        ////        }
        ////    }
        ////}


        /////// <summary>
        /////// Gets the offset of an gameItem table.
        /////// </summary>
        /////// <param name="currentLevelIndex">The currentLevelIndex the gameItem table belongs to.</param>
        /////// <param name="data">ROM data to obtain offset from.</param>
        /////// <returns>The offset of an gameItem table.</returns>
        ////public static int GetItemTableOffset(Level level, byte[] data) {
        ////    // Gets a pointer to the pointer to the table offset.
        ////    int pointerOffset = GetItemTablePointerOffset().GetDataOffset(level);
        ////    // Resolves to a pointer to the table offset
        ////    pCpu tablePointer = new pCpu(data, pointerOffset);
        ////    // Resolves to the table offset
        ////    return tablePointer.GetDataOffset(level);
        ////}

        /////// <summary>
        /////// Gets a offset of a pointer to the gameItem table (or a "handle" to the gameItem table).
        /////// </summary>
        /////// <returns>T offset of a pointer to the gameItem table (or a "handle" to the gameItem table).</returns>
        ////public static pCpu GetItemTablePointerOffset() {
        ////    return new pCpu(0x98, 0x95);
        ////}

        ////public ItemRowEntry GetRowByIndex(ItemIndex_DEPRECATED index) {
        ////    return GetRowByIndex(index.Row);
        ////}
        public ItemRowEntry GetRowByIndex(int index) {
            //return entries[index];
            foreach (ItemRowEntry row in entries) {
                if (row.OrderIndex == index) return row;
            }
            throw new IndexOutOfRangeException("Row index was out of range");
        }
        ////public ItemRowEntry GetRow(int mapY) {
        ////    foreach (ItemRowEntry entry in this) {
        ////        if (entry.MapY == mapY) return entry;
        ////    }

        ////    return null;
        ////}
        ////public ItemSeeker GetItem(ItemIndex_DEPRECATED index) {
        ////    ItemRowEntry row = GetRowByIndex(index);

        ////    ItemSeeker item = row.Seek();
        ////    // Seek to screen
        ////    for (int i = 0; i < index.Screen; i++) {
        ////        item.NextScreen();
        ////    }

        ////    // Seek to screen
        ////    for (int i = 0; i < index.Item; i++) {
        ////        item.NextItem();
        ////    }

        ////    return item;
        ////}
        
        ////public ItemInstance GetItemInstance(ItemIndex_DEPRECATED item) {
        ////    return new ItemInstance(GetItem(item), GetRowByIndex(item).MapY);
        ////}

        IEnumerator<ItemRowEntry> IEnumerable<ItemRowEntry>.GetEnumerator() {
            return entries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return entries.GetEnumerator();
        }

        #region IRomDataParentObject Members

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return entries.ToArray();
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            return RomDataObjects.EmptyList;
        }

        #endregion

        #region IRomDataObject Members

        int IRomDataObject.Offset { get {
                return GetFirstRowOffset();
        } }


        int IRomDataObject.Size {
            get {
                return GetTotalBytes();
            }
        }

        bool IRomDataObject.HasListItems { get { return false; } }

        bool IRomDataObject.HasSubItems { get { return true; } }
        string IRomDataObject.DisplayName { get { return "Item Data"; } }

        #endregion

        public int GetTotalBytes() {
            int size = 0;
            entries.ForEach(delegate(ItemRowEntry e) { size += e.Size; });
            return size;
        }
        /// <summary>
        /// Gets the offset of the first row IN MEMORY. This may not be the first logical row of the data.
        /// </summary>
        /// <returns></returns>
        public pRom GetFirstRowOffset() {
            int firstEntryOffset = this.entries[0].Offset;
            for (int i = 0; i < entries.Count; i++) {
                if (entries[i].Offset < firstEntryOffset)
                    firstEntryOffset = entries[i].Offset;
            }
            return (pRom)firstEntryOffset;
        }
    }
    /// <summary>
    /// Represents an gameItem data entry, which contains one or more currentLevelItems from the
    /// same currentLevelIndex on the same horizontal row on the map.
    /// </summary>
    public class ItemRowEntry: IEnumerable<ScreenItems>, IRomDataParentObject
    {
        int index;
        public int Index { get { return index; } }

        int offset;
        int orderIndex = 0;
        byte[] data;
        MetroidRom rom;
        LevelIndex level;
        public LevelIndex Level { get { return level; } }
        public MetroidRom Rom { get { return rom; } }

        /// <summary>
        /// Gets the index of this item row data.
        /// </summary>
        public int OrderIndex { get { return orderIndex; } }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="currentLevelIndex"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        public ItemRowEntry(LevelIndex level, MetroidRom rom, int offset, int index) {
            this.index = index;
            this.offset = offset;
            this.data = rom.data;
            this.level = level;
            this.rom = rom;
        }



        /// <summary>Gets the offset of this entry.</summary>
        public int Offset { get { return offset; } }

        /// <summary>
        /// Gets or sets the vertical map position of this gameItem.
        /// </summary>
        public int MapY {
            get {
                return data[offset];
            }
            set {
                data[offset] = (byte)value;
            }
        }

        /// <summary>
        /// Gets a metroid pointer to the next entry.
        /// </summary>
        public pCpu NextEntryPointer {
            get {
                return new pCpu(data, offset + 1);
            }
        }

        /// <summary>
        /// Gets the offset of the next entry.
        /// </summary>
        private int NextEntryOffset {
            get {
                return NextEntryPointer.GetDataOffset(level);
            }
        }

        /// <summary>Gets whether there is al ItemRowEntry entry following this one.</summary>
        public bool HasNextEntry {
            get {
                return data[offset + 1] != 0xFF || data[offset + 2] != 0xFF;
            }
        }

        /// <summary>
        /// Gets the next ItemRowEntry. Chech HasNextEntry first.
        /// </summary>
        /// <returns>The next ItemRowEntry.</returns>
        public ItemRowEntry LoadNextRow() {
            ItemRowEntry result = new ItemRowEntry(level, rom, NextEntryOffset, index + 1);
            result.orderIndex = orderIndex + 1;
            return result;
        }


        /// <summary>
        /// Provides an object to enumerate currentLevelItems.
        /// </summary>
        /// <returns>An ItemSeeker.</returns>
        public ItemSeeker Seek() {
            return new ItemSeeker(this);
        }


        public IEnumerator<ScreenItems> GetEnumerator() {
            return new ScreenEnumerator(Seek(), MapY);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public ScreenItems GetScreen(int mapX) {
            foreach (ScreenItems i in this) {
                if (i.MapX == mapX)
                    return i;
            }

            return null;
        }


        #region IRomDataParentObject Members

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            List<IRomDataObject> subitems = new List<IRomDataObject>();

            foreach (ScreenItems i in this) {
                subitems.Add(i);
            }

            return subitems;
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            return RomDataObjects.EmptyList;
            
        }

        #endregion

        #region IRomDataObject Members

        public int Size {
            get {
                ItemSeeker seeker = this.Seek();
                int size = seeker.ScreenEntrySize;


                while (seeker.MoreScreensPresent) {
                    seeker.NextScreen();

                    size += seeker.ScreenEntrySize;

                }


                return size + 3;
            }
        }

        bool IRomDataObject.HasListItems { get { return false; } }
        bool IRomDataObject.HasSubItems { get { return true; } }
        string IRomDataObject.DisplayName { get {return "Item Row " + MapY.ToString("X"); } }

        #endregion


    }

    /// <summary>
    /// Enumerates screens in a row.
    /// </summary>
    public class ScreenEnumerator : IEnumerator<ScreenItems>
    {
        ItemSeeker data;
        ScreenItems current;
        ScreenItems next;
        int mapY;
        public ScreenEnumerator(ItemSeeker data, int mapY) {
            this.data = data;
            this.next = new ScreenItems(data,mapY);
            this.mapY = mapY;
        }
        public ScreenItems Current {
            get { return current; }
        }

        public void Dispose() {
            current = null;
        }

        object System.Collections.IEnumerator.Current {
            get { return current; }
        }

        public bool MoveNext() {
            current = next;

            if (data.MoreScreensPresent) {
                data.NextScreen();
                next = new ScreenItems(data, mapY);
            } else {
                next = null;
            }

            return current != null;
        }

        public void Reset() {
            data.SeekToFirstScreen();
            current = null;
            next = new ScreenItems(data, mapY);
        }
    }

    /// <summary>
    /// Enumerates items in a screen
    /// </summary>
    public class ScreenItems: IEnumerable<ItemSeeker>, IRomDataParentObject
    {
        ItemSeeker data;
        int mapY;
        public ScreenItems(ItemSeeker data, int mapY) {
            this.data = data;
            this.mapY = mapY;
        }

        IEnumerator<ItemSeeker> IEnumerable<ItemSeeker>.GetEnumerator() {
            return new ItemEnumerator(data, mapY);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return new ItemEnumerator(data, mapY);
        }

        public int MapX {
            get { return data.MapX; }
            set {
                data.MapX = value;
            }
        }

        public ItemSeeker GetItem(int index) {
            int i = 0;
            foreach (ItemSeeker item in this) {
                if (i == index)
                    return item;
                i++;
            }

            return new ItemSeeker();
        }

        #region IRomDataObject Members

        int IRomDataObject.Offset { get { return data.screenOffset; } }
        int IRomDataObject.Size { get { return data.ScreenEntrySize; } }
        bool IRomDataObject.HasListItems { get { return true; } }
        bool IRomDataObject.HasSubItems { get { return false; } }

        string IRomDataObject.DisplayName { get { return "Screen Entry"; } }

        #endregion

        #region IRomDataParentObject Members

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return RomDataObjects.EmptyNode;
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() { // Tode: test, make sure this works
            List<LineDisplayItem> items = new List<LineDisplayItem>();
            var seeker = this.data;
            //foreach(ItemInstance item in this){

            items.Add(CreateLineDisplayItem(seeker));
            while (seeker.MoreItemsPresent) {
                seeker.NextItem();
                items.Add(CreateLineDisplayItem(seeker));
            }

            //}
                return items;
        }

        private static LineDisplayItem CreateLineDisplayItem(ItemSeeker seeker) {
            string text = seeker.ItemType.ToString();
            if (seeker.ItemType == ItemTypeIndex.PowerUp)
                text += ": " + seeker.PowerUp.ToString();
            var newItem = new LineDisplayItem(text, seeker.itemOffset, seeker.CurrentItemDataSize, seeker.Data);
            return newItem;
        }

        #endregion
    }
    /// <summary>
    /// Enumerates items in a screen 
    /// </summary>
    public class ItemEnumerator : IEnumerator<ItemSeeker>
    {
        ItemSeeker current;
        ItemSeeker nextData;
        int mapY;
        bool noMoreItems = false;

        public ItemEnumerator(ItemSeeker data, int MapY) {
            nextData = data;
            this.mapY = MapY;
        }

        public ItemSeeker Current {
            get {
                return current;
            }
        }

        public void Dispose() {
        }

        object System.Collections.IEnumerator.Current {
            get { return current; }
        }

        public bool MoveNext() {
            if (noMoreItems) return false;

            current = nextData;

            if (nextData.MoreItemsPresent) {
                nextData.NextItem();
            } else {
                noMoreItems = true;
            }
            return true;
        }

        public void Reset() {
            nextData.SeekToFirstItemInScreen();
        }
    }
    /// <summary>
    /// Provides a mechanism to seek through screen entries in an gameItem data entry,
    /// and thorugh currentLevelItems if there are multiple currentLevelItems in a screen.
    /// </summary>
    public struct ItemSeeker
    {
        /// <summary>The offset of the item row entry being examined.</summary>
        public int startOffset;
        /// <summary>The offset of the screen entry being examined.</summary>
        public int screenOffset;
        /// <summary>The offset of the individual item being examined.</summary>
        public int itemOffset;
        byte[] data;

        private ItemIndex_DEPRECATED itemIndex;
        public ItemIndex_DEPRECATED ID {
            get { return itemIndex; }
        }

        public int GetRemainingItemsCount() {
            ItemSeeker i = this;
            int count = 1;
            while (i.MoreItemsPresent) {
                count++;
                i.NextItem();

                if (count >= 16) return count;
            }

            return count;
        }

        public string PowerUpName {
            get {
                switch (PowerUp) {
                    case PowerUpType.Bomb:
                        return "bombs";
                    case PowerUpType.HiJump:
                        return "high jump";
                    case PowerUpType.LongBeam:
                        return "long beam";
                    case PowerUpType.ScrewAttack:
                        return "screw attack";
                    case PowerUpType.MaruMari:
                        return "maru mari";
                    case PowerUpType.Varia:
                        return "varia";
                    case PowerUpType.WaveBeam:
                        return "wave beam";
                    case PowerUpType.IceBeam:
                        return "ice beam";
                    case PowerUpType.EnergyTank:
                        return "energy tank";
                    case PowerUpType.Missile:
                        return "missiles";
                    default:
                        return "None";
                }
            }
        }
        /// <summary>Gets the ROM data this object represents.</summary>
        public byte[] Data { get { return data; } }


        /// <summary>
        /// Creates an item seeker.
        /// </summary>
        /// <param name="data">ItemRowEntry to seek.</param>
        public ItemSeeker(ItemRowEntry data) {
            this.data = data.Rom.data;
            this.startOffset = this.screenOffset = data.Offset + 3;
            itemOffset = startOffset + 2;

            itemIndex = new ItemIndex_DEPRECATED();
            itemIndex.Row = (byte)data.OrderIndex;
            itemIndex.Level = data.Level;
        }

        public int EnemyTypeIndex {
            get {
                return data[itemOffset + 1] & 0x0F;
            }
            set {
                data[itemOffset + 1] =(byte)(
                    (data[itemOffset + 1] & 0xF0)
                | (byte)(value & 0x0F));
            }
        }
        public bool EnemyIsHard {
            get {
                return data[itemOffset + 1] > 0x7F;
            }
            set {
                if (value)
                    data[itemOffset + 1] |= 0x80;
                else
                    data[itemOffset + 1] &= 0x7F;
            }
        }
        /// <summary>
        /// Gets/sets the x-coordinate of the screen being examined.
        /// </summary>
        public int MapX {
            get {
                return data[screenOffset];
            }
            set {
                data[screenOffset] = (byte)value;
            }
        }

        /// <summary>
        /// Gets the sprite slot. This value is irrelevant unless this data represents an enemy.
        /// </summary>
        public int SpriteSlot {
            get {
                return (data[itemOffset] & 0xF0) / 0x10; // Get upper nibble value.
            }
            set {
                byte bval = (byte)((value & 0x0F) * 0x10); // Convert to upper nibble
                data[itemOffset] =(byte)((data[itemOffset] & 0x0F) | bval); // Replace upper nibble of ROM data.
            }
        }

        /// <summary>
        /// Gets/sets what type of gameItem is being examined.
        /// </summary>
        public ItemTypeIndex ItemType {
            get {
                return (ItemTypeIndex)(
                    data[itemOffset] & 0x0F); // Return lower nibble
            }
            set {
                byte bval = (byte)value;
                data[itemOffset] = (byte)(
                    (data[itemOffset] & 0xF0) | bval); // Apply to lower nibble
            }
        }
        /// <summary>
        /// Gets/sets what type of gameItem is being examined.
        /// </summary>
        public Byte ItemTypeByte {
            get {
                return data[itemOffset]; // Return lower nibble
            }
            set {
                data[itemOffset] = value; // Apply to lower nibble
            }
        }

        /// <summary>
        /// Gets the type of item being examined, if the current gameItem is a item.
        /// </summary>
        public PowerUpType PowerUp {
            get {
                return (PowerUpType)data[itemOffset + 1];
            }
            set {
                data[itemOffset + 1] = (byte)value;
            }
        }

        /// <summary>
        /// Gets the item sub type (i.e. power up type, enemy type, elevator dest, ect)
        /// </summary>
        public byte SubTypeByte { get { return data[itemOffset + 1]; } }

        public DoorSide DoorSide { get { return (DoorSide)(data[itemOffset + 1] & 0xF0); } }
        public DoorType DoorType{ get { return (DoorType)(data[itemOffset + 1] & 0x0F); } }
        /// <summary>
        /// Gets/sets the destination of the elevator being examined, if the current gameItem is an elevator.
        /// </summary>
        public ElevatorDestination Destination {
            get {
                return (ElevatorDestination)data[itemOffset + 1];
            }
            set {
                data[itemOffset + 1] = (byte)value;
            }
        }



        /// <summary>
        /// Gets/sets the position of a power-up, enemy, or turret.
        /// </summary>
        public ScreenCoordinate ScreenPosition {
            get {
                switch (ItemType) {
                    case ItemTypeIndex.Enemy:
                    case ItemTypeIndex.PowerUp:
                        return new ScreenCoordinate(data[itemOffset + 2]);
                    case ItemTypeIndex.Turret:
                        return new ScreenCoordinate(data[itemOffset + 1]);
                }

                return new ScreenCoordinate(0, 0);
            }
            set {
                switch (ItemType) {
                    case ItemTypeIndex.Enemy:
                    case ItemTypeIndex.PowerUp:
                        data[itemOffset + 2] = value.Value;
                        break;
                    case ItemTypeIndex.Turret:
                        data[itemOffset + 1] = value.Value;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the size, in bytes, of the data for the current gameItem.
        /// </summary>
        public int CurrentItemDataSize {
            get {
                return ItemType.GetItemType().NetByteCount;
                ////switch(ItemType) {
                ////    case ItemTypeIndex.Enemy:
                ////    case ItemTypeIndex.PowerUp:
                ////        return 3;
                ////    case ItemTypeIndex.Mella:
                ////    case ItemTypeIndex.Rinkas:
                ////    case ItemTypeIndex.PalSwap:
                ////    case ItemTypeIndex.Zebetite:
                ////    case ItemTypeIndex.MotherBrain:
                ////        return 1;
                ////    case ItemTypeIndex.Elevator:
                ////    case ItemTypeIndex.Door:
                ////    case ItemTypeIndex.Turret:
                ////        return 2;
                ////    case ItemTypeIndex.Nothing:
                ////    case ItemTypeIndex.Unused_b:
                ////    case ItemTypeIndex.Unused_c:
                ////    case ItemTypeIndex.Unused_d:
                ////    case ItemTypeIndex.Unused_e:
                ////    case ItemTypeIndex.Unused_f:
                ////    default:
                ////        return 1;
                ////}
            }
        }



        /// <summary>
        /// Returns true if there are more currentLevelItems in the screen being examined.
        /// </summary>
        public bool MoreItemsPresent {
            get { return data[itemOffset + CurrentItemDataSize] != 0x00; }
        }

        /// <summary>
        /// Seeks to the next gameItem, if there are more currentLevelItems in the screen.
        /// </summary>
        public void NextItem() {
            itemOffset += CurrentItemDataSize;
            itemIndex.Item++;
        }
        /// <summary>
        /// Seeks to the first gameItem in the screen being examined.
        /// </summary>
        public void SeekToFirstItemInScreen(){
            itemOffset = screenOffset + 2;
            itemIndex.Item = 0;
        }

        /// <summary>
        /// Seeks to the first room in the gameItem entry.
        /// </summary>
        public void SeekToFirstScreen() {
            screenOffset = startOffset;
            itemOffset = startOffset + 2;

            itemIndex.Item = 0;
            itemIndex.Screen = 0;
        }

        /// <summary>
        /// Gets true if there are more screens in the entry.
        /// </summary>
        public bool MoreScreensPresent {
            get {
                return data[screenOffset + 1] != 0xFF && data[screenOffset + 1] != 0;
            }
        }

        /// <summary>
        /// Gets/sets size of this screen's gameItem data.
        /// </summary>
        public byte ScreenEntrySize {
            get {
                if (data[screenOffset + 1] == 0xFF) {
                    ItemSeeker itm = this;

                    itm.SeekToFirstItemInScreen();
                    int size = 3 + itm.CurrentItemDataSize;
                    while (itm.MoreItemsPresent) {
                        itm.NextItem();
                        size += itm.CurrentItemDataSize;
                    }

                    return (byte)(size & 0xFF);

                }
                return data[screenOffset + 1];
            }
            set {
                data[screenOffset + 1] = value;
            }
        }      
        /// <summary>
        /// Gets/sets size of this screen's gameItem data.
        /// </summary>
        public byte ScreenEntrySizeByte {
            get {
                return data[screenOffset + 1];
            }
        }

        /// <summary>
        /// Seeks to the next screen, if there are more screens in this gameItem data entry.
        /// </summary>
        /// <returns>TRUE if the seeker is pointing at another screen data after the operation, or false if the seeker is beyond the end of screen data.</returns>
        public bool NextScreen() {
            bool hasMore = this.MoreScreensPresent;

            screenOffset += ScreenEntrySize;
            if (ScreenEntrySize == 0) return false;

            itemOffset = screenOffset + 2;

            itemIndex.Screen++;
            itemIndex.Item = 0;

            return hasMore;
        }
    }

    public struct ItemID
    {
        byte mapX, mapY, index;
        public int MapX {
            get { return mapX; }
            set { mapX = (byte)(value & 0xFF); }
        }
        public int MapY {
            get { return mapY; }
            set { mapY = (byte)(value & 0xFF); }
        }
        public int Index {
            get { return index; }
            set { index = (byte)(value & 0xFF); }
        }
        public Point MapPosition {
            get { return new Point(MapX, MapY); }
            set { MapX = value.X; MapY = value.Y; }
        }
    }

    /// <summary>
    /// Identifies an item row entry, screen entry, or item entry, within a
    /// level. A Composite value can be retrieved via the Composite property.
    /// </summary>
    //[StructLayout(LayoutKind.Explicit)]
    public struct ItemIndex_DEPRECATED
    {
        public byte Item;
        public byte Screen;
        public byte Row;
        public byte level;

        public LevelIndex Level { 
            get { return (LevelIndex)level; }
            set { level = (byte)(int)value; }
        }


        /////// <summary>
        /////// Gets a new ItemInstance describing the specified item.
        /////// </summary>
        /////// <param name="r">The ROM containing the item the ID identifies.</param>
        /////// <returns>A newly created ItemInstance.</returns>
        ////public ItemInstance GetItem(MetroidRom r) {
        ////    ItemTable items = r.GetLevel(Level).ItemTable_DEPRECATED;
        ////    return new ItemInstance(
        ////        items.GetItem(this),
        ////        items.GetRowByIndex(this).MapY);
        ////}

        public bool IsInSameLevel(ItemIndex_DEPRECATED item) {
            return this.level == item.level;
        }
        public bool IsInSameRowEntry(ItemIndex_DEPRECATED item) {
            return this.level == item.level
                && this.Row == item.Row;
        }
        public bool IsInSameScreen(ItemIndex_DEPRECATED item) {
            return this.level == item.level
                && this.Row == item.Row
                && this.Screen == item.Screen;
            
        }

 
    }


    /// <summary>Represents different power ups.</summary>
    public enum PowerUpType:byte
    {
        /// <summary>Maru mari bombs.</summary>
        Bomb = 0,
        /// <summary>High jump boots.</summary>
        HiJump = 1,
        /// <summary>Long beam.</summary>
        LongBeam = 2,
        /// <summary>Screw attack.</summary>
        ScrewAttack = 3,
        /// <summary>Maru Mari ("morph ball")</summary>
        MaruMari = 4,
        /// <summary>Varia suit.</summary>
        Varia = 5,
        /// <summary>Wave beam.</summary>
        WaveBeam = 6,
        /// <summary>Ice beam.</summary>
        IceBeam = 7,
        /// <summary>Energy tank.</summary>
        EnergyTank = 8,
        /// <summary>Missile expansion.</summary>
        Missile = 9,
        /// <summary>Missile expansion with incorrect graphics.</summary>
        Invalid_Missile_Dot = 0xA,
        /// <summary>Missile expansion with incorrect graphics.</summary>
        Invalid_Missile_Arrow = 0xB,
        /// <summary>Unknown.</summary>
        Unknown_c = 0xC,
        /// <summary>Unknown.</summary>
        Unknown_d = 0xD,
        /// <summary>Unknown.</summary>
        Unknown_e = 0xE,
        /// <summary>Unknown.</summary>
        Unknown_f = 0xF
    }

    /// <summary>Represents destinations for elevators.</summary>
    public enum ElevatorDestination
    {
        BrinstarToBrinstar = 0,
        /// <summary>Brinstar.</summary>
        BrinstarToNorfair = 1, 
        /// <summary>Norfair.</summary>
        BrinstarToKraid = 2,
        /// <summary>Kraid.</summary>
        BrinstarToTourian = 3,
        /// <summary>Tourian.</summary>
        NorfairToRidley = 4,
        NorfairExit = 0x81,
        KraidExit = 0x82,
        TourianExit = 0x83,
        RidleyExit = 0x84,
        EndOfGame = 0x8F
    }

    /// <summary>
    /// Represents the screen coordinate of an gameItem, object, or enemy. This object is not directly tied to ROM 
    /// data and setting its properties does not modify the rom. It is intended to be passed into
    /// fonctions that do modify the rom.
    /// </summary>
    public struct ScreenCoordinate
    {
        byte val;
        /// <summary>
        /// The xTile coordinate this object represents.
        /// </summary>
        public int X {
            get {
                return val & 0xF; 
            }
            set {
                val = (byte)((val & 0xF0) | (value & 0xF)); 
            }
        }
        /// <summary>
        /// The yTile coordinate this object represents.
        /// </summary>
        public int Y {
            get {
                return (val & 0xF0) / 0x10;
            }
            set {
                val = (byte)((val & 0x0F) | ((value * 0x10) & 0xFF));
            }
        }

        /// <summary>
        /// Instantiates this object.
        /// </summary>
        /// <param name="value">The byte that contains the data.</param>
        public ScreenCoordinate(byte value) {
            val = value;
        }
        /// <summary>
        /// Instantiates this object.
        /// </summary>
        /// <param name="x">xTile position.</param>
        /// <param name="y">yTile position.</param>
        public ScreenCoordinate(int x, int y) {
            val = 0;
            X = x;
            Y = y;
        }
        /// <summary>
        /// Gets/sets the data for this object.
        /// </summary>
	    public byte Value
	    {
		    get { return val;}
		    set { val = value;}
	    }

    }
}

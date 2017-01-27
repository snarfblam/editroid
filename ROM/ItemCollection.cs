using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Editroid.ROM
{
    /// <summary>
    /// Represents a list of items for a level, grouped by screen.
    /// </summary>
    public class ItemCollection : List<ItemScreenData>
    {
        public Level Level { get; private set; }

        /// <summary>
        /// Returns the size of the item data in the ROM image, as of last time item data was
        /// saved to or loaded from the in-memory ROM image.
        /// </summary>
        public int SerializedSize { get; private set; } 

        public ItemCollection(Level level) {
            this.Level = level;
        }

        public void LoadItems() {
            ItemLoader itemTable = Level.ItemTable_DEPRECATED;

            Clear();

            for (int i = 0; i < itemTable.RowCount; i++) {
                LoadRow(itemTable.GetRowByIndex(i));
            }

            SerializedSize = itemTable.GetTotalBytes();
        }


        private void LoadRow(ItemRowEntry row) {
            var seeker = row.Seek();
            LoadScreen(seeker, row.MapY);
            while (seeker.MoreScreensPresent) {
                seeker.NextScreen();
                LoadScreen(seeker, row.MapY);
            }
        }

        private void LoadScreen(ItemSeeker seeker, int mapY) {
            var data = new ItemScreenData();
            data.MapX = seeker.MapX;
            data.MapY = mapY;

            var item = LoadItem(seeker);
            if (item != null)
                data.Items.Add(item);

            while (seeker.MoreItemsPresent) {
                seeker.NextItem();
                item = LoadItem(seeker);
                if (item != null)
                    data.Items.Add(item);
            }

            Add(data);
            
        }

        private ItemData LoadItem(ItemSeeker seeker) {
            ItemData result;
            switch (seeker.ItemType) {
                case ItemTypeIndex.Enemy:
                    result = new ItemEnemyData();
                    break;
                case ItemTypeIndex.PowerUp:
                    result = new ItemPowerupData();
                    break;
                case ItemTypeIndex.Mella:
                case ItemTypeIndex.Rinkas:
                case ItemTypeIndex.MotherBrain:
                case ItemTypeIndex.PalSwap:
                case ItemTypeIndex.Zebetite:
                    result = new ItemSingleByteData();
                    break;
                case ItemTypeIndex.Elevator:
                    result = new ItemElevatorData();
                    break;
                case ItemTypeIndex.Turret:
                    result = new ItemTurretData();
                    break;
                case ItemTypeIndex.Door:
                    result = new ItemDoorData();
                    break;
                case ItemTypeIndex.Nothing:
                case ItemTypeIndex.Unused_b:
                case ItemTypeIndex.Unused_c:
                case ItemTypeIndex.Unused_d:
                case ItemTypeIndex.Unused_e:
                case ItemTypeIndex.Unused_f:
                default:
                    return null;
            }

            result.LoadData(seeker);
            return result;
        }

        #region saving
        public void SaveItemData() {
            int calculatedSize = CalculateDataSize();
            int sizeDiff = calculatedSize - SerializedSize;

            if (Level.Format.ItemExpansionMode == Editroid.ROM.Formats.ItemExpansion.ExpandBackward) {
                ApplyItemData(-sizeDiff);
            } else {
                ApplyItemData(0);
            }
        }

        /// <summary>
        /// Sets the item data pointer to the address of the current beginning of the item data plus the value
        /// specified as 'offset'.
        /// </summary>
        /// <param name="offset">The amount to offset the address of the item data pointer.</param>
        /// <returns></returns>
        pRom OffsetItemPointer(int offset) {
            var currentOffset = Level.Format.ItemDataOffset;
            var newOffset = currentOffset + offset;
            var pNewLocation = Level.Bank.ToPtr(newOffset);

            Level.Bank.SetPtr(Level.Format.pPointerToItemList, pNewLocation);
            ////level.PRom[level.Format.PointerToItemList] = newOffset;
            return newOffset;
        }


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeInOffset">The distance the beginning data should be written to, relative to the current location.</param>
        private void ApplyItemData(int changeInOffset) {
            SortScreenData();

            int currentRow = -1;
            bool lastRow = false;
            //int currentOffset = originalData.GetFirstRowOffset();
            int currentOffset = (int)OffsetItemPointer(changeInOffset);

            byte[] data = Level.Rom.data;

            // Each row has a pointer to next row. This can't be written until we actually get
            // to the next row, so we remember where the pointer is so that when we start next
            // row we can go back and set the pointer.
            int pendingPointerOffset = -1;

            for (int i = 0; i < Count; i++) {
                var screen = this[i];
                Point location = new Point(screen.MapX, screen.MapY);

                bool lastScreenInRow = (i == Count - 1) || (screen.MapY != this[i + 1].MapY);

                // Process new row, if applicable
                if (location.Y != currentRow) {
                    System.Diagnostics.Debug.Assert(currentRow < location.Y);

                    currentRow = location.Y;

                    // Write pointer to this new row:
                    if (pendingPointerOffset != -1) {
                        var pointer = Level.CreatePointer((pRom)currentOffset);
                        data[pendingPointerOffset] = pointer.Byte1;
                        data[pendingPointerOffset + 1] = pointer.Byte2;
                    }

                    // Specify map Y
                    data[currentOffset] = (byte)currentRow;
                    currentOffset++;

                    // Mark pointer to next row, and seek past it
                    pendingPointerOffset = currentOffset;
                    currentOffset += 2;
                }

                var items = this[i].Items;

                int screenBytes = 3; // 2 for header [mapX, size], 1 for footer [$00]
                // Calculate size of screen data
                for (int iItem = 0; iItem < items.Count; iItem++) {
                    screenBytes += items[iItem].Size;
                }

                // Write screen header
                data[currentOffset] = (byte)location.X;
                currentOffset++;
                data[currentOffset] = (byte)(lastScreenInRow ? 0xFF : screenBytes);
                currentOffset++;

                // Write item data
                for (int iItem = 0; iItem < items.Count; iItem++) {
                    items[iItem].WriteData(data, ref currentOffset);
                }

                data[currentOffset] = 0;
                currentOffset++;
            }

            // Last row has 0xFFFF for a pointer
            data[pendingPointerOffset] = 0xFF;
            data[pendingPointerOffset + 1] = 0xFF;

        }

        int CalculateDataSize() {
            int currentRow = -1;
            int totalBytes = 0;

            for (int iScreen = 0; iScreen < Count; iScreen++) {
                totalBytes += 3; // Each screen has 2 byte header and 1 byte footer
                var screen = this[iScreen];

                Point location = new Point(screen.MapX, screen.MapY);

                if (location.Y != currentRow) {
                    System.Diagnostics.Debug.Assert(currentRow < location.Y);

                    currentRow = location.Y;
                    totalBytes += 3; // Each row has 3 byte header
                }

                for (int iItem = 0; iItem < screen.Items.Count; iItem++) {
                    totalBytes += screen.Items[iItem].Size;
                }

            }
            return totalBytes;
        }

        private void SortScreenData() {
            Sort(screenSorter);
        }

        int screenSorter(ItemScreenData a, ItemScreenData b) {
            int aVal = a.MapX + a.MapY * 32;
            int bVal = b.MapX + b.MapY * 32;

            return aVal - bVal;
        }

        #endregion
    }

    public class ItemScreenData
    {
        public ItemScreenData() {
            Items = new List<ItemData>();
        }
        public IList<ItemData> Items { get; private set; }
        int mapX;
        public int MapX { get { return mapX; } set { mapX = value & 0x1f; } }
        int mapY;
        public int MapY { get { return mapY; } set { mapY = value & 0x1f; } }


    }
    public abstract class ItemData
    {
        [Browsable(false)]
        public abstract int Size { get; }

        ItemTypeIndex type;
        [Browsable(false)]
        public ItemTypeIndex ItemType {
            get { return type; }
            protected set {
                bool wasSingleByte = ((IList<ItemTypeIndex>)singleByteTypes).Contains(type);
                bool willBeSingleByte = ((IList<ItemTypeIndex>)singleByteTypes).Contains(value);
                if (!wasSingleByte || !willBeSingleByte) throw new InvalidOperationException("ItemData.ItemType property may only be modified from one single-byte type to another.");

                type = value;

            } 
        }

        private static ItemTypeIndex[] singleByteTypes = { ItemTypeIndex.MotherBrain, ItemTypeIndex.Zebetite, ItemTypeIndex.Mella, ItemTypeIndex.Rinkas, ItemTypeIndex.PalSwap };

        public ItemData() { }
        public ItemData(ItemTypeIndex type) { this.type = type; }

        int spriteSlot;
        public int SpriteSlot { get { return spriteSlot; } set { spriteSlot = value & 0xF; } }


        public virtual void LoadData(ItemSeeker data) {
            this.type = data.ItemType;
            this.SpriteSlot = data.SpriteSlot;
        }

        public virtual void WriteData(byte[] data, ref int offset) {
            data[offset] = (byte)((int)ItemType | spriteSlot << 4);
            offset++;
        }

        protected static void WritePoint(byte[] data, ref int offset, Point point) {
            ScreenCoordinate coord = new ScreenCoordinate(point.X, point.Y);
            data[offset] = coord.Value;
            offset++;
        }

    }
    public class ItemSingleByteData : ItemData
    {

        public ItemSingleByteData() { }
        public ItemSingleByteData(ItemTypeIndex type) : base(type) { }

        [Browsable(false)]
        public byte SingleByteItemType {
            get { return (byte)((int)ItemType | (SpriteSlot << 4)); }
            set {
                ItemType = (ItemTypeIndex)(value & 0x0F);
                SpriteSlot = (value >> 4);

            }
        }

        public override int Size {
            get { return 1; }
        }

    }
    public class ItemTurretData : ItemData, IItemScreenPosition
    {
        public ItemTurretData() : base(ItemTypeIndex.Turret) { }
        public override int Size {
            get { return 2; }
        }

        Point screenPosition;
        public Point ScreenPosition { get { return screenPosition; } set { screenPosition = new Point(value.X & 0xf, value.Y & 0xf); } }

        public override void LoadData(ItemSeeker data) {
            base.LoadData(data);
            screenPosition = new Point(data.ScreenPosition.X, data.ScreenPosition.Y);
        }

        public override void WriteData(byte[] data, ref int offset) {
            base.WriteData(data, ref offset);

            WritePoint(data, ref offset, screenPosition);
        }

    }
    public class ItemElevatorData : ItemData
    {
        public ItemElevatorData() : base(ItemTypeIndex.Elevator) { }
        public override int Size {
            get { return 2; }
        }

        public ElevatorDestination ElevatorType { get; set; }

        public override void LoadData(ItemSeeker data) {
            base.LoadData(data);

            ElevatorType = data.Destination;
        }

        public override void WriteData(byte[] data, ref int offset) {
            base.WriteData(data, ref offset);

            data[offset] = (byte)ElevatorType;
            offset++;
        }
    }
    public class ItemDoorData : ItemData
    {
        public ItemDoorData() : base(ItemTypeIndex.Door) { }
        public override int Size {
            get { return 2; }
        }

        public DoorSide Side { get; set; }
        public DoorType Type { get; set; }

        public override void LoadData(ItemSeeker data) {
            base.LoadData(data);

            Side = data.DoorSide;
            Type = data.DoorType;
        }

        public override void WriteData(byte[] data, ref int offset) {
            base.WriteData(data, ref offset);

            data[offset] = (byte)((int)Side | (int)Type);
            offset++;
        }
    }
    public abstract class Item3Data : ItemData, IItemScreenPosition
    {
        public Item3Data(ItemTypeIndex type) : base(type) { }
        public override int Size { get { return 3; } }

        Point screenPosition;
        public Point ScreenPosition { get { return screenPosition; } set { screenPosition = new Point(value.X & 0xf, value.Y & 0xf); } }

        public override void LoadData(ItemSeeker data) {
            base.LoadData(data);

            screenPosition = new Point(data.ScreenPosition.X, data.ScreenPosition.Y);
        }
    }
    public class ItemEnemyData : Item3Data
    {
        public ItemEnemyData() : base(ItemTypeIndex.Enemy) { }
        public bool Difficult { get; set; }
        int enemyType;
        public int EnemyType { get { return enemyType; } set { enemyType = value & 0xf; } }

        public override void LoadData(ItemSeeker data) {
            base.LoadData(data);

            Difficult = data.EnemyIsHard;
            enemyType = data.EnemyTypeIndex;
        }

        public override void WriteData(byte[] data, ref int offset) {
            base.WriteData(data, ref offset);

            data[offset] = (byte)(enemyType | (Difficult ? 0x80 : 0));
            offset++;

            WritePoint(data, ref offset, ScreenPosition);
        }
    }
    public class ItemPowerupData : Item3Data
    {
        public ItemPowerupData() : base(ItemTypeIndex.PowerUp) { }

        public PowerUpType PowerUp { get; set; }

        public override void LoadData(ItemSeeker data) {
            base.LoadData(data);

            PowerUp = data.PowerUp;
        }

        public override void WriteData(byte[] data, ref int offset) {
            base.WriteData(data, ref offset);

            data[offset] = (byte)PowerUp;
            offset++;

            WritePoint(data, ref offset, ScreenPosition);
        }
    }
    public interface IItemScreenPosition
    {
        Point ScreenPosition { get; set; }
    }
}

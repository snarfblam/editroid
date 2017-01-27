using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;
using Editroid;
using System.Diagnostics;
using iLab;

namespace Editroid
{
    public partial class frmAdvancedItems : Form
    {
        List<ItemEditorScreenControl> Screens = new List<ItemEditorScreenControl>();
        ItemEditorScreenControl _selectedScreen;

        public const int MapTileWidth = 16;
        public const int MapTileHeight = 16;


        ItemEditorScreenControl SelectedScreen {
            get { return _selectedScreen; }
            set {
                if (_selectedScreen != null)
                    _selectedScreen.Selected = false;
                _selectedScreen = value;
                if (_selectedScreen == null) {
                    UpdateForNoSelectedScreen();
                }else{
                    _selectedScreen.Selected = true;
                    UpdateForSelectedScreen();
                }
            }
        }

        int originalSize;
        int totalAvailBytes;
        int calculatedSize;

        ItemLoader originalData;
        Level level;

        public frmAdvancedItems() {
            InitializeComponent();

            drawnMap = new Bitmap(32, 32, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gDrawMap = Graphics.FromImage(drawnMap);
            this.Disposed += new EventHandler(frmAdvancedItems_Disposed);
        }

        Bitmap drawnMap;
        Graphics gDrawMap;

        ItemTypeIndex ItemTypeToAdd = ItemTypeIndex.Enemy;

        public void LoadItemData(Level level) {
            ItemLoader data = level.ItemTable_DEPRECATED;
            originalData = data;
            this.level = level;

            ClearItemData();

            for (int i = 0; i < data.RowCount; i++) {
                LoadRow(data.GetRowByIndex(i));
            }

            originalSize = data.GetTotalBytes();

            totalAvailBytes = level.Format.AvailableItemMemory;

            CalculateDataSize();

            Text += ": " + level.Index.ToString();
        }

        private void ClearItemData() {
            foreach (var screen in Screens) {
                RemoveAndDisposeScreenControl(screen);
            }

            Screens.Clear();
        }

        /// <summary>
        /// Removes the specified screen control from the UI and collection and unwires events
        /// </summary>
        /// <param name="screen"></param>
        private void RemoveAndDisposeScreenControl(ItemEditorScreenControl screen) {
            Screens.Remove(screen);
            picMap.Controls.Remove(screen);
            screen.Dispose();
            screen.MouseDown -= new MouseEventHandler(OnScreenMouseDown);
            screen.ScreenMoved -= new EventHandler(OnScreenMoved);
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
            ItemEditorScreenControl control = new ItemEditorScreenControl();
            var data = control.Data;
            control.MapLocation = new Point(seeker.MapX, mapY);


            var item = LoadItem(seeker);
            if (item != null)
                data.Items.Add(item);

            while (seeker.MoreItemsPresent) {
                seeker.NextItem();
                item = LoadItem(seeker);
                if (item != null)
                    data.Items.Add(item);
            }

            AddScreenControl(control);
            
        }

        /// <summary>
        /// Adds the specified screen control to the UI and screen collection and wires events.
        /// </summary>
        /// <param name="control"></param>
        private void AddScreenControl(ItemEditorScreenControl control) {
            Screens.Add(control);
            picMap.Controls.Add(control);
            control.MouseDown += new MouseEventHandler(OnScreenMouseDown);
            control.ScreenMoved += new EventHandler(OnScreenMoved);
        }

        void OnScreenMouseDown(object sender, MouseEventArgs e) {
            SelectedScreen = sender as ItemEditorScreenControl;
        }
        void OnScreenMoved(object sender, EventArgs e) {
            CalculateDataSize();
        }


        void frmAdvancedItems_Disposed(object sender, EventArgs e) {
            gDrawMap.Dispose();
            drawnMap.Dispose();
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


        private void ScreenList_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateForSelectedScreen();
        }


        private void UpdateForNoSelectedScreen() {
            ItemList.Items.Clear();
            UpdateForSelectedItem();

            btnRemoveScreen.Enabled = false;

        }

        private void UpdateForSelectedScreen() {
            ItemList.Items.Clear();

            var screenData = SelectedScreen.Data;

            for (int i = 0; i < screenData.Items.Count; i++) {
                var itemData = screenData.Items[i];
                var itemItem = new ListViewItem(itemData.ItemType.ToString());
                itemItem.Tag = itemData;

                ItemList.Items.Add(itemItem);
            }

            ItemList.SelectedIndices.Clear();
            if (ItemList.Items.Count > 0)
                ItemList.SelectedIndices.Add(0);
            else
                UpdateForSelectedItem(); // Need to manually invoke update, if there are no items SelectedIndexChanged is not raised.

            btnRemoveScreen.Enabled = true;

        }

        private void ItemList_SelectedIndexChanged(object sender, EventArgs e) {

            UpdateForSelectedItem();
        }

        private void UpdateForSelectedItem() {
            bool itemSelected = ItemList.SelectedIndices.Count > 0;
            bool screenSelected = SelectedScreen != null;
            
            btnAddItem.Enabled = screenSelected;
            btnDeleteItem.Enabled = itemSelected;

            // Show selected item in property grid
            if (itemSelected) {
                propertyGrid1.SelectedObject = ItemList.SelectedItems[0].Tag;
            } else {
                propertyGrid1.SelectedObject = null;
            }

        }


        public void SetMapImage(Image image) {
            picMap.BackgroundImage = image;
         }

        private void btnDeleteItem_Click(object sender, EventArgs e) {
            SelectedScreen.Data.Items.RemoveAt(ItemList.SelectedIndices[0]);
            UpdateForSelectedScreen();
            CalculateDataSize();
        }

        private void OnItemMenuClick(object sender, EventArgs e) {
            var menuItem = sender as ToolStripMenuItem;
            int index = ItemMenu.Items.IndexOf(menuItem);

            ItemTypeToAdd = (ItemTypeIndex)(index + 1);
            btnAddItem.Text = menuItem.Text;

            AddItem();
        }

        private void AddItem() {
            ItemData newItem = null;
            
            switch (ItemTypeToAdd) {
                case ItemTypeIndex.Enemy:
                    newItem = new ItemEnemyData();
                    break;
                case ItemTypeIndex.PowerUp:
                    newItem = new ItemPowerupData();
                    break;
                case ItemTypeIndex.Mella:
                    newItem = new ItemSingleByteData(ItemTypeIndex.Mella);
                    break;
                case ItemTypeIndex.Elevator:
                    newItem = new ItemElevatorData();
                    break;
                case ItemTypeIndex.Turret:
                    newItem = new ItemTurretData();
                    break;
                case ItemTypeIndex.MotherBrain:
                    newItem = new ItemSingleByteData(ItemTypeIndex.MotherBrain);
                    break;
                case ItemTypeIndex.Zebetite:
                    newItem = new ItemSingleByteData(ItemTypeIndex.Zebetite);
                    break;
                case ItemTypeIndex.Rinkas:
                    newItem = new ItemSingleByteData(ItemTypeIndex.Rinkas);
                    break;
                case ItemTypeIndex.Door:
                    newItem = new ItemDoorData();
                    break;
                case ItemTypeIndex.PalSwap:
                    newItem = new ItemSingleByteData(ItemTypeIndex.PalSwap);
                    break;
            }

            if (newItem == null) return;

            SelectedScreen.Data.Items.Add(newItem);
            UpdateForSelectedScreen();
            CalculateDataSize();

        }

        private void btnAddItem_Click(object sender, EventArgs e) {
            //AddItem();
            ItemMenu.Show(btnAddItem, 0, btnAddItem.Height);
        }

        void CalculateDataSize() {
            SortScreenData();

            ScreenWarningLabel.Visible = false;

            int currentRow = -1;
            int currentScreen = -1;
            int totalBytes = 0;

            for (int i = 0; i < Screens.Count; i++) {
                totalBytes += 3; // Each screen has 2 byte header and 1 byte footer

                Point location = Screens[i].MapLocation;

                if (location.Y != currentRow) {
                    Debug.Assert(currentRow < location.Y);

                    currentRow = location.Y;
                    totalBytes += 3; // Each row has 3 byte header
                } else if (location.X == currentScreen) {
                    ScreenWarningLabel.Visible = true;
                }

                currentScreen = location.X;

                var items = Screens[i].Data.Items;
                for (int iItem = 0; iItem < items.Count; iItem++) {
                    totalBytes += items[iItem].Size;
                }

            }
            calculatedSize = totalBytes;

            //string template = DataSizeLabel.Tag as string;
            //DataSizeLabel.Text = template.Replace("{1}", totalBytes.ToString()).Replace("{2}", originalSize.ToString()).Replace("{3}", totalAvailBytes.ToString());
            DataSizeLabel.Text = string.Format("Using ${0} of ${2} bytes" + Environment.NewLine + "(original: ${1} bytes)",
                totalBytes.ToString("X"),
                originalSize.ToString("X"),
                totalAvailBytes.ToString("X"));

            if (calculatedSize > totalAvailBytes) {
                btnApply.Enabled = false;
                DataSizeLabel.ForeColor = Color.Red;
            } else {
                if (ScreenWarningLabel.Visible)
                    btnApply.Enabled = false;
                else
                    btnApply.Enabled = true;
                DataSizeLabel.ForeColor = ForeColor;
            }
        }

        private void SortScreenData() {
            Screens.Sort(screenSorter);
        }

        int screenSorter(ItemEditorScreenControl a, ItemEditorScreenControl b) {
            int aVal = a.MapLocation.X + a.MapLocation.Y * 32;
            int bVal = b.MapLocation.X + b.MapLocation.Y * 32;

            return aVal - bVal;
        }

        private void button2_Click(object sender, EventArgs e) {
            Close();
        }

        private void btnApply_Click(object sender, EventArgs e) {
            ApplyChanges();

            Close();
        }

        private void ApplyChanges() {
            RemoveEmptyScreens();
            CalculateDataSize();

            int sizeDiff = calculatedSize - originalSize;

            Program.MainForm.GameRom.SerializeAllData();
            if (level.Format.ItemExpansionMode == Editroid.ROM.Formats.ItemExpansion.ExpandBackward) {
                ApplyItemData(-sizeDiff);
            } else {
                ApplyItemData(0);
            }
            Program.MainForm.ReloadCurrentRomFromMemory();
        }

        private void RemoveEmptyScreens() {
            for (int i = Screens.Count - 1; i >= 0; i--) {
                if (Screens[i].Data.Items.Count == 0)
                    RemoveAndDisposeScreenControl(Screens[i]); 
            }
        }

        /// <summary>
        /// Sets the item data pointer to the address of the current beginning of the item data plus the value
        /// specified as 'offset'. (This is based on the first item data in memory, not the first link in the linked list)
        /// </summary>
        /// <param name="offset">The amount to offset the address of the item data pointer.</param>
        /// <returns></returns>
        pRom OffsetItemPointer(int offset) {
            var currentOffset = originalData.GetFirstRowOffset();
            var newOffset = currentOffset + offset;

            level.PRom[level.Format.pPointerToItemList] = newOffset;
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

            byte[] data = level.Rom.data;

            // Each row has a pointer to next row. This can't be written until we actually get
            // to the next row, so we remember where the pointer is so that when we start next
            // row we can go back and set the pointer.
            int pendingPointerOffset = -1; 

            for (int i = 0; i < Screens.Count; i++) {
                Point location = Screens[i].MapLocation;
                bool lastScreenInRow = (i == Screens.Count - 1) || (Screens[i + 1].MapLocation.Y != Screens[i].MapLocation.Y);

                if (location.Y != currentRow) {
                    Debug.Assert(currentRow < location.Y);

                    currentRow = location.Y;
                    
                    // Write pointer to this new row:
                    if (pendingPointerOffset != -1) {
                        var pointer = level.CreatePointer((pRom)currentOffset);
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

                var items = Screens[i].Data.Items;

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

        ////private void MoveStructData(int sizeDiff) {
        ////    level.TrasposeStructData(sizeDiff);
        ////}

        private void btnAddScreen_Click(object sender, EventArgs e) {
            ItemEditorScreenControl newScreen = new ItemEditorScreenControl();
            newScreen.MapLocation = new Point(0, 0);
            AddScreenControl(newScreen);
            CalculateDataSize();

            SelectedScreen = newScreen;
        }

        private void btnRemoveScreen_Click(object sender, EventArgs e) {
            var screen = SelectedScreen;

            RemoveAndDisposeScreenControl(screen);
            SelectedScreen = null;

            CalculateDataSize();
        } 

    }

    class ItemEditorScreenControl : Control
    {
        public const int MapTileWidth = frmAdvancedItems.MapTileWidth;
        public const int MapTileHeight = frmAdvancedItems.MapTileHeight;
        
        public event EventHandler ScreenMoved;
        public ItemEditorScreenControl() {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.UserPaint, true);
            Data = new ItemScreenData();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Selected ? Brushes.Red : Brushes.White, e.ClipRectangle);
        }

        Point mapLocation;
        public Point MapLocation {
            get { return mapLocation; }
            set {
                Data.MapX = value.X;
                Data.MapY = value.Y;
                mapLocation = new Point(Data.MapX, Data.MapY);
                Bounds = new Rectangle(Data.MapX * MapTileWidth, Data.MapY * MapTileHeight, MapTileWidth, MapTileHeight);

                if (ScreenMoved != null)
                    ScreenMoved(this, EventArgs.Empty);
            }
        }

        bool selected;
        public bool Selected { get { return selected; }
            set {
                if (selected != value) {
                    selected = value;
                    Invalidate();
                }
            }
        }
        public ItemScreenData Data { get; set; }

        bool dragging;
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
                dragging = true;
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            dragging = false;
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            Point mousePos = e.Location;
            mousePos.Offset(Location);
            Point cell = new Point(mousePos.X / MapTileWidth, mousePos.Y / MapTileHeight);

            if (cell != MapLocation && cell.X >= 0 && cell.X < 32 && cell.Y >= 0 && cell.Y < 32) {
                MapLocation = cell; 
            }
        }
    }
    
    class RowData
    {
        public RowData() {
            Screens = new List<ItemScreenData>();
        }
        public IList<ItemScreenData> Screens { get; private set; }
        int mapY;
        public int MapY { get { return mapY; } set { mapY = value & 0x1f; } }
    }
    ////public class ScreenData
    ////{
    ////    public ScreenData() {
    ////        Items = new List<ItemData>();
    ////    }
    ////    public IList<ItemData> Items { get; private set; }
    ////    int mapX;
    ////    public int MapX { get { return mapX; } set { mapX = value & 0x1f; } }
    ////    int mapY;
    ////    public int MapY { get { return mapY; } set { mapY = value & 0x1f; } }

    ////}
    
    ////abstract class ItemData
    ////{
    ////    [Browsable(false)]
    ////    public abstract int Size { get; }

    ////    ItemTypeIndex type;
    ////    public ItemTypeIndex ItemType {
    ////        get { return type; }
    ////    }

    ////    public ItemData() { }
    ////    public ItemData(ItemTypeIndex type) { this.type = type; }

    ////    int spriteSlot;
    ////    public int SpriteSlot { get { return spriteSlot; } set { spriteSlot = value & 0xF; } }


    ////    public virtual void LoadData(ItemSeeker data) {
    ////        this.type = data.ItemType;
    ////        this.SpriteSlot = data.SpriteSlot;
    ////    }

    ////    public virtual void WriteData(byte[] data, ref int offset) {
    ////        data[offset] = (byte)((int)ItemType | spriteSlot << 4);
    ////        offset++;
    ////    }

    ////    protected static void WritePoint(byte[] data, ref int offset, Point point) {
    ////        ScreenCoordinate coord = new ScreenCoordinate(point.X, point.Y);
    ////        data[offset] = coord.Value;
    ////        offset++;
    ////    }

    ////}
    ////class SingleByteData : ItemData
    ////{
        
    ////    public SingleByteData() { }
    ////    public SingleByteData(ItemTypeIndex type) : base(type) { }


    ////    public override int Size {
    ////        get { return 1; }
    ////    }

    ////}
    ////class TurretData : ItemData
    ////{
    ////    public TurretData() : base(ItemTypeIndex.Turret) { }
    ////    public override int Size {
    ////        get { return 2; }
    ////    }

    ////    Point screenPosition;
    ////    public Point ScreenPosition { get { return screenPosition; } set { screenPosition = new Point(value.X & 0xf, value.Y & 0xf); } }

    ////    public override void LoadData(ItemSeeker data) {
    ////        base.LoadData(data);
    ////        screenPosition = new Point(data.ScreenPosition.X, data.ScreenPosition.Y);
    ////    }

    ////    public override void WriteData(byte[] data, ref int offset) {
    ////        base.WriteData(data, ref offset);

    ////        WritePoint(data, ref offset, screenPosition);
    ////    }

    ////}
    ////class ElevatorData : ItemData
    ////{
    ////    public ElevatorData() : base(ItemTypeIndex.Elevator) { }
    ////    public override int Size {
    ////        get { return 2; }
    ////    }

    ////    public ElevatorDestination ElevatorType { get; set; }

    ////    public override void LoadData(ItemSeeker data) {
    ////        base.LoadData(data);

    ////        ElevatorType = data.Destination;
    ////    }

    ////    public override void WriteData(byte[] data, ref int offset) {
    ////        base.WriteData(data, ref offset);

    ////        data[offset] = (byte)ElevatorType;
    ////        offset++;
    ////    }
    ////}
    ////class DoorData : ItemData
    ////{
    ////    public DoorData() : base(ItemTypeIndex.Door) { }
    ////    public override int Size {
    ////        get { return 2; }
    ////    }

    ////    public DoorSide Side { get; set; }
    ////    public DoorType Type { get; set; }

    ////    public override void LoadData(ItemSeeker data) {
    ////        base.LoadData(data);

    ////        Side = data.DoorSide;
    ////        Type = data.DoorType;
    ////    }

    ////    public override void WriteData(byte[] data, ref int offset) {
    ////        base.WriteData(data, ref offset);

    ////        data[offset] = (byte)((int)Side | (int)Type);
    ////        offset++;
    ////    }
    ////}
    ////abstract class Item3Data : ItemData
    ////{
    ////    public Item3Data (ItemTypeIndex type):base(type){}
    ////    public override int Size { get { return 3; } }

    ////    Point screenPosition;
    ////    public Point ScreenPosition { get { return screenPosition; } set { screenPosition = new Point(value.X & 0xf, value.Y & 0xf); } }

    ////    public override void LoadData(ItemSeeker data) {
    ////        base.LoadData(data);

    ////        screenPosition = new Point(data.ScreenPosition.X, data.ScreenPosition.Y);
    ////    }
    ////}

    ////class EnemyData : Item3Data
    ////{
    ////    public EnemyData():base(ItemTypeIndex.Enemy){}
    ////    public bool Difficult { get; set; }
    ////    int enemyType;
    ////    public int EnemyType { get { return enemyType; } set { enemyType = value & 0xf; } }

    ////    public override void LoadData(ItemSeeker data) {
    ////        base.LoadData(data);

    ////        Difficult = data.EnemyIsHard;
    ////        enemyType = data.EnemyTypeIndex;
    ////    }

    ////    public override void WriteData(byte[] data, ref int offset) {
    ////        base.WriteData(data, ref offset);

    ////        data[offset] = (byte)(enemyType | (Difficult ? 0x80 : 0));
    ////        offset++;

    ////        WritePoint(data, ref offset, ScreenPosition);
    ////    }
    ////}
    ////class PowerupData : Item3Data
    ////{
    ////    public PowerupData() : base(ItemTypeIndex.PowerUp) { }

    ////    public PowerUpType PowerUp { get; set; }

    ////    public override void LoadData(ItemSeeker data) {
    ////        base.LoadData(data);

    ////        PowerUp = data.PowerUp;
    ////    }

    ////    public override void WriteData(byte[] data, ref int offset) {
    ////        base.WriteData(data, ref offset);

    ////        data[offset] = (byte)PowerUp;
    ////        offset++;

    ////        WritePoint(data, ref offset, ScreenPosition);
    ////    }
    ////}
}
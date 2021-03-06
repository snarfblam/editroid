using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;
using Editroid.Actions;
using Editroid.UndoRedo;

namespace Editroid
{
    public partial class ItemDataEditor : UserControl
    {
        public ItemDataEditor() {
            InitializeComponent();

            //LoadList(elevatorList);
            GlobalEventManager.Manager.ObjectSelected += new EventHandler<ScreenObjectEventArgs>(OnSelectionChanged);
        }

        void OnSelectionChanged(object sender, ScreenObjectEventArgs e) {
            //LoadItems(Program.MainForm.CurrentLevel);
        }


        lists currentList = (lists)(-1);
        void LoadList(lists list) {
            if (currentList == list) return;
            currentList = list;

            string[] items = new string[0];
            switch (list) {
                case lists.elevator:
                    items = elevatorList;
                    break;
                case lists.powerup:
                    items = powerupList;
                    break;
                case lists.singlebyte:
                    items = singleByteList;
                    break;
                case lists.doors:
                    items = doorList;
                    break;
                case lists.turret:
                    items = turretList;
                    break;
            }

            itemValueList.Items.Clear();

            for (int i = 0; i < items.Length; i++)
			{
                string[] parts = items[i].Split(':');
                itemValueList.Items.Add(parts[2], parts[1]).Tag = parts[0];
            }
        }

        private MetroidRom rom;

        ItemScreenData currentRoom;
        ItemScreenControl SelectedScreen = null;
        ItemEditTool editTool;


        public MetroidRom Rom {
            get { return rom; }
            set {
                rom = value;
            }
        }
       

        #region Item lists
        static string[] elevatorList = new string[]{
            "0:downPal:Brinstar ► Brinstar",
            "1:downPal:Brinstar ► Norfair",
            "2:down:Brinstar ► Kraid",
            "3:down:Brinstar ► Tourian",
            "4:down:Norfair ► Ridley",
            "81:upPal:Norfair ► Brinstar",
            "82:up:Kraid ► Brinstar",
            "83:up:Tourian ► Brinstar",
            "84:up:Ridley ► Narfair",
            "8f:up:Win Game!"
        };
        static string[] powerupList = new string[]{
            "0:bomb:Bombs",
            "1:highJump:High Jump",
            "2:longBeam:Long Beam",
            "3:screwAttack:Screw Attack",
            "4:maruMari:Maru Mari",
            "5:varia:Varia",
            "6:waveBeam:Wave Beam",
            "7:iceBeam:Ice Beam",
            "8:energyTank:Energy Tank",
            "9:missiles:Missiles",
            "A:invalid:Invalid",
            "B:invalid:Invalid",
            "C:invalid:Invalid",
            "D:invalid:Invalid",
            "E:invalid:Invalid",
            "F:invalid:Invalid"
        };
        static string[] singleByteList = new string[]{
            "3::Mella",
            "7::Zebetite 1  (Right)",
            "17::Zebetite 2  (Left)",
            "27::Zebetite 3  (Right)",
            "37::Zebetite 4  (Left)",
            "47::Zebetite 5  (Right)",
            "6::Mother Brain",
            "8::Rinka",
            "18::Rinka B",
            "A::Palette Swap"
        };
        static string[] doorList = new string[]{
            "B0::Left Missile",
            "B1::Left",
            "B2::Left 10 Missile",
            "B3::Left Music Change",
            "A0::Right Missile",
            "A1::Right",
            "A2::Right 10 Missile",
            "A3::Right Music Change",
        };
        ////static string[] turretList = new string[]{
        ////    "05::Up Left Down",
        ////    "15::Up Right Down",
        ////    "25::Left Down Right",
        ////    "35::Left Down",
        ////};
        static string[] turretList = new string[]{
            "0::Up Left Down",
            "1::Up Right Down",
            "2::Left Down Right",
            "3::Left Down",
        };

        enum lists
        {
            elevator,
            powerup,
            singlebyte,
            doors,turret
        }
        #endregion

        private void LoadItemsForSelectedScreen() {

            lstItems.Items.Clear();

            if (currentRoom != null) {

                for (int i = 0; i < currentRoom.Items.Count; i++) {
                    var item = currentRoom.Items[i];
                    lstItems.Items.Add(GetItemDescription(item.ItemType));
                }

                lstItems.SelectedIndices.Clear();
            }
        }



        Screen loadedScreen;
        Point mapLocation;
        ItemInstance selectedItem;

        /// <summary>
        /// Clears any cached data. Call when item data is edited by another part of the program.
        /// </summary>
        internal void UnloadScreen() {
            this.mapLocation = new Point(-1, -1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="mapLocation"></param>
        /// <param name="currentSelection">Specifies currently selected item, so selection can be restored
        /// after reloading screen.</param>
        internal void LoadScreen(LevelIndex level, Point mapLocation, ItemScreenData currentRoom,  ItemInstance currentSelection) {
            BeginUpdate();

            this.selectedItem = currentSelection;
            this.currentRoom = currentRoom;

            // Check to see if screen is already loaded
            if (this.mapLocation != mapLocation) {
                this.mapLocation = mapLocation;

                lstItems.BeginUpdate();
                lstItems.Items.Clear();
                ItemSeeker items;
                ////if (rom.GetLevel(level).CheckForItemsAt(mapLocation, out items)) {
                LoadItemsForSelectedScreen();
                ////}
                lstItems.EndUpdate();
            }

            itemValueList.BeginUpdate();
            ShowSelectedItem();
            itemValueList.EndUpdate();

            EndUpdate();
        }

        private void ShowSelectedItem() {
            lstItems.SelectedIndices.Clear();

            int selectionIndex = -1;
            if(currentRoom != null) selectionIndex= currentRoom.Items.IndexOf(selectedItem.Data);
            if (selectionIndex >= 0) {
                lstItems.SelectedIndices.Add(selectionIndex);
            }
            ShowOptions();
        }

        private void ShowOptions() {
            DisplayEditor();
            ShowValue();
        }

        private void DisplayEditor() {
            switch (selectedItem.Data.ItemType) {
                case ItemTypeIndex.Enemy:
                    if (!itemValueList.Visible) itemValueList.Show();
                    itemValueList.Visible = false;
                    EnemyEditor.Visible = true;
                    break;
                case ItemTypeIndex.PowerUp:
                case ItemTypeIndex.Rinkas:
                case ItemTypeIndex.Mella:
                case ItemTypeIndex.PalSwap:
                case ItemTypeIndex.Elevator:
                case ItemTypeIndex.MotherBrain:
                case ItemTypeIndex.Door:
                case ItemTypeIndex.Turret:
                case ItemTypeIndex.Zebetite:
                    if (!itemValueList.Visible) itemValueList.Show();
                    itemValueList.Enabled = true;
                    EnemyEditor.Visible = false;
                    break;
                default:
                    itemValueList.Visible = false;
                    EnemyEditor.Visible = false;
                    break;
            }
        }

        private void ShowValue() {
            var iEnemy = selectedItem.Data as ItemEnemyData;
            var iPowerUp = selectedItem.Data as ItemPowerupData;
            var iElevator = selectedItem.Data as ItemElevatorData;
            var iDoor = selectedItem.Data as ItemDoorData;

            switch (selectedItem.Data.ItemType) {
                case ItemTypeIndex.Nothing:
                    break;
                case ItemTypeIndex.Enemy:
                    if ((int)EnemySlotNud.Value != (int)selectedItem.Data.SpriteSlot)
                        EnemySlotNud.Value = selectedItem.Data.SpriteSlot;
                    if ((int)EnemyTypeNud.Value != iEnemy.EnemyType)
                        EnemyTypeNud.Value = iEnemy.EnemyType;
                    if (DifficultCheck.Checked != iEnemy.Difficult)
                        DifficultCheck.Checked = iEnemy.Difficult;
                    break;
                case ItemTypeIndex.PowerUp:
                    LoadList(lists.powerup);
                    SelectValueItem((int)iPowerUp.PowerUp);

                    break;
                case ItemTypeIndex.Elevator:
                    LoadList(lists.elevator);
                    SelectValueItem((int)iElevator.ElevatorType);
                    break;
                case ItemTypeIndex.Turret:
                    int turrettype = selectedItem.Data.SpriteSlot;
                    LoadList(lists.turret);
                    SelectValueItem(turrettype);
                    break;
                case ItemTypeIndex.Door:
                    int doorType = (int)iDoor.Side | (int)iDoor.Type; // selectedItem.Data.Data[selectedItem.Data.itemOffset + 1];
                    LoadList(lists.doors);
                    SelectValueItem(doorType);
                    break;
                case ItemTypeIndex.Mella:
                case ItemTypeIndex.MotherBrain:
                case ItemTypeIndex.Zebetite:
                case ItemTypeIndex.PalSwap:
                case ItemTypeIndex.Rinkas:
                    int type = (int)selectedItem.Data.ItemType | (selectedItem.Data.SpriteSlot << 4); //selectedItem.Data.Data[selectedItem.Data.itemOffset];
                    LoadList(lists.singlebyte);
                    SelectValueItem(type);
                    break;
                case ItemTypeIndex.Unused_b:
                    break;
                case ItemTypeIndex.Unused_c:
                    break;
                case ItemTypeIndex.Unused_d:
                    break;
                case ItemTypeIndex.Unused_e:
                    break;
                case ItemTypeIndex.Unused_f:
                    break;
                default:
                    break;
            }
        }

        static string GetItemDescription(ItemTypeIndex i) {
            switch (i) {
                case ItemTypeIndex.Nothing:
                    return "None";
                case ItemTypeIndex.Enemy:
                    return "Enemy";
                case ItemTypeIndex.PowerUp:
                    return "Power Up";
                case ItemTypeIndex.Elevator:
                    return "Elevator";
                case ItemTypeIndex.MotherBrain:
                case ItemTypeIndex.Mella:
                case ItemTypeIndex.Rinkas:
                case ItemTypeIndex.PalSwap:
                case ItemTypeIndex.Zebetite:
                    return "Single Byte";
                case ItemTypeIndex.Door:
                    return "Door";
                case ItemTypeIndex.Turret:
                    return "Turret";
                case ItemTypeIndex.Unused_b:
                case ItemTypeIndex.Unused_c:
                case ItemTypeIndex.Unused_d:
                case ItemTypeIndex.Unused_e:
                case ItemTypeIndex.Unused_f:
                    return "Unknown";
                default:
                    return "Invalid";
            }
        }

        private void SelectValueItem(int value) {
            int indexToSelect = GetListItemIndexForValue(value);

            if(itemValueList.SelectedIndices.Count > 0 && itemValueList.SelectedIndices[0] == indexToSelect)
                return;

            //if(value < itemValueList.Items.Count)
                itemValueList.Items[indexToSelect].Selected = true;
        }

        private int GetListItemIndexForValue(int index) {
            for (int i = 0; i < itemValueList.Items.Count; i++) {
                ListViewItem item = itemValueList.Items[i];
                if (int.Parse(item.Tag.ToString(), System.Globalization.NumberStyles.HexNumber) == index)
                    return i;
            }

            return 0;
        }

        ////private void LoadItemsForSelectedScreen(ItemSeeker items) {
        ////    bool hasItems = true;
        ////    while (hasItems) {
        ////        lstItems.Items.Add(GetItemDescription(items));

        ////        hasItems = items.MoreItemsPresent;
        ////        items.NextItem();
        ////    }
        ////}


        int updateLevel = 0;
        void BeginUpdate() {
            updateLevel++;
        }
        void EndUpdate() {
            updateLevel--;
        }
        bool IsUpdating {
            get {
                return updateLevel > 0;
            }
        }
        private void itemValueList_SelectedIndexChanged(object sender, EventArgs e) {
            if (IsUpdating) return;    
            if (itemValueList.SelectedIndices.Count == 0) return;

            int selectedValue = int.Parse(itemValueList.SelectedItems[0].Tag.ToString(), System.Globalization.NumberStyles.HexNumber);
            ItemProperty itemprop = ItemProperty.invalid;

            switch (selectedItem.Data.ItemType) {
                case ItemTypeIndex.PowerUp:
                    itemprop = ItemProperty.power_up_type;
                    ////Program.PerformAction(Program.Actions.EditItemProperty(
                    ////    mapLocation, 
                    ////    selectedItem.Data.ID, 
                    ////    ItemProperty.power_up_type, 
                    ////    selectedValue));
                    break;
                case ItemTypeIndex.Elevator:
                    itemprop = ItemProperty.elevator_destination;
                    ////Program.PerformAction(Program.Actions.EditItemProperty(
                    ////    mapLocation, 
                    ////    selectedItem.Data.ID, 
                    ////    ItemProperty.elevator_destination, 
                    ////    selectedValue));
                    break;
                case ItemTypeIndex.Mella:
                case ItemTypeIndex.PalSwap:
                case ItemTypeIndex.Rinkas:
                case ItemTypeIndex.Zebetite:
                case ItemTypeIndex.MotherBrain:
                    itemprop = ItemProperty.single_byte_item_type;
                    ////Program.PerformAction(Program.Actions.EditItemProperty(
                    ////    mapLocation, 
                    ////    selectedItem.Data.ID, 
                    ////    ItemProperty.single_byte_item_type,
                    ////    selectedValue));
                    break;
                case ItemTypeIndex.Door:
                    itemprop = ItemProperty.door_value;
                    ////Program.PerformAction(Program.Actions.EditItemProperty(
                    ////    mapLocation,
                    ////    selectedItem.Data.ID,
                    ////    ItemProperty.doorValue,
                    ////    selectedValue));
                    break;
                case ItemTypeIndex.Turret:
                    itemprop = ItemProperty.turret_type;
                    ////Program.PerformAction(Program.Actions.EditItemProperty(
                    ////    mapLocation,
                    ////    selectedItem.Data.ID,
                    ////    ItemProperty.turretType,
                    ////    selectedValue));
                    break;
                default:
                    return;
            }

            Program.PerformAction(Program.Actions.EditItemProperty(
                mapLocation,
                currentRoom,
                selectedItem.Data,
                itemprop,
                selectedValue));

 }

        private void itemValueList_Click(object sender, EventArgs e) {

        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e) {
            if (!IsUpdating && lstItems.SelectedIndices.Count > 0) {
                Program.MainForm.SelectItemInScreen(lstItems.SelectedIndices[0]);
            }
        }

        private void EnemyTypeNud_ValueChanged(object sender, EventArgs e) {
            if (IsUpdating) return;

            Program.PerformAction(
                Program.Actions.EditItemProperty(
                    this.mapLocation,
                    currentRoom,
                    selectedItem.Data,
                    ItemProperty.enemy_type,
                    (int)EnemyTypeNud.Value));

        }

        private void EnemySlotNud_ValueChanged(object sender, EventArgs e) {
            if (IsUpdating) return;

            Program.PerformAction(
                Program.Actions.EditItemProperty(
                    this.mapLocation,
                    currentRoom,
                    selectedItem.Data,
                    ItemProperty.enemy_slot,
                    (int)EnemySlotNud.Value));
        }

        private void DifficultCheck_CheckedChanged(object sender, EventArgs e) {
            if (IsUpdating) return;

            Program.PerformAction(
                Program.Actions.EditItemProperty(
                    this.mapLocation,
                    currentRoom,
                    selectedItem.Data,
                    ItemProperty.enemy_difficulty,
                    DifficultCheck.Checked ? -1 : 0));
        }

        private void EnemyEditor_Paint(object sender, PaintEventArgs e) {
            e.Graphics.DrawRectangle(SystemPens.ControlLight, new Rectangle(0, 0, EnemyEditor.Width - 1, EnemyEditor.Height - 1));
        }

    }

    /// <summary>
    /// Contians a group of controls that represents all items in a single level.
    /// </summary>
    public class MapItemDisplay : WindowlessControl
    {
        LevelIndex currentLevel;
        public LevelIndex CurrentLevel { get { return currentLevel; } }


        public MapItemDisplay() {
            Bounds = new Rectangle(0, 0, 512, 512);
            blinkTimer.Tick += new EventHandler(blinkTimer_Tick);
        }


        public void LoadLevel(LevelIndex level) {
            this.currentLevel = level;

            ClearItemDisplay();

            if (rom == null || level == LevelIndex.None)
                LevelItems = null;
            else {
                LevelItems = rom.GetLevel(level).Items;
                CreateItemDisplay();
            }
        }

        bool _isLoaded;
        /// <summary>
        /// Gets a value indicating whether this MapItemDisplay has been populated with data.
        /// </summary>
        public bool IsLoaded {
            get { return _isLoaded; }
            set {
                if (_isLoaded != value) {
                    _isLoaded = value;

                    if (_isLoaded) {
                        StartBlinkTimer();
                    } else {
                        StopBlinkTimer();
                    }
                }
            }
        }

        #region Blinking
        const int offTime = 250;
        const int onTime = 2000;

        Timer blinkTimer = new Timer();

        private void StopBlinkTimer() {
            blinkTimer.Stop();
            if (!Visible) Visible = true;
        }

        private void StartBlinkTimer() {
            blinkTimer.Interval = onTime;
            blinkTimer.Start();
        }

        void blinkTimer_Tick(object sender, EventArgs e) {
            Visible = !Visible;

            if (Visible) {
                blinkTimer.Interval = onTime;
            } else {
                blinkTimer.Interval = offTime;
            }
        }

        #endregion

        private void CreateItemDisplay() {
            ////foreach (ItemRowEntry e in currentLevelItems) {
            ////    ItemRowControl entryEditor = new ItemRowControl(e);
            ////    entryEditor.MapDisplay = this;

            ////    Controls.Add(entryEditor);
            ////}
            foreach (ItemScreenData screen in LevelItems) {
                ItemScreenControl newScreen = new ItemScreenControl(screen);
                Controls.Add(newScreen);
            }

            IsLoaded = true;
        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { rom = value; }
        }

        ItemLoader currentLevelItems_DEPRECATED;
        public ItemCollection LevelItems { get; private set; }
        private MapControl mapControl;

        public MapControl MapControl {
            get { return mapControl; }
            set {
                mapControl = value;
                Host = value;
            }
        }

        ItemScreenControl SelectedScreen = null;

        public void ClearItemDisplay() {
            ////SelectedScreen = null;

            ////for (int i = Controls.Count - 1; i >= 0; i--) {
            ////    ItemRowControl c = Controls[i] as ItemRowControl;
            ////    if (c != null) {
            ////        Controls.RemoveAt(i);
            ////        c.Dispose();
            ////        c.MapDisplay = null;
            ////    }
            ////}

            ////IsLoaded = false;
            SelectedScreen = null;

            for (int i = Controls.Count - 1; i >= 0; i--) {
                ItemScreenControl c = Controls[i] as ItemScreenControl;
                if (c != null) {
                    Controls.RemoveAt(i);
                    c.Dispose();
                    c.MapDisplay = null;
                }
            }

            IsLoaded = false;
        }

        /////// <summary>
        /////// Gets the control that owns the row entry with the specified index, or null if not found.
        /////// </summary>
        /////// <returns></returns>
        ////internal ItemRowControl_DEPRECATED GetEntryOwner_DEPRECATED(int index) {
        ////    foreach (WindowlessControl c in Controls) {
        ////        ItemRowControl_DEPRECATED row = c as ItemRowControl_DEPRECATED;
        ////        if (row != null && row.Entry.OrderIndex == index)
        ////            return row;
        ////    }

        ////    return null;
        ////}
        internal void NotifyActionOccurred(Editroid.UndoRedo.EditroidAction a) {
           if (a is EditItemProperty) {

                foreach (WindowlessControl c in Controls) {
                    ItemScreenControl screen = c as ItemScreenControl;
                    if (screen != null && screen.ItemData.Items.Contains(((EditItemProperty)a).Item))
                        screen.ReloadImage();
                }
               
            }

            ////if (a is ItemAction) {
            ////    ItemIndex_DEPRECATED ID = new ItemIndex_DEPRECATED();
            ////    ItemRowControl_DEPRECATED entryOwner = null;

            ////    ID = ((ItemAction)a).ItemIndex;
            ////    entryOwner = GetEntryOwner_DEPRECATED(ID.Row);
            ////    if (entryOwner == null) return;

            ////    if (a is SetItemTilePosition) {
            ////        entryOwner.NotifyScreenEntryMoved(ID);
            ////    } else if (a is SetItemRowPosition) {
            ////        entryOwner.NotifyRowMoved(ID);
            ////    }
            ////} else if (a is EditItemProperty) {
            ////    ID = ((EditItemProperty)a).ItemID;
            ////    entryOwner = GetEntryOwner_DEPRECATED(ID.Row);
            ////    if (entryOwner == null) return;

            ////    entryOwner.NotifyItemChanged(ID);
            ////}
        }
    }
}

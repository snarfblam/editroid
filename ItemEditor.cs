using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;
using Editroid.Graphic;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Editroid
{
    internal partial class ItemEditor:Form
    {
        public ItemEditor() {
            InitializeComponent();

            pnlScreen.BackgroundImage = ScreenImage;
        }

        private Rom rom;
        ItemTable currentLevelItems;

        public Rom Rom {
            get { return rom; }
            set { 
                rom = value;
                LoadItems();
            }
        }
        PasswordDatum passwordDataEntry = new PasswordDatum();

        List<Panel> ItemPanels = new List<Panel>();

        Bitmap ScreenImage = new Bitmap(256, 240, PixelFormat.Format8bppIndexed);
        Blitter ScreenBlitter = new Blitter();

        private LevelIndex level = LevelIndex.Brinstar;

        public LevelIndex Level {
            get { return level; }
            set { 
                level = value;
                LoadItems();
            }
        }

        private MapControl mapControl;

        public MapControl MapSource {
            get { return mapControl; }
            set { mapControl = value; }
        }


        private void LoadItems() {
            if(rom == null || level == LevelIndex.None)
                currentLevelItems = null;
            else
                currentLevelItems = rom.GetLevel(level).ItemTable;

            ClearItemDisplay();
            CreateItemDisplay();
        }

        private void CreateItemDisplay() {
            foreach(ItemRowEntry e in currentLevelItems) {
                ItemEntryEditor entryEditor = new ItemEntryEditor(e);

                pnlMap.Controls.Add(entryEditor);
            }

        }

        

        
        ItemEntryScreenTile SelectedScreen = null;
        ItemEditTool editTool;
        #region Mouse Dragging
        void ItemEntry_MouseMove(object sender, MouseEventArgs e) {
        }

        void ItemEntry_MouseUp(object sender, MouseEventArgs e) {
        }

        void ItemEntry_MouseDown(object sender, MouseEventArgs e) {

        }


        int DragLabelX = 0;
        int DragLabelY = 0;
        bool DragLabel = false;
        void itemLabel_MouseUp(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left)
                DragLabel = false;
        }

        void itemLabel_MouseMove(object sender, MouseEventArgs e) {
            if(DragLabel) {
                int dragX = e.X / 8;
                if(e.X < 0) dragX--; // Account for integer division rounding UP instead of DOWN with negative numbers

                int dragY = e.Y / 8;
                if(e.Y < 0) dragY--; // Account for integer division rounding UP instead of DOWN with negative numbers

                Label itemControl = (Label)sender;
                Panel entryControl = (Panel)itemControl.Parent;

                CheckDragBounds(ref dragX, ref dragY, itemControl, entryControl);
                
                if(dragX != DragLabelX) {
                    int destX = ((Label)sender).Parent.Left / 8 +  ((Label)sender).Left / 8 + dragX;
                    if(destX < 1) destX = 1;
                    if(destX > 30) destX = 30;

                    DragLabel = false; // Suspend handling of mousemove
                    SetItemLocationX((Label)sender, destX);
                    DragLabel = true;

                    ShowScreenAt(GetMapPosition());
                }

                if(dragY != DragLabelY) {
                    int destY = (((Label)sender).Parent.Top) / 8 + dragY ;
                    if(destY < 1) destY = 1;
                    if(destY > 30) destY = 30;

                    DragLabel = false;
                    SetRowLocation(((Label)sender).Parent, destY);
                    DragLabel = true;

                    ShowScreenAt(GetMapPosition());
                }

            }
        }

        void itemLabel_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                if(SelectedScreen != null) SelectedScreen.BackColor = Color.Wheat;

                SelectedScreen = sender as ItemEntryScreenTile;
                SelectedScreen.BackColor = Color.Red;
                LoadScreen();

                SeekToSelection();
                GetPasswordEntry();
                editTool = ItemEditTool.GetEditTool(currentRoom);

                ShowScreenAt(GetMapPosition());

                DragLabel = true;
                DragLabelX = e.X / 8;
                DragLabelY = e.Y / 8;

                DisplaySelectedItem();

            }
        }

        private void DisplaySelectedItem() {
            ShowItemList();

            if(currentRoom.ItemType == ItemType.PowerUp || currentRoom.ItemType == ItemType.Enemy) {
                picScreenLocation.Left = currentRoom.PowerupScreenPosition.X * 16;
                picScreenLocation.Top = currentRoom.PowerupScreenPosition.Y * 16;
                picScreenLocation.Visible = true;
            } else {
                picScreenLocation.Visible = false;
            }
        }

        private void LoadScreen() {
            lstItems.Items.Clear();

            currentEntry = SelectedScreen.EntryEditor.Entry;
            currentRoom = SelectedScreen.Item;

            int item = 1;
            bool moreItems = true;
            while(moreItems) {
                lstItems.Items.Add("Item " + item.ToString());

                if(currentRoom.MoreItemsPresent) {
                    item++;
                    currentRoom.NextItem();
                } else {
                    moreItems = false;
                }
            }

            //Not necessary, currentRoom is a struct and source copy is not modified
            //currentRoom.ResetItemPointer();
            lstItems.SelectedIndex = -1;
        }


        #endregion

        private void CheckDragBounds(ref int dragX, ref int dragY, Label itemControl, Panel entryControl) {
            // Ensure that currentLevelItems aren't dragged past eachother in an entry
            foreach(Control c in entryControl.Controls) { // Moving right
                if(((int)c.Tag > (int)itemControl.Tag) // If c is a further right
                    && c.Left < itemControl.Left + dragX * 8 + 10) { // And this control is being dragged past
                    dragX = (c.Left - itemControl.Left - 4) / 8; // Move it back.
                }

                if(((int)c.Tag < (int)itemControl.Tag) // If c is a further left
                    && c.Left > itemControl.Left + dragX * 8 - 10) { // And this control is being dragged past
                    dragX = (c.Left - itemControl.Left - 4) / 8 + 1; // Move it back.
                }
            }

            int adjust = (dragY < 0) ? 0 : -1;

            bool rowCollision = true;
            while(rowCollision) {
                rowCollision = false;
                foreach(Control c in entryControl.Parent.Controls) {
                    if((c != entryControl) && 
                        ((c.Top ) / 8 == (dragY + (entryControl.Top +  adjust) / 8))) {
                        this.FindForm().Text = dragY.ToString();
                        dragY++;
                        if(dragY > 30) dragY = 1;
                        rowCollision = true;
                    }
                }
            }

        }

        private void SetRowLocation(Control panel, int destY) {
            ((ItemRowEntry)(panel.Tag)).MapY = destY;
            panel.Top = destY * 8;

            // Update password
            if(chkEasy.Checked && passwordDataEntry.offset != 0)
                passwordDataEntry.MapY = destY;
        }

        private void SetItemLocationX(Label label, int destX) {
            currentRoom.MapX = destX;
            label.Left = destX * 8 - label.Parent.Left;
            RetrimPanel(label.Parent as Panel);

            // Update password
            if(chkEasy.Checked && passwordDataEntry.offset != 0)
                passwordDataEntry.MapX = destX;
        }


        private void RetrimPanel(Panel p) {
            int leftmost = 255;
            int rightmost = 0;
            foreach(Control c in p.Controls) {
                if(c.Left < leftmost) leftmost = c.Left;
                if(c.Right > rightmost) rightmost = c.Right;
            }
            foreach(Control c in p.Controls) {
                c.Left -= leftmost;
            }

            p.SetBounds(p.Left + leftmost, p.Top, rightmost - leftmost, p.Height);
        }

        private void TrimPanel(Panel p) {
            int leftMost = -1;
            int rightMost = -1;

            // Expand panel to include all labels
            foreach(Control c in p.Controls) {
                if(leftMost == -1)
                    leftMost = c.Left;
                else
                    leftMost = Math.Min(leftMost, c.Left); // Expand left

                if(rightMost == -1)
                    rightMost = c.Right;
                else
                    rightMost = Math.Max(rightMost, c.Right); // Expand right
            }

            if(leftMost != -1 && rightMost != -1) { // Make sure labels were found to contain
                // Expand to include borders of contained controls
                leftMost--; rightMost++;

                foreach(Control c in p.Controls) {
                    c.Left -= leftMost + 1; // Reposition the labels to fit in the new dimensions
                }

                p.Left = leftMost; // Resize
                p.Width = rightMost - leftMost - 2;
            }
        }



        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            // Necessary for currentLevelItems to work when re-ordered
            rom.Brinstar.ItemTable.ResortEntries();
            rom.Ridley.ItemTable.ResortEntries();
            rom.Kraid.ItemTable.ResortEntries();
            rom.Norfair.ItemTable.ResortEntries();
            rom.Tourain.ItemTable.ResortEntries();
        }

        /// <summary>
        /// Finds the password data entry that corresponds to the selected gameItem, if there is one.
        /// </summary>
        private void GetPasswordEntry() {
            if(currentRoom.ItemType != ItemType.PowerUp) return; // Only applies to power-ups

            for(int i = 0; i < PasswordData.DataCount; i++) { // Loop through password entries
                PasswordDatum d = rom.PasswordData.GetDatum(i);
                if( d.MapX == GetMapPosition().X && // If coordinates are the same
                    d.MapY == GetMapPosition().Y &&
                    d.Item == (int)currentRoom.PowerUp) { // And it is the same item

                    passwordDataEntry = d; // we have our entry.
                    return;
                }
            }

            passwordDataEntry = new PasswordDatum(); // No entry has been found
        }

        private void ShowScreenAt(Point point) {
            screenRenderer.Level = rom.GetLevel(mapControl.GetLevel(point.X, point.Y));
            screenRenderer.Clear();
            screenRenderer.DrawScreen(rom.GetScreenIndex(point.X, point.Y));
            screenRenderer.Render(ScreenBlitter, ScreenImage);

            screenRenderer.ApplyPalette(ScreenImage, false);

            pnlScreen.Invalidate();
        }

        bool draggingScreen;
        int screenX, screenY;
        private void picScreenLocation_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                draggingScreen = true;
                screenX = e.X / 16;
                screenY = e.Y / 16;
            }
        }

        private void picScreenLocation_MouseMove(object sender, MouseEventArgs e) {
            if(draggingScreen) {
                int x = picScreenLocation.Left + e.X;
                if(x < 0) x = 0;
                if(x > 255) x = 255;

                int y = picScreenLocation.Top + e.Y;
                if(y < 0) y = 0;
                if(y > 239) y = 239;

                picScreenLocation.Location = new Point(x / 16 * 16, y / 16 * 16);
                ScreenCoordinate c = new ScreenCoordinate(x / 16, y / 16);
                currentRoom.PowerupScreenPosition = c;
            }
        }

        private void picScreenLocation_MouseUp(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left)
                draggingScreen = false;
        }


        ItemRowEntry currentEntry;
        ItemSeeker currentRoom;

        private void SeekToSelection() {
        }

        private Point GetMapPosition() {
            return new Point(SelectedScreen.MapX, SelectedScreen.EntryEditor.MapY);
        }


        ScreenRenderer screenRenderer = new ScreenRenderer();

                                    

        private void ClearItemDisplay() {
            SelectedScreen = null;

            for(int i = pnlMap.Controls.Count - 1; i >= 0; i--) {
                Control c = pnlMap.Controls[i];
                pnlMap.Controls.RemoveAt(i);
                c.Dispose();
            }

        }

        public Image MapImage {
            get { return pnlMap.BackgroundImage; }
            set { pnlMap.BackgroundImage = value; }
        }

        private void LevelButton_Click(object sender, EventArgs e) {
            btnBrinstar.Checked = 
                btnKraid.Checked = 
                btnNorfair.Checked = 
                btnRidley.Checked = 
                btnTourain.Checked = false;

            ((ToolStripButton)sender).Checked = true;

            switch(((ToolStripButton)sender).Text) {
                case "Brinstar":
                    Level = LevelIndex.Brinstar;
                    break;
                case "Norfair":
                    Level = LevelIndex.Norfair;
                    break;
                case "Ridley":
                    Level = LevelIndex.Ridley;
                    break;
                case "Kraid":
                    Level = LevelIndex.Kraid;
                    break;
                case "Tourain":
                    Level = LevelIndex.Tourain;
                    break;
            }
        }


        #region ItemInstance Lists

        //bool singleByteMode = false;
        bool loadingItem = false;
        void ShowItemList() {
            loadingItem = true;
            //singleByteMode = false;

            lstSubtypes.Items.Clear();

            if(editTool is EnemyEditTool) {
                pnlEnemyEditor.Enabled = pnlEnemyEditor.Visible = true;
                EnemyEditTool enemyEditor = ((EnemyEditTool)editTool) ;
                nudEnemyType.Value = editTool.GetIndex(currentRoom);

                nudSpriteSlot.Value = enemyEditor.GetSlot(currentRoom);
                chkHard.Checked = enemyEditor.GetHard(currentRoom);
            } else {
                lstSubtypes.Items.AddRange(editTool.GetList());
                lstSubtypes.SelectedIndex = editTool.GetIndex(currentRoom);
                lstSubtypes.Enabled = !(editTool is UnknowItemEditTool);

                pnlEnemyEditor.Enabled = pnlEnemyEditor.Visible = false;
            }
            //switch(currentItem.ItemType) {
            //    case ItemType.Elevator:
            //        lstSubtypes.Items.AddRange(elevatorItems);
            //        lstSubtypes.SelectedIndex = GetEditorIndex((int)currentItem.Destination);
            //        lstSubtypes.Enabled = true;
            //        break;
            //    case ItemType.PowerUp:
            //        lstSubtypes.Items.AddRange( standardItems);
            //        lstSubtypes.SelectedIndex = (int)currentItem.PowerUp;
            //        lstSubtypes.Enabled = true;
            //        break;
            //    case ItemType.PalSwap:
            //        lstSubtypes.Items.AddRange(SingleByteItems);
            //        lstSubtypes.SelectedIndex = 2;
            //        lstSubtypes.Enabled = true;
            //        singleByteMode = true;
            //        break;
            //    case ItemType.Mella:
            //        lstSubtypes.Items.AddRange(SingleByteItems);
            //        lstSubtypes.SelectedIndex = 0;
            //        lstSubtypes.Enabled = true;
            //        singleByteMode = true;
            //        break;
            //    case ItemType.Rinkas:
            //        lstSubtypes.Items.AddRange(SingleByteItems);
            //        lstSubtypes.SelectedIndex = 1;
            //        lstSubtypes.Enabled = true;
            //        singleByteMode = true;
            //        break;
            //    case ItemType.Enemy:
            //        lstSubtypes.Items.AddRange(enemyItems);
            //        lstSubtypes.SelectedIndex = EnemyIndexToListIndex((int)currentItem.PowerUp);
            //        lstSubtypes.Enabled = true;
            //        break;
            //    case ItemType.RespawnEnemy:
            //        lstSubtypes.Items.AddRange(respawnEnemyItems);
            //        lstSubtypes.SelectedIndex = EnemyIndexToListIndex((int)currentItem.PowerUp);
            //        lstSubtypes.Enabled = true;
            //        break;
            //    default:
            //        lstSubtypes.Items.AddRange(unknownItems);
            //        lstSubtypes.SelectedIndex = 0;
            //        lstSubtypes.Items[0] = lstSubtypes.Items[0].ToString() + "  0x" + ((int)currentItem.ItemType).ToString("x").PadLeft(2, ' ') + " " + ((int)currentItem.PowerUp).ToString("x").PadLeft(2, ' ');
            //        lstSubtypes.Enabled = false;
            //        break;
            //}

            loadingItem = false;
        }

        /// <summary>Returns the list count that corresponds to an enemy value</summary>
        int EnemyIndexToListIndex(int index) {
            bool hard = (index & 0x80) != 0;
            return (index & 0x0F) | (hard ? 0x10 : 0);
        }
        /// <summary>Returns the enemy value that corresponds to a list count</summary>
        int ListIndexToEnemyIndex(int index) {
            bool hard = (index & 0x10) != 0;
            return (index & 0x0F) | (hard ? 0x80 : 0);
        }

        /// <summary>
        /// Converts an elevator destination to an ItemEditor count.
        /// </summary>
        /// <param name="dest">The elevator destination numerical value.</param>
        /// <returns>An integer representing a list gameItem.</returns>
        private static int GetEditorIndex(int dest) {
            if((dest & 0xF) == 0xF) dest -= 0xA;
            if((dest & 0x80) == 0x80) dest += 4;
            dest = dest & 0x7F;
            return dest;
        }

        /// <summary>
        /// Converts an ItemEditor count into an ElevatorDestination equivalent integer value.
        /// </summary>
        /// <param name="count">Index.</param>
        /// <returns>Integer representation of an ElevatorDestination.</returns>
        private static int GetDestinationIndex(int index) {
            if(index == 9) return (int)(ElevatorDestination.GameEnd | ElevatorDestination.ElevatorUp);
            if(index > 4) {
                index -= 4;
                index |= 0x80;
            }

            return index;
        }

        //string[] standardItems = { "Bomb", "High Jump", "Long Beam", "Screw Attack", "Maru Mari", "Varia", "Wave Beam", "Ice Beam", "Energy Tank", "Missile", "(Invalid)", "(Invalid)", "(Invalid)", "(Invalid)", "(Invalid)", "(Invalid)" };

        //string[] elevatorItems = { "Brinstar", "Norfair", "Kraid", "Tourain", "Ridley", "Norfair Exit", "Kraid Exit", "Tourain Exit", "Ridley Exit", "Complete Game" };

        ////string[] mellaItems = { "Mella" };
        ////string[] rinkaItems = { "Rinkas" };
        ////string[] palSwapItems = { "Palette Switch" };
        //string[] unknownItems = { "Unknown" };
        //string[] SingleByteItems = { "Mella", "Rinka", "Palette Swap" };
        //string[] enemyItems = { "enemy (0)", "enemy (1)", "enemy (2)", "enemy (3)", "enemy (4)", "enemy (5)", "enemy (6)", "enemy (7)", "enemy (8)", "enemy (9)", "enemy (T)", "enemy (B)", "enemy (C)", "enemy (D)", "enemy (E)", "enemy (F)", "hard enemy (0)", "hard enemy (1)", "hard enemy (2)", "hard enemy (3)", "hard enemy (4)", "hard enemy (5)", "hard enemy (6)", "hard enemy (7)", "hard enemy (8)", "hard enemy (9)", "hard enemy (T)", "hard enemy (B)", "hard enemy (C)", "hard enemy (D)", "hard enemy (E)", "hard enemy (F)" };
        //string[] respawnEnemyItems = { "respawn enemy (0)", "respawn enemy (1)", "respawn enemy (2)", "respawn enemy (3)", "respawn enemy (4)", "respawn enemy (5)", "respawn enemy (6)", "respawn enemy (7)", "respawn enemy (8)", "respawn enemy (9)", "respawn enemy (T)", "respawn enemy (B)", "respawn enemy (C)", "respawn enemy (D)", "respawn enemy (E)", "respawn enemy (F)", "hard respawn enemy (0)", "hard respawn enemy (1)", "hard respawn enemy (2)", "hard respawn enemy (3)", "hard respawn enemy (4)", "hard respawn enemy (5)", "hard respawn enemy (6)", "hard respawn enemy (7)", "hard respawn enemy (8)", "hard respawn enemy (9)", "hard respawn enemy (T)", "hard respawn enemy (B)", "hard respawn enemy (C)", "hard respawn enemy (D)", "hard respawn enemy (E)", "hard respawn enemy (F)" };
        #endregion

        private void lstSubtypes_SelectedIndexChanged(object sender, EventArgs e) {
            if(!loadingItem) {
                editTool.SetIndex(currentRoom, lstSubtypes.SelectedIndex);
            }
        }

        private void nudEnemyType_ValueChanged(object sender, EventArgs e) {
            if(!loadingItem) {
                editTool.SetIndex(currentRoom, (int)nudEnemyType.Value);
            }
        }

        private void nudSpriteSlot_ValueChanged(object sender, EventArgs e) {
            if(!loadingItem) {
                ((EnemyEditTool)editTool).SetSlot(currentRoom, (int)nudSpriteSlot.Value);
            }
        }

        private void chkHard_CheckedChanged(object sender, EventArgs e) {
            if(!loadingItem)
                ((EnemyEditTool)editTool).SetHard(currentRoom, chkHard.Checked);
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e) {
            currentRoom.SeekToFirstItemInScreen();
            for(int i = 0; i < lstItems.SelectedIndex; i++) {
                currentRoom.NextItem();
            }

            DisplaySelectedItem();
        }


        internal void SelectItem(ItemEntryScreenTile editor) {
            SelectedScreen = editor;
            LoadScreen();

            GetPasswordEntry();
            this.editTool = ItemEditTool.GetEditTool(currentRoom);

            ShowScreenAt(GetMapPosition());

            DisplaySelectedItem();
        }
    }


}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Editroid.Graphic;
using Editroid.ROM;

namespace Editroid
{
    /// <summary>
    /// Component that allows the editing of a Metroid screen by providing a GUI and/or exposing .
    /// </summary>
    /// <remarks>Editing changes will be made directly to ROM data immediately and can not be undone.</remarks>
#if DEBUG
    public partial class ScreenControl:UserControl
#else
	public partial class ScreenControl:Panel
#endif
    {

        /// <summary>Creates an instance of this control.</summary>
        public ScreenControl() {
            this.SetStyle(ControlStyles.Selectable, false);
            InitializeComponent();

            ScreenBitmap = new Bitmap(256, 256, PixelFormat.Format8bppIndexed);
            pnlScreen.Image = ScreenBitmap;
            pnlScreen.MouseDown += new MouseEventHandler(Screen_MouseDown);
            pnlScreen.MouseMove += new MouseEventHandler(Screen_MouseMove);
            pnlScreen.MouseUp += new MouseEventHandler(Screen_MouseUp);
        }


        #region Variables
        // Graphic objects
        /// <summary>The screen image is drawn to this bitmap.</summary>
        Bitmap ScreenBitmap;
        /// <summary>Takes screen data and renders a screen image using a Graphic.Blitter object.</summary>
        Graphic.ScreenDisplay screenRenderer = new Editroid.Graphic.ScreenDisplay();
        /// <summary>Performs actual drawing for screen rendering.</summary>
        Graphic.Blitter screenBlitter = new Editroid.Graphic.Blitter();
        /// <summary>The highlighting effect used for selected object.</summary>
        private HighlightEffect selectionHighlight = HighlightEffect.Lighten;


        // Rom data objects
        /// <summary>The ROM being edited.</summary>
        private Rom _Rom;
        /// <summary>Level specific data for sprites (used for rendering enemies).</summary>
        Sprite[] SpriteData = null;
        /// <summary>General level data for displayed screen.</summary>
        Level LevelData = null;
        /// <summary>General screen data for displayed screen.</summary>
        Screen ScreenData;


        // Editor objects
        /// <summary>The currently selected object or enemy.</summary>
        ScreenItem _SelectedItem = new ScreenItem();
        /// <summary>Specifies the map location of the current screen, used for referencing map data (e.x. level specified on map) and global data (e.x. item data).</summary>
        private Point mapLocation;
        #endregion

        #region Public properties

        Levels _LevelIndex = Levels.Brinstar;
        /// <summary>Specifies which level data will be loaded for or is currently loaded for.</summary>
        public Levels LevelIndex {
            get { return _LevelIndex; }
            set {
                _LevelIndex = value;
                //LoadData();
            }
        }

        private int _ScreenIndex;
        /// <summary>The index of the screen to display.</summary>
        public int ScreenIndex {
            get { return _ScreenIndex; }
            set {
                _ScreenIndex = value;
                lblScreen.Text = value.ToString("x");
                LoadData();
            }
        }

        /// <summary>
        /// Gets or sets the map location that the current screen represents. This is
        /// used to reference global data that might be relevant to this screen.
        /// </summary>
        public Point MapLocation { get { return mapLocation; } set { mapLocation = value; } }

        /// <summary>
        /// Gets/sets which ROM image to load data from
        /// </summary>
        public Rom Rom {
            get { return _Rom; }
            set {
                _Rom = value;
                LoadData();
            }
        }

        private bool _UseAlternatePalette = false;
        /// <summary>
        /// Whether or not to use this level's alternate palette
        /// </summary>
        public bool AltPalette { get { return _UseAlternatePalette; } set { _UseAlternatePalette = value; } }

        /// <summary>Gets/sets the highlighting effect used for the selected object.</summary>
        public HighlightEffect SelectionHighlight { get { return selectionHighlight; } set { selectionHighlight = value; } }

        /// <summary>Gets the data for the screen being displayed.</summary>
        public Screen CurrentScreen { get { return ScreenData; } }

        #endregion

        #region Hit testing
        /// <summary>
        /// Gets the enemy at the specified location.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns>The enemy at the specified location, or 
        /// ScreenEnemy.Nothing if no enemy is found.</returns>
        public ScreenEnemy EnemyAtPixel(int x, int y) {
            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;
            x = x > 255 ? 255 : x;
            y = y > 239 ? 239 : y;

            return EnemyAtTile(x / 8, y / 8);
        }

        private ScreenEnemy GetEnemyAt(int x, int y) {
            for(int i = 0; i < ScreenData.Enemies.Length; i++) {
                ScreenEnemy enemy = ScreenData.Enemies[i];

                // Errors can occur if there are not enough enemies defined
                if(SpriteData.Length < enemy.EnemyType) continue;

                Sprite enemySprite = SpriteData[enemy.EnemyType];

                Rectangle enemyBounds = enemySprite.Measure();
                enemyBounds.X += enemy.X * 16;
                enemyBounds.Y += enemy.Y * 16;

                if(enemyBounds.Contains(x, y)) return enemy;
            }

            return ScreenEnemy.Nothing;
        }

        private int GetEnemyIndexAt(int x, int y) {
            if(ScreenData == null) return -1;
            if(SpriteData == null) return -1;

            for(int i = 0; i < ScreenData.Enemies.Length; i++) {
                ScreenEnemy enemy = ScreenData.Enemies[i];

                // Errors can occur if there are not enough enemies defined
                if(SpriteData.Length < enemy.EnemyType) continue;

                Sprite enemySprite = SpriteData[enemy.EnemyType];

                Rectangle enemyBounds = enemySprite.Measure();
                enemyBounds.X += enemy.X * 16;
                enemyBounds.Y += enemy.Y * 16;

                if(enemyBounds.Contains(x, y)) return i;
            }

            return -1;
        }

        private ScreenEnemy EnemyAtTile(int x, int y) {
            for(int i = 0; i < ScreenData.Enemies.Length; i++) {
                ScreenEnemy enemy = ScreenData.Enemies[i];

                if(enemy.X < x || enemy.Y < y) continue;
                if(SpriteData.Length < enemy.EnemyType) continue;

                Sprite enemySprite = SpriteData[enemy.EnemyType];
                if((enemy.X - x + 1) > enemySprite.Width ||
                    (enemy.Y - y + 1) > enemySprite.Height)
                    continue;

                return enemy;
            }

            return ScreenEnemy.Nothing;
        }

        /// <summary>
        /// Gets the enemy found in the specified tile.
        /// </summary>
        /// <param name="x">X coordiante.</param>
        /// <param name="y">Y coordinate.</param>
        /// <returns></returns>
        public ScreenEnemy EnemyAt(int x, int y) {
            return EnemyAtTile(2 * x, 2 * y);
        }

        /// <summary>
        /// Gets which object is at a specified pixel coorinate.
        /// </summary>
        /// <param name="x">X-coordinate.</param>
        /// <param name="y">Y-coordinate.</param>
        /// <returns>The object at the specified coordinates.</returns>
        public ScreenObject ObjectAtPixel(int x, int y) {
            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;
            x = x > 255 ? 255 : x;
            y = y > 239 ? 239 : y;
            return ObjectAt(x / 16, y / 16);
        }
        /// <summary>
        /// Gets which object is at a specified tile coordinate
        /// </summary>
        /// <param name="tileX">The x-coordinate.</param>
        /// <param name="tileY">The y-coordinate</param>
        /// <returns>The object at a specified coordinate.</returns>
        public ScreenObject ObjectAt(int tileX, int tileY) {
            if(ScreenData == null || LevelData == null) return ScreenObject.Nothing;

            for(int i = ScreenData.Objects.Length - 1; i >= -1; i--) {
                if(i == -1) { // If object not found
                    return ScreenObject.Nothing;
                }

                ScreenObject o = ScreenData.Objects[i];
                if(tileX < o.X || tileY < o.Y) continue;
                Struct s = LevelData.GetStruct(o.ObjectType);
                if(tileX >= o.X + s.Width) continue;
                if(tileY >= o.Y + s.Height) continue;

                //if(s.Tiles[tileY - o.X].Length - tileX > 0) return o;
                if(tileY - o.Y >= s.Combos.Length ||
                    s.Combos[tileY - o.Y].Length <= (tileX - o.X) ||
                    s.Combos[tileY - o.Y][tileX - o.X] == Struct.EmptyTile) continue;
                return ScreenData.Objects[i];
            }
            return ScreenData.Objects[0];
        }

        private void OnSelectedObjectChanged() {
            if(SelectedObjectChanged != null)
                SelectedObjectChanged(this, new EventArgs());

            UpdateSelectionStatus();
        }
        #endregion

        #region Selection Management
        /// <summary>
        /// Gets the index of the selected object
        /// </summary>
        /// <returns>The index of an object</returns>
        public int SelectedObjectIndex() {
            int i = 0;
            while(i < ScreenData.Objects.Length && !(SelectedItem.Equals(ScreenData.Objects[i]))) {
                i++;
            }

            if(!(SelectedItem.Equals(ScreenData.Objects[i]))) throw new Exception("Object not found in this screen.");

            return i;
        }

        private void ShowSelection() {
            ShowScreen();
        }

        private bool ShouldSerializeSelectedItem() { return false; }
        /// <summary>Gets which item has been selected within this control.</summary>
        [Browsable(false)]
        public ScreenItem SelectedItem {
            get {
                return _SelectedItem;
            }
        }

        /// <summary>
        /// Updates controls to properly reflect the state of the selection.
        /// </summary>
        private void UpdateSelectionStatus() {
            if(SelectedItem.IsNothing || LevelData == null) {
                //lblType.Text = "00";
                //lblSlot.Text = lblPal.Text = "0";
                lblType.Visible = false;
                lblSlot.Visible = false;
                lblPal.Visible = false;
                lblScreen.Visible = (LevelData != null);
                lblMemory.Visible = (LevelData != null);
                EnemyMode = false;
            } else {
                lblType.Visible = true;
                lblSlot.Visible = true;
                lblPal.Visible = true;
                lblScreen.Visible = true;
                lblMemory.Visible = true;

                lblType.Text = SelectedItem.Type.ToString("x").PadLeft(2, '0');
                EnemyMode = SelectedItem.SelectionType == ScreenItemType.Enemy;
                if(EnemyMode) {
                    lblPal.Text = GetSelectedEnemyPaletteString();
                    lblSlot.Text = SelectedItem.ThisEnemy.SpriteSlot.ToString("x");
                } else {
                    lblPal.Text = SelectedItem.Palette.ToString();
                }
            }
        }

        private string GetSelectedEnemyPaletteString() {
            return (SelectedItem.Palette == 0) ?
                                "0" : "1";
        }

        /// <summary>Raised when an object is selected.</summary>
        public event EventHandler SelectedObjectChanged;

        /// <summary>
        /// Gets or sets the index which specifies which enemy to highlight
        /// on the screen. This is for display purposes only.
        /// </summary>
        public int SelectedEnemyIndex {
            get { return screenRenderer.SelectedEnemy; }
            set { screenRenderer.SelectedEnemy = value; }
        }

        /// <summary>
        /// Deselects the selected item.
        /// </summary>
        public void Deselect() {
            _SelectedItem = new ScreenItem();
        }

        #endregion

        #region Mouse Editing
        bool DraggingObject = false;
        int lasttilex, lasttiley;

        /// <summary>
        /// Overrides base member.
        /// </summary>
        /// <param name="sender">Sender parameter.</param>
        /// <param name="e">e paremeter.</param>
        protected void Screen_MouseDown(object sender, MouseEventArgs e) {
            // Figure out what was clicked
            int clickedEnemyIndex = GetEnemyIndexAt(e.X, e.Y);
            ScreenEnemy clickedEnemy = ScreenEnemy.Nothing;
            if(clickedEnemyIndex != -1) clickedEnemy = ScreenData.Enemies[clickedEnemyIndex]; //GetEnemyAt(e.X, e.Y);
            ScreenObject clickedObject = ObjectAtPixel(e.X, e.Y);



            if(e.Button == MouseButtons.Left) {
                if(clickedEnemy.IsNothing) {
                    // Select clicked object, if it isnt already selected
                    if(!clickedObject.Equals(_SelectedItem)) {
                        _SelectedItem.ThisObject = clickedObject;
                        screenRenderer.SelectedEnemy = -1;
                        OnSelectedObjectChanged();
                        ShowSelection();
                    }
                } else {
                    _SelectedItem.ThisEnemy = clickedEnemy;

                    OnSelectedObjectChanged();
                    ShowSelection();
                }

                // Begin dragging
                lasttilex = e.X / 16;
                lasttiley = e.Y / 16;

                if(!(clickedEnemy.IsNothing && clickedObject.IsNothing))
                    DraggingObject = true;
            } else if(e.Button == MouseButtons.Right) {
                if(!SelectedItem.IsNothing) {
                    if(!clickedObject.IsNothing) {
                        // Remove object
                        LevelData.CropScreenData(ScreenData, clickedObject.Offset, 3);

                            // Select something 
                            MakeDefaultSelection();

                        // Refresh
                        RefreshData(true, false);
                    }else if(!clickedEnemy.IsNothing) {
                        // Remove object
                        LevelData.CropScreenData(ScreenData, clickedEnemy.Offset, 3);

                            // Select something 
                            MakeDefaultSelection();

                        // Refresh
                        RefreshData(true, false);
                    }
                }
            }
        }

        private void Screen_MouseMove(object sender, MouseEventArgs e) {
            if(DraggingObject) {
                int tilex = e.X / 16;
                int tiley = e.Y / 16;
                int dx = tilex - lasttilex;
                int dy = tiley - lasttiley;
                if(dx != 0 || dy != 0) {
                    MoveObject(dx, dy);
                    OnSelectionDragged();
                }
                lasttilex =
                    (tilex < 0) ?
                        0 :
                        (tilex > 15) ?
                            15 :
                            tilex;
                lasttiley =
                    (tiley < 0) ?
                        0 :
                        (tiley > 14) ?
                            15 :
                            tiley;

            }
        }

        private void Screen_MouseUp(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) DraggingObject = false;
        }

        /// <summary>Occurs when the selected object is dragged.</summary>
        public event EventHandler SelectionDragged;
        /// <summary>
        /// Raises the SelectionDragged event.
        /// </summary>
        protected void OnSelectionDragged() { if(SelectionDragged != null)SelectionDragged(this, new EventArgs()); }

        #endregion

        #region Code Editing
        /// <summary>
        /// Selects the struct after the currently selected struct
        /// </summary>
        public void SelectNext() {
            if (_SelectedItem.SelectionType == ScreenItemType.Object) {
                int structindex = GetObjectIndex(_SelectedItem.ThisObject) + 1;
                if(structindex == ScreenData.Objects.Length) structindex = 0;
                _SelectedItem.ThisObject = ScreenData.Objects[structindex];
                OnSelectedObjectChanged();
            } else if (_SelectedItem.SelectionType == ScreenItemType.Enemy) {
                int enemyIndex = SelectedEnemyIndex + 1;
                if(enemyIndex == ScreenData.Enemies.Length) enemyIndex = 0;
                _SelectedItem.ThisEnemy = ScreenData.Enemies[enemyIndex];
                SelectedEnemyIndex = enemyIndex;

                OnSelectedObjectChanged();
            } else {
                if(ScreenData.Objects.Length > 0) {
                    _SelectedItem.ThisObject = ScreenData.Objects[0];
                    OnSelectedObjectChanged();
                } else {
                    _SelectedItem.IsNothing = true;
                }
            }
            //SeekSelectedObjectType(1);
        }

        /// <summary>Selects the struct before the currently selected struct.</summary>
        public void SelectPrevious() {
            switch(_SelectedItem.SelectionType) {
                case ScreenItemType.Object:
                    // If we are at the first item, loop to the end, else go back 1 item
                    if(_SelectedItem.ThisObject.Equals(ScreenData.Objects[0]))
                        _SelectedItem.ThisObject = ScreenData.Objects[ScreenData.Objects.Length - 1];
                    else
                        _SelectedItem.ThisObject = _SelectedItem.ThisObject.PreviousItem;
                    OnSelectedObjectChanged();
                    break;
                case ScreenItemType.Door:
                    // If we are at the first item, loop to the end, else go back 1 item
                    if(_SelectedItem.ThisDoor.Equals(ScreenData.Doors[0]))
                        _SelectedItem.ThisDoor = ScreenData.Doors[ScreenData.Doors.Length - 1];
                    else
                        _SelectedItem.ThisDoor = _SelectedItem.ThisDoor.PreviousItem;
                    OnSelectedObjectChanged();

                    break;
                case ScreenItemType.Enemy:
                    int enemyIndex = SelectedEnemyIndex - 1;
                    if(enemyIndex == -1) enemyIndex += ScreenData.Enemies.Length;
                    _SelectedItem.ThisEnemy = ScreenData.Enemies[enemyIndex];
                    SelectedEnemyIndex = enemyIndex;

                    OnSelectedObjectChanged();
                    break;
                default:
                    throw new InvalidOperationException("Operation not valid on current selection.");
            }
            //SeekSelectedObjectType(-1);
        }

        void SetPalette(byte palette, ScreenObject obj) {
            obj.PalData = (byte)palette;
        }

        /// <summary>
        /// Sets the palette of an object.
        /// </summary>
        /// <param name="palette">The palette to apply to the object</param>
        /// <param name="obj">The index of the object to modify</param>
        public void SetPalette(byte palette, int obj) {
            SetPalette(palette, ScreenData.Objects[obj]);
        }

        /// <summary>
        /// Sets the palette of the selected object if that object is not
        /// an enemy or door.
        /// </summary>
        /// <param name="palette">The palette to apply to the object</param>
        public void SetPalette(byte palette) {
            if (SelectedItem.SelectionType == ScreenItemType.Object)
                SetPalette(palette, SelectedItem.ThisObject);
        }
        void NextPalette(ScreenObject obj) {
            obj.PalData = (byte)((obj.PalData + 1) % 4);
        }
        void NextPalette(ScreenEnemy obj) {
            obj.SpritePal = obj.SpritePal ^ 0x8;
        }

        /// <summary>
        /// Changes the palette of the specified object.
        /// </summary>
        /// <param name="obj">Index of the object to modify</param>
        public void NextPalette(int obj) {
            NextPalette(ScreenData.Objects[obj]);
        }

        /// <summary>
        /// Changes the palette of the selected object
        /// </summary>
        public void NextPalette() {
            if (SelectedItem.SelectionType == ScreenItemType.Object)
                NextPalette(SelectedItem.ThisObject);
            else if (SelectedItem.SelectionType == ScreenItemType.Enemy)
                NextPalette(SelectedItem.ThisEnemy);
            // Operation is ignored if there is no selection
        }

        /// <summary>
        /// Seeks through the possible types for the selected item.
        /// </summary>
        /// <param name="count">The number of structs to seek past if the selected item is an object, or a positive number or negative number to indicate which way to seek through the enemies.</param>
        public void SeekSelectedObjectType(int count) {
            if (_SelectedItem.SelectionType == ScreenItemType.Object) {
                int newIndex = (_SelectedItem.Type + count) % LevelData.StructCount;
                if(newIndex < 0) newIndex += LevelData.StructCount;
                _SelectedItem.Type = newIndex;
            } else if (_SelectedItem.SelectionType == ScreenItemType.Enemy) {
                Sprite[] levelSprites = Sprite.GetSprites(LevelIndex);
                int enemyTypeCount = levelSprites.Length;
                ScreenEnemy enemy = _SelectedItem.ThisEnemy;

                if(count > 0) {
                    do {
                        enemy.EnemyType++;
                        if(enemy.EnemyType >= enemyTypeCount) enemy.EnemyType = 0;
                    } while(levelSprites[enemy.EnemyType].IsBlank);
                } else {
                    do {
                        enemy.EnemyType--;
                        if(enemy.EnemyType < 0 || enemy.EnemyType > enemyTypeCount - 1) enemy.EnemyType = enemyTypeCount - 1;
                    } while(levelSprites[enemy.EnemyType].IsBlank);
                }
            }
        }

        /// <summary>
        /// Private implementation of BringObjToFront
        /// </summary>
        /// <param name="obj">Object to modify</param>
        void BringToFront(ScreenObject obj) {
            int FrontObjectData = obj.CopyData();


            for(int i = GetObjectIndex(obj); i < ScreenData.Objects.Length - 1; i++) {
                ScreenData.Objects[i + 1].CopyData(ScreenData.Objects[i]);
            }

            SelectedItem.Item = ScreenData.Objects[ScreenData.Objects.Length - 1];
            SelectedItem.ThisObject.SetData(FrontObjectData);
        }

        /// <summary>
        /// Brings a screen object in front of all other objects in the screen.
        /// </summary>
        /// <param name="obj">The object to bring to the front</param>
        public void BringObjToFront(int obj) {
            BringToFront(ScreenData.Objects[obj]);
        }

        /// <summary>
        /// Brings a screen object in front of all other objects in the screen.
        /// </summary>
        public void BringObjToFront() {
            if (_SelectedItem.SelectionType == ScreenItemType.Object)
                BringToFront(_SelectedItem.ThisObject);
        }

        /// <summary>
        /// Sends a screen object behind all other objects in the screen.
        /// </summary>
        /// <param name="obj">The object to send to the back</param>
        public void SendObjToBack(int obj) {
            SendToBack(ScreenData.Objects[obj]);
        }

        /// <summary>
        /// Sends a screen object behind all other objects in the screen.
        /// </summary>
        public void SendObjToBack() {
            if (_SelectedItem.SelectionType == ScreenItemType.Object)
                SendToBack(_SelectedItem.ThisObject);
        }

        /// <summary>
        /// Private implementation of Object.SendToBack
        /// </summary>
        /// <param name="obj">Object to send</param>
        void SendToBack(ScreenObject obj) {
            int BackObjectData = obj.CopyData();


            for(int i = GetObjectIndex(obj); i > 0; i--) {
                ScreenData.Objects[i - 1].CopyData(ScreenData.Objects[i]);
            }


            SelectedItem.ThisObject = ScreenData.Objects[0];
            SelectedItem.ThisObject.SetData(BackObjectData);
        }

        /// <summary>
        /// Toggles selection between enemies and objects
        /// </summary>
        public void ToggleSelectionType() {
            if (SelectedItem.SelectionType == ScreenItemType.Enemy) {
                if(ScreenData.Objects.Length > 0) {
                    SelectedItem.ThisObject = ScreenData.Objects[0];
                    SelectedEnemyIndex = -1;
                }
            } else {
                if(ScreenData.Enemies.Length > 0) {
                    SelectedItem.ThisEnemy = ScreenData.Enemies[0];
                    SelectedEnemyIndex = 0;
                }
            }

            OnSelectedObjectChanged();
        }

        /// <summary>Increases the selected enemy's slot.</summary>
        public void NextEnemySlot() {
            if (SelectedItem.SelectionType == ScreenItemType.Enemy) {
                ScreenEnemy enemy = SelectedItem.ThisEnemy;
                enemy.SpriteSlot = (enemy.SpriteSlot + 1) % 6;
                lblSlot.Text = enemy.SpriteSlot.ToString("x");
            }
        }
        /// <summary>
        /// Deletes the selected object from the screen data and updates the ROM image to free the memory the object used.
        /// </summary>
        public void RemoveSelectedItem() {
            int itemSize = 3;
            int offset = 0;

            switch(_SelectedItem.SelectionType) {
                case ScreenItemType.Object:
                    offset = _SelectedItem.ThisObject.Offset;
                    break;
                case ScreenItemType.Door:
                    offset = _SelectedItem.ThisDoor.Offset;
                    itemSize = 2; // Doors are only two bytes
                    break;
                case ScreenItemType.Enemy:
                    offset = _SelectedItem.ThisEnemy.Offset;
                    break;
                default:
                    return;
            }
            LevelData.CropScreenData(ScreenData, offset, itemSize);

            RefreshData(true, true);
        }

        /// <summary>
        /// Toggles whether the selected enemy will respawn.
        /// </summary>
        public void ToggleRespawn() {
            if (_SelectedItem.SelectionType == ScreenItemType.Enemy) {
                ScreenEnemy enemy = _SelectedItem.ThisEnemy;
                enemy.Respawn = !enemy.Respawn;
            }
        }

        /// <summary>
        /// Offsets the position of an object.
        /// </summary>
        /// <param name="distX">The distance to move the object.</param>
        /// <param name="distY">The distance to move the object.</param>
        /// <remarks>If the object is moved out of bounds, it will automatically
        /// be moved to the nearest border.</remarks>
        public void MoveObject(int distX, int distY) {
            //if(_SelectedItem.SelectionType == ScreenItem.ObjectType.Object) {
            //    ScreenObject selectedObject = _SelectedItem.ThisObject;
            if(SelectedItem.IsNothing) return;

            int newX = _SelectedItem.X + distX;
            newX = newX < 0 ? 0 : newX;
            newX = newX > 0xF ? 0xF : newX;
            int newY = _SelectedItem.Y + distY;
            newY = newY < 0 ? 0 : newY;
            newY = newY > 0xE ? 0xE : newY;

            _SelectedItem.X = newX;
            _SelectedItem.Y = newY;
            ShowSelection();
            //}
        }

        internal ScreenObject AddObject() {
            int newObjectOffset = ScreenData.Offset + 1 + ScreenData.Objects.Length * 3;

            LevelData.ExpandScreenData(ScreenData,
                newObjectOffset,
                3);

            ScreenObject newObject = new ScreenObject(_Rom.data, newObjectOffset);
            newObject.PalData = 0;
            newObject.X = 0;
            newObject.Y = 0;
            newObject.ObjectType = 0;

            RefreshData(false, false);
            return newObject;
        }

        /// <summary>Expands screen data and adds an enemy to the screen.</summary>
        /// <returns>A ScreenEnemy object that represents the newly added enemy.</returns>
        public ScreenEnemy AddEnemy() {
            // Get offset of new data
            int newEnemyOffset = ScreenData.EnemyDataOffset;
            // We prefer to add enemy data AFTER door data
            newEnemyOffset += ScreenData.Doors.Length * 2;

            int dataSize = 3;
            int newSpriteSlot = GetNewSpriteSlot(); // new SpriteSlot value.

            ExpandEnemyData(newEnemyOffset, dataSize);

            ScreenEnemy newEnemy = new ScreenEnemy(Rom.data, newEnemyOffset);
            newEnemy.Respawn = false;
            newEnemy.SpritePal = 0;
            newEnemy.SpriteSlot = newSpriteSlot;
            newEnemy.EnemyType = GetNewEnemyType();
            newEnemy.X = 3;
            newEnemy.Y = 3;

            RefreshData(false, false);

            return newEnemy;
        }

        /// <summary>Gets the index of the first available sprite slot</summary>
        private int GetNewSpriteSlot() {
            int slot = -1;
            bool found = false;

            // Until we find a free slot
            while(!found) {
                slot++; // Look at the next slot
                found = true; // We succeed...

                foreach(ScreenEnemy e in ScreenData.Enemies) {
                    if(e.SpriteSlot == slot) // ...unless the slot is in use
                        found = false;
                }
            }

            return slot;
        }

        private int GetNewEnemyType() {
            switch(LevelData.LevelIdentifier) {
                case Levels.Brinstar:
                    return 2;
                case Levels.Norfair:
                case Levels.Tourain:
                case Levels.Kraid:
                case Levels.Ridley:
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Expands current screens data for enemies/doors. If necessary,
        /// an enemy data section will be added.
        /// </summary>
        /// <param name="offset">The offset to expand at.</param>
        /// <param name="bytes">The number of free bytes needed.</param>
        /// <returns>The offset of the free space.</returns>
        int ExpandEnemyData(int offset, int bytes) {
            // Determine whether we need to add an enemy data segment marker (0xFD)
            bool needsEnemySegmentMarker = !ScreenData.HasEnemyDataSegment;
            if(needsEnemySegmentMarker) {
                offset--; // If so, it needs to go BEFORE the 0xFF screen data terminator
                bytes++; // And it will take up an extra byte.
            }

            LevelData.ExpandScreenData(CurrentScreen, offset, bytes);

            // Mark enemy data section (if necessary)
            if(needsEnemySegmentMarker) {
                _Rom.data[offset] = 0xFD;
                offset++;
            }

            return offset;
        }

        /// <summary>Specifies the door type for the left door.</summary>
        /// <param name="type"></param>
        /// <returns>The data object that was modified.</returns>
        public ScreenDoor SetLeftDoor(DoorType type) {
            // If there is alread a door, change it.
            foreach(ScreenDoor d in ScreenData.Doors) {
                if(d.Side == DoorSide.Left) {
                    ScreenDoor door = d; // Workaround for compiler lameness
                    door.Type = type;
                    return door;
                }
            }

            // There is no door yet, so add one.
            return AddDoorToCurrentScreen(type, DoorSide.Left);
        }
        /// <summary>Removes the left door.</summary>
        public void DeleteLeftDoor() {
            // Seek through doors and delete any that are left-side.
            foreach(ScreenDoor d in ScreenData.Doors) {
                if(d.Side == DoorSide.Left) {
                    LevelData.CropScreenData(CurrentScreen, d.Offset, 2);
                    return;
                }
            }
        }
        /// <summary>Specifies the door type for the right door.</summary>
        /// <param name="type"></param>
        /// <returns>The data object that was modified.</returns>
        public ScreenDoor SetRightDoor(DoorType type) {
            // If there is alread a door, change it.
            foreach(ScreenDoor d in ScreenData.Doors) {
                if(d.Side == DoorSide.Right) {
                    ScreenDoor door = d; // Workaround for compiler lameness
                    door.Type = type;
                    return door;
                }
            }

            // There is no door yet, so add one.
            return AddDoorToCurrentScreen(type, DoorSide.Right);
        }
        /// <summary>Removes the right door.</summary>
        public void DeleteRightDoor() {
            // Seek through doors and delete any that are right-side.
            foreach(ScreenDoor d in ScreenData.Doors) {
                if(d.Side == DoorSide.Right) {
                    LevelData.CropScreenData(CurrentScreen, d.Offset, 2);
                    return;
                }
            }
        }

        /// <summary>
        /// Expands level room data and adds a door to CurrentScreen
        /// </summary>
        private ScreenDoor AddDoorToCurrentScreen(DoorType type, DoorSide side) {
            int doorOffset = ScreenData.EnemyDataOffset; // Offset to insert data at
            int dataSize = 2;

            // Expand data (Door data is stored with enemy data)
            ExpandEnemyData(doorOffset, dataSize);

            // Add door data marker (0x02)
            _Rom.data[doorOffset] = 0x02;

            // ScreenDoor object is only used to initialize data
            ScreenDoor newDoor = new ScreenDoor(_Rom.data, doorOffset);
            newDoor.Side = side;
            newDoor.Type = type;
            return newDoor;
        }

        #endregion

        #region Rendering
        /// <summary>Finds any items at this map location and stores the data. This is used for rendering only.</summary>
        private void FindItems() {
            ItemTable table = Rom.GetLevel(LevelIndex).ItemTable;
            foreach(ItemEntry e in table) {
                if(e.MapY == mapLocation.Y) {
                    ItemSeeker seeker = e.Seek();
                    bool moreScreens = true;
                    while(moreScreens) {
                        if(seeker.MapX == mapLocation.X) {
                            if(seeker.ItemType == ItemType.PowerUp) {
                                screenRenderer.AddSprite(
                                    PowerUpSprites.GetSprite(seeker.PowerUp),
                                    seeker.ScreenPosition.X * 2,
                                    seeker.ScreenPosition.Y * 2,
                                    0x8);
                            } else if(seeker.ItemType == ItemType.Elevator) {
                                screenRenderer.AddSprite(
                                    ItemSprites.Elevator,
                                    14, 16, 8);
                            } else if(seeker.ItemType == ItemType.PalSwap) {
                                screenRenderer.AddSprite(
                                    ItemSprites.PalSwitch,
                                    15, 12, 8);
                            }
                        }
                        if(seeker.MoreScreens) {
                            seeker.NextScreen();
                        } else {
                            moreScreens = false;
                        }
                    }
                }
            }
        }
        /// <summary>Applies highlight effect to highlight palettes (colors 0x10-0x1F and 0x30-0x3F, which should mirror 0x00-0x0F and 0x20-0x2F prior to calling this function).</summary>
        private void ApplyHighlightFilter(ColorPalette p) {
            ApplyHighlightFilterToRange(p, 16, 31);
            ApplyHighlightFilterToRange(p, 48, 63);
        }
        /// <summary>Applies highlight effect to the specified range within a palette.</summary>
        private void ApplyHighlightFilterToRange(ColorPalette p, int start, int end) {
            int clr;
            for(int i = start; i <= end; i++) {
                switch(selectionHighlight) {
                    case HighlightEffect.Invert:
                        clr = p.Entries[i].ToArgb();
                        p.Entries[i] = Color.FromArgb((~clr & 0xFFFFFF) | -16777216);
                        break;
                    case HighlightEffect.InvertBack:
                        if(i % 4 == 0) {
                            clr = p.Entries[i].ToArgb();
                            p.Entries[i] = Color.FromArgb((~clr & 0xFFFFFF) | -16777216);
                        }
                        break;
                    case HighlightEffect.Lighten:
                        p.Entries[i] = Color.FromArgb(255,
                            255 - ((255 - p.Entries[i].R) / 2),
                            255 - ((255 - p.Entries[i].G) / 2),
                            255 - ((255 - p.Entries[i].B) / 2));
                        break;
                    case HighlightEffect.LightenInvertBack:
                        if(i % 4 == 0) {
                            clr = p.Entries[i].ToArgb();
                            p.Entries[i] = Color.FromArgb((~clr & 0xFFFFFF) | -16777216);
                        } else {
                            p.Entries[i] = Color.FromArgb(255,
                                255 - ((255 - p.Entries[i].R) / 2),
                                255 - ((255 - p.Entries[i].G) / 2),
                                255 - ((255 - p.Entries[i].B) / 2));
                        }
                        break;
                }
            }
        }

        /// <summary>Refreshes the display.</summary>
        public void ShowScreen() {
            ShowScreen(false);
        }
        /// <summary>Refreshes the display.</summary>
        /// <param name="physics">If true, the physics of the screen will be shown.</param>
        public void ShowScreen(bool physics) {
            if(_Rom == null | LevelData == null) {
                // Clear image
                screenRenderer.Clear();
                if(_Rom != null) {
                    screenRenderer.Level = Rom.Brinstar;
                    screenRenderer.Render(screenBlitter, ScreenBitmap);

                }

                // Update
                pnlScreen.Invalidate();
                UpdateSelectionStatus();

                return;
            }
            lblMemory.Text = LevelData.FreeRoomData.ToString();

            screenRenderer.DefaultPalette = _Rom.GetLevel(_LevelIndex).GetScreen(_ScreenIndex).ColorAttributeTable;
            screenRenderer.Clear();
            screenRenderer.Level = this.LevelData;
            screenRenderer.DrawScreen(_ScreenIndex, _SelectedItem.ThisObject);

            // Update enemy selection rectangle
            if (_SelectedItem.SelectionType == ScreenItemType.Enemy) {
                screenRenderer.SelectedEnemy = ScreenData.GetIndex(_SelectedItem.ThisEnemy);
            } else {
                screenRenderer.SelectedEnemy = -1;
            }
            screenRenderer.Level = _Rom.GetLevel(_LevelIndex);

            // Display item if one is found for this screen
            FindItems();

            screenRenderer.Render(screenBlitter, ScreenBitmap, ScreenData.Enemies, ScreenData.Doors, physics);

            ColorPalette bitmapPalette = ScreenBitmap.Palette;
            NesPalette bgPalette = LevelData.BgPalette;
            NesPalette spritePalette = LevelData.SpritePalette;
            if(_UseAlternatePalette && (LevelData.LevelIdentifier == Levels.Brinstar || LevelData.LevelIdentifier == Levels.Norfair)) 
                bgPalette = LevelData.BgAltPalette;

            // Load palettes twice
            bgPalette.ApplyTable(bitmapPalette.Entries);
            bgPalette.ApplyTable(bitmapPalette.Entries, 16);
            spritePalette.ApplyTable(bitmapPalette.Entries, 32);
            spritePalette.ApplyTable(bitmapPalette.Entries, 48);
            // Invert second palette for selections
            ApplyHighlightFilter(bitmapPalette);
            ScreenBitmap.Palette = bitmapPalette;

            bitmapPalette.Entries[NesPalette.HighlightEntry] = SystemColors.Highlight;

            pnlScreen.Invalidate();

            UpdateSelectionStatus();
        }

        #endregion


        /// <summary>Gets data (Level, Screen) needed to render the screen</summary>
        private void LoadData() {
            if(_Rom == null || _LevelIndex == Levels.None) {
                LevelData = null;
                SpriteData = null;
            } else {
                LevelData = Rom.GetLevel(_LevelIndex);
                SpriteData = Sprite.GetSprites(_LevelIndex);
            }

            if(LevelData != null) {
                ScreenData = LevelData.GetScreen(_ScreenIndex);
                //MakeDefaultSelection();
            }
        }
        /// <summary>Handles UI event.</summary>
        private void lblSlot_Click(object sender, EventArgs e) {
            NextEnemySlot();
        }

        /// <summary>
        /// Gets the index of a specified object. This object must be contained in the
        /// screen being displayed.
        /// </summary>
        /// <param name="obj">The object to obtain the index of</param>
        /// <returns>The index of an object</returns>
        public int GetObjectIndex(ScreenObject obj) {
            int i = 0;
            while(i < ScreenData.Objects.Length && !(obj.Equals(ScreenData.Objects[i]))) {
                i++;
            }

            if(i >= ScreenData.Objects.Length) throw new Exception("Object not found in this screen.");

            return i;
        }

        /// <summary>
        /// Gets the index of the specified door, enemy, or object.
        /// </summary>
        /// <param name="item">Object to get index of.</param>
        /// <returns>An integer index.</returns>
        public int GetItemIndex(ScreenItem item) { // Unused
            int i = 0;
            if (item.SelectionType == ScreenItemType.Object) {
                ScreenObject obj = item.ThisObject;
                for(i = 0; i < ScreenData.Objects.Length; i++) {
                    if(obj.Equals(ScreenData.Objects[i]))
                        return i;
                }

                throw new Exception("Item not found in this screen.");
            } else if (item.SelectionType == ScreenItemType.Enemy) {
                ScreenEnemy enemy = item.ThisEnemy;

                for(i = 0; i < ScreenData.Objects.Length; i++) {
                    if(enemy.Equals(ScreenData.Enemies[i]))
                        return i;
                }

                throw new Exception("Item not found in this screen.");
            } else if (item.SelectionType == ScreenItemType.Door) {
                ScreenDoor door = item.ThisDoor;

                for(i = 0; i < ScreenData.Objects.Length; i++) {
                    if(door.Equals(ScreenData.Doors[i]))
                        return i;
                }

                throw new Exception("Item not found in this screen.");
            }
            throw new InvalidOperationException("This operation can not be performed on the current selection.");
        }


        /// <summary>
        /// Refreshes the data for the currently displayed screen and then redraws the screen.
        /// </summary>
        public void RefreshData() {
            RefreshData(true, true);
        }
        /// <summary>
        /// Refreshes the data for the currently displayed screen.
        /// </summary>
        /// <param name="showScreen">If true, the screen image will be refreshed.</param>
        /// <param name="defaultSelection">If true, a default selection will be made.</param>
        public void RefreshData(bool showScreen, bool defaultSelection) {
            if(_Rom == null || _LevelIndex == Levels.None) {
                LevelData = null;
                SpriteData = null;
            } else {
                LevelData = Rom.GetLevel(_LevelIndex);
                SpriteData = Sprite.GetSprites(_LevelIndex);
            }

            if(LevelData != null) {
                ScreenData = LevelData.GetScreen(_ScreenIndex);
                if(defaultSelection) MakeDefaultSelection();
                if(showScreen) ShowSelection();
            }
        }

        /// <summary>Makes a default selection, for instance, when a new screen is shown.</summary>
        public void MakeDefaultSelection() {
            if(LevelIndex == Levels.None) {
                _SelectedItem.IsNothing = true;
                OnSelectedObjectChanged();
            } else if(ScreenData.Objects.Length > 0) {
                if(!_SelectedItem.ThisObject.Equals(ScreenData.Objects[0])) {

                    _SelectedItem.ThisObject = ScreenData.Objects[0];
                    OnSelectedObjectChanged();
                }
            } else if(ScreenData.Enemies.Length > 0) {
                _SelectedItem.ThisEnemy = ScreenData.Enemies[0];
                OnSelectedObjectChanged();
            } else {
                _SelectedItem.IsNothing = true;
                OnSelectedObjectChanged();
            }
        }

        /// <summary>
        /// Gets/sets whether the selection display will be set up for
        /// enemies.
        /// </summary>
        private bool EnemyMode {
            get { return lblSlot.Visible; }
            set {
                lblSlot.Visible = value;
            }
        }




    }
        /// <summary>Enumerates possible highlight effects for objects in a ScreenControl.</summary>
        public enum HighlightEffect
        {
            /// <summary>The object is inverted.</summary>
            Invert,
            /// <summary>The object is displayed using lighter colors</summary>
            Lighten,
            /// <summary>The object is displayed using lighter colors on an inverted background</summary>
            LightenInvertBack,
            /// <summary>The object is displayed on an inverted background</summary>
            InvertBack
        }
}


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Editroid.Graphic;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using Editroid.ROM;
using Editroid.Properties;
using Editroid.Extension;

namespace Editroid
{
    /// <summary>"Lightweight control" (windowless) that provides an interface to edit screen data.</summary>
    public class ScreenEditor: IDisposable 
    {

        public static readonly Size CellSize = new Size(258, 242);
        public static readonly Size ScreenImageSize = new Size(256, 256);
        bool isExtraneuos;

        public ScreenEditor() {
            ScreenBitmap = new Bitmap(ScreenImageSize.Width, ScreenImageSize.Height, PixelFormat.Format8bppIndexed);

        }

        #region Fields
        // Graphic objects
        /// <summary>The screen image is drawn to this brush.</summary>
        Bitmap ScreenBitmap;
        /// <summary>Takes screen data and renders a screen image using a Graphic.Blitter object.</summary>
        Graphic.ScreenRenderer screenRenderer = new Editroid.Graphic.ScreenRenderer();
        /// <summary>Performs actual drawing for screen rendering.</summary>
        Graphic.Blitter screenBlitter = new Editroid.Graphic.Blitter();
        /// <summary>The highlighting effect used for selected object.</summary>
        private HighlightEffect selectionHighlight = HighlightEffect.Lighten;
        Pen borderPen = new Pen(Color.FromArgb(32, 32, 32));


        // Rom data objects
        /// <summary>Level specific data for enemies (used for rendering enemies).</summary>
        SpriteDefinition[] SpriteData = null;
        /// <summary>General currentLevelIndex data for displayed screen.</summary>
        Level LevelData = null;
        /// <summary>General screen data for displayed screen.</summary>
        Screen ScreenData;


        // Editor objects
        /// <summary>The currently selected object or enemy.</summary>
        ObjectInstance _SelectedItem = null;
        /// <summary>Specifies the map location of the current screen, used for referencing map data (otherRow.x. currentLevelIndex specified on map) and global data (otherRow.x. gameItem data).</summary>
        private Point mapLocation;

        int DisplayOffsetX, DisplayOffsetY;
#endregion
        void on_HostViewportChanged(object sender, EventArgs<Point> e) {
            UpdateDisplayOffset();
        }
        /// <summary>Updates the cached value of the location at which this control should be rendered.</summary>
        private void UpdateDisplayOffset() {
            if (_host == null ) return ;
            DisplayOffsetX = mapLocation.X * CellSize.Width - _host.WorldViewport.X;
            DisplayOffsetY = mapLocation.Y * CellSize.Height - _host.WorldViewport.Y;
        }

        
        public Bitmap GetScreenImage() {
            return ScreenBitmap;
        }

        public void Paint(Graphics g) {
            Paint(WorldBounds, g);
        }
        public void Paint(Rectangle worldClipRect, Graphics g) {
            worldClipRect.Intersect(WorldBounds);

            //Destination
            Rectangle screenClippingRect = worldClipRect;
            screenClippingRect.X -= _host.WorldViewport.X;
            screenClippingRect.Y -= _host.WorldViewport.Y;
            if (isExtraneuos) {
                g.FillRectangle(Brushes.White, screenClippingRect);
                return;
            }
            
            Rectangle src = worldClipRect;
            src.X -= this.WorldBounds.X + 1;
            src.Y -= this.WorldBounds.Y + 1;

            //Rectangle src = new Rectangle(0, 0, ScreenImageSize.Width, ScreenImageSize.Height);
            //Rectangle dest = src;
            //dest.xTile = DisplayOffsetX + 1;
            //dest.yTile = DisplayOffsetY + 1;
            g.DrawImage(ScreenBitmap, screenClippingRect, src, GraphicsUnit.Pixel);
            if (HasFocus)
                g.DrawRectangle(Pens.Red, new Rectangle(DisplayOffsetX, DisplayOffsetY, CellSize.Width - 1, CellSize.Height - 1));
            else
                g.DrawRectangle(borderPen, new Rectangle(DisplayOffsetX, DisplayOffsetY, CellSize.Width - 1, CellSize.Height - 1));

            PaintFocusBorders(worldClipRect, g);
        }

        /// <summary>
        /// Paints focus border if the focused editor is adjacent (border acturally appears in
        /// client area of adjacent editors).
        /// </summary>
        private void PaintFocusBorders(Rectangle worldClipRect, Graphics g) {
            //var focusedEditor = this.EditorHost.FocusedEditor;
            //if (focusedEditor != null) {
            //    var focusMapLocation = focusedEditor.MapLocation;

            //    bool isAdjacenttHorizontally = (focusMapLocation.X == mapLocation.X - 1 || focusMapLocation.X == mapLocation.X + 1);
            //    bool isAdjacentVertically = (focusMapLocation.Y == mapLocation.Y - 1 || focusMapLocation.Y == mapLocation.Y + 1);
            //    bool isAdjacent = isAdjacenttHorizontally && isAdjacentVertically;

            //    if (isAdjacent) {
            //        var borderScreenBounds = _host.ConvertWorldToControl(focusedEditor.WorldBounds);


            //        // Expand to include bounds.
            //        borderScreenBounds.Offset(-12, -12);
            //        borderScreenBounds.Width += 24;
            //        borderScreenBounds.Height += 46;

            //        Rectangle controlClipRect = _host.ConvertWorldToControl(worldClipRect);

            //        var initialState = g.Save();

            //        g.SetClip(controlClipRect, System.Drawing.Drawing2D.CombineMode.Intersect);

            //        Image border = Resources.ScreenFrame;
            //        Rectangle sourceRect = new Rectangle(0, 0, border.Width, border.Height);
            //        Rectangle destRect = sourceRect;
            //        destRect.Location = borderScreenBounds.Location;

            //        g.DrawImage(border, destRect, sourceRect, GraphicsUnit.Pixel);

            //        g.Restore(initialState);

            //    }
            //}
        }

        #region Public properties
        public LevelIndex LevelIndex {
            get {
                if (isExtraneuos) return LevelIndex.None;
                return _host.GetLevel(mapLocation.X, mapLocation.Y); 
            }
        }

        public Level Level { get { return LevelData; } }

        RomViewport _host;
        internal RomViewport EditorHost {
            get { return _host; }
            set {
                if (_host != null) _host.WorldViewChanged -= new EventHandler<EventArgs<Point>>(on_HostViewportChanged);
                _host = value;
                if (_host != null) _host.WorldViewChanged += new EventHandler<EventArgs<Point>>(on_HostViewportChanged);
            }
        }

        public Rectangle WorldBounds {
            get { return new Rectangle(CellSize.Width * mapLocation.X, CellSize.Height * mapLocation.Y, CellSize.Width, CellSize.Height); }
        }

        /// <summary>The index of the screen to display.</summary>
        public int ScreenIndex {
            get { return _host.Rom.GetScreenIndex(mapLocation.X, mapLocation.Y); }
        }

        
        bool ScreenLoaded { get { return ScreenIndex != 0xFF; } }

        /// <summary>
        /// Gets or sets the map location that the current screen represents. This is
        /// used to reference global data that might be relevant to this screen.
        /// </summary>
        public Point MapLocation { get { return mapLocation; }
            set { mapLocation = value;
            isExtraneuos = (value.X < 0 || value.Y < 0 || value.X > 31 || value.Y > 31);
            UpdateDisplayOffset();
            }
        }


        /// <summary>
        /// Gets/sets which ROM image to load data from
        /// </summary>
        public MetroidRom Rom {
            get { return _host.Rom; }
        }

        /// <summary>Gets/sets the highlighting effect used for the selected object.</summary>
        public HighlightEffect SelectionHighlight { get { return selectionHighlight; } set { selectionHighlight = value; } }

        /// <summary>Gets the data for the screen being displayed.</summary>
        public Screen Screen { get { return ScreenData; } }

        #endregion

        #region Hit testing
        /// <summary>
        /// Gets the enemy at the specified location.
        /// </summary>
        /// <param name="x">xTile coordinate.</param>
        /// <param name="y">yTile coordinate.</param>
        /// <returns>The enemy at the specified location, or 
        /// EnemyInstance.Nothing if no enemy is found.</returns>
        public EnemyInstance EnemyAtPixel(int x, int y) {
            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;
            x = x > 255 ? 255 : x;
            y = y > 239 ? 239 : y;

            return EnemyAtTile(x / 8, y / 8);
        }

        private EnemyInstance GetEnemyAt(int x, int y) {
            for (int i = 0; i < ScreenData.Enemies.Count; i++) {
                EnemyInstance enemy = ScreenData.Enemies[i];

                // Errors can occur if there are not enough enemies defined
                if (SpriteData.Length < enemy.EnemyType) continue;

                SpriteDefinition enemySprite = SpriteData[enemy.EnemyType];

                Rectangle enemyBounds = enemySprite.Measure();
                enemyBounds.X += enemy.X * 16;
                enemyBounds.Y += enemy.Y * 16;

                if (enemyBounds.Contains(x, y)) return enemy;
            }

            return null;
        }

        private int GetEnemyIndexAt(int x, int y) {
            if (ScreenData == null) return -1;
            if (SpriteData == null) return -1;

            for (int i = 0; i < ScreenData.Enemies.Count; i++) {
                EnemyInstance enemy = ScreenData.Enemies[i];

                // Errors can occur if there are not enough enemies defined
                if (SpriteData.Length < enemy.EnemyType) continue;

                SpriteDefinition enemySprite = SpriteData[enemy.EnemyType];

                Rectangle enemyBounds = enemySprite.Measure();
                enemyBounds.X += enemy.X * 16;
                enemyBounds.Y += enemy.Y * 16;

                if (enemyBounds.Contains(x, y)) return i;
            }

            return -1;
        }

        private EnemyInstance EnemyAtTile(int x, int y) {
            for (int i = 0; i < ScreenData.Enemies.Count; i++) {
                EnemyInstance enemy = ScreenData.Enemies[i];

                if (enemy.X < x || enemy.Y < y) continue;
                if (SpriteData.Length < enemy.EnemyType) continue;

                SpriteDefinition enemySprite = SpriteData[enemy.EnemyType];
                if ((enemy.X - x + 1) > enemySprite.Width ||
                    (enemy.Y - y + 1) > enemySprite.Height)
                    continue;

                return enemy;
            }

            return null;
        }

        /// <summary>
        /// Gets the enemy found in the specified tile.
        /// </summary>
        /// <param name="x">xTile coordiante.</param>
        /// <param name="y">yTile coordinate.</param>
        /// <returns></returns>
        public EnemyInstance EnemyAt(int x, int y) {
            return EnemyAtTile(2 * x, 2 * y);
        }

        /// <summary>
        /// Gets which object is at a specified pixel coorinate.
        /// </summary>
        /// <param name="x">xTile-coordinate.</param>
        /// <param name="y">yTile-coordinate.</param>
        /// <returns>The object at the specified coordinates.</returns>
        public StructInstance ObjectAtPixel(int x, int y) {
            x = x < 0 ? 0 : x;
            y = y < 0 ? 0 : y;
            x = x > 255 ? 255 : x;
            y = y > 239 ? 239 : y;
            return ObjectAt(x / 16, y / 16);
        }
        /// <summary>
        /// Gets which object is at a specified tile coordinate
        /// </summary>
        /// <param name="destTileX">The x-coordinate.</param>
        /// <param name="destTileY">The y-coordinate</param>
        /// <returns>The object at a specified coordinate.</returns>
        public StructInstance ObjectAt(int tileX, int tileY) {
            if (ScreenData == null || LevelData == null) return null;

            for (int i = ScreenData.Structs.Count - 1; i >= -1; i--) {
                if (i == -1) { // If object not found
                    return null;
                }

                StructInstance o = ScreenData.Structs[i];
                if (tileX < o.X || tileY < o.Y) continue;

                if (o.ObjectType < LevelData.StructCount) {
                    var s = LevelData.GetStruct(o.ObjectType);
                    if (s.Data[tileX - o.X, tileY - o.Y] == Struct.EmptyTile) continue;
                    return ScreenData.Structs[i];
                } else {
                    return null;
                }
            }
            return ScreenData.Structs[0];
        }

        private void OnSelectedObjectChanged() {
            if (SelectedObjectChanged != null)
                SelectedObjectChanged(this, new EventArgs());

            UpdateSelectionStatus();
        }
        #endregion

        #region Selection Management
        /// <summary>
        /// Gets the count of the selected object
        /// </summary>
        /// <returns>The count of an object</returns>
        public int SelectedObjectIndex() {
            int i = 0;
            while (i < ScreenData.Structs.Count && !(SelectedItem.Equals(ScreenData.Structs[i]))) {
                i++;
            }

            if (!(SelectedItem.Equals(ScreenData.Structs[i]))) throw new Exception("Object not found in this screen.");

            return i;
        }

        /// <summary>
        /// Renders and invalidates this screen editor.
        /// </summary>
        public void Redraw() {
            RenderScreen();
            Invalidate();
        }

        public void Invalidate() {
            _host.InvalidateWorld(WorldBounds);
        }

        private bool ShouldSerializeSelectedItem() { return false; }
        /// <summary>Gets which gameItem has been selected within this control.</summary>
        [Browsable(false)]
        public ObjectInstance SelectedItem {
            get {
                return _SelectedItem;
            }
            set {
                _SelectedItem = value;
            }
        }


        public bool HasFocus {
            get { return _host.FocusedEditor == this; }
        }

        public void GetFocus() {
            Redraw();
            //if (!CanEdit) 
            if(!skipSelectiedObjectChanged)
                OnSelectedObjectChanged();
            skipSelectiedObjectChanged = false;
        }
        public void LoseFocus() {
            _SelectedItem = null;
            skipSelectiedObjectChanged = false;
            Redraw();
        }

        /// <summary>
        /// Updates controls to properly reflect the state of the selection.
        /// </summary>
        private void UpdateSelectionStatus() {
            ////if (SelectedItem.IsNothing || LevelData == null) {
            ////    EnemyMode = false;
            ////} else {
            ////    EnemyMode = SelectedItem.InstanceType == ObjectInstanceType.Enemy;
            ////}
        }

        /// <summary>Raised when an object is selected.</summary>
        public event EventHandler SelectedObjectChanged;

        /// <summary>
        /// Gets or sets the count which specifies which enemy to highlight
        /// on the screen. This is for display purposes only.
        /// </summary>
        public int SelectedEnemyIndex {
            get { return screenRenderer.SelectedEnemy; }
            set { screenRenderer.SelectedEnemy = value; }
        }

        /// <summary>
        /// Deselects the selected gameItem.
        /// </summary>
        public void Deselect() {
            _SelectedItem = null;
        }

        #endregion

        #region Mouse Editing
        bool DraggingObject = false;
        int lasttilex, lasttiley;


        /// <summary>Occurs when the selected object is dragged.</summary>
        public event EventHandler SelectionDragged;
        /// <summary>Occurs when the selected object is dragged to inform the event handler to modify the selected object.</summary>
        public event EventHandler<EventArgs<Point>> UserDraggedSelection;
        /// <summary>
        /// Raises the UserDraggedSelection event.
        /// </summary>
        protected void OnUserDraggedSelection(Point p) { if (UserDraggedSelection != null)UserDraggedSelection(this, new EventArgs<Point>(p)); }
        /// <summary>
        /// Raises the SelectionDragged event.
        /// </summary>
        protected void OnSelectionDragged() { if (SelectionDragged != null)SelectionDragged(this, new EventArgs()); }

        #endregion

        #region Public UI Members
        /// <summary>
        /// Selects the struct after the currently selected struct
        /// </summary>
        public void SelectNext() {
            var s = _SelectedItem as StructInstance;
            var e = _SelectedItem as EnemyInstance;

            if (s != null) {
                int structindex = GetObjectIndex(s) + 1;
                if (structindex == ScreenData.Structs.Count) structindex = 0;
                _SelectedItem = ScreenData.Structs[structindex];
                OnSelectedObjectChanged();
            } else if (e != null) {
                int enemyIndex = SelectedEnemyIndex + 1;
                if (enemyIndex == ScreenData.Enemies.Count) enemyIndex = 0;
                _SelectedItem = ScreenData.Enemies[enemyIndex];
                SelectedEnemyIndex = enemyIndex;

                OnSelectedObjectChanged();
            } else {
                if (ScreenData.Structs.Count > 0) {
                    _SelectedItem = ScreenData.Structs[0];
                    OnSelectedObjectChanged();
                } else {
                    _SelectedItem = null;
                }
            }
            //SeekSelectedObjectType(1);
        }

        /// <summary>Selects the struct before the currently selected struct.</summary>
        public void SelectPrevious() {
            var s = _SelectedItem as StructInstance;
            var e = _SelectedItem as EnemyInstance;
            var d = _SelectedItem as DoorInstance;

            if (s != null) {
                int newIndex = ScreenData.Structs.IndexOf(s) - 1;
                if (newIndex < 0) newIndex = ScreenData.Structs.Count - 1;
                _SelectedItem = ScreenData.Structs[newIndex];

                OnSelectedObjectChanged();
            } else if (d != null) {
                int newIndex = ScreenData.Enemies.IndexOf(e) - 1;
                if (newIndex < 0) newIndex = ScreenData.Enemies.Count - 1;
                _SelectedItem = ScreenData.Enemies[newIndex];

                OnSelectedObjectChanged();

            } else if (e != null) {
                int enemyIndex = SelectedEnemyIndex - 1;
                if (enemyIndex == -1) enemyIndex += ScreenData.Enemies.Count;
                _SelectedItem = ScreenData.Enemies[enemyIndex];
                SelectedEnemyIndex = enemyIndex;

                OnSelectedObjectChanged();
            } else {
                throw new InvalidOperationException("Operation not valid on current selection.");
            }
        }
        /// <summary>
        /// Toggles selection between enemies and objects
        /// </summary>
        public void ToggleSelectionType() {
            if (SelectedItem is EnemyInstance) {
                if (ScreenData.Structs.Count > 0) {
                    SelectedItem = ScreenData.Structs[0];
                    SelectedEnemyIndex = -1;
                }
            } else {
                if (ScreenData.Enemies.Count > 0) {
                    SelectedItem = ScreenData.Enemies[0];
                    SelectedEnemyIndex = 0;
                }
            }

            OnSelectedObjectChanged();
        }

        /// <summary>
        /// Raises the UserDraggedSelection event to indicate that an object
        /// should be moved.
        /// </summary>
        /// <param name="distX">The distance to move the object.</param>
        /// <param name="distY">The distance to move the object.</param>
        /// <remarks>If the object is moved out of bounds, it will automatically
        /// be moved to the nearest border.</remarks>
        public void RaiseMoveObject(int distX, int distY) {
            int newX_unsafe, newY_unsafe;

            if (SelectedItem is StructInstance) {
                newX_unsafe = ((StructInstance)_SelectedItem).X + distX;
                newY_unsafe = ((StructInstance)_SelectedItem).Y + distY;
            } else if (SelectedItem is EnemyInstance) {
                newX_unsafe = ((EnemyInstance)_SelectedItem).X + distX;
                newY_unsafe = ((EnemyInstance)_SelectedItem).Y + distY;
            } else if(SelectedItem is ItemInstance) {
                var pos = ((ItemInstance)SelectedItem).Data as IItemScreenPosition;
                if (pos == null) return;

                newX_unsafe = pos.ScreenPosition.X + distX;
                newY_unsafe = pos.ScreenPosition.Y + distY;
            }else{
                return;
            }

            // xTile and yTile coordinates must be between 0 and 0x0F
            int newX = newX_unsafe.Clamp(0x00, 0x0F);
            int newY = newY_unsafe.Clamp(0x00, 0x0F);

            OnUserDraggedSelection(new Point(newX, newY));
        }
        #endregion

        #region Rendering

        List<ItemInstance> loadedItems = new List<ItemInstance>();

        /////// <summary>Finds any items at this map location and stores the appropriate sprites in the screen renderer, and sets 
        /////// them it them to the loaded powerups.</summary>
        ////private void LoadItems() {
        ////    loadedItems.Clear();

        ////    ItemTable table = Rom.GetLevel(LevelIndex).ItemTable_DEPRECATED;
        ////    foreach (ItemRowEntry e in table) {
        ////        if (e.MapY == mapLocation.Y) {
        ////            ItemSeeker seeker = e.Seek();
        ////            bool moreScreens = true;
        ////            while (moreScreens) {
        ////                if (seeker.MapX == mapLocation.X) {
        ////                    bool moreItemsPresent = true;
        ////                    while (moreItemsPresent) {
        ////                        ItemInstance item = new ItemInstance(seeker, e.MapY);

        ////                        // Add to renderer
        ////                        int spriteX, spriteY, spritePal;
        ////                        SpriteDefinition sprite = item.GetSprite(out spriteX, out spriteY, out spritePal);
        ////                        if (sprite != null)
        ////                            screenRenderer.AddSprite(sprite, spriteX, spriteY, (byte)spritePal);

        ////                        loadedItems.Add(item);

        ////                        moreItemsPresent = seeker.MoreItemsPresent;
        ////                        if (moreItemsPresent)
        ////                            seeker.NextItem();
        ////                    }
        ////                }
        ////                if (seeker.MoreScreensPresent) {
        ////                    seeker.NextScreen();
        ////                } else {
        ////                    moreScreens = false;
        ////                }
        ////            }
        ////        }
        ////    }
        ////}

        public ItemScreenData ScreenItems { get; private set; }
        /// <summary>Finds any items at this map location and stores the appropriate sprites in the screen renderer, and sets 
        /// them it them to the loaded powerups.</summary>
        private void LoadItems() {
            loadedItems.Clear();
            ScreenItems = null;

            var levelItems = Rom.Levels[LevelIndex].Items;
            foreach (var screen in levelItems) {
                if (screen.MapX == MapLocation.X && screen.MapY == MapLocation.Y) {
                    ScreenItems = screen;

                    foreach (var item in screen.Items) {
                        ItemInstance itemInst = new ItemInstance(screen, item);

                        // Add to renderer
                        int spriteX, spriteY, spritePal;
                        SpriteDefinition sprite = itemInst.GetSprite(out spriteX, out spriteY, out spritePal);
                        if (sprite != null)
                            screenRenderer.AddSprite(sprite, spriteX, spriteY, (byte)spritePal);

                        loadedItems.Add(itemInst);
                    }
                }
            }
        }





        /// <summary>Applies highlight effect to highlight palettes (colors 0x10-0x1F and 0x30-0x3F, which should mirror 0x00-0x0F and 0x20-0x2F prior to calling this function).</summary>
        private void ApplyHighlightFilter(ColorPalette p) {
            ApplyHighlightFilterToRange(p, 16, 31);
            ApplyHighlightFilterToRange(p, 48, 63);
        }
        /// <summary>Applies highlight effect to the specified range within a paletteIndex.</summary>
        private void ApplyHighlightFilterToRange(ColorPalette p, int start, int end) {
            int clr;
            for (int i = start; i <= end; i++) {
                switch (selectionHighlight) {
                    case HighlightEffect.Invert:
                        clr = p.Entries[i].ToArgb();
                        p.Entries[i] = Color.FromArgb((~clr & 0xFFFFFF) | -16777216);
                        break;
                    case HighlightEffect.InvertBack:
                        if (i % 4 == 0) {
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
                        if (i % 4 == 0) {
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



        public enum EditorColor
        {
            PhysDoorBubble = 0xF9,
            PhysSolid = 0xFA,
            PhysAir = 0xFB,
            PhysBreakable = 0xFC,
            PhysDoor = 0xFD,
            PhysAltDoor = 0xFE,
        }
        static Color[] EditorColors = {
              Color.Turquoise,
              Color.White,
              Color.Black,
              Color.Red,
              Color.LimeGreen,
              Color.Yellow,
                                      };

        /// <summary>Renders the game screen, but does not draw it on the host control.</summary>
        public void RenderScreen() {
            RenderScreen(_host.ShowPhysics);
        }

        /// <summary>Renders the game screen, but does not draw it on the host control.</summary>
        /// <param name="showPhysics">If true, the showPhysics of the screen will be shown.</param>
        public void RenderScreen(bool showPhysics) {
            if (isExtraneuos) return;

            LoadData();
            if (Rom == null | LevelData == null) {
                // Clear image
                screenRenderer.Clear();
                if (Rom != null) {
                    screenRenderer.Level = Rom.Brinstar;
                    screenRenderer.Render(screenBlitter, ScreenBitmap);

                }

                // Clear palette to black (this prevents problems 
                // related to empty screen editors that previously had a screen
                // loadeded with a non-black bg
                ColorPalette p = ScreenBitmap.Palette;
                for (int i = 0; i < 64; i++) 
                    p.Entries[i] = Color.Black;
                ScreenBitmap.Palette = p;

                // Update
                UpdateSelectionStatus();

                return;
            }

            if(ScreenIndex != 0xFF)
                screenRenderer.DefaultPalette = ScreenData.ColorAttributeTable;
            var selection = this.HasFocus ? _SelectedItem : null;
            screenRenderer.Clear();
            screenRenderer.Level = this.LevelData;
            screenRenderer.DrawScreen(ScreenData, selection as StructInstance);
            screenRenderer.SelectedItem = _SelectedItem as ItemInstance;

            if (ScreenLoaded && ScreenData.HasBridge) {
                screenRenderer.AddSprite(ItemSprites.AccessBridgeStatues, 12, 4, 9);
            }

            // Update enemy selection rectangle
            if (_SelectedItem is EnemyInstance) {
                screenRenderer.SelectedEnemy = ScreenData.GetIndex((EnemyInstance)_SelectedItem);
                screenRenderer.SelectedDoor = -1;
            } else if(_SelectedItem is DoorInstance){
                screenRenderer.SelectedEnemy = -1;
                screenRenderer.SelectedDoor = ScreenData.GetIndex((DoorInstance)_SelectedItem);
            } else {
                screenRenderer.SelectedEnemy = -1;
                screenRenderer.SelectedDoor = -1;
            }
            screenRenderer.Level = Rom.GetLevel(LevelIndex);

            // Display gameItem if one is found for this screen
            LoadItems();

            
            if(ScreenLoaded)
                screenRenderer.Render(screenBlitter, ScreenBitmap, ScreenData.Enemies, ScreenData.Doors, showPhysics);

            ColorPalette bitmapPalette = ScreenBitmap.Palette;
            NesPalette bgPalette = LevelData.BgPalette;
            NesPalette spritePalette = LevelData.SpritePalette;
            if (_host.UseAltPalette && LevelData.Format.SupportsAltBgPalette)
                bgPalette = LevelData.BgAltPalette;
            if (_host.UseAltPalette && LevelData.Format.SupportsAltSpritePalette)
                spritePalette = LevelData.SpriteAltPalette;

            // Load palettes twice
            bgPalette.ApplyTable(bitmapPalette.Entries);
            bgPalette.ApplyTable(bitmapPalette.Entries, 16);
            spritePalette.ApplyTable(bitmapPalette.Entries, 32);
            spritePalette.ApplyTable(bitmapPalette.Entries, 48);
            // Invert second paletteIndex for selections
            ApplyHighlightFilter(bitmapPalette);
            //bitmapPalette.Entries[NesPalette.HighlightEntry] = SystemColors.Highlight;

            ApplyEditorColors(bitmapPalette);

            ScreenBitmap.Palette = bitmapPalette;


            UpdateSelectionStatus();
        }

        private void ApplyEditorColors(ColorPalette bitmapPalette) {
            int startOffset = 0xFF - EditorColors.Length;
            for (int i = 0; i < EditorColors.Length; i++) {
                bitmapPalette.Entries[i + startOffset] = EditorColors[i];
            }
        }
        /////// <summary>Renders the game screen, but does not draw it on the host control.</summary>
        /////// <param name="showPhysics">If true, the showPhysics of the screen will be shown.</param>
        ////public void BeginRenderScreen(bool physics, bool invalidate) {
        ////    if (isExtraneuos) return;

        ////    LoadData();
        ////    if (Rom == null | LevelData == null) {
        ////        // Clear image
        ////        screenRenderer.Clear();
        ////        if (Rom != null) {
        ////            screenRenderer.Level = Rom.Brinstar;
        ////            screenRenderer.Render(screenBlitter, ScreenBitmap);

        ////        }

        ////        // Update
        ////        UpdateSelectionStatus();

        ////        return;
        ////    }

        ////    screenRenderer.DefaultPalette = Rom.GetLevel(LevelIndex).Screens[ScreenIndex].ColorAttributeTable;
        ////    screenRenderer.Clear();
        ////    screenRenderer.Level = this.LevelData;
        ////    screenRenderer.DrawScreen(ScreenData, _SelectedItem.Struct);

        ////    // Update enemy selection rectangle
        ////    if (_SelectedItem.InstanceType == ObjectInstanceType.Enemy) {
        ////        screenRenderer.SelectedEnemy = ScreenData.GetIndex(_SelectedItem.Enemy);
        ////    } else {
        ////        screenRenderer.SelectedEnemy = -1;
        ////    }
        ////    screenRenderer.Level = Rom.GetLevel(LevelIndex);

        ////    // Display gameItem if one is found for this screen
        ////    LoadItems();


        ////    //ColorPalette bitmapPalette = ScreenBitmap.Palette;
        ////    //NesPalette bgPalette = LevelData.BgPalette;
        ////    //NesPalette spritePalette = LevelData.SpritePalette;
        ////    //if (_host.UseAltPalette && (LevelData.LevelIdentifier == LevelIndex.Brinstar || LevelData.LevelIdentifier == LevelIndex.Norfair))
        ////    //    bgPalette = LevelData.BgAltPalette;

        ////    //// Load palettes twice
        ////    //bgPalette.ApplyTable(bitmapPalette.Entries);
        ////    //bgPalette.ApplyTable(bitmapPalette.Entries, 16);
        ////    //spritePalette.ApplyTable(bitmapPalette.Entries, 32);
        ////    //spritePalette.ApplyTable(bitmapPalette.Entries, 48);
        ////    //// Invert second paletteIndex for selections
        ////    //ApplyHighlightFilter(bitmapPalette);
        ////    //ScreenBitmap.Palette = bitmapPalette;

        ////    //bitmapPalette.Entries[NesPalette.HighlightEntry] = SystemColors.Highlight;

        ////    SetPendingRender(
        ////        screenRenderer.BeginRender(screenBlitter, ScreenBitmap, ScreenData.Enemies, ScreenData.Doors, physics, invalidate)
        ////    );


        ////    UpdateSelectionStatus();
        ////}

        #endregion


        /// <summary>Gets data (Level, Screen) needed to render the screen</summary>
        private void LoadData() {
            if (Rom == null || LevelIndex == LevelIndex.None) {
                LevelData = null;
                SpriteData = null;
            } else {
                LevelData = Rom.GetLevel(LevelIndex);
                SpriteData = LevelSprites.GetSprites(LevelIndex);
            }

            if (LevelData != null) {
                if (ScreenIndex >= LevelData.Screens.Count)
                    ScreenData = LevelData.Screens[0];
                else
                    ScreenData = LevelData.Screens[ScreenIndex];
                //MakeDefaultSelection();
            }
        }


        /// <summary>
        /// Gets the count of a specified object. This object must be contained in the
        /// screen being displayed.
        /// </summary>
        /// <param name="obj">The object to obtain the count of</param>
        /// <returns>The count of an object</returns>
        public int GetObjectIndex(StructInstance obj) {
            int i = 0;
            while (i < ScreenData.Structs.Count && !(obj.Equals(ScreenData.Structs[i]))) {
                i++;
            }

            if (i >= ScreenData.Structs.Count) throw new Exception("Object not found in this screen.");

            return i;
        }

        /// <summary>
        /// Gets the count of the specified door, enemy, or object.
        /// </summary>
        /// <param name="gameItem">Struct to get count of.</param>
        /// <returns>An integer count.</returns>
        public int GetItemIndex(ObjectInstance item) { 
            int i = 0;
            if (item is StructInstance) {
                ////StructInstance obj = item.Struct;
                ////if (ScreenData.Objects.Count == 0 || obj.Offset < ScreenData.Objects[0].Offset || obj.Offset > ScreenData.Objects[ScreenData.Objects.Length - 1].Offset)
                ////    return -1;

                ////for (i = 0; i < ScreenData.Objects.Length; i++) {
                ////    if (obj.Equals(ScreenData.Objects[i]))
                ////        return i;
                ////}

                ////return -1;
                return ScreenData.Structs.IndexOf((StructInstance)item);
            } else if (item is EnemyInstance) {
                ////EnemyInstance enemy = item.Enemy;
                ////if (ScreenData.Enemies.Length == 0 || enemy.Offset < ScreenData.Enemies[0].Offset || enemy.Offset > ScreenData.Enemies[ScreenData.Enemies.Length - 1].Offset)
                ////    return -1;

                ////for (i = 0; i < ScreenData.Objects.Length; i++) {
                ////    if (enemy.Equals(ScreenData.Enemies[i]))
                ////        return i;
                ////}

                ////return -1;
                return ScreenData.Enemies.IndexOf((EnemyInstance)item);
            } else if (item is DoorInstance) {
                ////DoorInstance door = item.Door;

                ////for (i = 0; i < ScreenData.Doors.Length; i++) {
                ////    if (door.Equals(ScreenData.Doors[i]))
                ////        return i;
                ////}

                ////return -1;
                return ScreenData.Doors.IndexOf((DoorInstance)item);
            } else if (item is ItemInstance) {
                //return ((ItemInstance)item).Data.ID.Composite;

                return loadedItems.IndexOf((ItemInstance)item);
            }
            return -1;
        }


        /// <summary>
        /// Refreshes the data for the currently displayed screen and then redraws the screen.
        /// </summary>
        public void RefreshData() {
            RefreshData(true, SelectedItem == null);
        }
        /// <summary>
        /// Refreshes the data for the currently displayed screen.
        /// </summary>
        /// <param name="showScreen">If true, the screen image will be refreshed.</param>
        /// <param name="defaultSelection">If true, a default selection will be made.</param>
        public void RefreshData(bool showScreen, bool defaultSelection) {
            if (Rom == null || LevelIndex == LevelIndex.None) {
                LevelData = null;
                SpriteData = null;
                SelectedItem = null;
            } else {
                LevelData = Rom.GetLevel(LevelIndex);
                SpriteData = LevelSprites.GetSprites(LevelIndex);
                var oldScreenData = ScreenData;

                if (ScreenIndex < LevelData.Screens.Count)
                    ScreenData = LevelData.Screens[ScreenIndex];
                else
                    ScreenData = LevelData.Screens[0];

                // If we are now looking at a different screen, we shouldn't keep the object in the old screen selected
                if (ScreenData != oldScreenData) SelectedItem = null;
                // If the selection is no longer contained in the screen, it shouldn't be selected
                if (!ScreenData.Structs.Contains(SelectedItem as StructInstance) && !ScreenData.Enemies.Contains(SelectedItem as EnemyInstance) && !ScreenData.Doors.Contains(SelectedItem as DoorInstance) &&!(SelectedItem is ItemInstance)) {
                    SelectedItem = null;
                }
            }

            if (LevelData != null) {
                if (ScreenIndex < LevelData.Screens.Count)
                    ScreenData = LevelData.Screens[ScreenIndex];
                else
                    ScreenData = LevelData.Screens[0];

                if (defaultSelection) MakeDefaultSelection();
                if (showScreen) Redraw();
            }
        }

        /// <summary>Makes a default selection, for instance, when a new screen is shown.</summary>
        public void MakeDefaultSelection() {
            if (LevelIndex == LevelIndex.None || !HasFocus) {
                // Nothing should be selected in empty screen
                _SelectedItem = null;
                //OnSelectedObjectChanged();
            } else if ( ScreenData.Structs.Count > 0) {
                if (_SelectedItem != ScreenData.Structs[0]) {

                    _SelectedItem = ScreenData.Structs[0];
                    OnSelectedObjectChanged();
                }
            } else if ( ScreenData.Enemies.Count > 0) {
                _SelectedItem = ScreenData.Enemies[0];
                OnSelectedObjectChanged();
            } else {
                _SelectedItem = null;
                OnSelectedObjectChanged();
            }
        }



        #region IDisposable Members

        public void Dispose() {
            if (_host != null && HasFocus) _host.RequestUnfocus(this);
            borderPen.Dispose();

            this.EditorHost = null;
            this.LevelData = null;
            if(ScreenBitmap != null) this.ScreenBitmap.Dispose();
            ScreenBitmap = null;
            this.screenBlitter = null;
            this.screenRenderer = null;
            this.SpriteData = null;
        }

        #endregion


        /// <summary>
        /// Gets whether this editor currently has editable data loaded.
        /// </summary>
        public bool CanEdit {
            get {
                return LevelIndex != LevelIndex.None && ScreenIndex < LevelData.Screens.Count;
            }
        }

        /// <summary>
        /// True if there is no data loaded into the editor.
        /// </summary>
        public bool IsEmpty { get { return LevelIndex == LevelIndex.None; } }

        // HACK: This is set to true if an object is selected
        //    because the main form is informed that a different
        //    editor has focus via SelectedObjectChanged, and if
        //    focus changes due to an object being selected, the
        //    event will be raised twice, unnecessarily.
        bool skipSelectiedObjectChanged;
        internal void OnMouseDown(MouseButtons buttons, int worldX, int worldY) {
            if (!HasFocus) _host.RequestFocus(this);


            if (LevelIndex == LevelIndex.None) return;
            // HACK:
            skipSelectiedObjectChanged = false;

            int x = worldX - WorldBounds.X;
            int y = worldY - WorldBounds.Y;

            // Figure out what was clicked
            int clickedEnemyIndex = GetEnemyIndexAt(x, y);
            EnemyInstance clickedEnemy = null;
            if (clickedEnemyIndex != -1) clickedEnemy = ScreenData.Enemies[clickedEnemyIndex]; //GetEnemyAt(otherRow.xTile, otherRow.yTile);
            StructInstance clickedObject = ObjectAtPixel(x, y);
            ItemInstance clickedItem;
            bool ItemWasClicked = TryFindItemAtPixel(x, y, out clickedItem);
            DoorInstance clickedDoor = DoorAtPixel(x, y);


            if (buttons != MouseButtons.Middle) {
                if (clickedEnemy != null) {
                    _SelectedItem = clickedEnemy;

                    OnSelectedObjectChanged();
                    // HACK:
                    skipSelectiedObjectChanged = true;
                    Redraw();
                }else if(clickedDoor != null){
                    _SelectedItem = clickedDoor;

                    OnSelectedObjectChanged();
                    // HACK:
                    skipSelectiedObjectChanged = true;

                    Redraw();
                } else if (ItemWasClicked) {
                    _SelectedItem = clickedItem;

                    OnSelectedObjectChanged();
                    // HACK:
                    skipSelectiedObjectChanged = true;

                    Redraw();
                } else if (clickedObject != null) {
                    // Select clicked object, if it isnt already selected
                    if (!clickedObject.Equals(_SelectedItem)) {
                        _SelectedItem = clickedObject;
                        screenRenderer.SelectedEnemy = -1;
                        OnSelectedObjectChanged();
                        // HACK:
                        skipSelectiedObjectChanged = true;
                        Redraw();
                    }
                }
            }
            if (buttons == MouseButtons.Left) {
                // Begin dragging
                lasttilex = x / 16;
                lasttiley = y / 16;

                if ((ItemWasClicked && clickedItem.HasScreenLocation) || clickedEnemy != null || clickedObject != null)
                    DraggingObject = true;
            } else if (buttons == MouseButtons.Right) {
                if (SelectedItem != null) {
                    bool somethingWasSelected = (clickedEnemy != null || clickedObject != null);
                    bool canDelete = (SelectedItem is StructInstance || SelectedItem is EnemyInstance);
                    if (somethingWasSelected && canDelete) {
                        Program.MainForm.PerformAction(Program.Actions.RemoveSelection());
                    }
                }
            }


        }

        private DoorInstance DoorAtPixel(int x, int y) {
            foreach (DoorInstance door in ScreenData.Doors) {
                if (door.Bounds.Contains(x, y)) return door;
            }

            return null;
        }
        private bool TryFindItemAtPixel(int x, int y, out ItemInstance result) {
            result = null;

            foreach (ItemInstance item in loadedItems) {
                Rectangle r = item.GetCollisionRect();
                if (r.Contains(x, y)) {
                    result = item;
                    return true;
                }
            }

            return false;
        }

        internal void OnMouseMove(object buttons, int worldX, int worldY) {
            int x = worldX - WorldBounds.X;
            int y = worldY - WorldBounds.Y;

            if (DraggingObject) {
                int tilex = x / 16;
                int tiley = y / 16;
                int dx = tilex - lasttilex;
                int dy = tiley - lasttiley;
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
                if (dx != 0 || dy != 0) {
                    RaiseMoveObject(dx, dy);
                    OnSelectionDragged();
                }

            }

            ////System.Diagnostics.Debug.Write(new System.Diagnostics.StackTrace().ToString() + Environment.NewLine);
        }

        internal void OnMouseUp(MouseButtons buttons, int worldX, int worldY) {
            int x = worldX - WorldBounds.X;
            int y = worldY - WorldBounds.Y;

            if (buttons == MouseButtons.Left) DraggingObject = false;
        }

        internal void SelectItem(ItemData item) {
            ////_SelectedItem = LevelData.ItemTable_DEPRECATED.GetItemInstance(id);
            for (int i = 0; i < loadedItems.Count; i++) {
                if (loadedItems[i].Data == item) {
                    _SelectedItem = loadedItems[i];
                    break;
                }
            }

            OnSelectedObjectChanged();
            Redraw();
   
        }
    }
}

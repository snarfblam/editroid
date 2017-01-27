using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Editroid.Properties;
using Editroid.UndoRedo;
using Editroid.Actions;

namespace Editroid
{
    /// <summary>
    /// Allows a user to navigate and edit the map of the world.
    /// </summary>
    public partial class MapControl:PictureBox, ILevelMap
    {
        /// <summary>The recommended amount of space to allocate in a map file for map data.</summary>
        public const int MapDataBufferSize = 1040;

        /// <summary>
        /// Instantiates this control.
        /// </summary>
        public MapControl() {
            base.Image = this._MapImage;
            this._gMapImage = Graphics.FromImage(this._MapImage);
            this._gMapImage.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            this.Cursor = Cursors.Cross;

            for(int i = 0; i < 0x20; i++) {
                for(int j = 0; j < 0x20; j++) {
                    this.MapData[i, j] = MapLevel.Blank;
                }
            }

            this.Selection = new MapControlSelection();
            this.Selection.BackColor = Color.FromArgb(64, SystemColors.Highlight); //Color.Transparent;
            //((PictureBox)this.Selection).Image = Resources.Select;
            ((PictureBox)this.Selection).SizeMode = PictureBoxSizeMode.StretchImage;
            this.Selection.Text = null;
            this.Selection.Size = new Size(16, 16);
            this.Selection.Cursor = Cursors.SizeAll;
            base.Controls.Add(this.Selection);

            this.Selection.MouseDown += new MouseEventHandler(Selection_MouseDown);
            this.Selection.MouseMove += new MouseEventHandler(Selection_MouseMove);
            this.Selection.MouseUp += new MouseEventHandler(Selection_MouseUp);
            this.Selection.MouseLeave += new EventHandler(Selection_MouseLeave);
            this.Selection.MouseEnter += new EventHandler(Selection_MouseEnter);

            mapHideTimer.Interval = 500;
            mapHideTimer.Enabled = false;
            mapHideTimer.Tick += new EventHandler(mapHideTimer_Tick);

            itemDisplay.MapControl = this;
            passwordDisplay.Host = this;
            passwordDisplay.Visible = false;
            passwordDisplay.EntrySelected += new EventHandler(passwordDisplay_EntrySelected);
        }

        

        const int mapTileWidth = 16;
        const int mapTileHeight = 16;

        #region Fields
        private bool useScreenImages;
        private MetroidRom rom;

        private Bitmap _MapImage = new Bitmap(Resources.MapBase, new Size(mapTileWidth * 32, mapTileHeight * 32)); //(Bitmap)Resources.MapBase.Clone();
        private Graphics _gMapImage;
        private Bitmap ClassicTiles = Resources.MapTiles;

        private Control Selection;
        public Control SelectionControl { get { return Selection; } }

        private bool Drawing = false;
        private Rectangle bltDest = new Rectangle(0, 0, mapTileWidth, mapTileHeight);
        private Rectangle bltSource = new Rectangle(0, 0, mapTileWidth, mapTileHeight);

        private MapLevel[,] MapData = new MapLevel[0x20, 0x20];
        private MapLevel pen = MapLevel.Blank;

        MapItemDisplay itemDisplay = new MapItemDisplay();
        private PasswordDataDisplay passwordDisplay = new PasswordDataDisplay();
        #endregion

        public Bitmap MapImage { get { return _MapImage; } }
        public MapItemDisplay ItemDisplay { get { return itemDisplay; } }
        public PasswordDataDisplay PasswordDisplay { get { return passwordDisplay; } }

        /// <summary>Gets/sets whether the map will be drawn with actual screenshots rather than the classic map style.</summary>
        public bool UseScreenImages {
            get { return useScreenImages; }
            set {
                useScreenImages = value;
                if (!value) {
                    RedrawClassicMap();
                }
            }
        }

        [Browsable(false)]
        public MetroidRom Rom {
            get { return rom; }
            set {
                rom = value;
                itemDisplay.Rom = value;
                passwordDisplay.Rom = value;
            }
        }

        void passwordDisplay_EntrySelected(object sender, EventArgs e) {
            OnPasswordEntrySelected();
        }



        public int SelectedPasswordDataIndex {
            get { return passwordDisplay.SelectedIndex; }
            set { passwordDisplay.SelectedIndex = value; }
        }

        public void SetViewport(Rectangle worldView) {
            worldView.X = (int)(((float)worldView.X) / (ScreenEditor.CellSize.Width * 32) * Width);
            worldView.Width = (int)(((float)worldView.Width) / (ScreenEditor.CellSize.Width * 32) * Height);
            worldView.Y = (int)(((float)worldView.Y) / (ScreenEditor.CellSize.Height* 32) * Width);
            worldView.Height = (int)(((float)worldView.Height) / (ScreenEditor.CellSize.Height * 32) * Height);

            Selection.Bounds = worldView;

            Update();
        }


        #region Mouse handling
        bool isDraggingSelection;
        int selectionDragX, selectionDragY;

        void Selection_MouseUp(object sender, MouseEventArgs e) {
            ////OnMouseUp(new MouseEventArgs(otherRow.Button, otherRow.Clicks, otherRow.xTile + Selection.Left, otherRow.yTile + Selection.Top, otherRow.Delta));
            if (isDraggingSelection)
                OnEndDragSelection();
            isDraggingSelection = false;

            // Pass mouse events to windowless controls
            OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X + Selection.Left, e.Y + Selection.Top, e.Delta));
        }

        void Selection_MouseMove(object sender, MouseEventArgs e) {
            //OnMouseMove(new MouseEventArgs(otherRow.Button, otherRow.Clicks, otherRow.xTile + Selection.Left, otherRow.yTile + Selection.Top, otherRow.Delta));
            if(isDraggingSelection) {
                int dx = e.X  - selectionDragX;
                int dy = e.Y- selectionDragY;

                if(dx != 0 || dy != 0)
                    SetSelection(Selection.Left + dx, Selection.Top + dy);
            }

            // Pass mouse events to windowless controls
            OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X + Selection.Left, e.Y + Selection.Top, e.Delta));
        }

        bool MouseIsOverItem { get { return Selection.Cursor == Cursors.Arrow; } }
        void Selection_MouseDown(object sender, MouseEventArgs e) {
            ////OnMouseDown(new MouseEventArgs(otherRow.Button, otherRow.Clicks, otherRow.xTile + Selection.Left, otherRow.yTile + Selection.Top, otherRow.Delta));
            if (e.Button == MouseButtons.Left && !MouseIsOverItem) {
                isDraggingSelection = true;
                OnBeginDragSelection();
                selectionDragX = e.X ;
                selectionDragY = e.Y ;
            }

            // Pass mouse events to windowless controls
            OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X + Selection.Left, e.Y + Selection.Top, e.Delta));
        }

        public event EventHandler BeginDragSelection;
        protected virtual void OnBeginDragSelection() {
            if (BeginDragSelection != null)
                BeginDragSelection(this, new EventArgs());
        }
        public event EventHandler EndDragSelection;
        protected virtual void OnEndDragSelection() {
            if (EndDragSelection != null)
                EndDragSelection(this, new EventArgs());
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (!Selection.Bounds.Contains(e.Location))
                SetSelection(e.X - Selection.Width / 2, e.Y - Selection.Height / 2);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            //if(this.Drawing) {
            //    int x = otherRow.xTile / 8;
            //    int y = otherRow.yTile / 8;
            //    x = (x < 0) ? 0 : x;
            //    y = (y < 0) ? 0 : y;
            //    x = (x > 0x1f) ? 0x1f : x;
            //    y = (y > 0x1f) ? 0x1f : y;
            //    if(this.MapData[x, y] != this.pen) {
            //        this.SetImage(x, y, this.pen);
            //        if(this.TileChanged != null) {
            //            this.TileChanged(this, x, y);
            //        }
            //        base.Invalidate();
            //    }
            //}
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            //if(otherRow.Button == MouseButtons.Left) {
            //    this.Drawing = false;
            //}
        }


        #endregion


        #region Map hiding
        Timer mapHideTimer = new Timer();

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);

            CheckForMouseLeave();
        }

        void Selection_MouseLeave(object sender, EventArgs e) {
            CheckForMouseLeave();
        }
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);

            mapHideTimer.Stop();
        }
        void Selection_MouseEnter(object sender, EventArgs e) {
            mapHideTimer.Stop();
        }

        void mapHideTimer_Tick(object sender, EventArgs e) {
            mapHideTimer.Stop();
            Hide();
        }

        private void CheckForMouseLeave() {
            if (lockLevel <= 0) {
                var mousePos = PointToClient(MousePosition);
                if (mousePos.X < 0 || mousePos.Y < 0 || mousePos.X >= Width || mousePos.Y >= Height)
                    mapHideTimer.Start();
            }
        }

        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);

            if (Visible && lockLevel < 1) CheckForMouseLeave(); // If the mouse is not in the map control, we will autohide it
        }
        #endregion



        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);

            // Draw outline
            pe.Graphics.DrawRectangle(Pens.White, new Rectangle(0, 0, Width, Height));
        }

        /// <summary>
        /// Moves the map selection to the specified coordinates.
        /// </summary>
        /// <param name="x">xTile coordinate.</param>
        /// <param name="y">yTile coordinate.</param>
        private void SetSelection(int x, int y) {
            Rectangle newBounds = Selection.Bounds;
            newBounds.X = x;
            newBounds.Y = y;

            if (newBounds.Right < 1) newBounds.X -= newBounds.Right;
            if (newBounds.Bottom < 1) newBounds.Y -= newBounds.Bottom;
            if (newBounds.Left >= Width) newBounds.X = Width - 1;
            if (newBounds.Top >= Height) newBounds.Y = Height - 1;

            //Selection.Bounds = newBounds;
            if (SelectionChanged != null) SelectionChanged(this, newBounds.X, newBounds.Y);
        }



        
        
        
        public LevelIndex GetLevel(int x, int y) {
            MapLevel level = this.MapData[x, y] & ((MapLevel)0xfc);
            if(level == MapLevel.Ridley)
                return LevelIndex.Ridley;
            else if(level == MapLevel.Kraid)
                return LevelIndex.Kraid;
            else if(level == MapLevel.Tourian)
                return LevelIndex.Tourian;
            else if(level == MapLevel.Norfair)
                return LevelIndex.Norfair;
            else if(level == MapLevel.Blank)
                return LevelIndex.None;
            else
                return LevelIndex.Brinstar;
        }
        public void SetLevel(int x, int y, LevelIndex level) {
            switch (level) {
                case LevelIndex.Brinstar:
                    SetMapLocationImage(x, y, MapLevel.Brinstar);
                    break;
                case LevelIndex.Norfair:
                    SetMapLocationImage(x, y, MapLevel.Norfair);
                    break;
                case LevelIndex.Tourian:
                    SetMapLocationImage(x, y, MapLevel.Tourian);
                    break;
                case LevelIndex.Kraid:
                    SetMapLocationImage(x, y, MapLevel.Kraid);
                    break;
                case LevelIndex.Ridley:
                    SetMapLocationImage(x, y, MapLevel.Ridley);
                    break;
                case LevelIndex.None:
                    SetMapLocationImage(x, y, MapLevel.Blank);
                    break;
            }
        }


        /// <summary>
        /// Gets a MapLevel value representing the image to be drawn for a classic map.
        /// </summary>
        private MapLevel GetMapLocationImage(int x, int y) {
            return this.MapData[x, y];
        }
        /// <summary>
        /// Sets the image to be used at a specified map position.
        /// </summary>
        private void SetMapLocationImage(int x, int y, MapLevel image) {
            if (image > MapLevel.Blank) {
                image = MapLevel.Blank;
            }
            if (this.MapData[x, y] != image) {
                this.bltDest.X = x * mapTileWidth;
                this.bltDest.Y = y * mapTileHeight;
                this.bltSource.X = (int)image * 8;
                this.bltSource.Width = this.bltSource.Height = 8;

                this._gMapImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                this._gMapImage.DrawImage(this.ClassicTiles, this.bltDest, this.bltSource, GraphicsUnit.Pixel);
                this.MapData[x, y] = image;
                if (image != MapLevel.Blank && rom != null && rom.GetScreenIndex(x, y) == 0xFF) {
                }

                //if(!loading && 
                //    SelectionX == x && SelectionY == y
                //    && selectedRoomModified != null)

                //    selectedRoomModified(this, new EventArgs());

                if (!isLoadingMapData && RoomSet != null) {
                    RoomSet(this, x, y);
                }
            }
        }

        #region Map loading/saving
        /// <summary>
        /// Indicates whether the control is currently loading data, so that
        /// events that should not occur at this time will be surpressed.
        /// </summary>
        bool isLoadingMapData;

        /// <summary>
        /// Loads a map from a stream.
        /// </summary>
        /// <param name="s">Stream containing map data.</param>
        public bool TryLoadData(Stream s) {
            return TryLoadData(s, true);
        }
        /// <summary>
        /// Loads a map from a stream.
        /// </summary>
        /// <param name="s">Stream containing map data.</param>
        /// <param name="hasHeader"></param>
        /// <returns>T boolean value that indicates success or failure.</returns>
        public bool TryLoadData(Stream s, bool hasHeader) {
            isLoadingMapData = true;
            try {
                if(hasHeader) {
                    for(int i = 0; i < MagicValue.Length; i++) {
                        try {
                            if(s.CanRead == false)
                                return false;

                            if(s.ReadByte() != MagicValue[i])
                                return false;
                        }
                        catch(Exception) {
                            return false;
                        }
                    }

                    Invalidate();
                }
                for(int i = 0; i < 0x20; i++) {
                    for(int j = 0; j < 0x20; j++) {
                        this.SetMapLocationImage(j, i, (MapLevel)((byte)s.ReadByte()));
                    }
                }

                return true;
            }
            finally {
                isLoadingMapData = false;
            }
        }

        /// <summary>
        /// Loads a map from a file.
        /// </summary>
        /// <param name="filename">Name of file containing map data.</param>
        public void LoadData(string filename) {
            using(FileStream s = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                this.TryLoadData(s, true);
            }
            base.Invalidate();
        }

        /// <summary>Binary data marker to identify map data.</summary>
        static byte[] MagicValue = { 0x45, 0x44, 0x49, 0x54, 0x52, 0x4F, 0x49, 0x44 }; // "EDITROID"
        /// <summary>
        /// Saves map to a stream.
        /// </summary>
        /// <param name="s">Stream to write data to.</param>
        public void SaveData(Stream s) {
            s.Write(MagicValue, 0, MagicValue.Length);

            for (int i = 0; i < 0x20; i++) {
                for (int j = 0; j < 0x20; j++) {
                    s.WriteByte((byte)this.MapData[j, i]);
                }
            }
        }


        /// <summary>
        /// Saves map to a file.
        /// </summary>
        /// <param name="filename">Filename.</param>
        public void SaveData(string filename) {
            using (FileStream s = new FileStream(filename, FileMode.OpenOrCreate)) {
                this.SaveData(s);
            }
        }
        #endregion



        #region Map rendering
        ScreenImageGenerator renderer;
        internal void SetRenderer(ScreenImageGenerator r) {
            renderer = r;
            r.LayoutRendered += new EventHandler(OnLayoutRendered);
        }

        void OnLayoutRendered(object sender, EventArgs e) {
            if (!useScreenImages) return;

            Rectangle src = new Rectangle(0, 0, 256, 240);

            LayoutIndex index = renderer.RenderedLayout;
            for (int x = 0; x < 32; x++) {
                for (int y = 0; y < 32; y++) {
                    if (GetLevel(x, y) == index.Level && rom.GetScreenIndex(x, y) == index.ScreenIndex) {
                        Rectangle dest = new Rectangle(x * mapTileWidth, y * mapTileHeight, mapTileWidth, mapTileHeight);
                        _gMapImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                        _gMapImage.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                        _gMapImage.DrawImage(renderer.RenderedImage, dest, src, GraphicsUnit.Pixel);
                        if (PerformingInitialRender) {
                            if ((index.ScreenIndex % 5) == 0 || index.ScreenIndex == rom.GetLevel(index.Level).Screens.Count - 1)
                                   Invalidate();
                        } else {
                            Invalidate();
                        }
                    }
                }
            }
        }

        /// <summary>Setting this property to true causes the control to paint itself less frequently to allow more time for other tasks.</summary>
        public bool PerformingInitialRender { get;set; }

        private void RedrawClassicMap() {
            Rectangle bltDest = new Rectangle(0, 0, mapTileWidth, mapTileHeight);
            Rectangle bltSource = bltDest;

            for (int x = 0; x < 32; x++) {
                for (int y = 0; y < 32; y++) {
                    this.bltDest.X = x * mapTileWidth;
                    this.bltDest.Y = y * mapTileHeight;
                    this.bltSource.X = (int)GetMapLocationImage(x, y) * 8;
                    this.bltSource.Width = this.bltSource.Height = 8;
                    this._gMapImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    this._gMapImage.DrawImage(this.ClassicTiles, this.bltDest, this.bltSource, GraphicsUnit.Pixel);

                }
            }
            Invalidate();
        }

        #endregion

        internal void NotifyActionOccurred(EditroidAction a) {
                itemDisplay.NotifyActionOccurred(a);
                passwordDisplay.NotifyActionOccurred(a);
        }



        protected virtual void OnPasswordEntrySelected() {
            if (PasswordEntrySelected != null)
                PasswordEntrySelected(this, new EventArgs());
        }
        public event EventHandler PasswordEntrySelected;



        private bool ShouldSerializeImage() {
            return false;
        }
        /// <summary>
        /// Hides the Image property.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new Image Image {
            get {
                return base.Image;
            }
            set {
            }
        }







        /// <summary>
        /// Represents an event with coordinate data.
        /// </summary>
        /// <param name="sender">Struct that raises the event.</param>
        /// <param name="xTile">xTile coordinate.</param>
        /// <param name="yTile">yTile coordinate.</param>
        public delegate void CoordEvent(object sender, int X, int Y);

        // Events
        /// <summary>
        /// Occurs when a room is selected.
        /// </summary>
        public event CoordEvent SelectionChanged;
        /// <summary>
        /// Occurs when a room is cleared.
        /// </summary>
        public event CoordEvent RoomCleared;
        /// <summary>
        /// Occurs when a room is changed.
        /// </summary>
        public event CoordEvent RoomSet;
        /// <summary>
        /// Occurs when a map tile is changed.
        /// </summary>
        public event CoordEvent TileChanged;



        /// <summary>
        /// Visually clears any map locations that do not exist in the specified ROM.
        /// </summary>
        internal void FilterEmptyLocations() {
            for(int x = 0; x < 32; x++) {
                for(int y = 0; y < 32; y++) {
                    if(rom.GetScreenIndex(x, y) == 0xFF)
                        SetMapLocationImage(x, y, MapLevel.Blank);
                }
            }
        }




        internal void ReloadItemData() {
            if (itemDisplay != null && itemDisplay.IsLoaded)
                itemDisplay.LoadLevel(itemDisplay.CurrentLevel);
        }


        /// <summary>
        /// If password data is present for specified map location, first such
        /// piece of data is selected. If the password display is not shown or 
        /// the data isn't found, this method will fail silently.
        /// </summary>
        internal void SelectPasswordDat(Point mapLocation) {
            if (passwordDisplay != null)
                passwordDisplay.SelectDatAt(mapLocation);
        }

        int lockLevel = 0;
        /// <summary>
        /// Shows the control, if hidden, and prevents it from being auto-hidden until a corresponding
        /// call to UnlockVisible is made.
        /// </summary>
        internal void LockVisible() {
            lockLevel++;
            if (lockLevel > 0 && !Visible) Show();
        }

        /// <summary>
        /// Releases a "visible lock". When UnlockVisible has been called as many times as LockVisible (or more)
        /// the control will auto-hide.
        /// </summary>
        internal void UnlockVisible() {
            lockLevel--;
            if (lockLevel < 1) CheckForMouseLeave();
        }
    }


	

    /// <summary>
    /// Enumerates values that can be combined to specifiy the image to use
    /// for a screen on a map.
    /// </summary>
    [Flags]
    public enum MapLevel:byte
    {
        /// <summary>Screen is in Brinstar.</summary>
        Brinstar = 0,
        /// <summary>Screen is in Norfair.</summary>
        Norfair = 4,
        /// <summary>Screen is in Tourian.</summary>
        Tourian = 8,
        /// <summary>Screen is in Kraid's hideout.</summary>
        Kraid = 0xC,
        /// <summary>Screen is blank.</summary>
        Blank = 0x14,
        /// <summary>Screen is in Ridley's hideout.</summary>
        Ridley = 0x10,
    }

    public interface ILevelMap
    {
        LevelIndex GetLevel(int x, int y) ;
        void SetLevel(int x, int y, LevelIndex level);
    }

}

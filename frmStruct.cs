using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Editroid.Graphic;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid
{
    internal partial class frmStruct : Form, IUpdatable
    {

        Level[] levels = new Level[5];
        PatternTable[] displayTiles = new PatternTable[5];
        PatternTable[] renderingTiles = new PatternTable[5];
        Bitmap[] combos = new Bitmap[5];

        StructOkIndicator addTileIndicator = new StructOkIndicator();

        public frmStruct() {
            InitializeComponent();
            gCurrentStruct = Graphics.FromImage(bmpCurrentStruct);
            picStruct.Image = bmpCurrentStruct;
            GlobalEventManager.Manager.ObjectSelected += new EventHandler<ScreenObjectEventArgs>(Global_ObjectSelected);
            GlobalEventManager.Manager.ObjectEdited += new EventHandler<ScreenObjectEventArgs>(Global_ObjectSelected);

            addTileIndicator.Container = picStruct;
            addTileIndicator.Click += new EventHandler(addTileIndicator_Click);
            addTileIndicator.MouseLeave += new EventHandler(addTileIndicator_MouseLeave);
            addTileIndicator.Hide();

            updater = new UpdateScoper<frmStruct>(this);
        }

        void addTileIndicator_MouseLeave(object sender, EventArgs e) {
            Perform_ContainsMouse_Test();
        }

        void addTileIndicator_Click(object sender, EventArgs e) {
            // if State is false this is an invalid location for a tile
            if (!addTileIndicator.State)
                return;

            int x = addTileIndicator.Left / 16;
            int y = addTileIndicator.Top / 16;

            //if (y == loadedStructData.Combos.Length)
            //    PerformAction(Actions.AddStructRow(StructIndex, x, y, comboEditor.SelectedCombo)); 
            //else if (x == loadedStructData.Combos[y].Length)
            //    PerformAction(Actions.AppendStructTile(StructIndex, y, comboEditor.SelectedCombo)); 
            //else
            //    PerformAction(Actions.PrependStructTile(StructIndex, y, comboEditor.SelectedCombo));

            PerformAction(Actions.AddStructTile(StructIndex, x, y, (byte)comboEditor.SelectedCombo));
            addTileIndicator.Hide();

        }

        ActionGenerator Actions { get { return Program.Actions; } }
        void PerformAction(Editroid.UndoRedo.EditroidAction a) {
            Program.PerformAction(a);
        }


        private MetroidRom rom;
        public MetroidRom Rom {
            get { return rom; }
            set {
                if (rom == value) return;
                rom = value;
                LoadRomData();
            }
        }

        
        /// <summary>Loads data from a ROM necessary for editing features..</summary>
        private void LoadRomData() {
            levels[0] = rom.Brinstar;
            levels[1] = rom.Norfair;
            levels[2] = rom.Tourian;
            levels[3] = rom.Kraid;
            levels[4] = rom.Ridley;

            for (int i = 0; i < levels.Length; i++) {
                displayTiles[i] = levels[i].CreatePatternBitmap(false);
                renderingTiles[i] = levels[i].CreatePatternBitmap(true);

            }
            LevelIndex = LevelIndex.Brinstar;

            if (!rom.Format.SupportsExtendedComboTable) {
                comboEditor.Height = 64;
            }
        }


        void Global_ObjectSelected(object sender, ScreenObjectEventArgs e) {
            StructInstance structInst = e.Item as StructInstance;

            if (structInst != null) {
                // Load currentLevelIndex and paletteIndex data
                using (BeginUpdate()) {
                    LevelIndex = e.Level;
                    PaletteIndex = structInst.PalData;
                    StructIndex = structInst.ObjectType;
                }
            }
        }







        /// <summary>
        /// Identifies data to be loaded (on assignment to frmStruct property
        /// or, if updating, upon call to frmStruct.EndUpdate().
        /// </summary>
        LoadedData data = new LoadedData();
        /// <summary>
        /// Identifies data that has been loaded.
        /// </summary>
        LoadedData loadedData = new LoadedData();

        private void PerformUpdates() {
            if (IsUpdating) return;

            if (data.Level != loadedData.Level)
                LoadLevel();
            if (data.PaletteIndex != loadedData.PaletteIndex)
                LoadPalette();
            if (data.StructIndex != loadedData.StructIndex)
                LoadStruct();
        }

        /// <summary>
        /// Gets/sets the currently displayed combo's count
        /// </summary>
        /// <remarks>To update multiple properties without unnecessary redraws, 
        /// wrap the operation with a call to BeginUpdate() and EndUpdate().</remarks>
        public int StructIndex {
            get { return data.StructIndex; }
            set {
                data.StructIndex = value;
                PerformUpdates();
            }
        }
        /// <summary>
        /// Gets/sets the currently diplayed palette.
        /// </summary>
        /// <remarks>To update multiple properties without unnecessary redraws, 
        /// wrap the operation with a call to BeginUpdate() and EndUpdate().</remarks>
        public int PaletteIndex { get { return data.PaletteIndex; }
            set {
                data.PaletteIndex = value;
                PerformUpdates();
            }
        }

        private bool useAlternatePalette;
        /// <summary>
        /// Gets/sets whether to use a level's alternate palette.
        /// </summary>
        /// <remarks>To update multiple properties without unnecessary redraws, 
        /// wrap the operation with a call to BeginUpdate() and EndUpdate().</remarks>
        public bool UseAlternatePalette {
            get { return useAlternatePalette; }
            set {
                useAlternatePalette = value;

                loadedData.Level = LevelIndex.None;
                PerformUpdates();
            }
        }
        /// <summary>
        /// Gets/sets the currently loaded level.
        /// </summary>
        /// <remarks>To update multiple properties without unnecessary redraws, 
        /// wrap the operation with a call to BeginUpdate() and EndUpdate().</remarks>
        public LevelIndex LevelIndex {
            get { return data.Level; }
            set {
                data.Level = value;
                PerformUpdates();
            }
        }

        public string NeededBytesCaption {
            get { return NeededBytesLabel.Text; }
            set {
                NeededBytesLabel.Text = value;
            }
        }



        /// <summary>Loads a structure.</summary>
        public void LoadStruct() {
            loadedData.StructIndex = data.StructIndex;


            loadedStructure = rom.GetLevel(loadedData.Level).GetStruct(StructIndex);
            addTileIndicator.StructData = loadedStructure;
            addTileIndicator.FreeSpace = loadedLevel.FreeStructureMemory;
            lblFreeMem.Text = "$" + addTileIndicator.FreeSpace.ToString("x") + " free bytes";

            var comboData = loadedStructure.Data;
            Rectangle source = new Rectangle(0, 0, 16, 16);
            Rectangle dest = source;

            gCurrentStruct.Clear(Color.Transparent);

            for (int y = 0; y < Structure.StructureData.height ; y++) {
                for (int x = 0; x < Structure.StructureData.width; x++) {
                    byte tile = comboData[x,y];
                    if (tile != Struct.EmptyTile) {
                        source.X = 16 * (tile % 16);
                        source.Y = 16 * (tile / 16);
                        dest.X = x * 16;
                        dest.Y = y * 16;

                        //gCurrentStruct.DrawImage(combos[levelIndex], dest, source, GraphicsUnit.Pixel);
                        gCurrentStruct.DrawImage(comboEditor.BackgroundImage, dest, source, GraphicsUnit.Pixel);
                    }
                }
            }
            picStruct.Invalidate();
            addTileIndicator.Hide();

        }

        /// <summary>
        /// Loads a palette and the affected structure.
        /// </summary>
        void LoadPalette() {
            loadedData.PaletteIndex = data.PaletteIndex;

            // Load tiles and combos
            tsTiles.SetPalette(GetLevelPalette(), data.PaletteIndex);
            tsTiles.Invalidate();
            comboEditor.PaletteTable = data.PaletteIndex;
            comboEditor.SetPalette(GetLevelPalette());
            comboEditor.Draw();

            LoadStruct();
        }

        internal void RefreshChr() {
            LoadLevel();
        }
        /// <summary>
        /// Loads a level and its contained palette and structure.
        /// </summary>
        private void LoadLevel() {
            loadedData.Level = data.Level;
            if (loadedData.Level == LevelIndex.None) return;


            if (StructIndex < 0 || StructIndex >= levels[levelIndex].StructCount) StructIndex = 0;

            tsTiles.Image = loadedLevel.Patterns.PatternImage;
            tsTiles.Image = displayTiles[levelIndex].PatternImage;
            tsTiles.SetPalette(GetLevelPalette(), data.PaletteIndex);

            comboEditor.SetPatterns(loadedLevel.Patterns);
            comboEditor.SetCombos(loadedLevel.Combos);
            comboEditor.Level = loadedData.Level;
            comboEditor.Invalidate();

            // LoadPalette will call LoadStruct.
            LoadPalette();
        }


        #region Updating
        UpdateScoper<frmStruct> updater;
        void IUpdatable.OnUpdateStart() {
        }
        void IUpdatable.OnUpdateEnd() {
            PerformUpdates();
        }

        public bool IsUpdating { get { return updater.IsUpdating; } }
        public ActionScope BeginUpdate() {
            return updater.BeginUpdate();
        }
        public void EndUpdate() {
            updater.EndUpdate();
        }
        #endregion


        Bitmap bmpCurrentStruct = new Bitmap(512, 512);
        Graphics gCurrentStruct;

        //int levelIndex = 0;
        int levelIndex { get { return (int)data.Level; } }
        public Level loadedLevel {
            get {
                if (loadedData.Level == LevelIndex.None) return null;
                return rom.Levels[loadedData.Level];
            }
        }


        private Structure loadedStructure;


        /// <summary>
        /// Records all structs back to the ROM data.
        /// </summary>
        public void WriteStructsToRom() {
            //foreach (Level l in levels) {
                Rom.SerializeAllData();
            //}
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            GlobalEventManager.Manager.ObjectSelected -= new EventHandler<ScreenObjectEventArgs>(Global_ObjectSelected);
            GlobalEventManager.Manager.ObjectEdited -= Global_ObjectSelected;
        }

        private void picStruct_MouseDown(object sender, MouseEventArgs e) {
            if (loadedStructure == null) return;

            int tilex = e.X / 16;
            int tiley = e.Y / 16;



            if (e.Button == MouseButtons.Left && GetTileFromCurrentStruct(tilex, tiley) != Struct.EmptyTile) {

                PerformAction(Actions.SetStructTile(StructIndex, tilex, tiley, comboEditor.SelectedCombo));
                GlobalEventManager.Manager.OnStructureChanged(data.Level);
            }
        }

        /// <summary>
        /// Gets a tile index from the currently loaded struct or the EmptyTile value
        /// of the specified position is out of bounds.
        /// </summary>
        byte GetTileFromCurrentStruct(int x, int y) {
            if (x < 0 || y < 0 || x >= Structure.StructureData.width || y >= Structure.StructureData.height)
                return Struct.EmptyTile;

            return loadedStructure.Data[x, y];
        }

        private void cmnStructContext_Opening(object sender, CancelEventArgs e) {
            //If a tile was clicked (not including blanks)
            if (loadedStructure == null) {
                e.Cancel = true;
                return;
            }

            byte tile = GetTileFromCurrentStruct(lastMousePosition.X,lastMousePosition.Y);
            if (tile != Struct.EmptyTile) {
                clickedMousePosititon = lastMousePosition;

                mnuShiftLeft.Enabled = loadedStructure.Data[0, lastMousePosition.Y] == Struct.EmptyTile;
                mnuShiftRight.Enabled = loadedStructure.Data[Structure.StructureData.width - 1, lastMousePosition.Y] == Struct.EmptyTile;

                lblComboData.Text = tile.ToString("x");

                // Determine if tile may be deleted
                btnDelete.Enabled = CanDeleteTile(clickedMousePosititon);
            } else {
                e.Cancel = true;
            }
        }

        private bool CanDeleteTile(Point location) {
            bool TileIsEmpty = GetTileFromCurrentStruct(location.X, location.Y) == Struct.EmptyTile;
            if(TileIsEmpty) return false;

            bool TileToLeftIsEmpty = GetTileFromCurrentStruct(location.X - 1, location.Y) == Struct.EmptyTile;
            bool TileToRightIsEmpty = GetTileFromCurrentStruct(location.X + 1, location.Y) == Struct.EmptyTile;
            bool IsTopRow = location.Y == 0;
            bool IsLastRow = loadedStructure.LastRow == location.Y;

            bool RowHasOneTile = TileToLeftIsEmpty && TileToRightIsEmpty;
            bool OnEndOfRow = TileToLeftIsEmpty || TileToRightIsEmpty;

            if (!OnEndOfRow) return false;

            if (RowHasOneTile) {
                if (IsTopRow) return false;
                return IsLastRow;
            } else {
                return true;
            }
            
        }


        /// <summary>Caches index of tile the mouse is over.</summary>
        Point lastMousePosition;
        /// <summary>Caches index of tile the mouse clicked.</summary>
        Point clickedMousePosititon;

        private void picStruct_MouseMove(object sender, MouseEventArgs e) {
            lastMousePosition = new Point(e.X / 16, e.Y / 16);
        }

        ////#region Row Shifting/Splitting/Joinging and relevant UI events
        ////private void mnuShiftLeft_Click(object sender, EventArgs e) {
        ////    //PerformAction(Actions.ShiftStructRowLeft(StructIndex, clickedMousePosititon.Y));
        ////}



        ////private void mnuShiftRight_Click(object sender, EventArgs e) {
        ////    //PerformAction(Actions.ShiftStructRowRight(StructIndex, clickedMousePosititon.Y));
        ////}


        /////// <summary>Gets the length of a row.</summary>
        /////// <param name="row">T byte array representing a row.</param>
        /////// <param name="includeBlanks">If true, preceding blank tiles will be included.</param>
        ////private int GetRowLength(byte[] row, bool includeBlanks) {
        ////    if (includeBlanks) return row.Length;

        ////    int blanks = 0;
        ////    while (blanks < row.Length && row[blanks] == Struct.EmptyTile)
        ////        blanks++;

        ////    return row.Length - blanks;
        ////}


        ////#endregion






        /// <summary>Gets a currentLevelIndex paletteIndex, taking into consideration the UseAlternatePalette property.</summary>
        private NesPalette GetLevelPalette() {
            LevelIndex level = loadedData.Level;
            if (useAlternatePalette && (level == LevelIndex.Brinstar || level == LevelIndex.Norfair))
                return loadedLevel.BgAltPalette;

            return loadedLevel.BgPalette;
        }

        internal void SetIndex(int p) {
            StructIndex = p;
            LoadStruct();
        }



        private void tsTiles_TileSelected(object sender, int index) {
            comboEditor.CurrentTile = (byte)index;
            
            lblTile.Text = index.ToString("X").PadLeft(2, '0') + " " + rom.GetPhysics((byte)index).ToString();
        }

        private void comboEditor_ComboEdited(object sender, EventArgs e) {
            // Redraw struct incase it contains any combos that were edited
            LoadStruct();
            GlobalEventManager.Manager.OnComboChanged(data.Level, 0);
        }

        private void comboEditor_UserGrabbedCombo(object sender, EventArgs e) {
            tsTiles.SetSeletion(comboEditor.CurrentTile);
        }

        private void lblComboData_Click(object sender, EventArgs e) {
            comboEditor.SelectedCombo = loadedStructure.Data[clickedMousePosititon.X, clickedMousePosititon.Y];
        }

        private void btnDelete_Click(object sender, EventArgs e) {
            //byte[] row = loadedStructData.Combos[clickedMousePosititon.Y];
            //int rowLen = GetRowLength(row, false);
            //int spacing = row.Length - rowLen;

            //if (rowLen == 1) { // Row will be removed
            //    PerformAction(Actions.RemoveStructRow(StructIndex, clickedMousePosititon.Y));
            //} else if (clickedMousePosititon.X == spacing) {
            //    PerformAction(Actions.RemoveLeadingStructTile(StructIndex, clickedMousePosititon.Y));
            //} else if (clickedMousePosititon.X == row.Length - 1) {
            //    PerformAction(Actions.RemoveTrailingStructTile(StructIndex, clickedMousePosititon.Y));
            //}
            PerformAction(Actions.SetStructTile(StructIndex, clickedMousePosititon.X, clickedMousePosititon.Y, Structure.EmptyTile));
        }

        public void picStruct_MouseLeave(object sender, EventArgs e) {
            Perform_ContainsMouse_Test();
        }

        private void Perform_ContainsMouse_Test() {
            Point mouseLocationRelative = picStruct.PointToClient(MousePosition);
            bool containsMouse = mouseLocationRelative.X >= 0 &&
                mouseLocationRelative.Y >= 0 &&
                mouseLocationRelative.X < picStruct.Width &&
                mouseLocationRelative.Y < picStruct.Height;
            if (!containsMouse) {
                addTileIndicator.Visible = false;
                NeededBytesLabel.Text = null;
            }
        }

        static T[] ArrayAppend<T>(T[] array, T value) {
            T[] newValue = new T[array.Length + 1];
            Array.Copy(array, newValue, array.Length);
            newValue[array.Length] = value;
            return newValue;
        }
        static T[] ArrayString<T>(T value, int count) {
            T[] newValue = new T[count];
            for (int i = 0; i < count; i++) {
                newValue[i] = value;
            }
            return newValue;
        }

        internal void NotifyAction(LevelAction a, bool undo) {
            if (a.AffectedLevel != loadedData.Level) return;

            using (BeginUpdate()) {
                Actions.ModifyObject modAction = a as Actions.ModifyObject;
                if (modAction != null && modAction.AffectedLevel == data.Level && modAction.GetItem() is StructInstance) {
                    if (((StructInstance)modAction.GetItem()).ObjectType == StructIndex && modAction.Change == Editroid.Actions.ObjectModification.Palette) {
                        loadedData.PaletteIndex = LoadedData.Invalid;
                    }
                }

                Actions.SetPaletteColor palAction = a as Actions.SetPaletteColor;
                if (palAction != null) {
                    if (palAction.AffectedLevel == data.Level && (palAction.PaletteIndex == data.PaletteIndex || palAction.EntryIndex == 0))
                        loadedData.PaletteIndex = LoadedData.Invalid;
                }
            }


            Editroid.Actions.ModifyCombo comboAction = a as Editroid.Actions.ModifyCombo;
            if (comboAction != null) {
                if (comboAction.Changes.Count == 1)
                    comboEditor.DrawCombo(comboAction.Changes[0].index, true);
                else
                    comboEditor.Draw();
            }

            loadedData.StructIndex = LoadedData.Invalid;
            PerformUpdates();


        }
        class LoadedData
        {
            private LevelIndex level;

            public LevelIndex Level {
                get { return level; }
                set { level = value; }
            }

            public const int Invalid = -1;

            private int pal;

            public int PaletteIndex {
                get { return pal; }
                set { pal = value; }
            }

            private int structure;

            public int StructIndex {
                get { return structure; }
                set { structure = value; }
            }


            public LoadedData Clone() {
                LoadedData result = new LoadedData();
                result.level = level;
                result.pal = pal;
                result.structure = structure;

                return result;
            }
        }

        private void tileEditor1_Paint(object sender, PaintEventArgs e) {

        }

        private void tsTiles_Paint(object sender, PaintEventArgs e) {

        }


        internal void NotifyStructureDeleted(LevelIndex CurrentLevel, int deletedIndex) {
            StructIndex = 0;
            LoadStruct();
        }

   
    }
}
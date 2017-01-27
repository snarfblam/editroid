using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace Editroid
{
    /// <summary>Provides a grid of screen editors with an incorperated map navigator to allow editing of multiple adjacent map locations.</summary>
    public partial class ScreenGrid:Panel
    {
        int selectedScreenIndex;
        List<ScreenControl> screens = new List<ScreenControl>();
        Size gridSize;
        Point mapEditorLocation;
        int mapEditor_LocationIndex;

        /// <summary>
        /// Creates a ScreenGrid control.
        /// </summary>
        public ScreenGrid() {
            InitializeComponent();


            mapEditor.SelectionChanged += new MapControl.CoordEvent(mapEditor_SelectionChanged);
            MapEditor.TileChanged += new MapControl.CoordEvent(MapEditor_TileChanged);
            BackColor = Color.Black;
            ForeColor = Color.White;
        }

        void MapEditor_TileChanged(object sender, int X, int Y) {
            int screenIndex = 0;
            for(int sy = 0; sy < gridSize.Height; sy++) {
                for(int sx = 0; sx < gridSize.Width; sx++) {
                    if(sx + MapEditor.SelectionX == mapEditorLocation.X && sy + MapEditor.SelectionY == mapEditorLocation.Y) {
                    } else {
                        if(sx + MapEditor.SelectionX == X && sy + MapEditor.SelectionY == Y) {
                            if (mapEditor_LocationIndex == screenIndex) return;

                            screens[screenIndex].LevelIndex = mapEditor.GetLevel(X, Y);
                            screens[screenIndex].ScreenIndex = screens[screenIndex].ScreenIndex;
                            screens[screenIndex].ShowScreen();
                        }
                        screenIndex++;
                    }
                }
            }
        }

        void mapEditor_SelectionChanged(object sender, int X, int Y) {
            Display(X, Y);
        }

        /// <summary>
        /// Gets a collection of screen editors used in this control.
        /// </summary>
        public ReadOnlyCollection<ScreenControl> Screens { get { return screens.AsReadOnly(); } }

        /// <summary>
        /// Redraws all screens.
        /// </summary>
        public void RedrawAllScreens() {
            foreach(ScreenControl screen in screens) {
                screen.ShowScreen();
            }
        }

        /// <summary>Redraws any ScreenControl that is displaying the specified screen.</summary>
        /// <param name="level">The level the screen belongs to.</param>
        /// <param name="screenIndex">The index of the screen, or -1 to specify any screen in the specified level.</param>
        public void RedrawAllInstances(Levels level, int screenIndex) {
            foreach(ScreenControl screen in screens) {
                if(screen.LevelIndex == level && (screenIndex == -1 || screen.ScreenIndex == screenIndex))
                    screen.ShowScreen();
            }
        }

        /// <summary>
        /// Reloads data for the specified level.
        /// </summary>
        /// <param name="level">The level to load data for.</param>
        public void ReloadLevel(Levels level) {
            foreach(ScreenControl screen in screens) {
                if(screen.LevelIndex == level)
                    screen.ScreenIndex = screen.ScreenIndex; // This should cause the screen to be reloaded
                screen.ShowScreen();
            }
        }

        /// <summary>
        /// Sets the size of the screen grid.
        /// </summary>
        /// <param name="width">The width of the grid in screens.</param>
        /// <param name="height">The height of the grid in screens.</param>
        /// <param name="mapX">The location of the map editor.</param>
        /// <param name="mapY">The location of the map editor.</param>
        public void SetSize(int width, int height, int mapX, int mapY) {
            int newCount = width * height - 1;
            int diff = screens.Count - newCount;

            // Destroy extra screens
            for(int i = 0; i < diff; i++) {
                ScreenControl removedControl = screens[screens.Count - 1];
                Controls.Remove(removedControl);
                removedControl.Dispose();
                removedControl.SelectedObjectChanged -= new EventHandler(ScreenControl_SelectedObjectChanged);
                removedControl.SelectionDragged -= new EventHandler(ScreenControl_SelectionDragged);
                screens.Remove(removedControl);
            }

            // Create needed screens
            for(int i = 0; i > diff; i--) {
                ScreenControl newControl = new ScreenControl();
                newControl.Rom = rom;
                newControl.Size = new Size(256, 256);
                newControl.SelectedObjectChanged += new EventHandler(ScreenControl_SelectedObjectChanged);
                newControl.SelectionDragged += new EventHandler(ScreenControl_SelectionDragged);
                screens.Add(newControl);
                Controls.Add(newControl);
            }
            ScreenBorder.SendToBack();


            // position screens, specify indecies
            int screenIndex = 0;
            for(int sy = 0; sy < height; sy++) {
                for(int sx = 0; sx < width; sx++) {
                    if(sx == mapX && sy == mapY) {
                        mapEditor.Bounds = new Rectangle(2 + sx * 258, sy * 258, 256, 256);
                    } else {
                        screens[screenIndex].Bounds = new Rectangle(2 + sx * 258, 2 + sy * 258, 256, 256);
                        screens[screenIndex].Tag = screenIndex;
                        screens[screenIndex].Show();

                        screenIndex++;
                    }
                }
            }

            // Update variables
            gridSize = new Size(width, height);
            mapEditorLocation = new Point(mapX, mapY);
            mapEditor_LocationIndex = mapX + mapY * width;
            MapEditor.SelectionSize = new Size(width, height);
            ScreenBorder.Location = new Point(screens[0].Left - 2, screens[0].Top - 2);
            screens[0].BackColor = SystemColors.Highlight;
            screens[0].ForeColor = SystemColors.HighlightText;
            selectedScreenIndex = 0;
        }

        void ScreenControl_SelectionDragged(object sender, EventArgs e) {
            OnSelectedObjectDragged();
        }

        /// <summary>Occurs when the active screen editor changes.</summary>
        public event EventHandler SelectedScreenChanged;
        /// <summary>
        /// Raises the SelectedScreenChanged event.
        /// </summary>
        protected virtual void OnSelectedScreenChanged() { if(SelectedScreenChanged != null) SelectedScreenChanged(this, new EventArgs()); }
        /// <summary>Occurs when an object is selected in any screen editor.</summary>
        public event EventHandler SelectedObjectChanged;
        /// <summary>
        /// Raises the SelectedObjectChanged event.
        /// </summary>
        protected virtual void OnSelectedObjectChanged() {
            if(SelectedObjectChanged != null) SelectedObjectChanged(this, new EventArgs());
        }
        /// <summary>Occurs when an object is dragged within a screen editor.</summary>
        public event EventHandler SelectedObjectDragged;
        /// <summary>
        /// Raises the SelectedObjectDragged event.
        /// </summary>
        protected virtual void OnSelectedObjectDragged() { if(SelectedObjectDragged != null) SelectedObjectDragged(this, new EventArgs()); }

        void ScreenControl_SelectedObjectChanged(object sender, EventArgs e) {
            ScreenControl newControl = sender as ScreenControl;
            ScreenControl oldSelectedScreen = SelectedScreen;

            selectedScreenIndex = (int)(newControl.Tag);

            if(newControl != oldSelectedScreen) {
                oldSelectedScreen.Deselect();
                oldSelectedScreen.ShowScreen();
                oldSelectedScreen.BackColor = this.BackColor;
                oldSelectedScreen.ForeColor = this.ForeColor;
                newControl.BackColor = SystemColors.Highlight;
                newControl.ForeColor = SystemColors.HighlightText;
                ScreenBorder.Bounds = new Rectangle(newControl.Left - 2, newControl.Top - 2, 260, 260);
                OnSelectedScreenChanged();
            }

            OnSelectedObjectChanged();
            GlobalEventManager.Manager.OnObjectSelected(this, newControl.LevelIndex, newControl.SelectedItem);

        }

        /// <summary>
        /// Gets/sets whether screens will be displayed using alternate palettes.
        /// </summary>
        [DefaultValue(false)]
        public bool UseAlternatePalette {
            get {
                if(screens.Count == 0) return false;
                return screens[0].AltPalette;
            }
            set {
                foreach(ScreenControl screen in screens) {
                    screen.AltPalette = value;
                }
            }
        }

        /// <summary>
        /// Gets the currently active screen editor.
        /// </summary>
        public ScreenControl SelectedScreen { get { return screens[selectedScreenIndex]; } }

        /// <summary>
        /// Moves the display location by specifying the map coordinates of the upper-left window in the viewport.
        /// </summary>
        public void Display(int mapX, int mapY) {
            int screenIndex = 0;
            for(int sy = 0; sy < gridSize.Height; sy++) {
                for(int sx = 0; sx < gridSize.Width; sx++) {
                    if(sy == mapEditorLocation.Y && sx == mapEditorLocation.X) {
                    } else {
                        // If off of the game map, show blank screen
                        if(sx + mapX > 31 || sy + mapY > 31 || sx + mapX < 0 || sy + mapY < 0) {
                            screens[screenIndex].MapLocation = new Point(mapX + sx, mapY + sy);
                            screens[screenIndex].LevelIndex = Levels.Invalid;
                            screens[screenIndex].ScreenIndex = 0;
                            OnSelectedObjectChanged();
                            screens[screenIndex].ShowScreen();
                        } else {
                            Levels levelIndex = mapEditor.GetLevel(mapX + sx, mapY + sy);
                            int layoutIndex = rom.GetRoomIndex(mapX + sx, mapY + sy);

                            screens[screenIndex].MapLocation = new Point(mapX + sx, mapY + sy);
                            screens[screenIndex].LevelIndex = levelIndex;
                            screens[screenIndex].ScreenIndex = layoutIndex;
                            if(screenIndex == selectedScreenIndex)
                                screens[screenIndex].MakeDefaultSelection();
                            screens[screenIndex].ShowScreen();
                            //screens[screenIndex].RefreshData(true);

                        }
                        screenIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the map navigation control hosted in this ScreenGrid.
        /// </summary>
        public MapControl MapEditor { get { return mapEditor; } }

        private Rom rom;

        /// <summary>
        /// Gets/sets the ROM data will be read from.
        /// </summary>
        public Rom Rom {
            get { return rom; }
            set {
                rom = value;
                foreach(ScreenControl screen in screens) {
                    screen.Rom = rom;
                    mapEditor.Rom = rom;
                }
            }
        }

    }
}

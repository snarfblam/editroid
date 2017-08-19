using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Editroid.Graphic;
using Editroid.ROM;

namespace Editroid
{
    /// <summary>
    /// Creates a map 
    /// </summary>
    internal class MapMaker
    {

        public const int ScreenWidth = 256;
        public const int ScreenHeight = 240;
        public const int ScreenImageHeight = 256;

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { rom = value; }
        }

        private bool showPhysics;

        public bool ShowPhysics {
            get { return showPhysics; }
            set { showPhysics = value; }
        }

        private bool fillEmptySpots;

        public bool FillEmptySpots {
            get { return fillEmptySpots; }
            set { fillEmptySpots = value; }
        }

        private Rectangle mapArea;

        public Rectangle MapArea {
            get { return mapArea; }
            set { mapArea = value; }
        }

        private LevelIndex levelFilter = LevelIndex.None;

        public LevelIndex LevelFilter {
            get { return levelFilter; }
            set { levelFilter = value; }
        }

        private int scale = 1;

        public int Scale {
            get { return scale; }
            set { scale = value; }
        }

        private MapControl levelMap;

        public MapControl LevelMap {
            get { return levelMap; }
            set { levelMap = value; }
        }

        private bool highQuality;

        public bool HighQuality {
            get { return highQuality; }
            set { highQuality = value; }
        }

        private bool hideEnemies;

        public bool HideEnemies {
            get { return hideEnemies; }
            set { hideEnemies = value; }
        }



        ScreenRenderer renderer = new ScreenRenderer();
        Blitter b = new Blitter();
        Bitmap screenBitmap = new Bitmap(ScreenWidth, ScreenImageHeight, PixelFormat.Format8bppIndexed);

        Bitmap map;
        Graphics gMap;

        public Bitmap Render() {
            map = new Bitmap(mapArea.Width * ScreenWidth / scale, mapArea.Height * ScreenHeight / scale, PixelFormat.Format24bppRgb);
            gMap = Graphics.FromImage(map);
            gMap.InterpolationMode = highQuality ?
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic :
                System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            gMap.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;


            for(int x = 0; x < mapArea.Width; x++) {
                for(int y = 0; y < mapArea.Height; y++) {
                    DrawScreen(x + mapArea.Left, y + mapArea.Y, x ,y);

                    if(RoomDrawn != null)
                        RoomDrawn(this, new EventArgs());
                }
            }

            gMap.Dispose();
            return map;
        }

        private void DrawScreen(int gameX, int gameY, int mapx, int mapY) {
            // Get currentLevelIndex and screen
            LevelIndex level = levelMap.GetLevel(gameX, gameY);
            bool invalidLevel = false;

            if(level == LevelIndex.None ||
                (levelFilter != LevelIndex.None && level != levelFilter)) {
                return;
                ////if(!fillEmptySpots) return;

                ////if(levelFilter != LevelIndex.None)
                ////    level = levelFilter;
                ////else 
                ////    level = GuessLevel(gameX, gameY);

                ////if(level == LevelIndex.Tourian)
                ////    level = LevelIndex.Kraid;

                ////invalidLevel = true;
            }

            Level levelData = rom.GetLevel(level);
            int screenIndex = rom.GetScreenIndex(gameX, gameY);
            if(screenIndex == 0xFF || invalidLevel) { // Blank screen
                screenIndex = GetBlankScreenIndex(level);
            }



            Screen screen = levelData.Screens[screenIndex];
            // Load data into renderer
            renderer.Level = levelData;
            renderer.SelectedEnemy = -1;

            // Apply paletteIndex
            //screen.ApplyLevelPalette(screenBitmap); // Sprites
            //renderer.ApplyPalette(screenBitmap, levelMap.GetAltPal(gameX, gameY)); // Backgrounds
            ScreenEditor.ApplyPalette(levelMap.GetAltPal(gameX, gameY), levelData, screenBitmap, HighlightEffect.Invert);

            // Render Screen
            renderer.DefaultPalette = screen.ColorAttributeTable;
            renderer.Clear();
            renderer.SelectedDoor = -1;
            renderer.DrawScreen(screenIndex);
            IList<EnemyInstance> enemies = hideEnemies ? null : screen.Enemies;
            renderer.Render(b, screenBitmap, enemies, screen.Doors, showPhysics);

            Rectangle source = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
            Rectangle dest = new Rectangle(
                mapx * ScreenWidth / scale, mapY * ScreenHeight / scale,
                ScreenWidth / scale, ScreenHeight / scale);

            gMap.DrawImage(screenBitmap, dest, source, GraphicsUnit.Pixel);

        }

        private int GetBlankScreenIndex(LevelIndex level) {
            switch(level) {
                case LevelIndex.Brinstar:
                    return 0x08;
                case LevelIndex.Norfair:
                    return 0x06;
                case LevelIndex.Tourian:
                    return 0x0B;
                case LevelIndex.Kraid:
                    return 0x0B;
                case LevelIndex.Ridley:
                    return 0x05;
            }

            return 0x08;
        }

        private LevelIndex GuessLevel(int x, int y) {
            LevelIndex l;
            l = levelAt(x, y - 1);
            if(l != LevelIndex.None) return l;
            l = levelAt(x, y + 1);
            if(l != LevelIndex.None) return l;
            l = levelAt(x - 1, y);
            if(l != LevelIndex.None) return l;
            l = levelAt(x + 1, y);
            if(l != LevelIndex.None) return l;
            l = levelAt(x - 1, y - 1);
            if(l != LevelIndex.None) return l;
            l = levelAt(x + 1, y - 1);
            if(l != LevelIndex.None) return l;
            l = levelAt(x - 1, y + 1);
            if(l != LevelIndex.None) return l;
            l = levelAt(x + 1, y + 1);
            if(l != LevelIndex.None) return l;

            return LevelIndex.Brinstar;
        }

        /// <summary>For use exclusively by GuessLevel as a safe
        /// way to check the currentLevelIndex at a map position.</summary>
        private LevelIndex levelAt(int x, int y) {
            if(x < 0 || y < 0 || x > 31 || y > 31) return LevelIndex.None;
            return levelMap.GetLevel(x, y);
        }
        /// <summary>
        /// Raised each time a room is drawn to the map. This event will also be
        /// raised when the renderer skips over a blank room.
        /// </summary>
        public event EventHandler RoomDrawn;

    }
}

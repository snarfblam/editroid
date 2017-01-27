using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.Graphic;
using System.Drawing.Imaging;
using Editroid.ROM;

namespace Editroid
{
    public partial class frmRearrageTiles : Form
    {
        TileDisplay tdArrangement, tdUnplaced;
        MetroidRom rom;
            Level levelData;

        public frmRearrageTiles() {
            InitializeComponent();

            tdArrangement = new TileDisplay();
            tdArrangement.Zoom = true;
            tdArrangement.Bounds = new Rectangle(8, 8, 256, 256);
            tdArrangement.SetInitialArrangement();
            tdArrangement.MouseDown += new MouseEventHandler(tdArrangement_MouseDown);
            Controls.Add(tdArrangement);

            tdUnplaced = new TileDisplay();
            tdUnplaced.Zoom = true;
            tdUnplaced.Bounds = new Rectangle(272, 8, 256, 256);
            tdUnplaced.SetEmptyArrangement();
            tdUnplaced.MouseDown += new MouseEventHandler(tdUnplaced_MouseDown);
            Controls.Add(tdUnplaced);
        }

        bool tileMarked;
        TileDisplay markedControl;

        void tdUnplaced_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left){
                var c = (TileDisplay)sender;
                TileClick(c, c.TileFromPoint(e.Location));
            }else if (e.Button == MouseButtons.Right) {
                RedrawArrangers();
            }
        }

        void tdArrangement_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left){
                var c = (TileDisplay)sender;
                TileClick(c, c.TileFromPoint(e.Location));
            }else if (e.Button == MouseButtons.Right) {
                RedrawArrangers();
            }
        }

        private void TileClick(TileDisplay ctrl, Point tile) {
            if (tileMarked) {
                SwapTiles(markedControl, markedControl.InvertedTile, ctrl, tile);

                tileMarked = false;
                markedControl.InvertedTile = new Point(-1, -1);
            } else {
                markedControl = ctrl;
                ctrl.InvertedTile = tile;
                tileMarked = true;
            }
        }

        private void SwapTiles(TileDisplay source, Point srcTile, TileDisplay dest, Point destTile) {
            byte tileA = source.GetTile(srcTile);
            byte tileB = dest.GetTile(destTile);

            source.SetTile(srcTile, tileB);
            dest.SetTile(destTile, tileA);
        }




        private void RedrawArrangers() {
            tdArrangement.PaletteIndex = (tdArrangement.PaletteIndex + 1) & 7;
            tdUnplaced.PaletteIndex = tdArrangement.PaletteIndex;

            tdArrangement.Redraw();
            tdUnplaced.Redraw();
        }

        public void LoadRomData(Level lvl) {
            this.rom = lvl.Rom;
            levelData = lvl;

            tdArrangement.LoadLevel(lvl);
            tdUnplaced.LoadLevel(lvl);

            tdArrangement.Redraw();
            tdUnplaced.Redraw();
        }

        void PerformTileRelocation() {
            const int chrBankSize = 0x1000;
            const int tileSize = 0x10;

            byte[] CHR = new byte[chrBankSize];
            var oBgPatterns = EnhancedPatternOffsets.GetBgBank(levelData.Index);
            var bankCount = rom.ChrUsage.GetBgLastPage(levelData.Index) - rom.ChrUsage.GetBgFirstPage(levelData.Index) + 1;


            // Re-arrange CHR ROM
            for (int i = 0; i < bankCount; i++) {
                // Copy CHR bank
                Array.Copy(rom.data, oBgPatterns, CHR, 0, chrBankSize);

                // Paste new arrangement of tiles into ROM
                for (int tile = 0; tile < 0x100; tile++) {
                    byte tileIndex = tdArrangement.GetTile(tile);
                    Array.Copy(CHR, tileIndex * 0x10, rom.data, oBgPatterns, tileSize);
                    oBgPatterns += tileSize;
                }

                // oBgPatterns now points to next CHR bank
            }

            // Re-arrange combos (metatiles)
            byte[] lookup = tdArrangement.CreateLookupTable();
            // Assuming 256 combos
            for (int i = 0; i < 0x100; i++) {
                var def = levelData.Combos[i];
                def[0] = lookup[def[0]];
                def[1] = lookup[def[1]];
                def[2] = lookup[def[2]];
                def[3] = lookup[def[3]];
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            PerformTileRelocation();
            DialogResult = DialogResult.OK;
            Close();
        }


        private class TileDisplay : Control
        {
            readonly Blitter renderer;
            readonly Byte[,] tileIndecies;
            readonly Bitmap buffer;

            bool readyToRender = false;

            Bitmap Patterns;
            NesPalette Palette;
            NesPalette altPalette;
            Level levelData;
            int _PaletteIndex;
            int _scale = 1;


            public TileDisplay() {
                renderer = new Blitter();
                tileIndecies = new byte[16, 16];
                buffer = new Bitmap(256, 256, PixelFormat.Format8bppIndexed);

                SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, true);
            }

            Point _InvertedTile;
            public Point InvertedTile {
                get { return _InvertedTile; }
                set {
                    if (value != _InvertedTile) {
                        _InvertedTile = value;
                        Redraw();
                    }
                }
            }

            public void LoadLevel(Level lvl) {
                levelData = lvl;
                Patterns = (Bitmap)lvl.Patterns.PatternImage.Clone();
                Palette = lvl.BgPalette;
                altPalette = lvl.BgAltPalette;

                var buffpal = buffer.Palette;
                Palette.ApplyTable(buffpal.Entries);
                altPalette.ApplyTable(buffpal.Entries, 16);
                Palette.ApplyTable(buffpal.Entries, 32, 0, 16, invertColor);
                altPalette.ApplyTable(buffpal.Entries, 48, 0, 16, invertColor);
                buffer.Palette = buffpal;

                readyToRender = true;
            }
            private static Color invertColor(Color c) { return Color.FromArgb(c.A, 255 - c.R, 255 - c.G, 255 - c.B); }

            public int PaletteIndex {
                get { return _PaletteIndex; }
                set {
                    _PaletteIndex = value;
                    paint();
                    Invalidate();
                }
            }

            public bool Zoom {
                get { return _scale != 1; }
                set {
                    _scale = value ? 2 : 1;
                }
            }

            public void Redraw() {
                paint();
                Invalidate();
            }

            void paint() {
                if (readyToRender) {
                    renderer.Begin(Patterns, buffer);

                    for (int tileY = 0; tileY < 16; tileY++) {
                        for (int tileX = 0; tileX < 16; tileX++) {
                            if (tileX == _InvertedTile.X & tileY == _InvertedTile.Y) {
                                renderer.BlitTile(tileIndecies[tileX, tileY], tileX, tileY, (byte)(_PaletteIndex | 8));
                            } else {
                                renderer.BlitTile(tileIndecies[tileX, tileY], tileX, tileY, (byte)_PaletteIndex);
                            }
                        }
                    }

                    renderer.End();
                }
            }

            protected override void OnPaint(PaintEventArgs e) {
                base.OnPaint(e);

                int crossScale = _scale * 8;
                int crossSize = crossScale - 1;

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                int width = 128 * _scale;
                int height = 128 * _scale;

                e.Graphics.DrawImage(buffer, new Rectangle(0, 0, width, height), new Rectangle(0, 0, 128, 128), GraphicsUnit.Pixel);
                for (int tileY = 0; tileY < 16; tileY++) {
                    for (int tileX = 0; tileX < 16; tileX++) {
                        if (tileIndecies[tileX, tileY] == 0xFF) {
                            e.Graphics.DrawLine(Pens.White, tileX * crossScale, tileY * crossScale, tileX * crossScale + crossSize, tileY * crossScale + crossSize);
                            e.Graphics.DrawLine(Pens.White, tileX * crossScale + crossSize, tileY * crossScale, tileX * crossScale, tileY * crossScale + crossSize);
                        }
                    }
                }
            }

            internal void SetInitialArrangement() {
                int index = 0;
                for (int tileY = 0; tileY < 16; tileY++) {
                    for (int tileX = 0; tileX < 16; tileX++) {
                        tileIndecies[tileX, tileY] = (byte)index;
                        index++;
                    }
                }
            }

            internal void SetEmptyArrangement() {
                for (int tileY = 0; tileY < 16; tileY++) {
                    for (int tileX = 0; tileX < 16; tileX++) {
                        tileIndecies[tileX, tileY] = 0xFF;
                    }
                }
            }

            public Point TileFromPoint(Point location) {
                Point result = new Point(location.X / 8, location.Y / 8);
                if (_scale > 1) {
                    result = new Point(result.X / _scale, result.Y / _scale);
                }
                return result;
            }

            public byte GetTile(Point location) {
                return tileIndecies[location.X, location.Y];
            }
            public byte GetTile(int index) {
                return tileIndecies[index % 16, index / 16];
            }
            public void SetTile(Point location, byte value) {
                tileIndecies[location.X, location.Y] = value;
                Redraw();
            }

            /// <summary>
            /// Returns a 256-entry table where for (table[i] = d), i is the index of the original tile and d is the index the tile has been moved to.
            /// </summary>
            /// <returns></returns>
            internal byte[] CreateLookupTable() {
                // Inititalize to all 0xFFs
                byte[] result = new byte[0x100];
                for (int i = 0; i < result.Length; i++) { result[i] = 0xFF; }

                int destTile = 0;

                for (int tileY = 0; tileY < 16; tileY++) {
                    for (int tileX = 0; tileX < 16; tileX++) {
                        var sourceTile = tileIndecies[tileX, tileY];
                        result[sourceTile] = (byte)destTile;

                        destTile++;
                    }
                }
                return result;
            }

        }

        private void button2_Click(object sender, EventArgs e) {

        }


    }
}

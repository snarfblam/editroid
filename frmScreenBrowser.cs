using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Editroid.Graphic;

namespace Editroid
{
    public partial class frmScreenBrowser : Form
    {
        ScreenRenderer renderer = new ScreenRenderer();
        Blitter blitter = new Blitter();
        Bitmap blitterBuffer = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

        Bitmap buffer = new Bitmap(552, 520, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        Graphics gBuffer;

        public frmScreenBrowser() {
            InitializeComponent();

            gBuffer = Graphics.FromImage(buffer);
            picScreens.Image = buffer;
        }

        public static int GetScreenIndex(Level level, int defaultSelection, bool altPalette, bool showPhysics) {
            frmScreenBrowser brawsuh = new frmScreenBrowser();
            brawsuh.Level = level;
            brawsuh.HighlightedScreen = defaultSelection;
            brawsuh.UseAlternatePalette = altPalette;
            brawsuh.ShowPhysics = showPhysics;
            brawsuh.BaseIndex = defaultSelection - (defaultSelection % 16);

            brawsuh.Render();

            brawsuh.DialogResult = DialogResult.Cancel;

            if (brawsuh.ShowDialog() == DialogResult.OK) {
                return brawsuh.SelectedScreen;
            } else {
                return -1;
            }

            brawsuh.Dispose();
        }

        

        const int cellWidth = 128;
        const int cellHeight = 120;
        const int cellPadding = 8;
        const int cellSpacingX = cellPadding + cellWidth;
        const int cellSpacingY = cellPadding + cellHeight;
        void Render() {
            Text = "Screen Browser [" + BaseIndex.ToString("x") + " - " + Math.Min(BaseIndex + 15, Level.Screens.Count - 1).ToString("x") + "]";

            gBuffer.Clear(Color.Black);
            gBuffer.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            gBuffer.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            renderer.Level = this.Level;
            //renderer.ApplyPalette(blitterBuffer, UseAlternatePalette);
            var pal = blitterBuffer.Palette;
            if (UseAlternatePalette) {
                Level.BgAltPalette.ApplyTable(pal.Entries,0);
                Level.BgAltPalette.ApplyTable(pal.Entries, 16);
                Level.SpriteAltPalette.ApplyTable(pal.Entries, 32);
            } else {
                Level.BgPalette.ApplyTable(pal.Entries, 0);
                Level.BgPalette.ApplyTable(pal.Entries, 16);
                Level.SpritePalette.ApplyTable(pal.Entries, 32);
            }

            blitterBuffer.Palette = pal;


            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    int index = BaseIndex + x + y * 4;

                    if (index < Level.Screens.Count) {
                        var screen = Level.Screens[index];

                        renderer.SelectedEnemy = -1;
                        renderer.SelectedDoor = -1;
                        renderer.DefaultPalette = screen.ColorAttributeTable;

                        renderer.Clear();
                        renderer.DrawScreen(index);
                        renderer.Render(blitter, blitterBuffer, screen.Enemies, screen.Doors, ShowPhysics);

                        Rectangle source = new Rectangle(0, 0, 256, 240);
                        Rectangle dest = new Rectangle(cellPadding + cellSpacingX * x, cellPadding + cellSpacingY * y, cellWidth, cellHeight);

                        gBuffer.DrawImage(blitterBuffer, dest, source, GraphicsUnit.Pixel);

                        if (index == HighlightedScreen) {
                            dest.Inflate(2, 2);
                            gBuffer.DrawRectangle(SystemPens.Highlight, dest);
                            dest.Inflate(1, 1);
                            gBuffer.DrawRectangle(SystemPens.Highlight, dest);
                        }
                    }
                }
            }
        }

        public Level Level { get; private set; }
        public int BaseIndex { get; private set; }
        public int HighlightedScreen { get; private set; }
        public int SelectedScreen { get; private set; }

        public bool UseAlternatePalette { get; set; }
        public bool ShowPhysics { get; set; }

        private void btnNext_Click(object sender, EventArgs e) {
            BaseIndex += 16;
            if (BaseIndex >= Level.Screens.Count)
                BaseIndex = 0;

            Render();
            picScreens.Invalidate();
        }

        private void btnBack_Click(object sender, EventArgs e) {
            BaseIndex -= 16;
            if (BaseIndex < 0)
                BaseIndex = (Level.Screens.Count - 1) - (Level.Screens.Count - 1) % 16;

            Render();
            picScreens.Invalidate();
        }

        private void picScreens_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                // Need to compensate for images being offset and spaced
                int x = e.X - cellPadding;
                int y = e.Y - cellPadding;

                // Index of cell
                int screenX = x / cellSpacingX;
                int screenY = y / cellSpacingY;
                // Position within cell
                int cellX = x % cellSpacingX;
                int cellY = y % cellSpacingY;

                bool notInPadding = cellX < cellWidth && cellY < cellHeight;
                bool isValidCell = screenX >= 0 && screenX < 4 && screenY >= 0 && screenY < 4;
                SelectedScreen = screenX + screenY * 4 + BaseIndex;
                isValidCell &= SelectedScreen < Level.Screens.Count;
                if (isValidCell & notInPadding) {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void frmScreenBrowser_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;

namespace Editroid
{
    public partial class frmStructBrowser : Form
    {
        StructRenderer renderer = new StructRenderer();
        Bitmap structimg;
        Graphics gStructimg;

        public int BaseIndex { get; private set; }
        public Level Level { get; private set; }
        public int Palette { get; private set; }
        public int HighlightedIndex { get; private set; }
        public int SelectedIndex { get; private set; }

        public bool UseAlternatePalette { get; set; }

        public frmStructBrowser() {
            InitializeComponent();

            structimg = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gStructimg = Graphics.FromImage(structimg);
            gStructimg.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            picStructDisplay.Image = structimg;
        }

        public int ShowStructs(Level l, int pallette, int baseIndex) {
            return ShowStructs(l, pallette, baseIndex, -1);
        }
        public int ShowStructs(Level l, int pallette, int baseIndex, int selectedIndex) {
            HighlightedIndex = selectedIndex;
            SelectedIndex = -1;
            DialogResult = DialogResult.Cancel; // Unless we decide otherwise.

            this.Level = l;
            this.Palette = pallette;
            this.BaseIndex = baseIndex;
            this.HighlightedIndex = selectedIndex;

            RenderStructures();
            Program.Dialogs.ShowDialog(this, Program.MainForm); 

            return SelectedIndex;
        }
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape) Close();
        }
        const int structTileWidth = 16;
        const int structTileHeight = 16;
        const int structCellWidth_Tiles = 4;
        const int structCellHeight_Tiles = 4;
        const int structCellMargin = 8;
        const int structCellWidth = structTileWidth * structCellWidth_Tiles;
        const int structCellHeight = structTileHeight * structCellHeight_Tiles;
        const int structDisplayOffset = 8;
        const int structCellSpacingX = structCellWidth + structCellMargin;
        const int structCellSpacingY = structCellHeight + structCellMargin;
        private void RenderStructures() {
            Text = "Structure Browser [" + BaseIndex.ToString("x2") + "-" + Math.Min(Level.Structures.Count - 1, BaseIndex + 15).ToString("x2") + "]";



            this.gStructimg.Clear(Color.Black);

            Bitmap img;
            bool palApplied = false;

            for (int x = 0; x < 4; x++) {
                for (int y = 0; y < 4; y++) {
                    int index = BaseIndex + x + y * 4;
                    if (index < Level.Structures.Count) {
                        int structWidth, structHeight;
                        img = renderer.RenderSingleStructure(Level, index, Palette, out structWidth, out structHeight);

                        if (!palApplied) {
                            var pal = img.Palette;
                            if (UseAlternatePalette) {
                                Level.BgAltPalette.ApplyTable(pal.Entries);
                            } else {
                                Level.BgPalette.ApplyTable(pal.Entries);
                            }
                            img.Palette = pal;

                            palApplied = true;
                        }

                        Rectangle source = new Rectangle(0, 0, structWidth * structTileWidth, structHeight * structTileHeight);
                        Rectangle dest = new Rectangle(x * structCellSpacingX + 8, y * structCellSpacingY + 8, source.Width, source.Height);

                        // Structs will be scaled down if larger that 64 px
                        if (structWidth > structCellWidth_Tiles || structHeight > structCellHeight_Tiles) {
                            source = new Rectangle(0, 0, structTileWidth * structWidth, structTileHeight * structHeight);

                            float scaleX = (float)structCellWidth_Tiles / (float)structWidth;
                            float scaleY = (float)structCellHeight_Tiles / (float)structHeight;

                            // Whether scale is based on width or height depends on which is greater
                            if (scaleX > scaleY) { // Taller
                                dest = new Rectangle(dest.X, dest.Y, (int)(source.Width * scaleY), (int)(source.Height * scaleY));
                                // center (height - width) adjusted for scale and halved
                                // Note that this assumes the cells are square
                                dest.X += (int)((source.Height - source.Width) * scaleY / 2);
                            } else { // Wider
                                dest = new Rectangle(dest.X, dest.Y, (int)(source.Width * scaleX), (int)(source.Height * scaleX));
                                // center (width - height) adjusted for scale and halved
                                // Note that this assumes the cells are square
                                dest.Y += (int)((source.Width - source.Height) * scaleX / 2);
                            }

                        } else {
                            // Otherwise, just center
                            dest.X += (structCellWidth - source.Width) / 2;
                            dest.Y += (structCellHeight - source.Height) / 2;
                        }

                        // Draw selection
                        gStructimg.DrawImage(img, dest, source, GraphicsUnit.Pixel);
                        if (index == HighlightedIndex) {
                            var selectionRect = dest;
                            selectionRect.X-=2;
                            selectionRect.Y-=2;
                            selectionRect.Width += 4;
                            selectionRect.Height += 4;
                            gStructimg.DrawRectangle(SystemPens.Highlight,selectionRect);
                        }
                    }
                }
            }



        }

        private void btnNext_Click(object sender, EventArgs e) {
            BaseIndex += 16;
            if (BaseIndex >= Level.Structures.Count)
                BaseIndex = 0;
            RenderStructures();
            this.picStructDisplay.Invalidate();
        }

        private void btnBack_Click(object sender, EventArgs e) {
            BaseIndex -= 16;
            if (BaseIndex < 0) {
                BaseIndex = (Level.Structures.Count - 1) - (Level.Structures.Count - 1) % 16;
            }
            RenderStructures();
            this.picStructDisplay.Invalidate();
        }

        private void picStructDisplay_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                // Need to compensate for images being offset and spaced
                int x = e.X - structDisplayOffset;
                int y = e.Y - structDisplayOffset;

                // Index of cell
                int structX = x / structCellSpacingX;
                int structY = y / structCellSpacingY;
                // Position within cell
                int cellX = x % structCellSpacingX;
                int cellY = y % structCellSpacingY;

                bool notInPadding = cellX < structCellWidth && cellY < structCellHeight;
                bool isValidCell = structX >= 0 && structX < 4 && structY >= 0 && structY < 4;
                SelectedIndex = structX + structY * 4 + BaseIndex;
                isValidCell &= SelectedIndex < Level.Structures.Count;
                if (isValidCell & notInPadding) {
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }else{
                Palette = (Palette + 1) % 4;
                RenderStructures();
                picStructDisplay.Invalidate();
            }
        }

        
    }

    class StructRenderer
    {
        Bitmap buffer = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
        Editroid.Graphic.Blitter blitter = new Editroid.Graphic.Blitter();

        const int maxStructWidth = 16;
        const int maxStructHeight = 16;
        const int paddingX = 1;
        const int paddingY = 1;
        const int cellWidth = maxStructWidth + paddingX;
        const int cellHeight = maxStructHeight + paddingY;
        const int cellCountX = 3;
        const int cellCountY = 3;

        public StructRenderer() {
            StructsPerRow = 3;
        }

        public int StructsPerRow { get; set; }

        Level rendering_level;
        int rendering_baseIndex;
        int rendering_palette;

        public Bitmap Render(Level level, int baseIndex, int palette) {
            int ignored1, ignored2;

            rendering_level = level;
            rendering_baseIndex = baseIndex;
            rendering_palette = palette;

            blitter.Begin(level.Patterns.PatternImage, buffer);
            blitter.Clear();

            for (int x = 0; x < cellCountX; x++) {
                for (int y = 0; y < cellCountY; y++) {
                    int index = baseIndex + x + y * StructsPerRow;
                    if (index < level.Structures.Count) {
                        RenderStructure(level.Structures[index], x * cellWidth, y * cellHeight, out ignored1, out ignored2);
                    }
                }
            }

            blitter.End();

            return buffer;
        }
        public Bitmap RenderSingleStructure(Level level, int index, int palette, out int width, out int height) {
            rendering_level = level;
            rendering_baseIndex = index;
            rendering_palette = palette;

            blitter.Begin(level.Patterns.PatternImage, buffer);
            blitter.Clear();

            if (index < level.Structures.Count) {
                RenderStructure(level.Structures[index], 0, 0, out width, out height);
            } else {
                throw new ArgumentException("Specified index is out of range.");
            }



            blitter.End();

            return buffer;
        }

        private void RenderStructure(Structure s, int x, int y, out int width, out int height) {
            width = 0;
            height = 0;
            for (int structX = 0; structX < maxStructWidth; structX++) {
                for (int structY = 0; structY < maxStructHeight; structY++) {
                    var dat = s.Data[structX, structY];
                    if (dat != 0xFF) {
                        RenderCombo(dat, x + structX, y + structY);
                        if (structX == width) width = structX + 1; // e.g. structX = 0, width = 0 -> width = 1
                        if (structY == height) height = structY + 1; //    structX = 1, width = 1 -> width = 2 (etc.)
                    }
                }
            }
        }

        private void RenderCombo(int index, int x, int y) {
            var cbo = rendering_level.Combos[index];

            x *= 2;
            y *= 2;
            blitter.BlitTile(cbo[0], x, y, (byte)rendering_palette);
            blitter.BlitTile(cbo[1], x + 1, y, (byte)rendering_palette);
            blitter.BlitTile(cbo[2], x, y + 1, (byte)rendering_palette);
            blitter.BlitTile(cbo[3], x+ 1, y + 1, (byte)rendering_palette); 
        }
    }
}

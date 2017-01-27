using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Editroid.ROM;
using fart = System.Int32;

namespace Editroid
{
    class TileEditor:Control
    {
        Bitmap bg = new Bitmap(8, 8, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        public TileEditor() {
            this.SetStyle(ControlStyles.Opaque, true);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(bg, new Rectangle(0, 0, Width, Height), new Rectangle(0, 0, 8, 8), GraphicsUnit.Pixel);
        }


        bool drawing;
        List<Point> paintedPixels = new List<Point>();

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left) {
                drawing = true;
                DrawPixel(8 * e.X / Width, 8 * e.Y / Height);
                Invalidate();
                Update();
            }
        }

        private void DrawPixel(int x, int y) {
            if (x < 0 || x > 7 || y < 0 || y > 7)
                return;

            bg.SetPixel(x, y, palette[selectedColor]);
            paintedPixels.Add(new Point(x, y));
        }
        private int selectedColor;

        public int SelectedColor {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        Color[] palette = new Color[4];
        public void SetPalette(IList<Color> colors) {
            if (colors.Count < 4) throw new ArgumentException("Invalid palette specified.", "colors");

            for (int i = 0; i < 4; i++) {
                palette[i] = colors[i];
            }

            Redraw();
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(drawing)
                DrawPixel(8 * e.X / Width, 8 * e.Y / Height);
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (drawing && e.Button == MouseButtons.Left) {
                drawing = false;
                CommitChanges();
            }
        }

        private void Redraw() {
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    //bg.SetPixel(x, y, putsomethinghere);
                }
            }

            Invalidate();
            Update();
        }

        private pRom patternOffset;
        public pRom PatternOffset {
            get { return patternOffset; }
        }
        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { rom = value; }
        }

        public void LoadPattern(pRom offset) {
            patternOffset = offset;
        }
        private void CommitChanges() {
            //Todo: create an action
            //  mark w/ and track unique id to avoid redraw when action is performed
            //      after 1 match, discard tracking value to avoid skipping subsequent redraws for undo/redos
            //  action to simply contain before & after binary data for tile
        }


    }
}

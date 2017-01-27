using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace Editroid.Controls
{
    class PalControlBase:UserControl
    {
        Bitmap bg;
        Graphics gbg;
        Bitmap oldBg; // Bitmap waiting to be disposed
        Graphics oldgbg; // Graphics waiting to be disposed.

        protected Bitmap Buffer { get { return bg; } }
        protected Graphics gBuffer { get { return gbg; } }

        protected const int CellWidth = 16;
        protected const int CellHeight = 16;
        
        Size gridSize = new Size(1, 1);
        public Size GridSize { get { return gridSize; } }

        public PalControlBase() {
            BackgroundImageLayout = ImageLayout.None;
        }
        protected void SetGridSize(Size size) {
            Size neededBufferSize = new Size(size.Width * CellWidth, size.Height * CellHeight);
            if (neededBufferSize.Width < 1) neededBufferSize.Width = 1;
            if (neededBufferSize.Height < 1) neededBufferSize.Height = 1;

            if (bg == null || neededBufferSize.Width < bg.Width || neededBufferSize.Height < Buffer.Height) {
                Size newBufferSize = neededBufferSize;
                if (bg != null && !AllowBufferToShrink) {
                    // Never shrink back buffer
                    newBufferSize = new Size(Math.Max(bg.Width, neededBufferSize.Width), Math.Max(neededBufferSize.Height, bg.Height));
                }
            }

            oldBg = bg;
            oldgbg = gbg;
            bg = new Bitmap(neededBufferSize.Width, neededBufferSize.Height, PixelFormat.Format32bppArgb);
            gbg = Graphics.FromImage(bg);
            OnNewBufferCreated();

            base.BackgroundImage = bg;
            this.gridSize = size;
            if (oldBg != null) {
                oldBg.Dispose();
                oldBg = null;
            }
            if (oldgbg != null) {
                oldgbg.Dispose();
                oldgbg = null;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            int cellY = e.Y / 16;
            int cellX = e.X / 16;

            if (cellX < 0 || cellY < 0 || cellX >= gridSize.Width || cellY >= gridSize.Height)
                return;

            OnCellClicked(cellX, cellY, e.Button);
            
        }

        public event EventHandler<EventArgs<Point>> CellClicked;
        protected virtual void OnCellClicked(int cellX, int cellY, MouseButtons mouseButtons) {
            if (CellClicked != null)
                CellClicked(this, new EventArgs<Point>(new Point(cellX, cellY)));
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Image BackgroundImage { get { return null; } set { base.BackgroundImage = value; } }

        [DefaultValue(false)]
        public bool AllowBufferToShrink { get; set; }

        protected void DrawSquare(int x, int y, byte color) {
            Rectangle rect = new Rectangle(x * 16 + 1, y * 16 + 1, 14, 14);
            using (Brush b = new SolidBrush(NesPalette.NesColors[color])) {
                gbg.FillRectangle(b, rect);
                //gbg.FillRectangle(overlay, rect);
            }
            gbg.DrawRectangle(Pens.White, rect);
            Invalidate(rect);
        }
        /// <summary>
        /// Called when a new background bitmap is created. The old buffer may be disposed immediately after this function completes.
        /// </summary>
        protected virtual void OnNewBufferCreated() {
        }


        int updateLevel = 0;
        /// <summary>
        /// Prevents invalidation until EndUpdate is called as many times as BeginUpdate
        /// </summary>
        protected void BeginUpdate() {
            updateLevel++;
        }
        protected void EndUpdate() {
            updateLevel--;
            if (updateLevel == 0)
                Invalidate();
            else if (updateLevel < 0) {
                throw new InvalidOperationException("EndUpdate called more times than BeginUpdate.");
            }
        }
    }
}

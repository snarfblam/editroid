using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class frmChrSelect : Form
    {
        const int RowHeight = 0x10;
        const int TileWidth = 0x10;
        const int TilePerRow = 0x10;
        const int RowWidth = TileWidth * TilePerRow;
        const int RowsPerPage = 0x10;
        const int bytesPerRow = 0x100;

        const int RawTileSize = 8;
        const int RawTileSheetSize = 128;

        

        byte[] tileData;

        public frmChrSelect() {
            InitializeComponent();

            // Adjust width depending on OS theme's scrollbar size
            var width = RowWidth + System.Windows.Forms.SystemInformation.VerticalScrollBarWidth;
            pnlScroller.Width = width;
            this.ClientSize = new Size(width, this.ClientSize.Height);
            this.MinimumSize = new Size(Width, 256);
            this.MaximumSize = new Size(width, int.MaxValue);
        }

        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);

            if (Visible)
                ScrollSelectionIntoView();
        }
        NesPalette _Palette = new NesPalette(new byte[0x10], 0);
        public void SetPalette(NesPalette palette) {
            _Palette = palette ?? _Palette;
        }

        public void SetData(byte[] rom, int dataStart, int rowCount) {
            this.tileData = rom;

            int availableDataSize = rom.Length - dataStart;
            int maxRowCount = availableDataSize / 0x100;

            _RowCount = Math.Min(rowCount, maxRowCount);
            _DataStart = dataStart;
            picTiles.Size = new Size(256, 0x10 * _RowCount);
        }

        byte[] data = new byte[0x100];
        int _DataStart = 0;
        int _RowCount = 0x1;
        int _SelectionRowCount=4 ;
        public int SelectionRowCount {
            get { return _SelectionRowCount; }
            set {
                if (value < 1 || value > 0x10) throw new ArgumentException("Invalid selection size");
                _SelectionRowCount = value;
                picTiles.Invalidate();
            }
        }

        PatternTable gfxLoader = new PatternTable(false);

        private void picTiles_Paint(object sender, PaintEventArgs e) {
            int firstRow = e.ClipRectangle.Top / RowHeight;
            int lastRow = e.ClipRectangle.Bottom / RowHeight;
            if (lastRow >= _RowCount) {
                lastRow = _RowCount - 1;
            }

            int RenderY = firstRow * RowHeight;

            gfxLoader.LoadColors(_Palette, 0);
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            for (int i = firstRow; i <= lastRow; i+= RowsPerPage) {
                // Draw up to 16 rows
                int rowCount = Math.Min(RowsPerPage, lastRow - i + 1);

                int dataStart = i * bytesPerRow + _DataStart;
                int dataEnd = dataStart + bytesPerRow * rowCount - 1;

                if (dataEnd >= tileData.Length) {
                    System.Diagnostics.Debug.Fail("Attempted to draw tiles beyond end of ROM");
                } else {
                    gfxLoader.BeginWrite();
                    gfxLoader.LoadTiles(tileData, dataStart, 0, rowCount * TilePerRow);
                    gfxLoader.EndWrite();

                    Rectangle source = new Rectangle(0, 0, RawTileSheetSize, RawTileSize * rowCount);
                    Rectangle dest = new Rectangle(0, RenderY, RowWidth, RowHeight * rowCount);
                    e.Graphics.DrawImage(gfxLoader.PatternImage, dest, source, GraphicsUnit.Pixel);

                    RenderY += rowCount * RowHeight;
                }
            }

            DrawSelection(e.Graphics);
        }


        SolidBrush _SelectionBrush = new SolidBrush(Color.FromArgb(0x64,SystemColors.Highlight));
        Pen _SelectionPen = new Pen(SystemColors.Highlight, 3);
        private void DrawSelection(Graphics graphics) {
            Rectangle selection = new Rectangle(0, _SelectedRow * RowHeight, RowWidth - 1, _SelectionRowCount * RowHeight);
            graphics.FillRectangle(_SelectionBrush, selection);
            graphics.DrawRectangle(_SelectionPen, selection);
            
        }

        private void picTiles_MouseDown(object sender, MouseEventArgs e) {
            int tileY = e.Y / RowHeight;

            int selectionY = tileY - (tileY % SelectionRowCount);
            _SelectedRow = selectionY;
            picTiles.Invalidate();
        }

        int _SelectedRow = 0;
        public int SelectedOffset {
            get { return _SelectedRow * bytesPerRow + _DataStart; }
            set {
                // Can't be before start of CHR
                if (value < _DataStart) value = _DataStart;

                _SelectedRow = (value - _DataStart) / bytesPerRow;
                ScrollSelectionIntoView();
                picTiles.Invalidate();
            }
        }

        private void ScrollSelectionIntoView() {
            var scroll = new Point(0, _SelectedRow * RowHeight);
            scroll.Y -= pnlScroller.Height / 2;
            scroll.Y += (_SelectionRowCount * RowHeight) / 2;
            if (scroll.Y < 0) scroll.Y = 0;
            if (scroll.Y + pnlScroller.Height > picTiles.Height) scroll.Y = picTiles.Height - pnlScroller.Height;

            pnlScroller.AutoScrollPosition = scroll;
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Close();
            DialogResult = DialogResult.OK;
        }

        private void bntCancel_Click(object sender, EventArgs e) {
            Close();
            DialogResult = DialogResult.Cancel;
        }

        private void picTiles_DoubleClick(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
        }
    }
}

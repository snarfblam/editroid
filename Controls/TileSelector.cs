using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Imaging;

namespace Editroid
{
    class TileSelector : PictureBox
    {

        const int TileSize = 16;

        Label Selection;
        byte[] _Data;


        PatternTable BrinstarTiles = new PatternTable(false);

        public TileSelector() {
            Selection = new Label();
            Selection.Text = string.Empty;
            Selection.Size = new Size(TileSize, TileSize);
            //Selection.Image = Properties.Resources.Select;
            //Selection.BackColor = Color.Transparent;
            Selection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            Selection.BorderStyle = BorderStyle.Fixed3D;

            SizeMode = PictureBoxSizeMode.Zoom;

            this.Controls.Add(Selection);
        }

        ////private void LoadTiles() {
        ////    BrinstarTiles.BeginWrite();
        ////    BrinstarTiles.loa
        ////    BrinstarTiles.EndWrite();


        ////}

        public void SetSeletion(byte index) {
            Selection.Location = new Point(
                (index % 16) * TileSize,
                (index / 16) * TileSize);

            OnTileSelected(index);
        }

        NesPalette queuedPalette; int queuedTable;
        public void SetPalette(NesPalette pal, int table) {
            if (Image == null) {
                queuedPalette = pal;
                queuedTable = table;
                return;
            }

            ColorPalette p = Image.Palette;
            pal.ApplyTable(p.Entries, 0, table * 4, 4);
            Image.Palette = p;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            int TileX = e.X / TileSize;
            int TileY = e.Y / TileSize;

            Selection.Left = TileX * TileSize;
            Selection.Top = TileY * TileSize;

            int Tile = TileX + TileY * 16;
            OnTileSelected(Tile);
        }

        private void OnTileSelected(int Tile) {
            if (TileSelected != null) {
                TileSelected(this, Tile);
            }
        }


        public delegate void IndexEvent(object sender, int index);
        public event IndexEvent TileSelected;

        public void SetData(Byte[] Data) {
            _Data = Data;
            //LoadTiles();
        }

        private bool ShouldSerializeImage() { return false; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Image Image {
            get { return base.Image; }
            set {
                base.Image = value;
                if (queuedPalette != null) {
                    SetPalette(queuedPalette, queuedTable);
                    queuedPalette = null;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe) {
            var oldOffset = pe.Graphics.PixelOffsetMode;
            var oldInterp = pe.Graphics.InterpolationMode;

            pe.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            base.OnPaint(pe);

            pe.Graphics.InterpolationMode = oldInterp;
            pe.Graphics.PixelOffsetMode = oldOffset;
        }
    }

}

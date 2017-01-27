using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    public partial class frmTilePhysics : Form
    {
        Level level;

        Bitmap tiles;

        /// <summary>
        /// If true, the entire rendered tiles image is invalid
        /// </summary>
        bool renderedTilesInvalid;
        /// <summary>
        /// If non-negative, the specified tile number is invalid
        /// </summary>
        int invalidRenderedTile = -1;
        Bitmap renderedTiles;
        Graphics gRenderedTiles;

        Dictionary<Physics, Color> PhysicsColors = new Dictionary<Physics, Color>();

        /// <summary>
        /// Size of tile as drawn onto screen
        /// </summary>
        const int tileSize = 16;

        SolidBrush PhysicsBrush = new SolidBrush(Color.White);
        Pen PhysicsPen = new Pen(Color.White);

        int selectedPhysics = 0xFF;

        byte[] data = new byte[0x100];

        public frmTilePhysics() {
            InitializeComponent();

            renderedTiles = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gRenderedTiles = Graphics.FromImage(renderedTiles);

            PhysicsColors.Add(Physics.Solid, Color.FromArgb(96, Color.White));
            PhysicsColors.Add(Physics.Air, Color.FromArgb(48, Color.Black));
            PhysicsColors.Add(Physics.Breakable, Color.FromArgb(96, Color.Red));
            PhysicsColors.Add(Physics.Door, Color.FromArgb(96,0,255,0));
            PhysicsColors.Add(Physics.DoorHorizontal, Color.FromArgb(96, Color.Yellow));
            PhysicsColors.Add(Physics.DoorBubble, Color.FromArgb(96, Color.Blue));
        }

        void SetData(Level level) {
            this.level = level;

            var patterns = level.CreatePatternBitmap(false);
            this.tiles = patterns.PatternImage;
            SetPalette(false, 0, true);

            renderedTilesInvalid = true;

            Array.Copy(level.Rom.data, level.TilePhysicsTableLocation, data, 0, 0x100);
        }

        void SetPalette(bool alt, int index, bool drawToScreen) {
            var palset = level.BgPalette;
            if (alt) palset = level.BgAltPalette;

            var palette = tiles.Palette;
            palset.ApplyTable(palette.Entries, 0, index * 4, 4);
            tiles.Palette = palette;

            // We will need to redraw the tiles-with-physics image
            renderedTilesInvalid = true;

            if (drawToScreen) picTiles.Invalidate();

        }
        public static void EditTilePhysics(Level level, Form mainForm) {
            using (var form = new frmTilePhysics()) {
                form.SetData(level);
                Program.Dialogs.ShowDialog(form, mainForm);
            }

        }

        private void picTiles_Paint(object sender, PaintEventArgs e) {
            if (renderedTilesInvalid) {
                // Render whole tile sheet
                gRenderedTiles.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gRenderedTiles.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                Rectangle src = new Rectangle(0, 0, 128, 128);
                Rectangle dest = new Rectangle(0, 0, 256, 256);
                gRenderedTiles.DrawImage(tiles, dest, src, GraphicsUnit.Pixel);

                    for (int x = 0; x < 16; x++) {
                        for (int y = 0; y < 16; y++) {
                            PaintTilePhysics(x, y);
                        }
                    }

                renderedTilesInvalid = false;
                invalidRenderedTile = -1;
            } else if (invalidRenderedTile >= 0 && invalidRenderedTile < 256) {
                gRenderedTiles.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gRenderedTiles.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                int x = invalidRenderedTile % 16;
                int y = invalidRenderedTile / 16;
                Rectangle src = new Rectangle(x*8, y*8, 8,8);
                Rectangle dest = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                gRenderedTiles.DrawImage(tiles, dest, src, GraphicsUnit.Pixel);
                
                PaintTilePhysics(x, y);
                invalidRenderedTile = -1;
            }

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.DrawImage(renderedTiles, 0, 0);
        }

        private void PaintTilePhysics(int x, int y) {
            SolidBrush PhysicsBrush = new SolidBrush(Color.Black);

            // Get physics
            int index = x + y * 16;
            byte physicsValue = GetEntry(index);
            var physicsType = level.Rom.GetPhysics(physicsValue);

            // Set brush color
            Color c;
            if (!PhysicsColors.TryGetValue(physicsType, out c))
                c = Color.Transparent;
            PhysicsBrush.Color = c;

            // Draw
            Rectangle tileRect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
            gRenderedTiles.FillRectangle(PhysicsBrush, tileRect);
            tileRect.Width -= 1;
            tileRect.Height -= 1;
            tileRect.Offset(1, 1);
            PhysicsPen.Color = Color.FromArgb(c.R, c.G, c.B);
            gRenderedTiles.DrawRectangle(PhysicsPen, tileRect);
        }

        private void btnReset_Click(object sender, EventArgs e) {
            for (int i = 0; i < 256; i++) {
                SetEntry(i, (byte)i);
            }
            renderedTilesInvalid = true;
            picTiles.Invalidate();
        }

        byte GetEntry(int index) {
            return data[index];
        }
        void SetEntry(int index, byte value) {
            data[index] = value;
        }

        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);


        }

        private void picTiles_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left && e.X >= 0 && e.X < 256 && e.Y >= 0 && e.Y < 256) {
                EditTile(e.X / 16, e.Y / 16);
            } else if (e.Button == MouseButtons.Right) {
                var tileX = e.X / 16;
                var tileY = e.Y / 16;
                int index = tileX + tileY * 16;
                readCustom.Checked = true;
                nudCustom.Value = GetEntry(index);
            }
        }

        private void EditTile(int x, int y) {
            int index = x + y * 16;
            int physics = selectedPhysics;
            if (physics == -1) physics = (int)nudCustom.Value;
            if (physics == -2) physics = x + y * 16;

            SetEntry(index, (byte)physics);
            invalidRenderedTile = index;
            picTiles.Invalidate(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
        }

        private void picTiles_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button ==  MouseButtons.Left && e.X >= 0 && e.X < 256 && e.Y >= 0 && e.Y < 256) {
                EditTile(e.X / 16, e.Y / 16);
            }
        }

        private void btnAccept_Click(object sender, EventArgs e) {
            Array.Copy(data, 0, level.Rom.data, level.TilePhysicsTableLocation, 0x100);
        }

        private void radSolid_CheckedChanged(object sender, EventArgs e) {
            if (radSolid.Checked) selectedPhysics = 0;
        }

        private void radAir_CheckedChanged(object sender, EventArgs e) {
            if (radAir.Checked) selectedPhysics = 0xFF;
        }

        private void radDoorH_CheckedChanged(object sender, EventArgs e) {
            if (radDoorH.Checked) {
                selectedPhysics = 0xA1;
                for (int i = 0xA1; i < 0xFF; i++) {
                    if (level.Rom.GetPhysics((byte)i) == Physics.DoorHorizontal) {
                        selectedPhysics = i;
                        break;
                    }
                }
            }
        }

        private void radDoorV_CheckedChanged(object sender, EventArgs e) {
            if (radDoorV.Checked) selectedPhysics = 0xA0;
        }

        private void radDoorcap_CheckedChanged(object sender, EventArgs e) {
            if (radDoorcap.Checked) selectedPhysics = 0x4E;
        }

        private void radBreakable_CheckedChanged(object sender, EventArgs e) {
            
        }

        private void readCustom_CheckedChanged(object sender, EventArgs e) {
            if (readCustom.Checked) selectedPhysics = -1;
        }

        private void radRevert_CheckedChanged(object sender, EventArgs e) {
            if (radRevert.Checked) selectedPhysics = -2;
        }
    }
}

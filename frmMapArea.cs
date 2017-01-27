using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Editroid
{
    internal partial class frmMapArea:Form
    {
        public frmMapArea() {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            cboLevel.SelectedIndex = 0;
            cboScale.SelectedIndex = 0;
        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { rom = value; }
        }

        private MapControl map;

        /// <summary>
        /// Gets or sets the MapControl object that holds map data. The image from this control will be used also.
        /// </summary>
        public MapControl MapControl {
            get { return map; }
            set {
                map = value;
                MapImage = map.MapImage;
            }
        }

            
        private void btnOK_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
        }


        int startX, startY;
        int endX, endY;
        private void pnlMap_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                startX = e.X / 16;
                startY = e.Y / 16;
                endX = startX;
                endY = startY;

                ShowSelection();
            }
        }


        private void pnlMap_MouseMove(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                endX = e.X / 16;
                endY = e.Y / 16;

                ShowSelection();
            }
        }

        private void ShowSelection() {
            if(startX < 0) startX = 0;
            if(startY < 0) startY = 0;
            if(endX > 31) endX = 31;
            if(endY > 31) endY = 31;

            if(endX < 0) endX = 0;
            if(endY < 0) endY = 0;
            if(startX > 31) startX = 31;
            if(startY > 31) startY = 31;

            Rectangle bounds = lblSelection.Bounds;
            bounds.X = (startX < endX ? startX : endX) * 16;
            bounds.Y = (startY < endY ? startY : endY) * 16;
            bounds.Width = 16 + 16 * (startX > endX ?
                (startX - endX) :
                (endX - startX));
            bounds.Height = 16 + 16 * (startY > endY ?
                (startY - endY) :
                (endY - startY));


            if(!bounds.Equals(lblSelection.Bounds))
                lblSelection.Bounds = bounds;

            long imgSize = (bounds.Width / 16 * bounds.Height / 16) * 0xF000 * 3 / scale;
            Text = "View Map (" + FormatSize(imgSize) + ")";
        }

        private string FormatSize(long imgSize) {
            if(imgSize < 1024) return imgSize.ToString() + " B";
            imgSize /= 1024;
            if(imgSize < 1024) return imgSize.ToString() + " kB";
            imgSize /= 1024;
            if(imgSize < 1024) return imgSize.ToString() + " mB";
            imgSize /= 1024;
            return imgSize.ToString() + " gB";
        }

        private void lblSelection_MouseDown(object sender, MouseEventArgs e) {
            e = new MouseEventArgs(e.Button, e.Clicks, e.X + lblSelection.Left, e.Y + lblSelection.Top, e.Delta);

            pnlMap_MouseDown(sender, e);
        }
        private void lblSelection_MouseMove(object sender, MouseEventArgs e) {
            e = new MouseEventArgs(e.Button, e.Clicks, e.X + lblSelection.Left, e.Y + lblSelection.Top, e.Delta);

            pnlMap_MouseMove(sender, e);
        }

        public Image MapImage {
            get {
                return pnlMap.BackgroundImage;
            }
            set {
                pnlMap.BackgroundImage = value;
            }
        }

        LevelIndex currentLevel;
        MapLevel currentMapLevel;
        private void cboLevel_SelectedIndexChanged(object sender, EventArgs e) {
            switch(cboLevel.SelectedIndex) {
                case 0:
                    currentLevel = LevelIndex.None;
                    currentMapLevel = MapLevel.Blank;
                    break;
                case 1:
                    currentLevel = LevelIndex.Brinstar;
                    currentMapLevel = MapLevel.Brinstar;
                    break;
                case 2:
                    currentLevel = LevelIndex.Norfair;
                    currentMapLevel = MapLevel.Norfair;
                    break;
                case 3:
                    currentLevel = LevelIndex.Ridley;
                    currentMapLevel = MapLevel.Ridley;
                    break;
                case 4:
                    currentLevel = LevelIndex.Kraid;
                    currentMapLevel = MapLevel.Kraid;
                    break;
                case 5:
                    currentLevel = LevelIndex.Tourian;
                    currentMapLevel = MapLevel.Tourian;
                    break;
            }

            SelectLevel();
        }

        private void SelectLevel() {
            if(currentMapLevel == MapLevel.Blank)
                lblSelection.Bounds = new Rectangle(0, 0, 512, 512);
            else {
                startX = 31;
                endX = 0;
                startY = 31;
                endY = 0;

                for(int x = 0; x < 32; x++) {
                    for(int y = 0; y < 32; y++) {
                        if(map.GetLevel(x, y) == currentLevel) {
                            if(x < startX) startX = x;
                            if(x > endX) endX = x;
                            if(y < startY) startY = y;
                            if(y > endY) endY = y;
                        }
                    }
                }

                if(startX > endX) 
                    startX = startY = endX = endY = 0;

                //lblSelection.Bounds = new Rectangle(
                //    startX * 8, startY * 8,
                //    (endX - startX + 1) * 8, (endY - startY + 1) * 8);
                ShowSelection();
            }
        }


        Rectangle selection;
        /// <summary>
        /// Gets a rectangle representing the selected area of the map.
        /// </summary>
        public Rectangle MapSelection {
            get {
                return selection;
            }
        }
        /// <summary>
        /// Gets a value indicating the selected currentLevelIndex filter.
        /// </summary>
        public LevelIndex LevelFilter {
            get {
                return currentLevel;
            }
        }

        private void lblSelection_SizeChanged(object sender, EventArgs e) {
            selection = new Rectangle(
             lblSelection.Left / 16,
             lblSelection.Top / 16,
             lblSelection.Width / 16,
             lblSelection.Height / 16);
        }

        int scale;
        private void cboScale_SelectedIndexChanged(object sender, EventArgs e) {
            scale = (int)Math.Pow(2, cboScale.SelectedIndex);
            ShowSelection();
        }
        /// <summary>
        /// Returns the selected map scale (int the form of its reciprocal: 4 equals one quarter, 2 equals one half).
        /// </summary>
        public int DrawScale { get { return scale; } }


        private bool fillEmptySpots;
        public bool FillEmptySpots {
            get { return fillEmptySpots; }
        }

        private void chkFillEmptySpots_CheckedChanged(object sender, EventArgs e) {
            fillEmptySpots = chkFillEmptySpots.Checked;
        }

        private bool physics;
        public bool ShowPhysics {
            get { return physics; }
        }

        private void chkPhysics_CheckedChanged(object sender, EventArgs e) {
            physics = chkPhysics.Checked;
            //if(physics) 
            //    chkEnemies.Checked = false;
        }


        private bool hideEnemies;
        public bool HideEnemies {
            get { return hideEnemies; }
        }

        private void chkEnemies_CheckedChanged(object sender, EventArgs e) {
            hideEnemies = !chkEnemies.Checked;
            //if(chkEnemies.Checked)
            //    chkPhysics.Checked = false;
        }

        private bool quality = true;
        public bool Quality {
            get { return quality; }
        }

        private void chkQuality_CheckedChanged(object sender, EventArgs e) {
            quality = chkQuality.Checked;
        }

        
    }
}
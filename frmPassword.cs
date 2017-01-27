using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Editroid.ROM;

namespace Editroid
{
    internal partial class frmPassword:Form
    {
        public frmPassword() {
            InitializeComponent();

            gMapImage = Graphics.FromImage(mapImage);
            pnlMap.BackgroundImage = mapImage;
        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set { 
                rom = value;
                DrawMap();
            }
        }

        private void DrawMap() {
            if(map == null || rom == null) return;

            gMapImage.DrawImage(bgImage, new Rectangle(0,0,256,256), new Rectangle(0,0,256,256), GraphicsUnit.Pixel);

            for(int i = 0; i < PasswordData.DataCount; i++) {
                PasswordDatum d = rom.PasswordData.GetDatum(i);
                gMapImage.DrawRectangle(Pens.Yellow, d.MapX * 8, d.MapY * 8, 7, 7);

                lstEntries.Items.Add(d);
            }

            lstEntries.SelectedIndex = 0;
        }

        private MapControl map;
        Bitmap mapImage = new Bitmap(256, 256, PixelFormat.Format24bppRgb);
        Graphics gMapImage;
        Image bgImage;

        public MapControl MapEditor {
            get { return map; }
            set { 
                map = value;
                bgImage = map.MapImage;
                DrawMap();
            }
        }

        private void pnlMap_MouseDown(object sender, MouseEventArgs e) {
            int i = 0;
            bool found = false;
            PasswordDatum d = rom.PasswordData.GetDatum(0); 
            while(i < PasswordData.DataCount & !found){
                 d = rom.PasswordData.GetDatum(i);
                 if(d.MapX == e.X / 8 && d.MapY == e.Y / 8) {
                     lstEntries.SelectedIndex = i;
                 }

                i++;
            }
        }


        bool updatingItem = false;
        private void lstEntries_SelectedIndexChanged(object sender, EventArgs e) {
            if(!updatingItem) {
                PasswordDatum d = (PasswordDatum)lstEntries.SelectedItem;
                lblCurrentItem.SetBounds(d.MapX * 8, d.MapY * 8, 8, 8);
                lstSubtypes.SelectedIndex = d.Item;
            }
        }

        private void lstSubtypes_SelectedIndexChanged(object sender, EventArgs e) {
            if(!updatingItem) {
                PasswordDatum d = ((PasswordDatum)lstEntries.SelectedItem);
                d.Item = lstSubtypes.SelectedIndex;

                UpdateSelectedItem();
            }
        }

        void UpdateSelectedItem() {
            updatingItem = true;

            lstEntries.BeginUpdate();
            int i = lstEntries.SelectedIndex;
            lstEntries.Items[i] = lstEntries.Items[i];
            lstEntries.EndUpdate();
            
            updatingItem = false;
        }


        #region ItemDragging
        bool isDraggingItem;
        int dragStartX;
        int dragStartY;
        private void lblCurrentItem_MouseDown(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                isDraggingItem = true;
                dragStartX = e.X / 8;
                dragStartY = e.Y / 8;
            }
        }

        private void lblCurrentItem_MouseMove(object sender, MouseEventArgs e) {
            if(isDraggingItem) {
                int destX = lblCurrentItem.Left / 8 +
                    (e.X / 8 - dragStartX);
                int destY = lblCurrentItem.Top / 8 +
                    (e.Y / 8 - dragStartY);

                // Account for rounding up instead of down with negatives
                if(e.X < 0) destX--;
                if(e.Y < 0) destY--;

                // Constrain to game map
                if(destX < 0) destX = 0;
                if(destX > 31) destX = 31;
                if(destY < 0) destY = 0;
                if(destY > 31) destY = 31;

                PasswordDatum d = currentDat;
                if(destX != d.MapX || destY != d.MapY) {
                    UndrawDat(d);
                    d.MapX = destX;
                    d.MapY = destY;
                    DrawDat(d);

                    lblCurrentItem.SetBounds(d.MapX * 8, d.MapY * 8, 8, 8);
                    pnlMap.Invalidate();
                    UpdateSelectedItem();

                }

            }
        }


        // Todo: a better solution would be to UndrawDat/DrawDat every
        // time a different object is selected instead of every time
        // the selection is moved.
        private void DrawDat(PasswordDatum d) {
            gMapImage.DrawRectangle(Pens.Yellow, d.MapX * 8, d.MapY * 8, 7, 7);
        }

        private void UndrawDat(PasswordDatum d) {
            Rectangle rect = new Rectangle(d.MapX * 8, d.MapY * 8, 8, 8);

            bool erase = true; // We may not want to erase the rectangle...
            for(int i = 0; i < PasswordData.DataCount; i++) {
                PasswordDatum pd = rom.PasswordData.GetDatum(i);
                // ... if there is a different entry with the same map coords
                if(pd.offset != d.offset && pd.MapX == d.MapX && pd.MapY == d.MapY)
                    erase = false;
            }

            if(erase)
                gMapImage.DrawImage(bgImage, rect, rect, GraphicsUnit.Pixel);
        }

        PasswordDatum currentDat {
            get {
                return (PasswordDatum)lstEntries.SelectedItem;
            }
        }

        private void lblCurrentItem_MouseUp(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                isDraggingItem = false;
            }
        }
        #endregion
    }

}
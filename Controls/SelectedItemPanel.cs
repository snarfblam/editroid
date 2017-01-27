using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;

namespace Editroid
{
    public partial class SelectedItemPanel : UserControl
    {
        public SelectedItemPanel() {
            InitializeComponent();
            PasswordDataToggle.State = true;
        }

        ScreenEditor currentEditor;
        bool slotAvailableForCreepyMusicRoom;


        public void Display(ScreenEditor editor) {
            this.currentEditor = editor;

            // Get state
            bool editableScreenSelected = editor.CanEdit && editor.LevelIndex != LevelIndex.None;
            bool somethingInScreenSelected = editableScreenSelected && editor.SelectedItem != null;
            bool structSelected = somethingInScreenSelected && editor.SelectedItem is StructInstance;
            bool enemySelected = somethingInScreenSelected && editor.SelectedItem is EnemyInstance;
            bool hasPasswordData = editableScreenSelected && editor.Rom.PasswordData.HasDataFor(editor.MapLocation);
            bool hasMusic = editableScreenSelected && editor.Rom.GetLevel(editor.LevelIndex).AlternateMusicRooms.Contians((byte)editor.ScreenIndex);

            // Set visibilities
            IndexLabel.Visible = PaletteLabel.Visible = (enemySelected || structSelected);
            SlotImage.Visible = SlotLabel.Visible = enemySelected;
            LayoutLabel.Visible = FreespaceLabel.Visible = editableScreenSelected;
            musicButton.Visible = editableScreenSelected;
            PasswordDataToggle.Visible = hasPasswordData;


            // Update values
            if (enemySelected)
                SlotLabel.Text = ((EnemyInstance) editor.SelectedItem).SpriteSlot.ToString();
            if (enemySelected || structSelected) {
                int typeIndex = -1;
                int palette = -1;

                if (enemySelected) {
                    typeIndex = ((EnemyInstance)editor.SelectedItem).EnemyType;
                    palette = ((EnemyInstance)editor.SelectedItem).DifficultByteValue;
                }
                if (structSelected) {
                    typeIndex = ((StructInstance)editor.SelectedItem).ObjectType;
                    palette = ((StructInstance)editor.SelectedItem).PalData;
                }

                IndexLabel.Text = FormatHex(typeIndex);
                PaletteLabel.Text = palette.ToString();
            }
            musicButton.State = hasMusic;
            if (editableScreenSelected) {
                slotAvailableForCreepyMusicRoom = editor.Rom.GetLevel(editor.LevelIndex).AlternateMusicRooms.UsedEntryCount < currentEditor.Level.Format.AltMusicRoomCount;
                LayoutLabel.Text = FormatHex(editor.ScreenIndex);
                if (editor.Rom.RomFormat == RomFormats.MMC3) {
                    FreespaceLabel.Text = "~" + FormatHex(Math.Max(0, editor.Rom.GetLevel(editor.LevelIndex).FreeScreenData));
                } else {
                    FreespaceLabel.Text = FormatHex(Math.Max(0, editor.Rom.GetLevel(editor.LevelIndex).FreeScreenData));
                }

                lblMapLocation.Text = FormatHex(editor.MapLocation.X) + ", " + FormatHex(editor.MapLocation.Y);
                lblDefaultPal.Text = editor.Screen.ColorAttributeTable.ToString();
            }
        }

        private string FormatHex(int i) {
            return i.ToString("x").PadLeft(2, '0');
        }

        private void musicButton_TryClick(object sender, CancelEventArgs e) {
            if (!slotAvailableForCreepyMusicRoom && musicButton.State == false)
                e.Cancel = true;
        }

        public event EventHandler AlternateMusicClicked;

        private void musicButton_Click(object sender, EventArgs e) {

        }

        private void musicButton_Toggled(object sender, EventArgs e) {
            if (AlternateMusicClicked != null)
                AlternateMusicClicked(this, EventArgs.Empty);
        }

        private void PasswordDataToggle_Click(object sender, EventArgs e) {
            if (PasswordDataToggle.State == true)
                OnPasswordIconClicked();
        }

        void OnPasswordIconClicked() {
            if (PasswordIconClicked != null)
                PasswordIconClicked(this, EventArgs.Empty);
        }
        public event EventHandler PasswordIconClicked;
        public event EventHandler DefaultPalClicked;

        private void DefaultPalClicker_Click(object sender, EventArgs e) {
            if (DefaultPalClicked != null) {
                DefaultPalClicked(this, EventArgs.Empty);
            }
        }

    }
}

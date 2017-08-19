using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM.Projects;
using Editroid.ROM;

namespace Editroid
{
    public partial class frmChrAnimation : Form
    {
        int _miscUpdateLevel = 0;
        void BeginMiscUIUpdate() {
            _miscUpdateLevel++;
        }
        void EndMiscUIUpdate() {
            _miscUpdateLevel--;
        }

        updateMode _UpdateMode = updateMode.None;
        bool IsUpdatingUI { get { return _UpdateMode != updateMode.None || _miscUpdateLevel != 0; } }

        MetroidRom rom;
        Project project;

        PatternTable sprGfxLoader = new PatternTable(false);
        PatternTable bgGfxLoader = new PatternTable(false);

        Image _BgImage, _SprImage;
        Image BgImage { get { return _BgImage; } set { _BgImage = value; picBg.Invalidate(); } }
        Image SprImage { get { return _SprImage; } set { _SprImage = value; picSpr.Invalidate(); } }

        public bool Animating { get { return tmrAnimate.Enabled; } }
        public frmChrAnimation() {
            InitializeComponent();
        }

        internal void SetRom(MetroidRom rom, Project p) {
            this.rom = rom;
            this.project = p;
            UpdateUI(updateMode.Level);
        }

        private void UpdateUI(updateMode updateMode) {
            this._UpdateMode = updateMode;

            switch (updateMode) {
                case updateMode.Level:
                    DisplaySelectedLevel();
                    break;
                case updateMode.Animation:
                    DisplaySelectedAnimation();
                    break;
                case updateMode.Frame:
                    DisplaySelectedFrame();
                    break;
                case updateMode.BackgroundGfx:
                    RenderSelectedBgFrame();
                    break;
                case updateMode.SpriteGfx:
                    RenderSprites();
                    break;
            }

            this._UpdateMode = updateMode.None;
        }

        static readonly LevelIndex[] levelIndecies = { LevelIndex.None, LevelIndex.Brinstar, LevelIndex.Norfair, LevelIndex.Tourian, LevelIndex.Kraid, LevelIndex.Ridley };
        LevelIndex SelectedLevelIndex { get { return levelIndecies[cboLevel.SelectedIndex]; } }
        ChrAnimationLevelData SelectedLevelData {
            get {
                if (SelectedLevelIndex == LevelIndex.None) return rom.TitleChrAnimationTable;
                return rom.Levels[SelectedLevelIndex].ChrAnimation;
            }
        }
        /// <summary>
        /// Returns null for title screen, otherwise the currently selected level
        /// </summary>
        Level SelectedLevel {
            get {
                if (SelectedLevelIndex == LevelIndex.None) return null;
                return rom.Levels[SelectedLevelIndex];
            }
        }
        ChrAnimationTable SelectedAnimation {
            get {
                if (SelectedLevelIndex == LevelIndex.None) return rom.TitleChrAnimationTable.Animations[selectedAnimationIndex];
                return SelectedLevelData.Animations[selectedAnimationIndex];
            }
        }
        ChrAnimationFrame SelectedAnimationFrame {
            get { return SelectedAnimation.Frames[lstFrames.SelectedIndex]; }
            set {
                SelectedAnimation.Frames[lstFrames.SelectedIndex] = value;
            }
        }

        private void DisplaySelectedLevel() {
            if (cboLevel.SelectedIndex < 0) {
                cboLevel.SelectedIndex = 0;
            }

            //var level = SelectedLevel;

            cboAnimation.BeginUpdate();
            {
                cboAnimation.Items.Clear();
                var animations = SelectedLevelData.Animations;
                for (int i = 0; i < animations.Count; i++) {
                    cboAnimation.Items.Add(GetAnimationListItemName(i, animations[i]));
                }

                //if (project != null) {
                //    cboAnimation.Items.Add(NewAnimationComboboxItem);
                //}

            }
            cboAnimation.EndUpdate();
            var selectedLevelIndex = SelectedLevelIndex;
            nudSpr0.Value = rom.ChrUsage.MMC3_GetSpr0(selectedLevelIndex);
            nudSpr1.Value = rom.ChrUsage.MMC3_GetSpr1(selectedLevelIndex);

            UpdateJustinBaileyLabel();

            if (cboAnimation.Items.Count > 0) {
                cboAnimation.SelectedIndex = 0;
            }
            DisplaySelectedAnimation();
            RenderSprites();
        }

        private static string GetAnimationListItemName(int i, ChrAnimationTable animation) {
            bool isDefaultAnimation = i == 0;
            string name = i.ToString() + " - " + animation.Name;
            if (isDefaultAnimation && string.IsNullOrEmpty(animation.Name)) name += "(default)";
            return name;
        }

        private void DisplaySelectedAnimation() {
            var animation = SelectedAnimation;

            lstFrames.BeginUpdate();
            
            lstFrames.Items.Clear();

            for (int i = 0; i < animation.Frames.Count; i++) {
                lstFrames.Items.Add("Frame " + i.ToString());
            }

            lstFrames.SelectedIndex = 0;

            lstFrames.EndUpdate();

            txtAnimationName.Text = animation.Name;
            if (cboAnimation.SelectedIndex == 0 && string.IsNullOrEmpty(animation.Name))
                txtAnimationName.Text = "(Default)";
            txtAnimationName.Enabled = project != null;
            btnDeleteAnimation.Enabled = (project != null && cboAnimation.Items.Count > 1);

            DisplaySelectedFrame();
        }
        private void DisplaySelectedFrame() {
            var frame = SelectedAnimationFrame;

            nudBg0.Value = frame.Bank0;
            nudBg1.Value = frame.Bank1;
            nudBg2.Value = frame.Bank2;
            nudBg3.Value = frame.Bank3;
            nudFrametime.Value = Math.Max(nudFrametime.Minimum, frame.FrameTime);

            RenderSelectedBgFrame();
            btnRemoveFrame.Enabled = SelectedAnimation.Frames.Count > 1;

            UpdateFreeSpace();
        }

        private void UpdateFreeSpace() {
            int usedMem = CalculateUsedFrames();
            int freeMem = 255 - usedMem;

            lblFreeSpace.Text = "Free space: " + freeMem.ToString() + " frames";
            btnAddAnimation.Enabled = project != null && freeMem > 0;
            btnAddFrame.Enabled = freeMem > 0;
        }

        private int CalculateUsedFrames() {
            int result = 0;

            
            foreach (var level in rom.Levels.Values) {
                for (int i = 0; i < level.ChrAnimation.Animations.Count; i++) {
                    result += level.ChrAnimation.Animations[i].Frames.Count;
                }
            }
            for (int i = 0; i < rom.TitleChrAnimationTable.Animations.Count; i++) {
                result += rom.TitleChrAnimationTable.Animations[i].Frames.Count;
            }

            return result;
        }

        const int chrRomOffset = 0x10 + 0x4000 * 0x20;
        const int chrBankSize = 0x400;

        private void RenderSelectedBgFrame() {
            if (Animating) return;
            var frame = SelectedAnimationFrame;
            RenderBgFrame(frame);
        }
        private void RenderSprites() {
            UpdateJustinBaileyLabel();

            sprGfxLoader.BeginWrite();

            int justinBaileyAdjust = chkJustinBailey.Checked ? 4 : 0;
            LoadGfxSlot(sprGfxLoader, 0, justinBaileyAdjust + SelectedLevelData.SprBank0 & 0xFE);
            LoadGfxSlot(sprGfxLoader, 1, 1 + justinBaileyAdjust + (SelectedLevelData.SprBank0 & 0xFE));
            LoadGfxSlot(sprGfxLoader, 2, SelectedLevelData.SprBank1 & 0xFE);
            LoadGfxSlot(sprGfxLoader, 3, 1 + (SelectedLevelData.SprBank1 & 0xFE));

            sprGfxLoader.EndWrite();

            sprGfxLoader.LoadColors((SelectedLevel ?? rom.Brinstar).SpritePalette, 0);
            SprImage = sprGfxLoader.PatternImage;
        }


        private void RenderBgFrame(ChrAnimationFrame frame) {

            bgGfxLoader.BeginWrite();

            LoadGfxSlot(bgGfxLoader, 0, frame.Bank0);
            LoadGfxSlot(bgGfxLoader, 1, frame.Bank1);
            LoadGfxSlot(bgGfxLoader, 2, frame.Bank2);
            LoadGfxSlot(bgGfxLoader, 3, frame.Bank3);

            bgGfxLoader.EndWrite();

            bgGfxLoader.LoadColors((SelectedLevel ?? rom.Brinstar).BgPalette, 0);
            BgImage = bgGfxLoader.PatternImage;
        }

        private void LoadGfxSlot(PatternTable gfx, int slot, int bank) {
            int offset = bank * chrBankSize + chrRomOffset;
            gfx.LoadTiles(rom.data, offset, slot * 0x40, 0x40);
        }

        private void UpdateJustinBaileyLabel() {
            lblJustinBailey.Text = "Justin Bailey Bank: " + (4 + (int)nudSpr0.Value).ToString("X2");
        }

        enum updateMode
        {
            /// <summary>Indicates that no updates are occuring</summary>
            None,
            /// <summary>Indicates that level information, animation data and frame data are all being updated.</summary>
            Level,
            /// <summary>Indicates that animation data and frame data are being updated</summary>
            Animation,
            /// <summary>Indicates that frame data is being updated.</summary>
            Frame,
            /// <summary>Indicates that the rendered background tiles are being updated.</summary>
            BackgroundGfx,
            /// <summary>Indicates that the rendered sprite tiles are being updated.</summary>
            SpriteGfx,

        }

        object NewAnimationComboboxItem = "New...";

        private void cboLevel_SelectedIndexChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                UpdateUI(updateMode.Level);
            }
        }

        private void picBg_Paint(object sender, PaintEventArgs e) {
            DrawTileSheet(picBg, BgImage, e.Graphics);
        }

        private void picSpr_Paint(object sender, PaintEventArgs e) {
            DrawTileSheet(picSpr, SprImage, e.Graphics);
        }

        private static void DrawTileSheet(PictureBox control, Image image, Graphics graphics) {
            if (image == null) return;

            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            Rectangle src = new Rectangle(0, 0, 128, 128);
            Rectangle dest = new Rectangle(0, 0, control.Width, control.Height);
            graphics.DrawImage(image, dest, src, GraphicsUnit.Pixel);
        }

        int selectedAnimationIndex = 0;

        private void cboAnimation_SelectedIndexChanged(object sender, EventArgs e) {
            selectedAnimationIndex = cboAnimation.SelectedIndex;

            if (!IsUpdatingUI && cboAnimation.SelectedIndex >= 0) {
                if (cboAnimation.SelectedItem == NewAnimationComboboxItem) {
                    if (CalculateUsedFrames() >= 255) {
                        // Can't create new animation if there isn't enough space
                        BeginMiscUIUpdate();
                        cboAnimation.SelectedIndex = cboAnimation.Items.Count - 2;
                        EndMiscUIUpdate();
                    } else {
                        int newIndex = SelectedLevelData.Animations.Count;

                        // Update data
                        ChrAnimationFrame newFrame = new ChrAnimationFrame();
                        newFrame.FrameTime = 0x10;
                        ChrAnimationTable newAnimation = new ChrAnimationTable();
                        newAnimation.Frames.Add(newFrame);
                        SelectedLevelData.Animations.Add(newAnimation);

                        // Update UI
                        BeginMiscUIUpdate();
                        cboAnimation.Items.Insert(newIndex, GetAnimationListItemName(newIndex, newAnimation));
                        cboAnimation.SelectedIndex = newIndex;
                        EndMiscUIUpdate();

                        UpdateUI(updateMode.Animation);
                    }
                }
                UpdateUI(updateMode.Animation);

            }
        }

        private void CreateNewAnimation() {
            if (CalculateUsedFrames() >= 255) {
                // Can't create new animation if there isn't enough space
                ////BeginMiscUIUpdate();
                ////cboAnimation.SelectedIndex = cboAnimation.Items.Count - 2;
                ////EndMiscUIUpdate();
                
                // do nothing
            } else {
                int newIndex = SelectedLevelData.Animations.Count;

                // Update data
                ChrAnimationFrame newFrame = new ChrAnimationFrame();
                newFrame.FrameTime = 0x10;
                ChrAnimationTable newAnimation = new ChrAnimationTable();
                newAnimation.Frames.Add(newFrame);
                SelectedLevelData.Animations.Add(newAnimation);

                // Update UI
                BeginMiscUIUpdate();
                cboAnimation.Items.Insert(newIndex, GetAnimationListItemName(newIndex, newAnimation));
                cboAnimation.SelectedIndex = newIndex;
                EndMiscUIUpdate();

                UpdateUI(updateMode.Animation);
            }
        }


        private void lstFrames_SelectedIndexChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                UpdateUI(updateMode.Frame);
            }
        }

        private void nudBg0_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                ApplySelectedBgBanks();
            }
        }

        private void nudBg1_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                ApplySelectedBgBanks();
            }
        }

        private void nudBg2_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                ApplySelectedBgBanks();
            }
        }

        private void nudBg3_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                ApplySelectedBgBanks();
            }
        }


        private void ApplySelectedBgBanks() {
            var frameData = SelectedAnimationFrame;
            frameData.Bank0 = (byte)nudBg0.Value;
            frameData.Bank1 = (byte)nudBg1.Value;
            frameData.Bank2 = (byte)nudBg2.Value;
            frameData.Bank3 = (byte)nudBg3.Value;
            SelectedAnimationFrame = frameData;

            UpdateUI(updateMode.BackgroundGfx);
        }

        private void nudFrametime_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                var frameData = SelectedAnimationFrame;
                frameData.FrameTime = 0x7F & (int)nudFrametime.Value;
                SelectedAnimationFrame = frameData;
            }
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            tmrAnimate.Stop();
        }

        private void chkAnimate_CheckedChanged(object sender, EventArgs e) {
            tmrAnimate.Enabled = chkAnimate.Checked;
            if (chkAnimate.Checked == false) {
                // Update displayed tiles to selected frame if animation is stopped
                UpdateUI(updateMode.BackgroundGfx);
            }
        }

        int animationFrame = 0;
        private void tmrAnimate_Tick(object sender, EventArgs e) {
            if (IsUpdatingUI) return;

            animationFrame++;
            if (animationFrame >= SelectedAnimation.Frames.Count) animationFrame = 0;

            RenderBgFrame(SelectedAnimation.Frames[animationFrame]);
        }

        private void btnAddFrame_Click(object sender, EventArgs e) {
            var frameToClone = SelectedAnimationFrame;
            // Update animation data
            SelectedAnimation.Frames.Add(frameToClone);
            // Update UI
            lstFrames.Items.Add("Frame " + lstFrames.Items.Count.ToString());
            // Select new frame
            lstFrames.SelectedIndex = lstFrames.Items.Count - 1;
        }

        private void btnRemoveFrame_Click(object sender, EventArgs e) {
            if (lstFrames.Items.Count > 1) {
                // Update animation data
                SelectedAnimation.Frames.RemoveAt(lstFrames.SelectedIndex);
                // Update ui
                ////BeginMiscUIUpdate();
                ////lstFrames.Items.RemoveAt(lstFrames.SelectedIndex);
                ////lstFrames.SelectedIndex = 0;
                ////EndMiscUIUpdate();

                ////UpdateUI(updateMode.Frame);
                UpdateUI(updateMode.Animation);
            }
        }

        private void txtAnimationName_TextChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                SelectedAnimation.Name = txtAnimationName.Text;
                var newItemText = GetAnimationListItemName(cboAnimation.SelectedIndex, SelectedAnimation);
                cboAnimation.Items[cboAnimation.SelectedIndex] = newItemText;
            }
        }

        private void chkJustinBailey_CheckedChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                UpdateUI(updateMode.SpriteGfx);
            }
        }

        private void nudSpr0_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                SelectedLevelData.SprBank0 = (byte)nudSpr0.Value;
                UpdateUI(updateMode.SpriteGfx);
            }
        }

        private void nudSpr1_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdatingUI) {
                SelectedLevelData.SprBank1 = (byte)nudSpr1.Value;
                UpdateUI(updateMode.SpriteGfx);
            }
        }



        private void PickChr(bool forSprite, NumericUpDown control) {

            using (var picker = new frmChrSelect()) {
                picker.SetData(rom.data, 0x80010, 0x400);

                if (forSprite) {
                    picker.SetPalette((SelectedLevel ?? rom.Brinstar).SpritePalette);
                    picker.SelectionRowCount = 8;
                } else {
                    picker.SetPalette((SelectedLevel ?? rom.Brinstar).BgPalette);
                    picker.SelectionRowCount = 4;
                }

                var selectedBank = (int)control.Value;
                if (forSprite) selectedBank &= (~1);
                picker.SelectedOffset =  selectedBank * 0x400 + 0x80010;

                if (Program.Dialogs.ShowDialog(picker,this) == DialogResult.OK) {
                    control.Value = (picker.SelectedOffset - 0x80010) / 0x400;
                }
            }
        }

        private void btnSpr0_Click(object sender, EventArgs e) {
            PickChr(true, nudSpr0);
        }

        private void btnSpr1_Click(object sender, EventArgs e) {
            PickChr(true, nudSpr1);
        }

        private void btnBg0_Click(object sender, EventArgs e) {
            PickChr(false, nudBg0);
        }

        private void btnBg1_Click(object sender, EventArgs e) {
            PickChr(false, nudBg1);
        }

        private void btnBg2_Click(object sender, EventArgs e) {
            PickChr(false, nudBg2);
        }

        private void btnBg3_Click(object sender, EventArgs e) {
            PickChr(false, nudBg3);
        }

        private void btnDeleteAnimation_Click(object sender, EventArgs e) {
            if (cboAnimation.SelectedItem == NewAnimationComboboxItem) return;
            if (SelectedLevelData.Animations.Count < 2) return;

            SelectedLevelData.Animations.RemoveAt(cboAnimation.SelectedIndex);
            UpdateUI(updateMode.Level);
        }

        private void btnOK_Click(object sender, EventArgs e) {
            ValidateChrNames();
            Close();
        }

        private void ValidateChrNames() {
            string invalidName = string.Empty;
            int invalidNameCount = 0;

            for (int i = 0; i < levelIndecies.Length; i++) {
                var lvlData = rom.ChrAnimationData[levelIndecies[i]];
                for (int iAnim = 0; iAnim < lvlData.Animations.Count; iAnim++) {
                    var name = lvlData.Animations[iAnim].Name;
                    if (!IsAnimNameValid(name)) {
                        if (!string.IsNullOrEmpty(name)) { // Empty name field allowed
                            if (string.IsNullOrEmpty(invalidName)) invalidName = name; // Remember first invalid name
                            invalidNameCount++;
                        }
                    }
                }
            }

            if (invalidNameCount > 0) {
                if (invalidName.Length == 0) invalidName = "(empty)";
                if (invalidNameCount > 1) invalidName += " and " + (invalidNameCount - 1).ToString() + " other(s)";
                MessageBox.Show("Warning: invalid CHR animation names may cause assembler errors -- " + invalidName, "Invalid Names", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static bool IsAnimNameValid(string name) {
            // Can't be empty
            if (name == null || name.Length == 0) return false;
            // Can't start with digit
            if (name[0] != '_' & !char.IsLetter(name[0])) return false;
            // Can only contain alphanumeric and _
            for (int i = 1; i < name.Length; i++) {
                if (name[i] != '_' & !char.IsLetterOrDigit(name[i])) return false;
            }
            return true;
        }

        private void btnAddAnimation_Click(object sender, EventArgs e) {
            CreateNewAnimation();
        }

    }
}

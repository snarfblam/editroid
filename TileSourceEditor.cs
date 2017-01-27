using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.ROM;

namespace Editroid
{
    public partial class TileSourceEditor : Form
    {
        public TileSourceEditor() {
            InitializeComponent();
        }

        private MetroidRom rom;

        public MetroidRom Rom {
            get { return rom; }
            set {
                rom = value;

                LoadingData = true;
                if (rom != null)
                    LoadRom();
                LoadingData = false;
            }
        }

        static readonly string SpriteString = "Sprite";
        static readonly string BgString = "Background";

        private void LoadRom() {
            LoadPatternGroups();

            PatternGroupsList.SelectedIndices.Clear();
            PatternGroupsList.SelectedIndices.Add(0);

            LoadLevelGroups();
        }

        /// <summary>Loads pattern groups from the rom.</summary>
        private void LoadPatternGroups() {
            PatternGroupsList.Items.Clear();

            PatternGroupsList.BeginUpdate();
            for (int i = 0; i < rom.PatternGroupOffsets.Count; i++) {
                PatternGroupOffsets o = rom.PatternGroupOffsets[i];
                ListViewItem item = new ListViewItem(new string[]{
                    i.ToString("X"),
                    o.SourceRomOffset.ToString("X"),
                    o.DestTileIndex.ToString("X"),
                    o.TileCount.ToString("X"),
                    o.IsPage0 ? SpriteString : BgString
                });

                PatternGroupsList.Items.Add(item);
            }
            PatternGroupsList.EndUpdate();
        }



        LevelIndex _selectedLevel = LevelIndex.None;
        LevelIndex SelectedLevel {
            get { return _selectedLevel; }
            set {
                GetLevelButton(_selectedLevel).Checked = false;
                _selectedLevel = value;
                GetLevelButton(_selectedLevel).Checked = true;

                LoadLevelGroups();
            }
        }

        PatternGroupIndexTable CurrentPatternGroupTable {
            get {
                if (_selectedLevel == LevelIndex.None)
                    return rom.GlobalPatternGroups;
                return rom.Levels[_selectedLevel].PatternGroups;
            }
        }

        private void LoadLevelGroups() {
            PatternGroupIndexTableList.Items.Clear();
            PatternGroupIndexTable groups = GetLevelPatterns(SelectedLevel);
            for (int i = 0; i < groups.Count; i++) {
                //PatternGroupOffsets offsets = rom.PatternGroupOffsets[groups[i]];

                PatternGroupIndexTableList.Items.Add(groups[i].ToString("X"));
                
            }

            //PatternGroupIndexTableList.SelectedIndex = 0;
            DrawLevelPatterns();
        }

        PatternTable spritePatterns = new PatternTable(false);
        PatternTable bgPatterns = new PatternTable(false);
        private void DrawLevelPatterns() {
            DrawLevelPatterns(SelectedLevel);
        }

        private void DrawLevelPatterns(LevelIndex level) {
            spritePatterns.Clear();
            bgPatterns.Clear();

            PatternGroupIndexTable groups = GetLevelPatterns(level);

            spritePatterns.BeginWrite();
            bgPatterns.BeginWrite();
            for (int i = 0; i < groups.Count; i++) {
                PatternGroupOffsets offsets = rom.PatternGroupOffsets[groups[i]];

                if (offsets.IsPage0) {
                    spritePatterns.LoadTiles(offsets);
                } else {
                    bgPatterns.LoadTiles(offsets);
                }
            }
            spritePatterns.EndWrite();
            bgPatterns.EndWrite();

            ApplyPalette(level);

            SpritePatternBox.Image = spritePatterns.PatternImage;
            BackgroundPatternBox.Image = bgPatterns.PatternImage;
        }

        private void ApplyPalette(LevelIndex level) {
            NesPalette bgPal = rom.Levels[LevelIndex.Brinstar].BgPalette;
            NesPalette sprPal = rom.Levels[LevelIndex.Brinstar].SpritePalette;
            if (level != LevelIndex.None) {
                bgPal = rom.Levels[level].BgPalette;
                sprPal = rom.Levels[level].SpritePalette;
            }

            spritePatterns.LoadColors(sprPal, 2);
            bgPatterns.LoadColors(bgPal, 1);
        }

        PatternGroupIndexTable GetLevelPatterns(LevelIndex index) {
            switch (index) {
                case LevelIndex.Brinstar:
                    return rom.Brinstar.PatternGroups;
                case LevelIndex.Norfair:
                    return rom.Norfair.PatternGroups;
                case LevelIndex.Tourian:
                    return rom.Tourian.PatternGroups;
                case LevelIndex.Kraid:
                    return rom.Kraid.PatternGroups;
                case LevelIndex.Ridley:
                    return rom.Ridley.PatternGroups;
                case LevelIndex.None:
                default:
                    return rom.GlobalPatternGroups;
            }
        }

        ToolStripButton GetLevelButton(LevelIndex level) {
            switch (level) {
                case LevelIndex.Brinstar:
                    return btnBrinstar;
                case LevelIndex.Norfair:
                    return btnNorfair;
                case LevelIndex.Tourian:
                    return btnTourian;
                case LevelIndex.Kraid:
                    return btnKraid;
                case LevelIndex.Ridley:
                    return btnRidley;
                case LevelIndex.None:
                default:
                    return btnGlobal;
            }
        }

        private void btnGlobal_Click(object sender, EventArgs e) { SelectedLevel = LevelIndex.None; }
        private void btnBrinstar_Click(object sender, EventArgs e) { SelectedLevel = LevelIndex.Brinstar; }
        private void btnNorfair_Click(object sender, EventArgs e) { SelectedLevel = LevelIndex.Norfair; }
        private void btnRidley_Click(object sender, EventArgs e) { SelectedLevel = LevelIndex.Ridley; }
        private void btnKraid_Click(object sender, EventArgs e) { SelectedLevel = LevelIndex.Kraid; }
        private void btnTourian_Click(object sender, EventArgs e) { SelectedLevel = LevelIndex.Tourian; }




        


        private void LevelGroupsList_SelectedIndexChanged(object sender, EventArgs e) {
            if (!LoadingData) {
                LoadingData = true;

                LoadSelectedGroupIndex();

                LoadingData = false;
            }
        }

        private void PatternGroupsList_SelectedIndexChanged(object sender, EventArgs e) {
            if (!LoadingData) {
                LoadingData = true;
                if (PatternGroupsList.SelectedIndices.Count > 0) {
                    // Use this group in the level's pattern group table
                    int tableIndex = PatternGroupIndexTableList.SelectedIndex;
                    if (tableIndex >= 0) {
                        var table = GetLevelPatterns(_selectedLevel);
                        int newPatternGroupIndex = PatternGroupsList.SelectedIndices[0];
                        table[tableIndex] = (byte)newPatternGroupIndex;

                        ReloadLevelValues();
                    }

                    LoadSelectedGroupsValues();
                }

                LoadingData = false;
            }
        }

        public int SelectedGroupIndex { get {
                if (PatternGroupsList.SelectedIndices.Count < 1)
                    return -1;
                return PatternGroupsList.SelectedIndices[0];
            }
        }

        bool LoadingData { get; set; }

        /// <summary>Loads and draws pattern-group-index-table for selected level, and shows the defauls-selected pattern group.</summary>
        void LoadLevelData() {
            PatternGroupIndexTableList.Items.Clear();
            PatternGroupIndexTable groups = GetLevelPatterns(SelectedLevel);
            for (int i = 0; i < groups.Count; i++) {
                PatternGroupIndexTableList.Items.Add(groups[i].ToString("X"));
            }

            PatternGroupIndexTableList.SelectedIndex = 1;
            LoadSelectedGroupIndex();
            DrawLevelPatterns();
        }
        /// <summary>Loads and draws pattern-group-index-table for selected, ALREADY LOADED level, but does not update the pattern group list or values.
        /// Call this method when the user selects an item in the PatternGroupList.</summary>
        void ReloadLevelValues() {
            PatternGroupIndexTable groups = GetLevelPatterns(SelectedLevel);
            if (groups.Count != PatternGroupIndexTableList.Items.Count) throw new InvalidOperationException("An invalid call to ReloadLevelValues was made it TileSoucreEditor. The number of items present did not match the number of items to load.");

            for (int i = 0; i < groups.Count; i++) {
                PatternGroupIndexTableList.Items[i] = groups[i].ToString("X");
            }

            DrawLevelPatterns();
        }

        /// <summary>Selects the pattern group associated with the selected parrern-group-index-table entry.</summary>
        void LoadSelectedGroupIndex() {
            if (PatternGroupIndexTableList.SelectedIndex != -1) {
                PatternGroupsList.SelectedItems.Clear();
                int selectionIndex = int.Parse(PatternGroupIndexTableList.SelectedItem.ToString(), System.Globalization.NumberStyles.HexNumber);
                PatternGroupsList.SelectedIndices.Add(selectionIndex);
                PatternGroupsList.EnsureVisible(selectionIndex);
                LoadSelectedGroupsValues();
            }
        }

        /// <summary>Loads the selected pattern group's parameters into edit boxes.</summary>
        void LoadSelectedGroupsValues() {
            if (PatternGroupsList.SelectedItems.Count > 0) {
                var selectedItem = PatternGroupsList.SelectedItems[0];
                SourceBox.Text = selectedItem.SubItems[1].Text;
                DestBox.Text = selectedItem.SubItems[2].Text;
                LenBox.Text = selectedItem.SubItems[3].Text;
                TypeBox.SelectedIndex = rom.PatternGroupOffsets[PatternGroupsList.SelectedIndices[0]].IsPage0 ? 0 : 1;
            }
        }

        void ReloadSelectedPatternGroupListItem() {
            if (PatternGroupsList.SelectedIndices.Count > 0) {

                PatternGroupsList.BeginUpdate();

                int index = PatternGroupsList.SelectedIndices[0];
                var item = PatternGroupsList.SelectedItems[0];
                var patternGroupData = rom.PatternGroupOffsets[index];

                item.SubItems[1].Text = patternGroupData.SourceRomOffset.ToString("X");
                item.SubItems[2].Text = patternGroupData.DestTileIndex.ToString("X");
                item.SubItems[3].Text = patternGroupData.TileCount.ToString("X");
                item.SubItems[4].Text = patternGroupData.IsPage0 ? SpriteString : BgString;
                PatternGroupsList.EndUpdate();
            }
        }

        private void SourceBox_TextChanged(object sender, EventArgs e) {
        }

        private void SourceBox_Leave(object sender, EventArgs e) {
            if (!LoadingData) {
                LoadingData = true;

                rom.PatternGroupOffsets[SelectedGroupIndex].SourceRomOffset = SourceBox.Value;
                ReloadSelectedPatternGroupListItem();
                DrawLevelPatterns();

                LoadingData = false;
            }

        }

        private void DestBox_Leave(object sender, EventArgs e) {
            if (!LoadingData) {
                LoadingData = true;

                rom.PatternGroupOffsets[SelectedGroupIndex].DestTileIndex = DestBox.Value;
                ReloadSelectedPatternGroupListItem();
                DrawLevelPatterns();

                LoadingData = false;
            }
        }

        private void LenBox_Leave(object sender, EventArgs e) {
            if (!LoadingData) {
                LoadingData = true;

                rom.PatternGroupOffsets[SelectedGroupIndex].TileCount = LenBox.Value;
                ReloadSelectedPatternGroupListItem();
                DrawLevelPatterns();

                LoadingData = false;
            }
        }

        private void TypeBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (!LoadingData) {
                LoadingData = true;

                rom.PatternGroupOffsets[SelectedGroupIndex].IsPage0 = TypeBox.SelectedIndex != 0;
                ReloadSelectedPatternGroupListItem();
                DrawLevelPatterns();

                LoadingData = false;
            }
        }

    }

}

 

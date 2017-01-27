using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Editroid.UndoRedo;
using Editroid.Actions;

namespace Editroid
{
	/// <summary>
	/// Allows editing of currentLevelIndex and sprite palettes in a Metroid ROM.
	/// </summary>
	internal partial class frmPalette:Form
	{
		/// <summary>
		/// Instantiates this component.
		/// </summary>
		public frmPalette() {
			InitializeComponent();
            bgPal.PalType = PaletteType.Background;
            bgPal2.PalType = PaletteType.AltBackground;
            spritePal.PalType = PaletteType.Sprite;
            spritePal2.PalType = PaletteType.AltSprite;
		}
        
        private Level levelData;
        public Level LevelData {
            get { return levelData; }
            set {
                if (levelData == value) return;

                levelData = value;

                bgPal.LevelData = levelData;
                bgPal.LoadLevelData();

                bgPal2.LevelData = levelData;
                bgPal2.LoadLevelData();

                spritePal.LevelData = levelData;
                spritePal.LoadLevelData();

                spritePal2.LevelData = levelData;
                spritePal2.LoadLevelData();

                bgPal2.Visible = levelData.Format.SupportsAltBgPalette;
                spritePal2.Visible = levelData.Format.SupportsAltSpritePalette;


            }
        }

        private void colorSelector1_ColorSelected(object sender, EventArgs e) {
            bgPal2.SelectedColor = bgPal.SelectedColor = spritePal.SelectedColor = spritePal2.SelectedColor = colorPicker.Selection;
        }

        public ActionGenerator Actions { get { return ((frmMain)Owner).Actions; } }


        internal void NotifyAction(EditroidAction a) {
            const int normalPaletteMacroIndex = 0;
            const int altPaletteMacroIndex = 5;

            var advancedPalAction = a as AdvancedPaletteEdit;
            if (advancedPalAction != null) {
                if (advancedPalAction.Level == this.levelData) {
                    bool affectsNormalPals = false;
                    bool affectsAltPal = false;
                    for (int i = 0; i < advancedPalAction.Edits.Count; i++) {
                        int palIndex = advancedPalAction.Edits[i].ppuMacroIndex;
                        if (palIndex == normalPaletteMacroIndex) affectsNormalPals = true;
                        if (palIndex == altPaletteMacroIndex) affectsAltPal = true;
                    }

                    if (affectsNormalPals) {
                        spritePal.RedrawAll();
                        bgPal.RedrawAll();
                    }
                    if (affectsAltPal) {
                        bgPal2.RedrawAll();
                        spritePal2.RedrawAll();
                    }
                }
            }

            var palAction = a as SetPaletteColor;
            if (palAction == null) return;

            if (palAction.AffectedLevel != levelData.Index)
                return;
            switch (palAction.Type) {
                case PaletteType.Background:
                    bgPal.RedrawColor(palAction.PaletteIndex, palAction.EntryIndex);
                    break;
                case PaletteType.AltBackground:
                    bgPal2.RedrawColor(palAction.PaletteIndex, palAction.EntryIndex);
                    break;
                case PaletteType.Sprite:
                    spritePal.RedrawColor(palAction.PaletteIndex, palAction.EntryIndex);
                    break;
                case PaletteType.AltSprite:
                    spritePal2.RedrawColor(palAction.PaletteIndex, palAction.EntryIndex);
                    break;
                case PaletteType.ZeroEntry:
                    spritePal2.RedrawColor(0, 0);
                    spritePal2.RedrawColor(1, 0);
                    spritePal2.RedrawColor(2, 0);
                    spritePal2.RedrawColor(3, 0);
                    spritePal.RedrawColor(0, 0);
                    spritePal.RedrawColor(1, 0);
                    spritePal.RedrawColor(2, 0);
                    spritePal.RedrawColor(3, 0);
                    bgPal.RedrawColor(0, 0);
                    bgPal.RedrawColor(1, 0);
                    bgPal.RedrawColor(2, 0);
                    bgPal.RedrawColor(3, 0);
                    bgPal2.RedrawColor(0, 0);
                    bgPal2.RedrawColor(1, 0);
                    bgPal2.RedrawColor(2, 0);
                    bgPal2.RedrawColor(3, 0);

                    break;
            }
        }

        internal void SelectColor(int p) {
            colorPicker.Selection = p;
            bgPal.SelectedColor = bgPal2.SelectedColor = spritePal.SelectedColor = spritePal2.SelectedColor = p;
        }
    }
}
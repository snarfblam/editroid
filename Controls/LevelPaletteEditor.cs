using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Editroid.Controls;

namespace Editroid
{
    class LevelPaletteEditor:PalControlBase
    {
        private Level levelData;
        private NesPalette palData;

        public Level LevelData {
            get { return levelData; }
            set { levelData = value;
                
            }
        }

        ////Bitmap bg = new Bitmap(64, 64, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        ////Graphics gbg;
        TextureBrush overlay = new TextureBrush(Editroid.Properties.Resources.PaletteOverlay);
        public LevelPaletteEditor() {
            base.SetGridSize(new Size(4, 4));
            ////base.BackgroundImage = bg;
            ////gbg = Graphics.FromImage(bg);
        }


        private PaletteType palType;

        [Browsable(false)]
        public PaletteType PalType {
            get { return palType; }
            set {
                if (!Enum.IsDefined(typeof(PaletteType), value))
                    throw new ArgumentException("Invalid palette type.");

                palType = value;
            }
        }

        public void LoadLevelData() {
            switch (palType) {
                case PaletteType.Background:
                    palData = levelData.BgPalette;
                    break;
                case PaletteType.AltBackground:
                    palData = levelData.BgAltPalette;
                    break;
                case PaletteType.Sprite:
                    palData = levelData.SpritePalette;
                    break;
                case PaletteType.AltSprite:
                    palData = levelData.SpriteAltPalette;
                    break;
            }

            RedrawAll();
        }

        public void RedrawAll() {
            BeginUpdate();
            for (int pal = 0; pal < 4; pal++) {
                for (int i = 0; i < 4; i++) {
                    DrawEntry(pal, i);

                }
            }
            EndUpdate();
        }

        protected override void OnCellClicked(int entryIndex, int palIndex, MouseButtons button) {
            base.OnCellClicked(entryIndex, palIndex, button);

            if (palData == null) return;

            if (button == MouseButtons.Left) {
                if (entryIndex == 0) {
                    ((frmMain)FindForm().Owner).PerformAction(Actions.SetPaletteColor(levelData, PaletteType.ZeroEntry, palIndex, entryIndex, selectedColor));
                } else {
                    ((frmMain)FindForm().Owner).PerformAction(Actions.SetPaletteColor(levelData, palType, palIndex, entryIndex, selectedColor));
                }
            } else if (button == MouseButtons.Right) {
                int palEntry = palIndex * 4 + entryIndex;
                ((frmPalette)FindForm()).SelectColor(palData.GetEntry(palEntry));

            }
        }

        void DrawEntry(int pal, int palEntryIndex) {
            base.DrawSquare(palEntryIndex,pal,palData.GetEntry(pal * 4 + palEntryIndex));
        }

        private int selectedColor;

        public int SelectedColor {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        ActionGenerator Actions { get { return ((frmPalette)FindForm()).Actions; } }

        internal void RedrawColor(int pal, int entry) {
            DrawEntry(pal, entry);
        }
    }
    enum PaletteType
    {
        Background,
        AltBackground,
        Sprite,
        AltSprite,
        ZeroEntry,
    }
}

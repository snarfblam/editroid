using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;

namespace Editroid.Actions
{
    class SetPaletteColor: LevelAction
    {
        int entryIndex;
        PaletteType type;
        byte oldValue, newValue;
        public SetPaletteColor(EditroidUndoRedoQueue q, Level level, PaletteType type, int palette, int index, int newValue) 
        :base(q,level) {
            this.type = type;
            this.entryIndex = palette * 4 + index;
            this.newValue = (byte)(newValue & 0xFF);
            this.oldValue = Palette.GetEntry(entryIndex);
        }

        public PaletteType Type { get { return type; } }
        public int PaletteIndex { get { return entryIndex / 4; } }
        public int EntryIndex { get { return entryIndex % 4; } }
        public NesPalette Palette {
            get {
                switch (type) {
                    case PaletteType.Background:
                        return Level.BgPalette;
                    case PaletteType.AltBackground:
                        return Level.BgAltPalette;
                    case PaletteType.Sprite:
                        return Level.SpritePalette;
                    case PaletteType.AltSprite:
                        return Level.SpriteAltPalette;
                    default:
                        return Level.BgPalette;
                }

                throw new InvalidOperationException("An invalid PaletteType was encountered in SetPaletteColor.Palette.get. This value should have been verified in the .ctor.");
            }
        }
        public override void Do() {
            if (type == PaletteType.ZeroEntry) 
                SetZeroPalette(newValue);
            else
                Palette.SetEntry(entryIndex, newValue);
        }

        private void SetZeroPalette(int newValue) {
            byte val = (byte)newValue;
            if (Level.Format.SupportsAltBgPalette) {
                Level.BgAltPalette.SetEntry(0, val);
                Level.BgAltPalette.SetEntry(4, val);
                Level.BgAltPalette.SetEntry(8, val);
                Level.BgAltPalette.SetEntry(0x0C, val);
            }
            if (Level.Format.SupportsAltSpritePalette) {
                Level.SpriteAltPalette.SetEntry(0, val);
                Level.SpriteAltPalette.SetEntry(4, val);
                Level.SpriteAltPalette.SetEntry(8, val);
                Level.SpriteAltPalette.SetEntry(0x0C, val);
            }
            Level.BgPalette.SetEntry(0, val);
            Level.BgPalette.SetEntry(4, val);
            Level.BgPalette.SetEntry(8, val);
            Level.BgPalette.SetEntry(0x0C, val);
            Level.SpritePalette.SetEntry(0, val);
            Level.SpritePalette.SetEntry(4, val);
            Level.SpritePalette.SetEntry(8, val);
            Level.SpritePalette.SetEntry(0x0C, val);
        }

        public override void Undo() {
            if (type == PaletteType.ZeroEntry)
                SetZeroPalette(oldValue);
            else
                Palette.SetEntry(entryIndex, oldValue);
        }

        public override string GetText() {
            return "Change palette entry";
        }
    }
}

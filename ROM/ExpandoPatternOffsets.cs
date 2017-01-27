using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    /// <summary>
    /// Contains the offsets of patterns in an expanded Metroid ROM.
    /// </summary>
    static class ExpandoPatternOffsets
    {
        public static Entry GlobalGameplaySprites = new Entry(0x22410, true, 0, 0x9A);
        public static Entry JustinBaileySprites = new Entry(0x23810, true, 0, 123);
        public static Entry DigitSprites = new Entry(0x23410, true, 0xA0, 0xA);
        public static Entry EndingSamusSprites = new Entry(0x24410,true,0,0x52);
        public static Entry TitleTextTiles = new Entry(0x23410, false, 0, 0x40);
        public static Entry TitleBgGraphics = new Entry(0x22F10, false, 0x40, 0x50);
        public static Entry TitleSpriteGraphics = new Entry(0x22db0, true, 0xC0, 0x10);
        public static Entry TheEndScriptTiles = new Entry(0x24010, false, 0, 0x40);

        public static Entry BrinstarBg = new Entry(0x1C010, false);
        public static Entry NorfairBg = new Entry(0x1D010, false);
        public static Entry RidleyBg = new Entry(0x1E010, false);
        public static Entry KraidBg = new Entry(0x1F010, false);
        public static Entry TourianBg = new Entry(0x20010, false);

        public static Entry BrinstarSpr = new Entry(0x21010, true, 0xC0, 0x40);
        public static Entry NorfairSpr = new Entry(0x21410, true, 0xC0, 0x40);
        public static Entry RidleySpr = new Entry(0x21C10, true, 0xC0, 0x40);
        public static Entry KraidSpr = new Entry(0x22010, true, 0xC0, 0x40);
        public static Entry TourianSpr = new Entry(0x21810, true, 0xC0, 0x40);

        /// <summary>This entry exists for creation of Enhanco ROMs, because the tiles need to be combined with TitleSpriteGraphics.</summary>
        public static Entry HudEnergyTank = new Entry(0x22AF0, true, 0x6E, 2);

        public static Entry GetBackgroundEntry(LevelIndex level) {
            switch (level) {
                case LevelIndex.Brinstar:
                    return BrinstarBg;
                case LevelIndex.Norfair:
                    return NorfairBg;
                case LevelIndex.Tourian:
                    return TourianBg;
                case LevelIndex.Kraid:
                    return KraidBg;
                case LevelIndex.Ridley:
                    return RidleyBg;
                default:
                    throw new ArgumentException("Invalid level index in ExpandoPatternOffsets.GetBackgroundEntry.");
            }
        }
        public static Entry GetSpriteEntry(LevelIndex level) {
            switch (level) {
                case LevelIndex.Brinstar:
                    return BrinstarSpr;
                case LevelIndex.Norfair:
                    return NorfairSpr;
                case LevelIndex.Tourian:
                    return TourianSpr;
                case LevelIndex.Kraid:
                    return KraidSpr;
                case LevelIndex.Ridley:
                    return RidleySpr;
                default:
                    throw new ArgumentException("Invalid level index in ExpandoPatternOffsets.GetSpriteEntry.");
            }
        }

        public struct Entry
        {
            public Entry(int offset, bool sprite, int dest, int count) {
                this.RomOffset = offset;
                this.IsSprite = sprite;
                this.DestTileindex = dest;
                this.TileCount = count;
            }
            public Entry(int offset, bool sprite)
                : this(offset, sprite, 0, 0x100) {
            }
            /// <summary>Rom offset, including header.</summary>
            public int RomOffset;
            /// <summary>If true, patterns are loaded to sprite page, otherwise bg.</summary>
            public bool IsSprite;

            // Usually 0
            public int DestTileindex;
            // Usually 0x100
            public int TileCount;
        }
    }
}

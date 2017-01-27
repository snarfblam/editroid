using System;
using System.Collections.Generic;
using System.Text;
using iLab;

namespace Editroid.ROM
{
    /// <summary>
    /// Encapsulates a table that lists locations of groups 
    /// of patterns that can be loaded.
    /// </summary>
    public class PatternGroupOffsetTable:VirtualCollection<PatternGroupOffsets>
    {
        public const int baseRomOffset = 0x1C6F0;
        public const int entryCount = 29;

        MetroidRom rom;

        public PatternGroupOffsetTable(MetroidRom rom) {
            this.IsReadOnly = false;
            
            this.rom = rom;
            for (int i = 0; i < entryCount; i++) {
                Add(new PatternGroupOffsets(rom, i));
            }

            this.IsReadOnly = true;
        }
    }

    /// <summary>
    /// Encapsulates data that specifies the source, destination, and
    /// size of a group of patterns to be loaded.
    /// </summary>
    public class PatternGroupOffsets
    {
        public const int EntrySize = 7;
        public const int baseRamOffset = 0x8000;
        public const int romBankSize = 0x4000;
        public const int tileSize = 0x10;

        public PatternGroupOffsets(MetroidRom rom, int index) {
            this.rom = rom;
            this.index = index;
            this.offset = EntrySize * index + PatternGroupOffsetTable.baseRomOffset;
        }

        private MetroidRom rom;
        public MetroidRom Rom { get { return rom; } }

        private int index;
        public int Index { get { return index; } }

        private int offset;
        public int Offset { get { return offset; } }

        public int SourceRomOffset {
            get {
                NesPointer24 pointer = new NesPointer24(rom.data, offset);
                return pointer.GetRomOffset(baseRamOffset);
            }
            set {
                NesPointer24 pointer = new NesPointer24();
                pointer.SetRomOffset(value, baseRamOffset);
                pointer.Write(rom.data, offset);
            }
        }

        /// <summary>
        /// Gets the PPU address where data is loaded. The pattern table starts at $0000 for sprites and $1000 for backgrounds
        /// </summary>
        public int DestPpuOffset {
            get {
                return rom.data[offset + 3] + (rom.data[offset + 4] << 8);
            }
            set {
                rom.data[offset + 3] = (byte)(value & 0xFF);
                rom.data[offset + 4] = (byte)((value >> 8) & 0xFF);
            }
        }

        /// <summary>
        /// Gets the tile number of the first tile of this
        /// groups PPU destination, from 0 to 255.
        /// </summary>
        public int DestTileIndex {
            get {
                return (DestPpuOffset / tileSize) & 0xFF;
            }
            set {
                // 0xF000 to maintain pattern ram page (sprite/bg)
                DestPpuOffset = (DestPpuOffset & 0xF000) | (value * tileSize);
            }
        }

        /// <summary>
        /// Returns true of this ppu data will be loaded to the background patterns (page 1).
        /// </summary>
        public bool IsPage1 {
            get { return (DestPpuOffset & 0xF000) != 0x0000; }
            set {
                if (value)
                    DestPpuOffset &= ~0x1000;
                else
                    DestPpuOffset |= 0x1000;
            }
        }
        public bool IsBackground { get { return IsPage1; } }

        /// <summary>
        /// Returns true of this ppu data will be loaded to the sprite patterns (page 0).
        /// </summary>
        public bool IsPage0 {
            get { return !IsPage1; }
            set { IsPage1 = !value; }
        }

        public bool IsSprite { get { return IsPage0; } }


        public int ByteCount {
            get {
                return rom.data[offset + 5] + (rom.data[offset + 6] << 8);
            }
            set {
                rom.data[offset + 5] = (byte)(value & 0xFF);
                rom.data[offset + 6] = (byte)((value >> 8) & 0xFF);
            }
        }

        public int TileCount {
            get { return ByteCount / tileSize; }
            set { ByteCount = value * tileSize; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM.Formats
{
    /// <summary>
    /// Exposes data and structure of various Metroid ROM formats used by Editroid. This class is meant
    /// to encapsulate the differences between the formats.
    /// </summary>
    public abstract class RomFormat
    {
        public MetroidRom Rom { get; private set; }
        public RomFormat(MetroidRom rom) {
            this.Rom = rom;
            MapLocation = FormatMapLocation.Unspecified;
            Banks = _Banks.AsReadOnly();

            LoadBanks();
        }

        // Public
        public IList<Bank> Banks { get; set; }

        // Private
        List<Bank> _Banks = new List<Bank>();

        // Protected
        protected abstract int PrgBankCount { get; }

        /// <summary>
        /// Location of the pointer to the CHR usage table. This pointer is in bank E. The chr usage table is in bank F.
        /// </summary>
        public static readonly pCpu ChrUsagePointer = new pCpu(0xBFFE);
        /// <summary>
        /// Pointer that specifies the location within bank $E of Editriod's general data section, if in use.
        /// </summary>
        public static readonly pCpu EditorDataPointer = new pCpu(0x8440);

        /// <summary>Gets a list containing all ranges of data that represent graphic tiles.</summary>
        public abstract RomRange[] GetAllPatternOffsets();

        public abstract LevelFormat CreateLevelFormat(Level level);

        private void LoadBanks() {
            int BankCount = PrgBankCount;
            for (int i = 0; i < BankCount; i++) {
                bool fixedBank = (i == BankCount - 1);
                _Banks.Add(new Bank(Rom, i, fixedBank));
            }
        }

        /// <summary>
        /// Gets the point at which general code will be inserted once assembled.
        /// </summary>
        /// <remarks>The return value specifies the address of the first byte that should come after the inserted code. For example, if the insertion point is 0xC000 and
        /// there are 0x111 bytes of assembled code, the code should occupy 0x111 bytes beginning at 0xBEEF and ending at 0xBFFF, leaving 0xC000 unmodified.</remarks>
        public virtual pCpu GeneralAsmInsertionAddress { get { return (pCpu)0xC000; } }

        


        /// <summary>Identifies whether the ROM uses the standard pattern tile loading system.</summary>
        public bool UsesStandardPpuLoader { get; protected set; }
        /// <summary>If true, the ROM contains a bank allocation table (see BankAllocation.cs) in Bank E to identify which banks can contain screen data.</summary>
        public bool HasPrgAllocationTable { get; protected set; }
        /// <summary>Identifies whether Editroid supports adding and deleting rooms, structures, and item data.</summary>
        public bool SupportsEnhancedMemoryManagement { get; protected set; }
        /// <summary>Identifies whether the ROM supports custom tile physics</summary>
        public bool SupportsCustomTilePhysics { get; protected set; }

        public FormatMapLocation MapLocation { get; protected set; }


        ////static int[] altMusicOffsets = new int[] { 
        ////    0x55e0, // (1) Brinstar
        ////    0x95e0, // (2) Norfair
        ////    0xD5e0, // (3) Tourian
        ////    0x115e0, // (4) Kraid
        ////    0x155e0, // (5) Ridley
        ////};
        ////public virtual pRom GetAltMusicOffset(LevelIndex level) {
        ////    return (pRom)altMusicOffsets[(int)level];
        ////}
        /// <summary>
        /// If true, the ROM supports 256 combos rather than the standard 64.
        /// </summary>
        public virtual bool SupportsExtendedComboTable { get { return false; } }
        /// <summary>
        /// If true, the ROM supports 256 max. structures rather than the standard 128.
        /// </summary>
        public virtual bool SupportsExtendedStructTable { get { return false; } }
        /// <summary>
        /// If true, the ROM contains a pointer at 0E:BFFE to a CHR usage table in bank F.
        /// </summary>
        public virtual bool HasChrUsageTable { get { return false; } }
        /// <summary>
        /// If true, the ROM uses a CHR animation table for MMC3 CHR bank cycling
        /// </summary>
        public virtual bool HasChrAnimationTable { get { return false; } }

        public abstract pRom ChrRomOffset { get; }
    }

    /// <summary>
    /// Exposes data and structure of the standard Metroid ROM.
    /// </summary>
    class StandardRomFormat : RomFormat
    {
        public StandardRomFormat(MetroidRom rom) : base(rom) {
            SupportsEnhancedMemoryManagement = false;
            UsesStandardPpuLoader = true;
            MapLocation = FormatMapLocation.Bank_0F;
        }

        public override RomRange[] GetAllPatternOffsets() {
            var patternData = Rom.PatternGroupOffsets;
            RomRange[] result = new RomRange[patternData.Count];

            for (int i = 0; i < patternData.Count; i++) {
                result[i] = new RomRange(patternData[i].Offset, patternData[i].ByteCount);
            }

            return result;
        }


        protected override int PrgBankCount { get { return 8; } }

        public override LevelFormat CreateLevelFormat(Level level) {
            return new StandardLevelFormat(level);
        }

        public override pRom ChrRomOffset {
            get { return (pRom)0x20010; }
        }
    }

    /// <summary>
    /// Exposes data and structure of an Expando ROM.
    /// </summary>
    class ExpandoRomFormat : RomFormat
    {
        public ExpandoRomFormat(MetroidRom rom) : base(rom) {
            SupportsEnhancedMemoryManagement = true;
            UsesStandardPpuLoader = false;
            MapLocation = FormatMapLocation.Bank_0F;
        }

        public override RomRange[] GetAllPatternOffsets() {
            RomRange[] Result = new RomRange[] {
                RomRangeFrom(ExpandoPatternOffsets.BrinstarBg),
                RomRangeFrom(ExpandoPatternOffsets.BrinstarSpr),
                RomRangeFrom(ExpandoPatternOffsets.NorfairBg),
                RomRangeFrom(ExpandoPatternOffsets.NorfairSpr),
                RomRangeFrom(ExpandoPatternOffsets.RidleyBg),
                RomRangeFrom(ExpandoPatternOffsets.RidleySpr),
                RomRangeFrom(ExpandoPatternOffsets.TourianBg),
                RomRangeFrom(ExpandoPatternOffsets.TourianSpr),
                RomRangeFrom(ExpandoPatternOffsets.KraidBg),
                RomRangeFrom(ExpandoPatternOffsets.KraidSpr),
                RomRangeFrom(ExpandoPatternOffsets.DigitSprites),
                RomRangeFrom(ExpandoPatternOffsets.GlobalGameplaySprites),
            };

            return Result;
        }

        private RomRange RomRangeFrom(ExpandoPatternOffsets.Entry entry) {
            return new RomRange(entry.RomOffset, entry.TileCount * 0x10);
        }


        protected override int PrgBankCount { get { return 16; } }

        public override LevelFormat CreateLevelFormat(Level level) {
            return new ExpandoLevelFormat(level);
        }

        public override pRom ChrRomOffset {
            get { return (pRom)0x40010; }
        }

    }

    /// <summary>
    /// Exposes data and structure of the enhanced ROM format, which expands upon the Expando format.
    /// </summary>
    class EnhancedRomFormat : ExpandoRomFormat
    {
        public EnhancedRomFormat(MetroidRom rom) : base(rom) {
            SupportsEnhancedMemoryManagement = true;
            UsesStandardPpuLoader = false;
        }

        public override RomRange[] GetAllPatternOffsets() {
            // How convenient... all patterns are in a solid block
            return new RomRange[]{
                new RomRange(0x40010, 0x20000),
            };
        }
        protected override int PrgBankCount { get { return 16; } }

        public override LevelFormat CreateLevelFormat(Level level) {
            return new EnhancedLevelFormat(level);
        }

        public override bool SupportsExtendedStructTable { get { return true; } }
        public override bool SupportsExtendedComboTable { get { return true; } }
        public override bool HasChrUsageTable { get { return true; } }

        public override pRom ChrRomOffset {
            get { return (pRom)0x40010; }
        }
    }
    class Mmc3RomFormat : EnhancedRomFormat
    {
        public Mmc3RomFormat(MetroidRom rom)
            : base(rom) {
            // "Enhanced memory management" (adding/removing level data items, e.g. screens/items/etc, may be removed from all but MMC3 ROMs for simplicity)
            SupportsEnhancedMemoryManagement = true;
            HasPrgAllocationTable = true;
            SupportsCustomTilePhysics = true;
        }

        protected override int PrgBankCount { get { return 32; } }

        public override LevelFormat CreateLevelFormat(Level level) {
            return new Mmc3LevelFormat(level);
        }


        public override pCpu GeneralAsmInsertionAddress {
            get {
                return (pCpu)0xA000;
            }
        }

        public override bool HasChrAnimationTable {
            get {
                return true;
            }
        }
        public override bool HasChrUsageTable {
            get {
                return false;
            }
        }

        public override pRom ChrRomOffset {
            get { return (pRom)0x80010; }
        }

        public override RomRange[] GetAllPatternOffsets() {
            return new RomRange[] { new RomRange(0x80010, 0x40000) };
        }
    }

    /// <summary>
    /// Defines where the editor map data is to be stored for the ROM format.
    /// </summary>
    public enum FormatMapLocation
    {
        /// <summary>The format does not have a specified map location. The map may be 
        /// appended to the end of the end of the ROM, stored in a separate file, or elsewhere.</summary>
        Unspecified,
        /// <summary>
        /// The map data is to be stored at the beginning of PRG bank 0F, which has the offset
        /// 0x38010 in a headered ROM.
        /// </summary>
        Bank_0F,
    }
    

}

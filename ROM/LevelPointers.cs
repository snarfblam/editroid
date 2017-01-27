using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;

namespace Editroid
{
    // Todo: move pointerTableData members/code into LevelPointers. There is no longer any need for this class since its purpose (identify offsets/length of data) is obsolete.
	/// <summary>
	/// Contains the offsets of pointer tables for the five different currentLevelIndex
	/// </summary>
    public class LevelPointers
    {
        internal readonly PointerTable structs;

        public MetroidRom Rom { get; private set; }
        public Level Level { get; private set; }
        public PointerTable StructTable { get { return structs; } }

        ////public int ItemDataOffset { get { return Level.DerefHandle(DataPointerOffsets.PointerToItemList); } }
        ////public int ComboDataOffset { get { return Level.DerefHandle(Level.Format.PointerToComboData); } }
        public int RoomTableOffset { get { return Level.DerefHandle(Level.Format.pPointerToRoomPTable); } }
        public int StructTableOffset { get { return Level.DerefHandle(Level.Format.pPointerToStructPTable); } }

        public LevelPointers(MetroidRom rom, Level level) {
            this.Rom = rom;
            this.Level = level;

            structs = new PointerTable(rom, (pRom)StructTableOffset, CalculateStructCount());


        }




        private int CalculateStructCount() {
            int StructCount;

            switch (Level.Index) {
                case LevelIndex.Brinstar:
                    StructCount = StandardRomCounts.brinstarStructCount;
                    break;
                case LevelIndex.Norfair:
                    StructCount = StandardRomCounts.norfairStructCount;
                    break;
                case LevelIndex.Tourian:
                    StructCount = StandardRomCounts.tourianStructCount;
                    break;
                case LevelIndex.Kraid:
                    StructCount = StandardRomCounts.kraidStructCount;
                    break;
                case LevelIndex.Ridley:
                    StructCount = StandardRomCounts.ridleyStructCount;
                    break;
                case LevelIndex.None:
                default:
                    throw new ArgumentException("Invalid level index.");
            }

            bool structDataRelocated = Level.PRom[Level.Format.pPointerToStructPTable] < Level.PRom[Level.Format.pPointerToRoomPTable];
            if (structDataRelocated) { // relocated expando
                pCpu firstStructOffset = new pCpu(Level.Rom.data, StructTableOffset);
                int firstStructRomOffset = firstStructOffset.BankedOffset + Level.BankOffset;

                StructCount = (firstStructRomOffset - StructTableOffset) / 2;
            }

            return StructCount;
        }

        ////public static class DataPointerOffsets
        ////{
        ////    public static readonly pCpu PointerToRoomPTable = new pCpu(0x959A);
        ////    public static readonly pCpu PointerToStructPTable = new pCpu(0x959C);
        ////    public static readonly pCpu PointerToComboData = new pCpu(0x959E);
        ////    public static readonly pCpu PointerToItemList = new pCpu(0x9598);
        ////}
        private static class StandardRomCounts
        {
            public const int brinstarStructCount = 0x32;
            public const int norfairStructCount = 0x31;
            public const int tourianStructCount = 0x20;
            public const int kraidStructCount = 0x27;
            public const int ridleyStructCount = 0x1D;
        }

    }
}

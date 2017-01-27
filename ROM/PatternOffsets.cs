using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editroid.ROM
{
    class EnhancedPatternOffsets
    {
        const int ChrRomOffset = 0x40010;
        const int ChrBankSize = 0x1000;

        const int BrinstarBaseBank = 2;
        const int NorfairBaseBank = 8;
        const int TourianBaseBank = 0xE;
        const int KraidBaseBank = 0x14;
        const int RidleyBaseBank = 0x1A;

        public static pRom GetBaseBank(LevelIndex level) {
            return (pRom)(GetBaseBankIndex(level) * ChrBankSize + ChrRomOffset);
        }

        private static int GetBaseBankIndex(LevelIndex level) {
            switch (level) {
                case LevelIndex.Brinstar: return BrinstarBaseBank;
                case LevelIndex.Norfair: return NorfairBaseBank;
                case LevelIndex.Tourian: return TourianBaseBank;
                case LevelIndex.Kraid: return KraidBaseBank;
                case LevelIndex.Ridley: return RidleyBaseBank;
                default: throw new ArgumentException("GetBaseBankIndex(): invalid argument");
            }
        }

        public static pRom GetSprBank(LevelIndex level) {
            return GetBaseBank(level);
        }

        public static pRom GetBgBank(LevelIndex level) {
            return GetChrBank(GetBaseBankIndex(level));
        }
        public static pRom GetChrBank(int index) {
            return (pRom)((index) * ChrBankSize + ChrRomOffset);
        }
    }
    class MMC3PatternOffsets
    {
        const int ChrRomOffset = 0x80010;
        const int ChrBankSize = 0x1000;

        const int BrinstarBaseBank = 2;
        const int NorfairBaseBank = 8;
        const int TourianBaseBank = 0xE;
        const int KraidBaseBank = 0x14;
        const int RidleyBaseBank = 0x1A;

        public static pRom GetBaseBank(LevelIndex level) {
            return (pRom)(GetBaseBankIndex(level) * ChrBankSize + ChrRomOffset);
        }

        private static int GetBaseBankIndex(LevelIndex level) {
            switch (level) {
                case LevelIndex.Brinstar: return BrinstarBaseBank;
                case LevelIndex.Norfair: return NorfairBaseBank;
                case LevelIndex.Tourian: return TourianBaseBank;
                case LevelIndex.Kraid: return KraidBaseBank;
                case LevelIndex.Ridley: return RidleyBaseBank;
                default: throw new ArgumentException("GetBaseBankIndex(): invalid argument");
            }
        }

        public static pRom GetSprBank(LevelIndex level) {
            return GetBaseBank(level);
        }

        public static pRom GetBgBank(LevelIndex level) {
            return GetChrBank(GetBaseBankIndex(level));
        }
        public static pRom GetChrBank(int index) {
            return (pRom)((index) * ChrBankSize + ChrRomOffset);
        }
    }
}

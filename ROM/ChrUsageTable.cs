using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    /// <summary>
    /// Reads and modifies the CHR usage table on enhanced ROMs.
    /// </summary>
    /// <remarks>Use LevelIndex.None to refer to the title bank.</remarks>
    public class ChrUsageTable
    {
        MetroidRom rom;
        LevelIndex[] tableOrder = { LevelIndex.None, LevelIndex.Brinstar, LevelIndex.Norfair, LevelIndex.Tourian, LevelIndex.Kraid, LevelIndex.Ridley };

        public ChrUsageTable(MetroidRom rom) {
            this.rom = rom;
        }

        private pCpu GetLevelTableAddress(LevelIndex l) {
            int levelIndex = Array.IndexOf(tableOrder, l);

            // Pointer to level's CHR usage table
            var pChrUsage = rom.Banks[0xE].GetPtr(Editroid.ROM.Formats.RomFormat.ChrUsagePointer);
            pChrUsage += levelIndex * 4;

            return pChrUsage;
        }

        Bank ChrUsageBank { get { return rom.Banks[rom.Format.Banks.Count - 1]; } }

        public byte GetSprPage(LevelIndex level) {
            // byte 1 of 4
            return ChrUsageBank[GetLevelTableAddress(level)];
        }

        public byte GetBgFirstPage(LevelIndex level) {
            // byte 2 of 4
            return ChrUsageBank[GetLevelTableAddress(level) + 1];
        }

        public byte GetBgLastPage(LevelIndex level) {
            // byte 3 of 4
            return ChrUsageBank[GetLevelTableAddress(level) + 2];
        }

        public byte GetAnimRate(LevelIndex level) {
            // byte 4 of 4
            return ChrUsageBank[GetLevelTableAddress(level) + 3];
        }


        public void SetSprPage(LevelIndex level, byte value) {
            // byte 1 of 4
            ChrUsageBank[GetLevelTableAddress(level)] = value;
        }

        public void SetBgFirstPage(LevelIndex level, byte value) {
            // byte 2 of 4
            ChrUsageBank[GetLevelTableAddress(level) + 1] = value;
        }

        public void SetBgLastPage(LevelIndex level, byte value) {
            // byte 3 of 4
            ChrUsageBank[GetLevelTableAddress(level) + 2] = value;
        }

        public void SetAnimRate(LevelIndex level, byte value) {
            // byte 4 of 4
            ChrUsageBank[GetLevelTableAddress(level) + 3] = value;
        }


        #region MMC3 accessors for repurposed fields
        public byte MMC3_GetSpr0(LevelIndex level) {
            return GetSprPage(level);
        }
        public void MMC3_SetSpr0(LevelIndex level, byte value) {
            SetSprPage(level, value);
        }
        public byte MMC3_GetSpr1(LevelIndex level) {
            return GetBgLastPage (level);
        }
        public void MMC3_SetSpr1(LevelIndex level, byte value) {
            SetBgLastPage(level, value);
        }
        public byte MMC3_GetBgLoopStart(LevelIndex level) {
            return GetBgFirstPage(level);
        }
        public void MMC3_SetBgLoopStart(LevelIndex level, byte value) {
            SetBgFirstPage(level, value);
        }
        /// <summary>
        /// Set to FF. Is the value is not FF, this indicates that the ROM uses a CHR usage table instead of a CHR animation table.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public byte MMC3_GetUnusedByte(LevelIndex level) {
            return GetAnimRate(level);
        }
        /// <summary>
        /// Set to FF. Is the value is not FF, this indicates that the ROM uses a CHR usage table instead of a CHR animation table.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="value"></param>
        public void MMC3_SetUnusedByte(LevelIndex level, byte value) {
            SetAnimRate(level, value);
        }
        #endregion

    }
}

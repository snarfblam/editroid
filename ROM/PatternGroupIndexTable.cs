using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    /// <summary>
    /// Provides facilities to read and modify which pattern groups
    /// a level loads.
    /// </summary>
    public class PatternGroupIndexTable:IList<byte>
    {
        private Level level;
        public Level Level { get { return level; } }
        MetroidRom rom;

        PatternGroupOffsetTable ranges;
        int[] indexOffsets;

        /// <summary>
        /// Loads level-specific pattern groups.
        /// </summary>
        /// <param name="level">The level to load patter groups for.</param>
        public PatternGroupIndexTable(Level level) {
            this.level = level;
            this.rom = level.Rom;
            this.ranges = level.Rom.PatternGroupOffsets;

            if (level.Index == LevelIndex.Brinstar)
                indexOffsets = PatternGroupIndexOffsets.Brinstar;
            else if (level.Index == LevelIndex.Norfair)
                indexOffsets = PatternGroupIndexOffsets.Norfair;
            else if (level.Index == LevelIndex.Tourian)
                indexOffsets = PatternGroupIndexOffsets.Tourian;
            else if (level.Index == LevelIndex.Kraid)
                indexOffsets = PatternGroupIndexOffsets.Kraid;
            else if (level.Index == LevelIndex.Ridley)
                indexOffsets = PatternGroupIndexOffsets.Ridley;
        }
        /// <summary>
        /// Loads global pattern groups.
        /// </summary>
        /// <param name="rom">The ROM to load pattern groups for.</param>
        public PatternGroupIndexTable(MetroidRom rom) {
            this.level = null;
            this.rom = rom;
            this.ranges = rom.PatternGroupOffsets;

            indexOffsets = PatternGroupIndexOffsets.Global;
        }

        #region IList<byte> Members

        public int IndexOf(byte item) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, byte item) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index) {
            throw new Exception("The method or operation is not implemented.");
        }

        public byte this[int index] {
            get {
                return rom.data[indexOffsets[index]];
            }
            set {
                rom.data[indexOffsets[index]] = value;
            }
        }

        #endregion

        #region ICollection<byte> Members

        public void Add(byte item) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Clear() {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(byte item) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void CopyTo(byte[] array, int arrayIndex) {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count {
            get { return indexOffsets.Length; }
        }

        public bool IsReadOnly {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool Remove(byte item) {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<byte> Members

        public IEnumerator<byte> GetEnumerator() {
            return new iLab.IListEnumerator<byte>(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Contains the rom offsets where the pattern group indecies are located for each level
        /// </summary>
        static class PatternGroupIndexOffsets
        {
            public static readonly int[] Global = { 0x1C5E8, 0x1C5ED, 0x1C5F7, 0x1C5FC, 0x1C601, 0x1C606, 0x1C60B, 0x1C610 };
            public static readonly int[] Brinstar = makeList(6, 0x1C615);
            public static readonly int[] Norfair = makeList(7, 0x1C633);
            public static readonly int[] Tourian = makeList(10, 0x1C656);
            public static readonly int[] Ridley = makeList(7, 0x1C6B0);
            public static readonly int[] Kraid = makeList(8, 0x1C688);

            const int entrySpacing = 0x05;
            static int[] makeList(int count, int offset) {
                int[] result = new int[count];
                for (int i = 0; i < count; i++) {
                    result[i] = offset;
                    offset += entrySpacing;
                }

                return result;
            }
        }


    }
}

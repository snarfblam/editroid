using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;

namespace Editroid
{
    ///// <summary>
    ///// Identifies the location within a ROM of a pointer table
    ///// </summary>
    //public struct PointerTable
    //{
    //    /// <summary>
    //    /// The offset of the beginning of the pointer table
    //    /// </summary>
    //    public int Offset;
    //    /// <summary>
    //    /// The number of entries in the pointer table
    //    /// </summary>
    //    public int Count;

    //    /// <summary>
    //    /// Pulls a Metroid pointer out of this pointer table in the specified ROM data
    //    /// </summary>
    //    /// <param name="count">The count of the entry</param>
    //    /// <param name="data">The ROM data to access</param>
    //    /// <returns>a Metroid pointer</returns>
    //    public MetroidPointer GetPointer(int index, byte[] data) {
    //        return new MetroidPointer(data, Offset + index * 2);
    //    }


    //}

    public class PointerTable
    {
        public pRom Offset { get; private set; }
        public MetroidRom Rom { get; private set; }
        public int Count { get; private set; }

        int EntrySize = 2;
        int pointerOffset = 0;
        bool using24BitPointers = false;

        public PointerTable(MetroidRom rom, pRom tableLocation, int tableSize) {
            this.Offset = tableLocation;
            this.Rom = rom;
            this.Count = tableSize;
        }
        public PointerTable(MetroidRom rom, pRom tableLocation, int tableSize, bool use24BitPointers) {
            this.Offset = tableLocation;
            this.Rom = rom;
            this.Count = tableSize;
            this.using24BitPointers = use24BitPointers;
            if (use24BitPointers) {
                EntrySize = 3;
                pointerOffset = 1;
            }
        }

        public pCpu this[int index]{
            get { return new pCpu(Rom.data, Offset + index * EntrySize  +pointerOffset); }
            set {
                Rom.data[Offset + index * EntrySize + pointerOffset] = value.Byte1;
                Rom.data[Offset + index * EntrySize + 1 + pointerOffset] = value.Byte2;
            }
        }
        public byte GetBank(int index) {
            if (!using24BitPointers) throw new InvalidOperationException("Can not get/set bank for 16-bit pointers");
            return Rom.data[Offset + index * EntrySize];
        }
        public void SetBank(int index, byte value) {
            if (!using24BitPointers) throw new InvalidOperationException("Can not get/set bank for 16-bit pointers");
            Rom.data[Offset + index * EntrySize] = value;
        }

        public static pCpu ReadTwoBytePointer(byte[] data, int tableOffset, int index) {
            return new pCpu(data, tableOffset + index * 2);
        }
        public static void WriteTwoBytePointer(byte[] data, int tableOffset, int index, pCpu pointer) {
            data[tableOffset + index * 2] = pointer.Byte1;
            data[tableOffset + index * 2 + 1] = pointer.Byte2;
        }


        public void ChangeCount(int count) {
            this.Count = count;
        }
    }

}

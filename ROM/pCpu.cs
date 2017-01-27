using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using Editroid.ROM;

namespace Editroid
{
	/// <summary>
	/// Represents a cpu pointer for Metroid.
	/// </summary>
    /// <remarks>Nes pointers typically refer to banked data.
    /// Use the GetDataOffset method to obtain the correctly offset pointer value.</remarks>
    public struct pCpu
    {
        internal Byte Byte1, Byte2;

        /// <summary>
        /// Creates a pointer from a composite value
        /// </summary>
        /// <param name="value">An unsigned 16-bit composite of two bytes making up a pointer</param>
        public pCpu(ushort value) {
            byte[] bytes = BitConverter.GetBytes(value);
            Byte1 = bytes[0];
            Byte2 = bytes[1];
        }
        public pCpu(int value)
            : this((ushort)(value & 0xFFFF)) {
        }
        public pCpu(pRom romOffset, Level level)
            : this(level.CreatePointer(romOffset).Value) {
        }

        public bool IsPrgBank { get { return Value >= 0xC000; } }
        public bool IsLevelBank { get { return Value >= 0x8000 & Value < 0xC000; } }
        /////// <summary>
        /////// Creates a pointer from a composite value
        /////// </summary>
        /////// <param name="value">T 16-bit composite of two bytes making up a pointer</param>
        ////public NesPointer(short value) {
        ////    byte[] bytes = BitConverter.GetBytes(value);
        ////    Byte1 = bytes[1];
        ////    Byte2 = bytes[0];
        ////}

        /// <summary>
        /// Creates a pointer from two bytes
        /// </summary>
        /// <param name="byte1">First byte of this Pointer</param>
        /// <param name="byte2">Second byte of this Pointer</param>
        public pCpu(byte byte1, byte byte2) {
            Byte1 = byte1;
            Byte2 = byte2;
        }
        /// <summary>
        /// Loads a pointer from raw data
        /// </summary>
        /// <param name="source">Source of the data</param>
        /// <param name="offset">The offset of the pointer</param>
        public pCpu(byte[] source, int offset) {
            Byte1 = source[offset];
            Byte2 = source[offset + 1];
        }
        /// <summary>
        /// Gets the actual value represented by this pointer.
        /// </summary>
        /// <remarks>This is not necessarily the offset of the 
        /// data that the pointer is intended to refer to. If the data is
        /// currentLevelIndex-specific, use the GetDataOffset function. This corrects
        /// offsets not reflected in the raw pointer value.</remarks>
        public int Value {
            get {
                return Byte1 | ((int)Byte2 << 8);
            }
            set {
                Byte2 = (byte)((value >> 8) & 0xFF);
                Byte1 = (byte)(value & 0xFF);
            }
        }


        /// <summary>
        /// Gets the bank-relative offset of this pointer.
        /// </summary>
        public int BankedOffset {
            get { return Value & 0x3FFF; }
            set {
                int offset = value & 0x3FFF;
                this.Value = ((this.Value & 0xC000) | offset);
            }
        }

        /// <summary>
        /// Gets the offset of the currentLevelIndex-specific data that this pointer
        /// refers to.
        /// </summary>
        /// <param name="currentLevelIndex">The currentLevelIndex this pointer refers to</param>
        /// <returns>the offset of the currentLevelIndex-specific data that this pointer
        /// refers to</returns>
        /// <remarks>This function only applies to certain currentLevelIndex-specific data.</remarks>
        public int GetDataOffset(LevelIndex level) {
            if (level == LevelIndex.Norfair || level == LevelIndex.Tourian)
                return Value + GetLevelOffset(level) + 0x8000;
            else if (level == LevelIndex.Ridley)
                return Value + GetLevelOffset(level) + 0xC000;
            else if (level == LevelIndex.Kraid)
                return Value + GetLevelOffset(level) + 0x4000;
            else if (level == LevelIndex.Brinstar)
                return Value + GetLevelOffset(level);
            else
                return Value;
        }

        /// <summary>
        /// Calculates and stores the correct pointer value to points to 
        /// currentLevelIndex-specific data
        /// </summary>
        /// <param name="currentLevelIndex">The currentLevelIndex this pointer refers to</param>
        /// <param name="offset">The offset of the actual data in the ROM</param>
        public void SetDataOffset(LevelIndex level, int offset) {
            Value = offset - GetLevelOffset(level);
            if (level == LevelIndex.Norfair || level == LevelIndex.Tourian)
                Value = offset - GetLevelOffset(level) - 0x8000;
            else if (level == LevelIndex.Ridley)
                Value = offset - GetLevelOffset(level) - 0xC000;
            else if (level == LevelIndex.Kraid)
                Value = offset - GetLevelOffset(level) - 0x4000;
            else if (level == LevelIndex.Brinstar)
                Value = offset - GetLevelOffset(level);
            else
                Value = offset;
        }

        /// <summary>
        /// Pointer offset correction for Brinstar. This value does not account for different memory banks.
        /// </summary>
        public static int BrinstarOffset { get { return -16368; } }
        /// <summary>
        /// Pointer offset correction for Ridley's Hideout. This value does not account for different memory banks.
        /// </summary>
        public static int RidleyOffset { get { return 16; } }
        /// <summary>
        /// Pointer offset correction for Kraid's Hideout. This value does not account for different memory banks.
        /// </summary>
        public static int KraidOffset { get { return 16400; } }
        /// <summary>
        /// Pointer offset correction for Norfair. This value does not account for different memory banks.
        /// </summary>
        public static int NorfairOffset { get { return -32752; } }
        /// <summary>
        /// Pointer offset correction for Tourian. This value does not account for different memory banks.
        /// </summary>
        public static int TourianOffset { get { return -16368; } }
        /// <summary>
        /// Gets the pointer offset correction for a specified currentLevelIndex.
        /// This value does not account for different memory banks.
        /// </summary>
        /// <param name="currentLevelIndex">The currentLevelIndex to get the correction value for</param>
        /// <returns>The pointer offset correction</returns>
        public static int GetLevelOffset(LevelIndex level) {
            switch (level) {
                case LevelIndex.Brinstar:
                    return BrinstarOffset;
                case LevelIndex.Kraid:
                    return KraidOffset;
                case LevelIndex.Norfair:
                    return NorfairOffset;
                case LevelIndex.Ridley:
                    return RidleyOffset;
                case LevelIndex.Tourian:
                    return TourianOffset;
                case LevelIndex.None:
                    return 0;
            }
            throw new ArgumentException("Invalid level");
        }

        /// <summary>
        /// Returns a pointer to the ROM image data this CPU pointer references.
        /// </summary>
        /// <param name="level">The level bank this pointer is used for.</param>
        /// <returns></returns>
        public Editroid.ROM.pRom AsPRom(Level level) {
            return level.ToPRom(this);
        }
                /// <summary>
        /// Returns a pointer to the ROM image data this CPU pointer references.
        /// </summary>
        /// <param name="level">The level bank this pointer is used for.</param>
        /// <returns></returns>
        public Editroid.ROM.pRom AsPRom(LevelIndex level) {
            return (pRom)(GetDataOffset(level));
        }
        public override string ToString() {
            return this.Value.ToString("X4");
        }

        public static explicit operator int(pCpu p) {
            return p.Value;
        }
        public static explicit operator pCpu(int i) {
            return new pCpu(i);
        }
        public static pCpu operator +(pCpu a, int b) {
            a.Value += b;
            return a;
        }
        public static pCpu operator -(pCpu a, int b) {
            a.Value -= b;
            return a;
        }
        public static int operator -(pCpu a, pCpu b) {
            return a.Value - b.Value;
        }
        public static bool operator <(pCpu a, pCpu b) {
            return a.Value < b.Value;
        }

        public static bool operator >(pCpu a, pCpu b) {
            return a.Value > b.Value;
        }

        public void Write(byte[] data, int offset) {
            data[offset] = Byte1;
            data[offset + 1] = Byte2;
        }
        public void Write(Stream s) {
            s.WriteByte(Byte1);
            s.WriteByte(Byte2);
        }

    }


    /// <summary>
    /// Handle points to a ROM offset that contains a pointer to ROM data, allowing access to the data and modification of the pointer.
    /// </summary>
    public struct MetroidHandle_DEPRECATED
    {
        //Rom rom;
        byte[] data;
        int offset;
        LevelIndex level;

        /// <summary>
        /// Gets/sets which currentLevelIndex this handle references data for.
        /// </summary>
        public LevelIndex Level {
            get { return level; }
            set { level = value; }
        }

        /// <summary>Gets the location in the rom where the actual pointer is stored.</summary>
        public int HandleOffset { get { return offset; } }
        /// <summary>Gets the uncorrected value of thie pointer this handle references.</summary>
        public int PointerValue {
            get {
                return Pointer.GetDataOffset(LevelIndex.None);
            }
        }
        /// <summary>Gets the ROM offset for the data that this handle's pointer references.</summary>
        public int PointerTarget {
            get {
                return Pointer.GetDataOffset(level);
            }
        }

        /// <summary>Instantiates this object.</summary>
        /// <param name="offset">Location of pointer in ROM data.</param>
        /// <param name="data">Rom image this pointer points to.</param>
        public MetroidHandle_DEPRECATED(byte[] data, int offset) {
            this.data = data;
            this.offset = offset;
            level = LevelIndex.None;
        }

        /// <summary>Instantiates this object.</summary>
        /// <param name="data">ROM data.</param>
        /// <param name="offset">Location of pointer in ROM data.</param>
        /// <param name="currentLevelIndex">The currentLevelIndex that the pointer refers to data for.</param>
        public MetroidHandle_DEPRECATED(byte[] data, int offset, LevelIndex level) {
            this.data = data;
            this.offset = offset;
            this.level = level;
        }

        /// <summary>
        /// Gets the metroid pointer that this handle references.
        /// </summary>
        public pCpu Pointer {
            get {
                return new pCpu(data[offset], data[offset + 1]);
            }
        }

        /// <summary>
        /// Modifies the underlying pointer, allowing pointer arithmetic to be performed.
        /// </summary>
        /// <param name="amount">The amount to offset the pointer.</param>
        public void OffsetPointer_DEPRECATED(int amount) {
            ////////////////////////// Get pointer data
            ////////////////////////Union_16 pointerValue = Union_16.Create();
            ////////////////////////pointerValue.Byte2 = data[offset + 1];
            ////////////////////////pointerValue.Byte1 = data[offset];
            ////////////////////////// Offset pointer data
            ////////////////////////pointerValue.UShort = (ushort)(pointerValue.UShort + amount);
            ////////////////////////// Store modified pointer data
            ////////////////////////data[offset + 1] = pointerValue.Byte2;
            ////////////////////////data[offset] = pointerValue.Byte1;

            pCpu ptr = new pCpu(data, offset);
            ptr += amount;
            ptr.Write(data, offset);
        }

        /////// <summary>
        /////// Returns a MetroidHandle to a neighboring pointer. For instance, to get a handle to a pointer that comes directly before this handle's pointer, specify a byte offset
        /////// of -2.
        /////// </summary>
        /////// <param name="bytesOffset"></param>
        /////// <returns></returns>
        ////public MetroidHandle_DEPRECATED GetOffsetHandle(int bytesOffset) {
        ////    return new MetroidHandle_DEPRECATED(this.data, this.offset - 2, this.level);
        ////}
    }

    /// <summary>
    /// Allows direct access to the same memory as different data types.
    /// </summary>
    /// <remarks>Editing one member of this struct will modify the value of some
    /// or all other members of this struct. All members share the same memory.</remarks>
    [StructLayout(LayoutKind.Explicit)]
    public struct Union_16
    {
        /// <summary>The first byte of the 16 bits.</summary>
        [FieldOffset(0)] public Byte Byte1;
        /// <summary>The second byte of the 16 bits.</summary>
        [FieldOffset(1)] public Byte Byte2;
        /// <summary>The value of the 16 bits as an unsigned integer.</summary>
        [FieldOffset(0)] public ushort UShort;
        /// <summary>The value of the 16 bits as a signed integer.</summary>
        [FieldOffset(0)] public short Short;
        /// <summary>The value of the 16 bits as a unicode character.</summary>
        [FieldOffset(0)] public Char Char;

        /// <summary>Gets an initialized instance of this struct.</summary>
        /// <returns>An initialized instance of this struct.</returns>
        public static Union_16 Create() {
            Union_16 result;

            result.UShort = 0;
            result.Short = 0;
            result.Char = '\0';
            result.Byte1 = result.Byte2 = 0;

            return result;
        }
    }

    //public struct PrgPointer_High
    //{
    //    public ushort Value { get; set; }
    //    public PrgPointer_High(ushort value)
    //        : this() {
    //        this.Value = value;
    //    }
    //    public PrgPointer_High(byte[] data, int offset)
    //        : this() {
    //        int high = ((int)(data[offset + 1])) << 8;
    //        int low = data[offset];
    //        this.Value = (ushort)(high | low);
    //    }
    //    public void Write(byte[] data, int offset){
    //        int high = (Value & 0xFF00) >> 8;
    //        int low = Value & 0xFF;
    //        data[offset] = (byte)low;
    //        data[offset + 1] = (byte)high;
    //    }

    //    public int BankOffset { 
    //        get { return Value & 0x3FFF; }
    //        set {
    //            int offset = value & 0x3FFF;
    //            this.Value = (ushort)((this.Value & 0xC000) | offset);
    //        }
    //    }
    //}
    /// <summary>
    /// Reads, writes, and modifies a 24-bit NES pointer. See remarks.
    /// </summary>
    /// <remarks>Warning: Some functions fail silently, others have no
    /// error checking. VALIDATE PARAMETERS.
    /// 
    /// NesPointer24 is useful with bank-switched ROM data.
    /// NES 24-bit pointers contain a ROM bank-number and a RAM pointer.
    /// The RAM pointer is the address the ROM data will be loaded to.
    /// 
    /// When bank-switching is used, the base address for the pointer
    /// will be a muliple of 4000.
    /// 
    /// This class assumes the ROM is headered!</remarks>
    public struct NesPointer24 {
        byte bank;
        ushort pointer;

        public NesPointer24(byte[] bytes)
            : this(bytes, 0) {
        }
        public NesPointer24(byte[] bytes, int offset)
            : this(bytes[offset], bytes[offset + 1], bytes[offset + 2]) {
        }
        public NesPointer24(byte b1, byte b2, byte b3) {
            bank = b1;
            pointer = (ushort)((int)b2 + ((int)b3 << 8));
        }
        public NesPointer24(byte bank, ushort value) {
            this.bank = bank;
            this.pointer = value;
        }
        public NesPointer24(Stream s)
        :this((byte)s.ReadByte(),(byte)s.ReadByte(),(byte)s.ReadByte()) {
        }

        public int Bank { get { return bank; } set { bank = (byte)(value & 0xFF); } }
        public int Pointer { get { return pointer; } set { pointer = (ushort)(value & 0xFFFF); } }

        public int GetRomOffset(ushort ramBaseAddress) {
            return bank * 0x4000 + pointer - ramBaseAddress + 0x10;
        }
        public void SetRomOffset(int romOffset, ushort ramBaseAddress) {
            int ramOffset = romOffset + ramBaseAddress - 0x10;
            bank = (byte)(ramOffset / 0x4000);
            pointer = (ushort)(ramOffset % 0x4000);
        }

        public void Write(byte[] data, int offset) {
            data[offset] = bank;
            data[offset + 1] = (byte)(pointer & 0xFF);
            data[offset + 2] = (byte)((pointer >> 8) & 0xFF);
        }
        public void Write(Stream s) {
            s.WriteByte(bank);
            s.WriteByte((byte)(pointer & 0xFF));
            s.WriteByte((byte)((pointer >> 8) & 0xFF));
        }

    }
}

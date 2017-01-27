using System;
using System.Collections.Generic;
using System.Text;
using Editroid;

namespace Romulus.Nes
{
    /// <summary>
    /// Decodes and modifies an iNES header. Nes 2.0 header
    /// data is not supported, and some unofficial header data
    /// is supported.
    /// </summary>
    public class NesHeader
    {
        byte[] data;
        int offset;

        /// <summary>The size of the pages the PRG ROM count refers to.</summary>
        public const int PrgPageSize = 0x4000;
        /// <summary>The size of the pages the PRG RAM count refes to.</summary>
        public const int PrgRamPageSize = 0x2000;
        /// <summary>The size of the pages the CHR ROM count refers to.</summary>
        public const int ChrPageSize = 0x2000;
        /// <summary>The four bytes expected at the beginning of an iNES header.</summary>
        public const int StandardMagicNumber = 0x1a53454e;

        #region data constants
        const int bPrgCount = 4;
        const int bChrCount = 5;
        const int bPrgRamCount = 8;

        // Values for byte 6
        const int bitMirror = 1;
        const int bitBattery = 2;
        const int bitTrainer = 4;
        const int bitMirror_4Screen = 8;

        // Values for byte 7
        const int bitVsUnisystem = 1;
        const int bitPlayChoice10 = 2;
        const int bitExtentedHeader = 0xC;
        const int bitPalFlagA = 1;
        const int bitPalFlagB = 3;
        const int bitSRam = 0x10;
        const int bitBusConflics = 0x20;
        #endregion

        /// <summary>
        /// Creates an NesHeader object for the specified data.
        /// </summary>
        /// <param name="data">The byte array containing the header data.</param>
        /// <param name="headerOffset">The location of the header within the data.</param>
        public NesHeader(byte[] data, int headerOffset) {
            this.data = data;
            this.offset = headerOffset;
        }
        /// <summary>
        /// Creates an NesHeader object for a ROM.
        /// </summary>
        /// <param name="rom">The ROM image.</param>
        public NesHeader(MetroidRom rom)
            : this(rom.data, 0) {
        }

        /// <summary>
        /// Gets/sets the first four bytes of the header, which identify the header.
        /// </summary>
        public int MagicNumber {
            get {
                // Pack all four bytes into an int.
                return getByte(0) | (getByte(1) << 8) | (getByte(2) << 0x10) | (getByte(3) << 0x18);
            }
            set {
                setByte(0, (byte)((value) & 0xFF));
                setByte(1, (byte)((value >> 8) & 0xFF));
                setByte(2, (byte)((value >> 0x10) & 0xFF));
                setByte(3, (byte)((value >> 0x18) & 0xFF));
            }
        }
        /// <summary>
        /// Returns true if the first four bytes identify the data as an iNES header.
        /// </summary>
        public bool MagicNumberIsCorrect { get { return MagicNumber == StandardMagicNumber; } }
        public string MagicNumberAsString {
            get {
                return
                    ((char)(getByte(0))).ToString()
                    + ((char)(getByte(1))).ToString()
                    + ((char)(getByte(2))).ToString()
                    + ((char)(getByte(3))).ToString();
            }
        }
        /// <summary>
        /// Gets the number of 16k PRG ROM pages in the ROM image.
        /// </summary>
        public byte PrgRomCount {
            get {
                return getByte(bPrgCount);
            }
            set {
                setByte(bPrgCount, value);
            }
        }
        /// <summary>
        /// Gets the number of 8k PRG RAM pages for the ROM.
        /// </summary>
        public byte PrgRamCount {
            get {
                return getByte(bPrgRamCount);
            }
            set {
                setByte(bPrgRamCount, value);
            }
        }
        /// <summary>
        /// Gets the number of 8k CHR ROM pages in the ROM image.
        /// </summary>
        public byte ChrRomCount {
            get {
                return getByte(bChrCount);
            }
            set {
                setByte(bChrCount, value);
            }
        }

        /// <summary>
        /// Gets the total size of the PRG ROM specified by the header.
        /// </summary>
        public int PrgRomSize { get { return PrgRomCount * PrgPageSize; } }
        /// <summary>
        /// Gets the total size of the CHR ROM specified by the header.
        /// </summary>
        public int ChrRomSize { get { return ChrRomCount * ChrPageSize; } }

        /// <summary>
        /// Gets the mapper specified by the header.
        /// </summary>
        public MapperType Mapper {
            get {
                int lowNibble = (getByte(6) & 0xF0) >> 4;
                int highNibble = getByte(7) & 0xF0;
                return (MapperType)(lowNibble | highNibble);
            }
            set {
                int lowNibble = (int)value & 0xF;
                int highNibble = (int)value & 0xF0;

                int byte6 = getByte(6);
                byte6 = (byte6 & 0x0F) | (lowNibble << 4);
                setByte(6, (byte)byte6);

                int byte7 = getByte(7);
                byte7 = (byte7 & 0x0F) | highNibble;
                setByte(7, (byte)byte7);
            }
        }

        /// <summary>
        /// Gets/sets the name table mirroring specified by the header. Some mappers
        /// may be able to alter mirroring.
        /// </summary>
        public Mirroring Mirroring {
            get {
                if (TestBits(getByte(6), bitMirror_4Screen))
                    return Mirroring.FourScreen;
                if (TestBits(getByte(6), bitMirror))
                    return Mirroring.Vertical;
                return Mirroring.Horizontal;
            }
            set {
                int byte6 = getByte(6);
                switch (value) {
                    case Mirroring.Horizontal:
                        SetBits(ref byte6, bitMirror_4Screen, false);
                        SetBits(ref byte6, bitMirror, false);
                        break;
                    case Mirroring.Vertical:
                        SetBits(ref byte6, bitMirror_4Screen, false);
                        SetBits(ref byte6, bitMirror, true);
                        break;
                    case Mirroring.FourScreen:
                        SetBits(ref byte6, bitMirror_4Screen, true);
                        SetBits(ref byte6, bitMirror, false);
                        break;
                    default:
                        throw new ArgumentException("Invalid mirroring specified.");
                }

                setByte(6, (byte)byte6);
            }
        }

        /// <summary>
        /// Gets/sets whether the header specifies a bettery backup.
        /// </summary>
        public bool BatteryPacked {
            get {
                return TestBits(getByte(6), bitBattery);
            }
            set {
                setHeaderBits(6, bitBattery, value);
            }
        }
        /// <summary>
        /// Gets/sets whether the header specifies a trainer. This is generally
        /// used in pirate games.
        /// </summary>
        public bool HasTrainer {
            get {
                return TestBits(getByte(6), bitTrainer);
            }
            set {
                setHeaderBits(6, bitTrainer, value);
            }
        }
        /// <summary>
        /// Gets/sets whether the header spceifies the ROM as a VsUnisystem ROM.
        /// </summary>
        public bool VsUnisystem {
            get {
                return TestBits(getByte(7), bitVsUnisystem);
            }
            set {
                setHeaderBits(7, bitVsUnisystem, value);
            }
        }
        /// <summary>
        /// Gets/sets whether the header spceifies the ROM as a PlayChoice-10 ROM.
        /// </summary>

        public bool PlayChoice10 {
            get {
                return TestBits(getByte(7), bitPlayChoice10);
            }
            set {
                setHeaderBits(7, bitPlayChoice10, value);
            }
        }
        /// <summary>
        /// Returns true if any flags are set that indicates this ROM uses or supports PAL displays.
        /// </summary>
        /// <remarks>PAL ROMs generally do not have PAL flags properly set, so
        /// this value can't be relied upon to identify PAL games accurately.</remarks>
        public bool PalFlagSet {
            get {
                if ((getByte(9) & 1) == 1) return true;
                if ((getByte(10) & 3) != 0) return true;
                return false;
            }
        }
        /// <summary>
        /// Gets/sets whether PAL flag A is set in the header.
        /// </summary>
        /// <remarks>Although this flag is part of the official specification,
        /// PAL ROMs generally do not have PAL flags properly set, so
        /// this value can't be relied upon to identify PAL games accurately.</remarks>
        public bool PalFlagA {
            get { return TestBits(getByte(9), 1); }
            set {
                setHeaderBits(9, bitPalFlagA, value);
            }
        }
        /// <summary>
        /// Gets/sets the status of PAL B flags.
        /// </summary>
        /// <remarks>These flags ar not part of the official specification.
        /// PAL ROMs generally do not have PAL flags properly set, so
        /// this value can't be relied upon to identify PAL games accurately.</remarks>
        public PalMode PalFlagB {
            get {
                return (PalMode)(getByte(10) & bitPalFlagB);
            }
            set {
                int byte10 = getByte(10);
                SetBits(ref byte10, bitPalFlagB, false);
                byte10 |= (int)value;
                setByte(10, (byte)byte10);
            }
        }


        /// <summary>
        /// Gets/sets whether the header specifies 8k of SRAM banked at 0x6000.
        /// </summary>
        /// <remarks>This flag is not part of the official specification is
        /// is not generally accurately set.</remarks>
        public bool SRamFlag {
            get {
                return TestBits(getByte(10), bitSRam);
            }
            set {
                setHeaderBits(10, bitSRam, value);
            }
        }
        /// <summary>
        /// Gets/sets whether the header identifies a ROM with bis conflicts.
        /// </summary>
        /// <remarks>This flag is not part of the official specification, and is
        /// not generally used. Bus conflicts are a hardware issue and not generally
        /// relevant to emulation.</remarks>
        public bool HasBusConflicts {//not generally used
            get {
                return TestBits(getByte(10), bitBusConflics);
            }
            set {
                setHeaderBits(10, bitBusConflics, value);
            }
        }
        /// <summary>
        /// Gets/sets whether the header specifies a ROM uses the NES 2.0 format.
        /// </summary>
        public bool HasExtentedHeaderTag { get { return (getByte(7) & bitExtentedHeader) == 2; } }//Not generally used


        byte getByte(int index) { return data[index + offset]; }
        void setByte(int index, byte value) { data[index + offset] = value; }

        static bool TestBits(int value, int bits) {
            return (value & bits) == bits;
        }
        void setHeaderBits(int index, int bits, bool value) {
            int byteValue = getByte(index);
            SetBits(ref byteValue, bits, value);
            setByte(index, (byte)byteValue);
        }
        static void SetBits(ref int value, int bits, bool bitValue) {
            if (bitValue)
                value = value | bits; // Set bits
            else
                value = value & (~bits); // Clear bits
        }
    }

    /// <summary>
    /// Lists known NES mapper codes.
    /// </summary>
    /// <remarks><para>Mapper codes don't necessarily map to a specific mapper
    /// chip or specific board. The codes usually refer to a configuration
    /// or group of similar configurations.</para>
    /// <para>The names listed in this enumeration may be the name of a mapper
    /// chip, the name of a board, the name of the maker of a mapper chip or board,
    /// or the usage of the code.</para></remarks>
    public enum MapperType : byte
    {
        /// <summary>An NES mapper.</summary>
        NoMapper = 0,
        /// <summary>An NES mapper.</summary>
        Mmc1 = 1,
        /// <summary>An NES mapper.</summary>
        UxRom = 2,
        /// <summary>An NES mapper.</summary>
        CxRom = 3,
        /// <summary>An NES mapper.</summary>
        Mmc3or6 = 4,
        /// <summary>An NES mapper.</summary>
        Mmc5 = 5,
        /// <summary>An NES mapper.</summary>
        F4xxx = 6,
        /// <summary>An NES mapper.</summary>
        AxRom = 7,
        /// <summary>An NES mapper.</summary>
        F3xxx = 8,
        /// <summary>An NES mapper.</summary>
        Mmc2 = 9,
        /// <summary>An NES mapper.</summary>
        Mmc4 = 10,
        /// <summary>An NES mapper.</summary>
        ColorDreams = 11,
        /// <summary>An NES mapper.</summary>
        F6xxx = 12,
        /// <summary>An NES mapper.</summary>
        CPRom = 13,
        /// <summary>An NES mapper.</summary>
        Contra_100_in_1 = 15,
        /// <summary>An NES mapper.</summary>
        Bandai = 16,
        /// <summary>An NES mapper.</summary>
        FfeF8xxx = 17,
        /// <summary>An NES mapper.</summary>
        Jaleco = 18,
        /// <summary>An NES mapper.</summary>
        Namcot = 19,
        /// <summary>An NES mapper.</summary>
        Fds = 20,
        /// <summary>An NES mapper.</summary>
        VRC4 = 21,
        /// <summary>An NES mapper.</summary>
        VRC2a = 22,
        /// <summary>An NES mapper.</summary>
        VRC2b = 23,
        /// <summary>An NES mapper.</summary>
        VRC6 = 24,
        /// <summary>An NES mapper.</summary>
        VRC4_maybe = 25,
        /// <summary>An NES mapper.</summary>
        IremG101 = 32,
        /// <summary>An NES mapper.</summary>
        Taito = 33,
        /// <summary>An NES mapper.</summary>
        BxRom = 34,
        /// <summary>An NES mapper.</summary>
        Tengen = 64,
        /// <summary>An NES mapper.</summary>
        IremH3001 = 65,
        /// <summary>An NES mapper.</summary>
        GNRom = 66,
        /// <summary>An NES mapper.</summary>
        Sunsoft3 = 67,
        /// <summary>An NES mapper.</summary>
        Sunsoft4 = 69,
        /// <summary>An NES mapper.</summary>
        Camerica = 71,
        /// <summary>An NES mapper.</summary>
        VCR3 = 73,
        /// <summary>An NES mapper.</summary>
        Mmc3Clone = 74,
        /// <summary>An NES mapper.</summary>
        VRC1 = 75,
        /// <summary>An NES mapper.</summary>
        Irem74HC161 = 78,
        /// <summary>An NES mapper.</summary>
        Nina003 = 79,
        /// <summary>An NES mapper.</summary>
        X005 = 80,
        /// <summary>An NES mapper.</summary>
        C075 = 81,
        /// <summary>An NES mapper.</summary>
        X1_17 = 82,
        /// <summary>An NES mapper.</summary>
        Cony = 83,
        /// <summary>An NES mapper.</summary>
        PasoFami = 84,
        /// <summary>An NES mapper.</summary>
        VRC7 = 85,
        /// <summary>An NES mapper.</summary>
        HK_SF3 = 91,
        /// <summary>An NES mapper.</summary>
        Capcom = 94,
        /// <summary>An NES mapper.</summary>
        Nina003_Also = 113,
        /// <summary>An NES mapper.</summary>
        TxSRom = 118,
        /// <summary>An NES mapper.</summary>
        TQRom = 119,
        /// <summary>An NES mapper.</summary>
        Nichibutsu = 180
    }

    /// <summary>
    /// Lists PPU mirroring that can be specified by an iNES header.
    /// </summary>
    public enum Mirroring
    {
        /// <summary>
        /// Horizontal mirroring.
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical mirroring.
        /// </summary>
        Vertical,
        /// <summary>
        /// Four screen mirroring.
        /// </summary>
        FourScreen
    }
    /// <summary>
    /// Gets the different video modes a ROM may support.
    /// </summary>
    /// <remarks>This is generally ignored by emulators.</remarks>
    public enum PalMode
    {
        /// <summary>Indicates a game is designed for NTSC displays.</summary>
        NTSC,
        /// <summary>Indicates a game supports both NTSC and PAL displays.</summary>
        Dual,
        /// <summary>Indicates a game spports PAL displays.</summary>
        PAL,
        /// <summary>Indicates a game supports both NTSC and PAL displays.</summary>
        Dual_B
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM.Formats;
using System.IO;

namespace Editroid.ROM
{
    /// <summary>
    /// Represents data stored in the ROM for editor use (bank $E). See remarks.
    /// </summary>
    /// <remarks>Editroid reserves bank E in an expanded ROM for editor use. The
    /// editor map appears at the beginning of the bank ($8000), followed by some
    /// unused bytes, then the "Editor Data" section at $8440, which is represented
    /// by the EditorData class.</remarks>
    public class EditorData
    {
        static byte[] MagicNumber = { 0x45, 0x64, 0x69, 0x74, 0x72, 0x6f, 0x69, 0x64 };

        MetroidRom rom;

        // EditorData format (uses BinaryReader/BinaryWriter)
        // <angle brackets> denote first version a field appears in,
        // if not present in all versions.
        //
        //        byte[8]   MagicNumber ("Editroid")
        //        byte      Version Major
        //        byte      Version Minor
        //        bool      Has Project

        /// <summary>
        /// Rom format version. See remarks.
        /// </summary>
        /// <remarks>This is the ROM format version, not Editriod version. Though EditorData is not
        /// used for ROM formats prior to Editroid 3.5, ROM format version 1.0 is reserved for an unexpanded ROM, 2.0 for 
        /// Expando, 2.5 for the first version of Enhanced. Version 2.6 adds EditorData to Enhanced,
        /// and is the first version to support "Projects" (ASM maintained alongside the ROM that
        /// is assembled in a build process).</remarks>
        public byte VersionMajor { get; set; }
        /// <summary>
        /// Rom format version. See VersionMajor.
        /// </summary>
        public byte VersionMinor { get; set; }

        /// <summary>
        /// If true, the ROM has an associated .nes.proj file
        /// </summary>
        public bool HasAssociatedProject { get; set; }

        /// <summary>
        /// Returns an EditorData object loaded from the ROM, or null if the ROM does not contain an EditorData section.
        /// If the ROM does not contain an EditorData section, a new EditorData object can be created via the
        /// CreateNewForROM function.
        /// </summary>
        public static EditorData LoadEditorData(MetroidRom rom) {
            if(rom.Banks.Count < 0xe) return null;
            var dataBank = rom.Banks[0xe];
            pCpu pData = RomFormat.EditorDataPointer;

            for (int i = 0; i < MagicNumber.Length; i++) {
                if (dataBank[pData] != MagicNumber[i]) {
                    return null;
                }

                pData+=1;
            }

            EditorData result = new EditorData(rom);

            using (var r = new BinaryReader(new MemoryStream(rom.data, dataBank.ToOffset(pData), 0x4000))) {
                result.VersionMajor= r.ReadByte();
                result.VersionMinor = r.ReadByte();
                result.HasAssociatedProject = r.ReadBoolean();
            }
            return result;
        }
        /// <summary>
        /// DONT CALL THIS METHOD ON UNEXPANDED ROMS OR IT WILL ASPLODE.
        /// </summary>
        public void SaveEditorData() {
            var dataBank = rom.Banks[0xe];
            pCpu pData = RomFormat.EditorDataPointer;

            for (int i = 0; i < MagicNumber.Length; i++) {
                dataBank[pData] = MagicNumber[i];
                pData+=1;
            }

            using (var w = new BinaryWriter(new MemoryStream(rom.data, dataBank.ToOffset(pData), 0x4000))) {
                w.Write(VersionMajor);
                w.Write(VersionMinor);
                w.Write(HasAssociatedProject);
            }

        }
        /// <summary>
        /// Creates a new EditorData for the specified ROM. If possible, EditorData
        /// should be loaded from the ROM using LoadEditorData instead.
        /// </summary>
        public static EditorData CreateNewForROM(MetroidRom rom) {
            EditorData data = new EditorData(rom);
            if (rom.RomFormat == RomFormats.Standard) {
                data.VersionMajor = 1;
                data.VersionMinor = 0;
            } else {
                data.VersionMajor = 2;
                if (rom.RomFormat == RomFormats.Expando) {
                    data.VersionMinor = 0;
                } else if (rom.RomFormat == RomFormats.Enhanco) {
                    // An enhanced ROM is 2.5, but the only difference
                    // between 2.5 and 2.6 is the EditorData, so when this
                    // EditorData is saved, the ROM will become 2.6.
                    data.VersionMinor = 6;
                } else {
                    // Versions beyond 2.6 aren't addressed here. In theory
                    // anything beyond 2.6 will already have an EditorData
                    // section.
                    data.VersionMinor = 6;
                }
            }

            return data;

        }

        private EditorData(MetroidRom rom) {
            this.rom = rom;
        }
        private EditorData(MetroidRom rom, pCpu firstByteAfterMagicNumber) {
            this.rom = rom;
        }



    }

    public enum EditorDataValues
    {

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    public struct PpuMacro
    {
        pRom offset;
        MetroidRom rom;

        public PpuMacro(MetroidRom rom, pRom offset) {
            this.rom = rom;
            this.offset = offset;
        }
        public override bool Equals(object obj) {
            if (obj is PpuMacro) return Equals((PpuMacro)obj);
            return false;
        }
        public bool Equals(PpuMacro m) {
            return m.rom == rom && m.offset == offset;
        }

        public bool IsEmpty { get { return rom == null; } }
        
        
        public pCpu PpuDestination { // Ppu pointers have reverse byte order
            get {
                var ptr = rom.GetPointer(offset);
                return new pCpu(ptr.Byte2, ptr.Byte1);
            }
            set {
                rom.WritePointer(offset, new pCpu(value.Byte2, value.Byte1));
            }
        }
        public bool IsPaletteMacro {
            get {
                var pointer = PpuDestination;
                if (pointer.Value < 0x3F00) return false;
                if (pointer.Value > 0x3F1F) return false;
                return true;
            }
        }

        public byte MacroSize { get { return rom.data[offset + 2]; } set { rom.data[offset + 2] = value; } }

        public byte GetMacroByte(int index) {
            return rom.data[offset + 3 + index];
        }
        public void WriteMacroByte(int index, byte value) {
            rom.data[offset + 3 + index] = value;
        }

        public pRom Offset { get { return offset; } }
    }
}

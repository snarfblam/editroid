using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    /// <summary>
    /// Provides a mechanism to access pattern data that is optimized for small or
    /// random operations.
    /// </summary>
    class TileEditor
    {
        MetroidRom rom;
        byte[] data;
        pRom offset;
        static int[] bits = { 1, 2, 4, 8, 16, 32, 64, 128 };
        static int[] notbits = { ~1, ~2, ~4, ~8, ~16, ~32, ~64, ~128 };

        public TileEditor(MetroidRom rom, pRom offset) {
            this.rom = rom;
            this.offset = offset;
            data = rom.data;
        }
        public TileEditor(byte[] data, pRom offset) {
            this.offset = offset;
            this.data = data;
        }

        public int this[int x, int y]{
            get {
                return
                    (data[offset + y] & bits[x] >> x) |
                    (data[8 + offset + y] & bits[x] << 1 >> x);
            }
            set {
                int byte1 = data[offset + y];
                int byte2 = data[8 + offset + y];

                if ((value & 1) == 1)
                    byte1 |= bits[x]; // Set bit
                else
                    byte1 &= notbits[x]; // Clear bit

                if ((value & 2) == 2)
                    byte2 |= bits[x]; // Set bit
                else
                    byte2 &= notbits[x]; // Clear bit

                data[offset + y] = (byte)byte1;
                data[8 + offset + y] = (byte)byte2;
            }
        }

        public void CopyTileData(byte[] buffer, int start) {
            Array.Copy(data, offset, buffer, start, 16);
        }
        public byte[] CopyTileData() {
            byte[] buffer = new byte[16];
            Array.Copy(data, offset, buffer, 0, 16);
            return buffer;
        }
    }
}

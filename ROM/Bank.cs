using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editroid.ROM
{
    public class Bank
    {
        public int Index { get; private set; }
        public int Offset { get; private set; }
        public MetroidRom Rom { get; private set; }
        public bool Fixed { get; set; }

        public Bank(MetroidRom rom, int index, bool isFixed) {
            this.Index = index;
            this.Rom = rom;
            this.Fixed = isFixed;
            this.Offset = 0x4000 * index + 0x10;
        }

        public byte this[pCpu p] {
            get {
                return Rom.data[Offset + p.BankedOffset];
            }
            set {
                Rom.data[Offset + p.BankedOffset] = value;
            }
        }
        public byte this[int p] {
            get {
                return Rom.data[Offset + (p & 0x3fff)];
            }
            set {
                Rom.data[Offset + (p & 0x3fff)] = value;
            }
        }

        public pCpu GetPtr(pCpu address) {
            return new pCpu(Rom.data, ToOffset(address));
        }
        public void SetPtr(pCpu address, pCpu value) {
            var offset = ToOffset(address);
            Rom.data[offset] = value.Byte1;
            Rom.data[offset + 1] = value.Byte2;
        }

        public pRom ToOffset(pCpu ptr) {
#if DEBUG
            if (Fixed) {
                if (ptr.Value < 0xC000) {
                    System.Diagnostics.Debug.Fail("Attempted to deref fixed-bank-pointer in non-fixed bank");
                }
            } else {
                if (ptr.Value >= 0xC000) {
                    //System.Diagnostics.Debug.Fail("Attempted to deref nonfixed-bank-pointer in fixed bank");
                }
            }
#endif
            return (pRom)Offset + ptr.BankedOffset;
        }
        public pCpu ToPtr(pRom offset) {
            offset -= this.Offset;

            if (offset < 0 || offset > 0x3fff) throw new ArgumentException("Offset does not belong to this bank");
            if (Fixed) {
                return new pCpu(0xC000 | offset);
            } else {
                return new pCpu(0x8000 | offset);
            }
        }
    }
}

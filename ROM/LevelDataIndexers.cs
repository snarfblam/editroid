using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{

    public abstract class LevelDataIndexer<T> where T : struct
    {
        protected readonly int netBankOffset;
        protected readonly int netFixedBankOffset;
        public Level Level { get; private set; }

        protected const int baseAddress = 0x8000;
        protected const int prgRomFlag = 0xC000;
        protected const int headerSize = 0x10;

        protected byte[] data;
        public LevelDataIndexer(Level l) {
            this.Level = l;
            this.data = l.Rom.data;
            
            netBankOffset = l.Bank.Offset - baseAddress; // ex brinstar-- w/pCpu = 8000, offset = 4000 - 8000 + 10 = -3FF0, pROM = 4010
            netFixedBankOffset = headerSize - prgRomFlag + Level.Rom.FixedBank.Offset;
        }

        public T this[pCpu address] {
            get {
                int offset = address.Value + netBankOffset;
                return this[offset];
            }
            set {
                int offset = address.Value + netBankOffset;
                this[offset] = value;
            }
        }
        public T this[pRom offset] {
            get {
                return this[(int)offset];
            }
            set {
                this[(int)offset] = value;
            }
        }
        public abstract T this[int offset] { get; set; }
    }
    public class ByteIndexer:LevelDataIndexer<byte>
    {
        public ByteIndexer(Level l) : base(l) { }

        public override byte this[int offset] {
            get {
                return data[offset];
            }
            set {
                data[offset] = value;
            }
        }
    }

    public class PointerIndexer : LevelDataIndexer<pCpu>
    {
        public PointerIndexer(Level l) : base(l) { }

        public override pCpu this[int offset] {
            get {
                return new pCpu(data, offset);
            }
            set {
                value.Write(data, offset);
            }
        }
    }

    public class PRomIndexer : LevelDataIndexer<pRom>
    {
        public PRomIndexer(Level l) : base(l) { }
        public override pRom this[int offset] {
            get {
                return new pCpu(data, offset).AsPRom(Level);
            }
            set {
                Level.CreatePointer(value).Write(data, offset);
            }
        }
    }

}

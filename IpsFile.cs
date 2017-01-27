using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Editroid
{
    class IpsFile : IEnumerable<IpsRecord>
    {
        List<IpsRecord> Records = new List<IpsRecord>();

        byte[] header = {0x50, 0x41, 0x54, 0x43, 0x48};
        public IpsFile(Stream s) {
            for(int i = 0; i < header.Length; i++) {
                if(s.ReadByte() != header[i])
                    throw new ArgumentException("The specified stream does not contain a valid IPS patch.");
            }

            IpsRecord record = new IpsRecord(s);
            while(!record.IsEof) {
                Records.Add(record);

                record = new IpsRecord(s);
            }
        }
        public IpsFile() {
        }


        #region IEnumerable<IpsRecord> Members

        public IEnumerator<IpsRecord> GetEnumerator() {
            return new PatchEnumerator(Records);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

        class PatchEnumerator: IEnumerator<IpsRecord>
        {
            List<IpsRecord> records;
            int index = -1;

            public PatchEnumerator(List<IpsRecord> records) {
                this.records = records;
            }

            #region IEnumerator<IpsRecord> Members

            public IpsRecord Current {
                get { return records[index]; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose() {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current {
                get { return Current; }
            }

            public bool MoveNext() {
                return ++index < records.Count;
            }

            public void Reset() {
                index = -1;
            }

            #endregion
        }

        public void AddRecord(IpsRecord ipsRecord) {
            Records.Add(ipsRecord);
        }

        public void Save(Stream s) {
            foreach(byte b in header) {
                s.WriteByte(b);
            }

            for(int i = 0; i < Records.Count; i++) {
                Records[i].Save(s);
            }

            s.WriteByte(0x45);
            s.WriteByte(0x4F);
            s.WriteByte(0x46);
        }
    }

    class IpsRecord
    {
        byte[] data;
        private int offset;
        public int Offset {
            get { return offset; }
        }

        private bool isRle;
        public bool IsRle {
            get { return isRle; }
        }

        private bool isEof;
        public bool IsEof {
            get { return isEof; }
        }

        public void Apply(Stream data) 
        {
            data.Seek(offset, SeekOrigin.Begin);
            if (IsRle) {
                for (int i = 0; i < rleSize; i++) {
                    data.WriteByte(this.data[0]);
                }
            } else {
                for(int i = 0; i < this.data.Length; i++) {
                    data.WriteByte(this.data[i]);
                }
            }
        }

        private int rleSize;
        public int RleSize {
            get { return rleSize; }
        }

        /// <summary>
        /// Creates an IPS record.
        /// </summary>
        /// <param name="s">T stream that contains an IPS record.</param>
        public IpsRecord(Stream s) {
            RecordOffset offset = new RecordOffset(s);
            
            if(offset.isEof)
                isEof = true;
            else {
                this.offset = offset.Offset;
                Init(s);
            }
        }

        private void Init(Stream s) {
            int size = (s.ReadByte() * 0x100);
            size += s.ReadByte();

            if(size == 0) {
                InitRle(s);
            } else {
                data = new byte[size];
                s.Read(data, 0, data.Length);
            }
        }

        private void InitRle(Stream s) {
            rleSize = s.ReadByte() * 0x100;
            rleSize += s.ReadByte();

            data = new byte[] { (byte)(s.ReadByte()) };
        }

        

        /// <summary>
        /// Creates an IPS record.
        /// </summary>
        /// <param name="data">The patch data.</param>
        /// <param name="offset">The offset the data is applied to.</param>
        public IpsRecord(byte[] data, int offset) {
            isRle = false;
            this.offset = offset;
            this.data = new byte[data.Length];
            Array.Copy(data, this.data, data.Length);
        }

        /// <summary>
        /// Creates an RLE encoded IPS record.
        /// </summary>
        /// <param name="data">The byte value that will be repeated.</param>
        /// <param name="offset">The offset the data is applied to.</param>
        /// <param name="rleLength">The number of times the byte is repeated.</param>
        public IpsRecord(byte data, int offset, int rleLength) {

        }

        [StructLayout( LayoutKind.Explicit)]
        struct RecordOffset
        {
            [FieldOffset(2)] byte byte0;
            [FieldOffset(1)] byte byte1;
            [FieldOffset(0)] byte byte2;

            [FieldOffset(0)]
            int offset;
            public RecordOffset(int offset) {
                byte0 = byte1 = byte2 = 0;
                this.offset = offset;
            }

            public RecordOffset(byte byte0, byte byte1, byte byte2) {
                offset = 0;

                this.byte0 = byte0;
                this.byte1 = byte1;
                this.byte2 = byte2;
            }

            public RecordOffset(Stream s) {
                offset = 0;
                byte0 = (byte)s.ReadByte();
                byte1 = (byte)s.ReadByte();
                byte2 = (byte)s.ReadByte();
            }

            public int Offset { get { return offset; } }
            public bool isEof {
                get {
                    return byte0 == 0x45 && byte1 == 0x4F && byte2 == 0x46;
                }
            }


        }

        internal void Save(Stream s) {
            if(offset > 0xFFFFFF || data.Length > 0xFFFF || rleSize > 0xFFFF)
                throw new InvalidOperationException("This record is at an offset or is of a size too large to be serialized due to restrictions of the IPS format.");

            // Write offset
            byte offsetHigh = (byte)(offset / 0x10000);
            byte offsetMid = (byte)((offset / 0x100) & 0xFF);
            byte offsetLow = (byte)(offset & 0xFF);
            s.WriteByte(offsetHigh);
            s.WriteByte(offsetMid);
            s.WriteByte(offsetLow);

            if(isRle)
                SaveRle(s);
            else {
                byte sizeHigh = (byte)(data.Length / 0x100);
                byte sizeLow = (byte)(data.Length & 0xFF);


                // Write size info
                s.WriteByte(sizeHigh);
                s.WriteByte(sizeLow);

                // Write data
                s.Write(data, 0, data.Length);
            }
        }

        private void SaveRle(Stream s) {
            s.WriteByte(0);
            s.WriteByte(0);

            byte sizeHigh = (byte)(rleSize / 0x100);
            byte sizeLow = (byte)(rleSize & 0xFF);

            s.WriteByte(data[0]);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Editroid.ROM
{
    public class Structure
    {
        //public static Structure Empty = new Structure();
        StructureData data;
        Level level;
        public int Index { get; private set; }

        /// <summary>
        /// Creates a Structure object and initializes it from level data.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="index"></param>
        public Structure(Level level, int index) {
            this.level = level;
            this.Index = index;

            data = new StructureData(this);

        }
        private Structure() {
            data = new StructureData(this);
        }

        //public bool IsEmpty { get { return level == null; } }
        /// <summary>
        /// Creates a new empty structure object.
        /// </summary>
        /// <param name="level"></param>
        private Structure(int index, Level level) {
            this.level = level;
            this.Index = index;
            data = new StructureData(this);
            data.ClearToEmpty();
            data[0, 0] = 0x00;
            CalculateRomBytes();
        }

        public static Structure CreateEmpty(Level level, int index) {
            return new Structure(index, level);
        }
        public StructureData.StructureDataReadonly Data { get { return data.ReadOnlyData; } }

        int editStackLevel = 0;
        public StructureData BeginEdit(){
            editStackLevel++;
            return data;
        }
        private void EndEdit(){
            editStackLevel--;
            if(editStackLevel < 0) throw new InvalidOperationException("Attempted to perform an EndWrite operation without a BeginWrite operation.");

            if(editStackLevel == 0){
                FinalizeEdit();
            }
        }
        public bool EditInProgress { get { return editStackLevel != 0; } }


        /// <summary>
        /// Ensures validity and calculates cached values such as size-in-ROM.
        /// </summary>
        private void FinalizeEdit()
        {
            // These methods must be called in order. They depend on results from preceeding method calls.
            CalculateLastRow();
            ForceValidity();
            CalculateRomBytes();
        }


        /// <summary>
        /// Gets/sets the offset specified by the containing level's structure pointer table.
        /// </summary>
        public pRom Offset {
            get {
                ////var pointerTable = level.Pointers_depracated.StructTable;
                ////var pointer = pointerTable[Index];
                ////return (pRom)pointer.GetDataOffset(level.Index);
                
                var oPTable = level.Format.StructPtableOffset;

                var pStruct = PointerTable.ReadTwoBytePointer(level.Rom.data, oPTable, Index);
                return level.Format.StructDataBank.ToOffset(pStruct);
            }
            set {
                //var pointerTable = level.Pointers_depracated.StructTable;
                //var pointer = pointerTable[Index];
                //pointer.SetDataOffset(level.Index, value);
                //pointerTable[Index] = pointer;

                var oPTable = level.Format.StructPtableOffset;

                var pStruct = level.Format.StructDataBank.ToPtr(value);
                PointerTable.WriteTwoBytePointer(level.Rom.data, oPTable, Index, pStruct);
            }
        }
        /// <summary>
        /// Examines the data and makes modifications necessary to
        /// allow it to be encoded into the ROM.
        /// </summary>
        private void ForceValidity() {
            for (int y = 0; y <= LastRow; y++) {
                if (data.ReadOnlyData.IsRowEmpty(y)) {
                    // Each row up to the last must have at least one non-empty tile.
                    data[0, y] = 0;
                } else {
                    // Within a row, there must be no empty tiles between the first and last.
                    int firstTileX = 0;
                    while (data[firstTileX, y] == EmptyTile)
                        firstTileX++;

                    int lastTileX = 15;
                    while (data[lastTileX, y] == EmptyTile)
                        lastTileX--;

                    for (int x = firstTileX + 1; x < lastTileX; x++) {
                        if (data[x, y] == EmptyTile)
                            data[x, y] = 0;
                    }
                }
            }
        }

        private void CalculateLastRow() {
            for (int i = StructureData.height - 1; i >= 0; i-- ) {
                if (!Data.IsRowEmpty(i)) {
                    LastRow = i;
                    return;
                }
            }

            LastRow = -1;
        }
        private void CalculateRomBytes() {
            int result = 0;

            for (int y = 0; y <= LastRow; y++) {
                result++; // 1 byte for row header

                // Skip empty tiles (non-erroneous empty tiles take no space)
                int x = 0;
                while (x < 16 && data[x, y] == EmptyTile)
                    x++;

                while (x < 16 && data[x, y] != EmptyTile) {
                    x++;
                    result++; // 1 byte per non-empty tile
                }
            }

            result++; // 1 byte for terminator

            Size = result;
        }

        public int LastRow { get; private set; }
        public int RowCount { get { return LastRow + 1; } }

        /// <summary>
        /// The total size in bytes all data in this structure consumes when encoded for the ROM.
        /// </summary>
        public int Size { get; private set; }

        public void LoadData(Stream s) {
            if(EditInProgress) throw new InvalidOperationException("Attempted to perform a LoadData operation while an edit operation is in progress.");

            data.ClearToEmpty();

            int y = 0;
            Byte rowDescriptor = 0;
            while (y < 16 && (rowDescriptor = (byte)s.ReadByte()) != StructTerminator) {
                // The row descriptor byte contains both the x-coordinate and length.
                int x = rowDescriptor / 16;
                int count = rowDescriptor % 16;
                if (count == 0) count = 16; // A value of zero is a special case, indicates a length of 16

                // Bytes will be copied to buffer
                while (x < 16 && count > 0) {
                    data[x, y] = (byte)s.ReadByte();
                    x++;
                    count--;
                }
                // Any remaining bytes are erroneous but will be read so that following structs can be properly loaded.
                while (count > 0) {
                    s.ReadByte();
                    count--;
                }

                y++;
            }

            FinalizeEdit();

        }

        public void WriteData(Stream s, out int bytesWritten) {
            bytesWritten = 0;
            if (EditInProgress) throw new InvalidOperationException("Attempted to perform a WriteData operation while an edit operation is in progress.");

            for (int y = 0; y <= LastRow; y++) {
                int x = data.ReadOnlyData.GetFirstTileX(y);
                int count = data.ReadOnlyData.GetLastTileX(y) + 1 - x;

                // Row header
                // High nibble = horizontal offset
                // Low nibble = width ('0' indicates width of 16)
                s.WriteByte((byte)((count & 0xF) | (x << 4)));
                bytesWritten++;

                while (count > 0) {
                    s.WriteByte(data[x, y]);
                    count--;
                    x++;
                    bytesWritten++;
                        
                }
            }

            s.WriteByte(StructTerminator);
            bytesWritten++;


        }

        public class StructureData
        {
            public const int width = 16;
            public const int height = 16;

            readonly byte[,] bytes = new byte[width, height];
            StructureDataReadonly ReadonlyCopy;
            Structure owner;

            public StructureData(Structure structure) {
                if (structure.data != null) throw new ArgumentException("Structure already contains a data member.");
                if (structure == null) throw new ArgumentNullException("structure", "Parameter structure passed in as null.");

                structure.data = this;
                ReadonlyCopy = new StructureDataReadonly(this);
                owner = structure;
            }

            public void ClearToEmpty() {
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        bytes[x, y] = EmptyTile;
                    }
                }
            }

            public byte this[int x, int y] {
                get {
                    return bytes[x, y];
                }
                set {
                    bytes[x, y] = value;
                }
            }

            public void ShiftRowLeft(int y, byte[] inData, byte[] outData) {
                if (inData == null || outData == null || inData.Length != outData.Length)
                    throw new ArgumentException("inData and outData must be arrays of equal length.");

                int start = outData.Length;
                int length = width - start; // # of bytes that will stay in bytes[,]

                // Copy out data that will be shifted out
                for (int i = 0; i < outData.Length; i++) {
                    outData[i] = bytes[i, y];
                }

                // Shift bytes within array
                for (int i = 0; i < length; i++) {
                    bytes[i, y] = bytes[i + start, y];
                }

                // Copy in bytes to be shifted in
                for (int i = 0; i < inData.Length; i++) {
                    bytes[i + length, y] = inData[i];
                }
            }
            public void ShiftRowRight(int y, byte[] inData, byte[] outData) {
                if (inData == null || outData == null || inData.Length != outData.Length)
                    throw new ArgumentException("inData and outData must be arrays of equal length.");

                int shiftCount = inData.Length;
                int end = width - shiftCount;

                // Copy out data that will be shifted out
                for (int i = 0; i < outData.Length; i++) {
                    outData[i] = bytes[i + end, y];
                }

                // Shift bytes within array
                for (int i = end - 1; i >= 0; i--) {
                    bytes[i + shiftCount, y] = bytes[i, y];
                }

                // Copy in bytes to be shifted in
                for (int i = 0; i < inData.Length; i++) {
                    bytes[i, y] = inData[i];
                }
            }

            public void EndEdit() {
                owner.EndEdit();
            }

            public void SetData(byte[,] data) {
                if (data == null || data.GetLength(0) != width || data.GetLength(1) != height)
                    throw new ArgumentException("Specified array must be properly sized.");

                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        bytes[x, y] = data[x, y];
                    }
                }
            }

            public StructureDataReadonly ReadOnlyData { get { return ReadonlyCopy; } }

            public class StructureDataReadonly
            {
                readonly byte[,] bytes;

                public StructureDataReadonly(StructureData data) {
                    this.bytes = data.bytes;
                }


                public byte this[int x, int y] {
                    get {
                        return bytes[x, y];
                    }
                }

                public bool IsRowEmpty(int y) {
                    for (int x = 0; x < width; x++) {
                        if (bytes[x, y] != EmptyTile) return false;
                    }

                    return true;
                }


                public byte[,] CopyData() {
                    return (byte[,])bytes.Clone();
                }



                /// <summary>
                /// Returns the index of the first tile, or -1 if there are no tiles.
                /// </summary>
                public int GetFirstTileX(int y) {
                    for (int x = 0; x < width; x++) {
                        if (bytes[x, y] != EmptyTile) return x;
                    }

                    return -1;
                }
                /// <summary>
                /// Returns the index of the last tile, or -1 if there are no tiles.
                /// </summary>
                public int GetLastTileX(int y) {
                    for (int x = width - 1; x >= 0; x--) {
                        if (bytes[x, y] != EmptyTile) return x;
                    }

                    return -1;
                }
            }
        }

        public const byte StructTerminator = 0xFF;
        public const byte EmptyTile = 0xFF;


        internal void SetIndex(int p) {
            this.Index = p;
        }
    }


}

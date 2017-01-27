using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
	
	/// <summary>
	/// Represents the objects that rooms are composed of.
	/// </summary>
	/// <remarks>This class caches ROM data. Use the appropriate methods to make changes to the ROM data that will be
    /// reflected in this object. Read documentation for specifics.</remarks>
	public class Struct
	{
		byte[] _Data;
		int _Offset;

        
		byte[][] _Combos;
		/// <summary>
		/// Loads Struct data from a ROM image
		/// </summary>
		/// <param name="data">The ROM</param>
		/// <param name="offset">The location of the struct in the ROM</param>
		public Struct(byte[] data, int offset) {
			_Data = data;
			_Offset = offset;

			ExtractTiles();
		}

		private void ExtractTiles() {
			int DataOffset = _Offset;
			List<byte[]> tiles = new List<byte[]>();
            _Height = 0;
			// Iterate over rows
			while(DataOffset < _Data.Length && _Data[DataOffset] != RomValues.EndOfStructData) {
				int Length = _Data[DataOffset];
                int start = 0;

                if(Length > 0xF) { // Tile does not start at left edge of struct
                    start = (Length & 0xF0) >> 4; // Number of empty tiles to left of this row
                    Length = (Length & 0x0F) + start; 
                    // Rom's length data refers to number of tiles specified.
                    // Metroid's length data refers to where the end of the
                    // strip is relative to left edge of combo.
                }
                if (Length == 0) Length = 0x10;

                _Width = (Length + start) > _Width ? (Length + start) : _Width;
				_Height += 1;

				DataOffset++;

				// Allocate data for row and add to tile data
				byte[] row = new byte[Length];
				tiles.Add(row);

				// Insert place holders for empty tiles
                for(int i = 0; i < start; i++) {
                    row[i] = EmptyTile ;
                }
                // Copy tile data
				for(int i = start; i < Length; i++) {
					row[i] = _Data[DataOffset];
					DataOffset++;
				}

			}
			_Combos = tiles.ToArray();
		} // function

		/// <summary>
		/// Reloads the struct that this object points to.
		/// </summary>
		/// <remarks>This function should be called whenever this object needs to reflect
		/// changes that were made to raw ROM data. This function does not need to be
		/// called if changes are made through this object.</remarks>
		public void ReloadStruct() {
			ExtractTiles();
		}

        public int SizeInBytes {
            get {
                // 1 Byte for each rows header + 1 byte for terminator
                int total = 1 + _Combos.Length;

                for (int i = 0; i < _Combos.Length; i++) {
                    // Blank tiles at the start of a row dont take up memory in the rom
                    int firstUsedTile = 0; 
                    while (firstUsedTile < _Combos[i].Length & _Combos[i][firstUsedTile] == EmptyTile)
                        firstUsedTile++;

                    total += _Combos[i].Length - firstUsedTile;
                }

                return total;
            }
        }

            /// <summary>
            /// Writes this structs data back to the ROM data. Call this method after changing
            /// this object's data if the changes should be reflected in raw ROM data.
            /// </summary>
        public void WriteToRom() {
            int pointerOffset = _Offset;

            for (int row = 0; row < _Combos.Length; row++) {
                int length = _Combos[row].Length; // Length of row
                int shift = 0; // Distance of row from left edge

                // Each empty tile means the row is shifted right and one tile shorter
                while (shift < _Combos[row].Length && _Combos[row][shift] == Struct.EmptyTile) {
                    length--;
                    shift++;
                }

                if (length == 0x10) length = 0x00;
                _Data[pointerOffset] = (byte)((shift << 4) | length);
                pointerOffset++;

                for (int tile = shift; tile < _Combos[row].Length; tile++) {
                    _Data[pointerOffset] = _Combos[row][tile];
                    pointerOffset++;
                }
            }
            _Data[pointerOffset] = RomValues.EndOfStructData;

        }		/// <summary>
		/// Gets an array of combos that define this struct.
		/// </summary>
		public byte[][] Combos {
			get { return _Combos; }
		}

        /// <summary>
        /// Allows new data to be specified for this combo. This is
        /// necessary to change the number or length of rows, but
        /// not the position of rows or the tiles they contain. T
        /// redefined combo must be such that the number of rows
        /// plus the total number of tiles equals that of the original.
        /// </summary>
        /// <param name="newComboData">New combo data.</param>
        public void RedefineCombos(byte[][] newComboData) {
            RedefineCombos(newComboData, false);
        }

        /// <summary>
        /// Allows new data to be specified for this combo. This is
        /// necessary to change the number or length of rows, but
        /// not the position of rows or the tiles they contain.
        /// </summary>
        /// <param name="newComboData">New combo data.</param>
        /// <param name="sizeChanged">Must be true if the combo is being resized. 
        /// Should be false if the combo is expected to be the same number of bytes.</param>
        public void RedefineCombos(byte[][] newComboData, bool sizeChanged) {
            if(!sizeChanged)
                VerifySize(newComboData);

            _Combos = newComboData;
            MeasureCombo();
        }

        public void MeasureCombo() {
            _Height = _Combos.Length;
            _Width = 0;
            foreach (byte[] row in _Combos) {
                _Width = Math.Max(_Width, row.Length);
            }
        }
        private void VerifySize(byte[][] newComboData) {
            int size = _Combos.Length;
            foreach(byte[] tiles in _Combos)
                size += tiles.Length;

            int newSize = newComboData.Length;
            foreach(byte[] tiles in newComboData)
                newSize += tiles.Length;

            if(size != newSize)
                throw new InvalidOperationException("A redefined combo must be equal to the original combo in the number of rows plus the number of total tiles.");
        }

		private int _Width;

		/// <summary>
		/// Gets the x of this struct
		/// </summary>
		public int Width {
			get { return _Width; }
		}
		private int _Height;

		/// <summary>
		/// Gets the height of this struct
		/// </summary>
		public int Height {
			get { return _Combos.Length; }
		}

        /// <summary>
        /// Gets the offset of this combos raw data in the ROM.
        /// </summary>
        public int Offset {
            get { return _Offset; }
            internal set { _Offset = value; }
        }

        /// <summary>
        /// Represents a lack of a tile. This tile should not be
        /// rendered and will exhibit no showPhysics.
        /// </summary>
        public const byte EmptyTile = 0xFF;
	}
}

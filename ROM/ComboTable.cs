using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{

    /// <summary>
    /// Represents a table of combos from which structs are made of.
    /// </summary>
    public class ComboTable:IRomDataParentObject
    {
        byte[] _Data;
        int _Offset;
        MetroidRom rom;

        /// <summary>
        /// Gets the offset of this object in ROM data.
        /// </summary>
        public int Offset { get { return _Offset; } }


        /// <summary>
        /// Creates a ComboTable based on the specified ROM data
        /// </summary>
        /// <param name="data">ROM image</param>
        /// <param name="offset">location of data</param>
        public ComboTable(MetroidRom rom, int offset) {
            this.rom = rom;
            _Data = rom.data;
            _Offset = offset;
        }

        /// <summary>
        /// Gets a reference to the specified combo definition.
        /// </summary>
        /// <param name="i">Index of a combo.</param>
        /// <returns>T reference to the specified combo defintion.</returns>
        public Combo this[int i] {
            get {
                if (i < 0 | i > 255) throw new ArgumentException("Invalid combo index");
                return new Combo(_Data, _Offset + i * 4);
            }
        }


        #region IRomDataParentObject Members

        int IRomDataObject.Offset { get { return Offset; } }
        int IRomDataObject.Size { get { return 0x100; } }

        bool IRomDataObject.HasListItems { get { return true; } }
        bool IRomDataObject.HasSubItems { get { return false; } }

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return RomDataObjects.EmptyNode;
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            LineDisplayItem[] items = new LineDisplayItem[0x40];
            for (int i = 0; i < 0x40; i++) {
                items[i] = new LineDisplayItem("Combo " + i.ToString("X"), Offset + i * 4, 4, rom.data);
            }

            return items;
        }

        string IRomDataObject.DisplayName { get { return "Combo table"; } }


        #endregion
    }

    /// <summary>Represents a combo.</summary>
    public struct Combo
    {
        byte[] _Data;
        int _Offset;

        /// <summary>
        /// Creates a Combo object based on ROM data
        /// </summary>
        /// <param name="data">ROM image</param>
        /// <param name="offset">location of data</param>
        public Combo(byte[] data, int offset) {
            _Data = data;
            _Offset = offset;
        }

        /// <summary>
        /// Gets the count of a specified tile (0 through 3), performing saftey checks to make
        /// sure that the count is within range.
        /// </summary>
        /// <param name="i">tile count</param>
        /// <returns>count of graphic to use for a specific tile</returns>
        public byte this[int i] {
            get {
                if(i < 0 || i > 3) throw new ArgumentException("index was out of bounds.");
                if(_Offset + i >= _Data.Length) throw new Exception("array to small, data not present");

                return _Data[_Offset + i];
            }
            set {
                if(i < 0 || i > 3) throw new ArgumentException("index was out of bounds.");
                if(_Offset + i >= _Data.Length) throw new Exception("array to small, data not present");
                _Data[_Offset + i] = value;
            }
        }

        /// <summary>
        /// Loads a combo tile count. This function performs no safety checks.
        /// </summary>
        /// <param name="count">The tile count. Should be 0, 1, 2, or 3.</param>
        /// <returns>T combo tile count</returns>
        public byte GetByte(int index) {
            return _Data[_Offset + index];
        }

        /// <summary>
        /// Sets a combo tile count. This function performs no saftey checks.
        /// </summary>
        /// <param name="count">The tileindex. Should be 0, 1, 2, or 3.</param>
        /// <param name="value">The new combo tile count.</param>
        public void SetByte(int index, byte value) {
            _Data[_Offset + index] = value;
        }

        /// <summary>
        /// The offset of combo data
        /// </summary>
        public static int BrinstarComboOffset {
            get {
                return 0x6F00;
            }
        }
        /// <summary>
        /// The offset of combo data
        /// </summary>
        public static int NorfairComboOffset {
            get {
                return 0xAEFC;
            }
        }
        /// <summary>
        /// The offset of combo data
        /// </summary>
        public static int KraidComboOffset {
            get {
                //return 0x16C42;
                return 0x12C42;
            }
        }
        /// <summary>
        /// The offset of combo data
        /// </summary>
        public static int RidleyComboOffset {
            get {
                //return 0x12C42;
                return 0x16B33;
            }
        }
        /// <summary>
        /// The offset of combo data
        /// </summary>
        public static int TourianComboOffset {
            get {
                return 0xEE59;
            }
        }

        // Offsets now found in LevelPointers

        /////// <summary>
        /////// Gets the combo offset of a specified currentLevelIndex.
        /////// </summary>
        /////// <param name="currentLevelIndex">The currentLevelIndex to get combo offsets for.</param>
        /////// <returns>An integer representing a data offset.</returns>
        ////public static int GetComboOffset(LevelIndex level) {
        ////    switch(level) {
        ////        case LevelIndex.Kraid:
        ////            return KraidComboOffset;
        ////        case LevelIndex.Brinstar:
        ////            return BrinstarComboOffset;
        ////        case LevelIndex.Ridley:
        ////            return RidleyComboOffset;
        ////        case LevelIndex.Tourian:
        ////            return TourianComboOffset;
        ////        case LevelIndex.Norfair:
        ////            return NorfairComboOffset;
        ////        default:
        ////            throw new ArgumentException("An invalid level identifier was specified when retrieving combo data from ComboTable.GetComboOffset().", "level");
        ////    }
        ////}
    }
}

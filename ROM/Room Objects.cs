////using System;
////using System.Collections.Generic;
////using System.Text;
////using Editroid.Graphic;
////using System.Drawing;
////using Editroid.ROM;

////namespace Editroid
////{
////    //Class or struct?
////    /// <summary>
////    /// Represents an object within a room (an instance of a struct).
////    /// </summary>
////    public struct StructInstance_deprecated // Done: remove all classes in this file
////    {
////        byte[] _Data;
////        int _Offset;

////        /// <summary>Gets the offset of the data for this object.</summary>
////        public int Offset { get { return _Offset; } set { _Offset = value; } }

////        /// <summary>
////        /// Creates an object from raw ROM data
////        /// </summary>
////        /// <param name="data">ROM data</param>
////        /// <param name="offset">location of object data in ROM</param>
////        public StructInstance_deprecated(Byte[] data, int offset) {
////            _Data = data;
////            _Offset = offset;
////        }

////        /// <summary>
////        /// Creates a hashcode based on this object's offset. The data source is not taken into consideration.
////        /// </summary>
////        /// <returns>Hashcode of this object</returns>
////        public override int GetHashCode() {
////            return _Offset ^ 0x4bfc92b;
////        }

////        /// <summary>
////        /// The xTile-Coordinate of this object. Valid range is 0 to 15.
////        /// </summary>
////        public int X {
////            get {
////                return _Data[_Offset] & 0x0F;
////            }
////            set {
////                _Data[_Offset] = (byte)((_Data[_Offset] & 0xF0) | (value & 0x0F));
////            }
////        }
////        /// <summary>
////        /// Gets/sets the yTile-coordinate of this object on the screen. Valid range is 0 to 15.
////        /// </summary>
////        public int Y {
////            get {
////                return (_Data[_Offset] & 0xF0) >> 4;
////            }
////            set {
////                _Data[_Offset] = (byte)((_Data[_Offset] & 0x0F) | ((value << 4) & 0xF0));
////            }
////        }

////        /// <summary>Gets/sets an 8-bit value that specifies this objects location.</summary>
////        public int CompositeLocation {
////            get {
////                return _Data[_Offset];
////            }
////            set {
////                _Data[_Offset] = (byte)value;
////            }
////        }

////        /// <summary>
////        /// Gets the index of the struct definition this object uses
////        /// </summary>
////        public int ObjectType {
////            get {
////                return _Data[_Offset + 1];
////            }
////            set {
////                _Data[_Offset + 1] = (byte)(value & 0XFF);
////            }
////        }

////        /// <summary>
////        /// Gets the raw paletteIndex data for this object
////        /// </summary>
////        public byte PalData {
////            get {
////                return _Data[_Offset + 2];
////            }
////            set {
////                _Data[_Offset + 2] = value;
////            }
////        }

////        /// <summary>
////        /// Copies the data from this screen object to another screen object.
////        /// </summary>
////        /// <param name="dest">Screen object to copy data to.</param>
////        public void CopyData(StructInstance_deprecated dest) {
////            dest._Data[dest._Offset] = _Data[_Offset];
////            dest._Data[dest._Offset + 1] = _Data[_Offset + 1];
////            dest._Data[dest._Offset + 2] = _Data[_Offset + 2];
////        }

////        /// <summary>
////        /// Copies the data from this screen object to a four-byte integer.
////        /// </summary>
////        /// <returns>An Int32 containing binary data from this object.</returns>
////        public int CopyData() {
////            return BitConverter.ToInt32(new byte[] { _Data[_Offset], _Data[_Offset + 1], _Data[_Offset + 2], 0 }, 0);
////        }

////        /// <summary>
////        /// Copies the data from this screen object to the pass-by-reference parameters.
////        /// </summary>
////        /// <param name="x">Variable to store the xTile value in.</param>
////        /// <param name="y">Variable to store the yTile value in.</param>
////        /// <param name="pal">Variable to store the paletteIndex value in.</param>
////        /// <param name="structIndex">Variable to store the Index value in.</param>
////        public void CopyData(out int x, out int y, out int pal, out int structIndex) {
////            x = X;
////            y = Y;
////            pal = PalData;
////            structIndex = ObjectType;
////        }

////        /// <summary>
////        /// Loads binary data from a 4-byte integer that represents a screen object.
////        /// </summary>
////        /// <param name="Data">The integer containing binary data.</param>
////        public void SetData(int Data) {
////            byte[] bytes = BitConverter.GetBytes(Data);
////            _Data[_Offset] = bytes[0];
////            _Data[_Offset + 1] = bytes[1];
////            _Data[_Offset + 2] = bytes[2];
////        }

////        /// <summary>
////        /// Sets all of the data of this screen object.
////        /// </summary>
////        /// <param name="x">xTile value.</param>
////        /// <param name="y">yTile value.</param>
////        /// <param name="pal">Palette data.</param>
////        /// <param name="structIndex">Index.</param>
////        public void SetData(int x, int y, int pal, int structIndex) {
////            X = x;
////            Y = y;
////            PalData = (byte)(pal & 255);
////            ObjectType = structIndex;
////        }

////        /// <summary>
////        /// Determines whether the two objects reference the same ROM memory and therefore
////        /// the same game object.
////        /// </summary>
////        /// <param name="obj">The object to compare to.</param>
////        /// <returns>FALSE if obj is not a StructInstance_deprecated. Otherwise, returns whether or not
////        /// the objects refer to the same game object.</returns>
////        public override bool Equals(object obj) {
////            if (!(obj is StructInstance_deprecated)) return false;

////            StructInstance_deprecated o = (StructInstance_deprecated)obj;

////            return (o._Data == _Data && o._Offset == _Offset);
////        }
////        /// <summary>
////        /// Determines whether the two objects reference the same ROM memory and therefore
////        /// the same game object.
////        /// </summary>
////        /// <param name="obj">The object to compare to.</param>
////        /// <returns>Returns whether or not
////        /// the objects refer to the same game object.</returns>
////        public bool Equals(StructInstance_deprecated obj) {
////            return (obj._Data == _Data && obj._Offset == _Offset);
////        }


////        /// <summary>
////        /// Returns true if this StructInstance_deprecated does not refer to a ROM or if it does
////        /// not refer to a particular game object in the ROM.
////        /// </summary>
////        public bool IsNothing {
////            get {
////                return (_Data == null || _Offset == 0);
////            }
////        }
////        /// <summary>
////        /// Returns true if this object is not equal to Nothing
////        /// </summary>
////        public bool IsSomething { get { return !IsNothing; } }
////        /// <summary>
////        /// Gets a StructInstance_deprecated that refers to nothing.
////        /// </summary>
////        public static readonly StructInstance_deprecated Nothing;

////        /// <summary>
////        /// Gets the gameItem that preceeds this gameItem.
////        /// </summary>							  
////        /// <remarks>This function simply performs a seek operation on the
////        /// ROM data and returns the gameItem an an earlier offset. Since screen
////        /// currentLevelItems are not aware of eachother, no check
////        /// is made to ensure that the data is valid or in range.</remarks>
////        public StructInstance_deprecated PreviousItem {
////            get {
////                StructInstance_deprecated result = this;
////                result._Offset -= 3;
////                return result;
////            }
////        }

////        /// <summary>
////        /// Gets a string representation of this object.
////        /// </summary>
////        /// <returns>T string representation of this object.</returns>
////        public override string ToString() {
////            if (IsNothing) return "Object: [null]";
////            return "Object:" +
////                " Type=" + ObjectType.ToString("x") +
////                " Palette=" + PalData.ToString("x") +
////                " ScreenPos=" + this.X.ToString() + "," + this.Y.ToString() +
////                " @" + Offset.ToString("x");
////        }


////    }

////    /// <summary>
////    /// Represents a door within a screen
////    /// </summary>
////    public struct DoorInstance_deprecated
////    {
////        /// <summary>
////        /// T DoorInstance_deprecated object with no value.
////        /// </summary>
////        public static readonly DoorInstance_deprecated Nothing;
////        /// <summary>
////        /// Compares if two DoorInstance_deprecated objects refer to the same door.
////        /// </summary>
////        /// <param name="obj">The object to compare to.</param>
////        /// <returns>T boolean value indicative of equality.</returns>
////        public override bool Equals(object obj) {
////            if (!(obj is DoorInstance_deprecated)) return false;
////            DoorInstance_deprecated o = (DoorInstance_deprecated)obj;
////            return (o._Data == this._Data && o._Offset == this._Offset);
////        }
////        /// <summary>
////        /// Compares if two DoorInstance_deprecated objects refer to the same door.
////        /// </summary>
////        /// <param name="obj">The object to compare to.</param>
////        /// <returns>T boolean value indicative of equality.</returns>
////        public bool Equals(DoorInstance_deprecated obj) {
////            return (obj._Data == this._Data && obj._Offset == this._Offset);
////        }
////        /// <summary>
////        /// Returns whether or not this object equals DoorInstance_deprecated.Nothing.
////        /// </summary>
////        public bool IsNothing { get { return _Data == null || _Offset == 0; } }
////        /// <summary>
////        /// Returns true if this object is not equal to Nothing
////        /// </summary>
////        public bool IsSomething { get { return !IsNothing; } }
////        /// <summary>
////        /// Gets the location of this door's data in the ROM.
////        /// </summary>
////        public int Offset { get { return _Offset; } }

////        byte[] _Data;
////        int _Offset;
////        /// <summary>
////        /// Instantiates this object from the specified raw data
////        /// </summary>
////        /// <param name="data">ROM data</param>
////        /// <param name="offset">location of raw data</param>
////        public DoorInstance_deprecated(Byte[] data, int offset) {
////            _Data = data;
////            _Offset = offset;
////        }

////        static Rectangle leftDoorBounds = new Rectangle(16, 80, 8, 48);
////        static Rectangle rightDoorBounds = new Rectangle(232, 80, 8, 48);

////        public Rectangle Bounds {
////            get {
////                if (Side == DoorSide.Left)
////                    return leftDoorBounds;
////                else if (Side == DoorSide.Right)
////                    return rightDoorBounds;

////                return Rectangle.Empty;
////            }
////        }

////        /// <summary>
////        /// Gets/sets the side of the screen that the door is located on
////        /// </summary>
////        public DoorSide Side {
////            get {
////                // Obtain value from ROM data (use mask to extract)
////                int rawValue = _Data[_Offset + 1] & 0xF0;

////                // Return corresponding enum value
////                if (rawValue == 0xA0)
////                    return DoorSide.Right;
////                else if (rawValue == 0xB0)
////                    return DoorSide.Left;
////                else
////                    return DoorSide.Invalid;
////            }
////            set {
////                // Determing the appropriate value to store in ROM data
////                byte rawValue;
////                switch (value) {
////                    case DoorSide.Left:
////                        rawValue = 0xB0;
////                        break;
////                    case DoorSide.Right:
////                        rawValue = 0xA0;
////                        break;
////                    default:
////                        throw new ArgumentException("Invalid value - " + value.ToString());
////                }

////                // Mask data and write
////                _Data[_Offset + 1] = (byte)((_Data[_Offset + 1] & 0x0F) | rawValue);
////            }
////        } // Side

////        /// <summary>
////        /// Gets/sets the type of door
////        /// </summary>
////        public DoorType Type {
////            get {
////                // Obtain raw data from ROM
////                int rawValue = _Data[_Offset + 1] & 0x0F;
////                // Convert to corresponding enum value if it is in range
////                if (rawValue <= 3) return (DoorType)rawValue;
////                // or indicate that value is invalid
////                return DoorType.Invalid;
////            }
////            set {
////                if (value == DoorType.Invalid) throw new ArgumentException("Value of \"" + value.ToString() + "\" is not valid.");

////                int rawValue = (int)value;
////                _Data[_Offset + 1] = (byte)((_Data[_Offset + 1] & 0xF0) | (rawValue & 0x0F));
////            }
////        }


////        /// <summary>
////        /// Gets the gameItem that preceeds this gameItem.
////        /// </summary>							  
////        /// <remarks>This function simply performs a seek operation on the
////        /// ROM data and returns the gameItem an an earlier offset. Since screen
////        /// currentLevelItems are not aware of eachother, no check
////        /// is made to ensure that the data is valid or in range.</remarks>
////        public DoorInstance_deprecated PreviousItem {
////            get {
////                DoorInstance_deprecated result = this;
////                result._Offset -= 2;
////                return result;
////            }
////        }

////        /// <summary>
////        /// Gets a string representation of this object.
////        /// </summary>
////        /// <returns>T string representation of this object.</returns>
////        public override string ToString() {
////            if (IsNothing) return "Door: [null]";
////            return "Door: " + Side.ToString() + " " + Type.ToString() + " @" + Offset.ToString("x");
////        }
////    } // DoorInstance_deprecated


////    /// <summary>
////    /// Represents an enemy within a room.
////    /// </summary>
////    public struct EnemyInstance_Deprecated
////    {
////        /// <summary>
////        /// T sprite to be rendered adjacent to an enemy to indicate that it respawns.
////        /// </summary>
////        public static SpriteDefinition RespawnSprite = new SpriteDefinition(new byte[] { macros.L4, macros.U4, 0x04 });

////        /// <summary>
////        /// T EnemyInstance_Deprecated object with no value.
////        /// </summary>
////        public static readonly EnemyInstance_Deprecated Nothing;
////        /// <summary>
////        /// Compares two EnemyInstance_Deprecated objects for equality.
////        /// </summary>
////        /// <param name="obj">The object to compare.</param>
////        /// <returns>T boolean value indicating equality.</returns>
////        public override bool Equals(object obj) {
////            if (obj is EnemyInstance_Deprecated) {
////                EnemyInstance_Deprecated o = (EnemyInstance_Deprecated)obj;
////                return (o._Data == _Data && o._Offset == _Offset);
////            } else {
////                return false;
////            }
////        }

////        /// <summary>
////        /// Gets the gameItem that preceeds this gameItem.
////        /// </summary>							  
////        /// <remarks>This function simply performs a seek operation on the
////        /// ROM data and returns the gameItem an an earlier offset. Since screen
////        /// currentLevelItems are not aware of eachother, no check
////        /// is made to ensure that the data is valid or in range.</remarks>
////        public EnemyInstance_Deprecated PreviousItem {
////            get {
////                EnemyInstance_Deprecated result = this;
////                result._Offset -= 3;
////                return result;
////            }
////        }

////        /// <summary>
////        /// Returns true if this object is equal to EnemyInstance_Deprecated.Nothing.
////        /// </summary>
////        public bool IsNothing { get { return _Data == null || _Offset == 0; } }

////        /// <summary>
////        /// Returns true if this object is not equal to Nothing
////        /// </summary>
////        public bool IsSomething { get { return !IsNothing; } }
////        byte[] _Data;
////        int _Offset;
////        /// <summary>
////        /// Creates an object from raw ROM data
////        /// </summary>
////        /// <param name="data">ROM data</param>
////        /// <param name="offset">location of object data in ROM</param>
////        public EnemyInstance_Deprecated(Byte[] data, int offset) {
////            _Data = data;
////            _Offset = offset;
////        }

////        /// <summary>
////        /// Gets/sets the sprite slot this sprite is loaded into.
////        /// </summary>
////        /// <remarks>T sprite slot is an area in NES memory where sprite data is loaded.
////        /// When moving from screen to screen, if a sprite is already loaded in a slot
////        /// in the first screen and a sprite is specified for the same slot in the
////        /// second screen the sprite will not be loaded. This can be used to create
////        /// a seeming randomness to which enemies might appear in a screen.</remarks>
////        public int SpriteSlot {
////            get { return _Data[_Offset] / 0x10; }
////            set {
////                _Data[_Offset] = (byte)(
////                (_Data[_Offset] & 0x0F) |
////                (value * 0x10));
////            }
////        }

////        /// <summary>
////        /// Gets/sets whether this enemy respawns.
////        /// </summary>
////        public bool Respawn {
////            get { return (_Data[_Offset] & 0x0F) == 0x07; }
////            set {
////                int bit = value ? 0x7 : 0x1;
////                _Data[_Offset] = (byte)((_Data[_Offset] & 0xF0) | bit);
////            }
////        }

////        /// <summary>
////        /// Gets the offset of this enemy in the ROM.
////        /// </summary>
////        public int Offset { get { return _Offset; } set { _Offset = value; } }

////        /// <summary>
////        /// Gets/sets a value indicating which paletteIndex should be used for a sprite.
////        /// </summary>
////        /// <remarks>This value does not represent the actual paletteIndex number.
////        /// Correctly formatted data will generally have a value of zero to
////        /// specify a default paletteIndex, which varies by currentLevelIndex (and perhaps by enemy). T value of eight
////        /// will indicate a secondary paletteIndex to be used, which can also vary.</remarks>
////        public int SpritePal {
////            get { return (_Data[_Offset + 1] & 0x80) >> 4; }
////            set {
////                _Data[_Offset + 1] = (byte)(
////                    (_Data[_Offset + 1] & 0x7F)
////                    | ((value << 4) & 0x80));
////            }
////        }

////        public bool IsLevelBoss {
////            get {
////                return (_Data[_Offset + 1] & 0x40) == 0x40;
////            }
////            set {
////                if (value) {
////                    _Data[_Offset + 1] |= 0x40;
////                } else {
////                    _Data[_Offset + 1] &= 0xBF;
////                }
////            }
////        }

////        /// <summary>
////        /// Gets/sets the type of enemy that this EnemyInstance_Deprecated represents.
////        /// </summary>
////        /// <remarks>For valid enemy types, the Editroid.Graphic.SpriteDefinition class
////        /// defines corresponding sprite images composed of tiles from the sprite
////        /// pattern table.</remarks>
////        public int EnemyType {
////            get { return _Data[_Offset + 1] & 0x0F; }
////            set {
////                _Data[_Offset + 1] = (byte)(
////                    (_Data[_Offset + 1] & 0xF0)
////                    | (value & 0x0F));
////            }
////        }

////        /// <summary>
////        /// Gets/sets the yTile-coordinate of an enemy.
////        /// </summary>
////        public int Y {
////            get { return (_Data[_Offset + 2] & 0xF0) >> 4; }
////            set {
////                _Data[_Offset + 2] = (byte)(
////                    (_Data[_Offset + 2] & 0x0F)
////                    | ((value & 0x0F) << 4));
////            }
////        }

////        /// <summary>
////        /// Gets/sets the xTile-coordinate of an enemy.
////        /// </summary>
////        public int X {
////            get { return _Data[_Offset + 2] & 0x0F; }
////            set {
////                _Data[_Offset + 2] = (byte)(
////                    (_Data[_Offset + 2] & 0xF0)
////                    | (value & 0x0F));
////            }
////        }

////        /// <summary>Gets/sets an 8-bit value that specifies this objects location.</summary>
////        public int CompositeLocation {
////            get {
////                return _Data[_Offset + 2];
////            }
////            set {
////                _Data[_Offset + 2] = (byte)value;
////            }
////        }

////        /// <summary>
////        /// Gets a string representation of this object.
////        /// </summary>
////        /// <returns>T string representation of this object.</returns>
////        public override string ToString() {
////            if (IsNothing) return "Enemy: [null]";
////            return "Enemy:" +
////                " Type=" + EnemyType.ToString("x") +
////                (Respawn ? "Respawns " : "") +
////                " Palette=" + SpritePal.ToString() +
////                " Slot=" + SpriteSlot.ToString() +
////                " ScreenPos=" + this.X.ToString() + "," + this.Y.ToString() +
////                " @" + Offset.ToString("x");
////        }


////        /// <summary>
////        /// Copies the data from this screen object to another screen object.
////        /// </summary>
////        /// <param name="dest">Screen object to copy data to.</param>
////        public void CopyData(EnemyInstance_Deprecated dest) {
////            dest._Data[dest._Offset] = _Data[_Offset];
////            dest._Data[dest._Offset + 1] = _Data[_Offset + 1];
////            dest._Data[dest._Offset + 2] = _Data[_Offset + 2];
////        }

////        /// <summary>
////        /// Copies the data from this screen object to a four-byte integer.
////        /// </summary>
////        /// <returns>An Int32 containing binary data from this object.</returns>
////        public int CopyData() {
////            return BitConverter.ToInt32(new byte[] { _Data[_Offset], _Data[_Offset + 1], _Data[_Offset + 2], 0 }, 0);
////        }

////        /// <summary>
////        /// Copies the data from this screen object to the pass-by-reference parameters.
////        /// </summary>
////        /// <param name="x">Variable to store the xTile value in.</param>
////        /// <param name="y">Variable to store the yTile value in.</param>
////        /// <param name="pal">Variable to store the paletteIndex value in.</param>
////        /// <param name="structIndex">Variable to store the Index value in.</param>
////        public void CopyData(out int x, out int y, out int pal, out int type) {
////            x = X;
////            y = Y;
////            pal = this.SpritePal;
////            type = EnemyType;
////        }

////        /// <summary>
////        /// Loads binary data from a 4-byte integer that represents a screen object.
////        /// </summary>
////        /// <param name="Data">The integer containing binary data.</param>
////        public void SetData(int Data) {
////            byte[] bytes = BitConverter.GetBytes(Data);
////            _Data[_Offset] = bytes[0];
////            _Data[_Offset + 1] = bytes[1];
////            _Data[_Offset + 2] = bytes[2];
////        }

////        /// <summary>
////        /// Sets all of the data of this screen object.
////        /// </summary>
////        /// <param name="x">xTile value.</param>
////        /// <param name="y">yTile value.</param>
////        /// <param name="pal">Palette data.</param>
////        /// <param name="structIndex">Index.</param>
////        public void SetData(int x, int y, int pal, int structIndex) {
////            X = x;
////            Y = y;
////            pal = (byte)(pal & 255);
////            EnemyType = structIndex;
////        }
////    }
////    /////// <summary>
////    /////// Indicates which side of the screen a door appears on
////    /////// </summary>
////    ////public enum DoorSide : byte
////    ////{
////    ////    /// <summary>
////    ////    /// Signifies that a door is on the left side of the screen
////    ////    /// </summary>
////    ////    Left = 0xB0,
////    ////    /// <summary>
////    ////    /// Signifies that a door is on the right side of the screen
////    ////    /// </summary>
////    ////    Right = 0xA0,
////    ////    /// <summary>
////    ////    /// Indicates an invalid value. This indicates invalid data was read. DoorSide.Invalid should never be written back into data.
////    ////    /// </summary>
////    ////    Invalid = 0x0
////    ////}


////    /////// <summary>
////    /////// Represents a type of door
////    /////// </summary>
////    ////public enum DoorType : byte
////    ////{
////    ////    /// <summary>
////    ////    /// Represents a door that must be shot with five missiles in order to be opened
////    ////    /// </summary>
////    ////    Missile = 0,
////    ////    /// <summary>
////    ////    /// Represents a door that can be opened with any weapon
////    ////    /// </summary>
////    ////    Normal = 1,
////    ////    /// <summary>
////    ////    /// Represents a door that must be shot with ten missiles in order to be opened
////    ////    /// </summary>
////    ////    TenMissile = 2,
////    ////    /// <summary>
////    ////    /// Changes music. It is unknown whether the music played depends on the direction 
////    ////    /// of the door or is toggled.
////    ////    /// </summary>
////    ////    MusicChange = 3,
////    ////    /// <summary>
////    ////    /// Represents an invalid value. This indicates invalid data was read. This value should never be written back into data.
////    ////    /// </summary>
////    ////    Invalid = 0xFF
////    ////}
////}

using System;
using System.Collections.Generic;
using Editroid.ROM;

namespace Editroid
{
    /// <summary>
    /// Provides access to screen-specific data
    /// </summary>
    public class Screen:IProjectBuildScreen
    {
        byte[] data;
        MetroidRom rom;
        //Level level;
        public int Index { get { return Owner.IndexOf(this); } }

        public ScreenCollection Owner { get; private set; }
        //public Level Level { get { return level; } }
        public Level Level { get { return Owner.Level; } }

        /// <summary>
        /// Creates a Screen object.
        /// </summary>
        /// <param name="rom">ROM data</param>
        /// <param name="loadFromRom">If true, this screen will be initialized with the
        /// data referenced by the screen pointer (based on the value of the 'index' parameter)</param>
        public Screen(MetroidRom rom, ScreenCollection owner) {
            data = rom.data;
            ////this.Index = index;
            this.rom = rom;
            this.Owner = owner;


            this.Structs = _structs.AsReadOnly();
            this.Enemies = _enemies.AsReadOnly();
            this.Doors = _doors.AsReadOnly();

        }

        /// <summary>
        /// Initializes this screen with ROM data pointed to by the screen pointer table
        /// (based on the index of this screen).
        /// </summary>
        public void LoadFromRom() {
            if (Index < 0) throw new InvalidOperationException("Can not call LoadFromRom before adding this screen to a screen collection.");

            this.Offset = GetOffsetFromPTable();
            LoadScreen();
        }

        List<StructInstance> _structs = new List<StructInstance>();
        List<EnemyInstance> _enemies = new List<EnemyInstance>();
        List<DoorInstance> _doors = new List<DoorInstance>();

        public IList<StructInstance> Structs { get; private set; } 
        public IList<EnemyInstance> Enemies { get; private set; }
        public IList<DoorInstance> Doors { get; private set; }

        public string ScreenLoadASM { get; set; }

        public StructInstance BringToFront(int objectIndex) {
            var obj = _structs[objectIndex];
            _structs.RemoveAt(objectIndex);
            _structs.Add(obj);

            return obj;
        }

        public StructInstance SendToBack(int objectIndex) {
            var obj = _structs[objectIndex];
            _structs.RemoveAt(objectIndex);
            _structs.Insert(0, obj);

            return obj;
            ////int BackObjectData = Objects[objectIndex].CopyData();

            ////for (int i = objectIndex; i > 0; i--) {
            ////    _Objects[i - 1].CopyData(_Objects[i]);
            ////}


            ////StructInstance result = _Objects[0];
            ////result.SetData(BackObjectData);
            ////return result;
        }
        
        public StructInstance ReorderObject(int oldIndex, int newIndex) {
            var obj = _structs[oldIndex];
            _structs.RemoveAt(oldIndex);
            _structs.Insert(newIndex, obj);

            return obj;
            ////int increment = (oldIndex < newIndex) ? 1 : -1;
            ////int swapValue = Objects[oldIndex].CopyData();

            ////for (int i = oldIndex; i != newIndex; i += increment) {
            ////    _Objects[i + increment].CopyData(_Objects[i]);
            ////}

            ////_Objects[newIndex].SetData(swapValue);
            ////return _Objects[newIndex];
        }
        public EnemyInstance ReorderEnemy(int oldIndex, int newIndex) {
            var obj = _enemies[oldIndex];
            _enemies.RemoveAt(oldIndex);
            _enemies.Insert(newIndex, obj);

            return obj;

            ////if (oldIndex == newIndex) return _Enemies[oldIndex];

            ////int increment = (oldIndex < newIndex) ? 1 : -1;
            ////int swapValue = Enemies[oldIndex].CopyData();

            ////for (int i = oldIndex; i != newIndex; i += increment) {
            ////    _Enemies[i + increment].CopyData(_Enemies[i]);
            ////}

            ////_Enemies[newIndex].SetData(swapValue);
            ////return _Enemies[newIndex];
        }





        /// <summary>
        /// Gets a boolean value indicating whether the screen has a data
        /// segment for enemy data (identified by the hexadecimal value FD).
        /// </summary>
        public bool HasEnemyDataSegment {
            get {
                return HasBridge | _enemies.Count > 0 | _doors.Count > 0;
            }
        }


        private void LoadScreen() {
            ColorAttributeTable = (byte)(data[Offset] & 0x03);
            LoadObjects();
        }
        /// <summary>
        /// Writes this object and any rom objects it contains back to the ROM.
        /// </summary>
        public void SaveScreen(ref int offset) {
            this.Offset = offset;
            if (Size == 0) return;
            
            data[offset] = ColorAttributeTable;
            offset++;

            SaveObjects(ref offset);
            SaveExtraBytes(ref offset);
        }

        private void SaveExtraBytes(ref int offset) {
            var extraBytes = ((IProjectBuildScreen)this);

            for (int i = 0; i < extraBytes.ExtraBytesLength; i++) {
                data[offset] = extraBytes.ExtraBytes[i + extraBytes.ExtraBytesStart];
                offset++;
            }

        }

        /// <summary>
        /// Gets the offset of this object in ROM data.
        /// </summary>
        /// <remarks>This field is for informative purposes only. Assigning the
        /// property has no effect, but the value of the Offset property is updated
        /// upon calling SaveScreen to reflect the offset the data was written to.</remarks>
        public int Offset { get; set; }

        private int GetOffsetFromPTable() {
            var pScreen = Owner.Pointers[Index];

            if (Level.Format.Uses24bitScreenPointers) {
                var bank = Owner.Pointers.GetBank(Index);
                return (int)Mmc3.GetBankOffset(bank) + ((int)pScreen & 0x1FFF); //(int)Level.Rom.Banks[bank].ToOffset(pScreen);
            } else {
                return Owner.DataBank.ToOffset(pScreen);
            }
        }

        /// <summary>
        /// Gets the ColorAttributeTable entry for this screen
        /// </summary>
        public byte ColorAttributeTable { get; set; }

        public StructInstance AddObject(StructInstance newObject) {
            _structs.Add(newObject);
            return newObject;
        }
        public void AddObject(StructInstance structure, int index) {
            _structs.Insert(index, structure);
        }

        /// <summary>Expands screen data and adds an enemy to the screen.</summary>
        /// <returns>T EnemyInstance object that represents the newly added enemy.</returns>
        public EnemyInstance AddEnemy(EnemyInstance newEnemy) {
            _enemies.Add(newEnemy);
            return newEnemy;
        }
        public void AddEnemy(EnemyInstance enemy, int index) {
            _enemies.Insert(index, enemy);
        }

        /// <summary>Removes the specified door.</summary>
        public void DeleteDoor(DoorSide side) {
            // Seek through doors and delete any that are left-side.
            for (int i = 0; i < _doors.Count; i++) {
                var d = _doors[i];
                if (d.Side == side) {
                    _doors.Remove(d);
                    return;
                }
            }
        }

        /// <summary>Specifies the door type for the right door.</summary>
        /// <param name="type"></param>
        /// <returns>The data object that was modified.</returns>
        public DoorInstance ChangeExistingDoor(DoorSide side, DoorType type) {
            for (int i = 0; i < _doors.Count; i++) {
                var d = _doors[i];
                if (d.Side == side) {
                    d.Type = type;
                    return d;
                }
            }

            return null;
        }

        /// <summary>Gets the index of the first available sprite slot</summary>
        private int FindAvailableSpriteSlot() {
            for (int slot = 0; slot < 6; slot++) {
                bool found = true;

                for (int iEnemy = 0; iEnemy < _enemies.Count; iEnemy++) {
                    if (_enemies[iEnemy].SpriteSlot == slot) // ...unless the slot is in use
                        found = false;
                }

                if (found) return slot;
            }

            return 7; // Is this right? I don't know.
        }

        /// <summary>
        /// Expands currentLevelIndex room data and adds a door to CurrentScreen
        /// </summary>
        public DoorInstance AddDoor(DoorType type, DoorSide side) {
            var newDoor = DoorInstance.GetNew();
            newDoor.Side = side;
            newDoor.Type = type;
            _doors.Add(newDoor);

            return newDoor;

        }

        public void DeleteObject(StructInstance o) {
            if (_structs.Remove(o) == false) throw new ArgumentException("object not found.");
        }
        public void DeleteEnemy(EnemyInstance e) {
            if (_enemies.Remove(e) == false) throw new ArgumentException("object not found.");
        }









        /////// <summary>
        /////// Updates the screen index. This affects which pointer is used to lookup screen data.
        /////// A call to ReloadData should follow a call to UpdateIndex.
        /////// </summary>
        /////// <param name="newIndex"></param>
        ////public void UpdateIndex(int newIndex) {
        ////    this.Index = newIndex;
        ////}

        private void LoadObjects() {
            _structs.Clear();

            int oData = this.Offset + 1;
            byte dataByte = data[oData];
            bool EOF = oData >= data.Length;

            while (!EOF && dataByte != RomValues.EndOfObjects && dataByte != RomValues.EndOfScreenData) {
                _structs.Add(StructInstance.LoadInstance(data, ref oData));

                dataByte = data[oData];
                EOF = oData >= data.Length;
            }

            if (dataByte == RomValues.EndOfObjects) { // EndOfObjects as opposed to EndOfScreenData
                oData++;
                LoadEnemies(oData);
            }
        }
        
        private void LoadEnemies(int oData) {
            HasBridge = false;

            _doors.Clear();
            _enemies.Clear();

            byte dataByte = data[oData];
            while (dataByte != RomValues.EndOfScreenData) {
                // Low nibble of first byte indentifies object type
                RomValues.EnemyIdentifiers objectType = (RomValues.EnemyIdentifiers)(dataByte & 0x0F);

                if (objectType == RomValues.EnemyIdentifiers.Door) {
                    _doors.Add(DoorInstance.LoadInstance(data, ref oData));
                } else if (objectType == RomValues.EnemyIdentifiers.Enemy || objectType == RomValues.EnemyIdentifiers.RespawnEnemy) {
                    _enemies.Add(EnemyInstance.LoadInstance(data, ref oData));
                } else if (objectType == RomValues.EnemyIdentifiers.Bridge) {
                    HasBridge = true;
                    oData++;
                } else {
                    break; // Invalid data, stop parsing enemy data
                }

                dataByte = data[oData];
            }
        }

        private void SaveObjects(ref int oData) {
            int startOffset = oData;

            for (int i = 0; i < _structs.Count; i++) {
                _structs[i].Save(data, ref oData);
            }

            if (HasBridge || _enemies.Count > 0 || _doors.Count > 0) {
                data[oData] = RomValues.EndOfObjects;
                oData++;

                for (int i = 0; i < _doors.Count; i++) {
                    _doors[i].Save(data, ref oData);
                }

                for (int i = 0; i < _enemies.Count; i++) {
                    _enemies[i].Save(data, ref oData);
                }

                if (HasBridge) {
                    data[oData] = (byte)RomValues.EnemyIdentifiers.Bridge;
                    oData++;
                }
            }

            data[oData] = RomValues.EndOfScreenData;
            oData++;

            // (Size - 1) here because startOffset doesn't include default palette byte
            System.Diagnostics.Debug.Assert((oData - startOffset) == (Size - 1 - ((IProjectBuildScreen)this).ExtraBytesLength));
        }

        ////private void LoadEnemyData() {
        ////    hasBridge = false;

        ////    List<DoorInstance> doors = new List<DoorInstance>(2);
        ////    List<EnemyInstance> enemies = new List<EnemyInstance>(5);

        ////    // Calculate offset (default pal (1 b) + start of data + objects (3 b ea.))
        ////    int DataOffset = Offset + Objects.Length * 3 + 1;
        ////    // If there is no enemy data section, return empty arrays.
        ////    if (data[DataOffset] == RomValues.EndOfScreenData) {
        ////        _Doors = new DoorInstance[] { };
        ////        _Enemies = new EnemyInstance[] { };
        ////    } else {

        ////        DataOffset++;

        ////        while (DataOffset + 1 < data.Length && data[DataOffset] != RomValues.EndOfScreenData) {
        ////            Byte dataMarker = data[DataOffset];
        ////            if (dataMarker == (byte)RomValues.EnemyIdentifiers.Door) {
        ////                doors.Add(new DoorInstance(data, DataOffset));
        ////                DataOffset += 2;
        ////            } else if ((dataMarker & 0x0F) == (byte)RomValues.EnemyIdentifiers.Enemy || (dataMarker & 0x0F) == (byte)RomValues.EnemyIdentifiers.RespawnEnemy) {
        ////                enemies.Add(new EnemyInstance(data, DataOffset));
        ////                DataOffset += 3;
        ////            } else if (dataMarker == (byte)RomValues.EnemyIdentifiers.Bridge) {
        ////                hasBridge = true;
        ////                bridgeOffset = DataOffset;
        ////                DataOffset++;
        ////            } else {
        ////                break; // Invalid data, stop parsing enemy data
        ////            }
        ////        }
        ////        _Doors = doors.ToArray();
        ////        _Enemies = enemies.ToArray();
        ////    }

        ////    size = DataOffset - Offset + 1; // + 1 to include FF end-marker




        ////}

        ////private void LoadObjects() {
        ////    List<StructInstance> objects = new List<StructInstance>(8);
        ////    int DataOffset = Offset + 1;

        ////    // While...
        ////    //    We haven't hit the end-of-objects marker...
        ////    //    and there is enough data left to represent an object {
        ////    while (
        ////        (DataOffset + 2 < data.Length && data[DataOffset] != RomValues.EndOfObjects && data[DataOffset] != RomValues.EndOfScreenData)) {

        ////        // Load the object
        ////        objects.Add(new StructInstance(data, DataOffset));

        ////        // Seek to next object
        ////        DataOffset += 3;
        ////    }

        ////    _Objects = objects.ToArray();
        ////}



        ////DoorInstance[] _Doors = null;
        /////// <summary>
        /////// Gets an array of DoorInstance objects that represents the doors of this room.
        /////// </summary>
        ////public DoorInstance[] Doors {
        ////    get {
        ////        // Create and cache the door references if it hasnt been done already
        ////        if (_Doors == null) {
        ////            LoadEnemyData();
        ////        } // if

        ////        return _Doors;
        ////    } // get
        ////}

        /// <summary>
        /// Gets the index of a screen gameItem.
        /// </summary>
        public int GetIndex(ObjectInstance i) {
            if (i == null) return -1;

            if (i is EnemyInstance)
                return _enemies.IndexOf((EnemyInstance)i);
            else if (i is StructInstance)
                return _structs.IndexOf((StructInstance)i);
            else if (i is DoorInstance)
                return _doors.IndexOf((DoorInstance)i);

            return -1;
        }

        /// <summary>
        /// Gets whether this screen has an access bridge (a bridge appears if
        /// both bosses have been beaten).
        /// </summary>
        public bool HasBridge { get; set; }





        public void ApplyLevelPalette(System.Drawing.Bitmap image) {
            System.Drawing.Imaging.ColorPalette bitmapPalette = image.Palette; // Get brush paletteIndex

            NesPalette bgPalette = Level.BgPalette; // Get ROM palettes
            NesPalette spritePalette = Level.SpritePalette;
            bgPalette.ApplyTable(bitmapPalette.Entries); // Apply to brush paletteIndex
            bgPalette.ApplyTable(bitmapPalette.Entries, 16);
            spritePalette.ApplyTable(bitmapPalette.Entries, 32);
            spritePalette.ApplyTable(bitmapPalette.Entries, 48);

            image.Palette = bitmapPalette; // Apply back to brush paletteIndex
        }

        public DoorInstance LeftDoor {
            get {
                for (int i = 0; i < _doors.Count; i++) {
                    var d = _doors[i];
                    if (d.Side == DoorSide.Left)
                        return d;
                }

                return null;
            }
        }
        public DoorInstance RightDoor {
            get {
                for (int i = 0; i < _doors.Count; i++) {
                    var d = _doors[i];
                    if (d.Side == DoorSide.Right)
                        return d;
                }
                return null;
            }
        }

        public int Size {
            get {
                // Size is 
                //     2 bytes: default pal, end-of-room token
                //     3 bytes per struct/enemy
                //     2 bytes per door
                //     1 byte for bridge
                //     1 byte if the room contains anything other than structs (end-of-struct token)

                int result = 2 + 3 * _structs.Count;
                int structOnlySize = result;

                result += 3 * _enemies.Count;
                result += 2 * _doors.Count;
                if (HasBridge) result += 1;

                if (result != structOnlySize) {
                    // The room contains something other than structs and needs an end-of-structs token
                    result += 1;
                }

                bool emptyRoom = (result == 2) && ((IProjectBuildScreen)this).ExtraBytesLength  == 0;
                if (emptyRoom) {
                    return 2; // Changed from 0 to 2 (oops... need default pal and end of room token!)
                }

                result += ((IProjectBuildScreen)this).ExtraBytesLength;

                return result;
            }
        }




        public int GetBytesNeededFor(ScreenObjectType objType) {
            // If there is not an enemy data marker, one may need to be created: 1 byte
            int bytesForFD = HasEnemyDataSegment ? 0 : 1;
            // If room is empty, we have to factor in mem needed for DefaultColorAttribute and FF terminator
            int bytesForHeaderFooter = (Size > 0) ? 0 : 2;

            switch (objType) {
                case ScreenObjectType.Struct:
                    // 3 bytes for obj
                    return bytesForHeaderFooter + 3;
                case ScreenObjectType.Enemy:
                    // 3 bytes for enemy data
                    return bytesForHeaderFooter + bytesForFD + 3;
                case ScreenObjectType.Door:
                    // 2 bytes for door data
                    return bytesForHeaderFooter + bytesForFD + 2;
                case ScreenObjectType.Bridge:
                    // 1 byte for bridge data
                    return bytesForHeaderFooter + bytesForFD + 1; 
                default:
                    throw new ArgumentException("Invalid object type.");
            }
        }

        #region IProjectBuildScreen Members

        IList<byte> IProjectBuildScreen.ExtraBytes { get; set; }

        int IProjectBuildScreen.ExtraBytesStart { get; set; }

        int IProjectBuildScreen.ExtraBytesLength { get; set; }

        #endregion
    } // Class

    /// <summary>
    /// Provides an interface for behavior specific to a project's build process for a Screen object.
    /// </summary>
    internal interface IProjectBuildScreen
    {
        /// <summary>
        /// Specifies an array containing extra bytes to append to the screen data.
        /// </summary>
        IList<byte> ExtraBytes { get; set; }
        /// <summary>
        /// Specifies the starting offset for extra bytes (see ExtraBytes)
        /// </summary>
        int ExtraBytesStart { get; set; }
        /// <summary>
        /// Specifies the number of extra bytes to append to screen data (see ExtraBytes)
        /// </summary>
        int ExtraBytesLength { get; set; }
    }
    public enum ScreenObjectType
    {
        Struct,
        Enemy,
        Door,
        Bridge
    }

} // Namespace

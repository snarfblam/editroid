//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.ComponentModel;

//namespace Editroid
//{
//    /// <summary>
//    /// Provides access to screen-specific data
//    /// </summary>
//    public class Screen
//    {
//        byte[] data;
//        MetroidRom rom;
//        Level level;
//        int size;
//        public int Index { get; private set; }
//        //MetroidHandle handle;

//        public StructInstance BringToFront(int objectIndex) {
//            int FrontObjectData = Objects[objectIndex].CopyData();
//            int newIndex = _Objects.Length;

//            for (int i = objectIndex; i < _Objects.Length - 1; i++) {
//                _Objects[i + 1].CopyData(_Objects[i]);
//            }

//            StructInstance result = _Objects[newIndex];
//            result.SetData(FrontObjectData);
//            return result;
//        }

//        public StructInstance SendToBack(int objectIndex) {
//            int BackObjectData = Objects[objectIndex].CopyData();

//            for (int i = objectIndex; i > 0; i--) {
//                _Objects[i - 1].CopyData(_Objects[i]);
//            }


//            StructInstance result = _Objects[0];
//            result.SetData(BackObjectData);
//            return result;
//        }
//        public StructInstance ReorderObject(int oldIndex, int newIndex) {
//            int increment = (oldIndex < newIndex) ? 1 : -1;
//            int swapValue = Objects[oldIndex].CopyData();

//            for (int i = oldIndex; i != newIndex; i += increment) {
//                _Objects[i + increment].CopyData(_Objects[i]);
//            }

//            _Objects[newIndex].SetData(swapValue);
//            return _Objects[newIndex];
//        }
//        public EnemyInstance ReorderEnemy(int oldIndex, int newIndex) {
//            if (oldIndex == newIndex) return _Enemies[oldIndex];

//            int increment = (oldIndex < newIndex) ? 1 : -1;
//            int swapValue = Enemies[oldIndex].CopyData();

//            for (int i = oldIndex; i != newIndex ; i += increment) {
//                _Enemies[i + increment].CopyData(_Enemies[i]);
//            }

//            _Enemies[newIndex].SetData(swapValue);
//            return _Enemies[newIndex];
//        }
//        public StructInstance ReorderObject(StructInstance o, int newIndex) {
//            return ReorderObject(GetIndex(o), newIndex);
//        }
//        public EnemyInstance ReorderEnemy(EnemyInstance e, int newIndex) {
//            return ReorderEnemy(GetIndex(e), newIndex);
//        }



//        /// <summary>
//        /// Gets the offset of enemy data (including door data). If there is
//        /// no enemy or door data, the return value will be the offset of the
//        /// first byte after this screen's data.
//        /// </summary>
//        public int EnemyDataOffset {
//        get{
//            return Offset + // Start
//                1 + // Default paletteIndex specifier
//                Objects.Length * 3 +// Enemies
//                1; // End of enemy data (FD) or end of data (FF) marker
//            }
//        }

//        /// <summary>
//        /// Gets a boolean value indicating whether the screen has a data
//        /// segment for enemy data (identified by the hexadecimal value FD).
//        /// </summary>
//        public bool HasEnemyDataSegment {
//            get {
//                return data[EnemyDataOffset - 1] == 0xFD;
//            }
//        }

//        /////// <summary>
//        /////// Creates a Screen object.
//        /////// </summary>
//        /////// <param name="handle">T metroid handle pointing to the data.</param>
//        /////// <param name="rom">ROM data</param>
//        /////// <param name="currentLevelIndex">The currentLevelIndex this screen belongs to.</param>
//        ////public Screen(Rom rom, LevelIndex level, MetroidHandle handle) {
//        ////    data = rom.data;
//        ////    this.handle = handle;
//        ////    this.rom = rom;
//        ////    this.level = level;
//        ////}
//        /// <summary>
//        /// Creates a Screen object.
//        /// </summary>
//        /// <param name="handle">T metroid handle pointing to the data.</param>
//        /// <param name="rom">ROM data</param>
//        /// <param name="currentLevelIndex">The currentLevelIndex this screen belongs to.</param>
//        public Screen(MetroidRom rom, Level level, int index) {
//            data = rom.data;
//            //this.handle = handle;
//            this.Index = index;
//            this.rom = rom;
//            this.level = level;
//        }
//        /// <summary>
//        /// Gets the offset of this object in ROM data.
//        /// </summary>
//        public int Offset { 
//            //get { return ScreenHandle.PointerTarget; } 
//            get {
//                var pScreen = level.Screens.Pointers[Index];
//                return level.Screens.DataBank.ToOffset(pScreen);
//            }
//        }

//        /// <summary>
//        /// Gets the ColorAttributeTable entry for this screen
//        /// </summary>
//        public byte ColorAttributeTable {
//            get {
//                return data[Offset];
//            }
//            set {
//                data[Offset] = value;
//            }
//        }

//        public StructInstance AddObject() {
//            int newObjectOffset = Offset + 1 + Objects.Length * 3;

//            level.ExpandScreenData(this,
//                newObjectOffset,
//                3);
//            _Objects = null;

//            StructInstance newObject = new StructInstance(data, newObjectOffset);
//            newObject.PalData = 0;
//            newObject.X = 0;
//            newObject.Y = 0;
//            newObject.ObjectType = 0;
            
//            ReloadData();
//            return newObject;
//        }

//        /// <summary>Expands screen data and adds an enemy to the screen.</summary>
//        /// <returns>T EnemyInstance object that represents the newly added enemy.</returns>
//        public EnemyInstance AddEnemy() {
//            // Get offset of new data
//            int newEnemyOffset = EnemyDataOffset;
//            // We prefer to add enemy data AFTER door data
//            newEnemyOffset += Doors.Length * 2;

//            int dataSize = 3;
//            int newSpriteSlot = FindAvailableSpriteSlot(); // new SpriteSlot value.

//            ExpandEnemyData(newEnemyOffset, dataSize);
//            _Enemies = null;

//            EnemyInstance newEnemy = new EnemyInstance(data, newEnemyOffset);
//            newEnemy.Respawn = false;
//            newEnemy.SpritePal = 0;
//            newEnemy.SpriteSlot = newSpriteSlot;
//            newEnemy.EnemyType = level.GetDefaultEnemy();
//            newEnemy.X = 3;
//            newEnemy.Y = 3;

//            ReloadData();

//            return newEnemy;
//        }

//        /// <summary>Removes the specified door.</summary>
//        public void DeleteDoor(DoorSide side) {
//            // Seek through doors and delete any that are left-side.
//            foreach (DoorInstance d in Doors) {
//                if (d.Side == side) {
//                    if (Doors.Length == 1 && Enemies.Length == 0 && !hasBridge)
//                        // Remove enemy data segment
//                        level.CropScreenData(this, d.Offset - 1, 3); 
//                    else
//                        level.CropScreenData(this, d.Offset, 2);
//                    return;
//                }
//            }

//            ReloadData();

//        }

//        /// <summary>Specifies the door type for the right door.</summary>
//        /// <param name="type"></param>
//        /// <returns>The data object that was modified.</returns>
//        public DoorInstance ChangeExistingDoor(DoorSide side, DoorType type) {
//            foreach (DoorInstance d in Doors) {
//                if (d.Side == side) {
//                    DoorInstance door = d; // Workaround for compiler lameness
//                    door.Type = type;
//                    return door;
//                }
//            }

//            return DoorInstance.Nothing;
//        }

//        /// <summary>Gets the count of the first available sprite slot</summary>
//        private int FindAvailableSpriteSlot() {
//            int slot = -1;
//            bool found = false;

//            // Until we find a free slot
//            while (!found) {
//                slot++; // Look at the next slot
//                found = true; // We succeed...

//                foreach (EnemyInstance e in Enemies) {
//                    if (e.SpriteSlot == slot) // ...unless the slot is in use
//                        found = false;
//                }
//            }

//            return slot;
//        }

//        /// <summary>
//        /// Expands currentLevelIndex room data and adds a door to CurrentScreen
//        /// </summary>
//        public DoorInstance AddDoor(DoorType type, DoorSide side) {
//            int doorOffset = EnemyDataOffset; // Offset to insert data at
//            int dataSize = 2;

//            // Expand data (Door data is stored with enemy data)
//            ExpandEnemyData(doorOffset, dataSize);

//            // Add door data marker (0x02)
//            data[doorOffset] = 0x02;

//            // DoorInstance object is only used to initialize data
//            DoorInstance newDoor = new DoorInstance(data, doorOffset);
//            newDoor.Side = side;
//            newDoor.Type = type;

//            ReloadData();
//            return newDoor;

//        }

//        public void DeleteObject(StructInstance o) {
//            Level.CropScreenData(this, o.Offset, 3);

//            ReloadData();
//        }
//        public void DeleteEnemy(EnemyInstance e) {
//            if (Enemies.Length == 1 && Doors.Length == 0 && !hasBridge)
//                Level.CropScreenData(this, e.Offset - 1, 4); // Remove enemy data segment marker
//            else
//                Level.CropScreenData(this, e.Offset, 3);

//            ReloadData();
            
//        }

//        public void AddBridge() {
//            if (hasBridge) return;

//            int insertOffset = EnemyDataOffset+ Doors.Length * 2 + Enemies.Length * 3;
//            // We need to add an enewy data segment, if necessary
//            if (HasEnemyDataSegment) {
//                Level.ExpandScreenData(this, insertOffset, 1); // 1 Byte for bridge data
//            } else {
//                // in this case insertOffset points to the first by after where the enemy data segment will be
//                Level.ExpandScreenData(this, insertOffset - 1, 2); // 1 byte for enemy data segment, 1 byte for bridge data
//                data[insertOffset - 1] = (byte)RomValues.EndOfObjects;
//            }

//            data[insertOffset] = (byte)RomValues.EnemyIdentifiers.Bridge;
//            ////_Enemies = null;
//            ////_Doors = null;
//            ////hasBridge = true;

//            ReloadData();

//        }
//        public void RemoveBridge() {
//            if (!hasBridge) return;

//            if (Doors.Length == 0 && Enemies.Length == 0) {
//                Level.CropScreenData(this, bridgeOffset - 1, 2);
//            } else {
//                Level.CropScreenData(this, bridgeOffset, 1);
//            }

//            ////_Enemies = null;
//            ////_Doors = null;
//            ////hasBridge = false;

//            ReloadData();

//        }

//        ////Level LevelData { get { return rom.GetLevel(level); } }
//        Level Level { get { return level; } }




//        /// <summary>
//        /// Expands current screens data for enemies/doors. If necessary,
//        /// an enemy data section will be added.
//        /// </summary>
//        /// <param name="offset">The offset to expand at.</param>
//        /// <param name="bytes">The number of free bytes needed.</param>
//        /// <returns>The offset of the free space.</returns>
//        int ExpandEnemyData(int offset, int bytes) {
//            // Determine whether we need to add an enemy data segment marker (0xFD)
//            bool needsEnemySegmentMarker = !HasEnemyDataSegment;
//            if (needsEnemySegmentMarker) {
//                offset--; // If so, it needs to go BEFORE the 0xFF screen data terminator
//                bytes++; // And it will take up an extra byte.
//            }

//            Level.ExpandScreenData(this, offset, bytes);

//            // Mark enemy data section (if necessary)
//            if (needsEnemySegmentMarker) {
//                data[offset] = 0xFD;
//                offset++;
//            }

//            return offset;
//        }



//        /// <summary>
//        /// Updates the screen index. This affects which pointer is used to lookup screen data.
//        /// A call to ReloadData should follow a call to UpdateIndex.
//        /// </summary>
//        /// <param name="newIndex"></param>
//        public void UpdateIndex(int newIndex) {
//            this.Index = newIndex;
//        }

//        StructInstance[] _Objects = null;


//        /// <summary>
//        /// Gets an array of ScreenObjects that this screen is composed of.
//        /// </summary>
//        /// <remarks>This property is cached.</remarks>
//        public StructInstance[] Objects {
//            get {
//                // If objects for this screen haven't been extracted yet, extract them
//                if(_Objects == null) {

//                    List<StructInstance> objects = new List<StructInstance>(8);
//                    int DataOffset = Offset + 1;

//                    // While...
//                    //    We haven't hit the end-of-objects marker...
//                    //    and there is enough data left to represent an object {
//                    while(
//                        (DataOffset + 2 < data.Length && data[DataOffset] != RomValues.EndOfObjects && data[DataOffset] != RomValues.EndOfScreenData)) {

//                        // Load the object
//                        objects.Add(new StructInstance(data, DataOffset));

//                        // Seek to next object
//                        DataOffset += 3;
//                    }

//                    _Objects = objects.ToArray();

//                }
//                return _Objects;
//            }
//        }
//        /// <summary>
//        /// Discards the cached list of ScreenObjects. 
//        /// </summary>
//        /// <remarks>This should be done when the number
//        /// or memory location of the screen object data changes. This does not need 
//        /// to be done when a screen object is merely modified because the 
//        /// actual data is not cached, only a reference to the data is chached.
//        ///</remarks>
//        public void InvalidateScreenObjects() {
//            _Objects = null;
//        }

//        DoorInstance[] _Doors = null;
//        /// <summary>
//        /// Gets an array of DoorInstance objects that represents the doors of this room.
//        /// </summary>
//        public DoorInstance[] Doors {
//            get {
//                // Create and cache the door references if it hasnt been done already
//                if(_Doors == null) {
//                    LoadEnemyData();
//                } // if

//                return _Doors;
//            } // get
//        }

//        /// <summary>
//        /// Gets the count of a screen gameItem.
//        /// </summary>
//        /// <param name="i">The gameItem to find the count of.</param>
//        /// <returns>The count of the specified gameItem, or -1 if the gameItem is not found.</returns>
//        public int GetIndex(ObjectInstance i) {
//            if(i.IsNothing) return -1;
//            if (i.InstanceType == ObjectInstanceType.Enemy)
//                return GetIndex(i.Enemy);
//            else if (i.InstanceType == ObjectInstanceType.Struct)
//                return GetIndex(i.Struct);
//            else if (i.InstanceType == ObjectInstanceType.Door)
//                return GetIndex(i.Door);
//            return -1;
//        }

//        /// <summary>
//        /// Gets the count of a screen gameItem.
//        /// </summary>
//        /// <param name="otherRow">The gameItem to find the count of.</param>
//        /// <returns>The count of the specified gameItem, or -1 if the gameItem is not found.</returns>
//        public int GetIndex(EnemyInstance e) {
//            if (e.IsNothing) return -1;

//            for (int i = 0; i < Enemies.Length; i++) {
//                if (e.Equals(_Enemies[i]))
//                    return i; // Return count of match
//            }

//            return -1; // Not found
//        }        /// <summary>
//        /// Gets the count of a screen gameItem.
//        /// </summary>
//        /// <param name="otherRow">The gameItem to find the count of.</param>
//        /// <returns>The count of the specified gameItem, or -1 if the gameItem is not found.</returns>
//        public int GetIndex(DoorInstance e) {
//            if (e.IsNothing) return -1;

//            for (int i = 0; i < Doors.Length; i++) {
//                if (e.Equals(_Doors[i]))
//                    return i; // Return count of match
//            }

//            return -1; // Not found
//        }

//        /// <summary>
//        /// Gets the count of a screen gameItem.
//        /// </summary>
//        /// <param name="otherRow">The gameItem to find the count of.</param>
//        /// <returns>The count of the specified gameItem, or -1 if the gameItem is not found.</returns>
//        public int GetIndex(StructInstance e) {
//            if (e.IsNothing) return -1;

//            for (int i = 0; i < Objects.Length; i++) {
//                if (e.Equals(_Objects[i]))
//                    return i; // Return count of match
//            }

//            return -1; // Not found
//        }

//        /// <summary>
//        /// Loads all objects in the enemy data section. This includes doors, enemies, Tourian access bridge, and possible other unknown data.
//        /// </summary>
//        private void LoadEnemyData() {
//            hasBridge = false;

//            List<DoorInstance> doors = new List<DoorInstance>(2);
//            List<EnemyInstance> enemies = new List<EnemyInstance>(5);

//            // Calculate offset (default pal (1 b) + start of data + objects (3 b ea.))
//            int DataOffset = Offset + Objects.Length * 3 + 1;
//            // If there is no enemy data section, return empty arrays.
//            if (data[DataOffset] == RomValues.EndOfScreenData) {
//                _Doors = new DoorInstance[] { };
//                _Enemies = new EnemyInstance[] { };
//            } else {

//                DataOffset++;

//                while (DataOffset + 1 < data.Length && data[DataOffset] != RomValues.EndOfScreenData) {
//                    Byte dataMarker = data[DataOffset];
//                    if (dataMarker == (byte)RomValues.EnemyIdentifiers.Door) {
//                        doors.Add(new DoorInstance(data, DataOffset));
//                        DataOffset += 2;
//                    } else if ((dataMarker & 0x0F) == (byte)RomValues.EnemyIdentifiers.Enemy || (dataMarker & 0x0F) == (byte)RomValues.EnemyIdentifiers.RespawnEnemy) {
//                        enemies.Add(new EnemyInstance(data, DataOffset));
//                        DataOffset += 3;
//                    } else if (dataMarker == (byte)RomValues.EnemyIdentifiers.Bridge) {
//                        hasBridge = true;
//                        bridgeOffset = DataOffset;
//                        DataOffset++;
//                    } else {
//                        break; // Invalid data, stop parsing enemy data
//                    }
//                }
//                _Doors = doors.ToArray();
//                _Enemies = enemies.ToArray();
//            }

//            size = DataOffset - Offset + 1; // + 1 to include FF end-marker




//        }
//        private bool hasBridge;
//        int bridgeOffset;

//        /// <summary>
//        /// Gets whether this screen has an access bridge (a bridge appears if
//        /// both bosses have been beaten).
//        /// </summary>
//        public bool HasBridge {
//            get {
//                // Load screen data if it is not loaded
//                if(_Doors == null)
//                    LoadEnemyData();

//                return hasBridge; 
//            }
//        }



//        EnemyInstance[] _Enemies = null;
//        /// <summary>
//        /// Gets an array of EnemyInstance objects that represents the enemies in this room.
//        /// </summary>
//        public EnemyInstance[] Enemies {
//            get {
//                if(_Enemies == null) {
//                    LoadEnemyData();
//                }

//                return _Enemies;
//            }
//        }


//        public void ReloadData() {
//            _Doors = null;
//            _Enemies = null;
//            _Objects = null;

//            LoadEnemyData();

//        }

//        public void ApplyLevelPalette(System.Drawing.Bitmap image) {
//            Level levelData = Level;

//            System.Drawing.Imaging.ColorPalette bitmapPalette = image.Palette; // Get brush paletteIndex

//            NesPalette bgPalette = levelData.BgPalette; // Get ROM palettes
//            NesPalette spritePalette = levelData.SpritePalette;
//            bgPalette.ApplyTable(bitmapPalette.Entries); // Apply to brush paletteIndex
//            bgPalette.ApplyTable(bitmapPalette.Entries, 16);
//            spritePalette.ApplyTable(bitmapPalette.Entries, 32);
//            spritePalette.ApplyTable(bitmapPalette.Entries, 48);

//            image.Palette = bitmapPalette; // Apply back to brush paletteIndex
//        }

//        public DoorInstance LeftDoor {
//            get {
//                foreach (DoorInstance d in Doors) {
//                    if (d.Side == DoorSide.Left)
//                        return d;
//                }

//                return DoorInstance.Nothing;
//            }
//        }
//        public DoorInstance RightDoor {
//            get {
//                foreach (DoorInstance d in Doors) {
//                    if (d.Side == DoorSide.Right)
//                        return d;
//                }

//                return DoorInstance.Nothing;
//            }
//        }

//        public int Size { get { return size; } }

//    } // Class
//} // Namespace

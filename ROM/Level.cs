using Editroid.ROM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Editroid.ROM;
using System.IO;
using Editroid.ROM.Formats;
using System.Diagnostics;

namespace Editroid
{
    /// <summary>
    /// This class encapsulates currentLevelIndex specific data.
    /// </summary>
    /// <remarks>This class stores some data locally. Certain changes will not necessarily be reflected in the ROM.</remarks>
    public class Level : IRomDataParentObject, ILevelDataSerializer
    {
        /// <summary>
        /// Loads all currentLevelIndex-specific data for a specific currentLevelIndex in a given ROM image
        /// </summary>
        /// <param name="rom">The Rom object to load data from</param>
        /// <param name="currentLevelIndex">The currentLevelIndex to load</param>
        public Level(MetroidRom rom, LevelIndex level) {
            this.rom = rom;
            data = rom.data;
            this.index = level;

            Format = rom.Format.CreateLevelFormat(this);
            this.Bank = rom.Format.Banks[Format.LevelBankIndex];

            Bytes = new ByteIndexer(this);
            PCpu = new PointerIndexer(this);
            PRom = new PRomIndexer(this);

            pointers_deprecated = new LevelPointers(rom, this);

            structures = new StructureCollection(this);
            screens = new ScreenCollection(this);

            patternGroups = new PatternGroupIndexTable(this);
            LoadPatterns();
            LoadPalette();

            combos = new ComboTable(rom, Format.ComboDataOffset);
            sprites = Graphic.LevelSprites.GetSprites(level);
            itemTable_DEPRECATED = new ItemLoader(this, rom);
            //structures = new StructureCollection(this);
            altMusic = new AlternateMusicRooms(rom, this);

            this.Items = new ItemCollection(this);
            Items.LoadItems();

            TilePhysicsTableLocation = Format.GetTilePhysicsTableLocation();
        }

        byte[] data;
        LevelPointers pointers_deprecated;
        LevelIndex index;
        ComboTable combos;
        PatternTable patterns;
        PatternTable spritePatterns;
        PatternGroupIndexTable patternGroups;
        ScreenCollection screens;
        Graphic.SpriteDefinition[] sprites;
        AlternateMusicRooms altMusic;
        int[] enemyPalettes;
        MetroidRom rom;
        public LevelFormat Format { get; private set; }

        public ByteIndexer Bytes { get; private set; }
        public PointerIndexer PCpu { get; private set; }
        public PRomIndexer PRom { get; private set; }

        public pRom TilePhysicsTableLocation { get; private set; }

        public ItemCollection Items { get; private set; }

        ////public Rom.LevelBanks Bank { get; private set; }
        public Bank Bank { get; private set; }
        public int BankOffset { get { return Bank.Offset; } }

        public PointerTable PalettePointers { get; private set; }

        /// <summary>
        /// Gets/sets the index of the CHR bank to be used when the level's CHR is loaded. Changing this value
        /// will not cause the loaded CHR to be updated. Call ReloadPatterns to update the CHR.
        /// </summary>
        public int? PreferredChrBank { get; set; }

        public PatternGroupIndexTable PatternGroups { get { return patternGroups; } }

        /// <summary>
        /// Gets screens for this level.
        /// </summary>
        public ScreenCollection Screens { get { return screens; } }

        /// <summary>
        /// Gets pointers for this level.
        /// </summary>
        public LevelPointers Pointers_depracated { get { return pointers_deprecated; } }

        /// <summary>Gets a LevelIndex enumeration value for this currentLevelIndex.</summary>
        public LevelIndex Index {
            get { return index; }
        }


        NesPalette _BgPalette;
        /// <summary>Gets a NesPalette object representing the background 
        /// paletteIndex for this currentLevelIndex.</summary>
        public NesPalette BgPalette { get { return _BgPalette; } }
        NesPalette _BgAltPalette;
        /// <summary>Gets a NesPalette object representing the alternate background 
        /// paletteIndex for this currentLevelIndex.</summary>
        public NesPalette BgAltPalette { get { return _BgAltPalette; } }
        NesPalette _SpritePalette;
        /// <summary>Gets a NesPalette object representing the sprite
        /// paletteIndex for this currentLevelIndex.</summary>
        public NesPalette SpritePalette { get { return _SpritePalette; } }
        NesPalette _SpriteAltPalette;
        public NesPalette SpriteAltPalette { get { return _SpriteAltPalette; } }

        /// <summary>
        /// Gets the combos for this currentLevelIndex.
        /// </summary>
        public ComboTable Combos { get { return combos; } }

        /// <summary>
        /// Returns a palette macro, or an empty PpuMacro object if the palette data pointer is not valid.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PpuMacro GetPaletteMacro(int index) {
            if (index < 0) throw new ArgumentException("Index out of range");

            var pointer = PalettePointers[index];
            if (!pointer.IsLevelBank) return new PpuMacro();

            var offset = ToPRom(pointer);
            return new PpuMacro(rom, offset);
        }

        public AlternateMusicRooms AlternateMusicRooms { get { return altMusic; } }

        pCpu LevelStartPointer = new pCpu(0xD7, 0x95);
        /// <summary>
        /// Gets the offset of the currentLevelIndex start screen.
        /// </summary>
        public int LevelStartOffset { get { return LevelStartPointer.GetDataOffset(index); } }

        /// <summary>
        /// Gets/sets the location where the player will start in this currentLevelIndex if he dies/continues.
        /// </summary>
        public int StartScreenX {
            get { return data[LevelStartOffset]; }
            set { data[LevelStartOffset] = (byte)value; }
        }
        /// <summary>
        /// Gets/sets the location where the player will start in this currentLevelIndex if he dies/continues.
        /// </summary>
        public int StartScreenY {
            get { return data[LevelStartOffset + 1]; }
            set { data[LevelStartOffset + 1] = (byte)value; }
        }

        public LevelDataStore GetContainingStore(pCpu offset) {
            if ((int)offset < 0x9560) return LevelDataStore.ExpandoStore;
            return LevelDataStore.StandardStore;
        }



        /// <summary>
        /// The offset of a level's palette pointer table.
        /// </summary>
        public static readonly pCpu PalettePointers_Offset = new pCpu(0x9560);
        /// <summary>
        /// The number of palette PPU macros identified in a level's palette pointer table.
        /// </summary>
        public const int PaletteCount = 0x1C;

        /// <summary>
        /// Takes a pointer and resolves it to the actual RomOffset the pointer references.
        /// Workes for both level-data banks and fixed-PRG banks, with both normal and expando roms.
        /// </summary>
        public pRom ToPRom(pCpu p) {
            if (p.Value < 0xC000 & p.Value >= 0x8000) {
                return (pRom)(p.BankedOffset + Bank.Offset);
            } else if (p.Value >= 0x8000) {
                return (pRom)(p.BankedOffset + rom.FixedBank.Offset);
            } else {
                System.Diagnostics.Debug.Fail("Level.ResolvePointer: attempted to dereference a pointer that referenced RAM or another address that does not map to a ROM location");
                return (pRom)(p.BankedOffset + MetroidRom.HeaderSize); // Silent failure == success?
            }
        }

        /// <summary>
        /// Gets the offset of the pointer referenced by the specified pointer.
        /// </summary>
        /// <param name="p">The address of the pointer to resolve.</param>
        /// <returns></returns>
        public pRom DerefHandle(pCpu p) {
            int pointerOffset = ToPRom(p);
            pCpu referencedPointer = new pCpu(data, pointerOffset);
            return ToPRom(referencedPointer);
        }
        /// <summary>
        /// Returns the offset of data that is identified by the pointer that is found at
        /// the offset 'pOffset' specifies.
        /// </summary>
        /// <param name="pOffset">The address of the pointer to resolve.</param>
        /// <returns></returns>
        public pRom DerefHandle(pRom pOffset) {
            pCpu referencedPointer = new pCpu(data, (int)pOffset);
            return ToPRom(referencedPointer);
        }
        /// <summary>
        /// Gets the offset of the pointer referenced by the specified pointer.
        /// </summary>
        /// <param name="p">The address of the pointer to resolve.</param>
        /// <returns></returns>
        public pCpu DerefHandleCpu(pCpu p) {
            int pointerOffset = ToPRom(p);
            return new pCpu(data, pointerOffset);
        }
        /// <summary>
        /// Returns the offset of data that is identified by the pointer that is found at
        /// the offset 'pOffset' specifies.
        /// </summary>
        /// <param name="pOffset">The address of the pointer to resolve.</param>
        /// <returns></returns>
        public pCpu DerefHandleCpu(pRom pOffset) {
            return new pCpu(data, (int)pOffset);
        }

        public pCpu CreatePointer(pRom offset) {
            if (offset >= rom.FixedBank.Offset) { // PRG ROM (mapped to 0xC000-0xFFFF)
                int relativeOffset = offset - rom.FixedBank.Offset;
                if (offset > 0x3FFF) throw new ArgumentException("Offset was beyond end of last PRG bank.");
                return new pCpu((ushort)(0xC000 + offset));
            } else { // Level data ROM (mapped to 0x8000-0xBFFF)
                int relativeOffset = offset - Bank.Offset;

                if (relativeOffset < 0 || relativeOffset >= 0x4000)
                    throw new ArgumentException("Offset does not refer to fixed PRG bank or this level's data bank.");

                return new pCpu((ushort)(relativeOffset | 0x8000));
            }
        }

        /// <summary>
        /// Gets which section of data the item data is stored in.
        /// </summary>
        public LevelDataStore ItemDataStore {
            get {
                var pItemPointer = Format.pPointerToItemList;
                var pItemData = DerefHandleCpu(pItemPointer);
                return GetContainingStore(pItemData);
            }
        }

        /// <summary>Identifies the end of the region of available memory freed up in an expando rom.</summary>
        public static readonly pCpu EndOfExpandoStore = new pCpu(0x9560);
        /// <summary>Identifies the end of the region of memory available in the standard level-data section.</summary>
        public static readonly pCpu EndOfStandardStore = new pCpu(0xB000);

        public int FreeStructureMemory {
            get {
                return Format.FreeStructureMemory;

            }
        }




        /// <summary>
        /// Gets the offset of structure data.
        /// </summary>
        public int StructDataOffset {
            get {
                return Structures[0].Offset;
            }
        }



        /// <summary>
        /// Gets the remaining free space for screen data for this level.
        /// </summary>
        public int FreeScreenData {
            get {
                return Format.FreeScreenMemory;
            }
        }

        /// <summary>
        /// Gets the gameItem table for this currentLevelIndex.
        /// </summary>
        public ItemLoader ItemTable_DEPRECATED { get { return itemTable_DEPRECATED; } } // Todo: replace with GetItemTable method, which should be called only by ItemCollection.LoadItems
        private ItemLoader itemTable_DEPRECATED;

        public StructureCollection Structures { get { return structures; } }
        private StructureCollection structures;


        ////public bool CheckForItemsAt(Point mapLocation, out ItemSeeker result) {
        ////    result = new ItemSeeker();

        ////    foreach (ItemRowEntry row in itemTable) {
        ////        if (row.MapY == mapLocation.Y) {
        ////            ItemSeeker items = row.Seek();

        ////            bool hasMoreScreens = true;
        ////            while (hasMoreScreens) {
        ////                if (items.MapX == mapLocation.X) {
        ////                    result = items;
        ////                    return true;
        ////                }
        ////                hasMoreScreens = items.MoreScreensPresent;
        ////                if (hasMoreScreens)
        ////                    items.NextScreen();
        ////            }
        ////        }
        ////    }

        ////    return false;
        ////}
        public ItemScreenData GetItemsAt(Point mapLocation) {
            for (int i = 0; i < Items.Count; i++) {
                if (Items[i].MapX == mapLocation.X && Items[i].MapY == mapLocation.Y)
                    return Items[i];
            }
            return null;
        }

        public int GetDefaultEnemyType() {
            switch (index) {
                case LevelIndex.Brinstar:
                    return 2;
                case LevelIndex.Norfair:
                case LevelIndex.Tourian:
                case LevelIndex.Kraid:
                case LevelIndex.Ridley:
                default:
                    return 0;
            }
        }


        /// <summary>
        /// Gets the number of enemies in this currentLevelIndex.
        /// </summary>
        public int SpriteCount { get { return sprites.Length; } }
        /// <summary>
        /// Gets a sprite by count.
        /// </summary>
        /// <param name="count">Index of the sprite to get.</param>
        /// <returns>T Graphic.SpriteDefinition object.</returns>
        public Graphic.SpriteDefinition GetSprite(int index) {
            if (index >= sprites.Length) return sprites[0];
            return sprites[index];
        }

        public int GetSpritePalette(int index, bool altPal) {
            if (index >= enemyPalettes.Length) return enemyPalettes[0];
            if (altPal) return enemyPalettes[index * 2 + 1];
            return enemyPalettes[index * 2];

        }

        private void LoadPalette() {
            PalettePointers = new PointerTable(
                rom,
                (pRom)(NesPalette.LevelPalettePointerTableOffset + Bank.Offset),
                0x1c);

            _SpritePalette = NesPalette.FromLevel_SPR(this);
            _BgPalette = NesPalette.FromLevel_BG(this);
            if (Format.SupportsAltBgPalette) {
                _BgAltPalette = NesPalette.FromLevel_BG_Alt(this);
            } else {
                _BgAltPalette = _BgPalette;
            }
            if (Format.SupportsAltSpritePalette) {
                _SpriteAltPalette = NesPalette.FromLevel_SPR_Alt(this);
            } else {
                _SpriteAltPalette = _SpritePalette;
            }


            switch (this.index) {
                case LevelIndex.Brinstar:
                    enemyPalettes = new int[] { 0, 0, 0, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case LevelIndex.Norfair:
                    enemyPalettes = new int[] { 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 3, 2, 3, 2, 0 };
                    break;
                case LevelIndex.Tourian:
                    enemyPalettes = new int[] { 0, 3, 3, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0 };
                    break;
                case LevelIndex.Kraid:
                    enemyPalettes = new int[] { 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0 };
                    break;
                case LevelIndex.Ridley:
                    enemyPalettes = new int[] { 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 3, 2, 3, 2, 0, 2, 3, 2, 0, 2, 3, 2, 0 };
                    break;
            }
        }

        /// <summary>
        /// Gets the pattern table for this currentLevelIndex.
        /// </summary>
        public PatternTable Patterns { get { return patterns; } }
        /// <summary>
        /// Gets the sprite pattern table for this currentLevelIndex.
        /// </summary>
        public PatternTable SpritePatterns { get { return spritePatterns; } }

        /// <summary>
        /// Gets a linear pattern table that can be used for blitting to construct images using currentLevelIndex-specific graphics. The table is 1 graphic tile tall and 256 tiles wide.
        /// </summary>
        private void LoadPatterns() {
            patterns = CreatePatternBitmap(true);
            spritePatterns = CreateSpritePatternBitmap(true);
        }

        public PatternTable CreateSpritePatternBitmap(bool linear) {
            return Format.LoadSpritePatternsTable(linear);
        }

        public PatternTable CreatePatternBitmap(bool linear) {
            if (PreferredChrBank == null) {
                return Format.LoadBgPatternTable(linear);
            } else {
                return Format.LoadBgPatternTable_ChrUsage(linear, PreferredChrBank.Value);
            }
        }
        public void ReloadPatterns() {
            LoadPatterns();
        }





        private ScreenCollection _Screens { get { return screens; } }

        /// <summary>
        /// Gets a struct definition.
        /// </summary>
        /// <param name="count">Index of a struct.</param>
        /// <returns>T struct definition.</returns>
        public Structure GetStruct(int index) {
            return Structures[index];
        }

        /// <summary>
        /// Gets a combo in this currentLevelIndex by count.
        /// </summary>
        /// <param name="count">Index of the combo.</param>
        /// <returns>T Combo object.</returns>
        public Combo GetCombo(int index) {
            return combos[index];
        }


        /// <summary>
        /// Gets the number of structs in this level.
        /// </summary>
        public int StructCount {
            get { return Structures.Count; }
        }


        internal void ReloadItems() {
            itemTable_DEPRECATED = new ItemLoader(this, rom);
        }

        public MetroidRom Rom { get { return rom; } }


        #region IRomDataParentObject Members

        int IRomDataObject.Offset { get { return BankOffset; } }
        int IRomDataObject.Size { get { return 0; } }

        bool IRomDataObject.HasListItems { get { return true; } }
        bool IRomDataObject.HasSubItems { get { return true; } }

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            if (rom.ExpandedRom) {
                return new IRomDataObject[]{
                    Combos,
                    structures,
                    new PointerTableData(Format.StructPtableOffset, structures.Count,"Structure Pointer Table",rom),
                    Screens,
                    new PointerTableData(Format.RoomPtableOffset, screens.Count,"Screen Pointer Table",rom),
                    new PaletteTable(rom, index),
                    ItemTable_DEPRECATED,
                    new alternateMusicRoom_RomDataObject(this),
                };
            } else {
                return new IRomDataObject[]{
                    Combos,
                    structures,
                    new PointerTableData(Format.StructPtableOffset, structures.Count,"Structure Pointer Table",rom),
                    Screens,
                    new PointerTableData(Format.RoomPtableOffset, screens.Count,"Screen Pointer Table",rom),
                    new PaletteTable(rom, index),
                    patterns,
                    ItemTable_DEPRECATED,
                    new alternateMusicRoom_RomDataObject(this),
                };
            }
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            return new LineDisplayItem[]{
                new LineDisplayItem("Area Start", LevelStartOffset,2,data),
                };
        }

        string IRomDataObject.DisplayName { get { return index.ToString(); } }

        private class alternateMusicRoom_RomDataObject : IRomDataObject
        {
            public alternateMusicRoom_RomDataObject(Level l) {
                Offset = l.Format.AltMusicOffset;
                Size = l.Format.AltMusicRoomCount;
            }

            public int Offset { get; private set; }
            public int Size { get; private set; }
            public bool HasListItems { get { return false; } }
            public bool HasSubItems { get { return false; } }
            public string DisplayName { get { return "Alternate music rooms list"; } }

        }

        #endregion



        internal void InvalidatePatterns() {
            this.patterns.Dispose();
            LoadPatterns();
        }

        ////internal int AddRoom() {
        ////    return AddRoom(0);
        ////}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSize">The number of bytes to be available for enemy/object data. Excludes header/footer.</param>
        /// <returns></returns>
        internal int AddRoom() { ////int dataSize) {
            ////// Move rooms to create space for pointer
            ////ExpandScreenData_deprecated(-1, screens[0].Offset, 2);

            ////// Initialize new room
            ////var lastScreen = screens[screens.Count - 1];
            ////int newScreenOffset = lastScreen.Offset + lastScreen.Size;
            int newScreenIndex = screens.Count;

            ////data[newScreenOffset] = 0;
            ////data[newScreenOffset + 1 + dataSize] = 0xFF;

            ////// Set last room pointer to new room's offset
            ////pCpu newRoomPointer = CreatePointer((pRom)newScreenOffset);
            ////////pointers.RoomTable.ChangeCount(pointers.RoomTable.Count + 1);
            ////////pointers.RoomTable[newScreenIndex] = newRoomPointer;
            ////screens.Pointers[newScreenIndex] = newRoomPointer;
            ////int pointerOffset = pointers_deprecated.RoomTableOffset + newScreenIndex * 2;

            ////// Create new Room object
            ////System.Diagnostics.Debug.Fail("Badness!"); // Todo: the following should be performed by ScreenCollection (look for other instances of this sort of thing, e.g. with structs)
            ////{
            ////    var newScreen = new Screen(rom, screens, newScreenIndex, false);
            ////    screens.AddScreen(newScreen);
            ////    //newScreen.ReloadData();
            ////}
            ////return newScreenIndex;
            return screens.IndexOf(screens.AddScreen());
        }

        internal int CloneRoom(int screenIndex) {
            ////var original = screens[screenIndex];
            ////int newScreenIndex = AddRoom(original.Size - 2);

            ////int source = original.Offset;
            ////int dest = screens[newScreenIndex].Offset;

            ////for (int i = 0; i < original.Size; i++) {
            ////    rom.data[dest + i] = rom.data[source + i];
            ////}

            ////screens[newScreenIndex].ReloadData();
            ////return newScreenIndex;
            var oldRoom = screens[screenIndex];
            var newRoom = screens.AddScreen();
            newRoom.ColorAttributeTable = oldRoom.ColorAttributeTable;
            for (int i = 0; i < oldRoom.Structs.Count; i++) {
                newRoom.AddObject(oldRoom.Structs[i].Clone());
            }
            for (int i = 0; i < oldRoom.Enemies.Count; i++) {
                newRoom.AddEnemy(oldRoom.Enemies[i].Clone());
            }
            for (int i = 0; i < oldRoom.Doors.Count; i++) {
                //newRoom.Doors.Add(oldRoom.Doors[i].Clone());
                newRoom.AddDoor(oldRoom.Doors[i].Type,oldRoom.Doors[i].Side);
            }
            newRoom.HasBridge = oldRoom.HasBridge;

            return screens.IndexOf(newRoom);
        }

        internal void DeleteScreen(int deletedRoomIndex) {
            ////int roomDataOffset = screens[deletedRoomIndex].Offset;
            ////int roomDataSize = screens[deletedRoomIndex].Size;

            ////// Remove deleted room's data
            ////CropScreenData(deletedRoomIndex, roomDataOffset, roomDataSize);

            ////// Cut 2 bytes off pointer table
            ////CropScreenPointer(deletedRoomIndex);
            ////// Remove deleted room from collection

            screens.DeleteRoom(deletedRoomIndex);
            ////pointers.RoomTable.ChangeCount(pointers.RoomTable.Count - 1); // This seems reduntant!
        }
        
        /////// <summary>
        /////// Performs part of the function of the DeleteScreen method. Removes a pointer from the pointer table and moves all screen data back 2 bytes to overwrite
        /////// unused pointer at end of table.  Only valid for Expando ROMS. Assumses that the room data has already been deleted, that all pointers correctly point to the
        /////// CURRENT location of each screen's data, and that the deleted screen's pointer is still present in the table (it should have the same address as the following
        /////// pointer since the deleted screen has zero bytes of data).
        /////// </summary>
        /////// <param name="deletedRoomIndex"></param>
        ////private void CropScreenPointer(int deletedRoomIndex) {
        ////    const int pointerSize = 2;
        ////    if (deletedRoomIndex < 0 || deletedRoomIndex >= screens.Count)
        ////        throw new ArgumentException("Invalid screen index specified for CropScreenPointer. See code/XML comments for proper usage.");
        ////    if (rom.RomFormat == RomFormat.Standard) // Can only do with an expando rom
        ////        throw new InvalidOperationException("Attempted to delete screen pointer in a non-expanded ROM or non-relocated level.");

        ////    int screenDataOffset = screens[0].Offset;

        ////    // Subtract 2 from all screen pointers to account for removed pointer 
        ////    var pTable = screens.Pointers;
        ////    for (int i = 0; i < Screens.Count; i++) {
        ////        var newPtr = pTable[i] - pointerSize;
        ////        pTable[i] = newPtr;
        ////        ////_Screens[i].ScreenHandle.OffsetPointer_DEPRECATED(-pointerSize);
        ////    }

        ////    // Copy all pointers following deleted room back 2 bytes (effectively remove deleted room's pointer from list)
        ////    int removedPointerOffset = pointers_deprecated.RoomTableOffset + deletedRoomIndex * 2;
        ////    int endOfTableOffset = pointers_deprecated.RoomTableOffset + screens.Count * 2;

        ////    for (int i = removedPointerOffset; i < endOfTableOffset; i++) {
        ////        data[i] = data[i + 2];
        ////    }

        ////    // Remove two now-unused bytes from table (move room data back 2 bytes)
        ////    int newScreenDataOffset = screenDataOffset - pointerSize;
        ////    int endOfScreenData = EndOfAvailScreenData - pointerSize; // Offset of first byte AFTER data to be moved
        ////    // Copy screen data from DataOffset to END of currentLevelIndex screen data back 'DataSize' bytes.
        ////    for (int i = newScreenDataOffset; i < endOfScreenData; i++) {
        ////        data[i] = data[i + pointerSize];
        ////    }




        ////}

        internal int AddStructure() {
            var newIndex = structures.AddNew();
            
            // Update offset of first struct (others update themselves when ROM is written)
            for (int i = 0; i < structures.Count - 1; i++) {
                var s = structures[i];
                s.Offset += 2;

            }
            structures[newIndex].Offset = structures[newIndex - 1].Offset + structures[newIndex - 1].Size;

            return newIndex;
        }

        /// <summary>
        /// Deletes the specified structure. Any instances of this structure will
        /// be set to use structure #0. All other structures instances will have
        /// their index corrected.
        /// </summary>
        /// <param name="deletedIndex"></param>
        internal void DeleteStructure(int deletedIndex) {
            // Subtract 2 from all pointers before deleted struct
            for (int i = 0; i < deletedIndex; i++) {
                structures[i].Offset -= 2;
            }
            // Subtract 2 + deleted struct size from all pointers after deleted struct
            // Pointers also need to be moved back 1 entry
            int difference = 2 + structures[deletedIndex].Size;
            for (int i = deletedIndex + 1; i < structures.Count; i++) {
                structures[i - 1].Offset = structures[i].Offset - difference;
                
            }

            // Remove structure from collection
            structures.CropEntry(deletedIndex);

            // Update screen data (replace deleted structure with #0, adjust following struct indecies back 1)
            for (int i = 0; i < screens.Count; i++) {
                var screen = screens[i];
                for (int j = 0; j < screen.Structs.Count; j++) {
                    var obj = screen.Structs[j];
                    if (obj.ObjectType == deletedIndex)
                        obj.ObjectType = 0;
                    else if (obj.ObjectType > deletedIndex)
                        obj.ObjectType -= 1;
                }
            }
        }

        //public ChrAnimationLevelData ChrAnimation { get; set; }
        public ChrAnimationLevelData ChrAnimation { get { return rom.ChrAnimationData[index]; } set { rom.ChrAnimationData[index] = value; } }

        ////internal void TrasposeStructData(int distance) {
        ////    if (distance > 0 && FreeStructureMemory < distance)
        ////        throw new ArgumentException("Attempted to move structure data beyond the end of available memory.");

        ////    int oldDataStart = pointers_deprecated.StructTableOffset;
        ////    int oldDataEnd = Format.FindEndOfUsedStructMemory();
        ////    int dataLen = oldDataEnd - oldDataStart;
        ////    int newDataStart = oldDataStart + distance;

        ////    rom.SerializeAllData();
        ////    for (int i = 0; i < structures.Count; i++) {
        ////        structures[i].Offset += distance;
        ////    }


        ////    Array.Copy(data, oldDataStart, data, newDataStart, dataLen);
        ////    //int tableOffset = pointers.StructTableOffset;


        ////    //// Move and update pointers
        ////    //if (distance > 0) {
        ////    //    for (int i = structures.Count - 1; i >= 0; i--) {
        ////    //        int entryOffset = i * 2 + tableOffset;
        ////    //        int newEntryOffset = entryOffset + distance;

        ////    //        NesPointer entry = new NesPointer(data, entryOffset);
        ////    //        entry.Value += distance;
        ////    //        data[newEntryOffset] = entry.Byte1;
        ////    //        data[newEntryOffset + 1] = entry.Byte1;
        ////    //    }
        ////    //} else {
        ////    //    for (int i = 0; i < structures.Count; i++) {
        ////    //        int entryOffset = i * 2 + tableOffset;
        ////    //        int newEntryOffset = entryOffset + distance;

        ////    //        NesPointer entry = new NesPointer(data, entryOffset);
        ////    //        entry.Value += distance;
        ////    //        data[newEntryOffset] = entry.Byte1;
        ////    //        data[newEntryOffset + 1] = entry.Byte1;

        ////    //        structures[i].Offset += distance;
        ////    //    }
        ////    //}


        ////    var structTablePointer = ToPRom(Format.PointerToStructPTable);
        ////    pCpu pointer = new pCpu(data, structTablePointer);
        ////    pointer.Value += distance;
        ////    data[(int)structTablePointer] = pointer.Byte1;
        ////    data[1 + (int)structTablePointer] = pointer.Byte2;


        ////}

        #region Saving to ROM
        internal void SaveToRom() {
            Format.PerformSaveOperation(this);
            Items.SaveItemData();
        }

        void ILevelDataSerializer.SerializeStructTable(ref int offset) {
            for (int i = 0; i < structures.Count; i++) {
                var oStructData = structures[i].Offset;
                var pStructData = structures.DataBank.ToPtr((pRom)oStructData);

                rom.WritePointer((pRom)offset, pStructData);
                offset += 2;
            }
        }

        void ILevelDataSerializer.SerializeStructData(ref int offset) {
            MemoryStream romStream = new MemoryStream(data);

            romStream.Seek(offset, SeekOrigin.Begin);

            for (int i = 0; i < Structures.Count; i++) {
                Structure structToWrite = Structures[i];

                // Offset was previously calculated based on bytes written by previous structs.
                structToWrite.Offset = (pRom)offset;

                int bytesWritten;
                structToWrite.WriteData(romStream, out bytesWritten);
                offset += bytesWritten;
            }
        }

        void ILevelDataSerializer.SerializeRoomTable(ref int offset) {
            if (Format.Uses24bitScreenPointers) {
                for (int i = 0; i < screens.Count; i++) {
                    var oScreenData = screens[i].Offset;
                    // Get bank/address
                    int screenBank = Mmc3.GetBank((pRom)oScreenData);
                    var pScreenData = 0xA000 | (int)Mmc3.GetAddress8000((pRom)oScreenData);
                    Debug.WriteLine(screenBank.ToString("x2") + ":" + pScreenData.ToString("x4"));
                    if (screenBank == 0xe) { }
                    rom.data[offset] = (byte)screenBank;
                    offset++;
                    rom.WritePointer((pRom)offset, (pCpu)pScreenData);
                    offset += 2;
                }
            
                // FF FF FF terminator
                rom.data[offset] = 0;
                rom.data[offset + 1] = 0;
                rom.data[offset + 2] = 0;
                offset += 3;

            } else {
                for (int i = 0; i < screens.Count; i++) {
                    var oScreenData = screens[i].Offset;
                    var pScreenData = screens.DataBank.ToPtr((pRom)oScreenData);

                    rom.WritePointer((pRom)offset, pScreenData);
                    offset += 2;
                }
                // Enhanced ROMs require FFFF terminator for room pointers
                if (rom.RomFormat == RomFormats.Enhanco) {
                    rom.WritePointer((pRom)offset, new pCpu(0xFFFF));
                    offset += 2;
                }
            }
        }

        void ILevelDataSerializer.SerializeRoomData(ref int offset) {
            if (Format.Uses24bitScreenPointers) {
                for (int i = 0; i < screens.Count; i++) {
                    int bank, address;
                    rom.ScreenWritePointers.AllocateSpace(screens[i].Size, out bank, out address);
                    offset = (bank * 0x2000) + (address & 0x1FFF) + 0x10;

                    screens[i].SaveScreen(ref offset);
                }
            } else {
                for (int i = 0; i < screens.Count; i++) {
                    screens[i].SaveScreen(ref offset);
                }
            }
        }

        #endregion

        /// <summary>
        /// Called by MetroidRom.CorrectDoubleScreens to update 
        /// </summary>
        internal void CorrectScreenIndex(ref byte value) {
            while (screens.InvalidScreenIndecies.Contains((int)value)) {
                value++;
            }

            if (value >= screens.Count)
                value = 0;
        }
    }

    /// <summary>
    /// Defines a set of functions that serialize level data 
    /// and update a pointer to the first byte beyound serialized data.
    /// </summary>
    public interface ILevelDataSerializer
    {
        void SerializeStructTable(ref int offset);
        void SerializeStructData(ref int offset);
        void SerializeRoomTable(ref int offset);
        void SerializeRoomData(ref int offset);
    }

    public class PaletteTable : IRomDataParentObject
    {

        MetroidRom rom;
        LevelIndex level;

        public PaletteTable(MetroidRom rom, LevelIndex level) {
            this.rom = rom;
            this.level = level;
        }

        public bool HasSecondaryPalette { get { return level == LevelIndex.Brinstar || level == LevelIndex.Norfair; } }

        #region IRomDataParentObject Members

        int IRomDataObject.Offset { get { return 0; } }
        int IRomDataObject.Size { get { return 0; } }

        bool IRomDataObject.HasListItems { get { return true; } }
        bool IRomDataObject.HasSubItems { get { return false; } }

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return RomDataObjects.EmptyNode;
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            List<LineDisplayItem> items = new List<LineDisplayItem>();
            items.Add(new LineDisplayItem("Background 1", rom.GetLevel(level).BgPalette.Offset, 4, rom.data));
            items.Add(new LineDisplayItem("Background 2", rom.GetLevel(level).BgPalette.Offset + 4, 4, rom.data));
            items.Add(new LineDisplayItem("Background 3", rom.GetLevel(level).BgPalette.Offset + 8, 4, rom.data));
            items.Add(new LineDisplayItem("Background 4", rom.GetLevel(level).BgPalette.Offset + 12, 4, rom.data));

            if (HasSecondaryPalette) {
                items.Add(new LineDisplayItem("Background Alt 1", rom.GetLevel(level).BgAltPalette.Offset, 4, rom.data));
                items.Add(new LineDisplayItem("Background Alt 2", rom.GetLevel(level).BgAltPalette.Offset + 4, 4, rom.data));
                items.Add(new LineDisplayItem("Background Alt 3", rom.GetLevel(level).BgAltPalette.Offset + 8, 4, rom.data));
                items.Add(new LineDisplayItem("Background Alt 4", rom.GetLevel(level).BgAltPalette.Offset + 12, 4, rom.data));
            }

            items.Add(new LineDisplayItem("Sprite 1", rom.GetLevel(level).SpritePalette.Offset, 4, rom.data));
            items.Add(new LineDisplayItem("Sprite 2", rom.GetLevel(level).SpritePalette.Offset + 4, 4, rom.data));
            items.Add(new LineDisplayItem("Sprite 3", rom.GetLevel(level).SpritePalette.Offset + 8, 4, rom.data));
            items.Add(new LineDisplayItem("Sprite 4", rom.GetLevel(level).SpritePalette.Offset + 12, 4, rom.data));

            return items;
        }

        string IRomDataObject.DisplayName { get { return "Palette Table"; } }


        #endregion
    }

    /// <summary>
    /// Identifies location of a pointer table for the Pointer Explorer.
    /// </summary>
    class PointerTableData : RomDataParentObject
    {
        byte[] romData;
        int offset;
        int pointerCount;
        string displayName;

        public PointerTableData(int offset, int pointerCount, string text, MetroidRom Rom) {
            this.offset = offset;
            this.pointerCount = pointerCount;
            this.displayName = text;
            this.romData = Rom.data;
        }
        public override int Offset { get { return offset; } }

        public override int Size { get { return pointerCount * 2; } }

        public override string DisplayName { get { return displayName; } }

        public override bool HasListItems { get { return true; } }

        public override bool HasSubItems { get { return false; } }

        public override IList<LineDisplayItem> GetListItems() {
            List<LineDisplayItem> items = new List<LineDisplayItem>(pointerCount);
            for (int i = 0; i < pointerCount; i++) {
                int offset = this.offset + i * 2;
                pCpu p = new pCpu(romData, offset);
                items.Add(new LineDisplayItem(
                        i.ToString("x") + ":" + p.ToString(),
                        offset,
                        2,
                        romData
                    ));
            }
            return items;
        }
    }
    /// <summary>
    /// Represents a section of memory that data can be stored in.
    /// </summary>
    public enum LevelDataStore
    {
        /// <summary>Represents an invalid or empty value.</summary>
        None,
        /// <summary>New data store available in expanded ROMs created by relocating data.</summary>
        ExpandoStore,
        /// <summary>The region of memory that level data is stored in for an unmodified ROM.</summary>
        StandardStore
    }
}

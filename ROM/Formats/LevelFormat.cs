// todo: expando is same as standard, but justin bailey is group 17 instead of 27

using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM.Formats
{
    // Todo: Ideally, the RomFormat and LevelFormat class should do the following:
    //      -Initialize with "editing session invariants": things that never change in an editing session
    //          -examples: combo table location, offsets of the pointers to combos/ptables
    //          -Invariants will be different for different formats
    //              -Struct0 and Room0 have invariant offsets in standard ROM
    //              -Struct0 and Room0 have varying offsets in standard ROM
    //      -Produce an object that identifies the location of various objects of the newlny opened ROM
    //          -These object should be discarded after loading the ROM as the information it contains may become inaccurate
    //      -Manage aspects of saving that vary between the formats

    /// <summary>
    /// Expsoses level data and structure for different Metroid ROM formats used by Editroid.
    /// This class is meant to encapsulate the differences between the formats.
    /// </summary>
    public abstract class LevelFormat
    {
        public Level Level { get; private set; }
        public RomFormat RomFormat { get { return Level.Rom.Format; } }

        public LevelFormat(Level level) {
            this.Level = level;
            LevelBankIndex = GetLevelBank();
        }

        ////public virtual bool HasAlternateBgPal {
        ////    get {
        ////        return (Level.Index == LevelIndex.Brinstar || Level.Index == LevelIndex.Norfair|| Level.Index == LevelIndex.Ridley);
        ////    }
        ////}
        public virtual ItemExpansion ItemExpansionMode { get { return ItemExpansion.ExpandForward; } }

        /// <summary>Returns a collection of "blobs" of CHR data, along with metadata regarding the
        /// usage of these blobs.</summary>
        /// <returns>Stuff.</returns>
        public virtual ChrDump GetRawPatterns() { return null; } // Todo: ABSTRACT

        public abstract PatternTable LoadSpritePatternsTable(bool linear);
        public abstract PatternTable LoadBgPatternTable(bool linear);
        public abstract PatternTable LoadBgPatternTable_ChrUsage(bool linear, int chrBank);

        public abstract int FreeStructureMemory { get; }
        public abstract int FreeScreenMemory { get; }
        public abstract int AvailableItemMemory { get; }

        public virtual bool Uses24bitScreenPointers { get { return false; } }
        public virtual int FreeGeneralAsmSpace { get { return FreeScreenMemory; } }

        /// <summary>Gets the CHR group that references "Justin Bailey" player graphics, or null if the ROM format does not use CHR RAM.</summary>
        public virtual int? JustinBaileyChrGroup { get { return null; } }

        /// <summary>
        /// Returns the offset of the first byte AFTER used structure space.
        /// </summary>
        /// <returns></returns>
        public int FindEndOfUsedStructMemory() {
            var lastStruct = Level.Structures[Level.Structures.Count - 1];
            int endOfUsedSpace = lastStruct.Offset + lastStruct.Size;
            return endOfUsedSpace;
        }

        #region Banks
        protected IList<Bank> Banks { get { return Level.Rom.Banks; } }
        public int LevelBankIndex { get; private set; }

        protected int GetLevelBank() {
            switch (Level.Index) {
                case LevelIndex.Brinstar: return 1;
                case LevelIndex.Norfair: return 2;
                case LevelIndex.Tourian: return 3;
                case LevelIndex.Kraid: return 4;
                case LevelIndex.Ridley: return 5;
                default: throw new ArgumentException("invalid level index");
            }
        }

        protected virtual int GetStructPtableBank() {
            return LevelBankIndex;
        }
        protected virtual int GetStructDataBank() {
            return LevelBankIndex;
        }
        protected virtual int GetRoomPtableBank() {
            return LevelBankIndex;
        }
        protected virtual int GetRoomDataBank() {
            return LevelBankIndex;
        }
        protected virtual int GetComboBank() {
            return LevelBankIndex;
        }

        public Bank StructPtableBank { get { return Banks[GetStructPtableBank()]; } }
        public Bank StructDataBank { get { return Banks[GetStructDataBank()]; } }
        public Bank RoomDataBank { get { return Banks[GetRoomDataBank()]; } }
        public Bank RoomPtableBank { get { return Banks[GetRoomPtableBank()]; } }
        public Bank ComboBank { get { return Banks[GetComboBank()]; } }

        #endregion

        public pRom ComboDataOffset {
            get {
                var pCombos = Level.Bank.GetPtr(pPointerToComboData);
                return ComboBank.ToOffset(pCombos);
            }
        }
        public pRom StructPtableOffset {
            get {
                var pStructTable = Level.Bank.GetPtr(pPointerToStructPTable);
                return StructPtableBank.ToOffset(pStructTable);
            }
        }
        public pRom StructDataOffset {
            get { // Simply dereference first pointer
                var pStructData = Level.Rom.GetPointer(StructPtableOffset);
                return StructDataBank.ToOffset(pStructData);
            }
        }
        public pRom RoomPtableOffset {
            get {
                var pRoomTable = Level.Bank.GetPtr(pPointerToRoomPTable);
                return RoomPtableBank.ToOffset(pRoomTable);
            }
        }
        public pRom RoomDataOffset {
            get { // Simply dereference first pointer
                var pRoomData = Level.Rom.GetPointer(RoomPtableOffset);
                return RoomDataBank.ToOffset(pRoomData);
            }
        }
        /// <summary>
        /// Gets the offset of the beginning of item data. THIS MAY NOT BE
        /// THE BEGINNING OF THE LINKED LIST if the links in the list are
        /// not stored in order. To get the beginning of the linked list, use
        /// ItemDataFirstLinkOffset.
        /// </summary>
        public pRom ItemDataOffset {
            get {
                return Level.ItemTable_DEPRECATED.GetFirstRowOffset();
            }
        }
        /// <summary>
        /// Pointer to first link in item data linked list. This may not be the first
        /// link that appears in memory, because the links are not required to appear in order.
        /// </summary>
        public pRom ItemDataFirstLinkOffset {
            get {
                // PointerToItemList points to the first link, not necessarily
                // the first item in the row.
                var pLink = Level.Bank.GetPtr(pPointerToItemList);
                return Level.Bank.ToOffset(pLink);
            }
        }
        public pCpu pPointerToRoomPTable { get { return new pCpu(0x959A); } }
        public pCpu pPointerToStructPTable { get { return new pCpu(0x959C); } }
        public pCpu pPointerToComboData { get { return new pCpu(0x959E); } }
        public pCpu pPointerToItemList { get { return new pCpu(0x9598); } }
        public pCpu pPointerToItemMusicRoomList { get { return new pCpu(0xE759); } }
        public pCpu pItemMusicRoomCount { get { return new pCpu(0xE75F); } }

        /// <summary>
        /// Examines the ROM image and determines the number of structures. This requires the ROM image to be up to date.
        /// </summary>
        public abstract int CalculateStructCount();
        /// <summary>
        /// Examines the ROM image and determines the number of rooms. This requires the ROM image to be up to date.
        /// </summary>
        public abstract int CalculateRoomCount();

        /// <summary>
        /// Manages the level-data-saving operation by calling serialization functions
        /// in the proper order, which depends on the ROM format.
        /// </summary>
        public abstract void PerformSaveOperation(ILevelDataSerializer saver);

        public virtual void GetGeneralAsmInsertionBank(out int romOffset, out int size) {
            romOffset = Level.Screens.DataBank.Offset;
            size = 0x4000;
        }

        public pRom AltMusicOffset {
            get {
                ////return RomFormat.GetAltMusicOffset(Level.Index);
                pCpu ppList = pPointerToItemMusicRoomList;
                pCpu pList = Level.Rom.FixedBank.GetPtr(ppList);
                return Level.Bank.ToOffset(pList);
            }
        }
        public virtual int AltMusicRoomCount {
            get {
                pCpu pCount = pItemMusicRoomCount;
                return Level.Rom.FixedBank[pCount];
            }
        }

        public virtual bool SupportsAltBgPalette { get { return Level.Index == LevelIndex.Brinstar || Level.Index == LevelIndex.Norfair || Level.Index == LevelIndex.Ridley; } }
        public virtual bool SupportsAltSpritePalette { get { return false; } }

        public virtual pRom GetTilePhysicsTableLocation() {
            return pRom.Null;
        }
    }

    public enum ItemExpansion
    {
        /// <summary>
        /// Indicates that when item data increases in size, the data should be expanded normally
        /// (beginning offset stays the same). Likewise data should decrease in size normally.
        /// </summary>
        ExpandForward,
        /// <summary>
        /// Indicates that when item data increases in size, the new data should END at the same offset
        /// as the old data. The beginning of the data should be moved back to create more space.
        /// Conversely, when item data decreases in size, the beginning of the data should move forward.
        /// </summary>
        ExpandBackward
    }

    /// <summary>
    /// Represents the level format used by an unexpanded Metroid ROM
    /// </summary>
    class StandardLevelFormat : LevelFormat
    {
        public StandardLevelFormat(Level l) : base(l) { }

        public override int? JustinBaileyChrGroup { get { return 27; } }

        public override ChrDump GetRawPatterns() {
            var dumper = new StandardChrDumper(Level.Rom, Level.Index);
            return dumper.GetRawPatterns();
        }

     

        public override PatternTable LoadSpritePatternsTable(bool linear) {
            return new PatternTable(linear, PatternTableType.sprite, Level.Rom.GlobalPatternGroups, Level.PatternGroups, Level.Rom);
        }
        public override PatternTable LoadBgPatternTable(bool linear) {
            return new PatternTable(linear, PatternTableType.background, Level.Rom.GlobalPatternGroups, Level.PatternGroups, Level.Rom);
        }
        public override PatternTable LoadBgPatternTable_ChrUsage(bool linear, int chrBank) {
            return LoadBgPatternTable(linear);
            // chrBank param is ignored since standard format uses CHR RAM
        }

        public int TotalStructureMemory {
            get {
                return ComboDataOffset - StructPtableOffset;
            }
        }

        public int UsedStructureMemory {
            get {
                // Used bytes = end of used data - start of data
                return FindEndOfUsedStructMemory() - Level.Pointers_depracated.StructTable.Offset;
            }
        }

        public override int FreeStructureMemory {
            ////get { return TotalStructureMemory - UsedStructureMemory; }
            get {
                int endOfAvailSpace = ComboDataOffset;
                int startOfAvailSpace = StructDataOffset;

                int memSize = endOfAvailSpace - startOfAvailSpace;

                for (int i = 0; i < Level.Structures.Count; i++) {
                    memSize -= Level.Structures[i].Size;
                }

                return memSize;
            }
        }



        public override int CalculateStructCount() {
            return Level.Pointers_depracated.structs.Count;
        }

        public override int FreeScreenMemory {
            get {
                int endOfAvailSpace = StructDataOffset;
                int startOfAvailSpace = RoomDataOffset;

                int memSize = endOfAvailSpace - startOfAvailSpace;

                for (int i = 0; i < Level.Screens.Count; i++) {
                    memSize -= Level.Screens[i].Size;
                }

                return memSize;
            }
        }

        public override int AvailableItemMemory {
            get {
                // Available item memory is from the end of struct p table to start of room data
                var endOfMem = StructDataOffset;
                var startOfMem = StructPtableOffset + Level.StructCount * 2;

                return endOfMem - startOfMem;
            }
        }

        public override int CalculateRoomCount() {
            ////var ppStructTable = PointerToStructPTable;
            ////var pStructTable = Level.Bank.GetPtr(ppStructTable);

            ////var ppRoomTable = PointerToRoomPTable;
            ////var pRoomTable = Level.Bank.GetPtr(ppRoomTable);

            return (StructPtableOffset - RoomPtableOffset) / 2; // 2 bytes per pointer in table, assumes pointer tables are contiguous
        }

        public override void PerformSaveOperation(ILevelDataSerializer saver) {
            // Implements expando behavior

            // Struct pTable location is fixed
            int structTableOffset = (int)StructPtableOffset;
            // Struct0 location is fixed, get from pointer table
            pCpu pStructData = Level.Rom.GetPointer((pRom)structTableOffset);
            int structDataOffset = (int)Level.Structures.DataBank.ToOffset(pStructData);

            // Room pTable location is fixed
            int roomTableOffset = (int)RoomPtableOffset;
            // Room0 location is fixed, get from pointer table
            pCpu pRoomData = Level.Rom.GetPointer((pRom)roomTableOffset);
            int roomDataOffset = (int)Level.Screens.DataBank.ToOffset(pRoomData);

            // We serialize structs before their pointers so they can calculate and cache their offsets
            var refStructDataOffset = structDataOffset;
            saver.SerializeStructData(ref refStructDataOffset);
            saver.SerializeStructTable(ref structTableOffset);

            // Serialize rooms before pointers so they can calculate offsets
            var refRoomDataOffset = roomDataOffset;
            saver.SerializeRoomData(ref refRoomDataOffset);
            saver.SerializeRoomTable(ref roomTableOffset);

        }


    }

    class ExpandoLevelFormat : LevelFormat
    {
        public ExpandoLevelFormat(Level l) : base(l) { }

        public override int? JustinBaileyChrGroup { get { return 17; } }

        public override ChrDump GetRawPatterns() {
            var dumper = new ExpandoChrDumper(Level.Rom, Level.Index);
            return dumper.GetRawPatterns();
        }

        public override PatternTable LoadSpritePatternsTable(bool linear) {
            return PatternTable.LoadNewTableForExpandedRom(linear, PatternTableType.sprite, Level);
        }
        public override PatternTable LoadBgPatternTable(bool linear) {
            return PatternTable.LoadNewTableForExpandedRom(linear, PatternTableType.background, Level);
        }
        public override PatternTable LoadBgPatternTable_ChrUsage(bool linear, int chrBank) {
            // chrBank param is ignored since expando uses CHR RAM
            return LoadBgPatternTable(linear);
        }

        public override ItemExpansion ItemExpansionMode {
            get { return ItemExpansion.ExpandBackward; }
        }


        public override int FreeStructureMemory {
            get {
                // Free space = end of available data - end of used data
                int memSize = EndOfAvailStructMemory - StructPtableOffset;

                // Pointers are 2 bytes ea
                memSize -= Level.StructCount * 2;

                for (int i = 0; i < Level.Structures.Count; i++) {
                    memSize -= Level.Structures[i].Size;
                }

                return memSize;
            }
        }

        public int EndOfAvailStructMemory {
            get {
                // Item data goes at the end of one of two "stores"
                if (Level.ItemDataStore == LevelDataStore.ExpandoStore) {
                    // Item data is at end of store
                    // Available struct data ends where item data begins
                    return (int)Level.ItemTable_DEPRECATED.GetFirstRowOffset();
                } else {
                    // All the data up to the end of the store is available.
                    return Level.ToPRom(Level.EndOfExpandoStore);
                }
            }
        }

        public pRom EndOfAvailScreenMemory {
            get {
                if (Level.ItemDataStore == LevelDataStore.StandardStore) {
                    // Item data is at end of avail screen data
                    return Level.ItemTable_DEPRECATED.GetFirstRowOffset();
                } else {
                    return Level.ToPRom(Level.EndOfStandardStore);
                }
            }
        }

        public override int CalculateStructCount() {
            var pTable = Level.Bank.GetPtr(Level.Format.pPointerToStructPTable);
            var pStruct0 = StructPtableBank.GetPtr(pTable);

            // Structs immediately follow pointers. Pointers are two bytes each.
            return (pStruct0 - pTable) / 2;
        }

        public override int AvailableItemMemory {
            get {
                // Available item memory = item memory in use + free space in memory store (expando or standard store)
                int itemDataSize = Level.ItemTable_DEPRECATED.GetTotalBytes();

                if (Level.ItemDataStore == LevelDataStore.StandardStore) {
                    return itemDataSize + FreeScreenMemory;
                } else if (Level.ItemDataStore == LevelDataStore.ExpandoStore) {
                    return itemDataSize + FreeStructureMemory;
                }

                throw new Exception("something bad happened");
            }
        }

        public override int FreeScreenMemory {
            get {
                var memSize = EndOfAvailScreenMemory - RoomPtableOffset;

                // 2 bytes per pointer
                memSize -= 2 * Level.Screens.Count;

                for (int i = 0; i < Level.Screens.Count; i++) {
                    memSize -= Level.Screens[i].Size;
                }

                return memSize;
                ////// Free mem goes until start of struct data
                ////Screen lastScreen = Level.Screens[Level.Screens.Count - 1];
                ////int endOfUsedScreenMemory = lastScreen.Offset + lastScreen.Size;

                ////int screenDataStart = Level.Screens[0].Offset;
                ////int screenDataSize = 0;
                ////for (int i = 0; i < Level.Screens.Count; i++) {
                ////    screenDataSize += Level.Screens[i].Size;
                ////}


                ////return EndOfAvailScreenMemory - screenDataStart - screenDataSize;
            }
        }
        public override int CalculateRoomCount() {
            // Assumes room data immediately follows room ptable

            var ppRoomTable = pPointerToRoomPTable;
            var pRoomTable = Level.Bank.GetPtr(ppRoomTable);
            var pRoom0 = Level.Rom.Banks[GetRoomPtableBank()].GetPtr(pRoomTable);

            // Sanity check (who knows what insane thing I'll do down the road that breaks this function)
            if (GetRoomDataBank() != GetRoomPtableBank()) throw new Exception("Holy crap! Oh dear god! Oh no! Oh noes!");

            return (pRoom0 - pRoomTable) / 2; // Pointers are two bytes
        }

        public override void PerformSaveOperation(ILevelDataSerializer saver) {
            // Implements expando behavior

            // Struct pTable location is fixed
            int structTableOffset = (int)StructPtableOffset;
            // Structs follow table
            int structDataOffset = structTableOffset + Level.Structures.Count * 2;
            // Room pTable location is fixed
            int roomTableOffset = (int)RoomPtableOffset;
            // Rooms follow table
            int roomDataOffset = roomTableOffset + Level.Screens.Count * 2;

            // We serialize structs before their pointers so they can calculate and cache their offsets
            var refStructDataOffset = structDataOffset;
            saver.SerializeStructData(ref refStructDataOffset);
            saver.SerializeStructTable(ref structTableOffset);

            // Serialize rooms before pointers so they can calculate offsets
            var refRoomDataOffset = roomDataOffset;
            saver.SerializeRoomData(ref refRoomDataOffset);
            saver.SerializeRoomTable(ref roomTableOffset);

#if DEBUG
            if (this is StandardLevelFormat || this is ExpandoLevelFormat) {
                // structTableOffset should be updated to point just beyond the table, which is where the data belongs
                System.Diagnostics.Debug.Assert(structTableOffset == structDataOffset);
                // roomTableOffset should be updated to point just beyond the table, which is where the data belongs
                System.Diagnostics.Debug.Assert(roomTableOffset == roomDataOffset);
            }
#endif
        }


    }

    class EnhancedLevelFormat : ExpandoLevelFormat
    {
        public EnhancedLevelFormat(Level l) : base(l) { }

        public override PatternTable LoadBgPatternTable(bool linear) {
            int bankIndex = Level.PreferredChrBank ?? Level.Rom.ChrUsage.GetBgFirstPage(Level.Index);

            return LoadBgPatternTable_ChrUsage(linear, bankIndex);
        }
        public override PatternTable LoadBgPatternTable_ChrUsage(bool linear, int bankIndex) {
            var oBgPatterns = EnhancedPatternOffsets.GetChrBank(bankIndex);

            PatternTable pt = new PatternTable(linear);

            try {
                pt.BeginWrite();
                pt.LoadTiles(Level.Rom.data, oBgPatterns, 0, 0x100);
                return pt;
            } finally {
                pt.EndWrite();
            }
        }
        public override PatternTable LoadSpritePatternsTable(bool linear) {
            var oSpritePatterns = EnhancedPatternOffsets.GetSprBank(Level.Index);

            PatternTable pt = new PatternTable(linear);
            try {
                pt.BeginWrite();
                pt.LoadTiles(Level.Rom.data, oSpritePatterns, 0, 0x100);
                return pt;
            } finally {
                pt.EndWrite();
            }
        }

        public override ItemExpansion ItemExpansionMode {
            get {
                return ItemExpansion.ExpandForward;
            }
        }
        public override int FreeScreenMemory { get { return GetFreeMemInRoomBank(); } }

        public override int FreeStructureMemory { get { return GetFreeMemInRoomBank(); } }


        public override int AvailableItemMemory {
            get {
                var memEnd = Level.Bank.ToOffset(Level.EndOfExpandoStore);
                var memStart = Level.ItemTable_DEPRECATED.GetFirstRowOffset();

                return memEnd - memStart;
            }
        }

        private int GetFreeMemInRoomBank() {
            // (We don't consider the memory used by combos since they are fixed in size and location)

            // Data starts with the struct table and ends at the end of the bank.
            pCpu memStart = Level.Bank.GetPtr(pPointerToStructPTable);
            pCpu memEnd = (pCpu)0xC000;

            int memSize = memEnd - memStart;

            // subtract memory that is in use
            memSize -= Level.StructCount * 2; // Struct pTable uses two bytes per struct
            for (int i = 0; i < Level.Structures.Count; i++) {
                memSize -= Level.Structures[i].Size;
            }
            for (int i = 0; i < Level.Screens.Count; i++) {
                memSize -= Level.Screens[i].Size;
            }

            return memSize;
        }
        protected override int GetRoomDataBank() { return LevelBankIndex + 7; }
        protected override int GetStructDataBank() { return LevelBankIndex + 7; }
        protected override int GetStructPtableBank() { return LevelBankIndex + 7; }
        protected override int GetComboBank() { return LevelBankIndex + 7; }

        public override int CalculateRoomCount() {
            var pRoomTable = Level.Bank.GetPtr(pPointerToRoomPTable);
            var ptrBank = Level.Format.RoomPtableBank;


            // We'll walk the pointer table until we find the $FFFF terminator
            var ppRoom = pRoomTable;

            var pRoom = RoomPtableBank.GetPtr(ppRoom);
            int count = 0;

            while (pRoom.Value != 0xFFFF && count < 0xFF) {
                count++;
                // Point to next pointer
                ppRoom += 2;
                // Get pointer
                pRoom = ptrBank.GetPtr(ppRoom);
            }

            return count;

        }

        /// <summary>
        /// Manages the level-data-saving operation by calling serialization functions
        /// in the proper order, which depends on the ROM format.
        /// </summary>
        public override void PerformSaveOperation(ILevelDataSerializer saver) {
            // Implements standard and expando behavior

            // Struct pTable location is fixed
            int structTableOffset = (int)StructPtableOffset;
            // Structs follow table
            int structDataOffset = structTableOffset + Level.Structures.Count * 2;
            // Room pTable location is fixed
            int roomTableOffset = (int)RoomPtableOffset;
            // Room data will follow struct data

            // Rooms follow structs, and their offset wont be known until structs are written

            // We serialize structs before their pointers so they can calculate and cache their offsets
            saver.SerializeStructData(ref structDataOffset);
            saver.SerializeStructTable(ref structTableOffset);

            // Now that we've saved our structs, structDataOffset should point to where room data should go
            var roomDataOffset = structDataOffset;

            // Serialize rooms before pointers so they can calculate offsets
            saver.SerializeRoomData(ref roomDataOffset);
            saver.SerializeRoomTable(ref roomTableOffset);

        }

        public override bool SupportsAltBgPalette { get { return true; } }
        public override bool SupportsAltSpritePalette { get { return true; } }
    }

    class Mmc3LevelFormat : EnhancedLevelFormat
    {
        public Mmc3LevelFormat(Level l)
            : base(l) {
        }
        public override int CalculateRoomCount() {
            var offset = this.RoomPtableOffset + 1;
            int roomCount = 0;
            const int MaxRoomCount = 255;

            while (roomCount < MaxRoomCount) {
                // $0000 indicates end of table
                if (Level.Rom.GetPointer(offset).Value == 0)
                    return roomCount;

                // Otherwise its a room
                roomCount++;
                offset += 3;
            }

            return MaxRoomCount;
        }
        public override void PerformSaveOperation(ILevelDataSerializer saver) {
            base.PerformSaveOperation(saver);
        }

        public int GetUsedScreenMem() {
            int usedMem = 0;

            // Add up size of all screens for all levels
            for (int i = 0; i < RomFormat.Rom.Levels.Count; i++) {
                var level = RomFormat.Rom.Levels[(LevelIndex)i];

                for (int j = 0; j < level.Screens.Count; j++) {
                    usedMem += level.Screens[j].Size;
                }
            }

            return usedMem;
        }
        public override int FreeScreenMemory {
            get {
                // Calculate total available memory

                var banks = RomFormat.Rom.BankAllocation;
                int totalMem = 0;
                for (int i = 0; i < banks.Length; i++) {
                    var bank = banks[i];
                    if (!bank.Reserved & !bank.UserReserved) {
                        totalMem += Mmc3.PrgBankSize;
                    }
                }

                // Free mem = available mem - used mem
                return totalMem - GetUsedScreenMem();
            }
        }
        public override int FreeStructureMemory {
            get {
                return 0x2000 - GetUsedStructMem();
            }
        }

        private int GetUsedStructMem() {
            int usedMem = 0;
            var structs = Level.Structures;
            for (int i = 0; i < structs.Count; i++) {
                usedMem += structs[i].Size;
            }

            return usedMem;
        }
        protected override int GetRoomDataBank() {
            return base.GetRoomDataBank();
        }
        protected override int GetStructDataBank() {
            return base.GetStructDataBank();
        }
        protected override int GetRoomPtableBank() {
            return base.GetRoomPtableBank();
        }
        protected override int GetStructPtableBank() {
            return base.GetStructPtableBank();
        }

        protected override int GetComboBank() {
            return base.GetComboBank();
        }
        public override int AvailableItemMemory {
            get {
                return base.AvailableItemMemory;
            }
        }


        public override bool Uses24bitScreenPointers {
            get {
                return true;
            }
        }


        /// <summary>
        /// Returns true if the CHR usage table should be used in the same manner is an enhanced ROM, rather than the new CHR animation table
        /// </summary>
        bool useChrUsage { get { return Level.Rom.ChrAnimationTableMissing; } }

        public override PatternTable LoadBgPatternTable(bool linear) {
            if (useChrUsage) {
                int bankIndex = Level.PreferredChrBank ?? Level.Rom.ChrUsage.GetBgFirstPage(Level.Index);

                return LoadBgPatternTable_ChrUsage(linear, bankIndex);
            } else {
                var animation = Level.Rom.PreferredAnimationIndex;
                // Clamp to valid range
                if (animation < 0) animation = 0;
                if (animation > Level.ChrAnimation.Animations.Count - 1) animation = Level.ChrAnimation.Animations.Count - 1;

                return LoadBgPatternTable_ChrAnimation(linear, Level.ChrAnimation.Animations[animation]);
            }
        }

        private PatternTable LoadBgPatternTable_ChrAnimation(bool linear, ChrAnimationTable anim) {
            //PatternTable pt = new PatternTable(linear);

            var frame = anim.Frames[0];


            var bank0Offset = Level.Rom.Format.ChrRomOffset + Mmc3.ChrBankSize * frame.Bank0;
            var bank1Offset = Level.Rom.Format.ChrRomOffset + Mmc3.ChrBankSize * frame.Bank1;
            var bank2Offset = Level.Rom.Format.ChrRomOffset + Mmc3.ChrBankSize * frame.Bank2;
            var bank3Offset = Level.Rom.Format.ChrRomOffset + Mmc3.ChrBankSize * frame.Bank3;
            PatternTable pt = new PatternTable(linear);
            try {
                pt.BeginWrite();
                pt.LoadTiles(Level.Rom.data, bank0Offset, 0x00, 0x40);
                pt.LoadTiles(Level.Rom.data, bank1Offset, 0x40, 0x40);
                pt.LoadTiles(Level.Rom.data, bank2Offset, 0x80, 0x40);
                pt.LoadTiles(Level.Rom.data, bank3Offset, 0xC0, 0x40);
                return pt;
            } finally {
                pt.EndWrite();
            }
        }
        public override PatternTable LoadBgPatternTable_ChrUsage(bool linear, int bankIndex) {
            var oBgPatterns = MMC3PatternOffsets.GetChrBank(bankIndex);

            PatternTable pt = new PatternTable(linear);

            try {
                pt.BeginWrite();
                pt.LoadTiles(Level.Rom.data, oBgPatterns, 0, 0x100);
                return pt;
            } finally {
                pt.EndWrite();
            }
        }
        public override PatternTable LoadSpritePatternsTable(bool linear) {
            var oSpritePatterns = MMC3PatternOffsets.GetSprBank(Level.Index);

            PatternTable pt = new PatternTable(linear);
            try {
                pt.BeginWrite();
                pt.LoadTiles(Level.Rom.data, oSpritePatterns, 0, 0x100);
                return pt;
            } finally {
                pt.EndWrite();
            }
        }

        public override void GetGeneralAsmInsertionBank(out int romOffset, out int size) {
            romOffset = Level.Screens.DataBank.Offset;
            size = 0x2000;
        }

        public override int FreeGeneralAsmSpace {
            get {
                return FreeStructureMemory;
            }
        }

        public override pRom GetTilePhysicsTableLocation() {
            return (pRom)(0x7BB10 + 0x100 * (int)Level.Index);
        }
    }
}
//    /// <summary>
//    /// Represents a linear range of numbers.
//    /// </summary>
//    struct Range
//    {
//        /// <summary>The beginning of the range.</summary>
//        public int Start { get; private set; }
//        /// <summary>The size of the range.</summary>
//        public int Length { get; private set; }
//        /// <summary>The end of the range, exclusive. To get the last
//        /// number included in the range, use (End - 1).</summary>
//        public int End { get { return Start + Length; } }

//        public Range(int start, int length)
//            : this() {
//            this.Start = start;
//            this.Length = length;
//        }

//        public bool IsEmpty { get { return Start == 0 & Length == 0; } }
//    }

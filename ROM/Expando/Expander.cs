using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM.Expando
{
    /// <summary>
    /// Expands a standard metroid ROM to an Expando 
    /// </summary>
    class Expander
    {
        const int StandardRomSize = 0x20010;
        const int ExpandoRomSize = 0x40010;
        const int PrgRomSize = 0x4000;
        const int ExpandoSizeDifference = 0x20000;
        const int HeaderSize = 0x10;
        const int BankCountDescriptorOffset = 0x4;

        static readonly LevelIndex[] levels = new LevelIndex[] { (LevelIndex)0, (LevelIndex)1, (LevelIndex)2, (LevelIndex)3, (LevelIndex)4 };

        // Offsets where patterns will be moved to. For bg patterns, these will be "compiled" into a pseudo-ppu dump (the original Expando ROM used actual ppu dumps)
        static readonly int[] LevelBgDumpOffsets = new int[] { 0x1c010, 0x1D010, 0x20010, 0x1F010, 0x1E010 };
        static readonly int[] LevelSprOffsets = new int[] { 0x21010, 0x21410, 0x21810, 0x22010, 0x21C10 };
        static readonly int[] LevelSprGroups = new int[] { 6, 9, 0xE, 0x11, 0x13 };

        /// <summary>
        /// Lists cpu pointers to data pointers.
        /// </summary>
        public static class pointers
        {
            public static readonly pCpu FramePTables = (pCpu)0x95A0;
            public static readonly pCpu PlacementPTables = (pCpu)0x95A4;
            public static readonly pCpu ItemData = (pCpu)0x9598;
            public static readonly pCpu ScreenPTable = (pCpu)0x959A;
            public static readonly pCpu StructPTable = (pCpu)0x959C;
            public static readonly pCpu ComboData = (pCpu)0x959E;
            public static readonly pCpu FreeMem = (pCpu)0x8D60;
            public static readonly int FreeMemSize = 0x800;

            public static readonly pCpu PalData = (pCpu)0x9560;
            public const int PalPointerCount = 0x1C;

        }

        byte[] romData;
        byte[] newRomData;
        MetroidRom rom;


        public Expander(MetroidRom rom) {
            if (rom == null || rom.data.Length >= ExpandoRomSize)
                throw new ArgumentException("Specified rom image must be unexpanded");
            if (rom.data.Length < StandardRomSize)
                throw new ArgumentException("Specified rom image too small");

            this.rom = rom;
            this.romData = rom.data;

            PerformSimpleMmcExpand();

            foreach (var level in levels) {
                MoveBgPatterns(level);
                MoveSpritePatterns(level);
            }

            CopyMiscPatterns();
            DeleteOldPatterns();
            ApplyAsmPatches();
            RelocateData();
        }

        private void RelocateData() {
            LevelDataMoves.MoveLevelData(this, rom.Brinstar);
            LevelDataMoves.MoveLevelData(this, rom.Kraid);
            LevelDataMoves.MoveLevelData(this, rom.Norfair);
            LevelDataMoves.MoveLevelData(this, rom.Ridley);
            LevelDataMoves.MoveLevelData(this, rom.Tourian);
        }




        public byte[] ExpandedRomData { get { return newRomData; } }


        /////// <summary>
        /////// Moves level data within the expanded ROM data, zeroing out the location the data was moved from, and updating any pointers specified.
        /////// </summary>
        /////// <param name="sourceOffset"></param>
        /////// <param name="destOffset"></param>
        /////// <param name="len"></param>
        /////// <param name="pointerOffset"></param>
        /////// <param name="pointerCount"></param>
        ////void MoveLevelData(int sourceOffset, int destOffset, int len, int pointerOffset, int pointerCount) {
        ////    int pointerChange = destOffset - sourceOffset;

        ////    Array.Copy(newRomData, sourceOffset, newRomData, destOffset, len);

        ////    for (int i = 0; i < pointerCount; i++) {
        ////        int offset = pointerOffset + i * 2;
        ////        pCpu p = new pCpu(newRomData, offset);
        ////        p += pointerChange;
        ////        p.Write(newRomData, offset);
        ////    }
        ////}

        #region Assembly

        /// <summary>
        /// Applies precompiled assembly patches.
        /// </summary>
        private void ApplyAsmPatches() {
            byte[] ppuLoader = PrecompiledAsm.PpuLoader;
            byte[] bankingHack = PrecompiledAsm.BankingHack;

            Array.Copy(ppuLoader, 0, newRomData, 0x3C5E7, ppuLoader.Length);
            Array.Copy(bankingHack, 0, newRomData, 0x3C4FF, bankingHack.Length);

            UpdateMiscPointers();
        }

        /// <summary>
        /// Updates pointers to the newly patched PPU loader.
        /// </summary>
        private void UpdateMiscPointers() {
            Prg_WriteWord(0xC54D, 0xC5D7); // Update jsr InitTitleGFX
            Prg_WriteWord(0xC573, 0xC5FA); // Update jsr InitBrinstarGFX
            Prg_WriteWord(0xC58B, 0xC604); // Update jsr InitNorfairGFX
            Prg_WriteWord(0xC5A3, 0xC60E); // Update jsr InitTourianGFX
            Prg_WriteWord(0xC5BE, 0xC618); // Update jsr InitKraidGFX
            Prg_WriteWord(0xC5CB, 0xC622); // Update jsr InitRidlefGFX
            Prg_WriteWord(0xC5D5, 0xC62C); // Update jsr InitGFX6
            Prg_WriteWord(0xC7B7, 0xC63B); // update Lda GFXInfo, y

            // Update jsr InitGFX7 (called 3 times in title page bank: title, password, end of game)
            newRomData[0x1135] = 0x36;
            newRomData[0x1136] = 0xC6;

            newRomData[0x1378] = 0x36;
            newRomData[0x1379] = 0xC6;

            newRomData[0x13B9] = 0x36;
            newRomData[0x13BA] = 0xC6;
        }

        /// <summary>
        /// Writes a 2-byte word to the fixed PRG ROM found at $3C010
        /// </summary>
        private void Prg_WriteWord(int prgOffset, int data) {
            const int RomOffset = 0x3C010;
            const int RamOrigin = 0xC000;
            // lsB
            newRomData[prgOffset + RomOffset - RamOrigin] = (byte)(data & 0xFF);
            newRomData[prgOffset + RomOffset - RamOrigin + 1] = (byte)((data >> 8) & 0xFF);
        }
        #endregion

        #region Patterns
        private void DeleteOldPatterns() {
            for (int i = 0; i < rom.PatternGroupOffsets.Count; i++) {
                var groupData = rom.PatternGroupOffsets[i];

                int offset = groupData.SourceRomOffset;
                int dataSize = groupData.ByteCount;

                for (int iData = 0; iData < dataSize; iData++) {
                    newRomData[iData + offset] = 0;
                }
            }
        }

        private void CopyMiscPatterns() {
            // Player sprites
            CopyPatternGroup(0x00, 0x22410);
            // Title screen sprites
            CopyPatternGroup(0x14, 0x22DB0);
            // Title screen bg
            CopyPatternGroup(0x15, 0x22F10);
            // Letters
            CopyPatternGroup(0x17, 0x23410);
            // Suitless sprites
            CopyPatternGroup(0x1B, 0x23810);
            // "The End" graphics
            CopyPatternGroup(0x02, 0x24010);
            // Ending sprites
            CopyPatternGroup(0x01, 0x24410);
        }


        private void MoveSpritePatterns(LevelIndex level) {
            int patternGroupIndex = LevelSprGroups[(int)level];
            var groupData = rom.PatternGroupOffsets[patternGroupIndex];
            int dest = LevelSprOffsets[(int)level];

            CopyPatternGroup(groupData, dest);
        }

        private void CopyPatternGroup(int groupIndex, int dest) {
            CopyPatternGroup(rom.PatternGroupOffsets[groupIndex], dest);
        }
        private void CopyPatternGroup(PatternGroupOffsets groupData, int dest) {
            // Calculate location (source & dest) of data and size
            int sourceOffset = groupData.SourceRomOffset;
            int dataSize = groupData.ByteCount;

            Array.Copy(rom.data, sourceOffset, newRomData, dest, dataSize);
        }

        private void MoveBgPatterns(LevelIndex index) {
            Level level = rom.Levels[index];
            var patternGroupIndecies = level.PatternGroups;
            int DestinationBaseOffset = LevelBgDumpOffsets[(int)index];

            for (int i = 0; i < patternGroupIndecies.Count; i++) {
                int groupIndex = patternGroupIndecies[i];
                var groupData = rom.PatternGroupOffsets[groupIndex];

                if (groupData.IsBackground)
                    CopyBgPatternGroup(groupData, DestinationBaseOffset);
            }
        }

        /// <summary>
        /// Copies pattern groups for pseudo-ppu-dumps (used to generate a compiled set of bg patterns for each level)
        /// </summary>
        /// <param name="groupData">Information about the pattern group</param>
        /// <param name="baseOffset">The base offset of the 1 kb block of pattern data.</param>
        private void CopyBgPatternGroup(PatternGroupOffsets groupData, int baseOffset) {
            if (!groupData.IsBackground) throw new ArgumentException("Attempted to use sprite patterns to create a compiled bg pattern set.", "groupData");

            // Calculate location (source & dest) of data and size
            int sourceOffset = groupData.SourceRomOffset;
            int ppuDest = groupData.DestPpuOffset;
            int relativeOffset = ppuDest - 0x1000; // Bg patterns start at 0x1000 in PPU mem
            int offset = baseOffset + relativeOffset;
            int dataSize = groupData.ByteCount;

            Array.Copy(rom.data, sourceOffset, newRomData, offset, dataSize);
        }
        #endregion


        /// <summary>
        /// Increases the size of the ROM image, moves the last (fixed) PRG bank to the end of the image, and updates the
        /// header. Result is stored in newRomData.
        /// </summary>
        private void PerformSimpleMmcExpand() {
            newRomData = new byte[ExpandoRomSize];

            // We're making a point of ignoring any extraneous data beyond $20010 (standard ROM size)--ignore editroid map or other junk data
            int unmovedDataSize = StandardRomSize - PrgRomSize;
            int fixedPrgOffset = StandardRomSize - PrgRomSize;
            int sizeDiff = newRomData.Length - StandardRomSize;
            int fixedPrgNewOffset = fixedPrgOffset + sizeDiff;

            Array.Copy(romData, newRomData, unmovedDataSize);
            Array.Copy(romData, fixedPrgOffset, newRomData, fixedPrgNewOffset, PrgRomSize);

            // Update iNes header
            int bankCount = (newRomData.Length - HeaderSize) / PrgRomSize;
            newRomData[BankCountDescriptorOffset] = (byte)bankCount;


        }
    }

    abstract class LevelDataMoves
    {
        public static void MoveLevelData(Expander e, Level l) {
            LevelDataMoves m;
            switch (l.Index) {
                case LevelIndex.Brinstar:
                    m = new BrinstarDataMoves(e, l);
                    break;
                case LevelIndex.Norfair:
                    m = new NorfairDataMoves(e, l);
                    break;
                case LevelIndex.Tourian:
                    m = new TourianDataMoves(e, l);
                    break;
                case LevelIndex.Kraid:
                    m = new KraidDataMoves(e, l);
                    break;
                case LevelIndex.Ridley:
                    m = new RidleyDataMoves(e, l);
                    break;
                case LevelIndex.None:
                default:
                    throw new ArgumentException("The specified level did not have a valid index.", "l");
            }

            m.PerformDataMoves();
        }

        protected LevelDataMoves(Expander e, Level level) {
            expander = e;
            newRomData = expander.ExpandedRomData;
            this.level = level;

        }

        bool performed = false;
        public void PerformDataMoves() {
            if (performed) throw new InvalidOperationException("Method PerformDataMoves has already been called on this object.");

            CreateSegmentList();
            CreateQueue();
            UpdateItemLinkedList();
            DoMoves();

            performed = true;
        }


        Expander expander;
        byte[] newRomData;
        Level level;

        #region Data segement fields
        protected DataSegment placementData = new DataSegment();
        protected DataSegment placementPointers = new DataSegment();
        protected DataSegment frameData = new DataSegment();
        protected DataSegment framePointers = new DataSegment();
        protected DataSegment itemData = new DataSegment();
        protected DataSegment palData = new DataSegment();
        protected DataSegment roomPointers = new DataSegment();
        protected DataSegment roomData = new DataSegment();
        protected DataSegment structPointers = new DataSegment();
        protected DataSegment structData = new DataSegment();
        protected DataSegment comboData = new DataSegment();

        protected List<DataSegment> dataSegments = new List<DataSegment>(); // Segments ordered by index (index as defined by levelDataTypes)
        protected List<DataSegment> sourceOrderedSegments = new List<DataSegment>(); // Segments ordered by source offset
        protected List<DataSegment> moveQueue = new List<DataSegment>(); // List of segments to be moved
        List<byte[]> moveData = new List<byte[]>(); // Temporary list for data in transit

        private void CreateSegmentList() {
            dataSegments.AddRange(new DataSegment[]{
                frameData,
                framePointers,
                placementData,
                placementPointers,
                palData,
                roomPointers,
                roomData,
                structPointers,
                structData,
                itemData,
                comboData,
            });

            sourceOrderedSegments.AddRange(dataSegments);
        }
        #endregion



        private void UpdateItemLinkedList() {
            int dist = itemData.DataMoveDist;

            var pointerToData = Expander.pointers.ItemData.AsPRom(level); // Offset of program pointer to first item
            var itemPointer = new pCpu(newRomData, pointerToData); // Address of first item
            var dataOffset = itemPointer.AsPRom(level); // Offset of first item

            bool done = false;
            while (!done) {
                var OffsetOfpNext = dataOffset + 1;
                pCpu pNext = new pCpu(newRomData, OffsetOfpNext);

                done |= pNext.Value == 0xFFFF;
                if (!done) {
                    // Update pointer to next item to where item data will be moved to
                    pCpu updatedPNext = pNext + dist;
                    updatedPNext.Write(newRomData, OffsetOfpNext);
                    // Update dataOffset to point to next item
                    dataOffset = pNext.AsPRom(level);
                }
            }

        }



        #region Data moving code
        private void DoMoves() {
            
            // Update pointers
            for (int i = 0; i < moveQueue.Count; i++) {
                UpdatePointers(i);
            }

            // Extract all data to be moved
            for (int i = 0; i < moveQueue.Count; i++) {
                var move = moveQueue[i];
                var data = new byte[move.length];
                Array.Copy(newRomData, (int)move.pSource, data, 0, move.length);

                moveData.Add(data);
            }

            // Zero out extracted data
            for (int i = 0; i < moveQueue.Count; i++) {
                var move = moveQueue[i];
                int offset = (int)move.pSource;
                for (int byteIndex = 0; byteIndex < move.length; byteIndex++) {
                    newRomData[offset] = 0;
                    offset++;
                }
            }

            // Copy data to new location
            for (int i = 0; i < moveQueue.Count; i++) {
                DoSingleMove(i);
            }

            ////while (moveQueue.Count > 0) {
            ////    var move = moveQueue[0];
            ////    moveQueue.RemoveAt(0);


            ////    ////if (CheckForOverwrite(move)) {
            ////    ////    moveQueue.Add(move); // If this move overwrites another, put it at the end of the line (so other data can be moved out of its way)
            ////    ////} else {
            ////        DoSingleMove(move);
            ////    ////}
            ////}
        }



        private void UpdatePointers(int moveIndex) {
            var move = moveQueue[moveIndex];

            // Update pointers
            int offsetDiff = move.pDest - move.pSource;
            for (int iPointer = 0; iPointer < move.ptableCount; iPointer++) {
                var pointerOffset = move.pPTable + 2 * iPointer;
                var pointerValue = new pCpu(newRomData, (int)pointerOffset);
                pointerValue += offsetDiff;
                pointerValue.Write(newRomData, pointerOffset);
            }
        }

        private void DoSingleMove(int moveIndex) {
            // Apply data
            var move = moveQueue[moveIndex];
            var data = moveData[moveIndex];
            Array.Copy(data, 0, newRomData, (int)move.pDest, move.length);
        }


        #endregion

        #region Data move queueing
        void CreateQueue() {
            FindData();
            SortDataBySource();
            CalculateLengths();
            QueueFirstSegment();
            QueueSecondSegment();
            QueueItemMove();
        }

        private void QueueItemMove() {
            pCpu endOfItemData;
            switch (GetItemDataStore()) {
                case LevelDataStore.ExpandoStore:
                    endOfItemData = Level.EndOfExpandoStore;
                    break;
                case LevelDataStore.StandardStore:
                    endOfItemData = Level.EndOfStandardStore;
                    break;
                case LevelDataStore.None:
                default:
                    throw new ArgumentException("GetItemDataStore returned an invalid value.");
            }

            var newItemDataOffset = endOfItemData.AsPRom(level) - itemData.length;
            itemData.pDest = newItemDataOffset;
            moveQueue.Add(itemData);
        }

        void QueueMove(DataSegment move, ref pRom dest, ref int bytesUsed) {
            move.pDest = dest;
            dest += move.length;
            bytesUsed += move.length;

            moveQueue.Add(move);
        }
        /// <summary>
        /// Finds the offset of each data segment and the location of the data's pointer(s).
        /// </summary>
        private void FindData() {
            // Combo data
            comboData.SetPointers(level, Expander.pointers.ComboData, 1);
            // Struct data / pointers
            FindPointersAndData(level, structPointers, structData, Expander.pointers.StructPTable, level.StructCount);
            // Room data / pointers
            FindPointersAndData(level, roomPointers, roomData, Expander.pointers.ScreenPTable, level.Screens.Count);
            // Item data
            itemData.SetPointers(level, Expander.pointers.ItemData, 1);
            itemData.pPTable = Expander.pointers.ItemData.AsPRom(level);
            itemData.ptableCount = 1;
            itemData.pSource = GetOffsetOfFirstItemEntry(level); // Pointer to item data may not actually point to first entry
            // PalData
            palData.SetPointers(level, Expander.pointers.PalData, Expander.pointers.PalPointerCount);
            palData.pSource = GetOffsetOfFirstPalEntry(level); // First pal pointer may not actually point to first entry
            // Frame data
            FindPointersAndData(level, framePointers, frameData, Expander.pointers.FramePTables);
            framePointers.ptableCount = 2;
            // Placement data
            FindPointersAndData(level, placementPointers, placementData, Expander.pointers.PlacementPTables);

            frameData.ptableCount = (placementPointers.pSource - framePointers.pSource) / 2;
            placementData.ptableCount = (placementData.pSource - placementPointers.pSource) / 2;
        }
        /// <summary>
        /// Finds the offset of a pointer table and its associated data, given the offset of the game's pointer to the pointer table.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pointers"></param>
        /// <param name="data"></param>
        /// <param name="pointerCount">Specifies the number of pointers in the pointer table. This is used to init pointers.length and data.pointerCount.</param>
        /// <param name="pTablePointer"></param>
        void FindPointersAndData(Level level, DataSegment pointers, DataSegment data, pCpu pTablePointer) { //, int pointerCount) {
            pointers.pPTable = pTablePointer.AsPRom(level);
            pointers.ptableCount = 1;
            //pointers.length = pointerCount * 2;
            pointers.pSource = level.DerefHandle(pointers.pPTable);

            data.pPTable = pointers.pSource;
            //data.ptableCount = pointerCount;
            data.pSource = level.DerefHandle(data.pPTable);
        }
        /// <summary>
        /// Finds the offset of a pointer table and its associated data, given the offset of the game's pointer to the pointer table.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pointers"></param>
        /// <param name="data"></param>
        /// <param name="pointerCount">Specifies the number of pointers in the pointer table. This is used to init pointers.length and data.pointerCount.</param>
        /// <param name="pTablePointer"></param>
        void FindPointersAndData(Level level, DataSegment pointers, DataSegment data, pCpu pTablePointer, int pointerCount) {
            pointers.pPTable = pTablePointer.AsPRom(level);
            pointers.ptableCount = 1;
            pointers.length = pointerCount * 2;
            pointers.pSource = level.DerefHandle(pointers.pPTable);

            data.pPTable = pointers.pSource;
            data.ptableCount = pointerCount;
            data.pSource = level.DerefHandle(data.pPTable);
        }
        /// <summary>
        /// Calculates the length of each data segment. sourceOrderedSegments
        /// must be sorted.
        /// </summary>
        private void CalculateLengths() {
            comboData.length = 0x100; // THIS SHOULD BE LAST
            for (int i = 0; i < sourceOrderedSegments.Count - 1; i++) { // -1 to skip combo data
                var item = sourceOrderedSegments[i];
                item.length = sourceOrderedSegments[i + 1].pSource - sourceOrderedSegments[i].pSource;
            }
        }

        private void QueueFirstSegment() {
            var moveList = GetFirstSegmentMoves();
            pRom dest = GetFirstSegmentOffset();
            QueueDataSegment(moveList, dest);
        }
        private void QueueSecondSegment() {
            var moveList = GetSecondSegmentMoves();
            pRom dest = GetSecondSegmentOffset();
            QueueDataSegment(moveList, dest);
        }
        private void QueueDataSegment(IList<levelDataTypes> moveList, pRom dest) {
            int bytesUsed = 0;

            for (int i = 0; i < moveList.Count; i++) {
                var move = moveList[i];
                var datasegment = dataSegments[(int)move];

                QueueMove(datasegment, ref dest, ref bytesUsed);
            }
        }

        protected abstract IList<levelDataTypes> GetFirstSegmentMoves();
        protected abstract IList<levelDataTypes> GetSecondSegmentMoves();
        protected virtual pRom GetFirstSegmentOffset() {
            return Expander.pointers.FreeMem.AsPRom(level);
        }
        protected virtual pRom GetSecondSegmentOffset() {
            return sourceOrderedSegments[0].pSource;
        }

        private pRom GetOffsetOfFirstPalEntry(Level level) {
            pCpu pFirst = (pCpu)0xFFFF;

            for (int i = 0; i < Expander.pointers.PalPointerCount; i++) {
                var pData = level.PalettePointers[i];
                if (pData < pFirst)
                    pFirst = pData;
            }

            return pFirst.AsPRom(level);
        }
        private pRom GetOffsetOfFirstItemEntry(Level level) {
            return level.ItemTable_DEPRECATED.GetFirstRowOffset();
        }

        protected virtual LevelDataStore GetItemDataStore ()  { return LevelDataStore.ExpandoStore; } 


        /// <summary>
        /// Sorts sourceOrderedSegments
        /// </summary>
        private void SortDataBySource() {
            sourceOrderedSegments.Sort(Sorter_BySourceOffset);
        }
        /// <summary>
        /// Function to sort DataSegments by their offset.
        /// </summary>
        int Sorter_BySourceOffset(DataSegment a, DataSegment b) {
            return a.pSource - b.pSource;
        }
        #endregion

        protected class DataSegment
        {
            public DataSegment() { }

            public pRom pSource, pDest;
            public int length;

            public pRom pPTable;
            public int ptableCount;

            public void SetPointers(Level level, pCpu ppPTable, int pointerCount) {
                this.pPTable = ppPTable.AsPRom(level);
                this.ptableCount = pointerCount;
                this.pSource = level.DerefHandle(ppPTable);
            }

            internal void CalculateLength(DataSegment followingData) {
                length = followingData.pSource - pSource;
            }

            public int DataMoveDist { get { return pDest - pSource; } }
        }

        public enum levelDataTypes
        {
            FrameData,
            FramePTable,
            PlacementData,
            PlacementPTable,
            PalData,
            ScreenPTable,
            ScreenData,
            StructPTable,
            StructData,
            ItemData,
            ComboData
        }

    }

    class BrinstarDataMoves : LevelDataMoves
    {

        public BrinstarDataMoves(Expander e, Level l) : base(e, l) { }

        protected override IList<levelDataTypes> GetFirstSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.FrameData,
                levelDataTypes.PalData,
                //levelDataTypes.ItemData,
                levelDataTypes.ComboData,
                levelDataTypes.StructPTable,
                levelDataTypes.StructData,
            };
        }

        protected override IList<levelDataTypes> GetSecondSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.ScreenPTable,
                levelDataTypes.ScreenData,
            };
        }

        protected override LevelDataStore GetItemDataStore() {
            return LevelDataStore.ExpandoStore;
        }

        protected override pRom GetSecondSegmentOffset() {
            return frameData.pSource; // Done: return offset of first data segment, override with this behavior for only brinstar 
            // Todo: add a fucking comment explaining why Brinstar is different
        }
    }

    class KraidDataMoves : LevelDataMoves
    {
        public KraidDataMoves(Expander e, Level l) : base(e, l) { }

        protected override IList<LevelDataMoves.levelDataTypes> GetFirstSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.FramePTable,
                levelDataTypes.PlacementPTable,
                levelDataTypes.PlacementData,
                levelDataTypes.FrameData,
                levelDataTypes.PalData,
                levelDataTypes.StructPTable,
                levelDataTypes.StructData,
            };

        }

        protected override IList<LevelDataMoves.levelDataTypes> GetSecondSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.ComboData,
                levelDataTypes.ScreenPTable,
                levelDataTypes.ScreenData,
                //levelDataTypes.ItemData,
            };
        }
        protected override LevelDataStore GetItemDataStore() {
            return LevelDataStore.StandardStore;
        }
    }

    class NorfairDataMoves : LevelDataMoves
    {
        public NorfairDataMoves(Expander e, Level l) : base(e, l) { }



        protected override IList<LevelDataMoves.levelDataTypes> GetFirstSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.FramePTable,
                levelDataTypes.PlacementPTable,
                levelDataTypes.PlacementData,
                levelDataTypes.FrameData,
                levelDataTypes.StructPTable,
                levelDataTypes.StructData,
            };

        }

        protected override IList<LevelDataMoves.levelDataTypes> GetSecondSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.PalData,
                levelDataTypes.ComboData,
                levelDataTypes.ScreenPTable,
                levelDataTypes.ScreenData,
                //levelDataTypes.ItemData,
            };
        }

        protected override LevelDataStore GetItemDataStore() {
            return LevelDataStore.StandardStore;
        }
    }
    class RidleyDataMoves : LevelDataMoves
    {
        public RidleyDataMoves(Expander e, Level l) : base(e, l) { }



        protected override IList<LevelDataMoves.levelDataTypes> GetFirstSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.FramePTable,
                levelDataTypes.PlacementPTable,
                levelDataTypes.PlacementData,
                levelDataTypes.FrameData,
                levelDataTypes.PalData,
                levelDataTypes.StructPTable,
                levelDataTypes.StructData,
                //levelDataTypes.ItemData,
          };

        }

        protected override IList<LevelDataMoves.levelDataTypes> GetSecondSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.ComboData,
                levelDataTypes.ScreenPTable,
                levelDataTypes.ScreenData,
            };
        }

        protected override LevelDataStore GetItemDataStore() {
            return LevelDataStore.ExpandoStore;
        }
    }
    class TourianDataMoves : LevelDataMoves
    {
        public TourianDataMoves(Expander e, Level l) : base(e, l) { }



        protected override IList<LevelDataMoves.levelDataTypes> GetFirstSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.FramePTable,
                levelDataTypes.PlacementPTable,
                levelDataTypes.PlacementData,
                levelDataTypes.FrameData,
                levelDataTypes.PalData,
                levelDataTypes.ComboData,
                //levelDataTypes.ItemData,

                levelDataTypes.StructPTable,
                levelDataTypes.StructData,
            };

        }

        protected override IList<LevelDataMoves.levelDataTypes> GetSecondSegmentMoves() {
            return new levelDataTypes[]{
                levelDataTypes.ScreenPTable,
                levelDataTypes.ScreenData,
            };
        }
        protected override LevelDataStore GetItemDataStore() {
            return LevelDataStore.StandardStore;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using snarfblasm;

namespace Editroid.ROM.Enhanced
{
    /// <summary>
    /// Further Expands an Expando ROM
    /// </summary>
    class Enhancer
    {
        const int ExpandoSize = 0x40010;
        const int EnhacedSize = 0x60010;
        const int ChrRomOffset = 0x40010;
        const int ChrBankSize = 0x1000;

        static readonly pCpu pNewItemData = new pCpu(0xAFE0);
        static readonly pCpu pNewAltPal = new pCpu(0xAFBC);

        public MetroidRom Rom { get; private set; }
        private byte[] romData;
        private byte[] enhancedRomData;

        public Enhancer(MetroidRom rom) {
            this.Rom = rom;
            this.romData = rom.data;

            if (rom.data.Length != ExpandoSize) throw new ArgumentException("The specified ROM is not a proper Expando ROM (the size is not correct.");
        }

        public byte[] CreateEnhancedRom() {
            PerformMmcExpand();
            PerformTileCopy();
            PerformAsmPatches();

            RelocateRoomAndItemData();
            RelocateAndExpandItemMusicRooms();
            RelocateAndExpandAltPalettes();

            return enhancedRomData;
        }

        private void RelocateAndExpandAltPalettes() {
            RelocateLevelsAltPalette(Rom.Brinstar);
            RelocateLevelsAltPalette(Rom.Norfair);
            RelocateLevelsAltPalette(Rom.Ridley);
            RelocateLevelsAltPalette(Rom.Kraid);
            RelocateLevelsAltPalette(Rom.Tourian);
        }

        private void RelocateLevelsAltPalette(Level level) {
            byte[] palData = new byte[0x20];

            var pPal0 = level.PalettePointers[0];
            var pPal5 = level.PalettePointers[5];

            var oPal0 = level.Bank.ToOffset(pPal0);
            var oPal5 = level.Bank.ToOffset(pPal5);

            // We initialize the alternate palette with data from the normal palette
            // because the alt pal effectively falls back to normal palette values for
            // palette enttries not specified in alt pal.
            ApplyPalData(palData, oPal0);
            // Then we apply any alt pal data.
            ApplyPalData(palData, oPal5);

            var oNewPalData = level.Bank.ToOffset(pNewAltPal);

            // PPU dest = $3F00
            enhancedRomData[oNewPalData] = 0x3F;
            oNewPalData++;
            enhancedRomData[oNewPalData] = 0x00;
            oNewPalData++;

            // Data length = $20
            enhancedRomData[oNewPalData] = 0x20;
            oNewPalData++;

            // Actual pal data
            for (int i = 0; i < palData.Length; i++) {
                enhancedRomData[oNewPalData] = palData[i];
                oNewPalData++;
            }

            // 00 - PPU string list terminator
            enhancedRomData[oNewPalData] = 0x00;
            oNewPalData++;

            // Pointer to the newly created palette data
            var ppAltPal = new pCpu(0x956A); // Address of alt pal pointer
            var opAltPal = level.Bank.ToOffset(ppAltPal); // Offset of pointer
            pNewAltPal.Write(enhancedRomData, opAltPal);
        }

        private void ApplyPalData(byte[] palData, pRom ppuStringOffset) {
            var offset = (int)ppuStringOffset;
            int processedStringCount = 0;
            
            // More than two ppu strings is much more likely to be invalid data than 
            // palette data. 
            // A 00 byte indicates end of data
            while (processedStringCount < 2 && romData[offset] != 0x00) {
                int ppuDest = (romData[offset] << 8) | romData[offset + 1];
                offset+=2;
                int stringLen = romData[offset];
                offset++;

                for (int i = 0; i < stringLen; i++) {
                    // PPU writes to 3F00-3F1F apply to palette
                    int palDataIndex = ppuDest - 0x3F00 + i;
                    if (palDataIndex >= 0 && palDataIndex <= palData.Length) {
                        palData[palDataIndex] = romData[offset];
                    }

                    offset++;
                }

                processedStringCount++;
            }
        }


        private void RelocateAndExpandItemMusicRooms() {
            RelocateLevelsItemMusicRooms(Rom.Brinstar);
            RelocateLevelsItemMusicRooms(Rom.Norfair);
            RelocateLevelsItemMusicRooms(Rom.Ridley);
            RelocateLevelsItemMusicRooms(Rom.Tourian);
            RelocateLevelsItemMusicRooms(Rom.Kraid);
        }

        private void RelocateLevelsItemMusicRooms(Level level) {
            const int newDataSize = 0x20;
            var pNewData = pNewItemData; // $20 bytes from $AFE0 - $AFFF
            int newOffset = level.Bank.ToOffset(pNewData);

            int oldDataSize =level.Format.AltMusicRoomCount;
            int oldOffset = level.Format.AltMusicOffset;

            var pDataPointer = level.Format.pPointerToItemMusicRoomList;
            var oDataPointer = level.Rom.FixedBank.ToOffset(pDataPointer);

            // Clear new location
            for (int i = 0; i < newDataSize; i++) {
                enhancedRomData[newOffset + i] = 0xFF; // "unused" value
            }

            // Copy old values
            for (int i = 0; i < oldDataSize; i++) {
                enhancedRomData[newOffset + i] = romData[oldOffset + i];
            }

            // Update pointer
            ////int diff = newOffset - oldOffset;
            ////var pData = new pCpu(enhancedRomData, oDataPointer);
            ////pData.Value += diff;
            ////pData.Write(enhancedRomData, oDataPointer);
            pNewData.Write(enhancedRomData, oDataPointer);

            // Update count
            var pCount = level.Format.pItemMusicRoomCount;
            var oCount = level.Rom.FixedBank.ToOffset(pCount);
            enhancedRomData[oCount] = newDataSize;
        }

        private void RelocateRoomAndItemData() {
            CopyRoomItemData(Rom.Brinstar, 8);
            CopyRoomItemData(Rom.Norfair, 9);
            CopyRoomItemData(Rom.Kraid, 11);
            CopyRoomItemData(Rom.Tourian, 10);
            CopyRoomItemData(Rom.Ridley, 12);
            PreformRoomAsmPatch();

        }

        private void PreformRoomAsmPatch() {
            var labels = new AddressLabels();
            var Asm = EnhancerResources.RoomDataHack;
            var assembler = new Assembler("RoomDataHack", Asm, StandardFileSystem.FileSystem) {
                Labels = labels
            };

            var bin = assembler.Assemble();
            var errors = assembler.GetErrors();

            if (errors.Count > 0) throw new EnhancerException("Room data hack ASM patch failed to assemble with " + errors.Count.ToString() + " errors. First error: " + errors[0].Message + " on line " + errors[0].LineNumber.ToString());

            var patches = assembler.GetPatchSegments();
            for (int i = 0; i < patches.Count; i++) {
                var patch = patches[i];

                Array.Copy(bin, patch.Start, enhancedRomData, patch.PatchOffset, patch.Length);
            }

        }

        private void CopyRoomItemData(Level level, int bank) {
            int bankBase = 0x10 + 0x4000 * bank; // Brinstar data will go in bank 8
            int offset = 0;

            // Item data offsets are calculated now because I'm afraid the calculation will get screwed up after data starts getting moved.
            // The data is moved later, because first we need to move the data that is where we want to put the item data.
            int srcItemData = level.ItemTable_DEPRECATED.GetFirstRowOffset();
            int lenItemData = level.ItemTable_DEPRECATED.GetTotalBytes();
            // Item data will be moved to beginning of "expando store", which starts with either combo data or struct ptable
            int destItemData = Math.Min(level.Combos.Offset, (int)level.Format.StructPtableOffset);
            // This is how much we need to add to the existing pointers to repoint them to new location.
            int diffItemDataLocation = destItemData - srcItemData;


            // Copy combo table (assume 100 combos)
            {
                var comboOffset = level.Combos.Offset;
                var comboLen = 0x100 * 0x4;
                Array.Copy(romData, comboOffset, enhancedRomData, bankBase + offset, comboLen);
                offset += comboLen;
            }

            // Copy structures, and pointer table
            {
                int structCount = level.StructCount;

                // Pointer table will be followed by struct data
                int oDestStructTable = offset;
                int oDestStructData = offset + structCount * 2;

                for (int i = 0; i < level.StructCount; i++) {
                    // Write pointer to the new data in the new ptr table
                    PointerTable.WriteTwoBytePointer(enhancedRomData, bankBase + oDestStructTable, i, new pCpu(0x8000 | oDestStructData));

                    // Copy the struct data, and update oDestStructData to point where next struct will go
                    var oSrcStruct = level.Structures[i].Offset;
                    var structSize = level.Structures[i].Size;
                    Array.Copy(romData, oSrcStruct, enhancedRomData, bankBase + oDestStructData, structSize);
                    oDestStructData += structSize;
                }

                // Update offset to point past struct data (room data will follow)
                offset = oDestStructData;
            }

            // Copy rooms, and update pointer talbe
            {
                int oRoomPtable = (int)level.Pointers_depracated.RoomTableOffset;

                for (int i = 0; i < level.Screens.Count; i++) {
                    // Update room ptr table
                    PointerTable.WriteTwoBytePointer(enhancedRomData, oRoomPtable, i, new pCpu(0x8000 | offset));

                    // Copy room data
                    int oRoom = level.Screens[i].Offset;
                    int roomSize = level.Screens[i].Size;
                    Array.Copy(romData, oRoom, enhancedRomData, bankBase + offset, roomSize);
                    offset += roomSize;
                }

                // Terminate room ptr table with 0xFFFF
                PointerTable.WriteTwoBytePointer(enhancedRomData, oRoomPtable, level.Screens.Count, new pCpu(0xFFFF));
            }

            // Copy item data
            {
                Array.Copy(romData, srcItemData, enhancedRomData, destItemData, lenItemData);

                // We need to update the linked-list pointers
                // We don't have the benifit of MetroidRom, Bank, etc. objects because all we have right now is a byte array.
                int bankOffset = level.Bank.Offset;
                var ppNextEntry = level.Format.pPointerToItemList;
                const int tooManyRows = 33; // If we hit this many rows, we know there's a problem with the data (otherwise we optimistically assume we got it right)
                int processedRows = 0;

                var pNextEntry = enhanced_GetPtr(bankOffset, ppNextEntry);
                while (pNextEntry.Value != 0xFFFF) {
                    processedRows++;
                    if (processedRows >= tooManyRows) throw new EnhancerException("An error occurred while relocating item data.");

                    // Correct pointer
                    pNextEntry += diffItemDataLocation;
                    // Write corrected pointer
                    enhanced_SetPtr(bankOffset, ppNextEntry, pNextEntry);
                    // Derference corrected pointer
                    ppNextEntry = pNextEntry + 1; // linked-list pointer is located at byte 1 in next record
                    pNextEntry = enhanced_GetPtr(bankOffset, ppNextEntry);
                }
            }
        }

        pCpu enhanced_GetPtr(int bankOffset, pCpu address) {
            int offset = bankOffset + (address.Value & 0x3FFF);
            return new pCpu(enhancedRomData, offset);
        }
        void enhanced_SetPtr(int bankOffset, pCpu location, pCpu value) {
            int offset = bankOffset + (location.Value & 0x3FFF);
            value.Write(enhancedRomData, offset);
        }

        private void PerformAsmPatches() {
           
            { // First try
                ////// Pre-assembled code
                ////var ppuLoaderAsm = EnhancerResources.PPU_Loader;
                ////var NmiHijackAsm = EnhancerResources.NMI_Hijack;

                ////// Apply ASM patches
                ////Array.Copy(ppuLoaderAsm, 0, enhancedRomData, PpuLoader_Offset, ppuLoaderAsm.Length);
                ////Array.Copy(NmiHijackAsm, 0, enhancedRomData, NmiHijack_Offset, NmiHijackAsm.Length);
                ////enhancedRomData[ChrSwapSize_Offset] |= ChrSwapMode_4k;

                ////// Patch pointers to updated routines
                ////UpdateMiscPointers();
            }
            { // Second try
                ////// ASM patches
                ////var ppuLoaderSource = EnhancerResources.PpuLoader_ASM;
                ////var assm = new Assembler("Ppu Loader", ppuLoaderSource, StandardFileSystem.FileSystem);
                ////var ppuLoaderAsm = assm.Assemble();

                ////var nmiHijockSource = EnhancerResources.NmiHijack_ASM_unused;
                ////assm = new Assembler("NMI Hijack", nmiHijockSource, StandardFileSystem.FileSystem);
                ////var NmiHijackAsm = assm.Assemble();
                
                ////// Patch pointers to updated routines
                ////UpdateMiscPointers();
            }
            var labels = new AddressLabels();

            var ppuLoaderAsm = EnhancerResources.PpuLoader_ASM;
            var assembler = new Assembler("PPU Loader", ppuLoaderAsm, StandardFileSystem.FileSystem);
            assembler.Labels = labels;
            var bin = assembler.Assemble();
            var errors = assembler.GetErrors();

            if (errors.Count > 0) throw new EnhancerException("PPU Loader ASM patch failed to assemble with " + errors.Count.ToString() + " errors. First error: " + errors[0].Message + " on line " + errors[0].LineNumber.ToString());

            var patches = assembler.GetPatchSegments();
            for (int i = 0; i < patches.Count; i++) {
                var patch = patches[i];

                Array.Copy(bin, patch.Start, enhancedRomData, patch.PatchOffset, patch.Length);
            }

            // Patch pointers to updated routines
            UpdateMiscPointers(labels);


            FixCreditsPatternUsage();
        }

        /// <summary>
        /// The tiles used for the ending/credits have been moved. This method corrects the 
        /// nametable PPU strings that contain the relevant tile-layout-data.
        /// </summary>
        private void FixCreditsPatternUsage() {
            int oGroundTles = offsetFromPointer(0, 0xA052);
            
            int oEndingTextTable = offsetFromPointer(0, 0xA1BA);
            int endingTextTableLength = 4;
            
            int oCreditsTable = offsetFromPointer(0, 0xA291);
            int CreditsTableLength = 0x2C;

            // Fix the tile layout data for the ground (and some credits)
            FixCreditsPpuString(oGroundTles);
            
            // Fix the tile layout data for ending text ("GREAT!! YOU FULFILED YOUR MISSON...")
            for (int i = 0; i < endingTextTableLength; i++) {
                int pString = PtrAtOffset(enhancedRomData, oEndingTextTable);
                int oString = offsetFromPointer(0, pString);

                FixCreditsPpuString(oString);
                oEndingTextTable += 2; // Next pointer
            }

            // Fix the tile layout data for credit text 
            for (int i = 0; i < CreditsTableLength; i++) {
                int pString = PtrAtOffset(enhancedRomData, oCreditsTable);
                int oString = offsetFromPointer(0, pString);

                FixCreditsPpuString(oString);
                oCreditsTable += 2; // Next pointer
            }
        }

        /// <summary>
        /// Corrects tile layout data in a string of PPU strings.
        /// </summary>
        private void FixCreditsPpuString(int offset) {
            byte dataByte = enhancedRomData[offset];

            // The series of PPU strings is terminated with a $00
            while (dataByte != 0x00) {
                // Seek past VRAM pointer
                offset += 2;

                // Get length of string
                int length = enhancedRomData[offset];
                offset++;

                // Some strings may be RLE encoded. This means there is only one tile-index byte to process.
                bool RLE = ((length & 0x40) != 00);
                if (RLE) length = 1;

                // test to make sure we aren't following bad pointers and corrupting rom
                    if (offset < 0x0260 || (offset + length) >= 0x254E) {
                        throw new EnhancerException("Bad pointer in credit text pointers (target is outside allowed range).");
                    }

                // Pass over string, correcting any problematic tile indecies
                for (int i = 0; i < length; i++) {
                    // Get tile index
                    dataByte = enhancedRomData[offset];

                    // Affected tiles are those from $00 to $3F, which have been moved to $90 - $CF
                    if (dataByte < 0x40) {
                        enhancedRomData[offset] = (byte)(dataByte + 0x90);
                    }

                    offset++;
                }

                // Grab next byte, in case it is the $00 terminator.
                dataByte = enhancedRomData[offset];
            }
        }

        int PtrAtOffset(byte[] rom, int offset) {
            return rom[offset] + (rom[offset + 1] << 8);
        }

        int offsetFromPointer(int bank, int ptr) {
            return bank * 0x4000 + (ptr & 0x3fff) + 0x10;
        }

        /// <summary>
        /// Updates pointers to the newly patched PPU loader.
        /// </summary>
        private void UpdateMiscPointers(AddressLabels labels) {
            ////int InitTitleGFX = 0xC603;
            ////Prg_WriteWord(0xC54D, InitTitleGFX); // Update jsr InitTitleGFX
            ////Prg_WriteWord(0xC573, 0xC608); // Update jsr InitBrinstarGFX
            ////Prg_WriteWord(0xC58B, 0xC60d); // Update jsr InitNorfairGFX
            ////Prg_WriteWord(0xC5A3, 0xC612); // Update jsr InitTourianGFX
            ////Prg_WriteWord(0xC5BE, 0xC617); // Update jsr InitKraidGFX
            ////Prg_WriteWord(0xC5CB, 0xC61C); // Update jsr InitRidleyGFX
            ////Prg_WriteWord(0xC5D5, InitTitleGFX); // Update jsr InitGFX6

            // Todo: with support for .PATCH, this can actually be done via the ASM
            WriteLabel(0xC54D, labels, "InitTitleGFX");
            WriteLabel(0xC573, labels, "InitBrinstarGFX"); // Update jsr InitBrinstarGFX
            WriteLabel(0xC58B, labels, "InitNorfairGFX"); // Update jsr InitNorfairGFX
            WriteLabel(0xC5A3, labels, "InitTourianGFX"); // Update jsr InitTourianGFX
            WriteLabel(0xC5BE, labels, "InitKraidGFX"); // Update jsr InitKraidGFX
            WriteLabel(0xC5CB, labels, "InitRidleyGFX"); // Update jsr InitRidleyGFX
            WriteLabel(0xC54D, labels, "InitTitleGFX");

            ////// Update jsr InitGFX7 (called 3 times in title page bank: title, password, end of game)
            ////enhancedRomData[0x1135] = (byte)(InitTitleGFX & 0xFF);
            ////enhancedRomData[0x1136] = (byte)(InitTitleGFX >> 8);

            ////enhancedRomData[0x1378] = (byte)(InitTitleGFX & 0xFF);
            ////enhancedRomData[0x1379] = (byte)(InitTitleGFX >> 8);

            ////enhancedRomData[0x13B9] = (byte)(InitTitleGFX & 0xFF);
            ////enhancedRomData[0x13BA] = (byte)(InitTitleGFX >> 8);

            WriteLabel(0, 0x9125, labels, "InitTitleGFX");
            WriteLabel(0, 0x9368, labels, "InitTitleGFX");
            WriteLabel(0, 0x93A9, labels, "InitTitleGFX");
        }

        /////// <summary>
        /////// Updates pointers to the newly patched PPU loader.
        /////// </summary>
        ////private void UpdateMiscPointers() { // Done: comment out this unneeded method
        ////    int InitTitleGFX = 0xC603;
        ////    Prg_WriteWord(0xC54D, InitTitleGFX); // Update jsr InitTitleGFX
        ////    Prg_WriteWord(0xC573, 0xC608); // Update jsr InitBrinstarGFX
        ////    Prg_WriteWord(0xC58B, 0xC60d); // Update jsr InitNorfairGFX
        ////    Prg_WriteWord(0xC5A3, 0xC612); // Update jsr InitTourianGFX
        ////    Prg_WriteWord(0xC5BE, 0xC617); // Update jsr InitKraidGFX
        ////    Prg_WriteWord(0xC5CB, 0xC61C); // Update jsr InitRidleyGFX
        ////    Prg_WriteWord(0xC5D5, InitTitleGFX); // Update jsr InitGFX6

        ////    // Update jsr InitGFX7 (called 3 times in title page bank: title, password, end of game)
        ////    enhancedRomData[0x1135] = (byte)(InitTitleGFX & 0xFF);
        ////    enhancedRomData[0x1136] = (byte)(InitTitleGFX >> 8);

        ////    enhancedRomData[0x1378] = (byte)(InitTitleGFX & 0xFF);
        ////    enhancedRomData[0x1379] = (byte)(InitTitleGFX >> 8);

        ////    enhancedRomData[0x13B9] = (byte)(InitTitleGFX & 0xFF);
        ////    enhancedRomData[0x13BA] = (byte)(InitTitleGFX >> 8);
        ////}
        /// <summary>Writes a pointer to the specified address in the last bank.</summary>
        private void WriteLabel(int ptrAddress, AddressLabels labels, string labelName) {
            WriteLabel(0xF, ptrAddress, labels, labelName);
        }
        /// <summary>Writes a pointer to the specified address in the specified bank.</summary>
        private void WriteLabel(int bank, int ptrAddress, AddressLabels labels, string labelName) {
            const int ptrFilter = 0x3FFF;

            ushort? labelValue = TryGetLabelValue(labels, labelName);
            int bankOffset = 0x10 + 0x4000 * bank;

            if (labelValue == null) {
                throw new EnhancerException("The label \"" + labelName + "\" was not found in the applied ASM patch.");
            } else {
                ptrAddress = (ptrAddress & ptrFilter) + bankOffset;
                enhancedRomData[ptrAddress] = (byte)(labelValue.Value);
                enhancedRomData[ptrAddress + 1] = (byte)(labelValue.Value >> 8);
            }
        }

        private ushort? TryGetLabelValue(AddressLabels labels, string labelName) {
            const int fixedBank = 0x0F;
            var bank = (BankLabels)labels.Banks[fixedBank];
            foreach (var label in bank.Labels) {
                // Key is address
                // Value contains name
                if (label.Value.labelName.Equals(labelName, StringComparison.Ordinal)) {
                    return label.Key;
                }
            }

            return null;
        }

        /////// <summary>
        /////// Writes a 2-byte word to the fixed PRG ROM found at $3C010
        /////// </summary>
        ////private void Prg_WriteWord(int prgOffset, int data) { // Done: comment out this unneeded method
        ////    const int RomOffset = 0x3C010;
        ////    const int RamOrigin = 0xC000;
        ////    // lsB
        ////    enhancedRomData[prgOffset + RomOffset - RamOrigin] = (byte)(data & 0xFF);
        ////    enhancedRomData[prgOffset + RomOffset - RamOrigin + 1] = (byte)((data >> 8) & 0xFF);
        ////}

        /// <summary>Creates a new ROM image with empty CHR ROM.</summary>
        private void PerformMmcExpand() {
            enhancedRomData = new byte[EnhacedSize];
            Array.Copy(romData, enhancedRomData, romData.Length);

            // Update header to identify new ROM space as CHR ROM
            Romulus.Nes.NesHeader header = new Romulus.Nes.NesHeader(enhancedRomData, 0);
            header.ChrRomCount = 0x10; // 128k (32 x 4k banks)
        }

        private void PerformTileCopy() {
            ChrBankBuilder builder = new ChrBankBuilder(enhancedRomData);
            builder.SetOffset(0x40010);

            // skipping title banks for now
            builder.NextBank();
            builder.NextBank();

            ApplyLevelPatterns(builder, LevelIndex.Brinstar);
            ApplyLevelPatterns(builder, LevelIndex.Norfair);
            ApplyLevelPatterns(builder, LevelIndex.Tourian);
            ApplyLevelPatterns(builder, LevelIndex.Kraid);
            ApplyLevelPatterns(builder, LevelIndex.Ridley);

            CreateTitleAndEndingCompositeBanks();
        }

        private void CreateTitleAndEndingCompositeBanks() {
            // Some tiles from the ending need to be moved within the bank because
            // they conflict with title screen graphics.
            var endingBgTiles = ExpandoPatternOffsets.TheEndScriptTiles;
            endingBgTiles.DestTileindex = 0x90;

            // The cursive "THE END" tiles need to be relocated.
            var theendScriptText = ExpandoPatternOffsets.TheEndScriptTiles;
            theendScriptText.DestTileindex = 0x90;

            var bgPatternGroups = new[] {
                ExpandoPatternOffsets.TitleTextTiles,
                ExpandoPatternOffsets.TitleBgGraphics,
                theendScriptText,
            };
            var spritePatternGroups = new[] {
                ExpandoPatternOffsets.EndingSamusSprites,
                ExpandoPatternOffsets.DigitSprites,
                ExpandoPatternOffsets.TitleSpriteGraphics,
                ExpandoPatternOffsets.HudEnergyTank,
            };
            
            int TitleBgBankOffset = ChrRomOffset;
            int TitleSpriteBankOffset = ChrRomOffset + ChrBankSize;

            CreateCompositeBank(TitleBgBankOffset, bgPatternGroups);
            CreateCompositeBank(TitleSpriteBankOffset, spritePatternGroups);
        }

        private void CreateCompositeBank(int bankOffset, ExpandoPatternOffsets.Entry[] patternGroups) {
            for (int i = 0; i < patternGroups.Length; i++) {
                var group = patternGroups[i];

                int sourceOffset = group.RomOffset;
                int destOffset = bankOffset + group.DestTileindex * 0x10;
                int byteCount = group.TileCount * 0x10;

                Array.Copy(enhancedRomData, sourceOffset, enhancedRomData, destOffset, byteCount);
            }
        }

        private void ApplyLevelPatterns(ChrBankBuilder builder, LevelIndex level) {
            // Level sprites
            ApplyLevelSpritePatterns(builder, level, false);
            builder.NextBank();
            // Level sprites (Justin Bailey)
            ApplyLevelSpritePatterns(builder, level, true);
            builder.NextBank();
            // Level BG (copied four times)
            for (int i = 0; i < 4; i++) {
                ApplyPatternsToBank(builder, ExpandoPatternOffsets.GetBackgroundEntry(level));
                builder.NextBank();
            }
        }

        private void ApplyLevelSpritePatterns(ChrBankBuilder builder, LevelIndex levelIndex, bool justinBailey) {
            ApplyPatternsToBank(builder, ExpandoPatternOffsets.GlobalGameplaySprites);
            ApplyPatternsToBank(builder, ExpandoPatternOffsets.DigitSprites);
            if (justinBailey)
                ApplyPatternsToBank(builder, ExpandoPatternOffsets.JustinBaileySprites);
            ApplyPatternsToBank(builder, ExpandoPatternOffsets.GetSpriteEntry(levelIndex));
        }

        private void ApplyPatternsToBank(ChrBankBuilder builder, ExpandoPatternOffsets.Entry entry) {
            builder.CopyTiles(entry.RomOffset, entry.DestTileindex, entry.TileCount);
        }

        // CHR Bank allocation
        // 00 - Title/End/Credit background
        // 01 - Title/End/Credit spr
        // 02 - Brinstar Sprites
        // 03 - Brinstar Sprites (Justin Bailey)
        // 04 - Brinstar Bg 0
        // 05 - Brinstar Bg 1
        // 06 - Brinstar Bg 2
        // 07 - Brinstar Bg 3
        // 08 - Norfair Sprites
        // 09 - Norfair Sprites (Justin Bailey)
        // 0a - Norfair Bg 0
        // 0b - Norfair Bg 1
        // 0c - Norfair Bg 2
        // 0d - Norfair Bg 3
        // 0e - Tourian Sprites
        // 0f - Tourian Sprites (Justin Bailey)
        // 10 - Tourian Bg 0
        // 11 - Tourian Bg 1
        // 12 - Tourian Bg 2
        // 13 - Tourian Bg 3
        // 14 - Kraid Sprites
        // 15 - Kraid Sprites (Justin Bailey)
        // 16 - Kraid Bg 0
        // 17 - Kraid Bg 1
        // 18 - Kraid Bg 2
        // 19 - Kraid Bg 3
        // 1a - Ridley Sprites
        // 1b - Ridley Sprites (Justin Bailey)
        // 1c - Ridley Bg 0
        // 1d - Ridley Bg 1
        // 1e - Ridley Bg 2
        // 1f - Ridley Bg 3

        class ChrBankBuilder{
            byte[] romData;
            int bankOffset;

            public ChrBankBuilder(byte[] romData) {
                this.romData = romData;
            }

            public void SetOffset(int offset) {
                this.bankOffset = offset;
            }

            public void NextBank() {
                bankOffset += 0x1000;
            }

            public void CopyTiles(int srcOffset, int destIndex, int tileCount) {
                int destOffset = bankOffset + destIndex * 0x10;
                int byteCount = tileCount * 0x10;

                Array.Copy(romData, srcOffset, romData, destOffset, byteCount);
            }
        }
    }
}

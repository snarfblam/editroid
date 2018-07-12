using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Editroid.ROM;
using Editroid.Graphic;
using Editroid.ROM.Projects;

namespace Editroid
{
	/// <summary>
	/// Provides an object-oriented and organized mechanism for accessing raw ROM data
	/// </summary>
    public class MetroidRom : IRomDataParentObject
	{
		/// <summary>
		/// The raw data of the ROM
		/// </summary>
		public byte[] data;

        public const int RomSize = 0x20010;
        public const int RomWithMapSize = 0x20010 + MapControl.MapDataBufferSize;
        public const int ExpandedRomSize = 0x40010;
        public const int EnhancedRomSize = 0x60010;
        public const int MMC3RomSize = 0xC0010;

        internal Editroid.ROM.Formats.RomFormat Format { get; private set; }


		/// <summary>
		/// Creates a Editroid.Rom from a stream
		/// </summary>
		/// <param name="s">T stream that contains a Metroid ROM image</param>
		public MetroidRom(Stream s) {
			try {
                // Todo: why are we truncating (overdump from) the ROM here?

                if (s.Length >= MMC3RomSize) {
                    data = new byte[MMC3RomSize];
                }else if (s.Length >= EnhancedRomSize) {
                    data = new byte[EnhancedRomSize];
                } else if (s.Length >= ExpandedRomSize) {
                    data = new byte[ExpandedRomSize];
                } else {
                    data = new byte[RomWithMapSize];
                }
				s.Read(data, 0, data.Length);

			}
			catch(Exception Ex) {
				MessageBox.Show("Caught " + Ex.GetType().ToString() + " - " + Ex.Message);
				throw;
			}


            if (data.Length < ExpandedRomSize) {
                RomFormat = RomFormats.Standard;
                Format = new Editroid.ROM.Formats.StandardRomFormat(this);
            } else if (data.Length < EnhancedRomSize) {
                RomFormat = RomFormats.Expando;
                Format = new Editroid.ROM.Formats.ExpandoRomFormat(this);
            } else if (data.Length < MMC3RomSize) {
                RomFormat = RomFormats.Enhanco;
                Format = new Editroid.ROM.Formats.EnhancedRomFormat(this);
            } else {
                RomFormat = RomFormats.MMC3;
                Format = new Editroid.ROM.Formats.Mmc3RomFormat(this);
            }

            Banks = Format.Banks;
            FixedBank = Banks[Banks.Count - 1];
            ChrUsage = new ChrUsageTable(this);
            if (Format.HasPrgAllocationTable) {
                _bankAllocation = Editroid.ROM.BankAllocation.ReadAllocation(data, Editroid.ROM.BankAllocation.BankAllocationOffset);
            }

             if (Format.HasChrAnimationTable) {
                    LoadChrAnimationTable();
             }

            patternGroupOffsets = new PatternGroupOffsetTable(this);
            globalPatternGroups = new PatternGroupIndexTable(this);
            //MakeGameItems();
            passwordData = new PasswordData(this);
            titleText = new TitleText(this);
            LoadLevels();
            LoadEditorData();

            GetDoorTileCounts();

   
      

            CacheSavedVersion();
		}

        public ChrAnimationRomData ChrAnimationData{get; private set;}

        public void ReloadChrAnimationTable() {
            LoadChrAnimationTable();
        }
        private void LoadChrAnimationTable() {
            if (Format.HasChrAnimationTable) {
                this.ChrAnimationTableMissing = !HasChrAnimationIndicator();
                ChrAnimationData = ChrAnimation.DeserializeChrAnimation(this, null);

                // If the ROM does not have a CHR animation table, or we some how magically end up missing chr animation data for a level, default animation tables will be created

                foreach (LevelIndex level in new LevelIndex[] { LevelIndex.None, LevelIndex.Brinstar, LevelIndex.Norfair, LevelIndex.Tourian, LevelIndex.Kraid, LevelIndex.Ridley }) {
                    ChrAnimationLevelData levelData;
                    if (ChrAnimationTableMissing || !ChrAnimationData.TryGetValue(level, out levelData)) {
                        ChrAnimationData[level] = CreateDefaultAnimationsForLevel(level);
                    }
                }
            }
        }

        private ChrAnimationLevelData CreateDefaultAnimationsForLevel(LevelIndex level) {
            var defaultData = new ChrAnimationLevelData(level);
            var defaultTable = new ChrAnimationTable();
            var defaultFrame = new ChrAnimationFrame();

            defaultTable.Frames.Add(defaultFrame);
            defaultData.Animations.Add(defaultTable);
            //if (level == LevelIndex.None) {
            //    TitleChrAnimationTable = defaultData;
            //} else {
            //    Levels[level].ChrAnimation = defaultData;
            //}
            return defaultData;
        }

        private bool HasChrAnimationIndicator() {
            return
                ChrUsage.MMC3_GetUnusedByte(LevelIndex.None) == 0xFF &&
                ChrUsage.MMC3_GetUnusedByte(LevelIndex.Brinstar) == 0xFF &&
                ChrUsage.MMC3_GetUnusedByte(LevelIndex.Norfair) == 0xFF &&
                ChrUsage.MMC3_GetUnusedByte(LevelIndex.Tourian) == 0xFF &&
                ChrUsage.MMC3_GetUnusedByte(LevelIndex.Ridley) == 0xFF &&
                ChrUsage.MMC3_GetUnusedByte(LevelIndex.Kraid) == 0xFF;
        }

        private void SerializeChrAnimationTable() {
            int frameIndex = 0;

            SerializeLevelChrAnimation(ref frameIndex, LevelIndex.None);
            SerializeLevelChrAnimation(ref frameIndex, LevelIndex.Brinstar);
            SerializeLevelChrAnimation(ref frameIndex, LevelIndex.Norfair);
            SerializeLevelChrAnimation(ref frameIndex, LevelIndex.Tourian);
            SerializeLevelChrAnimation(ref frameIndex, LevelIndex.Kraid);
            SerializeLevelChrAnimation(ref frameIndex, LevelIndex.Ridley);
        }

        private void SerializeLevelChrAnimation(ref int frameIndex, LevelIndex index) {
            // LevelIndex.None = title screen
            ChrAnimationLevelData data = (index == LevelIndex.None) ? TitleChrAnimationTable : Levels[index].ChrAnimation;

            // Level's default animation will use next available frame entry
            ChrUsage.MMC3_SetBgLoopStart(index, (byte)frameIndex);
            // Write frame data to chr animation table
            ChrAnimation.SerializeChrAnimation(this, data, ref frameIndex);
        }

        /// <summary>
        /// Gets a comma-delimited list of CHR animation names that can be serialized in a project
        /// </summary>
        /// <returns></returns>
        public string GetChrAnimationNames() {
            List<string> names = new List<string>();
            GetChrAnimNames(TitleChrAnimationTable, names);
            GetChrAnimNames(Brinstar.ChrAnimation, names);
            GetChrAnimNames(Norfair.ChrAnimation, names);
            GetChrAnimNames(Tourian.ChrAnimation, names);
            GetChrAnimNames(Kraid.ChrAnimation, names);
            GetChrAnimNames(Ridley.ChrAnimation, names);
            return string.Join(",", names.ToArray());
        }

        /// <summary>
        /// Sets all chr animation names from a comma-delimited list of names
        /// </summary>
        /// <param name="allNames"></param>
        public void SetChrAnimationNames(string allNames) {
            if(string.IsNullOrEmpty(allNames)) return;

            // This routine assumes that names are in the proper order. If we run out of names, though, we'll fail silently.

            var names = allNames.Split(',');
            
            int iName = 0;

            for (int iAnim = 0; iAnim < TitleChrAnimationTable.Animations.Count; iAnim++) {
                TitleChrAnimationTable.Animations[iAnim].Name = names[iName];
                iName++;
                // Stop if we run out of names
                if (iName == names.Length) return;
            }

            for (int iLevel = 0; iLevel < Levels.Count; iLevel++) {
                var l = Levels[(LevelIndex)iLevel];
                for (int iAnim = 0; iAnim < l.ChrAnimation.Animations.Count; iAnim++) {
                    l.ChrAnimation.Animations[iAnim].Name = names[iName];
                    iName++;
                    // Stop if we run out of names
                    if (iName == names.Length) return;
                }
            }
        }

        private void GetChrAnimNames(ChrAnimationLevelData animations, List<string> names) {
            for (int iAnim = 0; iAnim < animations.Animations.Count; iAnim++) {
                var name = animations.Animations[iAnim].Name ?? string.Empty;
                // Commas will fuck shit up.
                names.Add(name.Replace(",", ""));
            }
        }

        internal void LoadAnimationNames(Project p) {
            // Todo: load animation names from project
        }

        private void LoadEditorData() {
            // Load an EditorData object from ROM, or create a 
            // new one if the ROM does not contain one.
            EditorData = EditorData.LoadEditorData(this)
                ?? EditorData.CreateNewForROM(this);
        }

        private void SaveEditorData() {
            if (RomFormat != RomFormats.Standard) {
                // EditorData is not saved on unexpanded ROMs
                EditorData.SaveEditorData();
            }
        }


        /// <summary>
        /// This method corrects a a problem in the original ROM (multiple screen pointers
        /// reference same data), and must be called after loading the ROM but before any edits are made.
        /// </summary>
        public void CorrectDoubledScreens(ILevelMap map) {
            for (int y = 0; y < 32; y++) {
                for (int x = 0; x < 32; x++) {
                    var lvl = map.GetLevel(x, y);
                    byte screenIndex = GetScreenIndex(x, y);

                    if (lvl != LevelIndex.None && screenIndex != 0xFF) {
                        levels[lvl].CorrectScreenIndex(ref screenIndex);
                        SetScreenIndex(x, y, screenIndex);
                    }
                }
            }

            // When the user closes the program, it looks like the ROM has been changed because we have modified
            // it to fix doubled screens, even though the user may not have edited the ROM.
            // To fix this, we "save" the unedited but de-glitched ROM in RAM to compare to for changes.
            SerializeAllData();
            CacheSavedVersion();
        }

        RoomSerializationPointers _ScreenWritePointers = new RoomSerializationPointers();
        internal RoomSerializationPointers ScreenWritePointers { get { return _ScreenWritePointers; } }

        public ChrUsageTable ChrUsage { get; private set; }


        public IList<Bank> Banks { get; private set; }
        public Bank FixedBank { get; private set; }
        public EditorData EditorData { get; private set; }

        /// <summary>
        /// Returns TRUE if the ROM file was expanded (this includes Expando and Enhanced).
        /// </summary>
        public bool ExpandedRom { get { return RomFormat != RomFormats.Standard; } }
        /// <summary>
        /// Gets the format of the ROM.
        /// </summary>
        public RomFormats RomFormat { get; private set; }

        BankAllocation[] _bankAllocation;
        public BankAllocation[] BankAllocation { get { return _bankAllocation; } }

        private void GetDoorTileCounts() {
            // Our patch changes the BEQ instruction to a BCC.
            bool DoorTilePatchApplied = (data[0x1e819] == (byte)Editroid.Asm.Opcodes.bcc);
            
            //if (DoorTilePatchApplied) {
            //    DoorTileCount = data[0x1e818] - 0xA0;
            //    HDoorTileCount = data[0x1e81C] - 0xa0 - DoorTileCount;
            //} else {
            //    DoorTileCount = HDoorTileCount = 1;
            //}
            DoorTileCount = HighestDoorTileIndex - 0x9F;
            DoorAltTileCount = HighestAltDoorTileIndex - 0x9F - DoorTileCount;
        }


        byte[] savedVersion = new byte[0];
        public byte[] SavedVersion { get { return savedVersion; } }
        private void CacheSavedVersion() {
            if (savedVersion.Length != data.Length) {
                savedVersion = new byte[data.Length];

            }
            data.CopyTo(savedVersion, 0);

        }

        /// <summary>
        /// Gets the number of tiles that will represent a door marker.
        /// </summary>
        public int DoorTileCount { get; private set; }
        /// <summary>
        /// Gets the number of tiles that will represent a horizontal-to-horizontal door marker.
        /// </summary>
        public int DoorAltTileCount { get; private set; }

        /// <summary>
        /// Gets the showPhysics of a tile.
        /// </summary>
        /// <param name="tile">The count of a tile.</param>
        /// <returns>T Physics value for the showPhysics of a tile.</returns>
        public Physics GetPhysics(byte tileIndex) {
            if (tileIndex == 0x4E)
                return Physics.DoorBubble;
            if (tileIndex == 0x6D) return Physics.SlopeRight;
            if (tileIndex == 0x6E) return Physics.SlopeLeft;
            if (tileIndex == 0x6F) return Physics.SlopeTop;
            if (tileIndex < 0x70)
                return Physics.Solid;
            if (tileIndex < 0x98)
                return Physics.Breakable;
            if (tileIndex < 0xA0)
                return Physics.Solid;
            if (tileIndex >= 0xA0 + DoorTileCount + DoorAltTileCount)
                return Physics.Air;
            if (tileIndex < 0xA0 + DoorTileCount)
                return Physics.Door;


            return Physics.DoorHorizontal;
        }

        #region Rom Version Comparing
        public IList<ChangeRange> CompareToSavedVersion() {
            return CompareVersions(data, savedVersion);
        }
        public IList<ChangeRange> CompareToVersion(byte[] otherVersion) {
            return CompareVersions(data, otherVersion);
        }

        public static IList<ChangeRange> CompareVersions(byte[] data, byte[] otherVersion) {
            List<ChangeRange> ranges = new List<ChangeRange>();

            int offset = 0;
            int end = Math.Min(data.Length, otherVersion.Length);

            while (offset < end) {
                if (data[offset] != otherVersion[offset]) {
                    ChangeRange range;
                    range.start = offset;
                    range.length = 1;
                    offset++;

                    while (offset < end && data[offset] != otherVersion[offset]) {
                        offset++;
                        range.length++;
                    }

                    ranges.Add(range);
                }
                offset++;
            }

            return ranges;
        }

        public struct ChangeRange
        {
            public int start, length;

            public override string ToString() {
                return "@ $" + start.ToString("x") + ", $" + length.ToString("x") + " bytes";
            }
        }
    #endregion

        private ROM.PatternGroupIndexTable globalPatternGroups;
        /// <summary>
        /// Gets the pattern groups that are loaded at the title screen. Some of these
        /// remain in memory (and are used) throughout gameplay.
        /// </summary>
        public ROM.PatternGroupIndexTable GlobalPatternGroups {
            get { return globalPatternGroups; }
        }


        private PasswordData passwordData;

        /// <summary>
        /// Gets password data for this currentLevelIndex.
        /// </summary>
        public PasswordData PasswordData { get { return passwordData; } }

        public PatternGroupOffsetTable PatternGroupOffsets { get { return patternGroupOffsets; } }
        private PatternGroupOffsetTable patternGroupOffsets;

        private TitleText titleText;
        /// <summary>
        /// Provides an object to modify text on the title screen.
        /// </summary>
        public TitleText TitleText { get { return titleText; } }

        LevelCollection levels;
        public LevelCollection Levels { get { return levels; } }

        public ChrAnimationLevelData TitleChrAnimationTable { get { return ChrAnimationData[LevelIndex.None]; } set { ChrAnimationData[LevelIndex.None] = value; } }

        public Level this[LevelIndex level] { get { return GetLevel(level); } }

		/// <summary>
		/// Saves the edited data of this ROM to a stream.
		/// </summary>
		/// <param name="s">The stream to output data to.</param>
		public void SaveTo(Stream s) {
            SerializeAllData();
            //foreach (Level l in levels.Values)
            //    l.ItemTable.ResortEntries();

			s.Write(data, 0, data.Length);

            CacheSavedVersion();
		}

        /// <summary>
        /// Writes all data to ROM image. Call this before using the ROM image to ensure that the ROM
        /// image is syncronized with any edits made.
        /// </summary>
        public void SerializeAllData() {
            if (Format.HasPrgAllocationTable) {
                ScreenWritePointers.Reset();
                ScreenWritePointers.SetReservedBanks(BankAllocation);
            }

            foreach (var level in levels) {
                level.Value.SaveToRom();
            }

            if (Format.HasChrAnimationTable && !ChrAnimationTableMissing) {
                SerializeChrAnimationTable();
            }

            SaveEditorData();
            if (BankAllocation != null) {
                ROM.BankAllocation.WriteAllocation(data, ROM.BankAllocation.BankAllocationOffset, BankAllocation);
            }
        }

        /// <summary>
        /// This method is to support ROMs created using a preliminary version of the MMC3 format. These ROMs still used the CHR usage table
        /// instead of the CHR animation table, thus it's important to not serialize the chr animation table, which will cause the CHR usage 
        /// table to be overwritten and lost.
        /// </summary>
        public bool ChrAnimationTableMissing { get; set; }


		private void LoadLevels() {
            levels = new LevelCollection(this);
		}


		const int RoomBaseOffset = 0x254E;
		/// <summary>
		/// Gets which screen to use for a specified map position
		/// </summary>
		/// <param name="X">The xTile-coordinate on the map</param>
		/// <param name="Y">The yTile-coordinate on the map</param>
		/// <returns>The room count of the specified room</returns>
		public byte GetScreenIndex(int X, int Y) {
			return GetScreenIndex(X + Y * 0x20);
		}
		/// <summary>
		/// Gets which screen to use for a specified map position
		/// </summary>
		/// <param name="roomOffset">The offset of the room</param>
		/// <returns>The room count of the specified room</returns>
		public byte GetScreenIndex(int roomOffset) {
			return data[RoomBaseOffset + roomOffset];
		}

		/// <summary>
		/// Sets which screen to use for a specified map position
		/// </summary>
		/// <param name="xTile">The xTile-coordinate of the room</param>
		/// <param name="yTile">The yTile-coordinate of the room</param>
		/// <param name="value">The count of the room</param>
		public void SetScreenIndex(int X, int Y, byte value) {
			SetScreenIndex(X + Y * 0x20, value);
		}
		/// <summary>
		/// Sets which screen to use for a specified map position
		/// </summary>
		/// <param name="Value">The count of the room</param>
		/// <param name="roomOffset">The count of the room</param>
		public void SetScreenIndex(int roomOffset, byte Value) {
			data[RoomBaseOffset + roomOffset] = Value;
		}

		//Level _Brinstar, _Ridley, _Kraid, _Tourian, _Norfair;
		/// <summary>
		/// Brinstar currentLevelIndex data.
		/// </summary>
		public Level Brinstar { get { return Levels[LevelIndex.Brinstar]; } }
		/// <summary>
		/// Ridley currentLevelIndex data.
		/// </summary>
		public Level Ridley { get { return Levels[LevelIndex.Ridley]; } }
		/// <summary>
		/// Kraid currentLevelIndex data.
		/// </summary>
		public Level Kraid { get { return Levels[LevelIndex.Kraid]; } }
		/// <summary>
		/// Tourian currentLevelIndex data.
		/// </summary>
		public Level Tourian { get { return Levels[LevelIndex.Tourian]; } }
		/// <summary>
		/// Norfair currentLevelIndex data.
		/// </summary>
		public Level Norfair { get { return Levels[LevelIndex.Norfair]; } }

		/// <summary>
		/// Returns a Editroid.Level based on the specified value.
		/// </summary>
		/// <param name="currentLevelIndex">The currentLevelIndex to get</param>
		/// <returns>T Editroid.Level based on the specified value</returns>
		public Level GetLevel(LevelIndex level) {
            if (level == LevelIndex.None) return Brinstar;

            return levels[level];
		}

        /// <summary>
        /// Converts game text into a unicode string.
        /// </summary>
        /// <param name="offset">ROM offset to load data from.</param>
        /// <param name="length">The number of characters to load.</param>
        /// <returns>T string representation of ROM text.</returns>
        /// <remarks>Invalid or unknown data will be converted to underscores.</remarks>
        public string GetRomText(int offset, int length) {
            char[] result = new char[length];
            for(int i = 0; i < length; i++) {
                byte b = data[i + offset];
                if(b >= 0 && b <= 9) // Digit
                    result[i] = (char)((int)'0' + b);
                else if(b >= 0xA && b <= 0x23) // Letter
                    result[i] = (char)((int)'A' + b - 0xA);
                else if(b >= 0x24 && b <= 0x3D) // Lowercase letter
                    result[i] = (char)((int)'a' + b - 0x24);
                else if(b == 0x3E) // Question Mark
                    result[i] = '?';
                else if(b == 0x3F) // Dash
                    result[i] = '-';
                else if(b == 0xFF) // Space
                    result[i] = ' ';
                else if(b == 0x8F) // Copyright
                    result[i] = '©'; 
                else // unknown
                    result[i] = '_';

            }

            return new string(result);
        }
        /// <summary>
        /// Converts a unicode string into Metroid formatted text and writes it to the ROM.
        /// </summary>
        /// <param name="offset">The offset to write the string to.</param>
        /// <param name="text">The text to write to the ROM.</param>
        /// <remarks>Underscores will be ignored and the ROM data in their place
        /// will remain. This is done so that invalid or unknown data will remain 
        /// intact in a round-trip operation, but can have unforseen consequences
        /// if data is inserted or removed from the a string during a round-trip
        /// operation, or if text happens to have underscores in it.</remarks>
        public void SetRomText(int offset, string text) {
            for(int i = 0; i < text.Length; i++) {

                char c = text[i];

                if(c >= '0' && c <= '9') // Digit
                    data[i + offset] = (byte)(c - '0');
                else if(c >= 'A' && c <= 'Z') // Letter
                    data[i + offset] = (byte)(c - 'A' + 0xA);
                else if(c >= 'a' && c <= 'z') // Lowercase letter
                    data[i + offset] = (byte)(c - 'a' + 0x24);
                else if(c == '?')
                    data[i + offset] = 0x3E;
                else if(c == '-')
                    data[i + offset] = 0x3F;
                else if(c == ' ')
                    data[i + offset] = 0xFF;
                else if(c == '©')
                    data[i + offset] = 0x8F;
                else if (c == '_') // Unknown
                    { } // Don't modify value
                else // Unknown
                    data[i + offset] = 0x3E;
            }
        }
        #region IRomDataParentObject Members

        int IRomDataObject.Offset { get { return 0; } }
        int IRomDataObject.Size { get { return 0; } }

        bool IRomDataObject.HasListItems { get { return false; } }
        bool IRomDataObject.HasSubItems { get { return true; } }

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return new IRomDataObject[] { Brinstar, Norfair, Ridley, Kraid, Tourian, PasswordData, TitleText };
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            return RomDataObjects.EmptyList;
        }

        string IRomDataObject.DisplayName { get { return "Metroid"; } }


        #endregion

        internal void ReloadPatterns(Stream s) {
            var patternLocations = Format.GetAllPatternOffsets();
            for (int i = 0; i < patternLocations.Length; i++) {
                s.Seek(patternLocations[i].Start, SeekOrigin.Begin);
                s.Read(data, patternLocations[i].Start, patternLocations[i].Length);
            }

            foreach (Level l in levels.Values) {
                l.InvalidatePatterns();
            }
        }

        public const int TableOffset_Bank0 = 0x7B610;
        public const int TableOffset_Bank1 = 0x7B710;
        public const int TableOffset_Bank2 = 0x7B810;
        public const int TableOffset_Bank3 = 0x7B910;
        public const int TableOffset_FrameData = 0x7BA10;

        public byte GetAnimationTable_Bank0(int frameIndex) {
            return data[TableOffset_Bank0 + frameIndex];
        }
        public void SetAnimationTable_Bank0(int frameIndex, byte value) {
            data[TableOffset_Bank0 + frameIndex] = value;
        }

        public byte GetAnimationTable_Bank1(int frameIndex) {
            return data[TableOffset_Bank1 + frameIndex];
        }
        public void SetAnimationTable_Bank1(int frameIndex, byte value) {
            data[TableOffset_Bank1 + frameIndex] = value;
        }

        public byte GetAnimationTable_Bank2(int frameIndex) {
            return data[TableOffset_Bank2 + frameIndex];
        }
        public void SetAnimationTable_Bank2(int frameIndex, byte value) {
            data[TableOffset_Bank2 + frameIndex] = value;
        }

        public byte GetAnimationTable_Bank3(int frameIndex) {
            return data[TableOffset_Bank3 + frameIndex];
        }
        public void SetAnimationTable_Bank3(int frameIndex, byte value) {
            data[TableOffset_Bank3 + frameIndex] = value;
        }


        public byte GetAnimationTable_FrameData(int frameIndex) {
            return data[TableOffset_FrameData + frameIndex];
        }
        public void SetAnimationTable_FrameData(int frameIndex, byte value) {
            data[TableOffset_FrameData + frameIndex] = value;
        }

        public int GetAnimationTable_FrameTime(int frameIndex) {
            return GetAnimationTable_FrameData(frameIndex) & 0x7F;
        }
        public void SetAnimationTable_FrameTime(int frameIndex, int value) {
            SetAnimationTable_FrameData(
                frameIndex,
                (byte)((GetAnimationTable_FrameData(frameIndex) & 0x80) | (value & 0x7F))
                );
        }
        public bool GetAnimationTable_FrameLast(int frameIndex) {
            return 0 != (GetAnimationTable_FrameData(frameIndex) & 0x80);
        }
        public void SetAnimationTable_FrameLast(int frameIndex, bool value) {
            SetAnimationTable_FrameData(
                frameIndex, 
                (byte)((value ? 0x80 : 0) | GetAnimationTable_FrameTime(frameIndex))
                );

        }

        public enum LevelBanks
        {
            Brinstar = 0x4000,
            Norfair = 0x8000,
            Tourian = 0xC000,
            Kraid = 0x10000,
            Ridley = 0x14000,
        }
        public const int HeaderSize = 0x10;

        static readonly pCpu pDoorPhysics_HighestTileIndex = new pCpu(0xe808);
        /// <summary>Gets the index of the highest tile value that has door physics in a combo.</summary>
        public byte HighestDoorTileIndex {
            get {
                ////var dataOffset = Format.ResolvePrgPointer(DoorTileOffset);
                ////return data[(int)dataOffset];
                return FixedBank[pDoorPhysics_HighestTileIndex];
            }
        }
        static readonly pCpu pAltDoorPhysics_HighestTileIndex = new pCpu(0xe80C);
        /// <summary>Gets the index of the highest tile value that has alt-door physics in a combo.</summary>
        public byte HighestAltDoorTileIndex {
            get {
                ////var dataOffset = Format.ResolvePrgPointer(pAltDoorPhysics_HighestTileIndex);
                ////return data[(int)dataOffset];
                return FixedBank[pAltDoorPhysics_HighestTileIndex];
            }
        }

        /// <summary>
        /// Gets a pointer from ROM data at the specified ROM offset.
        /// </summary>
        /// <param name="entryOffset"></param>
        /// <returns></returns>
        internal pCpu GetPointer(pRom offset) {
            return new pCpu(data, (int)offset);
        }

        internal void WritePointer(pRom offset, pCpu pointer) {
            data[(int)offset] = pointer.Byte1;
            data[(int)offset + 1] = pointer.Byte2;
        }



        /// <summary>
        /// Gets a value that indicates the preferred CHR animations to load patterns for. This value should be clamped to the range of available animations for each level.
        /// </summary>
        /// <remarks>This property is applicable only to ROMs that support CHR animation tables and is only for editor purpose.</remarks>
        internal int PreferredAnimationIndex { get; private set; }
        internal void ReloadChrAnimations(int chrAnimationIndex) {
            PreferredAnimationIndex = chrAnimationIndex;
            foreach (var level in Levels.Values) {
                level.ReloadPatterns();
            }
        }
    }


    // Todo: check any code that references RomFormats: may need to update to work with RomFormats.MMC3
    public enum RomFormats
    {
        Standard,
        Expando,
        Enhanco,
        MMC3
    }

    class RoomSerializationPointers
    {
        int[] pointers = new int[Mmc3.BankCount];

        public RoomSerializationPointers() {
            Reset();
        }

        public bool AllocateSpace(int size, out int bank, out int address) {
            bank = 0;
            address = 0;

            for (int i = 0; i < pointers.Length; i++) {
                var space = GetSpaceInBank(i);
                if (space >= size) {
                    bank = i;
                    address = pointers[i];
                    pointers[i] += size;
                    return true;
                }
            }

            return false;
        }

        private int GetSpaceInBank(int index) {
            int space = 0xC000 - pointers[index];
            return Math.Max(0, space);
        }

        public void Reset() {
            for (int i = 0; i < pointers.Length; i++) {
                pointers[i] = 0xA000;
            }
        }

        public void SetReservedBanks(BankAllocation[] banks) {
            int count = Math.Min(banks.Length, pointers.Length);

            for (int i = 0; i < count; i++) {
                if (banks[i].Reserved || banks[i].UserReserved) {
                    pointers[i] = 0xC000;
                }
            }
        }
    }

}


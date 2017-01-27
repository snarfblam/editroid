using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Editroid
{
	/// <summary>
	/// Represents a 16-color NES paletteIndex composed of four color tables.
	/// </summary>
	/// <remarks>This class stores data locally. Changes will not be reflected in the ROM.</remarks>
	public class NesPalette
	{
		/// <summary>
		/// Creates a paletteIndex with default values.
		/// </summary>
		public NesPalette() {
		}

		/// <summary>
		/// Loads paletteIndex data for four consecutive color tables (total of 16 colors) from an NES rom.
		/// </summary>
		/// <param name="data">ROM image</param>
		/// <param name="offset">Location of data</param>
		public NesPalette(byte[] data, int offset) {
			//Array.Copy(data, offset, Table, 0, 16);
			Data = data;
			_Offset = offset;
		}

        public static NesPalette FromLevel_BG(Level level) {
            return new NesPalette(level.Rom.data, GetBackgroundPaletteOffset(level));
        }
        public static NesPalette FromLevel_BG_Alt(Level level) {
            return new NesPalette(level.Rom.data, GetAltBackgroundPaletteOffset(level));
        }
        public static NesPalette FromLevel_SPR(Level level) {
            return new NesPalette(level.Rom.data, GetSpritePaletteOffset(level));
        }
        public static NesPalette FromLevel_SPR_Alt(Level level) {
            return new NesPalette(level.Rom.data, GetAltSpritePaletteOffset(level));
        }
		byte[] Data;
		int _Offset;

        /// <summary>Gets the offset of the paletteIndex data in the ROM.</summary>
        public int Offset { get { return _Offset; } }
        /// <summary>
        /// Gets a paletteIndex entry from this paletteIndex.
        /// </summary>
        /// <param name="count">The count of the entry to get.</param>
        /// <returns>T color corresponding to an NES paletteIndex color.</returns>
        public byte GetEntry(int index) {
            return Data[_Offset + index];
        }
        /// <summary>
        /// Gets a paletteIndex entry from this paletteIndex.
        /// </summary>
        /// <param name="count">The count of the entry to get.</param>
        /// <returns>T color corresponding to an NES paletteIndex color.</returns>
        public void SetEntry(int index, byte value) {
            Data[_Offset + index] = value;
        }

		/// <summary>
		/// Applies the colors that this paletteIndex represents to a color array
		/// </summary>
		/// <param name="colors">Color array to modify</param>
		public void ApplyTable(Color[] colors) {
			for(int i = 0; i < 16; i++) {
				//colors[i] = NesColors[Table[i]];
				colors[i] = NesColors[Data[_Offset + i]];
			}
		}

		/// <summary>
		/// Applies the colors that this paletteIndex represents to a color array
		/// </summary>
		/// <param name="colors">Color array to modify</param>
		/// <param name="offset">Where in the array these colors should appear</param>
		public void ApplyTable(Color[] colors, int offset) {
			for(int i = 0; i < 16; i++) {
				colors[i + offset] = NesColors[Data[_Offset + i]];
			}
		}
        /// <summary>
        /// Applies the colors that this paletteIndex represents to a color array
        /// </summary>
        /// <param name="colors">Color array to modify</param>
        /// <param name="offset">Where in the array these colors should appear</param>
        /// <param name="start">Which color in the paletteIndex to start copying from</param>
        /// <param name="count">How many colors to copy</param>
        public void ApplyTable(Color[] colors, int offset, int start, int count) {
            for (int i = start; i < start + count; i++) {
                colors[offset] = NesColors[Data[_Offset + i]];
                offset++;
                //colors[i + offset] = NesColors[Table[i]];
            }
        }
        public delegate Color ColorTransform(Color input);
        /// <summary>
        /// Applies the colors that this paletteIndex represents to a color array
        /// </summary>
        /// <param name="colors">Color array to modify</param>
        /// <param name="offset">Where in the array these colors should appear</param>
        /// <param name="start">Which color in the paletteIndex to start copying from</param>
        /// <param name="count">How many colors to copy</param>
        public void ApplyTable(Color[] colors, int offset, int start, int count, ColorTransform transform) {
            for (int i = start; i < start + count; i++) {
                colors[offset] = transform(NesColors[Data[_Offset + i]]);
                offset++;
                //colors[i + offset] = NesColors[Table[i]];
            }
        }

		/// <summary>
		/// Gets the offset of the background paletteIndex for the specified currentLevelIndex
		/// </summary>
		/// <param name="currentLevelIndex">a Metroid currentLevelIndex</param>
		/// <returns>The offset of the background paletteIndex for the specified currentLevelIndex</returns>
		public static int GetBackgroundOffset(LevelIndex level) {
			switch(level) {
				case LevelIndex.Kraid:
					return KraidBackgroundOffset;
				case LevelIndex.Brinstar:
					return BrinstarBackgroundOffset;
				case LevelIndex.Norfair:
					return NorfairBackgroundOffset;
				case LevelIndex.Ridley:
					return RidleyBackgroundOffset;
				case LevelIndex.Tourian:
					return TourianBackgroundOffset;
				default:
					return 0;
					//throw new ArgumentException("Specified value not valid", "currentLevelIndex");
			}
		}
		/// <summary>
		/// Gets the offset of the sprite paletteIndex for the specified currentLevelIndex
		/// </summary>
		/// <param name="currentLevelIndex">a Metroid currentLevelIndex</param>
		/// <returns>The offset of the background paletteIndex for the specified currentLevelIndex</returns>
		public static int GetSpriteOffset(LevelIndex level) {
			return GetBackgroundOffset(level) + 0x10;
		}
		/// <summary>
		/// Gets the offset of the background paletteIndex for the specified currentLevelIndex
		/// </summary>
		/// <param name="currentLevelIndex">a Metroid currentLevelIndex</param>
		/// <returns>The offset of the background paletteIndex for the specified currentLevelIndex</returns>
		public static int GetAltBackgroundOffset(LevelIndex level) {
			switch(level) {
				case LevelIndex.Kraid:
					return KraidAltBackgroundOffset;
				case LevelIndex.Brinstar:
					return BrinstarAltBackgroundOffset;
				case LevelIndex.Norfair:
					return NorfairAltBackgroundOffset;
				case LevelIndex.Ridley:
					return RidleyAltBackgroundOffset;
				case LevelIndex.Tourian:
					return TourianAltBackgroundOffset;
				default:
					return 0;
					//throw new ArgumentException("Specified value not valid", "currentLevelIndex");
			}
		}


        const int normalPalIndex = 0; // Normal palette macro's index
        const int altPalIndex = 5; // Alternate palette macro's index
        const int paletteSize = 0x10; // full 16-color pal
        const int macroHeaderSize = 3;
        public static int GetBackgroundPaletteOffset(Level level) {
            pCpu macroPointer = level.PalettePointers[0];
            int macroOffset = level.ToPRom(macroPointer);
            macroOffset += macroHeaderSize;
            return macroOffset;
        }

        public static int GetSpritePaletteOffset(Level level) {
            return GetBackgroundPaletteOffset(level) + paletteSize;
        }
        public static int GetAltBackgroundPaletteOffset(Level level) {
            pCpu macroPointer = level.PalettePointers[5];
            int macroOffset = level.ToPRom(macroPointer);
            macroOffset += macroHeaderSize;
            return macroOffset;
        }
        public static int GetAltSpritePaletteOffset(Level level) {
            return GetAltBackgroundPaletteOffset(level) + paletteSize;
        }

        /// <summary>Identifies the location of the palette macro pointer table, relative to the start
        /// of a level-data bank.</summary>
        public const int LevelPalettePointerTableOffset = 0x1560; // Banked in @ 9560

		/// <summary>
		/// Offset of the background palettes for Brinstar.
		/// </summary>
		public static int BrinstarBackgroundOffset { get { return 0x6284; } }
		/// <summary>
		/// Offset of the background palettes for Norfair.
		/// </summary>
		public static int NorfairBackgroundOffset { get { return 0xA18B; } }
		/// <summary>
		/// Offset of the background palettes for Kraid.
		/// </summary>
		public static int KraidBackgroundOffset { get { return 0x12168; } }
		/// <summary>
		/// Offset of the background palettes for Ridley.
		/// </summary>
		public static int RidleyBackgroundOffset { get { return 0x160FE; } }
		/// <summary>
		/// Offset of the background palettes for Tourian.
		/// </summary>
		public static int TourianBackgroundOffset { get { return 0xE72B; } }

		/// <summary>
		/// Offset of the background palettes for Brinstar.
		/// </summary>
		public static int BrinstarAltBackgroundOffset { get { return 0x62C0; } }
		/// <summary>
		/// Offset of the background palettes for Norfair.
		/// </summary>
		public static int NorfairAltBackgroundOffset { get { return 0xA18B + 0x3C; } }
		/// <summary>
		/// Offset of the background palettes for Kraid.
		/// </summary>
		public static int KraidAltBackgroundOffset { get { return 0x12168 + 0x3C; } }
		/// <summary>
		/// Offset of the background palettes for Ridley.
		/// </summary>
		public static int RidleyAltBackgroundOffset { get { return 0x160FE + 0x3C; } }
		/// <summary>
		/// Offset of the background palettes for Tourian.
		/// </summary>
		public static int TourianAltBackgroundOffset { get { return 0xE72B; } }


		/// <summary>
		/// Offset of the sprite palettes for Brinstar.
		/// </summary>
		public static int BrinstarSpriteOffset { get { return 0x6294; } }
		/// <summary>
		/// Offset of the sprite palettes for Norfair.
		/// </summary>
		public static int NorfairSpriteOffset { get { return 0xA19B; } }
		/// <summary>
		/// Offset of the sprite palettes for Kraid.
		/// </summary>
		public static int KraidSpriteOffset { get { return 0x12178; } }
		/// <summary>
		/// Offset of the sprite palettes for Ridley.
		/// </summary>
		public static int RidleySpriteOffset { get { return 0x1610E; } }
		/// <summary>
		/// Offset of the sprite palettes for Tourian.
		/// </summary>
		public static int TourianSpriteOffset { get { return 0xE73B; } }


        /// <summary>This field is used in the initialization of NesColors to improve readability.</summary>
        static Color black = Color.Black;

		/// <summary>
		/// This is full the color paletteIndex used for rendering NES graphics.
		/// The first sixty-four colors are the colors used by the NES. These
		/// can be changed to suit need if necessary. The remaining colors
		/// are there to complete a 256-color paletteIndex and can be used any way,
		/// but are initialized to black.
		/// </summary>
        public static Color[] NesColors = new Color[]{
			// 64 NES colors
			Color.FromArgb(-9079435), Color.FromArgb(-14214257), Color.FromArgb(-16777045), Color.FromArgb(-12124001), 
			Color.FromArgb(-7405449), Color.FromArgb(-5570541), Color.FromArgb(-5832704), Color.FromArgb(-8451328), 
			Color.FromArgb(-12374272), Color.FromArgb(-16759040), Color.FromArgb(-16756480), Color.FromArgb(-16761065), 
			Color.FromArgb(-14991521), Color.FromArgb(-16777216), Color.FromArgb(-16777216), Color.FromArgb(-16777216), 
			Color.FromArgb(-4408132), Color.FromArgb(-16747537), Color.FromArgb(-14468113), Color.FromArgb(-8191757), 
			Color.FromArgb(-4259649), Color.FromArgb(-1638309), Color.FromArgb(-2413824), Color.FromArgb(-3453169), 
			Color.FromArgb(-7638272), Color.FromArgb(-16738560), Color.FromArgb(-16733440), Color.FromArgb(-16739525), 
			Color.FromArgb(-16743541), Color.FromArgb(-16777216), Color.FromArgb(-16777216), Color.FromArgb(-16777216), 
			Color.FromArgb(-1), Color.FromArgb(-12599297), Color.FromArgb(-10512385), Color.FromArgb(-5796867), 
			Color.FromArgb(-558081), Color.FromArgb(-34889), Color.FromArgb(-34973), Color.FromArgb(-25797), 
			Color.FromArgb(-803009), Color.FromArgb(-8137965), Color.FromArgb(-11542709), Color.FromArgb(-10946408), 
			Color.FromArgb(-16716837), Color.FromArgb(-16777216), Color.FromArgb(-16777216), Color.FromArgb(-16777216), 
			Color.FromArgb(-1), Color.FromArgb(-5511169), Color.FromArgb(-3680257), Color.FromArgb(-2634753), 
			Color.FromArgb(-14337), Color.FromArgb(-14373), Color.FromArgb(-16461), Color.FromArgb(-9301), 
			Color.FromArgb(-6237), Color.FromArgb(-1835101), Color.FromArgb(-5508161), Color.FromArgb(-4980785), 
			Color.FromArgb(-6291469), Color.FromArgb(-16777216), Color.FromArgb(-16777216), Color.FromArgb(-16777216), 
	
			// These three blocks of 64 colors can be used for editor purposes.
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black,

			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 

			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, 
			black, black, black, black, black, black, black, black, black, black, black, black, black, black, black, black
		};

        /// <summary>
        /// The count of the paletteIndex entry reserved to hold the system's highlight color.
        /// </summary>
        public static byte  HighlightEntry = 0xFF;
	}

}

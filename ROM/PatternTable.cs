using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Editroid.ROM;

namespace Editroid
{
	/// <summary>
	/// Provides functions to load ROM graphic data into a System.Drawing.Bitmap.
	/// </summary>
	public class PatternTable:IRomDataParentObject, IDisposable
	{
		Bitmap patterns;
		bool _Linear;
        PatternGroupIndexTable groups;
        private MetroidRom rom;

        public MetroidRom Rom { get { return rom; } set { rom = value; } }

        private PatternTableType type;
        /// <summary>Specifies which pattern page this table represent. This can
        /// affect how patterns are loaded.</summary>
        public PatternTableType PatternType { get { return type; } set { type = value; } }

	

        /// <summary>
        /// Creates a new pattern table.
        /// </summary>
        /// <param name="linear">If true, the patterns will be loaded in a horizontal 
        /// string (good for constructing images from). If false, the patterns will be
        /// loaded in a 16 x 16 grid (good for displaying to user).</param>
        public PatternTable(bool linear) {
            _Linear = linear;
            if (linear)
                patterns = new Bitmap(2048, 8, PixelFormat.Format8bppIndexed);
            else
                patterns = new Bitmap(128, 128, PixelFormat.Format8bppIndexed);
        }

        public PatternTable(bool linear, PatternTableType type, PatternGroupIndexTable globalTiles, PatternGroupIndexTable levelTiles, MetroidRom rom) {
            this.type = type;
            this._Linear = linear;
            if (linear)
                patterns = new Bitmap(2048, 8, PixelFormat.Format8bppIndexed);
            else
                patterns = new Bitmap(128, 128, PixelFormat.Format8bppIndexed);

            this.rom = rom;
            this.groups = levelTiles;

            this.BeginWrite();
            LoadPatternGroups(globalTiles);
            LoadPatternGroups(levelTiles);
            this.EndWrite();

        }

        public static PatternTable LoadNewTableForExpandedRom(bool linear, PatternTableType type, Level level) 
        {
            PatternTable result = new PatternTable(linear);
            result.type = type;

            result.rom = level.Rom;
            result.groups = null;

            result.BeginWrite();
            if (type == PatternTableType.sprite) {
                // These sprites should be loaded for every level's sprites
                result.loadExpandoGroup(ExpandoPatternOffsets.GlobalGameplaySprites);
                result.loadExpandoGroup(ExpandoPatternOffsets.DigitSprites);

                result.loadExpandoGroup(ExpandoPatternOffsets.GetSpriteEntry(level.Index));
            } else if (type == PatternTableType.background) {
                result.loadExpandoGroup(ExpandoPatternOffsets.GetBackgroundEntry(level.Index));
            } else {
                throw new ArgumentException("Invalid pattern table type specified.");
            }
            result.EndWrite();

            return result;
        }

        void loadExpandoGroup(ExpandoPatternOffsets.Entry group) {
            LoadTiles(rom.data, group.RomOffset, group.DestTileindex, group.TileCount);
        }

        public void LoadPatternGroups(PatternGroupIndexTable table) {
            foreach (byte groupIndex in table) {
                PatternGroupOffsets offsets = rom.PatternGroupOffsets[groupIndex];
                // Page 0 = sprite pattern mem, 1 = bg.
                if (
                    (offsets.IsPage0 && type == PatternTableType.sprite)
                    || (offsets.IsPage1 && type == PatternTableType.background)) {

                    LoadTiles(rom.data, offsets.SourceRomOffset, offsets.DestTileIndex, offsets.TileCount);
                }
            }
        }


		BitmapData lockData = null;
		byte[] lockBits = null;

		/// <summary>
		/// Prepares the pattern table for loading tiles.
		/// </summary>
		public void BeginWrite() {
			if(lockData != null) throw new InvalidOperationException("The pattern table is already opened for writing.");

			// Opened as Read/Write so that old, unmodified data is left intact
			lockData = patterns.LockBits(new Rectangle(0, 0, patterns.Width, patterns.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
			if(lockBits == null) lockBits = new byte[0x4000];

			Marshal.Copy(lockData.Scan0, lockBits, 0, 0x4000);
		}

		/// <summary>
		/// Finalizing writing operations to the pattern table.
		/// </summary>
		public void EndWrite() {
			if(lockData == null) throw new InvalidOperationException("The pattern table is not opened for writing.");

			Marshal.Copy(lockBits, 0, lockData.Scan0, 0x4000);
			patterns.UnlockBits(lockData);
            lockData = null;
		}


        static byte[] _ErrorTiles;
        private byte[] ErrorTiles {
            get {
                if (_ErrorTiles == null)
                    _ErrorTiles = Editroid.Properties.Resources.Error_CHR;
                return _ErrorTiles;
            }
        }
		/// <summary>
		/// Loads a strip of tiles from raw ROM data into this pattern table.
		/// </summary>
		/// <param name="data">The ROM image to load tiles from</param>
        /// <param name="sourceOffset">The location of the graphic data</param>
        /// <param name="destTileStart">Which tile to begin loading at</param>
        /// <param name="destTileCount">The number of tiles to load</param>
		/// <remarks>T linear strip of tiles is loaded starting at the specified tile count. If the tiles
		/// reach the end of a row in a square pattern table they wrap to the next line. If the end of the pattern table is reached
		/// or the end of ROM data is reached, copying will stop without error.
		/// The function BeginWrite must be called at some points before this function
		/// is called. The function EndWrite must be called to finalize the process. Otherwise
		/// the loaded tiles will not be reflected in this pattern table.</remarks>
		public void LoadTiles(byte[] data, int sourceOffset, int destTileStart, int destTileCount) {
            int endOffset = sourceOffset + destTileCount * 0x10; // exclusive
            bool outOfRange = sourceOffset < 0 || endOffset > data.Length;

            // Use our error graphics (all 'x's) if we're trying to load from beyond the end of the ROM
            if (outOfRange) {
                data = ErrorTiles;
                sourceOffset = 0;
            }
            
            if(_Linear)
				LoadTilesString(data, (pRom)sourceOffset, destTileStart, destTileCount);
			else
				LoadTilesGrid(data, sourceOffset, destTileStart, destTileCount);
		}

        public void LoadTiles(PatternGroupOffsets tiles) {
            LoadTiles(tiles.Rom.data, tiles.SourceRomOffset, tiles.DestTileIndex, tiles.TileCount);
        }

        public void Clear() {
            bool mustBeginEnd = lockData == null;
            if (mustBeginEnd)
                BeginWrite();

            for (int i = 0; i < lockBits.Length; i++) {
                lockBits[i] = 0;
            }

            if (mustBeginEnd)
                EndWrite();
        }

		private void LoadTilesGrid(byte[] data, int offset, int tileOffset, int tileCount) {
			for(int Tile = tileOffset; Tile < tileOffset + tileCount; Tile++) {
				// The tile xTile/yTile coords are used to find the pixel offset within the resulting image
				int TileY = Tile / 16;
				int TileX = Tile % 16;

				// NES graphic data is bi-planar (2bpp image is actually stored as two 1bpp images)
				// Load first plane
				for(int PixelY = 0; PixelY < 8; PixelY += 1) {
					int imageOffset = TileY * 1024 + TileX * 8 + PixelY * 128;
					lockBits[imageOffset + 7] = (byte)(data[offset] & 0x01);
					lockBits[imageOffset + 6] = (byte)((data[offset] & 0x02) >> 1);
					lockBits[imageOffset + 5] = (byte)((data[offset] & 0x04) >> 2);
					lockBits[imageOffset + 4] = (byte)((data[offset] & 0x08) >> 3);
					lockBits[imageOffset + 3] = (byte)((data[offset] & 0x10) >> 4);
					lockBits[imageOffset + 2] = (byte)((data[offset] & 0x20) >> 5);
					lockBits[imageOffset + 1] = (byte)((data[offset] & 0x40) >> 6);
					lockBits[imageOffset + 0] = (byte)((data[offset] & 0x80) >> 7);
					offset++;

				}
				for(int PixelY = 0; PixelY < 8; PixelY += 1) {
					int imageOffset = TileY * 1024 + TileX * 8 + PixelY * 128;
					lockBits[imageOffset + 7] |= (byte)((data[offset] & 0x01) << 1);
					lockBits[imageOffset + 6] |= (byte)((data[offset] & 0x02));
					lockBits[imageOffset + 5] |= (byte)((data[offset] & 0x04) >> 1);
					lockBits[imageOffset + 4] |= (byte)((data[offset] & 0x08) >> 2);
					lockBits[imageOffset + 3] |= (byte)((data[offset] & 0x10) >> 3);
					lockBits[imageOffset + 2] |= (byte)((data[offset] & 0x20) >> 4);
					lockBits[imageOffset + 1] |= (byte)((data[offset] & 0x40) >> 5);
					lockBits[imageOffset + 0] |= (byte)((data[offset] & 0x80) >> 6);

					offset++;
				} // for
			} // for
		} // function
		private void LoadTilesString(byte[] data, pRom offset, int tileOffset, int tileCount) {
			for(int Tile = tileOffset; Tile < tileOffset + tileCount; Tile++) {
				// NES graphic data is bi-planar (2bpp image is actually stored as two 1bpp images)
				// Load first plane
				for(int PixelY = 0; PixelY < 8; PixelY += 1) {
					int imageOffset = Tile * 8 + PixelY * 2048;
					lockBits[imageOffset + 7] = (byte)(data[offset] & 0x01);
					lockBits[imageOffset + 6] = (byte)((data[offset] & 0x02) >> 1);
					lockBits[imageOffset + 5] = (byte)((data[offset] & 0x04) >> 2);
					lockBits[imageOffset + 4] = (byte)((data[offset] & 0x08) >> 3);
					lockBits[imageOffset + 3] = (byte)((data[offset] & 0x10) >> 4);
					lockBits[imageOffset + 2] = (byte)((data[offset] & 0x20) >> 5);
					lockBits[imageOffset + 1] = (byte)((data[offset] & 0x40) >> 6);
					lockBits[imageOffset + 0] = (byte)((data[offset] & 0x80) >> 7);
					offset++;

				}
				for(int PixelY = 0; PixelY < 8; PixelY += 1) {
					int imageOffset = Tile * 8 + PixelY * 2048;
					lockBits[imageOffset + 7] |= (byte)((data[offset] & 0x01) << 1);
					lockBits[imageOffset + 6] |= (byte)((data[offset] & 0x02));
					lockBits[imageOffset + 5] |= (byte)((data[offset] & 0x04) >> 1);
					lockBits[imageOffset + 4] |= (byte)((data[offset] & 0x08) >> 2);
					lockBits[imageOffset + 3] |= (byte)((data[offset] & 0x10) >> 3);
					lockBits[imageOffset + 2] |= (byte)((data[offset] & 0x20) >> 4);
					lockBits[imageOffset + 1] |= (byte)((data[offset] & 0x40) >> 5);
					lockBits[imageOffset + 0] |= (byte)((data[offset] & 0x80) >> 6);

					offset++;
				} // for
			} // for
		} // function

        public static void ExtractSingleTile(Bitmap b, Point tileLocation, byte[] data, pRom offset){
            BitmapData lockData = b.LockBits(new Rectangle(tileLocation.X, tileLocation.Y, 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            pRom outputOffset = pRom.Null;
            byte[] output = new byte[lockData.Stride * 8];

            // Image data in NES rom has 2 planes. This is the first
            for (int PixelY = 0; PixelY < 8; PixelY += 1) {
                output[outputOffset + 7] = (byte)(data[offset] & 0x01);
                output[outputOffset + 6] = (byte)((data[offset] & 0x02) >> 1);
                output[outputOffset + 5] = (byte)((data[offset] & 0x04) >> 2);
                output[outputOffset + 4] = (byte)((data[offset] & 0x08) >> 3);
                output[outputOffset + 3] = (byte)((data[offset] & 0x10) >> 4);
                output[outputOffset + 2] = (byte)((data[offset] & 0x20) >> 5);
                output[outputOffset + 1] = (byte)((data[offset] & 0x40) >> 6);
                output[outputOffset + 0] = (byte)((data[offset] & 0x80) >> 7);
                offset++;
                outputOffset += lockData.Stride;
            }

            // Second plane.
            outputOffset = pRom.Null;
            for (int PixelY = 0; PixelY < 8; PixelY += 1) {
                output[outputOffset + 7] |= (byte)((data[offset] & 0x01) << 1);
                output[outputOffset + 6] |= (byte)((data[offset] & 0x02));
                output[outputOffset + 5] |= (byte)((data[offset] & 0x04) >> 1);
                output[outputOffset + 4] |= (byte)((data[offset] & 0x08) >> 2);
                output[outputOffset + 3] |= (byte)((data[offset] & 0x10) >> 3);
                output[outputOffset + 2] |= (byte)((data[offset] & 0x20) >> 4);
                output[outputOffset + 1] |= (byte)((data[offset] & 0x40) >> 5);
                output[outputOffset + 0] |= (byte)((data[offset] & 0x80) >> 6);

                offset++;
                outputOffset += lockData.Stride;
            }

            Marshal.Copy(output, 0, lockData.Scan0, output.Length);
            b.UnlockBits(lockData);
        }

		/// <summary>
		/// Returns an 8 bit-per-pixel brush containing all loaded tiles.
		/// </summary>
		public Bitmap PatternImage {
			get {
				return patterns;
			}
		}

		/// <summary>
		/// Gets or sets the paletteIndex applied to the tiles.
		/// </summary>
		/// <remarks>Changes to the paletteIndex will be reflected in all references to
		/// this PatternTable's Patterns. If you need to use multiple palettes simultaneously
		/// you should, for each differently paletted version, set the desired paletteIndex,
		/// clone the pattern brush and retain the clone(s) as the paletted brush(s).</remarks>
		public Color[] Palette {
			get {
				return patterns.Palette.Entries;
			}
			set {
				ColorPalette pal = patterns.Palette;
				if(value.Length != 4) throw new InvalidOperationException("Only a four-color palette can be specified.");

				Array.Copy(value, pal.Entries, 4);
				patterns.Palette = pal;
			}
		}

        public void LoadColors(NesPalette palette, int palIndex) {
            Color[] palColors = new Color[16];
            palette.ApplyTable(palColors);

            Color[] pal4 = new Color[4];
            pal4[0] = palColors[4 * palIndex];
            pal4[1] = palColors[4 * palIndex + 1];
            pal4[2] = palColors[4 * palIndex + 2];
            pal4[3] = palColors[4 * palIndex + 3];
            Palette = pal4;
        }

        #region IRomDataParentObject Members

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return RomDataObjects.EmptyNode;
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            LineDisplayItem[] items = new LineDisplayItem[groups.Count];
            for (int i = 0; i < groups.Count; i++) {
                PatternGroupOffsets offsets = rom.PatternGroupOffsets[groups[i]];

                items[i] = new LineDisplayItem(
                    "Pattern Group " + i.ToString("X"),
                    offsets.SourceRomOffset,
                    offsets.TileCount * 16,
                    rom.data);
            }

            return items;
        }
        #endregion

        #region IRomDataObject Members

        int IRomDataObject.Offset { get { return 0; } }
        int IRomDataObject.Size { get { return 0; } }
        bool IRomDataObject.HasListItems { get { return true; } }
        bool IRomDataObject.HasSubItems { get { return false; } }
        string IRomDataObject.DisplayName { get { return "Patterns"; } }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            patterns.Dispose();    
        }

        #endregion

    }

    public enum PatternTableType
    {
        undefined,
        sprite,
        background
    }
}

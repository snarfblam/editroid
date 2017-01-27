using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Editroid.Graphic
{
	/// <summary>
	/// Specialized class that performs blitting operations from a 2048x8 tile source
	/// to a 256x256 destination.
	/// </summary>
	/// <remarks><para>This class is meant to be used for rendering from a pattern table to an
	/// NES screen image. Both images must be 8-bit. The source should use the first four
	/// colors of the paletteIndex and the destination should use sixteen. Blitting functions
	/// omittedBytes colors to use the appropriate four out of sixteen on the destination image.</para>
	/// <para>This class is designed to be highly optimized and does not perform
	/// any safety checks. Invalid or out of bounds values as well as incorrect image sizes
	/// and formats will produce irregular results and likely cause exceptions to be thrown.</para>
	/// </remarks>
	public class Blitter
	{
		Bitmap _Source, _Dest;
		BitmapData sourceLock = null;
		BitmapData destLock = null;
		Byte[] _sourceData, _destData = null;

		const int SourceScanWidth = 2048;
		const int DestScanWidth = 256;

        int destLockHeight = 0;

		/// <summary>
		/// Creates an instance of this class.
		/// </summary>
		public Blitter() {
		}

		/// <summary>
		/// Prepares for write-only blitting operations.
		/// </summary>
		/// <param name="source">The source image. This should be 2048 pixels wide and eight pixels tall.</param>
		/// <param name="dest">The destination image. This should be 256 pixels wide and tall.</param>
		public IDisposable Begin(Bitmap source, Bitmap dest) {
            _Source = source;
			_Dest = dest;

			if(sourceLock != null) throw new InvalidOperationException("Blitter already open");

            destLockHeight = Math.Min(0x100, dest.Height);
            sourceLock = _Source.LockBits(new Rectangle(0, 0, 0x800, 8), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
			destLock = _Dest.LockBits(new Rectangle(0, 0, 0x100, 0xF0), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

			if(_sourceData == null) _sourceData = new byte[0x4000];
			if(_destData == null) _destData = new byte[0x10000];

			// Copy source
			System.Runtime.InteropServices.Marshal.Copy(sourceLock.Scan0, _sourceData, 0, 0x4000);

            return new Blitter.BlitOperationScope(this);
		}


        /// <summary>
        /// Prepares for blitting operations that do not involve a source image.
        /// When the blitter is opened for blitting using this method, the behavior of 
        /// operations that require a source image are undefined.
        /// Supports read/write operations.
        /// </summary>
        /// <param name="dest">The destination image. This should be 256 pixels wide and tall.</param>
        public IDisposable Begin(Bitmap dest) {
            _Source = null;
            _Dest = dest;

            if(destLock != null) throw new InvalidOperationException("Blitter already open");

            destLockHeight = Math.Min(0x100, dest.Height);
            sourceLock = null;
            destLock = _Dest.LockBits(new Rectangle(0, 0, 0x100, 0xF0), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            _sourceData = null;
            if(_destData == null) _destData = new byte[0x10000];

            // Copy source
            System.Runtime.InteropServices.Marshal.Copy(destLock.Scan0, _destData, 0, 0x10000);

            return new Blitter.BlitOperationScope(this);
        }



		/// <summary>
		/// Finalizes the blitting operation
		/// </summary>
		public void End() {
			System.Runtime.InteropServices.Marshal.Copy(_destData, 0, destLock.Scan0, Math.Min(destLock.Stride * destLockHeight, _destData.Length));
			if(sourceLock != null) _Source.UnlockBits(sourceLock);
			_Dest.UnlockBits(destLock);

			sourceLock = null;
			destLock = null;
		}

		/// <summary>
		/// Selectes a different source for blitting without closing and opening the
		/// lock on the destination brush.
		/// </summary>
		/// <param name="newSource">The new source brush.</param>
		public void ChangeSource(Bitmap newSource) {
			if(sourceLock == null) throw new InvalidOperationException("Blitter not open");

			_Source.UnlockBits(sourceLock);
			_Source = newSource;
			sourceLock = _Source.LockBits(new Rectangle(0, 0, 0x800, 8), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

			System.Runtime.InteropServices.Marshal.Copy(sourceLock.Scan0, _sourceData, 0, 0x4000);
		}

		/// <summary>
		/// Performs a bit-block-transfer from the source brush to 
		/// the destination brush, applying the specified paletteIndex.
		/// </summary>
		/// <param name="tile">The count of the tile to be used from the source brush</param>
		/// <param name="destX">The x-coordinate of the tile to be drawn to in the destination</param>
		/// <param name="destY">The y-coordinate of the tile to be drawn to in the destination</param>
		/// <param name="pal">The paletteIndex to use.</param>
		/// <remarks>The paletteIndex specified will work for the range 0 through 31 if the source
		/// brush is formatted correctly, but for NES screen rendering typically only the first four
		/// palettes will be used.</remarks>
		public void BlitTile(int tile, int destX, int destY, byte pal) {
			int baseTileOffset = tile * 8;
			int destBaseOffset = destX * 8 + destY * DestScanWidth * 8;

			byte paldiff = (byte)(pal * 4);

			for(int row = 0; row < 8; row++) {
				int Screen = destBaseOffset + row * DestScanWidth;
				int Tile = baseTileOffset + row * SourceScanWidth;

				_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
			}
		}
		/// <summary>
		/// Performs a bit-block-transfer from the source brush to 
		/// the destination brush, applying the specified paletteIndex.
		/// Any pixels with a value of zero will be rendered transparently.
		/// </summary>
		/// <param name="tile">The count of the tile to be used from the source brush</param>
		/// <param name="destX">The x-coordinate of the tile to be drawn to in the destination</param>
		/// <param name="destY">The y-coordinate of the tile to be drawn to in the destination</param>
		/// <param name="pal">The paletteIndex to use.</param>
		/// <remarks>The paletteIndex specified will work for the range 0 through 31 if the source
		/// brush is formatted correctly, but for NES screen rendering typically only the first four
		/// palettes will be used.</remarks>
		public void BlitTileTransparent(int tile, int destX, int destY, byte pal) {
			int baseTileOffset = tile * 8;
			int destBaseOffset = destX * 8 + destY * DestScanWidth * 8;

			byte paldiff = (byte)(pal * 4);

			for(int row = 0; row < 8; row++) {
				int Screen = destBaseOffset + row * DestScanWidth;
				int Tile = baseTileOffset + row * SourceScanWidth;

				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
			}
		}


        /// <summary>
        /// Draws a 50% dither of the specified color on a 16x16 tile.
        /// </summary>
        /// <param name="x">The x-coordinate of a 16x16 tile.</param>
        /// <param name="y">The y-coordinate of a 16x16 tile.</param>
        /// <param name="color">The count of the color to draw with</param>
        /// <param name="parity">T value of 0 or 1 to indicate the parity of the dither.</param>
        public void DrawDither(int x, int y, byte color, int parity) {
            for(int row = y * 16; row < (y + 1) * 16; row++) {
                int startOffset = row * DestScanWidth + x * 16;
                int endOffset = startOffset + 16;

                for(int offset = startOffset; offset < endOffset; offset++) {
                    if(((offset + parity + row) & 1) == 1)
                        _destData[offset] = color;
                }
            }
        }

        /// <summary>
        /// Draws a 50% dither of the specified color on an 8x8 tile.
        /// </summary>
        /// <param name="x">The x-coordinate of an 8x8 tile.</param>
        /// <param name="y">The y-coordinate of an 8x8 tile.</param>
        /// <param name="color">The count of the color to draw with</param>
        public void DrawDitherTile(int x, int y, byte color) {
            for(int row = y * 8; row < (y + 1) * 8; row++) {
                int startOffset = row * DestScanWidth + x * 8;
                int endOffset = startOffset + 8;

                for(int offset = startOffset; offset < endOffset; offset++) {
                    if(((offset + row) & 1) == 1)
                        _destData[offset] = color;
                }
            }
        }

        public void DrawSlope_UR(int x, int y, byte color) {
            x *= 8;
            y *= 8;
            // Draw six pixels across the diagonal (corner pixels unaffected)
            for (int pixel = 1; pixel < 7; pixel++) {
                int offset = (8 - 1 - pixel + y) * DestScanWidth + x + pixel;
                _destData[offset] = color;
            }
        }
        public void DrawSlope_UL(int x, int y, byte color) {
            x *= 8;
            y *= 8;
            // Draw six pixels across the diagonal (corner pixels unaffected)
            for (int pixel = 1; pixel < 7; pixel++) {
                int offset = (pixel + y - 1) * DestScanWidth + x + pixel;
                _destData[offset] = color;
            }
        }

        internal void DrawSlope_Top(int x, int y, byte color) {
            x *= 8;
            y *= 8;
            // Draw six pixels across the diagonal (corner pixels unaffected)
            for (int pixel = 1; pixel < 7; pixel++) {
                int offset = (y + 6) * DestScanWidth + x + pixel;
                //int offset = (8 - pixel + y) * DestScanWidth + x + pixel;

                _destData[offset] = color;
            }
        }


        /// <summary>
        /// Fills a 16x16 tile with solid color
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="color">The count of the color to fill with.</param>
        public void FillTile(int x, int y, byte color) {
            for (int row = y * 16; row < (y + 1) * 16; row++) {
                int startOffset = row * DestScanWidth + x * 16;
                int endOffset = startOffset + 16;

                for (int offset = startOffset; offset < endOffset; offset++) {
                    _destData[offset] = color;
                }
            }

        }
        /// <summary>
        /// Fills a 16x16 tile with solid color
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="color">The count of the color to fill with.</param>
        public void FillTile8(int x, int y, byte color) {
            for (int row = y * 8; row < (y + 1) * 8; row++) {
                int startOffset = row * DestScanWidth + x * 8;
                int endOffset = startOffset + 8;

                for (int offset = startOffset; offset < endOffset; offset++) {
                    _destData[offset] = color;
                }
            }

        }
        /// <summary>
        /// Draws a 50% dither of the specified color.
        /// </summary>
        /// <param name="x">The x-coordinate of a 16x16 tile.</param>
        /// <param name="y">The y-coordinate of a 16x16 tile.</param>
        /// <param name="color">The count of the color to draw with</param>
        public void DrawDither(int x, int y, byte color) {
            DrawDither(x, y, color, 0);
        }

        /// <summary>
        /// Draws a rectangle to the destination.
        /// </summary>
        /// <param name="rect">The bounds of the rectangle.</param>
        /// <param name="color">The count of the color to use.</param>
        public void DrawRect(Rectangle rect, byte color) {
            // Calculate clipped line bounds
            int xstart = rect.X;
            if(xstart < 0) xstart = 0;
            if(xstart > OutputWidth) xstart = OutputWidth;
            int xend = rect.X + rect.Width;
            if(xend < 0) xend = 0;
            if(xend > OutputWidth) xend = OutputWidth;

            int ystart = rect.Y;
            if(ystart < 0) ystart = 0;
            if(ystart > OutputHeight) ystart = OutputHeight;
            int yend = rect.Y + rect.Height;
            if(yend < 0) yend = 0;
            if(yend > OutputHeight) yend = OutputHeight;

            // Top line
            if(rect.Y >= 0 && rect.Y < OutputHeight) { // Check if line is within bounds
                int startoffset = xstart + rect.Y * DestScanWidth; // Determine offsets in pixel data
                int endoffset = xend + rect.Y * DestScanWidth;
                for(int i = startoffset; i < endoffset; i++) {
                    _destData[i] = color; // Draw pixel
                }
            }
            // bottom line
            int bottom = rect.Bottom - 1;
            if(bottom >= 0 && bottom < OutputHeight) {
                int startoffset = xstart + bottom * DestScanWidth;
                int endoffset = xend + bottom * DestScanWidth;
                for(int i = startoffset; i < endoffset; i++) {
                    _destData[i] = color;
                }
            }
            // Left line
            if(rect.X >= 0 && rect.X < OutputWidth) {
                int startoffset = rect.X + ystart * DestScanWidth;
                int endoffset = rect.X + yend * DestScanWidth;
                for(int i = startoffset; i < endoffset; i += DestScanWidth) {
                    _destData[i] = color;
                }
            }
            // Right line
            int right = rect.Right - 1;
            if(right >= 0 && right < OutputWidth) {
                int startoffset = right + ystart * DestScanWidth;
                int endoffset = right + yend * DestScanWidth;
                for(int i = startoffset; i < endoffset; i += DestScanWidth) {
                    _destData[i] = color;
                }
            }
            

        }

        /// <summary>The x of the output brush</summary>
        public static int OutputWidth = 255;
        /// <summary>The height of the output brush</summary>
        public static int OutputHeight = 240;

		/// <summary>
		/// Performs a bit-block-transfer from the source brush to 
		/// the destination brush, applying the specified paletteIndex.
		/// Any pixels with a value of zero will be rendered transparently.
		/// </summary>
		/// <param name="tile">The count of the tile to be used from the source brush</param>
		/// <param name="destX">The x-coordinate of the tile to be drawn to in the destination</param>
		/// <param name="destY">The y-coordinate of the tile to be drawn to in the destination</param>
		/// <param name="pal">The paletteIndex to use.</param>
		/// <remarks>The paletteIndex specified will work for the range 0 through 31 if the source
		/// brush is formatted correctly, but for NES screen rendering typically only the first four
		/// palettes will be used.</remarks>
        /// <param name="flip">T FlipFlags value indicating how to flip the tile.</param>
        public void BlitTileTransparent(int tile, int destX, int destY, byte pal, FlipFlags flip) {
			int baseTileOffset = tile * 8;
			int destBaseOffset = destX * 8 + destY * DestScanWidth * 8;

			int screenInc = 1;
			int screenAdj = 0;
			if((flip & FlipFlags.Horizontal) == FlipFlags.Horizontal) {
				screenInc = -1;
				screenAdj = 7;
			}

			byte paldiff = (byte)(pal * 4);

			for(int row = 0; row < 8; row++) {
				int screenDataOffset = destBaseOffset + row * DestScanWidth + screenAdj;
				int tileDataOffset = baseTileOffset + row * SourceScanWidth; // Tile pixel data offset

				// Render a single pixel (this code is repeated eight
                // times instead using a loop as a speed optimization).
                if(_sourceData[tileDataOffset] != 0) // If pixel is not transparent
					_destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff); // render it
                // Seek to next pixel
                screenDataOffset += screenInc; 
				tileDataOffset++;

				if(_sourceData[tileDataOffset] != 0)
					_destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
				screenDataOffset += screenInc;
				tileDataOffset++;
				if(_sourceData[tileDataOffset] != 0)
					_destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
				screenDataOffset += screenInc;
				tileDataOffset++;
				if(_sourceData[tileDataOffset] != 0)
					_destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
				screenDataOffset += screenInc;
				tileDataOffset++;
				if(_sourceData[tileDataOffset] != 0)
					_destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
				screenDataOffset += screenInc;
				tileDataOffset++;
				if(_sourceData[tileDataOffset] != 0)
					_destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
				screenDataOffset += screenInc;
				tileDataOffset++;
				if(_sourceData[tileDataOffset] != 0)
					_destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
				screenDataOffset += screenInc;
				tileDataOffset++;
				if(_sourceData[tileDataOffset] != 0)
					_destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
			}
		}

        /// <summary>
        /// Performs a bit-block-transfer from the source brush to 
        /// the destination brush, applying the specified paletteIndex.
        /// Any pixels with a value of zero will be rendered transparently.
        /// This overload specifies a pixel location instead of a tile location.
        /// </summary>
        /// <param name="tile">The count of the tile to be used from the source brush</param>
        /// <param name="destX">The x-coordinate to draw to.</param>
        /// <param name="destY">The y-coordinate to draw to.</param>
        /// <param name="pal">The paletteIndex to use.</param>
        /// <remarks>The paletteIndex specified will work for the range 0 through 31 if the source
        /// brush is formatted correctly, but for NES screen rendering typically only the first four
        /// palettes will be used.</remarks>
        /// <param name="flip">T FlipFlags value indicating how to flip the tile.</param>
        public void BlitTileTransparent_Pixel(int tile, int destX, int destY, byte pal, FlipFlags flip) {
            int baseTileOffset = tile * 8;
            int destBaseOffset = destX + destY * DestScanWidth;

            int screenInc = 1;
            int screenAdj = 0;
            if((flip & FlipFlags.Horizontal) == FlipFlags.Horizontal) {
                screenInc = -1;
                screenAdj = 7;
            }

            byte paldiff = (byte)(pal * 4);


            for(int row = 0; row <8; row++) {
                int screenDataOffset = destBaseOffset + row * DestScanWidth + screenAdj;
                int tileDataOffset;
                if((flip & FlipFlags.Vertical) == FlipFlags.Vertical) {
                    tileDataOffset = baseTileOffset + (7 - row) * SourceScanWidth; // Tile pixel data offset
                } else {
                    tileDataOffset = baseTileOffset + row * SourceScanWidth; // Tile pixel data offset
                }
                // Render a single pixel (this code is repeated eight
                // times instead using a loop as a speed optimization).
                if(_sourceData[tileDataOffset] != 0) // If pixel is not transparent
                    _destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff); // render it
                // Seek to next pixel
                screenDataOffset += screenInc;
                tileDataOffset++;

                if(_sourceData[tileDataOffset] != 0)
                    _destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
                screenDataOffset += screenInc;
                tileDataOffset++;
                if(_sourceData[tileDataOffset] != 0)
                    _destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
                screenDataOffset += screenInc;
                tileDataOffset++;
                if(_sourceData[tileDataOffset] != 0)
                    _destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
                screenDataOffset += screenInc;
                tileDataOffset++;
                if(_sourceData[tileDataOffset] != 0)
                    _destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
                screenDataOffset += screenInc;
                tileDataOffset++;
                if(_sourceData[tileDataOffset] != 0)
                    _destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
                screenDataOffset += screenInc;
                tileDataOffset++;
                if(_sourceData[tileDataOffset] != 0)
                    _destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
                screenDataOffset += screenInc;
                tileDataOffset++;
                if(_sourceData[tileDataOffset] != 0)
                    _destData[screenDataOffset] = (byte)(_sourceData[tileDataOffset] | paldiff);
            }
        }

		/// <summary>
		/// Performs a bit-block-transfer from the source brush to 
		/// the destination brush, applying the specified paletteIndex.
		/// Any pixels with a value of zero will be rendered transparently.
		/// </summary>
		/// <param name="tile">The count of the tile to be used from the source brush</param>
		/// <param name="destX">The x-coordinate of the tile to be drawn to in the destination</param>
		/// <param name="destY">The y-coordinate of the tile to be drawn to in the destination</param>
		/// <param name="pal">The paletteIndex to use.</param>
		/// <param name="flip">Indicates how the tile should be flipped</param>
		/// <remarks>The paletteIndex specified will work for the range 0 through 31 if the source
		/// brush is formatted correctly, but for NES screen rendering typically only the first four
		/// palettes will be used.</remarks>
		public void BlitTileTransparentFlipped(int tile, int destX, int destY, byte pal, FlipFlags flip) {
			switch(flip) {
				case FlipFlags.Horizontal:
					BltTransFlipH(tile, destX, destY, pal);
					break;
				case FlipFlags.Vertical:
					BltTransFlipV(tile, destX, destY, pal);
					break;
				case (FlipFlags.Horizontal | FlipFlags.Vertical):
					BltTransFlipHV(tile, destX, destY, pal);
					break;
				default:
					BlitTileTransparent(tile, destX, destY, pal);
					break;
			}
		}
		private void BltTransFlipV(int tile, int destX, int destY, byte pal) {
			int baseTileOffset = tile * 8;
			int destBaseOffset = destX * 8 + destY * DestScanWidth * 8;

			byte paldiff = (byte)(pal * 4);

			for(int row = 0; row < 8; row++) {
				int Screen = destBaseOffset + (7 - row) * DestScanWidth;
				int Tile = baseTileOffset + row * SourceScanWidth;

				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen++;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
			}
		}
		private void BltTransFlipHV(int tile, int destX, int destY, byte pal) {
			int baseTileOffset = tile * 8;
			int destBaseOffset = destX * 8 + destY * DestScanWidth * 8;

			byte paldiff = (byte)(pal * 4);

			for(int row = 0; row < 8; row++) {
				int Screen = destBaseOffset + 7 + (7 - row) * DestScanWidth;
				int Tile = baseTileOffset + row * SourceScanWidth;

				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
			}
		}
		private void BltTransFlipH(int tile, int destX, int destY, byte pal) {
			int baseTileOffset = tile * 8;
			int destBaseOffset = destX * 8 + destY * DestScanWidth * 8;

			byte paldiff = (byte)(pal * 4);

			for(int row = 0; row < 8; row++) {
				int Screen = destBaseOffset + 7 + row * DestScanWidth;
				int Tile = baseTileOffset + row * SourceScanWidth;

				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
				Screen--;
				Tile++;
				if(_sourceData[Tile] != 0)
					_destData[Screen] = (byte)(_sourceData[Tile] | paldiff);
			}
		}



		/// <summary>
		/// Specifies flipping to be used when blitting a tile.
		/// </summary>
		/// <remarks>These values can be combined.</remarks>
		[Flags]
		public enum FlipFlags
		{
			/// <summary>Specifies horizontal flipping.</summary>
			Horizontal = 1,
			/// <summary>Specifies vertical flipping.</summary>
			Vertical = 2
		}

        class BlitOperationScope:IDisposable
        {
            Blitter b;

            public BlitOperationScope(Blitter b) {
                this.b = b;
            }
            public void Dispose() {
                b.End();
            }
        }

        internal void Clear() {
            Array.Clear(_destData, 0, _destData.Length);
        }

    }

}

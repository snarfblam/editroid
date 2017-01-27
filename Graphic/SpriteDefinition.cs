using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Editroid.Graphic
{
	/// <summary>
	/// Contains a definition of an image composed of graphic tiles
	/// </summary>
	public class SpriteDefinition
	{
		byte[] data;
		private int _Width = 2;
		/// <summary>
		/// Gets/sets the x of this sprite in tiles (eight pixel units)
		/// </summary>
        public int Width { get { return 2; } set { _Width = value; } }
        //public int Width { get { return _Width; } set { _Width = value; } }
		private int _Height = 2;
		/// <summary>
		/// Gets/sets the height of this sprite in tiles (eight pixel units)
		/// </summary>
		public int Height { get { return _Height; } set { _Height = value; } }
	

		/// <summary>
		/// Creates a sprite from the specified sprite data
		/// </summary>
		/// <param name="spriteData">Data used for sprite</param>
		public SpriteDefinition(byte[] spriteData) {
			data = spriteData;
            Rectangle r = Measure();
            _Width = r.Right / 8;
            _Height = r.Bottom / 8;
		}
		/// <summary>
		/// Creates a sprite from the specified sprite data
		/// </summary>
		/// <param name="spriteData">Data used for sprite</param>
		/// <param name="x">Width of sprite in tiles (eight pixel units)</param>
		/// <param name="height">Height of sprite in tiles (eight pixel units)</param>
		public SpriteDefinition(byte[] spriteData, int width, int height) {
			data = spriteData;
			_Width = width;
			_Height = height;
		}

		/// <summary>
		/// Creates an array of enemies from an array of sprite data arrays.
		/// </summary>
        /// <param name="data">Array of sprite data arrays.</param>
        /// <param name="sizes">SpriteDefinition sizes</param>
		/// <returns>An array of enemies.</returns>
		public static SpriteDefinition[] FromArray(byte[][] data, Size[] sizes){
			SpriteDefinition[] sprites = new SpriteDefinition[data.Length];
			bool noSizesSpecified = sizes == null;

            for (int i = 0; i < data.Length; i++) {
                if (data[i] == null) {
                    sprites[i] = LevelSprites.invalidEnemy;
                } else {
                    if (noSizesSpecified || sizes.Length <= i)
                        sprites[i] = new SpriteDefinition(data[i], 2, 2);
                    else
                        sprites[i] = new SpriteDefinition(data[i], sizes[i].Width, sizes[i].Height);
                }
            }

			return sprites;
		}

        /// <summary>
        /// Draws a sprite using a blitter.
        /// </summary>
        /// <param name="b_UTF32">The blitter to use to render the sprite.</param>
        /// <param name="xTile">The x-coordinate of the sprite (in 8x8 tiles).</param>
        /// <param name="yTile">The y-coordinate of the sprite (in 8x8 tiles).</param>
        /// <param name="pal">The number of the paletteIndex to render the sprite with.</param>
        /// <remarks>The blitter must be open for rendering before this function
        /// can be called.</remarks>
        public void Draw(Blitter b, int xTile, int yTile, byte pal) {
            int row = 0;
            int destTileX = xTile;
            int destTileY = yTile;
            Blitter.FlipFlags flip = 0;
            byte palette = pal;
            int Offset = 0;
            int offsetX = 0;
            int offsetY = 0;

            for(int i = 0; i < data.Length; i++) {
                palette = pal;
                if(data[i] >= 0xB0 && data[i] <= 0xBF) { // If data is macro, intepret
                    switch(data[i]) {
                        case macros.FlipX:
                            flip |= Blitter.FlipFlags.Horizontal;
                            break;
                        case macros.FlipY:
                            flip |= Blitter.FlipFlags.Vertical;
                            break;
                        case macros.D4:
                            Offset += 1024;
                            offsetY += 4;
                            break;
                        case macros.U4:
                            Offset -= 1024;
                            offsetY -= 4;
                            break;
                        case macros.L4:
                            Offset -= 4;
                            offsetX -= 4;
                            break;
                        case macros.R4:
                            Offset += 4;
                            offsetX += 4;
                            break;
                        case macros.D1:
                            Offset += 256;
                            offsetY += 1;
                            break;
                        case macros.U1:
                            Offset -= 256;
                            offsetY -= 1;
                            break;
                        case macros.L1:
                            Offset -= 1;
                            offsetX -= 1;
                            break;
                        case macros.R1:
                            Offset += 1;
                            offsetX += 1;
                            break;
                        case macros.NextPal:
                            palette = (byte)((palette + 1) % 4);
                            break;
                        case macros.NextRow:
                            row++;
                            Offset = 0;
                            destTileX = xTile;
                            destTileY = yTile + row;
                            offsetX = offsetY = 0;
                            break;
                    }
                } else { // if not a macro, it is a tile. Render.
                    if((destTileX * 8 + offsetX) >= 0 &&
                        (destTileX * 8 + offsetX + 7) < 256 &&
                        (destTileY * 8 + offsetY) >= 0 &&
                        (destTileY * 8 + offsetY + 7) < 240) {

                        b.BlitTileTransparent_Pixel(data[i], destTileX * 8 + offsetX, destTileY * 8 + offsetY, palette, flip); //, Offset);
                        destTileX++;
                    }
                    flip = 0;
                    palette = pal;
                }
            }
        }

        /// <summary>
        /// Measures a sprite.
        /// </summary>
        /// <returns>Rectangle that represents the bounds of a sprite
        /// in pixelsrelative to its zero coordinate.</returns>
        public Rectangle Measure() {
            int row = 0;
            int tileX = 0;
            int tileY = 0;
            Rectangle result = new Rectangle();

            for(int i = 0; i < data.Length; i++) {
                if(data[i] >= 0xB0 && data[i] <= 0xBF) { // If data is macro, intepret
                    switch(data[i]) {
                        case macros.FlipX:
                            break;
                        case macros.FlipY:
                            break;
                        case macros.D4:
                            tileY += 4;
                            break;
                        case macros.U4:
                            tileY -= 4;
                            break;
                        case macros.L4:
                            tileX -= 4;
                            break;
                        case macros.R4:
                            tileX += 4;
                            break;
                        case macros.NextPal:
                            break;
                        case macros.NextRow:
                            row++;
                            tileX = 0;
                            tileY = row * 8;
                            break;
                    }
                } else { // if not a macro, it is a tile. Render.
                    // Expand bounding rectangle to include rendered tile
                    if(tileX < result.Left) {
                        result.Width += result.Left - tileX;
                        result.X = tileX;
                    }
                    if(tileX + 8 > result.Right) {
                        result.Width += tileX + 8 - result.Right;
                    }
                    if(tileY < result.Top) {
                        result.Height += result.Top- tileY;
                        result.Y = tileY;
                    }
                    if(tileY + 8 > result.Bottom) {
                        result.Height += tileY + 8 - result.Bottom;
                    }

                    tileX += 8;
                }
            }

            return result;
        }


        /// <summary>
        /// Returns true if this tile has no data.
        /// </summary>
        public bool IsBlank { get { return this.data.Length == 0; } }

        /// <summary>Returns true if this sprite is the invalid enemy sprite.</summary>
        public bool IsInvalid { get { return this == LevelSprites.invalidEnemy; } }



	}



	/// <summary>
	/// Enumerates special values that can be used in sprite data
	/// </summary>
	internal static class macros
	{
		/// <summary>Tiles drawing will move down one row and back to origional x.
		/// This undoes any Move macros.</summary>
		public const byte NextRow = 0xB0;
		/// <summary>Next tile will be flipped horizontally.</summary>
		public const byte FlipX = 0xB1;
		/// <summary>Next tile will be flipped vertically.</summary>
		public const byte FlipY = 0xB2;
		/// <summary>Tiles will be drawn four pixels to the right.</summary>
        public const byte R4 = 0xB3;
        /// <summary>Tiles will be drawn four pixels to the left.</summary>
        public const byte L4 = 0xB4;
        /// <summary>Tiles will be drawn four pixels up.</summary>
        public const byte U4 = 0xB5;
        /// <summary>Tiles will be drawn four pixels down.</summary>
        public const byte D4 = 0xB6;
        /// <summary>Tiles will be drawn a pixel to the right.</summary>
        public const byte R1 = 0xB8;
        /// <summary>Tiles will be drawn a pixel to the left.</summary>
        public const byte L1 = 0xB9;
        /// <summary>Tiles will be drawn a pixel up.</summary>
        public const byte U1 = 0xBA;
        /// <summary>Tiles will be drawn a pixel down.</summary>
        public const byte D1 = 0xBB;
		/// <summary>Tile will be rendered with next paletteIndex.</summary>
		public const byte NextPal = 0xB7;
	}

}
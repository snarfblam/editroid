using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Editroid.Graphic
{
    public static class LevelSprites
    {
        /// <summary>
        /// Gets sprite definitions based on the specified currentLevelIndex.
        /// </summary>
        /// <param name="currentLevelIndex">The currentLevelIndex to get sprite definitions for.</param>
        /// <returns>An array of enemies.</returns>
        public static SpriteDefinition[] GetSprites(LevelIndex level) {
            switch (level) {
                case LevelIndex.Brinstar:
                    return BrinstarSprites;
                case LevelIndex.Kraid:
                    return KraidSprites;
                case LevelIndex.Ridley:
                    return RidleySprites;
                case LevelIndex.Norfair:
                    return NorfairSprites;
                case LevelIndex.Tourian:
                    return TourianSprites;
                default:
                    throw new ArgumentException("Specified level is invalid.", "level");
            }
        }


        public static readonly SpriteDefinition invalidEnemy = new SpriteDefinition(new byte[] { macros.R4, macros.D4, 0x6E }, 2, 2);
        
        /// <summary>
		/// Enemy enemies used in Brinstar.
		/// </summary>
		static public SpriteDefinition[] BrinstarSprites = SpriteDefinition.FromArray(new byte[][]{
			null,
			null,
			new byte[]{0xD0, 0xD1},
			new byte[]{0xC0, 0xC1},
			new byte[]{macros.R4,macros.R4, 0xCE, macros.NextRow,
						macros.R4,0xDE, 0xDF, macros.NextRow,
						macros.R4,0xEE, macros.FlipX, 0xEE},
			new byte[]{0xCC, 0xCD, macros.NextRow, 0xDC, 0xDD},
			new byte[]{macros.L4, macros.U4, 0xE4, macros.D4, 0xE2, macros.U4, macros.FlipX, 0xE4, macros.NextRow,
						0xE3, macros.FlipX, 0xE3},
			new byte[]{macros.L4, macros.L4, 0xC2, 0xC3, macros.NextRow, macros.L4, macros.L4, 0xD2, 0xD3},
            null, null, null, null, null, null, null, null,  
		}, new Size[]{
			new Size(0,0),
			new Size(0,0),
			new Size(2,1),
			new Size(2,1),
			new Size(2,3),
			new Size(2,2),
			new Size(2,2),
			new Size(2,2),
			});
		/// <summary>
		/// Enemy enemies used in Norfair.
		/// </summary>
		static public SpriteDefinition[] NorfairSprites = SpriteDefinition.FromArray(new byte[][]{
			new byte[]{macros.L4, 0xCE, 0xCF, macros.FlipX, 0xCE, macros.NextRow,
						0xDE, macros.FlipX, 0xDE},
			null,
			new byte[]{0xF6, 0xF7},
			null,
			null,
			null,
			new byte[]{0xCC, 0xCD, macros.NextRow, 0xDC, 0xDD},
			new byte[]{0xC2, 0xC3, macros.NextRow, 0xD2, 0xD3},
			null,
			null,
			null,
			new byte[]{0xEA, macros.FlipX, 0xEA, macros.NextRow,
						0xFA, macros.FlipX, 0xFA},
			new byte[]{0xEE, 0xEF, macros.NextRow,
						macros.FlipX, macros.FlipY, 0xEF, macros.FlipY, 0xEF},
			new byte[]{0xC8, 0xC9, macros.NextRow,
						0xD8, 0xD9, macros.NextRow,
						0xE8, 0xE9, macros.NextRow,
						0xF8, 0xF9, macros.NextRow},
			new byte[]{macros.R4, macros.R4, macros.R4,  0XC0},
            null,
		}, new Size[]{
			new Size(2, 2),
			new Size(0, 0),
			new Size(2, 1),
			new Size(0, 0),
			new Size(0, 0),
			new Size(0, 0),
			new Size(2, 2),
			new Size(2, 2),
			new Size(0, 0),
			new Size(0, 0),
			new Size(0, 0),
			new Size(2, 2),
			new Size(2, 2),
			new Size(2, 4),
			new Size(1, 1)
		});


		/// <summary>
		/// Enemy enemies used in Kraid's Hideout.
		/// </summary>
		static public SpriteDefinition[] KraidSprites = SpriteDefinition.FromArray(new byte[][]{
			new byte[]{macros.L4, 0xE2, 0xE3, macros.FlipX, 0xE2, macros.NextRow,
						macros.L4, 0xF2, 0xF3, macros.FlipX, 0xF2},
			new byte[]{macros.L4, macros.FlipY, 0xF2, macros.FlipY, 0xF3, macros.FlipY, macros.FlipX, 0xF2, macros.NextRow,
						macros.L4, macros.FlipY, 0xE2, macros.FlipY, 0xE3, macros.FlipY, macros.FlipX, 0xE2},
			null,
			new byte[]{0xC0, 0xC1},
			new byte[]{macros.R4,macros.R4, 0xCE, macros.NextRow,
						macros.R4,0xDE, 0xDF, macros.NextRow,
						macros.R4,0xEE, macros.FlipX, 0xEE},
			new byte[]{0xCC, 0xCD, macros.NextRow, 0xDC, 0xDD},
			null,
			new byte[]{macros.L4, macros.L4, 0xC2, 0xC3, macros.NextRow, macros.L4, macros.L4, 0xD2, 0xD3},
			new byte[]{0xC5, 0xC6, 0xC7, macros.NextRow,
						0xD5, 0xD6, 0xD7, macros.NextRow,
						0xE5, 0xE6, 0xE7, macros.NextRow,
						0xF5, 0xF6, 0xF7},
            null,null,null,null,null,null,null,
		}, new Size[] {
			new Size(2, 2),
			new Size(2, 2),
			new Size(0, 0),
			new Size(2, 1),
			new Size(2, 3),
			new Size(2, 2),
			new Size(0, 0),
			new Size(2, 2),
			new Size(3, 4)
		});
		/// <summary>
		/// Enemy enemies used in Ridley's Hideout.
		/// </summary>
		static public SpriteDefinition[] RidleySprites = SpriteDefinition.FromArray(new byte[][]{
			new byte[]{macros.L4, 0xCE, 0xCF, macros.FlipX, 0xCE, macros.NextRow,
						0xDE, macros.FlipX, 0xDE},
			null,
			// Check: these next two might be backwards order
			new byte[]{macros.L4, 0xE2, 0xE3, macros.FlipX, 0xE2, macros.NextRow,
						macros.L4, 0xF2, 0xF3, macros.FlipX, 0xF2},
			new byte[]{macros.L4, macros.FlipY, 0xF2, macros.FlipY, 0xF3, macros.FlipY, macros.FlipX, 0xF2, macros.NextRow,
						macros.L4, macros.FlipY, 0xE2, macros.FlipY, 0xE3, macros.FlipY, macros.FlipX, 0xE2},
			null,
			null,
			new byte[]{0xCC, 0xCD, macros.NextRow, 0xDC, 0xDD},
			new byte[]{0xC2, 0xC3, macros.NextRow, 0xD2, 0xD3},
			null,
			new byte[]{macros.U4, 0xC0, 0xC8, 0xC9, macros.NextRow,
						macros.U4, 0xC0, 0xC6, 0xC7, macros.NextRow,
						macros.U4, 0xC0, 0xD6, 0xD7, macros.NextRow,
						macros.U4, 0xE5, 0xE6, 0xE7, macros.NextRow,
						macros.U4, 0xF5, 0xF6, 0xF7, macros.NextRow,
			},
			null,
			null,
			new byte[]{0xEE, 0xEF, macros.NextRow,
						macros.FlipX, macros.FlipY, 0xEF, macros.FlipY, 0xEF},
			new byte[]{0xC8, 0xC9, macros.NextRow,
						0xD8, 0xD9, macros.NextRow,
						0xE8, 0xE9, macros.NextRow,
						0xF8, 0xF9, macros.NextRow},
			new byte[]{0X8F},
            null,
		}, new Size[] {
			new Size(2, 2),
			new Size(0, 0),
			new Size(2, 2),
			new Size(2, 2),
			new Size(0, 0),
			new Size(0, 0),
			new Size(2, 2),
			new Size(2, 2),
			new Size(0, 0),
			new Size(4, 5),
			new Size(0, 0),
			new Size(0, 0),
			new Size(2, 2),
			new Size(2, 2),
			new Size(2, 2)
		});

		/// <summary>
		/// Enemy enemies used in Tourian.
		/// </summary>
		static public SpriteDefinition[] TourianSprites = SpriteDefinition.FromArray(new byte[][]{
			new byte[]{0xC0, 0xC1, 0xC2, macros.NextRow,
						0xD0, 0xD1, 0xD2, macros.NextRow,
						0xE0, 0xE1, 0xE2, macros.NextRow},
			new byte[]{0xC3, 0xC4, 0xC5, macros.NextRow,
						0xD3, 0xD4, 0xD5, macros.NextRow,
						0xE3, 0xE4, 0xE5, macros.NextRow},
            null,null,null,null,null,null,null,null,null,null,null,null,null,null,
		}, new Size[]{
			new Size(3, 3),
			new Size(3, 3)
		});
    }
}

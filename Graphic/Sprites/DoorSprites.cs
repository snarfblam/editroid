using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.Graphic
{
    public static class DoorSprites
    {

        /// <summary>
        /// The sprite used for an opening door on the left edge of a screen.
        /// </summary>
        static public SpriteDefinition LeftDoor = new SpriteDefinition(new byte[] {
			0x0F, macros.NextRow,
			0x1F, macros.NextRow,
			0x2F, macros.NextRow,
			macros.FlipY, 0x2F, macros.NextRow,
			macros.FlipY, 0x1F, macros.NextRow,
			macros.FlipY, 0x0F
		}, 1, 6);

        /// <summary>
        /// The sprite used for an opening door on the right edge of a screen.
        /// </summary>
        static public SpriteDefinition RightDoor = new SpriteDefinition(new byte[] {
			macros.FlipX, 0x0F, macros.NextRow,
			macros.FlipX, 0x1F, macros.NextRow,
			macros.FlipX, 0x2F, macros.NextRow,
			macros.FlipX, macros.FlipY, 0x2F, macros.NextRow,
			macros.FlipX, macros.FlipY, 0x1F, macros.NextRow,
			macros.FlipX, macros.FlipY, 0x0F
		}, 1, 6);
        /// <summary>
        /// The sprite used for an opening door on the left edge of a screen.
        /// </summary>
        static public SpriteDefinition LeftDoorMusic = new SpriteDefinition(new byte[] {
			0x0F, macros.NextRow,
			0x1F, macros.NextRow,
			0x38, macros.NextRow,
			macros.FlipY, 0x38, macros.NextRow,
			macros.FlipY, 0x1F, macros.NextRow,
			macros.FlipY, 0x0F
		}, 1, 6);

        /// <summary>
        /// The sprite used for an opening door on the right edge of a screen.
        /// </summary>
        static public SpriteDefinition RightDoorMusic = new SpriteDefinition(new byte[] {
			macros.FlipX, 0x0F, macros.NextRow,
			macros.FlipX, 0x1F, macros.NextRow,
			macros.FlipX, 0x38, macros.NextRow,
			macros.FlipX, macros.FlipY, 0x38, macros.NextRow,
			macros.FlipX, macros.FlipY, 0x1F, macros.NextRow,
			macros.FlipX, macros.FlipY, 0x0F
		}, 1, 6);

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;

namespace Editroid.Graphic
{
    /// <summary>
    /// Sprites used for power-ups.
    /// </summary>
    public static class ItemSprites
    {
        /// <summary>SpriteDefinition for elevators.</summary>
        public static SpriteDefinition Elevator = new SpriteDefinition(new byte[]{macros.R4, 0x28, 0x38, macros.FlipX, 0x28}, 4,1);
        
        /// <summary>SpriteDefinition for a paletteIndex switch.</summary>
        public static SpriteDefinition PaletteSwitch = new SpriteDefinition(new byte[] { 0x6E, 0x6F, macros.NextRow, 0x6F, 0x6E });
        
        /// <summary>SpriteDefinition for an enemy.</summary>
        public static SpriteDefinition Enemy = new SpriteDefinition(new byte[] { 0xCC, 0xCD, macros.NextRow, 0xDC, 0xDD });
        
        /// <summary>SpriteDefinition for the mother brain single-byte-item.</summary>
        public static SpriteDefinition MotherBrain = new SpriteDefinition(new byte[] { 
            macros.R4, 0xCA, 0xCB, 0xCC, macros.NextRow,
            macros.R4, 0xDA, 0xDB, 0xDC, macros.NextRow,
            0xD8, 0xD9, 0xDB, 0xDB, 0xCE, macros.NextRow,
            0xE8, 0xE9, 0xDB, 0xDB, 0xDE, macros.NextRow,
            0xF8, 0xF9, 0xDB, 0xDB, 0xEE,
        });

        /// <summary>SpriteDefinition for bridge statues.</summary>
        public static SpriteDefinition AccessBridgeStatues = new SpriteDefinition(new byte[] {
            0xC8, 0xC9, macros.NextRow,
            0xD8, 0xD9, macros.NextRow,
            0xE8, 0xE9, macros.NextRow,
            0xFA, 0xFB, macros.NextRow,
            0xFF, 0xFF, 0xFF, macros.R4, 0xC5, 0xC6, 0xC7, macros.NextRow,
            0xFF, 0xFF, 0xFF, macros.R4, 0xD5, 0xD6, 0xD7, macros.NextRow,
            0xFF, 0xFF, 0xFF, macros.R4, 0xE5, 0xE6, 0xE7, macros.NextRow,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFA, 0xFB,
        });

        /// <summary>SpriteDefinition for rinka.</summary>
        public static SpriteDefinition Rinka = new SpriteDefinition(new byte[] {
            0xCF, macros.FlipX, 0xCF, macros.NextRow,
            macros.FlipY, 0xCF,macros.FlipY,macros.FlipX,0xCF,
        });

        /// <summary>SpriteDefinition for mella.</summary>
        public static SpriteDefinition Mella = new SpriteDefinition(new byte[] {
            0xED, macros.FlipX, 0xED
        });

        public static SpriteDefinition Turret1 = new SpriteDefinition(new byte[]{
            macros.U4,macros.U4,macros.L4,macros.L4,0xEB,macros.R4,0xEA,macros.NextRow,
            macros.L4,macros.L4,macros.U4,0xEC,macros.NextRow,
            macros.U1,macros.U1,macros.L4,macros.L4,macros.FlipY,0xEB,macros.R4, 0xEA,
        }, 1, 1);
        public static SpriteDefinition Turret2 = new SpriteDefinition(new byte[]{
            macros.U4,macros.U4,macros.R4,0xEA, macros.R4,macros.FlipX , macros.L1,macros.L1, 0xEB,macros.NextRow,
            0xFF,macros.U4,macros.R4,macros.R1,0xEC,macros.NextRow,
            macros.U1,macros.U1,macros.R4,macros.FlipY,0xEA,macros.R4,macros.L1,macros.L1,macros.FlipX,macros.FlipY,0xEB,
        }, 1, 1);
        public static SpriteDefinition Turret3 = new SpriteDefinition(new byte[]{
            macros.NextRow,
            macros.L4,macros.L4,macros.U4,0xEC,macros.R4,macros.R1,0xFF,0xEC,macros.NextRow,
            macros.U1,macros.U1, macros.L4,macros.L4,macros.FlipY,0xEB,macros.R4, 0xEA,macros.R1,macros.R1,macros.FlipX,macros.FlipY,0xEB
        }, 1, 1);
        public static SpriteDefinition Turret4 = new SpriteDefinition(new byte[]{
            macros.NextRow,
            macros.L4,macros.L4,macros.U4,0xEC,macros.NextRow,
            macros.U1,macros.U1, macros.L4,macros.L4,macros.FlipY,0xEB,macros.R4, 0xEA,
        }, 1, 1);

        public static SpriteDefinition ZebetiteMarker = new SpriteDefinition(new byte[] {
            0xfa, macros.NextRow,
            0xfa, macros.NextRow,
            0xfa, macros.NextRow,
            0xfa, macros.NextRow,
        }, 1, 2);
    }

}

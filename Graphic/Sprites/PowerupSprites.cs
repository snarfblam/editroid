using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;

namespace Editroid.Graphic
{
    /// <summary>
    /// Defines enemies for power-ups.
    /// </summary>
    public static class PowerUpSprites
    {
        static SpriteDefinition invalid = new SpriteDefinition(new byte[] { macros.FlipX, 0x81, 0x81, macros.NextRow, macros.FlipX, macros.FlipY, 0x81, macros.FlipY, 0x81 });

        /// <summary>SpriteDefinition for the bombs.</summary>
        public static SpriteDefinition Bombs = new SpriteDefinition(new byte[] { 0x86, 0x87, macros.NextRow, 0x96, 0x97 });
        /// <summary>SpriteDefinition for high jump boots.</summary>
        public static SpriteDefinition HighJump = new SpriteDefinition(new byte[] { 0x7B, 0x7C, macros.NextRow, 0x8B, 0x8C });
        /// <summary>SpriteDefinition for the longbeam.</summary>
        public static SpriteDefinition LongBeam = new SpriteDefinition(new byte[] { 0x88, 0x67, macros.NextRow, 0x98, 0x99 });
        /// <summary>SpriteDefinition for the screw attack.</summary>
        public static SpriteDefinition ScrewAttack = new SpriteDefinition(new byte[] { 0x80, 0x81, macros.NextRow, 0x90, 0x91 });
        /// <summary>SpriteDefinition for the maru mari.</summary>
        public static SpriteDefinition MaruMari = new SpriteDefinition(new byte[] { 0x7D, 0x7E, macros.NextRow, 0x8D, 0x8E });
        /// <summary>SpriteDefinition for the varia suit.</summary>
        public static SpriteDefinition Varia = new SpriteDefinition(new byte[] { 0x82, 0x83, macros.NextRow, 0x92, 0x93 });
        /// <summary>SpriteDefinition for the wavebeam.</summary>
        public static SpriteDefinition WaveBeam = new SpriteDefinition(new byte[] { 0x88, 0x8A, macros.NextRow, 0x98, 0x99 });
        /// <summary>SpriteDefinition for the icebeam.</summary>
        public static SpriteDefinition IceBeam = new SpriteDefinition(new byte[] { 0x88, 0x68, macros.NextRow, 0x98, 0x99 });
        /// <summary>SpriteDefinition for an energy tank.</summary>
        public static SpriteDefinition EnergyTank = new SpriteDefinition(new byte[] { 0x84, 0x85, macros.NextRow, 0x94, 0x95 });
        /// <summary>SpriteDefinition for missiles.</summary>
        public static SpriteDefinition Missile = new SpriteDefinition(new byte[] { 0x3F, macros.FlipX, 0x3F, macros.NextRow, 0x4F, macros.FlipX, 0x4F });
        /// <summary>SpriteDefinition for dot missiles.</summary>
        public static SpriteDefinition MissileDot = invalid; //new SpriteDefinition(new byte[] { 0x3F, SpriteMacros.FlipX, 0x3F, SpriteMacros.NextRow, 0x4F, SpriteMacros.FlipX, 0x4F });
        /// <summary>SpriteDefinition for arrow missiles.</summary>
        public static SpriteDefinition MissileArrow = invalid; // new SpriteDefinition(new byte[] { 0x3F, SpriteMacros.FlipX, 0x3F, SpriteMacros.NextRow, 0x4F, SpriteMacros.FlipX, 0x4F });
        /// <summary>SpriteDefinition for unknow object.</summary>
        public static SpriteDefinition UnknownC = invalid; //new SpriteDefinition(new byte[] {});
        /// <summary>SpriteDefinition for unknow object.</summary>
        public static SpriteDefinition UnknownD = invalid; //new SpriteDefinition(new byte[] { });
        /// <summary>SpriteDefinition for unknow object.</summary>
        public static SpriteDefinition UnknownE = invalid; //new SpriteDefinition(new byte[] { });
        /// <summary>SpriteDefinition for unknow object.</summary>
        public static SpriteDefinition UnknownF = invalid; //new SpriteDefinition(new byte[] { });

        /// <summary>
        /// Gets the sprite for the specified power-up.
        /// </summary>
        /// <param name="item">The type of power-up to get a sprite for.</param>
        /// <returns>T SpriteDefinition object.</returns>
        public static SpriteDefinition GetSprite(PowerUpType powerup) {
            switch (powerup) {
                case PowerUpType.Bomb:
                    return Bombs;
                case PowerUpType.HiJump:
                    return HighJump;
                case PowerUpType.LongBeam:
                    return LongBeam;
                case PowerUpType.ScrewAttack:
                    return ScrewAttack;
                case PowerUpType.MaruMari:
                    return MaruMari;
                case PowerUpType.Varia:
                    return Varia;
                case PowerUpType.WaveBeam:
                    return WaveBeam;
                case PowerUpType.IceBeam:
                    return IceBeam;
                case PowerUpType.EnergyTank:
                    return EnergyTank;
                case PowerUpType.Missile:
                    return Missile;
                case PowerUpType.Invalid_Missile_Dot:
                    return MissileDot;
                case PowerUpType.Invalid_Missile_Arrow:
                    return MissileArrow;
                case PowerUpType.Unknown_c:
                    return UnknownC;
                case PowerUpType.Unknown_d:
                    return UnknownD;
                case PowerUpType.Unknown_e:
                    return UnknownE;
                default:
                    return UnknownF;
            }
        }
    }
}

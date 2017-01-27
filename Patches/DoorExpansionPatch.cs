using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;
using System.ComponentModel;
using Editroid.Asm;

namespace Editroid.Patches
{
    class DoorExpansionPatch:RomPatch
    {
        bool expando;
        public DoorExpansionPatch(bool expando) {
            NormalDoorTiles = HorizontalDoorTiles = 1;

            this.expando = expando;

            System.Drawing.Point p;
            System.Drawing.Pen pp;
        }

        public override string Description {
            get {
                return "Increases the number of tiles that act as door markers to allow for more elaborate doors. Door tiles will" +
                    "start at tile A0. Horizontal-door tiles will follow normal door tiles. Total door tiles should not exceed 60.";
            }
        }
        protected override void BeforePatchApplied() {
            Segments.Clear();

            const byte doorTilesStart = 0xA0;
            const byte doorTileMaximumCount = 0x60;
            // Maximum is exclusive
            byte doorTileMax = (byte)((NormalDoorTiles % doorTileMaximumCount) + doorTilesStart - 1);
            byte hdoorTileMax = (byte)(((NormalDoorTiles + HorizontalDoorTiles) % doorTileMaximumCount) + doorTilesStart - 1);

            int offset = 0x1e817;
            if (expando) offset += 0x20000;
            Segments.Add(new PatchSegment((pRom)offset,
                new byte[] {
                     (byte)Opcodes.cmp_im, doorTileMax, 
                     (byte)Opcodes.bcc, 0x06,
                     (byte)Opcodes.cmp_im, hdoorTileMax,
                     (byte)Opcodes.bcs, 0x04
                 }));
        }

        [Description("The number of tiles to use as normal door tiles.")]
        [DefaultValue((ushort)1)]
        public ushort NormalDoorTiles { get; set; }
        [Description("The number of tiles to use as horizontal-horizontal door tiles.")]
        [DefaultValue((ushort)1)]
        public ushort HorizontalDoorTiles { get; set; }


    }
}

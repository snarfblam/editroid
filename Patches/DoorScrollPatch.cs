using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;
using System.ComponentModel;

namespace Editroid.Patches
{
    class DoorScrollPatch:RomPatch
    {
        public DoorScrollPatch(bool expando) 
        :base(expando){
        }

        bool initialized = false;
        protected override void BeforePatchApplied() {
            base.BeforePatchApplied();

            if (!initialized) {
                if (Behavior == Result.NewBehavior) {
                    this.Segments.Add(new PatchSegment((pRom)ExpandoAdjust(0x1e247), new byte[] { 0xA0, 0x00 }));
                } else {
                    this.Segments.Add(new PatchSegment((pRom)ExpandoAdjust(0x1e247), new byte[] { 0xA4, 0x57 }));
                }
                initialized = true;
            }
        }

        [Description("Select normal behavior to revert to standard Metroid doors, or new behavior for alternate door behavior.")]
        public Result Behavior { get; set; }
        public enum Result
        {
            NewBehavior,
            NormalBehavior,
        }

        public override string Description {
            get {
                return "This patch causes doorways to work differently with scrolling-changes. Instead of swapping between horizontal and vertical scrolling, " +
                    "normal doorways will always set scrolling to vertical. The other doorway type still always sets scrolling to horizontal.";
            }
        }
    }
}

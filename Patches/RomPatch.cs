using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using Editroid.ROM;

namespace Editroid.Patches
{
    /// <summary>
    /// Base class for ROM patches that Editroid can apply to enhance a ROM.
    /// </summary>
    public class RomPatch
    {
        List<PatchSegment> segments = new List<PatchSegment>();
        
        protected const int expandoPrgOffset = 0x20000;
        protected bool Expando{get; private set;}
        public RomPatch() {
        }
        public RomPatch(bool expando) {
            this.Expando = expando;
        }
        /// <summary>
        /// Automatically adjusts ROM offsets that refer to the main PRG bank if
        /// the Expando property returns true.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected int ExpandoAdjust(int offset) {
            if (offset > 0x1C010) offset += expandoPrgOffset;
            return offset;
        }
                /// <summary>
        /// Automatically adjusts ROM offsets that refer to the main PRG bank if
        /// the Expando property returns true.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected pRom ExpandoAdjust(pRom offset) {
            if ((int)offset > 0x1C010) offset += expandoPrgOffset;
            return offset;

        }
        [Browsable(false)]
        protected IList<PatchSegment> Segments { get { return segments; } }

        /// <summary>
        /// Called immediately before a patch is applied.
        /// </summary>
        protected virtual void BeforePatchApplied() { }

        public void Apply(Stream s) {
            BeforePatchApplied();
            foreach (var segment in segments) {
                s.Seek(segment.TargetOffset, SeekOrigin.Begin);
                s.Write(segment.data, 0, segment.data.Length);
            }
        }

        [Browsable(false)]
        public virtual string Description { get { return "Modifies a ROM."; } }
    }
}

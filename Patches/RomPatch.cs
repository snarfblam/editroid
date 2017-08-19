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
        protected const int mmc3PrgOffset = 0x60000;
        //protected bool Expando{get; private set;}
        protected bool Expando { get { return RomFormat == RomFormats.Expando; } }
        public RomFormats RomFormat { get; private set; }
        public RomPatch() {

        }
        public RomPatch(RomFormats format) {
            RomFormat = format;
            //this.Expando = expando;
        }
        /// <summary>
        /// Automatically adjusts ROM offsets that refer to the main PRG bank if
        /// the Expando property returns true.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected int FormatAdjust(int offset) {
            if (offset > 0x1C010) {
                if (RomFormat == RomFormats.Expando) {
                    offset += expandoPrgOffset;
                } else {
                    offset += mmc3PrgOffset;
                }
            }
            return offset;
        }
                /// <summary>
        /// Automatically adjusts ROM offsets that refer to the main PRG bank if
        /// the Expando property returns true.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected pRom FormatAdjust(pRom offset) {
            if (offset > 0x1C010) {
                if (RomFormat == RomFormats.Expando) {
                    offset += expandoPrgOffset;
                } else {
                    offset += mmc3PrgOffset;
                }
            }
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

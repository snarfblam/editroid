using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;

namespace Editroid.Patches
{
    /// <summary>
    /// Represents a linear section of ROM data to patch. Data buffers should 
    /// not be used after being passed to the constructor.
    /// </summary>
    public class PatchSegment
    {
        public PatchSegment(pRom offset, byte[] data) {
            this.TargetOffset = offset;
            this.data = data;
        }
        /// <summary>
        /// Creates a patch segment with the same data at a different offset.
        /// </summary>
        public PatchSegment Duplicate(pRom offset) {
            return new PatchSegment(offset, this.data);
        }
        public pRom TargetOffset { get; private set; }
        public byte[] data { get; private set; }

    }
}

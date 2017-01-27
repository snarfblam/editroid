using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM.Formats
{
    public struct RomRange
    {
        public RomRange(int start, int len)
        :this(){
            this.Start = start;
            this.Length = len;
        }
        public int Start { get; private set; }
        public int Length { get; private set; }
    }
}

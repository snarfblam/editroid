using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    class Mmc3
    {
        public const int PrgBankSize = 0x2000;
        public const int ChrBankSize = 0x400;
        public const int ScreenDataBaseAddress = 0xA000;
        public const int StructDataBaseAddress = 0x8000;
        public const int BankCount = 0x40;

        public static pRom GetBankOffset(int index){
            return (pRom)(index * PrgBankSize + 0x10);
            
        }

        /// <summary>Returns the bank number the specified offset occurs in.</summary>
        internal static int GetBank(pRom offset) {
            if (offset < 0x10) throw new ArgumentException("The specified offset refers to a position before the beginning of the ROM.");
            return ((int)(offset - 0x10)) / PrgBankSize;
        }
        /// <summary>Gets the address that would be used to refer to the specified offset if its bank were loaded into the $8000-$9FFF address range</summary>
        internal static pCpu GetAddress8000(pRom offset) {
            if (offset < 0x10) throw new ArgumentException("The specified offset refers to a position before the beginning of the ROM.");
            return new pCpu((0x1FFF & (int)(offset - 0x10)) | 0x8000);
        }
    }
}

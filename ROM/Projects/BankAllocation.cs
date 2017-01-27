using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    public class BankAllocation
    {
        public const int BankAllocationAddress = 0x8800;
        public const int BankAllocationBank = 0xE;
        public const int BankAllocationOffset = BankAllocationBank * 0x4000 + 0x10 + (BankAllocationAddress & 0x3FFF);

        public BankAllocation(int number) {
            this.BankNumber = number;
        }
        public int BankNumber { get; set; }
        public bool Reserved { get; set; }
        public bool UserReserved { get; set; }
        public string Description { get; set; }

        static readonly byte[] header = System.Text.Encoding.ASCII.GetBytes("mmc3_allocation");
        public const int MMC3BankCount = 0x40;

        public static BankAllocation[] CreateNewAllocationTable() {
            BankAllocation[] result = new BankAllocation[MMC3BankCount];
            for (int i = 0; i < DefaultAllocation.Length; i++) {
                result[i] = (BankAllocation)DefaultAllocation[i].MemberwiseClone();
            }

            return result;
        }
        public static BankAllocation[] ReadAllocation(byte[] data, int offset) {
            var result = CreateNewAllocationTable();

            ValidateOffset(data, offset);

            if (CheckHeader(data, ref offset)) {
                // Header present, there is a table to load.
                for (int i = 0; i < result.Length; i++) {
                    var entry = (binaryAllocationValues)data[offset + i];

                    switch (entry) {
                        case binaryAllocationValues.ReservedByGame:
                            result[i].Reserved = true;
                            result[i].UserReserved = false;
                            break;
                        case binaryAllocationValues.ReservedByUser:
                            result[i].Reserved = false;
                            result[i].UserReserved = true;
                            break;
                        case binaryAllocationValues.Free:
                            result[i].Reserved = false;
                            result[i].UserReserved = false;
                            break;
                        case binaryAllocationValues.Unspecified:
                        default:
                            // unknown/unspecified values are left as default
                            break;
                    }
                }
            }

            return result;

        }


        public static void WriteAllocation(byte[] data, int offset, BankAllocation[] allocation) {
            if (allocation.Length != MMC3BankCount) throw new ArgumentException("Invalid number of banks in array", "allocation");
            ValidateOffset(data, offset);

            for (int i = 0; i < header.Length; i++) {
                data[offset + i] = header[i];
            }

            offset += header.Length;

            for (int i = 0; i < allocation.Length; i++) {
                var all = allocation[i];
                binaryAllocationValues value;
                if (all.Reserved) {
                    value = binaryAllocationValues.ReservedByGame;
                } else if (all.UserReserved) {
                    value = binaryAllocationValues.ReservedByUser;
                } else {
                    value = binaryAllocationValues.Free;
                }

                data[offset + i] = (byte)value;
            }
        }


        private static void ValidateOffset(byte[] data, int offset) {
            int dataSize = header.Length + MMC3BankCount;
            if (offset + dataSize > data.Length) throw new ArgumentException("Specified offset is out of bounds.");
        }

        /// <summary>
        /// Confirms that the bank allocation header is found at the specified offset. If found, the return value of offset will be the offset of the first byte after the header.
        /// </summary>
        private static bool CheckHeader(byte[] data, ref int offset) {
            for (int i = 0; i < header.Length; i++) {
                if (data[offset + i] != header[i]) return false;
            }

            offset += header.Length;
            return true;
        }


        enum binaryAllocationValues : byte
        {
            Unspecified = 0,
            ReservedByGame = 1,
            ReservedByUser = 3,
            Free = 4
        }


        public static BankAllocation[] DefaultAllocation = new BankAllocation[MMC3BankCount];
        static BankAllocation() {
            int i = 0;
            // 16KB banks 0-5 (title/levels)
            AddDefaultBank_Multi(ref i, 2, true, "Title/credits/password");
            AddDefaultBank_Multi(ref i, 2, true, "Brinstar");
            AddDefaultBank_Multi(ref i, 2, true, "Norfair");
            AddDefaultBank_Multi(ref i, 2, true, "Tourian");
            AddDefaultBank_Multi(ref i, 2, true, "Kraid");
            AddDefaultBank_Multi(ref i, 2, true, "Ridley");

            // 6, 7 Originally graphic/game engine banks
            AddDefaultBank_Multi(ref i, 4, false, "Available");

            // 8, 9, 10, 11, 12 Structure/combo/general asm banks (now halved, no longer contain room data). Upper half is now free.
            AddDefaultBank(ref i, true, "Brinstar extended");
            AddDefaultBank(ref i, false, "Available");
            AddDefaultBank(ref i, true, "Norfair extended");
            AddDefaultBank(ref i, false, "Available");
            AddDefaultBank(ref i, true, "Tourian extended");
            AddDefaultBank(ref i, false, "Available");
            AddDefaultBank(ref i, true, "Kraid extended");
            AddDefaultBank(ref i, false, "Available");
            AddDefaultBank(ref i, true, "Ridley extended");
            AddDefaultBank(ref i, false, "Available");

            // D - Previously unused
            AddDefaultBank_Multi(ref i, 2, false, "Available");
            // E - reserved for editor
            AddDefaultBank_Multi(ref i, 2, true, "Reserved for editor");
            // F - Expando/enhanco game engine bank (relocated to 1F)
            AddDefaultBank_Multi(ref i, 2, false, "Available");

            // 10 - 1D, 1E lower (15 new 16KB banks!!)
            AddDefaultBank_Multi(ref i, 29, false, "Available");
            // 1E high reserved for new code
            AddDefaultBank_Multi(ref i, 1, true, "Game engine extended");

            // 1F - new fixed bank
            AddDefaultBank_Multi(ref i, 2, true, "Game engine");
        }


        private static void AddDefaultBank_Multi(ref int i, int count, bool reserved, string desc) {
            for (int j = 0; j < count; j++) {
                AddDefaultBank(ref i, reserved, desc);
            }
        }

        private static void AddDefaultBank(ref int i, bool reserved, string desc) {
            DefaultAllocation[i] = new BankAllocation(i) { Reserved = reserved, Description = desc };
            i++;
        }
    }
}
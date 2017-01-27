using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    /// <summary>
    /// Maintains/modifies a list of screen indecies that have beep-boop-beep-boop (alternate) music for a level.
    /// </summary>
    public class AlternateMusicRooms
    {
        ////static int[] offsets = new int[] { 
        ////    0x55e0, // (1) Brinstar
        ////    0x95e0, // (2) Norfair
        ////    0xD5e0, // (3) Tourian
        ////    0x115e0, // (4) Kraid
        ////    0x155e0, // (5) Ridley
        ////};
        public const byte empty = 0xFF;
        public Level Level { get; private set; }
        int offset;
        MetroidRom rom;

        /// <summary>Creates and initializes an AlternateMusicRooms object.</summary>
        /// <param name="rom">The rom that contains the data.</param>
        /// <param name="level">The level this object represents.</param>
        public AlternateMusicRooms(MetroidRom rom, Level level) {
            this.rom = rom;
            ////this.level = level;
            this.Level = level;
            this.offset = level.Format.AltMusicOffset; // offsets[(int)Level.Index];
        }

        /// <summary>
        /// Gets/sets a value from this list.
        /// </summary>
        /// <param name="i">The index of the value.</param>
        /// <returns>The screen index.</returns>
        public byte this[int i] {
            get {
                ValidateIndex("i", i, 0, Level.Format.AltMusicRoomCount - 1);
                return rom.data[offset + i];
            }
            set {
                ValidateIndex("i", i, 0, Level.Format.AltMusicRoomCount - 1);
                rom.data[offset + i] = value;
            }
        }

        void ValidateIndex(string name, int val, int min, int max) {
            if (val < min || val > max)
                throw new ArithmeticException("The value " + val.ToString() + " is not valid for parameter " + name + ".");
        }

        /// <summary>
        /// The number of screens stored in this list.
        /// </summary>
        public int UsedEntryCount {
            get {
                int count = 0;
                for (int i = 0; i < Level.Format.AltMusicRoomCount; i++) {
                    if (this[i] != empty)
                        count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Tries to remove the specified value from the list.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>True if the value was present and removed, or false if the value was
        /// not found in the list.</returns>
        public bool Remove(byte value) {
            for (int i = 0; i < Level.Format.AltMusicRoomCount; i++) {
                if (this[i] == value) {
                    this[i] = empty;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the value at the specified index from the list.
        /// </summary>
        /// <param name="index">The index of the value to remove.</param>
        public void RemoveAt(int index) {
            this[index] = empty;
        }

        /// <summary>
        /// Tries to add the specified value to the list.
        /// </summary>
        /// <param name="value">The screen index to add.</param>
        /// <returns>True if the value was added, false if there was not enough space.</returns>
        public bool Add(byte value) {
            for (int i = 0; i < Level.Format.AltMusicRoomCount; i++) {
                if (this[i] == empty) {
                    this[i] = value;
                    return true;
                }
            }

            return false;
        }

        public bool Contians(byte value) {
            for (int i = 0; i < Level.Format.AltMusicRoomCount; i++) {
                if (this[i] == value) {
                    return true;
                }
            }

            return false;
        }
    }
}

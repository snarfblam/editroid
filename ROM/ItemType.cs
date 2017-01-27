using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    /// <summary>
    /// Contains data about different item types.
    /// </summary>
    public class ItemType
    {
        static public readonly ItemType Nothing = new ItemType(ItemTypeIndex.Nothing, 1);
        static public readonly ItemType Enemy = new ItemType(ItemTypeIndex.Enemy, 3);
        static public readonly ItemType PowerUp = new ItemType(ItemTypeIndex.PowerUp, 3);
        static public readonly ItemType Mella = new ItemType(ItemTypeIndex.Mella, 1);
        static public readonly ItemType Elevator = new ItemType(ItemTypeIndex.Elevator, 2);
        static public readonly ItemType Turret = new ItemType(ItemTypeIndex.Turret, 2);
        static public readonly ItemType MotherBrain = new ItemType(ItemTypeIndex.MotherBrain, 1);
        static public readonly ItemType Zebetite = new ItemType(ItemTypeIndex.Zebetite, 1);
        static public readonly ItemType Rinkas = new ItemType(ItemTypeIndex.Rinkas, 1);
        static public readonly ItemType Door = new ItemType(ItemTypeIndex.Door, 2);
        static public readonly ItemType PalSwap = new ItemType(ItemTypeIndex.PalSwap, 1);
        static public readonly ItemType Unused_b = new ItemType(ItemTypeIndex.Unused_b, 1);
        static public readonly ItemType Unused_c = new ItemType(ItemTypeIndex.Unused_c, 1);
        static public readonly ItemType Unused_d = new ItemType(ItemTypeIndex.Unused_d, 1);
        static public readonly ItemType Unused_e = new ItemType(ItemTypeIndex.Unused_e, 1);
        static public readonly ItemType Unused_f = new ItemType(ItemTypeIndex.Unused_f, 1);

        
        static ItemType[] types;
        static ItemType(){
            types = new ItemType[] { Nothing, Enemy, PowerUp, Mella, Elevator, Turret, MotherBrain, Zebetite, Rinkas, Door, PalSwap, Unused_b, Unused_c, Unused_d, Unused_e, Unused_f };
        }
        public static ItemType GetItemType(ItemTypeIndex index) {
            int i = (int)index;
            if (i < 0 || i > 0x0F) throw new ArgumentException("Invalid ItemTypeIndex");

            return types[i];
        }

        private ItemType(ItemTypeIndex index, int netByteCount) {
            this.Index = index;
            this.NetByteCount = netByteCount;
            this.Name = index.ToString();
        }

        public ItemTypeIndex Index { get; private set; }
        public int NetByteCount { get; private set; }
        public string Name { get; private set; }
    }

    public static class ItemTypeIndexExtensions
    {
        public static ItemType GetItemType(this ItemTypeIndex index){
            return ItemType.GetItemType(index);
        }
    }

    /// <summary>Represents the different types of item data.</summary>
    public enum ItemTypeIndex : byte
    {
        /// <summary>No gameItem.</summary>
        Nothing = 0,
        /// <summary>Enemy.</summary>
        Enemy = 1,
        /// <summary>Power up</summary>
        PowerUp = 2,
        /// <summary>Unknown.</summary>
        Mella = 3,
        /// <summary>Elevator.</summary>
        Elevator = 4,
        /// <summary>Unknown.</summary>
        Turret = 5,
        /// <summary>Bridge to Tourian elevator.</summary>
        MotherBrain = 6,
        /// <summary>Enemy that respawns.</summary>
        Zebetite = 7,
        /// <summary>Unknown.</summary>
        Rinkas = 8,
        /// <summary>Unknown.</summary>
        Door = 9,
        /// <summary>Palette swap.</summary>
        PalSwap = 0xA,
        /// <summary>Unknown.</summary>
        Unused_b = 0xB,
        /// <summary>Unknown.</summary>
        Unused_c = 0xC,
        /// <summary>Unknown.</summary>
        Unused_d = 0xD,
        /// <summary>Unknown.</summary>
        Unused_e = 0xE,
        /// <summary>Unknown.</summary>
        Unused_f = 0xF,
    }

}

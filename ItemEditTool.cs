using System;
using System.Collections.Generic;
using System.Text;
using Editroid.ROM;
using System.Drawing;

namespace Editroid
{
    abstract class ItemEditTool
    {
        public abstract string[] GetList();
        public abstract int GetIndex(ItemSeeker s);
        public abstract void SetIndex(ItemSeeker s, int i);
        ////public static ItemEditTool GetEditTool(ItemSeeker s) {
        ////    ItemTypeIndex itemType = (ItemTypeIndex)(s.Data[s.itemOffset] & 0x0F);
        ////    switch(itemType) {
        ////        case ItemTypeIndex.Elevator:
        ////            return elevator;
        ////        case ItemTypeIndex.Enemy:
        ////            return enemy;
        ////        case ItemTypeIndex.PowerUp:
        ////            return pow;
        ////        case ItemTypeIndex.Nothing:
        ////            break;
        ////        case ItemTypeIndex.Door:
        ////            return door;
        ////        case ItemTypeIndex.Mella:
        ////        case ItemTypeIndex.Zebetite:
        ////        case ItemTypeIndex.Rinkas:
        ////        case ItemTypeIndex.PalSwap:
        ////        case ItemTypeIndex.MotherBrain:
        ////            return singleByte;
        ////    }

        ////    return unkTool;
        ////}
        public static ItemEditTool GetEditTool(ItemData item) {
            if (item is ItemElevatorData)
                return elevator;
            if (item is ItemEnemyData)
                return enemy;
            if(item is ItemPowerupData)
                return pow;
            if (item is ItemDoorData)
                return door;
            if (item is ItemSingleByteData)
                return singleByte;
            return unkTool;
           
        }

        public abstract Image GetImage(ItemData s);

        static EnemyEditTool enemy = new EnemyEditTool();
        static PowerUpEditTool pow = new PowerUpEditTool();
        static ElevatorEditTool elevator = new ElevatorEditTool();
        static SingleByteEditTool singleByte = new SingleByteEditTool();
        static UnknowItemEditTool unkTool = new UnknowItemEditTool();
        static DoorEditTool door = new DoorEditTool();
    }

    class EnemyEditTool:ItemEditTool
    {
        static Image itemImage = ItemImages.Enemy;
        public override Image GetImage(ItemData s) {
            return itemImage;
        }
        static string[] List = { "enemy (0)", "enemy (1)", "enemy (2)", "enemy (3)", "enemy (4)", "enemy (5)", "enemy (6)", "enemy (7)", "enemy (8)", "enemy (9)", "enemy (A)", "enemy (B)", "enemy (C)", "enemy (D)", "enemy (E)", "enemy (F)" };
        public override string[] GetList() {
            return List;
        }

        public override int GetIndex(ItemSeeker s) {
            return s.Data[s.itemOffset + 1] & 0x0F;
        }

        public override void SetIndex(ItemSeeker s, int i) {
            s.Data[s.itemOffset + 1] = (byte)(
                (s.Data[s.itemOffset + 1] & 0xF0) |
                (i & 0x0F));
        }

        public int GetSlot(ItemSeeker s) {
            return s.Data[s.itemOffset] / 0x10;
        }
        public void SetSlot(ItemSeeker s, int i) {
            s.Data[s.itemOffset] = (byte)(
                (s.Data[s.itemOffset] & 0x0F) |
                (i * 0x10));
        }
        public bool GetHard(ItemSeeker s) {
            return s.Data[s.itemOffset + 1] >= 0x80;
        }
        public bool GetRespawn(ItemSeeker s) {
            return (s.Data[s.itemOffset] & 0x07) == 0x07;
        }
        public void SetHard(ItemSeeker s, bool hard) {
            if(hard)
                s.Data[s.itemOffset + 1] = (byte)(s.Data[s.itemOffset + 1] | 0x80);
            else
                s.Data[s.itemOffset + 1] = (byte)(s.Data[s.itemOffset + 1] & 0x7F);
        }
    }
    class PowerUpEditTool:ItemEditTool
    {
        static Image[] itemImages = { ItemImages.Bombs, ItemImages.Boot, ItemImages.Long, ItemImages.Screw, ItemImages.MaruMari, ItemImages.Varia, ItemImages.Wave, ItemImages.Ice, ItemImages.Energy, ItemImages.Missile };
        public override Image GetImage(ItemData item) {
            ////int i = GetIndex(s);
            ////if(i < itemImages.Length) return itemImages[i];
            var powItem = item as ItemPowerupData;
            if(powItem == null) return null;

            int imageIndex = (int)powItem.PowerUp;
            if (imageIndex >= itemImages.Length) return null;

            return itemImages[imageIndex];
        }

        static string[] Items = { "Bomb", "High Jump", "Long Beam", "Screw Attack", "Maru Mari", "Varia", "Wave Beam", "Ice Beam", "Energy Tank", "Missile", "(Invalid)", "(Invalid)", "(Invalid)", "(Invalid)", "(Invalid)", "(Invalid)" };
        public override string[] GetList() {
            return Items;
        }

        public override int GetIndex(ItemSeeker s) {
            return s.Data[s.itemOffset + 1] & 0x0F;
        }

        public override void SetIndex(ItemSeeker s, int i) {
            s.Data[s.itemOffset + 1] = (byte)(
                (s.Data[s.itemOffset + 1] & 0xF0) |
                i & 0x0F);
        }
    }
    class ElevatorEditTool:ItemEditTool
    {
        static Image itemImage = ItemImages.Elevator;
        public override Image GetImage(ItemData s) {
            return itemImage;
        }

        static string[] elevatorItems = { "Brinstar", "Norfair", "Kraid", "Tourian", "Ridley", "Norfair Exit", "Kraid Exit", "Tourian Exit", "Ridley Exit", "Complete Game" };

        public override string[] GetList() {
            return elevatorItems;
        }

        public override int GetIndex(ItemSeeker s) {
            int dest = s.Data[s.itemOffset + 1];

            if((dest & 0xF) == 0xF) dest -= 0xA; // Complete game
            if((dest & 0x80) == 0x80) dest += 4; // Exit
            dest = dest & 0x7F; // I forget why
            return dest;
        }

        public override void SetIndex(ItemSeeker s, int i) {
            if(i == 9)
                s.Data[s.itemOffset + 1] = (byte)(ElevatorDestination.EndOfGame);
            else if(i > 4) {
                i = (i - 4) | 0x80;
            }

            s.Data[s.itemOffset + 1] = (byte)i;
        }
    }
    class SingleByteEditTool:ItemEditTool
    {
        static Image[] itemImage = { ItemImages.Mella, ItemImages.Rinka, ItemImages.Pal,ItemImages.Misc };

        public override Image GetImage(ItemData s) {
            //return itemImage[GetIndex(s)];
            switch (s.ItemType) {
                case ItemTypeIndex.Mella:
                    return itemImage[0];
                case ItemTypeIndex.Rinkas:
                    return itemImage[1];
                case ItemTypeIndex.PalSwap:
                    return itemImage[2];
                case ItemTypeIndex.MotherBrain:
                case ItemTypeIndex.Zebetite:
                    return itemImage[3];
            }
            return null;
        }
        static string[] SingleByteItems = { "Mella", "Rinka", "Palette Swap" };

        public override string[] GetList() {
            return SingleByteItems;
        }

        public override int GetIndex(ItemSeeker s) {
            ItemTypeIndex i = (ItemTypeIndex)(s.Data[s.itemOffset] & 0x0F);
            if(i == ItemTypeIndex.Mella) return 0;
            if(i == ItemTypeIndex.Rinkas) return 1;
            return 2;
        }

        public override void SetIndex(ItemSeeker s, int i) {
            ItemTypeIndex type;
            if(i == 0)
                type = ItemTypeIndex.Mella;
            else if(i == 1)
                type = ItemTypeIndex.Rinkas;
            else
                type = ItemTypeIndex.PalSwap;

            s.Data[s.itemOffset] = (byte)(
                (s.Data[s.itemOffset] & 0xF0) |
                ((int)type & 0x0F));
        }
    }
    class UnknowItemEditTool:ItemEditTool
    {

        static Image UnknownImage = ItemImages.Misc;
        public override Image GetImage(ItemData s) {
            return UnknownImage;
        }
        public override string[] GetList() {
            return new string[] { "unknown" };
        }

        public override int GetIndex(ItemSeeker s) {
            return 0;
        }

        public override void SetIndex(ItemSeeker s, int i) {
        }
    }
    class DoorEditTool:ItemEditTool
    {

        static Image DoorImage  = ItemImages.Door;
        public override Image GetImage(ItemData s) {
            return DoorImage;
        }
        public override string[] GetList() {
            return new string[] { "Door" };
        }

        public override int GetIndex(ItemSeeker s) {
            return 0;
        }

        public override void SetIndex(ItemSeeker s, int i) {
        }
    }
}

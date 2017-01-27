using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Editroid.ROM
{
    /// <summary>
    /// Examines and modifies which currentLevelItems and doors different parts of
    /// the password refer to.
    /// </summary>
    public class PasswordData:IRomDataParentObject
    {
        /// <summary>Data offset of the first piece of password data.</summary>
        public const int DataOffset = 0x1039;
        /// <summary>Number of password data currentLevelItems.</summary>
        public const int DataCount = 0x34;

        byte[] data;
        MetroidRom rom;
        /// <summary>
        /// Creates a PasswordData object.
        /// </summary>
        /// <param name="data">ROM data to create this object from.</param>
        public PasswordData(MetroidRom rom) {
            this.rom = rom;
            this.data = rom.data;
        }

        /// <summary>
        /// Gets a piece of password data.
        /// </summary>
        /// <param name="count">Index of data.</param>
        /// <returns>a piece of password data</returns>
        public PasswordDatum GetDatum(int index) {
            return new PasswordDatum(data, DataOffset + index * 2);
        }

        /// <summary>
        /// Gets all password data that pertains to a particular map location.
        /// </summary>
        /// <param name="mapLocation">A map location to get data for.</param>
        /// <returns>A list of PasswordDataum objects.</returns>
        public IList<PasswordDatum> GetData(Point mapLocation) {
            List<PasswordDatum> data = new List<PasswordDatum>();
            for (int i = 0; i < DataCount; i++) {
                var dat = GetDatum(i);
                if (dat.MapX == mapLocation.X && dat.MapY == mapLocation.Y)
                    data.Add(dat);
            }

            return data;
        }

        /// <summary>
        /// Checks if data exists for a specific map location.
        /// </summary>
        /// <param name="mapLocation">A map location to get data for.</param>
        /// <returns>True if there is password data associated with the location, otherwise false.</returns>
        public bool HasDataFor(Point mapLocation) {
            for (int i = 0; i < DataCount; i++) {
                var dat = GetDatum(i);
                if (dat.MapX == mapLocation.X && dat.MapY == mapLocation.Y)
                    return true;
            }

            return false;
        }
        
        public int GetDatumIndex(ItemInstance item) {
            int type = -1;
            if (item.Data.ItemType == ItemTypeIndex.Door)
                type = 10;
            else if (item.Data.ItemType == ItemTypeIndex.PowerUp)
                type = (int) ((ItemPowerupData)item.Data).PowerUp;

            return GetDatumIndex(item.MapLocation.X, item.MapLocation.Y, (PowerUpType)type);
            ////for (int i = 0; i < PasswordData.DataCount; i++) {
            ////    PasswordDatum d = GetDatum(i);
            ////    if (d.Item == type && d.MapX == item.MapLocation.X && d.MapY == item.MapLocation.Y)
            ////        return i;
            ////}

            ////return -1;
        }

        public int GetDatumIndex(int mapX, int mapY, PowerUpType type) {
            for (int i = 0; i < PasswordData.DataCount; i++) {
                PasswordDatum d = GetDatum(i);
                if (d.Item == (int)type && d.MapX == mapX && d.MapY == mapY)
                    return i;
            }

            return -1;
        }

        #region IRomDataParentObject Members

        int IRomDataObject.Offset { get { return DataOffset; } }
        int IRomDataObject.Size { get { return DataCount * 2; } }

        bool IRomDataObject.HasListItems { get { return true; } }
        bool IRomDataObject.HasSubItems { get { return false; } }

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return RomDataObjects.EmptyNode;
        }

        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            LineDisplayItem[] items = new LineDisplayItem[DataCount];
            for (int i = 0; i < DataCount; i++) {
                items[i] = new LineDisplayItem("Password entry " + i.ToString("X"), DataOffset + i * 2, 2, rom.data);
            }

            return items;
        }

        string IRomDataObject.DisplayName { get { return "Password Data"; } }


        #endregion
    }

    /// <summary>
    /// Exposes the function of a password bit.
    /// </summary>
    public struct PasswordDatum
    {
        // Two bytes of data in the following format:
        //   (first char is property, second indicates which bit of that property
        //    the bit in the data represents).
        // X2 X1 X0 Y4 Y3 Y2 Y1 Y0    I5 I4 I3 I2 I1 I0 X4 X3

        /// <summary>
        /// The offset of this piece of data.
        /// </summary>
        public int offset;
        /// <summary>
        /// The ROM data being examined.
        /// </summary>
        public byte[] data;

        /// <summary>
        /// Instantiates this struct.
        /// </summary>
        /// <param name="data">ROM data to examine.</param>
        /// <param name="offset">Offset within ROM data to examine.</param>
        public PasswordDatum(byte[] data, int offset) {
            this.data = data;
            this.offset = offset;
        }

        /// <summary>
        /// Gets/sets the yTile-coordinate that this datum refers to.
        /// </summary>
        public int MapY {
            get {
                return data[offset] & 0x1F;
            }
            set {
                data[offset] = (byte)(
                    (data[offset] & 0xE0) |
                    (value & 0x1F)
                );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if(obj is PasswordDatum) {
                return ((PasswordDatum)obj).data == data &&
                    ((PasswordDatum)obj).offset == offset;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets/sets the xTile-coordinate that this datum refers to.
        /// </summary>
        public int MapX {
            get {
                int result = (data[offset] & 0xE0) / 0x20;
                result |= (data[offset + 1] & 0x3) * 0x08;

                return result;
            }
            set {
                int lowHalf = (value & 0x07) * 0x20;
                int hiHalf = (value & 0x18) / 0x08;
                data[offset] = (byte)(
                    (data[offset] & 0x1F) |
                    (lowHalf));
                data[offset + 1] = (byte)(
                    (data[offset + 1] & 0xFC) |
                    (hiHalf));
            }
        }

        /// <summary>
        /// Gets/sets the gameItem type that this datum refers to.
        /// </summary>
        public int Item {
            get {
                return (data[offset + 1] & 0xFC) / 0x4;
            }
            set {
                data[offset + 1] = (byte)(
                    (data[offset + 1] & 0x3) |
                    value * 4);
            }
        }

        static string[] types = { "Bombs", "High Jump", "Long Beam", "Screw Attack", " Maru Mari", "Varia Suit", "Wave Beam", "Ice Beam", "Energy Tank", "Missile Expansion", "Door", "(Invalid)", "(Invalid)", "(Invalid)", "(Invalid)", "(Invalid)" };

        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>T string representation of this object.</returns>
        public override string ToString() {
            string type;
            if (Item < types.Length)
                type = types[Item];
            else
                type = "Item " + Item.ToString("X");

            return
                MapX.ToString("x").PadLeft(2, '0') + " "
                + MapY.ToString("x").PadLeft(2, '0') + ": " +
                type;
        }


    }
}

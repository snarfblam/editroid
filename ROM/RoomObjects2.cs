using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Editroid.Graphic;

namespace Editroid.ROM
{
    public abstract class ObjectInstance 
    {
        public virtual int GetTypeIndex() {
            throw new NotImplementedException("The method \"GetTypeIndex\" is not implemented on the class " + this.GetType().ToString());
        }
        public virtual void SetTypeIndex(int value) {
            throw new NotImplementedException("The method \"SetTypeIndex\" is not implemented on the class " + this.GetType().ToString());
        }
        public virtual int GetPalette() {
            throw new NotImplementedException("The method \"GetPalette\" is not implemented on the class " + this.GetType().ToString());
        }
        public virtual void SetPalette(int value) {
            throw new NotImplementedException("The method \"SetPalette\" is not implemented on the class " + this.GetType().ToString());
        }

        public virtual int CompositeLocation {
            get {
                throw new NotImplementedException("The property \"CompositeLocation\" is not implemented on the class " + this.GetType().ToString());
            }
            set {
                throw new NotImplementedException("The property \"CompositeLocation\" is not implemented on the class " + this.GetType().ToString());
            }
        }
    }
    public class StructInstance : ObjectInstance {
        private StructInstance() {
        }

        internal static StructInstance GetNew() {
            return new StructInstance();
        }
        internal static StructInstance GetNew(int type, byte palette) {
            return new StructInstance() { ObjectType = type & 0xFF , PalData = (byte)(palette & 0x03)};
        }

        internal static StructInstance LoadInstance(byte[] data, ref int offset) {
            var result = new StructInstance();

            result.CompositeLocation = data[offset];
            offset++;

            result.ObjectType = data[offset];
            offset++;

            result.PalData = data[offset];
            offset++;

            return result;
        }


        internal void Save(byte[] data, ref int offset) {
            data[offset] = (byte)CompositeLocation;
            offset++;

            data[offset] = (byte)ObjectType;
            offset++;

            data[offset] = PalData;
            offset++;
        }

        public override int GetTypeIndex() {
            return ObjectType;
        }
        public override void SetTypeIndex(int value) {
            ObjectType = value;
        }
        public override int GetPalette() {
            return PalData;
        }
        public override void SetPalette(int value) {
            PalData = (byte)value;
        }
        int _x;
        public int X { get { return _x; } set { _x = (value & 0x0F); } }
        
        int _y;
        public int Y { get { return _y; } set { _y = (value & 0x0F); } }
            /// <summary>Gets/sets an 8-bit value that specifies this objects location.</summary>
        public override int CompositeLocation {
            get {
                return _x | (_y << 4);
            }
            set {
                _x = value & 0x0F;
                _y = (value >> 4) & 0x0F;
            }
        }

            /// <summary>
            /// Gets the index of the struct definition this object uses
            /// </summary>
            public int ObjectType {get;set;}

            /// <summary>
            /// Gets the raw paletteIndex data for this object
            /// </summary>
            public byte PalData { get; set; }

            /// <summary>
            /// Copies the data from this screen object to another screen object.
            /// </summary>
            /// <param name="dest">Screen object to copy data to.</param>
            public void CopyDataTo(StructInstance dest) {
                dest._x = _x;
                dest._y = _y;
                dest.ObjectType = ObjectType;
                dest.PalData = PalData;
            }





            /// <summary>
            /// Gets a string representation of this object.
            /// </summary>
            /// <returns>T string representation of this object.</returns>
            public override string ToString() {
                return "Object:" +
                    " Type=" + ObjectType.ToString("x") +
                    " Palette=" + PalData.ToString("x") +
                    " ScreenPos=" + this.X.ToString() + "," + this.Y.ToString();
            }




            internal StructInstance Clone() {
                StructInstance result = new StructInstance();
                CopyDataTo(result);
                return result;
            }
    }
    public class DoorInstance : ObjectInstance   {
        private DoorInstance() { }
        public static DoorInstance GetNew() { return new DoorInstance(); }


        static Rectangle leftDoorBounds = new Rectangle(16, 80, 8, 48);
        static Rectangle rightDoorBounds = new Rectangle(232, 80, 8, 48);

        public Rectangle Bounds {
            get {
                if (Side == DoorSide.Left)
                    return leftDoorBounds;
                else if (Side == DoorSide.Right)
                    return rightDoorBounds;

                return Rectangle.Empty;
            }
        }

        internal static DoorInstance LoadInstance(byte[] data, ref int oData) {
            DoorInstance result = new DoorInstance();

            oData++; // First byte merely specifies its a door. We already know this.

            int low = data[oData] & 0x0F;
            int high = data[oData] & 0xF0;
            oData++;

            if (high == 0xA0) {
                result.Side = DoorSide.Right;
            } else if (high == 0xB0) {
                result.Side = DoorSide.Left;
            } else {
                result.Side = DoorSide.Invalid;
            }

            if (low <= 3) {
                result.Type = (DoorType)low;
            } else {
                result.Type = DoorType.Invalid;
            }

            return result;
        }

        internal void Save(byte[] data, ref int offset) {
            data[offset] = (byte)RomValues.EnemyIdentifiers.Door;
            offset++;

            data[offset] = (byte)((int)Side | (int)Type);
            offset++;
        }

        public DoorSide Side { get; set; }
        public DoorType Type { get; set; }



        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>T string representation of this object.</returns>
        public override string ToString() {
            return "Door: " + Side.ToString() + " " + Type.ToString() ;
        }



        internal DoorInstance Clone() {
            DoorInstance door = new DoorInstance();
            door.Side = this.Side;
            door.Type = this.Type;

            return door;
        }
    } 

    public class EnemyInstance: ObjectInstance
    {
        /// <summary>
        /// T sprite to be rendered adjacent to an enemy to indicate that it respawns.
        /// </summary>
        public static SpriteDefinition RespawnSprite = new SpriteDefinition(new byte[] {macros.L4, macros.U4,  0x04 });



        private EnemyInstance() { }
        internal static EnemyInstance GetNew() {
            return new EnemyInstance();
        }


        internal static EnemyInstance LoadInstance(byte[] data, ref int oData) {
            EnemyInstance result = new EnemyInstance();

            int byte1 = data[oData];
            oData++;
            int byte2 = data[oData];
            oData++;
            int byte3 = data[oData];
            oData++;

            result.SpriteSlot = byte1 >> 4;
            result.Respawn = (byte1 & 0x0F) == 0x07;

            result.Difficult = ((byte2 & 0x80) == 0x80);
            result.IsLevelBoss = (byte2 & 0x40) == 0x40;
            result.EnemyType = byte2 & 0x0F;

            result.CompositeLocation = byte3;

            return result;
        }

        internal void Save(byte[] data, ref int offset) {
            int byte1, byte2, byte3;


            if (Respawn)
                byte1 = (int)RomValues.EnemyIdentifiers.RespawnEnemy;
            else
                byte1 = (int)RomValues.EnemyIdentifiers.Enemy;
            byte1 |= (SpriteSlot << 4);

            byte2 = (int)EnemyType;
            if (IsLevelBoss) byte2 |= 0x40;
            if (Difficult) byte2 |= 0x80;

            byte3 = CompositeLocation;

            data[offset] = (byte)byte1;
            offset++;
            data[offset] = (byte)byte2;
            offset++;
            data[offset] = (byte)byte3;
            offset++;
        }

        public override int GetTypeIndex() {
            return EnemyType;
        }
        public override void SetTypeIndex(int value) {
            EnemyType = value;
        }
        public override int GetPalette() {
            return DifficultByteValue;
        }
        public override void SetPalette(int value) {
            DifficultByteValue = value;
        }
		/// <summary>
		/// Gets/sets the sprite slot this sprite is loaded into.
		/// </summary>
        public int SpriteSlot { get; set; }

        /// <summary>
        /// Gets/sets whether this enemy respawns.
        /// </summary>
        public bool Respawn { get; set; }

        
        /// <summary>
		/// Gets/sets the underlying value that indicates whether this enemy will be the difficult variant.
		/// </summary>
        public int DifficultByteValue { get; set; }
        public bool Difficult {
            get {
                return (DifficultByteValue & 8) != 0;
            }
            set {
                DifficultByteValue = value ? 8 : 0;
            }
        }

        public bool IsLevelBoss { get; set; }

		/// <summary>
		/// Gets/sets the type of enemy that this EnemyInstance represents.
		/// </summary>
		/// <remarks>For valid enemy types, the Editroid.Graphic.SpriteDefinition class
		/// defines corresponding sprite images composed of tiles from the sprite
		/// pattern table.</remarks>
        public int EnemyType { get; set; }

        int _y;
		/// <summary>
		/// Gets/sets the yTile-coordinate of an enemy.
		/// </summary>
		public int Y {
            get { return _y; }
            set { _y = value & 0x0F; }
		}

        int _x;
		/// <summary>
		/// Gets/sets the xTile-coordinate of an enemy.
		/// </summary>
        public int X {
            get { return _x; }
            set { _x = value & 0x0F; }
        }

        /// <summary>Gets/sets an 8-bit value that specifies this objects location.</summary>
        public override int CompositeLocation {
            get {
                return _x | (_y << 4);
            }
            set {
                _x = value & 0x0F;
                _y = (value >> 4) & 0x0F;
            }
        }

        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>T string representation of this object.</returns>
        public override string ToString() {
            return "Enemy:" +
                " Type=" + EnemyType.ToString("x") +
                (Respawn ? "Respawns " : "") +
                " Palette=" + DifficultByteValue.ToString() +
                " Slot=" + SpriteSlot.ToString() +
                " ScreenPos=" + this.X.ToString() + "," + this.Y.ToString();
        }


        /// <summary>
        /// Copies the data from this screen object to another screen object.
        /// </summary>
        /// <param name="dest">Screen object to copy data to.</param>
        public void CopyDataTo(EnemyInstance dest) {
            dest.CompositeLocation = CompositeLocation;
            dest.EnemyType = EnemyType;
            dest.IsLevelBoss = IsLevelBoss;
            dest.Respawn = Respawn;
            dest.Difficult = Difficult;
            dest.SpriteSlot = SpriteSlot;
        }

        internal EnemyInstance Clone() {
            EnemyInstance result = new EnemyInstance();
            CopyDataTo(result);
            return result;
        }
    }

    public class ItemInstance : ObjectInstance
    {
        ItemScreenData screen;
        ItemData item;

        public ItemInstance(ItemScreenData screen, ItemData item) {
            this.screen = screen;
            this.item = item;
        }

        public Point MapLocation {
            get {
                return new Point(screen.MapX, screen.MapY);
            }
        }
        public void SetMapX(int x) {
            screen.MapX = x;
        }
        public ScreenCoordinate ScreenLocation {
            get {
                //return data.ScreenPosition;
                if (item is IItemScreenPosition) {
                    var pos = ((IItemScreenPosition)item).ScreenPosition;
                    return new ScreenCoordinate(pos.X, pos.Y);
                }
                throw new InvalidOperationException("ScreenLocation Property is not valid on this object.");
            }
            set {
                //data.ScreenPosition = value;
                if (item is IItemScreenPosition) {
                    ((IItemScreenPosition)item).ScreenPosition = new Point(value.X, value.Y);
                } else {
                    throw new InvalidOperationException("ScreenLocation Property is not valid on this object.");
                }
            }
        }
        public ItemData Data { get { return item; } }



        public SpriteDefinition GetSprite(out int x, out int y, out int pal) {
            x = 0;
            y = 0;
            pal = 0;

            var posItem = item as IItemScreenPosition;

            switch (item.ItemType) {
                case ItemTypeIndex.Enemy:
                    x = posItem.ScreenPosition.X * 2;
                    y = posItem.ScreenPosition.Y * 2;
                    pal = 8;
                    return ItemSprites.Enemy;
                case ItemTypeIndex.PowerUp:
                    x = posItem.ScreenPosition.X * 2;
                    y = posItem.ScreenPosition.Y * 2;
                    pal = 8;
                    return PowerUpSprites.GetSprite(((ItemPowerupData)item).PowerUp);
                case ItemTypeIndex.Mella:
                    x = 17;
                    y = 14;
                    pal = 11;
                    return ItemSprites.Mella;
                case ItemTypeIndex.Elevator:
                    x = 14;
                    y = 16;
                    pal = 8;
                    return ItemSprites.Elevator;
                case ItemTypeIndex.MotherBrain:
                    x = 12;
                    y = 3;
                    return ItemSprites.MotherBrain;
                case ItemTypeIndex.Zebetite:
                    int zebetiteIndex = item.SpriteSlot;
                    ////if ((data.ItemTypeByte >> 4) == 1 || (data.ItemTypeByte >> 4) == 3)
                    ////    x = 3;
                    if (zebetiteIndex == 1 || zebetiteIndex == 3) {
                        x = 3;
                    } else {
                        x = 19;
                    }
                    y = 12;
                    pal = 8;
                    return ItemSprites.ZebetiteMarker;
                case ItemTypeIndex.Rinkas:
                    x = 17;
                    y = 12;
                    pal = 8;
                    return ItemSprites.Rinka;
                case ItemTypeIndex.Door:
                    //var door = AsDoorInstance();
                    var door = item as ItemDoorData;
                    y = 10;
                    pal = 8 + (int)door.Type;

                    if (door.Side == DoorSide.Left) {
                        x = 3;
                        return door.Type == DoorType.MusicChange ? DoorSprites.LeftDoorMusic : DoorSprites.LeftDoor;
                    } else {
                        x = 28;
                        return door.Type == DoorType.MusicChange ? DoorSprites.RightDoorMusic : DoorSprites.RightDoor;
                    }
                case ItemTypeIndex.PalSwap:
                    x = 15;
                    y = 12;
                    pal = 8;
                    return ItemSprites.PaletteSwitch;
                case ItemTypeIndex.Turret:
                    x = posItem.ScreenPosition.X * 2;
                    y = posItem.ScreenPosition.Y * 2;
                    pal = 10;
                    int type = item.SpriteSlot;
                    if (type == 0) return ItemSprites.Turret1;
                    if (type == 1) return ItemSprites.Turret2;
                    if (type == 2) return ItemSprites.Turret3;
                    return ItemSprites.Turret4;

            }
            return null;
        }

        /// <summary>
        /// Gets a rectangle that can bound the item visually and be used for click detection.
        /// </summary>
        public Rectangle GetCollisionRect() {
            switch (item.ItemType) {
                case ItemTypeIndex.Elevator:
                    return new Rectangle(112, 128, 32, 8);
                case ItemTypeIndex.MotherBrain:
                    return new Rectangle(92, 24, 45, 40);
                case ItemTypeIndex.Zebetite:
                    var zebIndex = item.SpriteSlot;
                    if (zebIndex  == 1 || zebIndex == 3)
                        return new Rectangle(24, 96, 8, 32);
                    else
                        return new Rectangle(152, 96, 8, 32);
                case ItemTypeIndex.Turret:
                    var pos = ((IItemScreenPosition)item).ScreenPosition;
                    return new Rectangle(pos.X * 16, pos.Y * 16, 16, 16);
            }

            // Try using generic sprite measurement if a special rect isn't provided.
            int x, y, p;
            SpriteDefinition sprite = GetSprite(out x, out y, out p);
            if (item.ItemType == ItemTypeIndex.Door)
                return new Rectangle(x * 8, y * 8, sprite.Width * 8 - 8, sprite.Height * 8);
            if (sprite != null)
                return new Rectangle(x * 8, y * 8, sprite.Width * 8, sprite.Height * 8);

            // Return no rect.
            return Rectangle.Empty;
        }

        public bool HasScreenLocation {
            get {
                return item is IItemScreenPosition;
            }
        }

        public override int CompositeLocation {
            get {
                if (!HasScreenLocation) throw new InvalidOperationException("Item type " + Data.ItemType.ToString() + " does not support CompositeLocation property.");
                var pos = item as IItemScreenPosition;
                return pos.ScreenPosition.X | (pos.ScreenPosition.Y << 4);
            }
            set {
                if (!HasScreenLocation) throw new InvalidOperationException("Item type " + Data.ItemType.ToString() + " does not support CompositeLocation property.");
                var pos = item as IItemScreenPosition;

                Point newPos = new Point();
                newPos.X = value & 0x0F;
                newPos.Y = (value >> 4) & 0x0F;

                pos.ScreenPosition = newPos;
            }
        }
    }

    /// <summary>
    /// Indicates which side of the screen a door appears on
    /// </summary>
    public enum DoorSide : byte
    {
        /// <summary>
        /// Signifies that a door is on the left side of the screen
        /// </summary>
        Left = 0xB0,
        /// <summary>
        /// Signifies that a door is on the right side of the screen
        /// </summary>
        Right = 0xA0,
        /// <summary>
        /// Indicates an invalid value. This indicates invalid data was read. DoorSide.Invalid should never be written back into data.
        /// </summary>
        Invalid = 0x0
    }


    /// <summary>
    /// Represents a type of door
    /// </summary>
    public enum DoorType : byte
    {
        /// <summary>
        /// Represents a door that must be shot with five missiles in order to be opened
        /// </summary>
        Missile = 0,
        /// <summary>
        /// Represents a door that can be opened with any weapon
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Represents a door that must be shot with ten missiles in order to be opened
        /// </summary>
        TenMissile = 2,
        /// <summary>
        /// Changes music. It is unknown whether the music played depends on the direction 
        /// of the door or is toggled.
        /// </summary>
        MusicChange = 3,
        /// <summary>
        /// Represents an invalid value. This indicates invalid data was read. This value should never be written back into data.
        /// </summary>
        Invalid = 0xFF
    }
}



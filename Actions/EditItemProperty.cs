using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using System.Drawing;
using Editroid.ROM;

namespace Editroid.Actions
{
    class EditItemProperty:MapLocationAction
    {
        ItemData item;
        ItemScreenData screen;
        ItemProperty prop;
        int newValue, oldValue;

        public EditItemProperty(EditroidUndoRedoQueue q, Point MapLocation, ItemScreenData screen, ItemData item, ItemProperty prop, int newValue) 
        :base(q,MapLocation) {
            this.prop = prop;
            this.item = item;
            this.screen = screen;

            this.newValue = newValue;
            this.oldValue = GetValue();
        }

        public ItemData Item { get { return item; } }

        private int GetValue() {
            ////ItemInstance i = item.GetItem(Queue.Rom);
            ////ItemSeeker data = i.Data;
            var iPowerUp = item as ItemPowerupData;
            var iEnemy = item as ItemEnemyData;
            var iElevator = item as ItemElevatorData;
            var iSingleByte = item as ItemSingleByteData;
            var iDoor = item as ItemDoorData;

            switch (prop) {
                case ItemProperty.power_up_type:
                    return (int)iPowerUp.PowerUp;
                case ItemProperty.elevator_destination:
                    return (int)iElevator.ElevatorType;
                case ItemProperty.single_byte_item_type: 
                    return iSingleByte.SingleByteItemType; 
                case ItemProperty.enemy_difficulty:
                    return iEnemy.Difficult ? -1 : 0;
                case ItemProperty.enemy_slot:
                    return item.SpriteSlot;
                case ItemProperty.enemy_type:
                    return iEnemy.EnemyType;
                case ItemProperty.door_value:
                    return (int)iDoor.Side | (int)iDoor.Type;
                case ItemProperty.turret_type:
                    return item.SpriteSlot;
                case ItemProperty.invalid:
                    throw new ArgumentException("Invalid item proprerty on EditItemProperty.GetValue.");

            }

            return 0;
        }
        private void SetValue(int val) {
            ////ItemInstance i = item.GetItem(Queue.Rom);
            ////ItemSeeker data = i.Data;

            var iPowerUp = item as ItemPowerupData;
            var iEnemy = item as ItemEnemyData;
            var iElevator = item as ItemElevatorData;
            var iSingleByte = item as ItemSingleByteData;
            var iDoor = item as ItemDoorData;

            switch (prop) {
                case ItemProperty.power_up_type:
                    iPowerUp.PowerUp = (PowerUpType)val;
                    break;
                case ItemProperty.elevator_destination:
                    iElevator.ElevatorType = (ElevatorDestination)val;
                    break;
                case ItemProperty.single_byte_item_type:
                    iSingleByte.SingleByteItemType = (byte)val;
                    break;
                case ItemProperty.enemy_slot:
                    item.SpriteSlot = val;
                    break;
                case ItemProperty.enemy_type:
                    iEnemy.EnemyType = val;
                    break;
                case ItemProperty.enemy_difficulty:
                    iEnemy.Difficult = (val != 0); // Non-zero for true
                    break;
                case ItemProperty.door_value:
                    iDoor.Side = (DoorSide)(val & 0xF0);
                    iDoor.Type = (DoorType)(val & 0x0F);
                    break;
                case ItemProperty.turret_type:
                    //data.ItemTypeByte = (byte)val;
                    item.SpriteSlot = val;
                    break;
                case  ItemProperty.invalid:
                    throw new ArgumentException("Invalid item proprerty on EditItemProperty.SetValue.");
            }
        }

        public override void Do() {
            SetValue(newValue);
        }

        public override void Undo() {
            SetValue(oldValue);
        }

        public override string GetText() {
            return "Change " + prop.ToString().Replace('_', ' ');
        }

        public override bool TryCombine(EditroidAction newerAction) {
            EditItemProperty a = newerAction as EditItemProperty;
            if (a == null || a.prop != prop || a.AffectedMapLocation != AffectedMapLocation || a.item != item)
                return false;

            this.newValue = a.newValue;
            return true;
        }

        public ItemProperty Property { get { return prop; } }

        ////public ItemIndex_DEPRECATED ItemID { get { return item; } }
    }

    /// <summary>
    /// Provides a list of item properties that can be modified with an EditItemProperty object.
    /// </summary>
    public enum ItemProperty
    {
        invalid,
        power_up_type,
        elevator_destination,
        single_byte_item_type,
        enemy_type,
        enemy_slot,
        enemy_difficulty,
        door_value,
        turret_type
    }
}

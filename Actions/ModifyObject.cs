using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using System.Drawing;
using Editroid.ROM;

namespace Editroid.Actions
{
    class ModifyObject : ScreenItemAction
    {
        int oldValue, newValue;
        ObjectModification change;
        public ModifyObject(EditroidUndoRedoQueue q, Screen screen, ObjectInstance obj, int newValue, ObjectModification change)
            : base(q, screen, obj) {
            this.change = change;
            this.newValue = newValue;
            oldValue = GetValue(); 
        }
        public ModifyObject(EditroidUndoRedoQueue q, Screen screen, ObjectInstance obj, bool newValue, ObjectModification change)
            : this(q, screen, obj, newValue ? -1 : 0, change) {
        }

        public ModifyObject(EditroidUndoRedoQueue q, Screen screen, ObjectInstance obj, Point newLocation)
            : base(q, screen, obj) {
            this.change =  ObjectModification.Location;
            // The ROM packs object locations into a single byte.
            this.newValue = 0xFF & ((newLocation.X & 0x0F) | (newLocation.Y << 4));
            oldValue = GetValue();
        }

        private int GetValue() {
            ObjectInstance i = this.GetItem();
            switch (change) {
                case ObjectModification.Type:
                    return i.GetTypeIndex();
                case ObjectModification.Palette:
                    return i.GetPalette();
                case ObjectModification.SpriteSlot:
                    return ((EnemyInstance)i).SpriteSlot;
                case ObjectModification.Location:
                    return i.CompositeLocation;
                case ObjectModification.Respawnable:
                    return ((EnemyInstance)i).Respawn ? -1 : 0;
                case ObjectModification.IsBoss:
                    return ((EnemyInstance)i).IsLevelBoss ? -1 : 0;
                case ObjectModification.None:
                default:
                    throw new InvalidOperationException("ModifyObject.GetValue() called with at invalid ObjectModification value (" + (change.ToString()) + ").");
            }
        }
        private void SetValue(int value) {
            ObjectInstance i = this.GetItem();
            EnemyInstance e = i as EnemyInstance; 
            switch (change) {
                case ObjectModification.Type:
                    i.SetTypeIndex(value);
                    break;
                case ObjectModification.Palette:
                    i.SetPalette(value);
                    break;
                case ObjectModification.SpriteSlot:
                    e.SpriteSlot = value;
                    break;
                case ObjectModification.Location:
                    i.CompositeLocation = value;
                    break;
                case ObjectModification.Respawnable:
                    e.Respawn = (value != 0);
                    break;
                case ObjectModification.IsBoss:
                    e.IsLevelBoss = (value != 0);
                    break;
                case ObjectModification.None:
                default:
                    throw new InvalidOperationException("ModifyObject.SetValue() called with at invalid ObjectModification value (" + (change.ToString()) + ").");
            }
        }

        public ObjectModification Change { get { return change; } }
        public int OldValue { get { return oldValue; } }
        public int NewValue { get { return newValue; } }

        protected override void Do(ObjectInstance screenItem) {
            SetValue(newValue);
        }

        protected override void Undo(ObjectInstance screenItem) {
            SetValue(oldValue);
        }

        public override string GetText() {
            switch (change) {
                case ObjectModification.Type:
                    return "Change type";
                case ObjectModification.Location:
                    return "Move object";
                case ObjectModification.Palette:
                    return "Change palette";
                case ObjectModification.SpriteSlot:
                    return "Change spriteslot";
                case ObjectModification.IsBoss:
                    return "Toggle enemy as level boss";
                default:
                    return "Edit object";
            }
        }
        

        public override bool TryCombine(EditroidAction newerAction) {
            ModifyObject a = newerAction as ModifyObject;
            if (a != null && a.change == this.change && CompareItemReference(a)) {
                newValue = a.newValue;
                return true;
            }
            return false;
        }
    }

    public enum ObjectModification
    {
        None,
        Type,
        Location,
        Palette,
        SpriteSlot,
        Respawnable,
        IsBoss
    }
}
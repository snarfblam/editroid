using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid.Actions
{
    class ChangeDoor:ScreenAction
    {
        DoorSide side;
        DoorType oldType, newType;
        public ChangeDoor(EditroidUndoRedoQueue q, Screen screen, DoorSide side, DoorType type) 
        :base(q,screen){
            this.side = side;
            this.newType = type;

            oldType = DoorType.Invalid;
            foreach (DoorInstance d in Screen.Doors) {
                if (d.Side == side)
                    oldType = d.Type;
            }
        }

        public override void Do() {
            SetDoor(newType);
        }

        private void SetDoor(DoorType type) {
            if (type == DoorType.Invalid)
                Screen.DeleteDoor(this.side);
            else {
                bool found = false;
                foreach (DoorInstance d in Screen.Doors) {
                    if (d.Side == this.side) {
                        DoorInstance dd = d;
                        dd.Type = type;
                        found = true;
                    }
                }

                if (!found)
                    Screen.AddDoor(type, this.side);
            }
        }

        public override void Undo() {
            SetDoor(this.oldType);
        }

        public override string GetText() {
            if (this.newType == DoorType.Invalid)
                return "Remove door";
            else
                return "Change door";
        }

        public override bool TryCombine(EditroidAction newerAction) {
            ChangeDoor a = newerAction as ChangeDoor;
            if (a != null) {
                if (a.AffectedLevel == AffectedLevel && a.AffectedScreenIndex == AffectedScreenIndex && a.side == side) {
                    this.newType = a.newType;
                    return true;
                }
            }

            return false;
        }
    }
}

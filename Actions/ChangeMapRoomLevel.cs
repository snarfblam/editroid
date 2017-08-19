using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using System.Drawing;

namespace Editroid.Actions
{
    class ChangeMapRoomLevel:MapLocationAction
    {
        LevelIndex newValue, oldValue;
        int oldScreenIndex, newScreenIndex;

        public ChangeMapRoomLevel(EditroidUndoRedoQueue q, Point location, LevelIndex newValue)
            : base(q, location) {

            this.newValue = newValue;
            oldValue = q.Editor.MapView.GetLevel(location.X, location.Y);

            // Ensure that the screen index will be valid for the new level
            oldScreenIndex = Queue.Rom.GetScreenIndex(location.X, location.Y);
            newScreenIndex = Math.Min(oldScreenIndex, q.Rom.GetLevel(newValue).Screens.Count - 1);
            if (newValue == LevelIndex.None) newScreenIndex = 0xFF; // ...FF indicates a blank room
        }


        public override void Do() {
            Queue.Editor.MapView.SetLevel(AffectedMapLocation.X, AffectedMapLocation.Y, newValue);
            Queue.Rom.SetScreenIndex(AffectedMapLocation.X, AffectedMapLocation.Y, (byte)newScreenIndex);
        }

        public override void Undo() {
            Queue.Editor.MapView.SetLevel(AffectedMapLocation.X, AffectedMapLocation.Y, oldValue);
            Queue.Rom.SetScreenIndex(AffectedMapLocation.X, AffectedMapLocation.Y, (byte)oldScreenIndex);
        }

        public override string GetText() {
            //return "Change location to " + newValue.ToString();
            if (newValue == LevelIndex.None) {
                return "Disable map location";
            } else if (oldValue == LevelIndex.None) {
                return "Enable map location (display as " + newValue.ToString() + ")";
            } else {
                return "Set location to display as " + newValue.ToString();
            }
        }

        public override bool IsNullAction {
            get {
                return newValue == oldValue && newScreenIndex == oldScreenIndex;
            }
        }

        public override bool TryCombine(EditroidAction newerAction) {
            ChangeMapRoomLevel a = newerAction as ChangeMapRoomLevel;
            if (a != null && a.AffectedMapLocation == AffectedMapLocation) {
                newValue = a.newValue;
                newScreenIndex = a.newScreenIndex;
                return true;
            }

            return false;
        }
    }
}

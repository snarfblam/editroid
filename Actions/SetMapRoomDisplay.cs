using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using System.Drawing;

namespace Editroid.Actions
{
    class SetMapRoomDisplay:MapLocationAction
    {
        //LevelIndex newValue, oldValue;
        //int oldScreenIndex, newScreenIndex;
        bool? oldPalValue, newPalValue;
        int? oldAnimationValue, newAnimationValue;

        public SetMapRoomDisplay(EditroidUndoRedoQueue q, Point location, bool? alternatePal, int? animationIndex)
            : base(q, location) {

            this.newPalValue = alternatePal;
            this.newAnimationValue = animationIndex;

            oldPalValue = q.Editor.MapView.GetAltPal(location.X, location.Y);
            oldAnimationValue = q.Editor.MapView.GetAnimation(location.X, location.Y);
        }


        public override void Do() {
            if (newPalValue != null) {
                Queue.Editor.MapView.SetAltPal(AffectedMapLocation.X, AffectedMapLocation.Y, newPalValue.Value);
            }
            if (newAnimationValue != null) {
                Queue.Editor.MapView.SetAnimation(AffectedMapLocation.X, AffectedMapLocation.Y, newAnimationValue.Value);
            }
        }

        public override void Undo() {
            if (oldPalValue != null) {
                Queue.Editor.MapView.SetAltPal(AffectedMapLocation.X, AffectedMapLocation.Y, oldPalValue.Value);
            }
            if (oldAnimationValue != null) {
                Queue.Editor.MapView.SetAnimation(AffectedMapLocation.X, AffectedMapLocation.Y, oldAnimationValue.Value);
            }
        }

        public override string GetText() {
            //return "Change location to " + newValue.ToString();
            if (newPalValue != null && newAnimationValue == null) return "Set displayed palette";
            if (newPalValue == null && newAnimationValue != null) return "Set displayed animation";
            return "Set display options";
        }

        public override bool IsNullAction {
            get {
                return oldPalValue == newPalValue && oldAnimationValue == newAnimationValue;
            }
        }

        public override bool TryCombine(EditroidAction newerAction) {
            SetMapRoomDisplay a = newerAction as SetMapRoomDisplay;
            if (a != null && a.AffectedMapLocation == AffectedMapLocation) {
                newPalValue = a.newPalValue ?? newPalValue;
                newAnimationValue = a.newAnimationValue ?? newAnimationValue;
                return true;
            }

            return false;
        }
    }
}

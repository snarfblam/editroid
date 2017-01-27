using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using System.Drawing;

namespace Editroid.Actions
{
    class SetScreenIndex:MapLocationAction
    {
        int oldValue, newValue;
        public SetScreenIndex(EditroidUndoRedoQueue q, Point mapLocation, int screenIndex)
            : base(q, mapLocation) {
            oldValue = Queue.Rom.GetScreenIndex(mapLocation.X, mapLocation.Y);
            newValue = screenIndex;
        }

        public override void Do() {
            Queue.Rom.SetScreenIndex(AffectedMapLocation.X, AffectedMapLocation.Y, (byte)newValue);
        }

        public override void Undo() {
            Queue.Rom.SetScreenIndex(AffectedMapLocation.X, AffectedMapLocation.Y, (byte)oldValue);
        }

        public override string GetText() {
            return "Select screen layout";
        }

        public override bool TryCombine(EditroidAction newerAction) {
            SetScreenIndex a = newerAction as SetScreenIndex;
            if (a != null && a.AffectedMapLocation == AffectedMapLocation) {
                newValue = a.newValue;
                return true;
            }

            return false;
        }
    }
}

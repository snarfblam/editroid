using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;

namespace Editroid.Actions
{
    class AddOrRemoveBridge: ScreenAction
    {
        bool oldValue, newValue;
        public AddOrRemoveBridge(EditroidUndoRedoQueue q, Screen screen, bool willHaveBridge)
            : base(q, screen) {
            newValue = willHaveBridge;
            oldValue = Screen.HasBridge;
        }
        public override void Do() {
            SetState(newValue);
        }

        public override void Undo() {
            SetState(oldValue);
        }

        private void SetState(bool newValue) {
            if (newValue == Screen.HasBridge) return;

            Screen.HasBridge = newValue;
        }

        public override string GetText() {
            return newValue ? "Add bridge" : "Remove bridge";
        }
    }
}

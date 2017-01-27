using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;

namespace Editroid.Actions
{
    class ToggleAlternateMusic: LevelAction
    {
        bool oldState, newState;
        int screenIndex;

        public ToggleAlternateMusic(EditroidUndoRedoQueue q, Level level, int screenIndex, bool state)
            : base(q, level) {
            this.newState = state;
            this.oldState = Level.AlternateMusicRooms.Contians((byte)screenIndex);
            this.screenIndex = screenIndex;
        }

        public override void Do() {
            SetState(newState);
        }

        private void SetState(bool state) {
            if (state)
                Level.AlternateMusicRooms.Add((byte)screenIndex);
            else
                Level.AlternateMusicRooms.Remove((byte)screenIndex);
        }

        public override void Undo() {
            SetState(oldState);
        }

        public override string GetText() {
            return newState ? "Apply alternate music" : "Remove alternate music";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;

namespace Editroid.Actions
{
    class ModifyCombo:LevelAction
    {
        static int generationIndex = 0;

        List<ComboChange> changes = new List<ComboChange>();
        int generation;

        public ModifyCombo(EditroidUndoRedoQueue q, Level level, int combo, int tile, int newValue)
            : base(q, level) {
            generationIndex++;
            generation = generationIndex;

            ComboChange change;
            change.index = (byte)(combo & 0xFF);
            change.tile = (byte)(tile & 0x03);
            change.newValue = (byte)(newValue & 0xFF);
            change.oldValue = Level.Combos[combo].GetByte(tile);
            changes.Add(change);
        }

        public IList<ComboChange> Changes { get { return changes.AsReadOnly(); } }
        public override void Do() {
            for (int i = 0; i < changes.Count; i++) {
                ComboChange c = changes[i];
                Level.Combos[c.index].SetByte(c.tile, c.newValue);
            }
        }

        public override void Undo() {
            for (int i = changes.Count - 1; i >= 0; i--) {
                ComboChange c = changes[i];
                Level.Combos[c.index].SetByte(c.tile, c.oldValue);
            }
        }

        public override string GetText() {
            return "Edit combo data";
        }

        public override bool IsNullAction {
            get {
                return changes.Count == 0;
            }
        }

        public override bool TryCombine(EditroidAction newerAction) {
            ModifyCombo action = newerAction as ModifyCombo;
            if (action != null && action.AffectedLevel == AffectedLevel && action.generation == generation) {
                foreach (ComboChange c in action.changes)
                    if (c.newValue != c.oldValue) changes.Add(c);

                return true;
            }
            return false;
        }

        public ModifyCombo CreateChainableAction(int combo, int tile, int newValue) {
            ModifyCombo result = new ModifyCombo(Queue, Level, combo, tile, newValue);
            result.generation = generation;
            return result;
        }

        public struct ComboChange
        {
            public byte index, tile, oldValue, newValue;

            public override bool Equals(object obj) {
                if (!(obj is ComboChange)) return false;
                ComboChange c = (ComboChange)obj;
                return c.index == index && c.newValue == newValue && c.oldValue == oldValue && c.tile == tile;
            }
        }
    }
}

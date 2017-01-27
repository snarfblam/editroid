using System;
using System.Collections.Generic;
using System.Text;
using Editroid.UndoRedo;
using Editroid.ROM;

namespace Editroid.Actions{
    abstract class StructureAction : LevelAction
    {
        private int structIndex;
        public int StructIndex {
            get { return structIndex; }
        }

        StructureAction chainedAction;

        public StructureAction(EditroidUndoRedoQueue q, Level level, int index)
            : base(q, level) {
            this.structIndex = index;
        }

        /// <summary>Performs this action.</summary>
        public override sealed void Do() {
            // Causes chained actions to be performed in order

            DoImplementation();
            if (chainedAction != null)
                chainedAction.Do();
        }

        protected abstract void DoImplementation();

        /// <summary>Undoes this action.</summary>
        public override sealed void Undo() {
            // Causes chained actions to be performed in reverse order

            if (chainedAction != null)
                chainedAction.Undo();
            UndoImplementation();
        }
        protected abstract void UndoImplementation();

        public sealed override string GetText() {
            if (chainedAction == null) return Description;
            return "Modify structure";
        }

        public abstract string Description { get; }

        public Structure GetStruct() {
            return Level.GetStruct(structIndex);
        }

        public override bool TryCombine(EditroidAction newerAction) {
            StructureAction a = newerAction as StructureAction;
            if (a == null || a.AffectedLevel != AffectedLevel || a.structIndex != structIndex)
                return false;

            if (chainedAction == null) {
                chainedAction = a;
                return true;
            } else {
                return chainedAction.TryCombine(newerAction);
            }
        }
    }
}
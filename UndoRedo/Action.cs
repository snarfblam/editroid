using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.UndoRedo
{
    partial class UndoRedoQueue<A>
    {
        public abstract class Action
        {
            UndoRedoQueue<A> queue;
            public UndoRedoQueue<A> Queue { get { return queue; } set { queue = value; } }

            public Action() {
            }

            public abstract void Do();
            public abstract void Undo();
            public abstract string GetText();

            protected internal virtual void OnBeforeDiscarded() { }
        }
    }
}

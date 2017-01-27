using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.UndoRedo
{
    public partial class UndoRedoQueue<A> where A : UndoRedoQueue<A>.Action
    {
        public class ActionStack
        {
            TransparentStack<A> stack;
            internal ActionStack(TransparentStack<A> stack) {
                this.stack = stack;
            }

            public int Count { get { return stack.Count; } }
            public A Peek() {
                return stack.Peek();
            }
            public A Peek(int index) {
                return stack.Peek(index);
            }

            public A this[int i] {
                get { return stack[i]; }
            }

        }
    }
}

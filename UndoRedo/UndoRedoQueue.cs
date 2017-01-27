using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.UndoRedo
{

    public partial class UndoRedoQueue<A> where A : UndoRedoQueue<A>.Action
    {
        TransparentStack<A> undos = new TransparentStack<A>();
        TransparentStack<A> redos = new TransparentStack<A>();
        ActionStack publicUndos;
        ActionStack publicRedos;

        public UndoRedoQueue() {
            publicUndos = new ActionStack(undos);
            publicRedos = new ActionStack(redos);
        }

        public virtual A Undo() {
            redos.Push(undos.Pop());
            CropRedos();
            redos.Peek().Undo();
            return redos.Peek();
        }
        public virtual A Redo() {
            undos.Push(redos.Pop());
            CropUndos();
            undos.Peek().Do();
            return undos.Peek();
        }
        public virtual void Do(A a) {
            redos.Clear();
            a.Do();
            undos.Push(a);
            CropUndos();
        }

        public void Clear() {
            undos.Clear();
            redos.Clear();
        }

        public bool CanUndo { get { return undos.Count > 0; } }
        public string UndoText { get { return undos.Peek().GetText(); } }
        public bool CanRedo { get { return redos.Count > 0; } }
        public string RedoText { get { return redos.Peek().GetText(); } }

        protected TransparentStack<A> undoQueue { get { return undos; } }
        protected TransparentStack<A> redoQueue { get { return redos; } }

        public ActionStack Undos { get { return publicUndos; } }
        public ActionStack Redos { get { return publicRedos; } }

        private int undoMaxCount = -1;

        /// <summary>Gets/sets the number of actions the queue will store, or -1 for no limit.</summary>
        /// <remarks>Items beyond the specified count will be discarded.</remarks>
        public int UndoMaxCount {
            get { return undoMaxCount; }
            set {
                undoMaxCount = value;
                CropUndos();
            }
        }
        /// <summary>If the number of undo actions exceeds the maximum count, the excess actions will are removed. Call this method every time an gameItem is added to the undo list.</summary>
        protected void CropUndos() {
            if (undoMaxCount == -1 && totalActionMaxCount == -1) return;
            while (
                (undoMaxCount != -1 && undos.Count > undoMaxCount ) || 
                (totalActionMaxCount != -1 && redos.Count + undos.Count > totalActionMaxCount)) {
                undos[undos.Count - 1].OnBeforeDiscarded();
                undos.CropAt(undos.Count - 1);
            }
        }

        private int redoMaxCount = -1;

        /// <summary>Gets/sets the number of actions the queue will store, or -1 for no limit.</summary>
        /// <remarks>Items beyond the specified count will be discarded.</remarks>
        public int RedoMaxCount {
            get { return redoMaxCount; }
            set { 
                redoMaxCount = value;
                CropRedos();
            }
        }

        private int totalActionMaxCount = -1;

        /// <summary>Gets/sets the number of actions the queue will store, or -1 for no limit.</summary>
        /// <remarks>Items beyond the specified count will be discarded from the undo queue.</remarks>
        public int TotalActionMaxCount {
            get { return totalActionMaxCount; }
            set { totalActionMaxCount = value; }
        }

        /// <summary>If the number of redo actions exceeds the maximum count, the excess actions will are removed. Call this method every time an gameItem is added to the redo list.</summary>
        protected void CropRedos() {
            if (redoMaxCount == -1) return;
            while (redos.Count > redoMaxCount) {
                redos[redos.Count - 1].OnBeforeDiscarded();
                redos.CropAt(redos.Count - 1);
            }
        }

    }
}

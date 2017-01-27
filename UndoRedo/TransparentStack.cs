using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.UndoRedo
{
    /// <summary>Provides a stack with access to all currentLevelItems in the stack, indexed by depth.</summary>
    /// <typeparam name="T">The type of gameItem the stack will store.</typeparam>
    public class TransparentStack<T>:ICollection<T>, IEnumerable<T>
    {
        List<T> items = new List<T>();

        public int IndexOf(T item) {
            return items.Count - 1 - items.IndexOf(item);
        }


        public T this[int index] {
            get {
                return items[Count - 1 - index];
            }
        }

        public void Push(T item){
            items.Add(item);
        }

        public T Pop(){
            T result = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return result;
        }
        public IList<T> Pop(int count) {
            if (count > Count)
                throw new ArgumentException("An attempt was made to remove more items on a stack than were present.", "count");

            T[] result = new T[count];

            for (int i = 0; i < count; i++) {
                result[i] = Pop();
            }

            return result;
        }
        /// <summary>Pops all currentLevelItems up to and including the specfied gameItem.</summary>
        public IList<T> PopTo(T item) {
            if (!items.Contains(item))
                throw new ArgumentException("An attempt was made to remove an item from a stack that was not present.", "item");

            List<T> result = new List<T>();
            // Loop over all currentLevelItems preceeding 'gameItem'
            while (!compare(Peek(), item))
                result.Add(Pop());

            result.Add(Pop());

            return result;
        }
        /// <summary>Pops all currentLevelItems up to, but not including, the specfied gameItem.</summary>
        public IList<T> PopUntil(T item) {
            if (!items.Contains(item))
                throw new ArgumentException("An attempt was made to remove an item from a stack that was not present.", "item");

            List<T> result = new List<T>();
            // Loop over all currentLevelItems preceeding 'gameItem'
            while (!compare(Peek(), item))
                result.Add(Pop());

            return result;
        }

        bool  compare(T a, T b) {
            if (a == null)
                return b == null;
            return a.Equals(b);
        }

        public T Peek(){
            return items[items.Count - 1];
        }

        public T Peek(int depth){
            return items[items.Count - 1 - depth];
        }
        /// <summary>Crops the gameItem deepest in the stack that is equal to the specified gameItem.</summary>
        public void Crop(T item) {
            items.Remove(item);
        }

        public void CropAt(int index) {
            items.RemoveAt(items.Count - 1 - index);
        }

        public void CropAll(Predicate<T> condition) {
            items.RemoveAll(condition);
        }

        

        #region ICollection<T> Members

        void ICollection<T>.Add(T item) {
            Push(item);
        }

        public void Clear() {
            items.Clear();
        }

        public bool Contains(T item) {
            return items.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            items.CopyTo(array ,arrayIndex);
        }

        public int Count {
            get { return items.Count; }
        }

        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item) {
            T peek = Peek();
            
            if (peek == null) {
                if (item != null)
                    return false;
            } else if (!Peek().Equals(item)) {
                return false;
            }

            Pop();
            return true;
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return items.GetEnumerator();
        }

        #endregion
    }
}

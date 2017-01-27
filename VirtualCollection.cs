using System;
using System.Collections.Generic;
using System.Text;

namespace iLab
{
    public class VirtualCollection<T>:IList<T>
    {
        List<T> items;

        public VirtualCollection() {
            items = new List<T>();
        }
        public VirtualCollection(int capacity) {
            items = new List<T>(capacity);
        }
        public VirtualCollection(IEnumerable<T> collection) {
            collection = new List<T>(collection);
        }

        public T[] ToArray() {
            return items.ToArray();
        }

        #region IList<T> Members

        public virtual int IndexOf(T item) {
            return items.IndexOf(item);
        }

        public virtual void Insert(int index, T item) {
            if (readOnly)
                throw new InvalidOperationException("This collection is read-only.");

            items.Insert(index, item);
        }

        public virtual void RemoveAt(int index) {
            if (readOnly)
                throw new InvalidOperationException("This collection is read-only.");

            items.RemoveAt(index);
        }

        public virtual T this[int index] {
            get {
                return items[index];
            }
            set {
                if (readOnly)
                    throw new InvalidOperationException("This collection is read-only.");

                items[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public virtual void Add(T item) {
            if (readOnly)
                throw new InvalidOperationException("This collection is read-only.");

            items.Add(item);
        }

        public virtual void Clear() {
            if (readOnly)
                throw new InvalidOperationException("This collection is read-only.");

            items.Clear();
        }

        public virtual bool Contains(T item) {
            return items.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex) {
            items.CopyTo(array, arrayIndex);
        }

        public virtual int Count {
            get {
                return items.Count;
            }
        }

        bool readOnly;
        public virtual bool IsReadOnly {
            get { return readOnly; }
            protected set { readOnly = value; }
        }

        public virtual bool Remove(T item) {
            if(readOnly)
                throw new InvalidOperationException("This collection is read-only.");

            return items.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public virtual IEnumerator<T> GetEnumerator() {
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion
    }

    public class IListEnumerator<T>:IEnumerator<T>
    {
        public IListEnumerator(IList<T> items) {
            this.items = items;
        }

        int index = -1;
        IList<T> items;
        #region IEnumerator<T> Members

        public T Current { get { return items[index]; } }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            items = null;
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current { get { return Current; } }

        public bool MoveNext() {
            return ++index < items.Count;
        }

        public void Reset() {
            index = -1;
        }

        #endregion
    }
}

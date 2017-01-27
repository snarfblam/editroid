using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    public class LevelCollection:IDictionary<LevelIndex, Level>
    {
        Level brinstar, norfair, ridley, kraid, tourian;

        Level Brinstar { get { return brinstar; } }
        Level Ridley { get { return ridley; } }
        Level Kraid { get { return kraid; } }
        Level Tourian { get { return tourian; } }
        Level Norfair { get { return norfair; } }

        static LevelIndex[] indecies = new LevelIndex[] { LevelIndex.Brinstar, LevelIndex.Kraid, LevelIndex.Norfair, LevelIndex.Ridley, LevelIndex.Tourian, };
        Level[] values;

        public LevelCollection(MetroidRom rom) {
            brinstar = new Level(rom, LevelIndex.Brinstar);
            ridley = new Level(rom, LevelIndex.Ridley);
            norfair = new Level(rom, LevelIndex.Norfair);
            kraid = new Level(rom, LevelIndex.Kraid);
            tourian = new Level(rom, LevelIndex.Tourian);

            values = new Level[] { brinstar, norfair, tourian, kraid, ridley, };
        }


        #region IDictionary<LevelIndex,Level> Members

        public void Add(LevelIndex key, Level value) {
            throw new InvalidOperationException("This collection is read-only.");
        }

        public bool ContainsKey(LevelIndex key) {
            return ((IList<LevelIndex>)indecies).Contains(key);
        }


        public ICollection<LevelIndex> Keys { get { return indecies; } }

        public bool Remove(LevelIndex key) {
            throw new InvalidOperationException("This collection is read-only.");
        }

        public bool TryGetValue(LevelIndex key, out Level value) {
            if (ContainsKey(key)) {
                value = this[key];
                return true;
            }

            value = null;
            return false;
        }

        public ICollection<Level> Values { get { return values; } }

        public Level this[LevelIndex key] {
            get {
                return values[(int)key];
            }
            set {
                throw new InvalidOperationException("This collection is read-only.");
            }
        }
        //public Level this[int index] {
        //    get {
        //        return values[index];
        //    }
        //    set {
        //        throw new InvalidOperationException("This collection is read-only.");
        //    }
        //}

        #endregion

        #region ICollection<KeyValuePair<LevelIndex,Level>> Members

        public void Add(KeyValuePair<LevelIndex, Level> item) {
            throw new InvalidOperationException("This collection is read-only.");
        }

        public void Clear() {
            throw new InvalidOperationException("This collection is read-only.");
        }

        public bool Contains(KeyValuePair<LevelIndex, Level> item) {
            Level l;
            if (ContainsKey(item.Key)) {
                return this[item.Key] == item.Value;
            }

            return false;
        }

        public void CopyTo(KeyValuePair<LevelIndex, Level>[] array, int arrayIndex) {
            array[arrayIndex] = new KeyValuePair<LevelIndex, Level>(LevelIndex.Brinstar, brinstar);
            array[arrayIndex + 1] = new KeyValuePair<LevelIndex, Level>(LevelIndex.Norfair, norfair);
            array[arrayIndex + 2] = new KeyValuePair<LevelIndex, Level>(LevelIndex.Tourian, Tourian);
            array[arrayIndex + 3] = new KeyValuePair<LevelIndex, Level>(LevelIndex.Kraid, kraid);
            array[arrayIndex + 4] = new KeyValuePair<LevelIndex, Level>(LevelIndex.Ridley, ridley);
        }

        public int Count { get { return 5; } }

        public bool IsReadOnly { get { return true; } }

        public bool Remove(KeyValuePair<LevelIndex, Level> item) {
            throw new InvalidOperationException("This collection is read-only.");
        }

        #endregion

        #region IEnumerable<KeyValuePair<LevelIndex,Level>> Members

        public IEnumerator<KeyValuePair<LevelIndex, Level>> GetEnumerator() {
            KeyValuePair<LevelIndex, Level>[] items = new KeyValuePair<LevelIndex,Level>[5];
            CopyTo(items, 0);

            return ((IList<KeyValuePair<LevelIndex, Level>>)items).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion


    }
}

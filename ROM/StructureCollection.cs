using System;
using System.Collections.Generic;
using System.Text;
using iLab;
using System.IO;

namespace Editroid.ROM
{
    public class StructureCollection : IList<Structure>, IRomDataParentObject
    {
        Structure[] structs;

        Level level;
        public StructureCollection(Level level) {
            this.level = level;

            LoadStructs();
        }

        public Bank PtableBank { get; private set; }
        public Bank DataBank { get; private set; }

        private void LoadStructs() {
            PtableBank = level.Format.StructPtableBank;
            DataBank = level.Format.StructDataBank;
            structs = new Structure[level.Format.CalculateStructCount()];

            MemoryStream stream = new MemoryStream(level.Rom.data);

            // Get the location of the first struct pointer
            pCpu ppStructTable = level.Format.pPointerToStructPTable;
            pCpu pStructTable = level.Bank.GetPtr(ppStructTable);

            // For all struct pointers
            for (int i = 0; i < structs.Length; i++) {
                // Load the pointer
                pCpu pStruct = PtableBank.GetPtr(pStructTable + i * 2);

                // Resolve the pointer to an offset and use that to load the struct
                pRom StructOffset = DataBank.ToOffset(pStruct);
                structs[i] = new Structure(level, i);
                //structs[i].Offset = StructOffset;
                stream.Seek((int)StructOffset, SeekOrigin.Begin);
                structs[i].LoadData(stream);
            }
        }

        public int IndexOf(Structure item) {
            for (int i = 0; i < Count; i++) {
                if (this[i] == item)
                    return i;
            }

            return -1;
        }

        void IList<Structure>.Insert(int index, Structure item) {
            throw new InvalidOperationException("This method is not supported.");
        }

        void IList<Structure>.RemoveAt(int index) {
            throw new InvalidOperationException("This method is not supported.");
        }

        public Structure this[int index] {
            get {
                return structs[index];
            }
        }

        Structure IList<Structure>.this[int index]{
            get {
                return structs[index];
            }
            set {
                throw new InvalidOperationException("Collection is read only.");
            }
        }

        public void Add(Structure item) {
            throw new InvalidOperationException("This method is not supported.");
        }

        public void Clear() {
            throw new InvalidOperationException("This method is not supported.");
        }

        public bool Contains(Structure item) {
            throw new Exception("The method or operation is not implemented.");
        }

        public void CopyTo(Structure[] array, int arrayIndex) {
            for (int i = 0; i < Count; i++) {
                array[i] = this[i];
            }
        }

        public int Count { get { return structs.Length; } }

        public bool IsReadOnly { get { return true; } }

        public bool Remove(Structure item) {
            throw new InvalidOperationException("This method is not supported.");
        }

        public IEnumerator<Structure> GetEnumerator() {
            return new IListEnumerator<Structure>(this);
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #region IRomDataParentObject Members

        IList<IRomDataObject> IRomDataParentObject.GetSubItems() {
            return RomDataObjects.EmptyNode;
        }

        // Todo: fix this shitB
        IList<LineDisplayItem> IRomDataParentObject.GetListItems() {
            LineDisplayItem[] items = new LineDisplayItem[Count];

            for (int i = 0; i < Count; i++) {
                items[i] = new LineDisplayItem(
                    "Structure " + i.ToString("X"),
                    this[i].Offset,
                    this[i].Size,
                    level.Rom.data
                    );
            }

            return items;
        }

        int IRomDataObject.Offset { get { return this[0].Offset; } }
        int IRomDataObject.Size { get { return CalculateSize(); } }
        bool IRomDataObject.HasListItems { get { return true; } }
        bool IRomDataObject.HasSubItems { get { return false; } }
        string IRomDataObject.DisplayName { get { return "Structures"; } }

        #endregion

        private int CalculateSize() {
            Structure s = this[Count - 1];
            return (s.Offset - this[0].Offset) + s.Size;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns>Index of new structure</returns>
        internal int AddNew() {
            int newIndex = structs.Length;
            Structure[] newArray = new Structure[structs.Length + 1];
            Array.Copy(structs, newArray, structs.Length);
            structs = newArray;

            structs[newIndex] = Structure.CreateEmpty(level, newIndex);
            return newIndex;
        }

        internal void CropEntry(int deletedIndex) {
            Structure[] newArray = new Structure[structs.Length - 1];
            // Copy items from deletedIndex back
            Array.Copy(structs, newArray, structs.Length - 1);
            
            // Copy items from deletedIndex forward.
            if (deletedIndex < (structs.Length - 1))
                Array.Copy(structs, deletedIndex + 1, newArray, deletedIndex, structs.Length - 1 - deletedIndex);

            for (int i = deletedIndex; i < newArray.Length; i++) {
                newArray[i].SetIndex(newArray[i].Index - 1);
            }
            structs = newArray;
        }
    }

}
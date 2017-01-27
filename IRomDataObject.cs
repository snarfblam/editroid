using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
    public static class RomDataObjects
    {
        public static readonly IList<LineDisplayItem> EmptyList = new LineDisplayItem[0];
        public static readonly IList<IRomDataObject> EmptyNode = new IRomDataObject[0];

        public static string FormatData(IRomDataObject obj, byte[] data) {
            StringBuilder result = new StringBuilder(obj.Size * 3);
            int offset = obj.Offset; ;
            int upperLimit = Math.Min(data.Length, offset + obj.Size);

            for (int i = offset; i < upperLimit; i++) {
                if (i != offset)
                    result.Append(' ');
                result.Append(data[i].ToString("X").PadLeft(2, '0'));
            }

            return result.ToString();
        }
    }

    public interface IRomDataObject
    {
        int Offset { get;}
        int Size { get;}
        bool HasListItems { get;}
        bool HasSubItems { get;}
        string DisplayName { get;}
    }
    public interface IRomDataObjectEx
    {
        bool Contiguous { get; }
    }
    public interface IRomDataParentObject : IRomDataObject
    {
        IList<IRomDataObject> GetSubItems();
        IList<LineDisplayItem> GetListItems();
    }

    /// <summary>
    /// Base class for types that exist specifically for the Pointer explorer or
    /// to describe the layout of ROM data.
    /// </summary>
    abstract class RomDataObject : IRomDataObject
    {

        #region IRomDataObject Members

        public abstract int Offset { get; }

        public abstract int Size { get; }

        public bool HasListItems { get { return false; } }

        public bool HasSubItems { get { return false; } }

        public abstract string DisplayName { get; }

        #endregion
    }
        /// <summary>
    /// Base class for types that exist specifically for the Pointer explorer or
    /// to describe the layout of ROM data.
    /// </summary>
    abstract class RomDataParentObject : IRomDataParentObject
    {

        #region IRomDataObject Members

        public abstract int Offset { get; }

        public abstract int Size { get; }

        public abstract bool HasListItems { get; }

        public abstract bool HasSubItems { get; }

        public abstract string DisplayName { get; }

        #endregion

        #region IRomDataParentObject Members

        public virtual IList<IRomDataObject> GetSubItems() { return RomDataObjects.EmptyNode; }


        public virtual IList<LineDisplayItem> GetListItems() { return RomDataObjects.EmptyList; }

        #endregion
    }
    public struct LineDisplayItem
    {
        public string text;
        public int offset;
        public int length;
        public byte[] data;

        /// <summary>
        /// True indicates that the offset and length identify a section in data.
        /// False indicates that offset and length are for display only and data should be read
        /// from the beginning of the array.
        /// </summary>
        bool dataIsSection;

        /// <summary>
        /// Creates display item for a section of the specified data.
        /// </summary>
        public LineDisplayItem(string text, int offset, int length, byte[] data)
            : this(text, offset, length, data, true) {
        }

        /// <summary>
        /// Creates display item.
        /// </summary>
        public LineDisplayItem(string text, int offset, int length, byte[] data, bool dataIsSection) {
            this.text = text;
            this.offset = offset;
            this.length = length;
            this.data = data;
            this.dataIsSection = dataIsSection;
        }



        public string FormatData() {
            StringBuilder result = new StringBuilder(length * 3);
            int offset = dataIsSection ? this.offset : 0;
            int upperLimit = Math.Min(data.Length, offset + this.length);

            for (int i = offset; i < upperLimit; i++) {
                if(i !=offset)
                    result.Append(' ');
                result.Append(data[i].ToString("X").PadLeft(2, '0'));
            }

            return result.ToString();
        }
    }

}

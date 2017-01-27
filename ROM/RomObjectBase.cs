using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid.ROM
{
    /// <summary>
    /// Inherit this class to implement a ROM.
    /// </summary>
    abstract class RomObject
    {
        public RomObject(pRom o) {
            Offset = o;
        }
        public RomObject() {
            Offset = pRom.Null;
        }
        public abstract bool EditsInPlace { get; }
        public virtual bool ReadOnly { get { return false; } }
        public pRom Offset { get; protected set; }
        public virtual int Size { get; protected set; }
        public RomObject Owner { get { return GetOwner(); } }
        protected virtual RomObject GetOwner() { return null; }

    }

    /// <summary>
    /// Provides a base for objects that access or modify a ROM.
    /// </summary>
    /// <typeparam name="TOwner"></typeparam>
    abstract class RomObjectBase<TOwner> : RomObject
        where TOwner : RomObject
    {

        sealed protected override RomObject GetOwner() {
            return Owner;
        }
        public new TOwner Owner { get; protected set; }

        public RomObjectBase(TOwner owner, pRom offset)
            : base(offset) {
            this.Owner = owner;
        }
        public RomObjectBase(TOwner owner)
            : base() {
            this.Owner = owner;
        }

        public RomObject Rom {
            get {
                RomObject rom = this;
                while (rom.Owner != null)
                    rom = rom.Owner;

                return rom;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace iLab.Sbl
{
    /// <summary>
    /// Represents a structured binary node.
    /// </summary>
    public abstract class SblNode: IEnumerable<SblNode>
    {
        /// <summary>
        /// The name of this node.
        /// </summary>
        public string Name;

        /// <summary>
        /// Returns an SblNode, including any of its subnodes, read from a stream.
        /// </summary>
        /// <param name="s">The stream to read a node from.</param>
        /// <returns>An SblNode.</returns>
        public static SblNode FromStream(Stream s) {
            return FromBinary(new BinaryReader(s));
        }
        /// <summary>
        /// Reads SblNodes from a stream until the end of the stream is encountered.
        /// </summary>
        /// <param name="s">The stream from which to read nodes.</param>
        /// <returns>T group containing all the read nodes.</returns>
        /// <remarks>This overload requires that the stream allows the Reposition and Length properties to be
        /// read.</remarks>
        public static SblNode GroupFromStream(Stream s) {
            SblGroup group = new SblGroup("StreamData");
            while(s.Position < s.Length) {
                group.Items.Add(FromStream(s));
            }

            return group;
        }

        #region AddItem()
        /// <summary>
        /// Adds an SBL value node to this node.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The value of the node.</param>
        /// <remarks>This method is provided for convenience and is not optimised for speed.</remarks>
        public SblNode AddItem(string name, int value) {
            SblGroup me = this as SblGroup;
            if(me == null)
                throw new InvalidOperationException("This operation is only valid on SBL groups.");

            me.Items.Add(new StrongSblValue<int>(name, value));
            return me.Items[me.Items.Count - 1];
        }
        /// <summary>
        /// Adds an SBL value node to this node.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The value of the node.</param>
        /// <remarks>This method is provided for convenience and is not optimised for speed.</remarks>
        public SblNode AddItem(string name, string value) {
            SblGroup me = this as SblGroup;
            if(me == null)
                throw new InvalidOperationException("This operation is only valid on SBL groups.");

            me.Items.Add(new StrongSblValue<string>(name, value));
            return me.Items[me.Items.Count - 1];
        }
        /// <summary>
        /// Adds an SBL value node to this node.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The value of the node.</param>
        /// <remarks>This method is provided for convenience and is not optimised for speed.</remarks>
        public SblNode AddItem(string name, char value) {
            SblGroup me = this as SblGroup;
            if(me == null)
                throw new InvalidOperationException("This operation is only valid on SBL groups.");

            me.Items.Add(new StrongSblValue<char>(name, value));
            return me.Items[me.Items.Count - 1];
        }
        /// <summary>
        /// Adds an SBL value node to this node.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The value of the node.</param>
        /// <remarks>This method is provided for convenience and is not optimised for speed.</remarks>
        public SblNode AddItem(string name, long value) {
            SblGroup me = this as SblGroup;
            if(me == null)
                throw new InvalidOperationException("This operation is only valid on SBL groups.");

            me.Items.Add(new StrongSblValue<long>(name, value));
            return me.Items[me.Items.Count - 1];
        }
        /// <summary>
        /// Adds an SBL value node to this node.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The value of the node.</param>
        /// <remarks>This method is provided for convenience and is not optimised for speed.</remarks>
        public SblNode AddItem(string name, float value) {
            SblGroup me = this as SblGroup;
            if(me == null)
                throw new InvalidOperationException("This operation is only valid on SBL groups.");

            me.Items.Add(new StrongSblValue<float>(name, value));
            return me.Items[me.Items.Count - 1];
        }
        /// <summary>
        /// Adds an SBL value node to this node.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The value of the node.</param>
        /// <remarks>This method is provided for convenience and is not optimised for speed.</remarks>
        public SblNode AddItem(string name, byte value) {
            SblGroup me = this as SblGroup;
            if(me == null)
                throw new InvalidOperationException("This operation is only valid on SBL groups.");

            me.Items.Add(new StrongSblValue<byte>(name, value));
            return me.Items[me.Items.Count - 1];
        }
        /// <summary>
        /// Adds an SBL value node to this node.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The value of the node.</param>
        /// <remarks>This method is provided for convenience and is not optimised for speed.</remarks>
        public SblNode AddItem(string name, byte[] value) {
            SblGroup me = this as SblGroup;
            if(me == null)
                throw new InvalidOperationException("This operation is only valid on SBL groups.");

            me.Items.Add(new StrongSblValue<byte[]>(name, value));
            return me.Items[me.Items.Count - 1];
        }
        #endregion


        /// <summary>
        /// Reads the specified number of SblNodes from a stream.
        /// </summary>
        /// <param name="s">The stream from which to read nodes.</param>
        /// <param name="nodeCount">The number of nodes to read.</param>
        /// <returns>T group containing all the read nodes.</returns>
        public static SblNode GroupFromStream(Stream s, int nodeCount) {
            SblGroup group = new SblGroup("StreamData");
            for(int i = 0; i < nodeCount; i++){
                group.Items.Add(FromStream(s));
            }

            return group;
        }
        private static SblNode FromBinary(BinaryReader b) {
            DataType type = (DataType)(b.ReadByte());
            switch(type) {
                case DataType.Group:
                    SblGroup newGroup = new SblGroup(b.ReadString());
                    int itemCount = b.ReadInt32();
                    for(int i = 0; i < itemCount; i++) {
                        newGroup.Items.Add(SblNode.FromBinary(b));
                    }

                    return newGroup;
                case DataType.Int32:
                    return new StrongSblValue<int>(b.ReadString(), b.ReadInt32());
                case DataType.Int64:
                    return new StrongSblValue<long>(b.ReadString(), b.ReadInt64());
                case DataType.Int8:
                    return new StrongSblValue<byte>(b.ReadString(), b.ReadByte());
                case DataType.Float:
                    return new StrongSblValue<float>(b.ReadString(), b.ReadSingle());
                case DataType.Date:
                    return new StrongSblValue<DateTime>(b.ReadString(), DateTime.FromBinary(b.ReadInt64()));
                case DataType.Binary:
                    string name = b.ReadString();
                    int count = b.ReadInt32();
                    return new StrongSblValue<byte[]>(name, b.ReadBytes(count));
                case DataType.Char:
                    return new StrongSblValue<char>(b.ReadString(), b.ReadChar());
                case DataType.String:
                    return new StrongSblValue<string>(b.ReadString(), b.ReadString());
                default:
                    throw new InvalidDataException("Invalid data was encountered when reading SBL data from a stream.");
            }
        }

        /// <summary>
        /// Writes this node to a stream.
        /// </summary>
        /// <param name="s">The stream to write to.</param>
        public void WriteToStream(Stream s) {
            WriteBinary(new BinaryWriter(s));
        }

        /// <summary>
        /// Creates a clone of this SblNode.
        /// </summary>
        /// <returns>a clone of this SblNode</returns>
        public abstract SblNode Clone();

        /// <summary>
        /// Gets the value of this node if it is a value node. If it is not a value node a type cast exception will be thrown.
        /// </summary>
        public Object Value {
            get {
                return ((SblValue)this).Value;
            }
            set {
                ((SblValue)this).Value = value;
            }
        }

        /// <summary>
        /// Gets the value of a subnode if this node is an SblGroup. If this is an SblValue or the gameItem specified is an SblGroup a type
        /// cast exception will be thrown. If the specified node is not found a NullReferenceException will be thrown.
        /// </summary>
        /// <param name="name">The name of the node from which to retrieve a value.</param>
        /// <returns>T value if the specified name is unique to a single node which is an SblValue node.</returns>
        public Object GetValue (string name){
            SblNode target = ((SblGroup)this)[name];
            return target == null?
                null:
                target.Value;
        }
        /// <summary>
        /// Gets the value of a subnode if this node is an SblGroup. If this is an SblValue or the gameItem specified is an SblGroup a type
        /// cast exception will be thrown. If the specified node is not found a NullReferenceException will be thrown.
        /// </summary>
        /// <param name="name">The name of the node from which to retrieve a value.</param>
        /// <param name="ignoreCase">Whether or not to ignore the casing of the node name.</param>
        /// <returns>T value if the specified name is unique to a single node which is an SblValue node.</returns>
        public Object GetValue(string name, bool ignoreCase) {
            return ((SblGroup)this).GetSblNode(name, ignoreCase).Value;
        }
        /// <summary>
        /// Gets the value of a subnode if this node is an SblGroup. If this is an SblValue a type
        /// cast exception will be thrown. If the specified node is not found the default value is returned.
        /// </summary>
        /// <param name="name">The name of the node from which to retrieve a value.</param>
        /// <param name="defaultValue">The value returned if the specified value is not found.</param>
        /// <returns>T value if the specified name is unique to a single node which is an SblValue node.</returns>
        public Object GetValue(string name, object defaultValue) {
            SblNode target = ((SblGroup)this)[name];
            return target == null ?
                defaultValue :
                (target.Value == null ?
                    defaultValue :
                    target.Value);
        }
        /// <summary>
        /// Gets the value of a subnode if this node is an SblGroup. If this is an SblValue a type
        /// cast exception will be thrown. If the specified node is not found the default value is returned.
        /// </summary>
        /// <param name="name">The name of the node from which to retrieve a value.</param>
        /// <param name="defaultValue">The value returned if the specified value is not found.</param>
        /// <param name="ignoreCase">Whether or not to ignore the casing of the node name.</param>
        /// <returns>T value if the specified name is unique to a single node which is an SblValue node.</returns>
        public Object GetValue(string name, object defaultValue, bool ignoreCase) {
            SblNode target = ((SblGroup)this).GetSblNode(name, ignoreCase);
            return target == null ?
                defaultValue :
                (target.Value == null ?
                    defaultValue :
                    target.Value);

        }


        /// <summary>
        /// Sets the value of a subnode if this node is an SblGroup. If this is an SblValue or the gameItem specified is an SblGroup a type
        /// cast exception will be thrown. If there are multiple nodes with the specified name an exception will be thrown. If no
        /// node exists with the specified name, the node will be created.
        /// </summary>
        /// <param name="name">The name of the value to set.</param>
        /// <param name="value">The value to set.</param>
        public void SetValue(string name, object value) {
            SblGroup group = (SblGroup)this;
            SblNode target = null;
            for(int i = 0; i < ((SblGroup)this).Items.Count; i++) {
                if(group.Items[i].Name.Equals(name)) {
                    if(target == null)
                        target = Items[i];
                    else
                        throw new InvalidOperationException("The specified name resulted in an ambiguous match.");
                }
            }
            if(target == null) 
                Items.Add(new WeakSblValue(name, value));
            else
                target.Value = value;
        }

                /// <summary>
        /// Sets the value of a subnode if this node is an SblGroup. If this is an SblValue or the gameItem specified is an SblGroup a type
        /// cast exception will be thrown. If there are multiple nodes with the specified name an exception will be thrown. If no
        /// node exists with the specified name, the node will be created.
        /// </summary>
        /// <param name="name">The name of the value to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="ignoreCase">Whether or not to ignore the casing of the node name.</param>
        public void SetValue(string name, object value, bool ignoreCase) {
                    SblGroup group = (SblGroup)this;
            SblNode target = null;
            for(int i = 0; i < ((SblGroup)this).Items.Count; i++) {
                if(group.Items[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
                    if(target == null)
                        target = Items[i];
                    else
                        throw new InvalidOperationException("The specified name resulted in an ambiguous match.");
                }

                target.Value = value;
            }
        }
        /// <summary>
        /// Gets the list of currentLevelItems in this node if this node is a group. If it is not a group a type cast exception will be thrown.
        /// </summary>
        public List<SblNode> Items {
            get {
                return ((SblGroup)this).Items;
            }
        }

        /// <summary>
        /// Writes ths node to a BinaryWriter.
        /// </summary>
        /// <param name="binaryReader">Stream to write to.</param>
        protected abstract void WriteBinary(BinaryWriter binaryReader);
        internal void WriteToBinary(BinaryWriter b) { WriteBinary(b); }

        /// <summary>
        /// Gets the standard SBL data type this object represents. T loosely typed SblValue
        /// object with a null value will throw an exception.
        /// </summary>
        public abstract DataType DataType { get;  }

        /// <summary>
        /// Gets a subnode by name if this is a group. For a case-insensitive search use the GetValue method.
        /// </summary>
        /// <param name="name">The name of the node to search for.</param>
        /// <returns>Null if no matching node is found or if the match is ambiguous, otherwise an SblNode.</returns>
        public virtual SblNode this[string name] {
            get {
                throw new NotImplementedException("This operation is not valid on this type of node.");
            }
        }
        /// <summary>
        /// Gets a sub-node by count if this is a group node.
        /// </summary>
        /// <param name="count">The count of the node to get.</param>
        /// <returns>Am SblNode at the specified count.</returns>
        public virtual SblNode this[int index] {
            get {
                throw new NotImplementedException("This operation is not valid on this type of node.");
            }
        }

        #region IEnumerable<SblNode> Members

        /// <summary>
        /// Gets an enumerator for this node.
        /// </summary>
        /// <returns>An IEnumerator.</returns>
        public IEnumerator<SblNode> GetEnumerator() {
            return new SblNodeEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        /// <summary>Gets an enumerator.</summary>
        /// <returns>an enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return new SblNodeEnumerator(this);
        }

        #endregion
    }

    /// <summary>Implements an enumerator for nodes.</summary>
    public class SblNodeEnumerator: IEnumerator<SblNode>{
        SblGroup group;
        int index = -1;
        /// <summary>Creates an SblNodeEnumerator.</summary>
        /// <param name="container">The node whose subnodes to enumerate.</param>
        public SblNodeEnumerator(SblNode container) {
            group = container as SblGroup;
            if(group == null) throw new ArgumentException("Container must be an SblGroup object.", "container");
        }

        #region IEnumerator<SblNode> Members

        /// <summary>Gets the current object in the enumeration.</summary>
        public SblNode Current {
            get { return group[index]; }
        }

        #endregion

        #region IDisposable Members
        /// <summary>Disposes of this object.</summary>
        public void Dispose() {
            group = null;
        }

        #endregion


        #region IEnumerator Members

        object IEnumerator.Current {
            get { return Current; }
        }

        /// <summary>Selects the next object to be enumerated.</summary>
        /// <returns>the next object to be enumerated</returns>
        public bool MoveNext() {
            index++;
            return (index) < group.Items.Count;
        }

        /// <summary>Resets the enumerator to the beginning.</summary>
        public void Reset() {
            index = -1;
        }

        #endregion
    }

    /// <summary>
    /// Represents a SBL node that contains other SBL nodes.
    /// </summary>
    public class SblGroup: SblNode
    {

        /// <summary>
        /// Gets a node by name. There must be a node with a unique name that matches the name
        /// parameter. If there are zero or more than one matching results no node will be returned.
        /// </summary>
        /// <param name="name">The name of the node to return.</param>
        /// <returns>T node if there is one and only one node that matches the specified name, or null otherwise.</returns>
        public override SblNode this[string name] {
            get {
                SblNode result = null;
                for(int i = 0; i < Items.Count; i++) {
                    if(Items[i].Name.Equals(name)) {
                        if(result != null) // Ambiguous match
                            return null;
                        result = Items[i];
                    }
                }

                return result;
            }
        }

        /// <summary>Indexer.</summary>
        /// <param name="i">Index.</param>
        /// <returns>The node with the specified count.</returns>
        public override SblNode this[int i] {
            get {
                return Items[i];
            }
        }


        /// <summary>
        /// Gets a node by name. There must be a node with a unique name that matches the name
        /// parameter. If there are zero or more than one matching results no node will be returned.
        /// </summary>
        /// <param name="name">The name of the node to return.</param>
        /// <param name="ignoreCase">Whether or not to take text casing into account.</param>
        /// <returns>T node if there is one and only one node that matches the specified name, or null otherwise.</returns>
        public SblNode GetSblNode(string name, bool ignoreCase) {
            if(ignoreCase)
                return GetSblNodeCaseInsensitive(name);
            else
                return GetSblNodeCaseSensitive(name);
        }

        private SblNode GetSblNodeCaseSensitive(string name) {
            SblNode result = null;
            for(int i = 0; i < Items.Count; i++) {
                if(Items[i].Name.Equals(name)) {
                    if(result != null) // Ambiguous match
                        return null;
                    result = Items[i];
                }
            }

            return result;
        }

        private SblNode GetSblNodeCaseInsensitive(string name) {
            SblNode result = null;
            for(int i = 0; i < Items.Count; i++) {
                if(Items[i].Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
                    if(result != null) // Ambiguous match
                        return null;
                    result = Items[i];
                }
            }

            return result;
        }
            /// <summary>
            /// The collection of currentLevelItems within this group.
            /// </summary>
            public new List<SblNode> Items;

        /// <summary>
        /// Creates an SblGroup.
        /// </summary>
        public SblGroup() {
            Items = new List<SblNode>();
        }

        /// <summary>
        /// Creates an SblGroup.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        public SblGroup(string name) {
            Name = name;
            Items = new List<SblNode>();
        }
        
        /// <summary>
        /// Creates and initializes an SblGroup.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="subItems">An enumerable object that represents a list of subitems.</param>
        public SblGroup(string name, IEnumerable subItems) {
            Name = name;
            Items = new List<SblNode>();
            foreach(object o in subItems) {
                SblNode node = o as SblNode;
                if(node != null) {
                    Items.Add(node);
                }
            }
        }
        /// <summary>
        /// Creates and initializes an SblGroup.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="subItems">An enumerable object that represents a list of subitems.</param>
        public SblGroup(string name, IEnumerable<SblNode> subItems) {
            Name = name;
            Items = new List<SblNode>(subItems);
        }


        /// <summary>Serializes this node to a BinaryWriter.</summary>
        /// <param name="b_UTF32">T BinaryWriter to write to.</param>
        protected override void WriteBinary(BinaryWriter b) {
            b.Write((Byte)DataType.Group);
            b.Write(Name);
            b.Write(Items.Count);
            foreach(SblNode node in Items) {
                node.WriteToBinary(b);
            }
        }

        /// <summary>
        /// Gets the standard SBL data type this object represents. T loosely typed SblValue
        /// object with a null value will throw an exception.
        /// </summary>
        public override DataType DataType {
            get { return DataType.Group; }
        }

        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>T string representation of this object.</returns>
        public override string ToString() {
            return "{SBL Group: " + Name + "}";
        }

        /// <summary>Creates a deep copy of this object.</summary>
        /// <returns>a deep copy of this object</returns>
        public override SblNode Clone() {
            SblGroup result = new SblGroup(Name);
            foreach(SblNode item in Items){
                result.Items.Add(item.Clone());
            }

            return result;
        }
    }
    
    /// <summary>
    /// Represents an SBL node that contains a value.
    /// </summary>
    public abstract class SblValue:SblNode
    {
        /// <summary>
        /// Gets/sets the value of this node.
        /// </summary>
        public new abstract object Value { get;set;}

        /// <summary>
        /// Gets the standard SBL data type this object represents. T loosely typed SblValue
        /// object with a null value will throw an exception.
        /// </summary>
        public override DataType DataType {
            get {
                // Generic values that can have null values
                if(this is StrongSblValue<string>) return DataType.String;
                if(this is StrongSblValue<byte[]>) return DataType.Binary;

                // Get value. T null value indicates that this is a loosely typed value object with
                // no value, which means that the type can not be inferred.
                object val = Value;
                if(val == null) throw new InvalidOperationException("This node has no value and therefore no type.");

                if(val is string) return DataType.String;
                if(val is Int64) return DataType.Int64;
                if(val is Int32) return DataType.Int32;
                if(val is byte) return DataType.Int8;
                if(val is char) return DataType.Char;
                if(val is byte[]) return DataType.Binary;
                if(val is float) return DataType.Float;
                if(val is DateTime) return DataType.Date;
                throw new Exception("The value of this SblValue is not a standard SBL type.");
            }
        }
        /// <summary>
        /// Creates a strongly typed SblValue.
        /// </summary>
        /// <typeparam name="T">The type of the SblValue.</typeparam>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The node's value.</param>
        /// <returns>T strongly typed SblValue.</returns>
        public SblValue MakeValue<T>(string name, T value) {
            return new StrongSblValue<T>(name, value);
        }
        /// <summary>
        /// Creates a loosely typed SblValue.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The node's value.</param>
        /// <returns></returns>
        public SblValue MakeValue(string name, object value) {
            return new WeakSblValue(name, value);
        }

        /// <summary>Serializes this object to a BinaryWriter.</summary>
        /// <param name="b_UTF32">The BinaryWriter to serialize to.</param>
        protected override void WriteBinary(BinaryWriter b) {
            DataType type = this.DataType;
            b.Write((byte)type);
            b.Write(Name);

            // We can assume that the type of Value is correct because
            // any inconsistancies will throw an exception when the DataType property is
            // read.
            switch(type) {
                case DataType.Int32:
                    b.Write((Int32)Value);
                    break;
                case DataType.Int64:
                    b.Write((Int64)Value);
                    break;
                case DataType.Int8:
                    b.Write((byte)Value);
                    break;
                case DataType.Float:
                    b.Write((float)Value);
                    break;
                case DataType.Date:
                    b.Write(((DateTime)Value).ToBinary());
                    break;
                case DataType.Binary:
                    if(Value == null)
                        b.Write(0);
                    else {
                        Byte[] binary = (Byte[])Value;
                        b.Write(binary.Length);
                        b.Write(binary, 0, binary.Length);
                    }
                    break;
                case DataType.Char:
                    b.Write((char)Value);
                    break;
                case DataType.String:
                    if(Value == null) {
                        b.Write(string.Empty);
                    } else {
                        b.Write((string)Value);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>T string representation of this object.</returns>
        public override string ToString() {
            object val = Value;
            if(val == null) {
                return "{SBL: " + Name + " = <no value>}";
            } else {
                return "{SBL: " + Name + " = (" + DataType.ToString() + ")" + Value.ToString() + "}";
            }
        }


    }

    /// <summary>
    /// An SblValue class that implements generics to be strongly typed.
    /// </summary>
    /// <remarks>This class can be more memory efficient and may offer better performance, but at
    /// the cost of static typing.</remarks>
    /// <typeparam name="T">The type that this value must have.</typeparam>
    public class StrongSblValue<T>:SblValue
    {
        T value;
        /// <summary>
        /// Gets/sets the value of this node.
        /// </summary>
        public override object Value {
            get {
                return value;
            }
            set {
                this.value = (T)value;
            }
        }

        /// <summary>
        /// Creates a typeSblValue
        /// </summary>
        public StrongSblValue() { }
        /// <summary>
        /// Creates and initialized a typeSbleValue.
        /// </summary>
        /// <param name="name">This node's name.</param>
        /// <param name="value">This node's value.</param>
        public StrongSblValue(string name, T value) {
            Name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets/sets this SblNode's value with a strongly typed accessor.
        /// </summary>
        public T TypedValue { get { return value; } set { this.value = value; } }

        /// <summary>Creates a copy of this SBL Node.</summary>
        /// <returns></returns>
        public override SblNode Clone() {
            return new StrongSblValue<T>(Name, value);
        }
    }
    
    /// <summary>
    /// An Sbl Value class that is loosely typed.
    /// </summary>
    /// <remarks>This class is flexible but does not offer strict typing, often
    /// requiring boxing and casting to be used.</remarks>
    public class WeakSblValue: SblValue
    {
        /// <summary>
        /// Creates an SblObjectValue.
        /// </summary>
        public WeakSblValue() { }

        /// <summary>
        /// Creates and initalizes an SblObjectValue.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="value">This node's value.</param>
        public WeakSblValue(string name, object value) {
            Name = name;
            val = value;
        }
        object val;
        /// <summary>
        /// Gets/sets the value of this node.
        /// </summary>
        public override object Value {
            get {
                return val;
            }
            set {
                val = value;
            }
        }

        /// <summary>Creates an clone of this node.</summary>
        /// <returns>An SBL Node.</returns>
        public override SblNode Clone() {
            return new WeakSblValue(Name, val);
        }
    }

    /// <summary>
    /// Represents the types allowable in SBL data.
    /// </summary>
    public enum DataType:byte
    {
        /// <summary>T node that contains subnodes.</summary>
        Group,
        /// <summary>T node that contains a 32-bit signed integer.</summary>
        Int32,
        /// <summary>T node that contains a 64-bit signed integer.</summary>
        Int64,
        /// <summary>T node that contains an 8-bit unsigned integer.</summary>
        Int8,
        /// <summary>T node that contains a floating-points number.</summary>
        Float,
        /// <summary>T node that contains a block of raw binary data.</summary>
        Binary,
        /// <summary>T node that contains a single character.</summary>
        Char,
        /// <summary>T node that contains a string of text.</summary>
        String,
        /// <summary>T node that contains a UTC date.</summary>
        Date
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.IO;
using System.Globalization;

namespace Windows
{
    public static class Utility
    {
        static Type IListType;
        static Encoding encoding;

        static Utility() {
            IListType = typeof(IList<>);
            
            int codePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
            if (codePage == 0) codePage = CultureInfo.InvariantCulture.TextInfo.ANSICodePage;
            encoding = Encoding.GetEncoding(codePage);
        }
        /// <summary>
        /// Creates a byte array contianing a structure with array references expanded to their contents. See remarks.
        /// </summary>
        /// <param name="t">The type of the structure.</param>
        /// <returns>The binary data of the structure.</returns>
        /// <remarks>
        /// <para>Valid Types: The specified object must be a structure. The only valid reference types 
        /// (including contents of arrays and their subfields, recursively)
        /// are strings, string builders, arrays and generic IList objects. ILists and 
        /// StringBuilders will be treated as arrays and strings, respectively.</para>
        /// 
        /// <para>Encoding: Strings will be written as ASNI strings unless 
        /// the field name ends with an encoding specifier (_UTF, _UTF7, _UTF8, _UTF32). Chars will
        /// always be ANSI. Store chars in a 1-char string field to specify encoding. Arrays of 
        /// strings will always be ANSI. Concatenate string arrays into a single-string composite
        /// field to specify encoding).</para>
        /// 
        /// Strings and arrays 
        /// will not be prefixed with their length. Invalid objects may be silently omitted.
        /// IHandle objects will be treated as IntPtrs.</remarks>
        public static Byte[] ExpandArraysInStruct<T>(T obj, out int insertedByteCount) where T : struct {
            Type type = typeof(T);
            IList<SortedFieldInfo> fields = SortedFieldData<T>.Data;
            MemoryStream result = new MemoryStream(1);
            BinaryWriter resultWriter = new BinaryWriter(result);
            int offsetAdjustment = 0;

            for (int i = 0; i < fields.Count; i++) {
                // Get data
                SortedFieldInfo field = fields[i];
                int targetOffset = field.offset + offsetAdjustment;
                object value = fields[i].field.GetValue(obj);

                // Seek to output offset
                while (result.Capacity < targetOffset)
                    result.Capacity *= 2;
                result.Seek(targetOffset, SeekOrigin.Begin);

                // Write
                if (value is StringBuilder) value = value.ToString();
                if (value is System.Enum) value = GetEnumValue(value);

                if (canWriteDirect(value)) {
                    outputField(resultWriter, value);
                } else if (fields[i].field.FieldType.IsArray) {
                    offsetAdjustment += outputList(resultWriter, fields[i].field.FieldType.GetElementType(), (System.Collections.IList)value);
                } else if (value is string || value is StringBuilder) {
                    if(field.field.Name.EndsWith("_UTF"))
                        offsetAdjustment += WriteStringUTF(resultWriter, value);
                    else if (field.field.Name.EndsWith("_UTF7"))
                        offsetAdjustment += WriteStringUTF7(resultWriter, value);
                    else if (field.field.Name.EndsWith("_UTF8"))
                        offsetAdjustment += WriteStringUTF8(resultWriter, value);
                    else if (field.field.Name.EndsWith("_UTF32"))
                        offsetAdjustment += WriteStringUTF32(resultWriter, value);
                    else
                        offsetAdjustment += WriteString(resultWriter, value);
                } else if (objectImplementsGenerigIList(value)) {
                    offsetAdjustment += outputList(resultWriter, value.GetType().GetGenericArguments()[0], (System.Collections.IList)value);
                } else if (value is ValueType) {
                    int extraBytes;
                    byte[] structData = ExpandArraysInStruct_Dynamic(value, out extraBytes);
                    result.Write(structData, 0, structData.Length);
                    offsetAdjustment += extraBytes;
                }
            }

            insertedByteCount = offsetAdjustment;
            result.Capacity = (int)result.Length;
            return result.GetBuffer();
        }

        private static int WriteStringUTF32(BinaryWriter resultWriter, object value) {
            if (value == null) return 0;

            byte[] data = Encoding.UTF32.GetBytes(value.ToString());
            resultWriter.Write(data, 0, data.Length);
            return data.Length;
        }

        private static int WriteStringUTF8(BinaryWriter resultWriter, object value) {
            if (value == null) return 0;

            byte[] data = Encoding.UTF8.GetBytes(value.ToString());
            resultWriter.Write(data, 0, data.Length);
            return data.Length;
        }

        private static int WriteStringUTF7(BinaryWriter resultWriter, object value) {
            if (value == null) return 0;

            byte[] data = Encoding.UTF7.GetBytes(value.ToString());
            resultWriter.Write(data, 0, data.Length);
            return data.Length;
        }

        private static int WriteStringUTF(BinaryWriter resultWriter, object value) {
            if (value == null) return 0;

            byte[] data = Encoding.Unicode.GetBytes(value.ToString());
            resultWriter.Write(data, 0, data.Length);
            return data.Length;
        }

        private static object GetEnumValue(object value) {
            return value.GetType().GetFields()[0].GetValue(value);
        }

        /// <summary>Outputs the items in the specified list.</summary>
        /// <param name="w"></param>
        /// <param name="elementType"></param>
        /// <param name="list">Must be an array or generic IList containing objects of the specified element type.</param>
        /// <returns>The number of bytes written</returns>
        private static int outputList(BinaryWriter w, Type elementType, System.Collections.IList list) {
            if (list == null) return 0;

            int result = 0;
            for (int i = 0; i < list.Count; i++) {
                object item = list[i];

                if (item is System.Enum) item = GetEnumValue(item);

                if (canWriteDirect(item)) {
                    result += outputField(w, item);
                } else if (elementType.IsArray) {
                    result += outputList(w, elementType.GetElementType(), (System.Collections.IList)item);
                } else if (item is string || item is StringBuilder) {
                    result += WriteString(w, item);
                } else if (objectImplementsGenerigIList(item)) {
                    result += outputList(w, item.GetType().GetGenericArguments()[0], (System.Collections.IList)item);
                } else if (item is ValueType) {
                    int extraBytes;
                    byte[] structData = ExpandArraysInStruct_Dynamic(item, out extraBytes);
                    w.Write(structData, 0, structData.Length);
                    result += structData.Length;
                }
            }

            return result;
        }
        /// <summary>Dynamicly performs a late-bound invocation of ExpandArraysInStruct
        /// using reflection.</summary>
        /// <param name="obj"></param>
        /// <param name="insertedByteCount"></param>
        /// <returns></returns>
        /// <exception cref="ReflectionException"></exception>
        public static Byte[] ExpandArraysInStruct_Dynamic(object obj, out int insertedByteCount) {
            MethodInfo method = typeof(Utility).GetMethod("ExpandArraysInStruct").GetGenericMethodDefinition();
            method = method.MakeGenericMethod(obj.GetType());
            object[] prams = new object[] { obj, 0 };

            object result = null;
            try {
                result = method.Invoke(null, prams);
            } catch (TargetInvocationException ex) {
                throw new ReflectionException("An exception was raised during reflection.", ex);
            }

            if (prams[1] is int && result is byte[]) {
                insertedByteCount = (int)prams[1];
                return (byte[])result;
            } else {
                throw new ReflectionException("An unexpected and invalid value was returned during reflection.", null);
            }
        }

        public class ReflectionException : ApplicationException
        {
            public ReflectionException(string message, Exception innerException)
                : base(message, innerException) { }
        }
        private static bool objectImplementsGenerigIList(object value) {
            Type t = value.GetType();
            Type[] interfaces = t.GetInterfaces();
            foreach (Type i in interfaces) {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>))
                    return true;
            }

            return false;
        }

        /// <summary></summary>
        /// <returns>The number of bytes written</returns>
        private static int WriteString(BinaryWriter resultWriter, object value) {
            if (value == null) return 0;

            byte[] data = encoding.GetBytes(value.ToString());
            resultWriter.Write(data, 0, data.Length);
            return data.Length;
        }



        static bool canWriteDirect(object o) {
            if (o is IHandle) return true;
            Type t = o.GetType();
            if (t.IsPrimitive) return true;
            return false;
        }
        /// <summary></summary>
        /// <returns>The number of bytes written</returns>
        private static int outputField(BinaryWriter w, object o) {
            if (o is IHandle)
                o = ((IHandle)o).Value;

            if (o is byte) {
                w.Write((byte)o);
                return 1;
            } else if (o is sbyte) {
                w.Write((sbyte)o);
                return 1;
            } else if (o is short) {
                w.Write((short)o);
                return 2;
            } else if (o is ushort) {
                w.Write((ushort)o);
                return 2;
            } else if (o is int) {
                w.Write((int)o);
                return 4;
            } else if (o is uint) {
                w.Write((uint)o);
                return 4;
            } else if (o is long) {
                w.Write((long)o);
                return 8;
            } else if (o is ulong) {
                w.Write((ulong)o);
                return 8;
            } else if (o is bool) {
                w.Write((bool)o);
                return 1;
            } else if (o is char) {
                w.Write(encoding.GetBytes(new char[] { (char)o })[0]); // Todo: this needs to be ANSI
                return 1;
            } else if (o is float) {
                w.Write((float)o);
                return 4;
            } else if (o is double) {
                w.Write((double)o);
                return 8;
            } else if (o is IntPtr) {
                w.Write(((IntPtr)o).ToInt32());
                return 4;
            } else if (o is UIntPtr) {
                w.Write(((UIntPtr)o).ToUInt32());
                return 4;
            }

            return 0;
        }

        static class SortedFieldData<T>
        {
            static List<SortedFieldInfo> data;
            private static int compactSize;
            public static int CompactSize {
                get { return compactSize; }
            }         
            private static int omittedBytes;
            public static int OmittedBytes {
                get { return omittedBytes; }
            }


            static SortedFieldData() {
                LoadData();
            }

            public static ReadOnlyCollection<SortedFieldInfo> Data {
                get {
                    return data.AsReadOnly();
                }
            }

            /// <summary>
            /// Examines the specified type and calculates necessary information 
            /// about the layout of the structure.
            /// </summary>
            private static void LoadData() {
                data = new List<SortedFieldInfo>();

                Type sortingType = typeof(T);

                // insert sort
                foreach (FieldInfo fi in sortingType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                    SortedFieldInfo info = new SortedFieldInfo();
                    info.field = fi;
                    info.offset = OffsetOf(fi);

                    int insertIndex = data.Count ;
                    while (insertIndex > 0 && info.offset <= data[insertIndex - 1].offset) {
                        insertIndex--;
                    }
                    data.Insert(insertIndex, info);

                }

                // Adjust reference type size to zero (arrays, lists, strings)
                int omittedBytes = 0;
                for (int i = 0; i < data.Count; i++) {
                    SortedFieldInfo dat = data[i];
                    int originalOffset = dat.offset;
                    dat.offset -= omittedBytes;
                    data[i] = dat;

                    Type type = dat.field.FieldType;
                    if (typeof(IHandle).IsAssignableFrom(type)) { // Handle
                        // Handle is 4 bytes
                        omittedBytes += (Marshal.SizeOf(type) - 4); 
                    } else if (type.IsSubclassOf(typeof(ValueType)) && !type.IsSubclassOf(typeof(System.Enum)) && !type.IsPrimitive) {
                        // Struct could have any # of omitted bytes
                        omittedBytes += getOmittedBytesFor_Dynamic(dat.field.FieldType);
                    } else if (!data[i].field.FieldType.IsValueType) {
                        // Ref is 4 bytes
                        if (i < data.Count - 1) {
                            // just in case of alignment issues
                            omittedBytes += data[i + 1].offset - originalOffset;
                        } else {
                            omittedBytes += 4;

                        }
                    }
                }
                compactSize = Marshal.SizeOf(typeof(T)) - omittedBytes;
                SortedFieldData<T>.omittedBytes = omittedBytes;
            }

            static int getOmittedBytesFor_Dynamic(Type t){
                return (int)typeof(SortedFieldData<>).MakeGenericType(new Type[] { t }).GetProperty("OmittedBytes").GetValue(null, null);
            }
        }

        static int OffsetOf(FieldInfo field) {
            return Marshal.OffsetOf(field.DeclaringType, field.Name).ToInt32();
        }

        struct SortedFieldInfo
        {

            public FieldInfo field;
            public int offset;
        }
    }
}

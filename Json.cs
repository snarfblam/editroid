using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Editroid
{
    class JsonObject : List<JsonProperty>
    {
        public static object Undefined = new object();

        public void Add(string name, object value) {
            //Add(new JsonProperty(name, value));
            Add(JsonProperty.FromObject(name, value));
            
        }

        /// <summary>
        /// Gets or sets a property on this object.
        /// </summary>
        /// <param name="name">Name of the property to get or set.</param>
        /// <returns>The value of the property, or JsonObject.Undefined if the property does not exist.</returns>
        public object this[string name] {
            get {
                for (int i = 0; i < Count; i++) {
                    if (strEqual(this[i].Name, name)) return this[i].Value;
                }

                return Undefined;
            }
            set {
                var index = IndexOf(name);
                if (index < 0) {
                    Add(name, value);
                } else {
                    this[index] = JsonProperty.FromObject(name, value);
                }
            }
        }

        public int IndexOf(string name) {
            return this.FindIndex(prop => strEqual(prop.Name, name));
        }

        public bool Remove(string name) {
            var index = IndexOf(name);
            if (index < 0) return false;

            RemoveAt(index);
            return true;
        }

        private static bool strEqual(string a, string b) {
            return string.Equals(a, b, StringComparison.Ordinal);
        }
    }
    class JsonProperty
    {
        public JsonProperty(string name, string value) {
            this.Name = name;
            this.Value = value;
        }
        public JsonProperty(string name, int value) {
            this.Name = name;
            this.Value = value;
        }
        public JsonProperty(string name, float value) {
            this.Name = name;
            this.Value = value;
        }
        public JsonProperty(string name, bool value) {
            this.Name = name;
            this.Value = value;
        }
        public JsonProperty(string name) {
            this.Name = name;
            this.Value = null;
        }
        public JsonProperty(string name, System.Collections.IList value) {
            this.Name = name;
            this.Value = value;
        }
        public JsonProperty(string name, JsonObject value) {
            this.Name = name;
            this.Value = value;
        }

        public static JsonProperty FromObject(string name, object value) {
            var result = new JsonProperty(name);
            result.Value = value;
            return result;
        }


        public string Name { get; private set; }
        /// <summary>
        /// Value of the property. See remarks.
        /// </summary>
        /// <remarks>
        /// If the value is an IList it will be treated as an array.
        /// The exception to this is JsonObject, which implements
        /// IList but is treated as an object.
        /// </remarks>
        public object Value { get; private set; }
    }

    public class JsonWriter
    {
        // Resource
        List<string> tabs = new List<string>();

        // Config
        TextWriter output;
        bool newlines = false;
        int indentSize = 4;

        // State
        int indentLevel = 0;

        public JsonWriter(TextWriter output) {
            this.output = output;
        }
        public JsonWriter(TextWriter output, int indentSize) {
            this.output = output;
            this.newlines = true;
            this.indentSize = indentSize;
        }

        public static void Write(TextWriter writer, int indentSize, object jsonData) {
            new JsonWriter(writer, indentSize).Write(jsonData);
        }
        public static void Write(TextWriter writer, object jsonData) {
            new JsonWriter(writer).Write(jsonData);
        }

        public void Write(object jsonData) {
            //if (jsonData == null) {
            //    output.Write("null");
            //} else if (jsonData is bool) {
            //    output.Write(boolToString((bool)jsonData));
            //} else if (jsonData is string) {
            //    OutputString((string)jsonData);
            //} else if (jsonData is float) {
            //    OutputFloat((float)jsonData);
            //} else if (jsonData is JsonObject) {
            //    OutputObject((JsonObject)jsonData);
            //} else if (jsonData is System.Collections.IList) {
            //    OutputArray((System.Collections.IList)jsonData);
            //}
            OutputValue(jsonData);

        }

        private void OutputArray(System.Collections.IList list) {
            if (list.Count == 0) {
                output.Write("[]");
                return;
            }
            output.Write('[');
            Indent();
            Newline();

            for (int i = 0; i < list.Count; i++) {
                var last = (i == list.Count - 1);
                var value = list[i];
                OutputValue(value);
                if (!last) {
                    output.Write(',');
                    Newline();
                }
            }

            Outdent();
            Newline();
            output.Write(']');
        }

        private void OutputObject(JsonObject jsonObject) {
            if (jsonObject.Count == 0) {
                output.Write("{}");
                return;
            }

            output.Write('{');
            Indent();
            Newline();

            for (int i = 0; i < jsonObject.Count; i++) {
                var last = (i == jsonObject.Count - 1);
                var value = jsonObject[i];
                OutputString(value.Name);
                output.Write(':');
                OutputValue(value.Value);

                if (!last) {
                    output.Write(',');
                    Newline();
                }
            }

            Outdent();
            Newline();
            output.Write('}');
        }

        private void OutputValue(object value) {
            if (value == null) {
                output.Write("null");
            } else if (value is bool) {
                output.Write(boolToString((bool)value));
            } else if (value is string) {
                OutputString((string)value);
            } else if (value is float) {
                OutputFloat((float)value);
            } else if (value is int) {
                OutputFloat((float)(int)value);
            } else if (value is byte) {
                OutputFloat((float)(byte)value);
            } else if (value is JsonObject) {
                OutputObject((JsonObject)value);
            } else if (value is System.Collections.IList) {
                OutputArray((System.Collections.IList)value);
            } else {
                throw new ArgumentException("Specified value contained a type that is not convertable to JSON.");
            }
        }

        private void Newline() {
            if (newlines) {
                output.WriteLine();
                output.Write(GetIndentString(indentLevel));
            }
        }
        private string GetIndentString(int level) {
            while (tabs.Count <= level) {
                tabs.Add(new string(' ', tabs.Count * indentSize));
            }

            return tabs[level];
        }
        void Indent() {
            indentLevel++; 
        }
        void Outdent() { 
            indentLevel--; 
        }

        private void OutputFloat(float value) {
            output.Write(value);
        }

        private void OutputString(string p) {
            output.Write('"');
            output.Write(JsonString.Escape(p));
            output.Write('"');
        }

        private static string boolToString(bool value) {
            return value ? "true" : "false";
        }
    }

    public static class JsonString
    {
        public static string Escape(string text) {
            StringBuilder sb = new StringBuilder(text.Length + 8);

            for (int i = 0; i < text.Length; i++) {
                char c = text[i];
                switch (c) {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if ('\x00' <= c && c <= '\x1f') {
                            sb.Append("\\u");
                            sb.Append(((int)c).ToString("x").PadLeft(4, '0'));
                        } else {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }
    }
}


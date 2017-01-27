using System;
using System.Collections.Generic;
using System.Text;

namespace Romulus
{
    public struct StringSection
    {
        // This field is mutable to enable an optimization that can reduce the number of string allocations (see ToString)
        private string baseString;
        private int start;
        public int Start { get { return start; } }
        private readonly int length;
        public int Length { get { return length; } }
        public String BaseString { get { return baseString; } }

        public static StringSection Empty;

        public StringSection(string str) {
            this.baseString = str;
            this.start = 0;
            this.length = str == null ? 0 : str.Length;
        }
        public StringSection(string str, int start, int len) {
            this.baseString = str;
            this.start = start;
            this.length = len;
        }

        public StringSection(StringSection str, int start, int len) {
            this.baseString = str.baseString;
            this.length = len;
            this.start = start + str.start;
        }

        public void Split(int index, out StringSection left, out StringSection right) {
            left = new StringSection(this, 0, index);
            right = new StringSection(this, index, length - index);
        }
        public StringSection Substring(int index, int length) {
            return new StringSection(this, index, length);
        }
        public StringSection Substring(int index) {
            return new StringSection(this, index, length - index);
        }
        public StringSection Left(int count) {
            return new StringSection(this, 0, count);
        }
        public StringSection Right(int count) {
            return new StringSection(this, length - count, count);
        }

        public override string ToString() {
            if (baseString == null)
                return string.Empty;

            if (start == 0 & length == baseString.Length) return baseString;

            // We will use the substring to be returned as the base string so that multiple calls
            // to Tostring will return the same string object.
            baseString = baseString.Substring(start, length);
            start = 0;

            return baseString;
        }

        public StringSection Trim() {
            if (IsNullOrEmpty) return this;

            int lastIndex = LastIndex;

            int newStart = start;
            int newLen = length;

            // While (we have chars left & the leftmost is whitespace)
            while ((newLen > 0) && (char.IsWhiteSpace(baseString[newStart]))) {
                // Remove char from left
                newStart++;
                newLen--;
            }

            while ((newLen > 0) && char.IsWhiteSpace(baseString[lastIndex])) {
                // Remove char from right
                newLen--;
                lastIndex--;
            }

            return new StringSection(baseString, newStart, newLen);
        }
        public StringSection TrimLeft() {
            if (IsNullOrEmpty) return this;

            int newStart = start;
            int newLen = length;

            // While (we have chars left & the leftmost is whitespace)
            while ((newLen > 0) && (char.IsWhiteSpace(baseString[newStart]))) {
                // Remove char from left
                newStart++;
                newLen--;
            }

            return new StringSection(baseString, newStart, newLen);
        }
        public StringSection TrimRight() {
            if (IsNullOrEmpty) return this;

            int lastIndex = LastIndex;
            int newLen = length;

            while ((newLen > 0) && char.IsWhiteSpace(baseString[lastIndex])) {
                // Remove char from right
                newLen--;
                lastIndex--;
            }

            return new StringSection(baseString, start, newLen);
        }

        public char this[int i] {
            get { return baseString[i + start]; }
        }
        public int IndexOf(char c) {
            if (baseString == null) return -1;

            var result = baseString.IndexOf(c, start);
            result -= start;
            if (result < 0 || result >= length) return -1;

            return result;
        }
        public int IndexOf(string s) {
            if (baseString == null) return -1;

            var result = baseString.IndexOf(s, start, length);
            result -= start;
            if (result < 0 || result >= length) return -1;

            return result;
        }
        private int LastIndex { get { return start + length - 1; } }

        public bool IsNullOrEmpty { get { return string.IsNullOrEmpty(baseString) | length == 0; } }

        public static implicit operator StringSection(string str) {
            return new StringSection(str);
        }

        public static int Compare(StringSection a, StringSection b, bool ignoreCase) {
            var result = string.Compare(a.baseString, a.start, b.baseString, b.start, Math.Min(a.length, b.length), ignoreCase);

            // If the two strings are different lengths, they are not equal. The longer string will be considered greater.
            if (result == 0 && a.length != b.length)
                return a.length > b.length ? 1 : -1;
            return result;
        }
        public static int Compare(StringSection a, StringSection b, StringComparison comparisonType) {
            var result = string.Compare(a.baseString, a.start, b.baseString, b.start, Math.Min(a.length, b.length), comparisonType);

            // If the two strings are different lengths, they are not equal. The longer string will be considered greater.
            if (result == 0 && a.length != b.length)
                return a.length > b.length ? 1 : -1;
            return result;
        }
        public static int Compare(StringSection a, StringSection b, bool ignoreCase, System.Globalization.CultureInfo culture, System.Globalization.CompareOptions options) {
            var result = string.Compare(a.baseString, a.start, b.baseString, b.start, Math.Min(a.length, b.length), culture, options);

            // If the two strings are different lengths, they are not equal. The longer string will be considered greater.
            if (result == 0 && a.length != b.length)
                return a.length > b.length ? 1 : -1;
            return result;
        }

        public override bool Equals(object obj) {
            if (obj is StringSection) {
                return Equals((StringSection)obj);
            }
            return base.Equals(obj);
        }
        public bool Equals(StringSection s) {
            return Compare(this, s, false) == 0;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public int IndexOfAny(char[] c) {
            if (baseString == null) return -1;

            var result = baseString.IndexOfAny(c, start, length);
            result -= start;
            if (result < 0 || result >= length) return -1;

            return result;
        }


        
    }
}

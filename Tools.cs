using System;
using System.Collections.Generic;
using System.Text;

namespace Editroid
{
    static class Tools
    {
        public static T[] Join<T>(T[] array, T value) {
            T[] newValue = new T[array.Length + 1];
            Array.Copy(array, newValue, array.Length);
            newValue[array.Length] = value;
            return newValue;
        }
        public static T[] Join<T>(T[] array, T[] secondArray) {
            T[] newValue = new T[array.Length + secondArray.Length];
            Array.Copy(array, newValue, array.Length);
            Array.Copy(secondArray, 0, newValue, array.Length, secondArray.Length);
            return newValue;
        }
        public static T[] Insert<T>(T[] array, T[] insertedArray, int index) {
            T[] newArray = new T[array.Length + insertedArray.Length];
            Array.Copy(array, newArray, index);
            Array.Copy(insertedArray, 0, newArray, index, insertedArray.Length);
            Array.Copy(array, index, newArray, index + insertedArray.Length, array.Length - index);
            return newArray;
        }
        public static T[] Insert<T>(T[] array, T value, int index) {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, index);
            newArray[index] = value;
            Array.Copy(array, index, newArray, index + 1, array.Length - index);
            return newArray;
        }
        public static T[] Subarray<T>(T[] array, int start, int count) {
            T[] newArray = new T[count];
            Array.Copy(array, start, newArray, 0, count);
            return newArray;
        }
        public static T[] Subarray<T>(T[] array, int start) {
            T[] newArray = new T[array.Length - start];
            Array.Copy(array, start, newArray, 0, newArray.Length);
            return newArray;
        }
        public static T[] ArrayString<T>(T value, int count) {
            T[] newValue = new T[count];
            for (int i = 0; i < count; i++) {
                newValue[i] = value;
            }
            return newValue;
        }

        public static string Quantify(int count, string singular, string plural) {
            return (count == 1) ? singular : plural;
        }
        public static string Quantify(int count) {
            return (count == 1) ? string.Empty : "s";
        }
    }
}

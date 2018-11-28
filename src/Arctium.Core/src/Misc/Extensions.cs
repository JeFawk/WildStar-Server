// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Numerics;
using System.Text;

namespace Arctium.Core
{
    public static class Extensions
    {
        public static uint LeftRotate(this uint value, int shiftCount) => (value << shiftCount) | (value >> (0x20 - shiftCount));

        public static T ChangeType<T>(this object value) => (T)ChangeType(value, typeof(T));

        public static object ChangeType(this object value, Type destType)
        {
            return destType.IsEnum ? Enum.ToObject(destType, value) : Convert.ChangeType(value, destType);
        }

        public static T MinValue<T>(this Type primitiveType) where T : struct, IComparable => (T)primitiveType.GetField("MinValue").GetRawConstantValue();

        public static T MaxValue<T>(this Type primitiveType) where T : struct, IComparable => (T)primitiveType.GetField("MaxValue").GetRawConstantValue();

        public static bool IsSigned(this Type t) => Convert.ToBoolean(t.GetField("MinValue").GetRawConstantValue());

        // TODO: Add long, ulong, float, double support.
        public static T[] FillRandom<T>(this T[] array) where T : struct, IComparable
        {
            var random = new Random((int)((uint)(Guid.NewGuid().GetHashCode() ^ 1 >> 89 << 2 ^ 42)).LeftRotate(13));
            var minValue = typeof(T).MinValue<long>();
            var maxValue = typeof(T).MaxValue<long>();

            if (maxValue > int.MaxValue)
                maxValue = int.MaxValue;

            for (int i = 0; i < array.Length; i++)
            {
                int randomInt;

                do
                {
                    randomInt = (int)((uint)random.Next((int)minValue, (int)maxValue)).LeftRotate(1) ^ i;
                } while (randomInt > maxValue || randomInt <= minValue);

                array[i] = randomInt.ChangeType<T>();
            }

            return array;
        }

        public static byte[] ToByteArray(this string s)
        {
            var data = new byte[s.Length / 2];

            for (var i = 0; i < s.Length; i += 2)
                data[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);

            return data;
        }

        public static string SubString(string s, char seperator) => s.Substring(s.IndexOf(seperator));

        public static bool Compare(this byte[] b, byte[] b2)
        {
            for (var i = 0; i < b2.Length; i++)
                if (b[i] != b2[i])
                    return false;

            return true;
        }

        public static bool Compare(this int[] b, int[] b2)
        {
            for (var i = 0; i < b2.Length; i++)
                if (b[i] != b2[i])
                    return false;

            return true;
        }

        public static byte[] Combine(this byte[] data, params byte[][] pData)
        {
            var combined = data;

            foreach (var arr in pData)
            {
                var currentSize = combined.Length;

                Array.Resize(ref combined, currentSize + arr.Length);

                Buffer.BlockCopy(arr, 0, combined, currentSize, arr.Length);
            }

            return combined;
        }

        public static string ToHexString(this byte[] data)
        {
            var hex = new StringBuilder();

            foreach (var b in data)
                hex.Append($"{b:X2}");

            return hex.ToString();
        }

        public static BigInteger ToBigInteger(this byte[] value, bool reverse = false)
        {
            var data = value as byte[];

            if (reverse)
                Array.Reverse(data);

            return new BigInteger(data.Combine(new byte[] { 0 }));
        }
    }
}

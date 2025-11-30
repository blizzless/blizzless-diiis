using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.Extensions
{
    public static class Extensions
    {
        public static bool HasAnyFlag<T>(this T value, T flag) where T : struct
        {
            long lValue = Convert.ToInt64(value);
            long lFlag = Convert.ToInt64(flag);
            return (lValue & lFlag) != 0;
        }

        public static string ToHexString(this byte[] byteArray)
        {
            return byteArray.Aggregate("", (current, b) => current + b.ToString("X2"));
        }

        public static byte[] ToByteArray(this string str)
        {
            str = str.Replace(" ", String.Empty);

            var res = new byte[str.Length / 2];
            for (int i = 0; i < res.Length; ++i)
            {
                string temp = String.Concat(str[i * 2], str[i * 2 + 1]);
                res[i] = Convert.ToByte(temp, 16);
            }
            return res;
        }

        public static byte[] ToByteArray(this string value, char separator)
        {
            return Array.ConvertAll(value.Split(separator), byte.Parse);
        }

        static uint LeftRotate(this uint value, int shiftCount)
        {
            return (value << shiftCount) | (value >> (0x20 - shiftCount));
        }

        public static byte[] GenerateRandomKey(this byte[] s, int length)
        {
            var random = new Random((int)((uint)(Guid.NewGuid().GetHashCode() ^ 1 >> 89 << 2 ^ 42)).LeftRotate(13));
            var key = new byte[length];

            for (int i = 0; i < length; i++)
            {
                int randValue;

                do
                {
                    randValue = (int)((uint)random.Next(0xFF)).LeftRotate(1) ^ i;
                } while (randValue > 0xFF && randValue <= 0);

                key[i] = (byte)randValue;
            }

            return key;
        }

        public static bool Compare(this byte[] b, byte[] b2)
        {
            for (int i = 0; i < b2.Length; i++)
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

        public static object[] Combine(this object[] data, params object[][] pData)
        {
            var combined = data;

            foreach (var arr in pData)
            {
                var currentSize = combined.Length;

                Array.Resize(ref combined, currentSize + arr.Length);

                Array.Copy(arr, 0, combined, currentSize, arr.Length);
            }

            return combined;
        }

        public static BigInteger ToBigInteger<T>(this T value, bool isBigEndian = false)
        {
            BigInteger ret;

            switch (typeof(T).Name)
            {
                case "Byte[]":
                    var data = value as byte[];

                    if (isBigEndian)
                        Array.Reverse(data);

                    ret = new BigInteger(data.Combine(new byte[] { 0 }));
                    break;
                case "BigInteger":
                    ret = (BigInteger)Convert.ChangeType(value, typeof(BigInteger));
                    break;
                default:
                    throw new NotSupportedException($"'{typeof(T).Name}' conversion to 'BigInteger' not supported.");
            }

            return ret;
        }

        public static void Swap<T>(ref T left, ref T right)
        {
            (left, right) = (right, left);
        }

        #region Strings
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static T ToEnum<T>(this string str) where T : struct
        {
            T value;
            if (!Enum.TryParse(str, out value))
                return default(T);

            return value;
        }

        public static string ConvertFormatSyntax(this string str)
        {
            string pattern = @"(%\W*\d*[a-zA-Z]*)";

            int count = 0;
            string result = Regex.Replace(str, pattern, m => string.Concat("{", count++, "}"));

            return result;
        }

        public static bool Like(this string toSearch, string toFind)
        {
            return toSearch.ToLower().Contains(toFind.ToLower());
        }

        public static bool IsNumber(this string str)
        {
            double value;
            return double.TryParse(str, out value);
        }

        public static int GetByteCount(this string str)
        {
            if (str.IsEmpty())
                return 0;

            return Encoding.UTF8.GetByteCount(str);
        }
        #endregion

        #region BinaryReader
        public static string ReadCString(this BinaryReader reader)
        {
            byte num;
            List<byte> temp = new List<byte>();

            while ((num = reader.ReadByte()) != 0)
                temp.Add(num);

            return Encoding.UTF8.GetString(temp.ToArray());
        }

        public static string ReadString(this BinaryReader reader, int count)
        {
            var array = reader.ReadBytes(count);
            return Encoding.ASCII.GetString(array);
        }

        public static string ReadStringFromChars(this BinaryReader reader, int count)
        {
            return new string(reader.ReadChars(count));
        }

        public static T[] ReadArray<T>(this BinaryReader reader, uint size) where T : struct
        {
            int numBytes = Unsafe.SizeOf<T>() * (int)size;

            byte[] source = reader.ReadBytes(numBytes);

            T[] result = new T[source.Length / Unsafe.SizeOf<T>()];

            if (source.Length > 0)
            {
                unsafe
                {
                    Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref result[0]), Unsafe.AsPointer(ref source[0]), (uint)source.Length);
                }
            }

            return result;
        }

        public static T Read<T>(this BinaryReader reader) where T : struct
        {
            byte[] result = reader.ReadBytes(Unsafe.SizeOf<T>());

            return Unsafe.ReadUnaligned<T>(ref result[0]);
        }
        #endregion
    }
}

//Blizzless Project 2022
//Blizzless Project 2022 
using System;
//Blizzless Project 2022 
using System.Collections.Generic;
//Blizzless Project 2022 
using System.Linq;
//Blizzless Project 2022 
using System.Text;
//Blizzless Project 2022 
using System.Threading.Tasks;

namespace DiIiS_NA.Core.Extensions
{
    //Blizzless Project 2022 
using System;
    //Blizzless Project 2022 
using System.Globalization;
    //Blizzless Project 2022 
using System.Text;

    public static class Utilities
    {
        public static string BinToHex(byte[] bin)
        {
            string str = "";
            for (int i = 0; i < bin.Length; i++)
            {
                str = str + bin[i].ToString("X2");
            }
            return str;
        }

        public static byte[] HexToBin(string hexString)
        {
            hexString = hexString.ToLower().Replace(" ", "");
            int num = hexString.Length / 2;
            byte[] buffer = new byte[num];
            for (int i = 0; i < num; i++)
            {
                buffer[i] = (byte)int.Parse(hexString.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return buffer;
        }   

        public static void MemSet(ref byte[] buffer, byte byteToSet, int size)
        {
            for (int i = 0; i < size; i++)
            { 
                buffer[i] = byteToSet;
            }
        }

        public static void SafeMemCpy(ref byte[] dest, int startPos, byte[] src, int len)
        {
            for (int i = startPos; i < (len + startPos); i++)
            {
                dest[i] = src[i - startPos];
            }
        }

        public static byte[] StrToByteArray(string str)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        public static string ToHexDump(byte[] byteArray)
        {
            string str = string.Empty;
            for (int i = 0; i < byteArray.Length; i++)
            {
                if ((i > 0) && ((i % 0x10) == 0))
                {
                    str = str + Environment.NewLine;
                }
                str = str + byteArray[i].ToString("X2") + " ";
            }
            return (str + Environment.NewLine);
        }
    }
}

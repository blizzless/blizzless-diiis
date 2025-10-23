using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiIiS_NA.REST.IO.Zlib
{
    public static partial class ZLib
    {
        public static byte[] Compress(byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteUInt8(0x78);
            buffer.WriteUInt8(0x9c);

            uint adler32 = ZLib.adler32(1, data, (uint)data.Length);// Adler32(1, data, (uint)data.Length);
            var ms = new MemoryStream();
            using (var deflateStream = new DeflateStream(ms, CompressionMode.Compress))
            {
                deflateStream.Write(data, 0, data.Length);
                deflateStream.Flush();
            }
            buffer.WriteBytes(ms.ToArray());
            buffer.WriteBytes(BitConverter.GetBytes(adler32).Reverse().ToArray());

            return buffer.GetData();
        }

        public static byte[] Decompress(byte[] data, uint unpackedSize)
        {
            byte[] decompressData = new byte[unpackedSize];
            using (var deflateStream = new DeflateStream(new MemoryStream(data, 2, data.Length - 6), CompressionMode.Decompress))
            {
                var decompressed = new MemoryStream();
                deflateStream.CopyTo(decompressed);

                decompressed.Seek(0, SeekOrigin.Begin);
                for (int i = 0; i < unpackedSize; i++)
                    decompressData[i] = (byte)decompressed.ReadByte();
            }

            return decompressData;
        }
    }
}

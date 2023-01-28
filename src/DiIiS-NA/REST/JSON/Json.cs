using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiIiS_NA.Core.Logging;
using DiIiS_NA.REST.Extensions;

namespace DiIiS_NA.REST.JSON
{
    public class Json
    {
        private static readonly Logger _logger = LogManager.CreateLogger(nameof(Json));
        public static string CreateString<T>(T dataObject)
        {
            return Encoding.UTF8.GetString(CreateArray(dataObject));
        }

        public static byte[] CreateArray<T>(T dataObject)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream();

            serializer.WriteObject(stream, dataObject);

            return stream.ToArray();
        }

        public static T CreateObject<T>(Stream jsonData)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(jsonData);
            }
            catch (Exception ex)
            {
                _logger.FatalException(ex, "Could not deserialize JSON data");
                return default(T);
            }
        }

        public static T CreateObject<T>(string jsonData, bool split = false)
        {
            return CreateObject<T>(Encoding.UTF8.GetBytes(split ? jsonData.Split(new[] { ':' }, 2)[1] : jsonData));
        }

        public static T CreateObject<T>(byte[] jsonData) => CreateObject<T>(new MemoryStream(jsonData));

        public static object CreateObject(Stream jsonData, Type type)
        {
            var serializer = new DataContractJsonSerializer(type);

            return serializer.ReadObject(jsonData);
        }

        public static object CreateObject(string jsonData, Type type, bool split = false)
        {
            return CreateObject(Encoding.UTF8.GetBytes(split ? jsonData.Split(new[] { ':' }, 2)[1] : jsonData), type);
        }

        public static object CreateObject(byte[] jsonData, Type type) => CreateObject(new MemoryStream(jsonData), type);

        // Used for protobuf json strings.
        public static byte[] Deflate<T>(string name, T data)
        {
            var jsonData = Encoding.UTF8.GetBytes(name + ":" + CreateString(data) + "\0");
            var compressedData = REST.IO.Zlib.ZLib.Compress(jsonData);

            return BitConverter.GetBytes(jsonData.Length).Combine(compressedData);
        }
    }
}

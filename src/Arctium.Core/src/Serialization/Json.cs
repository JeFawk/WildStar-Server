// Copyright (c) Arctium.

using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Arctium.Core.Compression;
using Arctium.Core.Cryptography;

namespace Arctium.Core.Serialization
{
    public class Json
    {
        public static byte[] CreateArray<T>(T dataObject)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var stream = new MemoryStream();

            serializer.WriteObject(stream, dataObject);

            return stream.ToArray();
        }

        public static string CreateString<T>(T dataObject) => Encoding.UTF8.GetString(CreateArray(dataObject));

        public static T CreateObject<T>(Stream jsonData) => (T)new DataContractJsonSerializer(typeof(T)).ReadObject(jsonData);

        public static T CreateObject<T>(string jsonData, bool split = false) => CreateObject<T>(Encoding.UTF8.GetBytes(split ? jsonData.Substring(':') : jsonData));

        public static T CreateObject<T>(byte[] jsonData) => CreateObject<T>(new MemoryStream(jsonData));

        public static object CreateObject(Stream jsonData, Type type) => new DataContractJsonSerializer(type).ReadObject(jsonData);

        public static object CreateObject(string jsonData, Type type, bool split = false) => CreateObject(Encoding.UTF8.GetBytes(split ? jsonData.Substring(':') : jsonData), type);

        public static object CreateObject(byte[] jsonData, Type type) => CreateObject(new MemoryStream(jsonData), type);

        // Used for protobuf json strings.
        public static byte[] Compress<T>(T data, string prefix)
        {
            var jsonData = Encoding.UTF8.GetBytes($"{prefix}:{CreateString(data)}\0");
            var jsonDataLength = BitConverter.GetBytes(jsonData.Length);
            var uncompressedAdler = BitConverter.GetBytes(Adler32.Calculate(jsonData)).Reverse().ToArray();

            return jsonDataLength.Combine(ZLib.Compress(jsonData), uncompressedAdler);
        }
    }
}

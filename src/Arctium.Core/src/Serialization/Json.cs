// Copyright (c) Arctium.

using System;
using System.Linq;
using System.Text;
using Arctium.Core.Compression;
using Arctium.Core.Cryptography;
using Newtonsoft.Json;

namespace Arctium.Core.Serialization
{
    public class Json
    {
        public static string Serialize(object obj) => JsonConvert.SerializeObject(obj);
        public static object Deserialize(string json) => JsonConvert.DeserializeObject(json);
        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);

        // Used for protobuf json strings.
        public static byte[] Compress<T>(T data, string prefix)
        {
            var jsonData = Encoding.UTF8.GetBytes($"{prefix}:{Serialize(data)}\0");
            var jsonDataLength = BitConverter.GetBytes(jsonData.Length);
            var uncompressedAdler = BitConverter.GetBytes(Adler32.Calculate(jsonData)).Reverse().ToArray();

            return jsonDataLength.Combine(ZLib.Compress(jsonData), uncompressedAdler);
        }
    }
}

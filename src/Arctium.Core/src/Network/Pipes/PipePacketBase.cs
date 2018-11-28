// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace Arctium.Core.Network.Pipes
{
    public abstract class PipePacketBase
    {
        public byte Message { get; set; }
        public byte[] Data { get; set; }

        protected BinaryReader readStream;
        protected BinaryWriter writeStream;

        protected PipePacketBase(GlobalPipeMessage globalPipeMessage)
        {
            writeStream = new BinaryWriter(new MemoryStream());

            Message = (byte)globalPipeMessage;

            writeStream.Write((byte)globalPipeMessage);
        }

        protected PipePacketBase(byte msg, Stream data)
        {
            readStream = new BinaryReader(data);

            Message = msg;
        }

        public byte[] ReadBytes(int count) => readStream.ReadBytes(count);

        public string ReadString() => readStream.ReadString();

        public void Write(byte[] value, int count = 0)
        {
            writeStream.Write(value, 0, count == 0 ? value.Length : count);
        }

        public void WriteString(string value) => writeStream.Write(value);

        public void Finish()
        {
            Write();

            Data = (writeStream.BaseStream as MemoryStream)?.ToArray();
        }

        public abstract void Write();

    }
}

// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace Arctium.Core.Network.Pipes.Messages
{
    public class ProcessStateInfo : PipePacketBase
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public PipeProcessState State { get; set; }

        public ProcessStateInfo() : base(GlobalPipeMessage.ProcessStateInfo)
        {
        }

        public ProcessStateInfo(byte msg, Stream data) : base(msg, data)
        {
            Id = readStream.ReadInt32();
            Alias = readStream.ReadString();
            State = (PipeProcessState)readStream.ReadByte();
        }

        public override void Write()
        {
            writeStream.Write(Id);
            writeStream.Write(Alias);
            writeStream.Write((byte)State);
        }
    }
}

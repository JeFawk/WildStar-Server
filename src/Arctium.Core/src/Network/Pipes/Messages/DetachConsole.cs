// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace Arctium.Core.Network.Pipes.Messages
{
    public class DetachConsole : PipePacketBase
    {
        public string Alias { get; set; }

        public DetachConsole() : base(GlobalPipeMessage.DetachConsole)
        {
        }

        public DetachConsole(byte ipcMessage, Stream ipcMessageData) : base(ipcMessage, ipcMessageData)
        {
            Alias = readStream.ReadString();
        }

        public override void Write()
        {
            writeStream.Write(Alias);
        }
    }
}

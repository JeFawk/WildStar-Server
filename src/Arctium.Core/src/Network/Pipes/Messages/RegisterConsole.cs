// Copyright (c) Arctium.

using System.IO;

namespace Arctium.Core.Network.Pipes.Messages
{
    public class RegisterConsole : PipePacketBase
    {
        public string Alias { get; set; }

        public RegisterConsole() : base(GlobalPipeMessage.RegisterConsole)
        {
        }

        public RegisterConsole(byte ipcMessage, Stream ipcMessageData) : base(ipcMessage, ipcMessageData)
        {
            Alias = readStream.ReadString();
        }

        public override void Write()
        {
            writeStream.Write(Alias);
        }
    }
}

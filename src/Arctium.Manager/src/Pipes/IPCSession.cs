// Copyright (c) Arctium.

using System;
using System.IO;
using System.Threading.Tasks;
using Arctium.Core.Network.Pipes;

namespace Arctium.Manager.Pipes
{
    class IPCSession : PipeSessionBase
    {
        public override Task ProcessPacket(byte ipcMessage, Stream ipcDataStream)
        {
            return IPCPacketManager.CallHandler(ipcMessage, ipcDataStream, this);
        }

        public override void OnDisconnect(int sessionId)
        {
            throw new NotImplementedException();
        }
    }
}

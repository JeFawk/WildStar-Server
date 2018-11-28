// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using Arctium.Core.Logging;
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
            Log.NewLine();
            Log.Message(LogTypes.Warning, "IPCSession.OnDisconnect not implemented");
        }
    }
}

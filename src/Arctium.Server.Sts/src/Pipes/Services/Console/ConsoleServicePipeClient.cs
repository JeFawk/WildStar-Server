// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using Arctium.Core.Network.Pipes;

namespace Arctium.Server.Sts.Pipes.Services.Console
{
    class ConsoleServicePipeClient : PipeClientBase
    {
        public ConsoleServicePipeClient(string pipeServerName, string pipeName) : base(pipeServerName, pipeName)
        {
        }

        public override Task ProcessPacket(byte ipcMessage, Stream ipcDataStream)
        {
            return IPCPacketManager.CallHandler(ipcMessage, ipcDataStream, this);
        }
    }
}

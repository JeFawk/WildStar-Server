// Copyright (c) Arctium.

using Arctium.Core.Network.Pipes;

namespace Arctium.Manager.Pipes
{
    internal class IPCServer : PipeServerBase<IPCSession>
    {
        public IPCServer(string pipeName) : base(pipeName)
        {
        }
    }
}

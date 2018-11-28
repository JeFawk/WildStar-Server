// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

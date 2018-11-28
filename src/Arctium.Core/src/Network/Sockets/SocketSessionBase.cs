// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Sockets;

namespace Arctium.Core.Network.Sockets
{
    public abstract class SocketSessionBase
    {
        // Set on session creation.
        public SocketAsyncEventArgs[] Sockets { get; set; }
        public Action<SocketSessionBase> Disconnect { get; set; }

        // Used to identify the current session.
        public Guid Guid { get; set;  }

        // AddressFamily, IPAddress and Port.
        public IPEndPoint AddressInfo => Sockets?[0].AcceptSocket.RemoteEndPoint as IPEndPoint;

        public abstract void Accept();
        public abstract void Process(object sender, SocketAsyncEventArgs e);
    }
}

// Copyright (c) Arctium.

using System;
using Arctium.Core.Network.Pipes;

namespace Arctium.Manager.Pipes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    partial class IPCMessageAttribute : Attribute
    {
        public byte Message { get; set; }

        public IPCMessageAttribute(GlobalPipeMessage message)
        {
            Message = (byte)message;
        }

        public IPCMessageAttribute(IPCMessage message)
        {
            Message = (byte)message;
        }
    }
}

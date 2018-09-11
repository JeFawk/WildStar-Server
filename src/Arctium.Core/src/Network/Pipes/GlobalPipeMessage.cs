// Copyright (c) Arctium.

namespace Arctium.Core.Network.Pipes
{
    public enum GlobalPipeMessage : byte
    {
        RegisterConsole  = 0x00,
        DetachConsole    = 0x01,
        StopConsole      = 0x02,
        ProcessStateInfo = 0x03
    }
}

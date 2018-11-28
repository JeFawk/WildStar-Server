// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

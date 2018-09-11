// Copyright (c) Arctium.

using System;

namespace Arctium.Core.Logging
{
    [Flags]
    public enum LogTypes : byte
    {
        None     = 0x00,
        Info     = 0x01,
        Debug    = 0x02,
        Trace    = 0x04,
        Warning  = 0x08,
        Error    = 0x10,
        Panic    = 0x20,

        All = byte.MaxValue
    }
}

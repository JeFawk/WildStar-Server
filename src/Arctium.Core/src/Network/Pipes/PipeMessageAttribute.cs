// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Arctium.Core.Network.Pipes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public partial class PipeMessageAttribute : Attribute
    {
        public byte Message { get; set; }

        public PipeMessageAttribute(GlobalPipeMessage message)
        {
            Message = (byte)message;
        }
    }
}

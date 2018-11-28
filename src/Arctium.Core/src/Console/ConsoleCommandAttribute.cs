// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Arctium.Core.Console
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string Command { get; }
        public int Arguments { get; }
        public string Description { get; }

        public ConsoleCommandAttribute(string command, int argCount, string description = "")
        {
            // Convert all command to lower case.
            Command = command.ToLower();
            Arguments = argCount;
            Description = description;
        }
    }
}

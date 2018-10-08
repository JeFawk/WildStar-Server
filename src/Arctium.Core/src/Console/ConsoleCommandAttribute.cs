// Copyright (c) Arctium.

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

// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Arctium.Core.Console
{
    public class CommandArgs
    {
        readonly string[] args;
        int index;

        public CommandArgs(string[] commandArgs)
        {
            args = commandArgs;
            index = 0;
        }

        public T Read<T>() => args[index++].ChangeType<T>();
    }
}

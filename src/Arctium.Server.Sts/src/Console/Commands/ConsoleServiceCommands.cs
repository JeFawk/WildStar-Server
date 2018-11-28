// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Arctium.Core.Console;
using Arctium.Core.Network.Pipes.Messages;

namespace Arctium.Server.Sts.Console.Commands
{
    public class ConsoleServiceCommands
    {
        [ConsoleCommand("detach", 0, "Detach...")]
        public static async void Detach(CommandArgs args)
        {
            await StsServer.ConsoleService.Send(new DetachConsole { Alias = StsServer.Alias.Value });
        }
    }
}

// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Arctium.Core.Console;
using Arctium.Core.Logging;
using Arctium.Manager.Pipes.Services.Console;

namespace Arctium.Manager.Commands
{
    class ConsoleServiceCommands
    {
        [ConsoleCommand("attach", 1, "Attach...")]
        public static void Attach(CommandArgs args)
        {
            var alias = args.Read<string>();

            ConsoleServiceManager.Attach(alias);
        }

        [ConsoleCommand("start", 2, "Start...")]
        public static void Start(CommandArgs args)
        {
            var server = args.Read<string>();
            var alias = args.Read<string>();

            ConsoleServiceManager.Start(server, alias, $"--alias {alias}");
        }

        [ConsoleCommand("stop", 1, "Stop...")]
        public static void Stop(CommandArgs args)
        {
            var alias = args.Read<string>();

            ConsoleServiceManager.Stop(alias);
        }

        [ConsoleCommand("servers", 0, "Show servers...")]
        public static void Show(CommandArgs args)
        {
            Log.Message(LogTypes.Info, $"Running servers ({ConsoleServiceManager.Childs.Count}):");

            foreach (var child in ConsoleServiceManager.Childs)
                Log.Message(LogTypes.Info, $"Alias: {child.Key}, Server: {child.Value.Item2.ProcessName}.");
        }
    }
}

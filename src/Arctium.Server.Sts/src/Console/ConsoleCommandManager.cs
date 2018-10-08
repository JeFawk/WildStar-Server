// Copyright (c) Arctium.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Arctium.Core.Console;
using Arctium.Core.Logging;

namespace Arctium.Server.Sts.Console
{
    class ConsoleCommandManager
    {
        static readonly Dictionary<string, HandleCommand> commands = new Dictionary<string, HandleCommand>();
        delegate void HandleCommand(CommandArgs args);

        public static void InitializeCommands()
        {
            var currentAsm = Assembly.GetEntryAssembly();

            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (var commandAttr in methodInfo.GetCustomAttributes<ConsoleCommandAttribute>())
                        if (commandAttr != null)
                            commands[commandAttr.Command] = (HandleCommand)methodInfo.CreateDelegate(typeof(HandleCommand), null);
                }
            }
        }

        public static void StartCommandHandler()
        {
            while (true)
            {
                Thread.Sleep(1);

                var line = System.Console.ReadLine()?.Split(new[] { " " }, StringSplitOptions.None);

                if (line?.Length > 0)
                {
                    var cmd = line[0].ToLower();
                    var args = line.Skip(1).ToArray();

                    if (commands.TryGetValue(cmd, out var command))
                    {
                        var argCount = command.GetMethodInfo().GetCustomAttribute<ConsoleCommandAttribute>().Arguments;

                        if (args.Length == argCount)
                            command.Invoke(new CommandArgs(args));
                        else
                            Log.Message(LogTypes.Warning, $"Wrong argument count for '{cmd}' command.");
                    }
                    else
                        Log.Message(LogTypes.Warning, $"'{cmd}' command doesn't exists.");
                }
            }
        }
    }
}

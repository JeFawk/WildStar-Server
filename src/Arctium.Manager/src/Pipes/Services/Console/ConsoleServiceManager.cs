// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Arctium.Core.Logging;
using Arctium.Core.Network.Pipes;
using Arctium.Core.Network.Pipes.Messages;
using Arctium.Manager.Commands;

namespace Arctium.Manager.Pipes.Services.Console
{
    class ConsoleServiceManager
    {
        public static bool Attached { get; private set; }
        public static Process SelectedChild { get; private set; }
        public static Dictionary<string, Tuple<string, Process>> Childs { get; }

        static readonly Dictionary<string, string> servers;
        static readonly Dictionary<string, IPCSession> consolePipeClients;

        static ConsoleServiceManager()
        {
            Childs = new Dictionary<string, Tuple<string, Process>>();

            var baseDir = Directory.GetCurrentDirectory();
            var binExtension = (Environment.OSVersion.Platform == PlatformID.Win32NT ? ".exe" : "");

            servers = new Dictionary<string, string>
            {
                { "stsserver", $"{baseDir}/bin/arctium.sts{binExtension}" },
            };

            consolePipeClients = new Dictionary<string, IPCSession>();
        }

        public static void AddConsoleClient(string alias, IPCSession session) => consolePipeClients.Add(alias, session);

        public static void RemoveConsoleClient(string alias) => consolePipeClients.Remove(alias);

        public static void Attach(string alias)
        {
            Attached = Childs.TryGetValue(alias, out var child);

            if (Attached && child != null)
            {
                // Clear the console.
                Log.Clear();

                ConsoleCommandManager.CommandPrefix = $"{alias}@{servers.Single(s => s.Value == child.Item1).Key}:$ ";

                System.Console.Title = $"Current console: {child.Item1} ({alias})";

                SelectedChild = child.Item2;
                SelectedChild.BeginOutputReadLine();
            }
            else
                Log.Message(LogTypes.Warning, $"Cannot attach: Server '{alias}' doesn't exists.");
        }

        public static void Detach(string alias)
        {
            if (Childs.TryGetValue(alias, out var child))
            {
                // Clear the console.
                Log.Clear();

                child?.Item2.CancelOutputRead();

                SelectedChild = null;
                Attached = false;

                ConsoleCommandManager.CommandPrefix = "@ServerManager:$ ";

                // Show current console int title.
                System.Console.Title = "Current console: ServerManager";
            }
        }

        public static void Start(string server, string alias, string args)
        {
            if (Childs.ContainsKey(alias))
            {
                Log.Message(LogTypes.Error, $"Server with name '{alias}' already exists.");
                return;
            }

            if (!servers.ContainsKey(server))
            {
                Log.Message(LogTypes.Warning, $"Server '{server}' doesn't exists.");
                return;
            }

            if (!File.Exists(servers[server]))
            {
                Log.Message(LogTypes.Error, $"Server file '{servers[server]}' doesn't exists.");
                return;
            }

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    FileName = servers[server],
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };

            process.OutputDataReceived += (sender, obj) =>
            {
                var splitIndex = obj.Data?.IndexOf("|") ?? -1;

                if (splitIndex != -1)
                {
                    var splitIndex2 = obj.Data.IndexOf('|', splitIndex + 1) - splitIndex - 1;

                    if (splitIndex2 != -1 && Enum.TryParse(obj.Data.Substring(splitIndex + 1, splitIndex2), out LogTypes logTypes))
                    {
                        var logMessage = obj.Data.Split(new[] { "|" }, StringSplitOptions.None);

                        if (logTypes != LogTypes.None)
                        {
                            System.Console.Write($"{logMessage[0]}|");

                            System.Console.ForegroundColor = Logger.LogTypeInfo[logTypes].Item1;
                            System.Console.Write(Logger.LogTypeInfo[logTypes].Item2);
                            System.Console.ForegroundColor = ConsoleColor.White;

                            System.Console.WriteLine($"|{logMessage[2]}");
                        }
                        else
                            System.Console.WriteLine(logMessage[2]);
                    }
                }
            };

            process.Exited += (s, o) =>
            {
                // Detach before removing the child.
                if (Attached)
                    Detach(alias);

                if (Childs.Remove(alias))
                {
                    Log.Message(LogTypes.Warning, $"Server '{alias}' exited without shutdown command!!!");
                    Log.Message(LogTypes.None, ConsoleCommandManager.CommandPrefix, false);
                }
            };

            process.Start();

            Childs.Add(alias, Tuple.Create(servers[server], process));

            Log.Message(LogTypes.Info, $"Started '{process.ProcessName}' with alias '{alias}'.");
        }

        public static async void Stop(string alias)
        {
            if (Childs.TryGetValue(alias, out var process))
            {
                Log.Message(LogTypes.Info, $"Shutting down '{process.Item2.ProcessName}' ({alias})...");

                Childs.Remove(alias);

                // Send exit state to the server.
                consolePipeClients[alias].Send(new ProcessStateInfo
                {
                    Id = process.Item2.Id,
                    Alias = alias,
                    State = PipeProcessState.Stop
                }).GetAwaiter().GetResult();

                // Wait for the server to exit.
                var waitForExitTask = Task.Run(() => process.Item2.WaitForExit(int.MaxValue));
                var whenAllTask = Task.WhenAll(waitForExitTask);

                if (await Task.WhenAny(whenAllTask) == whenAllTask && waitForExitTask.Result)
                {
                    RemoveConsoleClient(alias);

                    Log.Message(LogTypes.Info, $"'{process.Item2.ProcessName}' ({alias}) shutdown finished.");
                }
            }
            else
                Log.Message(LogTypes.Warning, $"Server with '{alias}' doesn't exists.");
        }
    }
}

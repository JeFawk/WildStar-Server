// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine;
using System.Threading;
using Arctium.Core.Logging;
using Arctium.Core.Network.Pipes.Messages;
using Arctium.Server.Sts.Misc;
using Arctium.Server.Sts.Pipes;
using Arctium.Server.Sts.Pipes.Services.Console;
using Lappa.ORM;
using Lappa.ORM.Constants;
using Newtonsoft.Json;

namespace Arctium.Server.Sts
{
    class StsServer
    {
        public static Argument<string> Alias { get; private set; }
        public static ConsoleServicePipeClient ConsoleService { get; private set; }

        const string serverName = nameof(StsServer);

        static void Main(string[] args)
        {
            ProcessCommandLine(args);

            // Start console logging.
            Log.Start(StsConfig.LogLevel, new LogFile(StsConfig.LogDirectory, StsConfig.LogConsoleFile));

            using (ConsoleService = new ConsoleServicePipeClient(StsConfig.ServiceConsoleServer, StsConfig.ServiceConsolServerPipe))
            {
                IPCPacketManager.DefineMessageHandler();

                // Register console to ServerManager and start listening for incoming ipc packets.
                ConsoleService.Send(new RegisterConsole { Alias = Alias.Value }).GetAwaiter().GetResult();
                ConsoleService.Process();

                Database.Auth.Initialize(new ConnectorSettings
                {
                    ApiHost = $"http://{StsConfig.ApiHost}:{StsConfig.ApiPort}/api/Auth",
                    ConnectionMode = ConnectionMode.Api,
                    DatabaseType = DatabaseType.MySql,

                    // Assign JSON serialize/deserialize functions (Newtonsoft.Json).
                    ApiSerializeFunction = JsonConvert.SerializeObject,
                    ApiDeserializeFunction = JsonConvert.DeserializeObject<object[][]>
                });

                while (true)
                {
                    Thread.Sleep(1);
                }
            }
        }

        static void ProcessCommandLine(string[] args)
        {
            string configFile = default;

            // Parse command line args first.
            var argSyntax = ArgumentSyntax.Parse(args, syntax =>
            {
                Alias = syntax.DefineOption("a|alias", "", true);

                configFile = syntax.DefineOption("c|config", @"configs/StsServer.conf").Value;
            });

            // Command line arg checks.
            if (string.IsNullOrEmpty(Alias.Value))
            {
                System.Console.WriteLine(argSyntax.GetHelpText());

                argSyntax.ReportError($"{Alias} is required.");
            }

            // Read config file.
            StsConfig.Initialize(configFile);
        }

        public static void Shutdown()
        {
            // TODO: Implement save shutdown.
            Log.Message(LogTypes.Info, "Shutting down in 5 seconds...");

            Thread.Sleep(5000);
            Environment.Exit(0);
        }
    }
}

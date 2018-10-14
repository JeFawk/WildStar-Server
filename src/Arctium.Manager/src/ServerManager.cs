// Copyright (c) Arctium.

using System.CommandLine;
using Arctium.Core.Logging;
using Arctium.Core.Misc;
using Arctium.Manager.Commands;
using Arctium.Manager.Misc;
using Arctium.Manager.Pipes;

namespace Arctium.Manager
{
    class ServerManager
    {
        const string serverName = nameof(ServerManager);

        static void Main(string[] args)
        {
            ProcessCommandLine(args);

            // Start console logging.
            Log.Start(ManagerConfig.LogLevel, new LogFile(ManagerConfig.LogDirectory, ManagerConfig.LogConsoleFile));

            Helper.PrintHeader(serverName);

            using (var consolePipeServer = new IPCServer(ManagerConfig.ServiceConsoleName))
            {
                consolePipeServer.Listen();

                // Register pipe message handlers.
                IPCPacketManager.DefineMessageHandler();

                // Register console commands.
                ConsoleCommandManager.InitializeCommands();

                Log.Message(LogTypes.Info, $"{serverName} successfully started.");

                // Listen for console commands.
                ConsoleCommandManager.StartCommandHandler();
            }
        }

        static void ProcessCommandLine(string[] args)
        {
            string configFile = default;

            // Parse command line args first.
            var argSyntax = ArgumentSyntax.Parse(args, syntax =>
            {
                configFile = syntax.DefineOption("c|config", "../configs/Manager.conf").Value;
            });

            // Read config file..
            ManagerConfig.Initialize(configFile);

        }
    }
}

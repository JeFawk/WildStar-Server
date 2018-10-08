// Copyright (c) Arctium.

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
            // Initialize the Manager server configuration file.
            ManagerConfig.Initialize("../configs/Manager.conf");

            Log.Start(ManagerConfig.LogLevel, null);

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
    }
}

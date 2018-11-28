// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Arctium.Core.Logging;
using Arctium.Core.Network.Pipes;
using Arctium.Core.Network.Pipes.Messages;

namespace Arctium.Manager.Pipes.Services.Console
{
    class ConsoleServiceHandler
    {
        [IPCMessage(GlobalPipeMessage.RegisterConsole)]
        public static void HandleRegisterConsole(RegisterConsole registerConsole, IPCSession session)
        {
            ConsoleServiceManager.AddConsoleClient(registerConsole.Alias, session);
        }

        [IPCMessage(GlobalPipeMessage.DetachConsole)]
        public static void HandleDetachConsole(DetachConsole detachConsole, IPCSession session)
        {
            ConsoleServiceManager.Detach(detachConsole.Alias);
        }

        [IPCMessage(GlobalPipeMessage.ProcessStateInfo)]
        public static void HandleProcessStateInfo(ProcessStateInfo processStateInfo, IPCSession session)
        {
            switch (processStateInfo.State)
            {
                // TODO: Implement on childs. Called in ConsoleManager.Stop for now.
                case PipeProcessState.Stopped:
                    ConsoleServiceManager.RemoveConsoleClient(processStateInfo.Alias);
                    break;
                default:
                    Log.Message(LogTypes.Warning, $"Received unhandled process state '{processStateInfo.State}' from '{processStateInfo.Alias}'.");
                    break;
            }
        }
    }
}

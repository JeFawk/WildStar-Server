// Copyright (c) Arctium.

using Arctium.Core.Logging;
using Arctium.Core.Network.Pipes;
using Arctium.Core.Network.Pipes.Messages;

namespace Arctium.Server.Sts.Pipes.Services.Console
{
    public class ConsoleServiceHandler
    {
        [PipeMessage(GlobalPipeMessage.ProcessStateInfo)]
        public static void HandleProcessStateInfo(ProcessStateInfo processStateInfo, PipeClientBase client)
        {
            switch (processStateInfo.State)
            {
                case PipeProcessState.Stop:
                    StsServer.Shutdown();
                    break;
                default:
                    Log.Message(LogTypes.Warning, $"Received unhandled process state '{processStateInfo.State}' from '{processStateInfo.Alias}'.");
                    break;
            }
        }
    }
}

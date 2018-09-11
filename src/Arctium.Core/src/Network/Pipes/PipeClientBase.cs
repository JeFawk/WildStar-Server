// Copyright (c) Arctium.

using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Arctium.Core.Logging;

namespace Arctium.Core.Network.Pipes
{
    public abstract class PipeClientBase
    {
        readonly NamedPipeClientStream pipeClientStream;

        protected PipeClientBase(string pipeServer, string pipeName)
        {
            pipeClientStream = new NamedPipeClientStream(pipeServer, pipeName, PipeDirection.InOut, PipeOptions.WriteThrough | PipeOptions.Asynchronous);

            pipeClientStream.Connect();
        }

        public async void Process()
        {
            try
            {
                while (true)
                {
                    if (pipeClientStream.IsConnected)
                    {
                        var ipcMessage = new byte[1];

                        if (await pipeClientStream.ReadAsync(ipcMessage, 0, 1) > 0)
                            await ProcessPacket(ipcMessage[0], pipeClientStream);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogTypes.Error, ex.Message);
            }
        }

        public abstract Task ProcessPacket(byte ipcMessage, Stream ipcDataStream);

        public Task Send(PipePacketBase pipePacket)
        {
            pipePacket.Finish();

            return pipeClientStream.WriteAsync(pipePacket.Data, 0, pipePacket.Data.Length);
        }

        public void Dispose()
        {
            pipeClientStream.Dispose();
        }
    }
}

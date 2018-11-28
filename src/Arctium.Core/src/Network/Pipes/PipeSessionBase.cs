// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Arctium.Core.Logging;

namespace Arctium.Core.Network.Pipes
{
    public abstract class PipeSessionBase
    {
        public int SessionId { get; set; }

        NamedPipeServerStream serverStream;

        public virtual async void Process(NamedPipeServerStream pipeServerStream)
        {
            serverStream = pipeServerStream;

            try
            {
                while (serverStream.IsConnected)
                {
                    var pipeMessage = new byte[1];

                    if (await serverStream.ReadAsync(pipeMessage, 0, 1) > 0)
                        await ProcessPacket(pipeMessage[0], serverStream);
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogTypes.Error, ex.Message);
            }
            finally
            {
                OnDisconnect(SessionId);
            }
        }

        public abstract Task ProcessPacket(byte ipcMessage, Stream pipeDataStream);

        public virtual Task Send(PipePacketBase pipePacket)
        {
            pipePacket.Finish();

            return serverStream.WriteAsync(pipePacket.Data, 0, pipePacket.Data.Length);
        }

        public virtual Task Send(byte[] pipePacketData)
        {
            return serverStream.WriteAsync(pipePacketData, 0, pipePacketData.Length);
        }

        public abstract void OnDisconnect(int sessionId);
    }

}

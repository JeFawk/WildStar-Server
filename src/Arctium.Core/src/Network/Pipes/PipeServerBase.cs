// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Arctium.Core.Logging;

namespace Arctium.Core.Network.Pipes
{
    public abstract class PipeServerBase<TSession> : IDisposable where TSession : PipeSessionBase, new()
    {
        public string PipeName { get; }

        readonly NamedPipeServerStream pipeServerStream;
        bool initialized;

        public PipeServerBase(string pipeName)
        {
            PipeName = pipeName;

            pipeServerStream = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 100, PipeTransmissionMode.Byte, PipeOptions.WriteThrough | PipeOptions.Asynchronous);
        }

        public void Listen()
        {
            new Thread(AcceptAsync).Start();
        }

        async void AcceptAsync()
        {
            try
            {
                while (true)
                {
                    // Accept one new IPC connection per 100 ms.
                    await Task.Delay(100);

                    // Called on first connection only.
                    if (!initialized)
                    {
                        if (pipeServerStream == null)
                            break;

                        await pipeServerStream.WaitForConnectionAsync();

                        if (pipeServerStream.IsConnected)
                        {
                            initialized = true;

                            new TSession().Process(pipeServerStream);
                        }
                    }
                    else
                    {
                        var pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 100, PipeTransmissionMode.Byte, PipeOptions.WriteThrough | PipeOptions.Asynchronous);

                        await pipe.WaitForConnectionAsync();

                        if (pipe.IsConnected)
                            new TSession().Process(pipe);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogTypes.Error, ex.Message);
            }
        }

        public void Dispose()
        {
            pipeServerStream.Dispose();

            initialized = false;
        }
    }
}

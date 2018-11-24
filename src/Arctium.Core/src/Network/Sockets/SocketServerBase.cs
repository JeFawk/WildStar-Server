// Copyright (c) Arctium.

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Arctium.Core.Logging;

namespace Arctium.Core.Network.Sockets
{
    public class SocketServerBase<TSession> where TSession : SocketSessionBase, IDisposable, new()
    {
        public bool IsListening { get; private set; }

        public int MaxSimultanAcceptConnections { get; }
        public int MaxActiveConnections { get; }
        public int BufferSize { get; }

        public int ActiveConnections => activeConnections;

        readonly ConcurrentStack<SocketAsyncEventArgs> acceptSockets;
        readonly ConcurrentStack<SocketAsyncEventArgs> readWriteSockets;

        Socket serverSocket;

        readonly IPAddress ipAddress;
        readonly int port;

        int activeConnections;
        bool acceptConnections;

        public SocketServerBase(string address, int port, int maxSimultanAcceptConnections, int maxActiveConnections, int bufferSize)
        {

            if (!IPAddress.TryParse(address, out ipAddress))
            {
                Log.Message(LogTypes.Error, $"'{address}' is not a valid IP address");
                return;
            }

            MaxSimultanAcceptConnections = maxSimultanAcceptConnections;
            MaxActiveConnections = maxActiveConnections;
            BufferSize = bufferSize;

            acceptSockets = new ConcurrentStack<SocketAsyncEventArgs>();
            readWriteSockets = new ConcurrentStack<SocketAsyncEventArgs>();

            this.port = port;

            Create(ipAddress, port);
        }

        void Create(IPAddress ipAddress, int port)
        {
            // Create accept sockets.
            for (var i = 0; i < MaxSimultanAcceptConnections; i++)
            {
                var socketEventArgs = new SocketAsyncEventArgs();

                socketEventArgs.Completed += async (sender, args) => await ProcessAcceptAsync(args);

                acceptSockets.Push(socketEventArgs);
            }

            // Create read/write sockets.
            for (var i = 0; i < MaxActiveConnections * 2; i++)
            {
                var socketEventArgs = new SocketAsyncEventArgs();

                socketEventArgs.SetBuffer(new byte[BufferSize], 0, BufferSize);

                readWriteSockets.Push(socketEventArgs);
            }

            // Create and bind the socket to the specified IP and port.
            serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                serverSocket.DualMode = true;

            serverSocket.Bind(new IPEndPoint(ipAddress, port));
        }

        public bool Listen(int acceptDelay = 10)
        {
            try
            {
                serverSocket.Listen(MaxActiveConnections);

                new Thread(AcceptAsync) { IsBackground = true }.Start(acceptDelay);

                IsListening = true;
            }
            catch (Exception ex)
            {
                Log.Message(LogTypes.Error, ex.Message);

                IsListening = false;
            }

            return IsListening;
        }

        async void AcceptAsync(object acceptDelay)
        {
            try
            {
                acceptConnections = true;

                while (acceptConnections)
                {
                    await Task.Delay((int)acceptDelay);

                    // Check for pending connections.
                    if (serverSocket.Poll(0, SelectMode.SelectRead))
                    {
                        if (!acceptSockets.TryPop(out var acceptEventArgs))
                        {
                            acceptEventArgs = new SocketAsyncEventArgs();

                            acceptEventArgs.Completed += async (sender, args) => await ProcessAcceptAsync(args);
                        }

                        // Call ProcessAcceptAsync if the socket IO operation completed synchronously.
                        if (!serverSocket.AcceptAsync(acceptEventArgs))
                            await ProcessAcceptAsync(acceptEventArgs);
                    }
                }

                if (!disposedValue && IsListening)
                {
                    while (!acceptConnections)
                        await Task.Delay((int)acceptDelay);

                    // Restart the AcceptAsync loop after 'StartAcceptConnections' call.
                    AcceptAsync(acceptDelay);
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogTypes.Error, ex.Message);
            }
        }

        async Task ProcessAcceptAsync(SocketAsyncEventArgs acceptEventArgs)
        {
            // Check if the socket was at least connected one time.
            if (acceptEventArgs.AcceptSocket?.Connected == true)
            {
                var readWriteEventArgs = new SocketAsyncEventArgs[2];

                // Get one event arg object for reading and one for writing.
                if (readWriteSockets.TryPopRange(readWriteEventArgs) == readWriteEventArgs.Length)
                {
                    // Increment the current active connection count.
                    Interlocked.Increment(ref activeConnections);

                    // Assign the client socket to the read/write socket event args.
                    readWriteEventArgs[0].AcceptSocket = acceptEventArgs.AcceptSocket;
                    readWriteEventArgs[1].AcceptSocket = acceptEventArgs.AcceptSocket;

                    await CreateSession(readWriteEventArgs);
                }
                else
                {
                    acceptEventArgs.AcceptSocket.Dispose();

                    Log.Message(LogTypes.Info, "Maximum number of active client connections has been reached.");
                }
            }

            // Accept operation finished, reset the the accept socket and put it back into our pool.
            acceptEventArgs.AcceptSocket = null;

            acceptSockets.Push(acceptEventArgs);
        }

        protected virtual Task CreateSession(SocketAsyncEventArgs[] sockets)
        {
            return Task.Factory.StartNew(new TSession
            {
                Guid = Guid.NewGuid(),
                Disconnect = DeleteSession,
                Sockets = sockets
            }.Accept);
        }

        protected void DeleteSession(SocketSessionBase session)
        {
            // Dispose the client socket.
            session.Sockets[0].AcceptSocket?.Dispose();
            session.Sockets[1].AcceptSocket?.Dispose();

            // Put the free sockets back into the stack.
            readWriteSockets.PushRange(session.Sockets);

            // Decrement the current active connection count.
            Interlocked.Decrement(ref activeConnections);
        }

        // Restarts the accept connection loop. No effect without calling 'StopAcceptConnections' before.
        public void StartAcceptConnections()
        {
            acceptConnections = true;
        }

        public void StopAcceptConnections()
        {
            acceptConnections = false;
        }

        public void Restart()
        {
            // Do nothing here. Stop methods needs to be called before!
            if (IsListening || acceptConnections)
                return;

            // Recreate and listen with the data provided on object creation.
            Create(ipAddress, port);
            Listen();
        }

        public void Stop()
        {
            StopAcceptConnections();

            // Immediately close & dispose the server socket and disconnect all clients.
            serverSocket.Close();

            // Clear our socket pools.
            acceptSockets.Clear();
            readWriteSockets.Clear();

            // Stop listening to end the accept thread.
            IsListening = false;
        }

        #region IDisposable Support
        bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

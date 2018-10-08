// Copyright (c) Arctium.

using System;
using Arctium.Server.Sts.Pipes.Services.Console;

namespace Arctium.Server.Sts
{
    class StsServer
    {
        public static string Alias { get; private set; }
        public static ConsoleServicePipeClient ConsoleService { get; private set; }

        const string serverName = nameof(StsServer);

        static void Main(string[] args)
        {

        }

        public static void Shutdown()
        {
            // TODO: Implement save shutdown.
            Environment.Exit(0);
        }
    }
}

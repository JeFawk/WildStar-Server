// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Arctium.Core.Configuration;
using Arctium.Core.Logging;

namespace Arctium.Server.Sts.Misc
{
    public class StsConfig : ConfigBase<StsConfig>
    {
        [ConfigEntry("Log.Level", LogTypes.All)]
        public static LogTypes LogLevel { get; }

        [ConfigEntry("Log.Directory", "logs/sts")]
        public static string LogDirectory { get; }

        [ConfigEntry("Log.Console.File", "")]
        public static string LogConsoleFile { get; }

        [ConfigEntry("API.Host", "127.0.0.1")]
        public static string ApiHost { get; }

        [ConfigEntry("API.Port", 5543)]
        public static int ApiPort { get; }

        [ConfigEntry("Service.Console.Server", ".")]
        public static string ServiceConsoleServer { get; }

        [ConfigEntry("Service.Console.Server.Pipe", "")]
        public static string ServiceConsolServerPipe { get; }
    }
}


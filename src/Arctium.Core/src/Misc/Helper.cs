// Copyright (c) Arctium.

using System;
using System.Text;
using Arctium.Core.Logging;

namespace Arctium.Core.Misc
{
    public class Helper
    {
        public static void PrintHeader(string serverName)
        {
            System.Console.ForegroundColor = ConsoleColor.Cyan;

            Log.Message(LogTypes.None, "_________________WildStar________________");
            Log.Message(LogTypes.None, "                   _   _                 ");
            Log.Message(LogTypes.None, @"    /\            | | (_)                ");
            Log.Message(LogTypes.None, @"   /  \   _ __ ___| |_ _ _   _ _ __ ___  ");
            Log.Message(LogTypes.None, @"  / /\ \ | '__/ __| __| | | | | '_ ` _ \ ");
            Log.Message(LogTypes.None, @" / ____ \| | | (__| |_| | |_| | | | | | |");
            Log.Message(LogTypes.None, @"/_/    \_\_|  \___|\__|_|\__,_|_| |_| |_|");
            Log.NewLine();

            var sb = new StringBuilder();

            sb.Append("_________________________________________");

            var nameStart = (42 - serverName.Length) / 2;

            sb.Insert(nameStart, serverName);
            sb.Remove(nameStart + serverName.Length, serverName.Length);

            Log.Message(LogTypes.None, sb.ToString());

            Log.NewLine();
            Log.Message(LogTypes.Info, $"Starting WildStar {serverName}...");
        }
    }
}

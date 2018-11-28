// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

        public static byte[] GenerateRandomKey(int length)
        {
            var random = new Random((int)((uint)(Guid.NewGuid().GetHashCode() ^ 1 >> 89 << 2 ^ 42)).LeftRotate(13));
            var key = new byte[length];

            for (int i = 0; i < length; i++)
            {
                int randValue = -1;

                do
                {
                    randValue = (int)((uint)random.Next(0xFF)).LeftRotate(1) ^ i;
                } while (randValue > 0xFF && randValue <= 0);

                key[i] = (byte)randValue;
            }

            return key;
        }
    }
}

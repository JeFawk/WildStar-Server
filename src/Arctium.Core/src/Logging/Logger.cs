// Copyright (c) Arctium.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Arctium.Core.Logging
{
    public class Logger
    {
        public static readonly Dictionary<LogTypes, Tuple<ConsoleColor, string>> LogTypeInfo = new Dictionary<LogTypes, Tuple<ConsoleColor, string>>
        {
            { LogTypes.None,    Tuple.Create(ConsoleColor.White,     "") },
            { LogTypes.Info,    Tuple.Create(ConsoleColor.Green,     " Info    ") },
            { LogTypes.Debug,   Tuple.Create(ConsoleColor.DarkGreen, " Debug   ") },
            { LogTypes.Trace,   Tuple.Create(ConsoleColor.Green,     " Trace   ") },
            { LogTypes.Warning, Tuple.Create(ConsoleColor.Yellow,    " Warning ") },
            { LogTypes.Error,   Tuple.Create(ConsoleColor.Red,       " Error   ") },
            { LogTypes.Panic,   Tuple.Create(ConsoleColor.Red,       " Panic   ") },
        };

        // No volatile support for properties, let's use a private backing field.
        public LogTypes LogTypes { get => logTypes; set => logTypes = value; }

        readonly BlockingCollection<Tuple<LogTypes, string, string>> logQueue;
        volatile LogTypes logTypes;

        bool isLogging;

        public Logger()
        {
            logQueue = new BlockingCollection<Tuple<LogTypes, string, string>>();
        }

        public void Start(LogFile logFile = null)
        {
            var logThread = new Thread(async () =>
            {
                using (logQueue)
                using (logFile)
                {
                    isLogging = true;

                    while (isLogging)
                    {
                        // Do nothing if logging is turned off (LogTypes.None) & the log queue is empty, but continue the loop.
                        if (logTypes == LogTypes.None || !logQueue.TryTake(out Tuple<LogTypes, string, string> log))
                            continue;

                        // LogTypes.None is also used for empty/simple log lines (without timestamp, etc.).
                        if (log.Item1 != LogTypes.None)
                        {
                            Console.ForegroundColor = ConsoleColor.White;

                            Console.Write($"{log.Item2} |");

                            Console.ForegroundColor = LogTypeInfo[log.Item1].Item1;
                            Console.Write(LogTypeInfo[log.Item1].Item2);
                            Console.ForegroundColor = ConsoleColor.White;

                            Console.WriteLine($"| {log.Item3}");

                            await logFile?.WriteAsync($"{log.Item2} |{LogTypeInfo[log.Item1].Item2}| {log.Item3}");
                        }
                        else
                        {
                            Console.WriteLine(log.Item3);

                            await logFile?.WriteAsync(log.Item3);
                        }
                    }
                }
            });

            logThread.IsBackground = true;
            logThread.Start();

            isLogging = logThread.ThreadState == ThreadState.Running;
        }

        public void Stop() => isLogging = false;

        public void Message(LogTypes logType, string text) => SetLogger(logType, text);

        public void NewLine() => SetLogger(LogTypes.None, "");

        public void WaitForKey() => Console.ReadKey(true);

        public void Clear() => Console.Clear();

        void SetLogger(LogTypes type, string text)
        {
            if ((logTypes & type) == type)
            {
                if (type == LogTypes.None)
                    logQueue.Add(Tuple.Create(type, "", text));
                else
                    logQueue.Add(Tuple.Create(type, DateTime.Now.ToString("T"), text));
            }
        }
    }
}

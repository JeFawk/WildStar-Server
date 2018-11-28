// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Arctium.Core.Logging
{
    public sealed class LogFile : IDisposable
    {
        readonly FileStream logStream;

        public LogFile(string directory, string file)
        {
            // File name is required.
            if (!string.IsNullOrEmpty(file))
            {
                // Make sure that the specified directory exists.
                Directory.CreateDirectory(directory);

                logStream = new FileStream($"{directory}/{DateTime.Now:yyyy-MM-dd_hh-mm-ss}_{file}", FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 4096, true);
            }
        }

        public async Task WriteAsync(string logMessage)
        {
            var logBytes = Encoding.UTF8.GetBytes($"{logMessage}\n");

            await logStream.WriteAsync(logBytes, 0, logBytes.Length);
            await logStream.FlushAsync();
        }

        public void Dispose()
        {
            logStream.Close();
        }
    }
}

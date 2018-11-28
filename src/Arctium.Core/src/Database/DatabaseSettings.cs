// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Arctium.Core.Database
{
    public class DatabaseSettings
    {
        public string Host { get; }
        public int Port { get; }
        public string Database { get; }
        public string User { get; }
        public string Password { get; }

        // Enabled pooling by default.
        public bool Pooling { get; } = true;
        public int MinPoolSize { get; } = 1;
        public int MaxPoolSize { get; } = 30;

        // Use the utf8 charset by default.
        public string CharSet { get; } = "utf8";
    }
}

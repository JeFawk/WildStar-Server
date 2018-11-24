// Copyright (c) Arctium.

using Lappa.ORM.Constants;
using System;

namespace Arctium.API
{
    public static class Database
    {
        public static Lappa.ORM.Database Auth = new Lappa.ORM.Database();

        public static string CreateConnectionString(string host, string user, string password, string database, int port, int minPoolSize, int maxPoolSize, DatabaseType dbType)
        {
            if (dbType == DatabaseType.MySql)
                return $"Server={host};User Id={user};Port={port};Password={password};Database={database};Pooling=True;Min Pool Size={minPoolSize};Max Pool Size={maxPoolSize};CharSet=utf8";

            if (dbType == DatabaseType.MSSql)
                return $"Data Source={host}; Initial Catalog = {database}; User ID = {user}; Password = {password};Pooling=True;Min Pool Size={minPoolSize};Max Pool Size={maxPoolSize}";

            throw new NotSupportedException($"{dbType}");
        }
    }
}

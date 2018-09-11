// Copyright (c) Arctium.

using System;
using System.Reflection;

namespace Arctium.Core.Misc
{
    public abstract class Singleton<T> where T : class
    {
        public bool Initialized => lazy.IsValueCreated;
        public static T Instance => lazy.Value;

        static readonly Lazy<T> lazy = new Lazy<T>(() =>
        {
            // Call the private, parameterless constructor.
            return typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0].Invoke(new object[0]) as T;
        });

        public void Initialize() { }
    }
}

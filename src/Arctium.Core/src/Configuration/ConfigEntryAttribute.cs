// Copyright (c) Arctium.

using System;

namespace Arctium.Core.Configuration
{
    public class ConfigEntryAttribute : Attribute
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }

        public ConfigEntryAttribute(string name, object defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
    }
}

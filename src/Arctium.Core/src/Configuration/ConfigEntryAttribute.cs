// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

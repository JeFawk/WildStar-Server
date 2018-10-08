// Copyright (c) Arctium.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Arctium.Core.Configuration
{
    public abstract class ConfigBase<TDerived>
    {
        static Dictionary<string, object> configEntries;

        public static void Initialize(string configFile)
        {
            if (!File.Exists(configFile))
                throw new FileNotFoundException(configFile);

            var fileContent = File.ReadAllText(configFile);

            configEntries = new Dictionary<string, object>();

            // Get key,key/value pair config options.
            foreach (Match m in Regex.Matches(fileContent, @"^([^\W].*)[^\S\r\n]?=\s*\[\s?([^\]]*)\]", RegexOptions.Multiline))
            {
                var key = m.Groups[1].Value.Trim();
                var value = new Dictionary<string, string>();

                foreach (Match m2 in Regex.Matches(m.Captures[0].Value.Trim(',', '\r', '\n'), @"([^\W].*)[^\S\r\n]?:[^\S\r\n]?(.*)$", RegexOptions.Multiline))
                {
                    var subKey = m2.Groups[1].Value.Trim();
                    var subValue = m2.Groups[2].Value.Trim(',', '"', '\r', '\n');

                    if (value.ContainsKey(subKey))
                    {
                        System.Console.WriteLine($"Replacing value for '{key}.{subKey}'.");

                        value[subKey] = subValue;
                    }
                    else
                        value.Add(subKey, subValue);
                }

                if (configEntries.ContainsKey(key))
                {
                    System.Console.WriteLine($"Duplicate entry found for '{key}'.");

                    configEntries[key] = value;
                }
                else
                    configEntries.Add(key, value);
            }

            // Get single line config options.
            foreach (Match m in Regex.Matches(fileContent, @"^([^\W].*)[^\S\r\n]?=[^\S\r\n](.*)$", RegexOptions.Multiline))
            {
                var key = m.Groups[1].Value.Trim();
                var value = m.Groups[2].Value.Trim(',', '"', '\r', '\n');

                if (configEntries.ContainsKey(key))
                    configEntries[key] = value;
                else
                    configEntries.Add(key, value);
            }

            AssignValues();
        }

        static void AssignValues()
        {
            var configEntryFields = typeof(TDerived).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static).Where(f => f.GetCustomAttribute<ConfigEntryAttribute>() != null);

            foreach (var field in configEntryFields)
            {
                object fieldValue = null;

                var configEntryAttribute = field.GetCustomAttribute<ConfigEntryAttribute>();

                if (!configEntries.TryGetValue(configEntryAttribute.Name, out var value))
                {
                    System.Console.WriteLine($"Can't find config entry '{configEntryAttribute.Name}'.");

                    fieldValue = configEntryAttribute.DefaultValue;
                }

                // Assign config options if found.
                if (fieldValue == null)
                    AssignValue(field, null, value);
            }
        }

        static void AssignValue(PropertyInfo field, object obj, object value)
        {
            object fieldValue;

            // Primitive types & numeric/string enum options.
            if (field.PropertyType.IsPrimitive || field.PropertyType.IsEnum)
            {
                // Convert the config value to a string.
                var stringValue = value.ToString();

                // Check for hex numbers (starting with 0x).
                var numberBase = stringValue.StartsWith("0x") ? 16 : 10;

                // Parse bool option by string.
                if (field.PropertyType == typeof(bool))
                    fieldValue = stringValue != "0";
                // Parse enum options by string.
                else if (field.PropertyType.IsEnum && numberBase == 10)
                    fieldValue = Enum.Parse(field.PropertyType, stringValue);
                else
                {
                    // Get the true type.
                    var valueType = field.PropertyType.IsEnum ? field.PropertyType.GetEnumUnderlyingType() : field.PropertyType;

                    // Check if it's a signed or unsigned type and convert it to the correct type.
                    if (valueType.IsSigned())
                        fieldValue = Convert.ToInt64(stringValue, numberBase).ChangeType(valueType);
                    else
                        fieldValue = Convert.ToUInt64(stringValue, numberBase).ChangeType(valueType);
                }
            }
            else if (field.PropertyType != typeof(string) && field.PropertyType.IsClass)
            {
                fieldValue = Activator.CreateInstance(field.PropertyType);

                var fieldValues = (Dictionary<string, string>)value;

                // Get class fields.
                foreach (var f in field.PropertyType.GetProperties())
                {
                    if (fieldValues.TryGetValue(f.Name, out var objFieldValue))
                        AssignValue(f, fieldValue, objFieldValue);
                }
            }
            else
            {
                // String values.
                fieldValue = value;
            }

            // Use the properties backing field to set the value.
            var backingFieldName = $"<{field.Name}>k__BackingField";
            var backingField = field.DeclaringType.GetTypeInfo().DeclaredFields.SingleOrDefault(f => f.Name == backingFieldName);

            backingField?.SetValue(obj, fieldValue);
        }
    }
}

using Tomlyn;
using Tomlyn.Model;
using System.Collections.Generic;
using System.IO;

namespace AutoBackupConfigLauncher
{
    public class ConfigReader
    {
        public Dictionary<string, object> ConfigurationDictionary { get; private set; }

        public ConfigReader(string configPath)
        {
            var configContent = File.ReadAllText(configPath);
            var config = Toml.Parse(configContent).ToModel();

            ConfigurationDictionary = FlattenTomlTable(config);
        }

        private Dictionary<string, object> FlattenTomlTable(object tomlObject, string parentKey = "")
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (tomlObject is TomlTable table)
            {
                foreach (var key in table.Keys)
                {
                    var newKey = string.IsNullOrEmpty(parentKey) ? key : $"{parentKey}.{key}";
                    var value = table[key];
                    if (value is TomlTable)
                    {
                        var childDict = FlattenTomlTable(value, newKey);
                        foreach (var childKey in childDict.Keys)
                        {
                            result[childKey] = childDict[childKey];
                        }
                    }
                    else
                    {
                        result[newKey] = value;
                    }
                }
            }
            return result;
        }
    }
}
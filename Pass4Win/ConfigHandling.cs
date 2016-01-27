namespace Pass4Win
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using System.IO.IsolatedStorage;

    public class ConfigHandling
    {
        // Logging
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, object> values = new Dictionary<string, object>();
        private readonly IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

        private string configName = "Pass4Win.json";

        public ConfigHandling()
        {
            Load();
        }

        /// <summary>
        /// Indexer for the configuration, providing dynamic access to the collection of values in the configuration.</summary>
        /// <param name="key">Designates what configuration item will be changed or retrieved.</param>
        public dynamic this[string key]
        {
            get
            {
                try { 
                    return this.values[key];
                } 
                catch
                {
                    return "";
                }
            }
            set
            {
                this.values[key] = value;
                Save();
            }
        }

        /// <summary>
        /// Saves the configuration to the disk.</summary>
        public void Save()
        {
            string json = JsonConvert.SerializeObject(this.values, Formatting.Indented);
            using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(configName, FileMode.OpenOrCreate, isoStore))
            {
                using (StreamWriter writer = new StreamWriter(isoStream))
                {
                    writer.Write(json);

                }
            }
        }

        /// <summary>
        /// Removes an item from the collection.</summary>
        public void Delete(string key)
        {
            this.values.Remove(key);
            Save();
        }

        /// <summary>
        /// Resets the config</summary>
        public void ResetConfig()
        {
            isoStore.DeleteFile(configName);
        }


        /// <summary>
        /// Reloads the configuration from the disk.</summary>
        private void Load()
        {
            if (isoStore.FileExists(configName))
            {
                string json;
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(configName, FileMode.Open, isoStore))
                {
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                try
                {
                    this.values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                }
                catch (Exception message)
                {
                    log.Debug(message.Message);
                }
            }
        }
    }
}

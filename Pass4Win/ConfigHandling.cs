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

        private int defaultPassValidTime = 45;
        private int passValidTimeBottom = 15;
        private int passValidTimeTop = 300;

        private int cachedPassValidTime = 0;

        public ConfigHandling()
        {
            Load();
        }

        /// <summary>
        ///  Default length of time a password will be kept in the clipboard before clearing, in seconds
        /// </summary>
        public int DefaultPassValidTime
        {
            get
            {
                return defaultPassValidTime;
            }
        }

        /// <summary>
        /// Length of time the password is kept in the clipboard before clearing, in seconds.
        /// </summary>
        public int PassValidTime
        {
            get
            {
                if (cachedPassValidTime == 0)
                {
                    // this is a little paranoid, but a user may have edited the config by hand.
                    if (! int.TryParse(this["PassValidTime"], out cachedPassValidTime) ||
                        ! this.PassValidTimeValidator(cachedPassValidTime))
                    {
                        cachedPassValidTime = defaultPassValidTime;
                    }
                }

                return cachedPassValidTime;
            }
        }

        /// <summary>
        /// Bottom end of PassValidTime values, in seconds
        /// </summary>
        public int PassValidTimeBottom
        {
            get
            {
                return passValidTimeBottom;
            }
        }

        /// <summary>
        /// Top end of PassValidTime values, in seconds
        /// </summary>
        public int PassValidTimeTop
        {
            get
            {
                return passValidTimeTop;
            }
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
            cachedPassValidTime = 0;  // reset cache so a new config value will apply

            log.Debug("Saving config");
            string json = JsonConvert.SerializeObject(this.values, Formatting.Indented);
            IsolatedStorageFileStream isoStream = null;

            try
            {
                isoStream = new IsolatedStorageFileStream(configName, FileMode.OpenOrCreate, isoStore);

                using (StreamWriter writer = new StreamWriter(isoStream))
                {
                    isoStream = null;
                    writer.Write(json);
                }
            }
            finally
            {
                if (isoStream != null)
                    isoStream.Dispose();
            }
        }

        /// <summary>
        /// Removes an item from the collection.</summary>
        public void Delete(string key)
        {
            log.Debug("Deleting: " + key);
            this.values.Remove(key);
            Save();
        }

        /// <summary>
        /// Resets the config</summary>
        public void ResetConfig()
        {
            log.Debug("Deleting config file");
            isoStore.DeleteFile(configName);
        }

        /// <summary>
        /// Validate value of PassValidTime.
        /// </summary>
        /// <param name="value">PassValidTime in seconds</param>
        /// <returns>True if within bounds, else False</returns>
        public bool PassValidTimeValidator(int value)
        {
            return value >= passValidTimeBottom && value <= passValidTimeTop;
        }

        /// <summary>
        /// Reloads the configuration from the disk.</summary>
        private void Load()
        {
            log.Debug("Loading config file");
            if (isoStore.FileExists(configName))
            {
                string json;
                IsolatedStorageFileStream isoStream = null;

                // need to use a try/finally because a nested using could Dispose of the stream twice
                try
                {
                    isoStream = new IsolatedStorageFileStream(configName, FileMode.Open, isoStore);
                    using (StreamReader reader = new StreamReader(isoStream))
                    {
                        isoStream = null;
                        json = reader.ReadToEnd();
                    }
                }
                finally
                {
                    if (isoStream != null)
                        isoStream.Dispose();
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

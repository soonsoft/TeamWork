using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BingImageDownloader.Logging
{
    public class LogSection : ConfigurationSection
    {
        public const string SectionName = "logging";

        public LogSection()
            : base()
        {
        }

        [ConfigurationProperty("fileName", IsRequired = true)]
        public string FileName
        {
            get
            {
                return this["fileName"].ToString();
            }
            set
            {
                this["fileName"] = value;
            }
        }

        [ConfigurationProperty("defaultLogger", IsRequired = true)]
        public string DefaultLogger
        {
            get
            {
                return this["defaultLogger"].ToString();
            }
            set
            {
                this["defaultLogger"] = value;
            }
        }
    }
}

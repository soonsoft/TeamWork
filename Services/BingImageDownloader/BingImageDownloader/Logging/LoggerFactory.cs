using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace BingImageDownloader.Logging
{
    public static class LoggerFactory
    {
        private static string defaultLoggerName;
        private static bool canCreateLogger = false;
        public static bool CanCreateLogger
        {
            get
            {
                return canCreateLogger;
            }
        }

        private static string GetConfigPath(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                    throw new InitializationLoggingConfigException("Log4net的配置文件路径不能为空！");
            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            }
            else
            {
                string logConfigPath = AppDomain.CurrentDomain.BaseDirectory + fileName;
                logConfigPath = Path.GetFullPath(logConfigPath);
                return logConfigPath;
            }
        }

        private static void DefualtConfiguration()
        {
            defaultLoggerName = "DefaultLogger";
            Stream configStream = typeof(LoggerFactory).Assembly.GetManifestResourceStream("BingImageDownloader.Logging.Log4net.config");
            log4net.Config.XmlConfigurator.Configure(configStream);
            canCreateLogger = true;
        }

        private static void InitializationLoggingConfig()
        {
            LogSection logSection = ConfigurationManager.GetSection(LogSection.SectionName) as LogSection;
            if (logSection != null)
            {
                defaultLoggerName = logSection.DefaultLogger;
                string configPath = GetConfigPath(logSection.FileName);
                if (!File.Exists(configPath))
                {
                    DefualtConfiguration();
                }
                try
                {
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
                    canCreateLogger = true;
                }
                catch
                { }
                
            }
            else
            {
                DefualtConfiguration();
            }
        }

        public static Log GetLogger()
        {
            if (!canCreateLogger)
                InitializationLoggingConfig();
            return new Log(log4net.LogManager.GetLogger(defaultLoggerName));
        }

        public static Log GetLogger(string loggerName)
        {
            if (String.IsNullOrEmpty(loggerName))
                throw new ArgumentException("loggerName不能为null或\"\"!");
            if (!canCreateLogger)
                InitializationLoggingConfig();
            return new Log(log4net.LogManager.GetLogger(loggerName));
        }
    }
}

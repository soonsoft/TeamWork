using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BingImageDownloader.Logging
{
    public class Log
    {
        private readonly log4net.ILog innerLog = null;

        public Log(log4net.ILog log)
        {
            innerLog = log;
        }

        public log4net.ILog Current
        {
            get
            {
                return innerLog;
            }
        }

        public static Log Get()
        {
            return LoggerFactory.GetLogger();
        }

        public static Log Get(string loggerName)
        {
            return LoggerFactory.GetLogger(loggerName);
        }
    }
}

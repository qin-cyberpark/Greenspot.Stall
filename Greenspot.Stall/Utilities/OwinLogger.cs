using log4net;
using Microsoft.Owin.Logging;
using System;
using System.Diagnostics;

namespace Greenspot.Stall.Utilities
{
    public class OwinLoggerFactory : ILoggerFactory
    {
        public ILogger Create(string name)
        {
            return new OwinLogger(log4net.LogManager.GetLogger("SysLogger"));
        }
    }

    public class OwinLogger : ILogger
    {
        private readonly ILog _logger;

        public OwinLogger(ILog logger)
        {
            _logger = logger;
        }

        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (state == null)
            {
                _logger.Info(eventType.ToString());
                return true;
            }
            else
            {
                _logger.Info(formatter(state, exception), exception);
                return true;
            }
        }
    }
}

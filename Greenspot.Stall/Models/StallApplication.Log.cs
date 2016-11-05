using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall
{
    public partial class StallApplication
    {
        private static readonly log4net.ILog _sysLogger = log4net.LogManager.GetLogger("SysLogger");
        private static readonly log4net.ILog _bizLogger = log4net.LogManager.GetLogger("BizLogger");

        public static void SysInfoFormat(string format, params object[] args)
        {
            _sysLogger.InfoFormat(format, args);
        }
        public static void SysInfo(string message)
        {
            _sysLogger.InfoFormat(message);
        }
        public static void SysErrorFormat(string format, params object[] args)
        {
            _sysLogger.ErrorFormat(format, args);
        }
        public static void SysError(object message)
        {
            _sysLogger.Error(message);
        }
        public static void SysError(object message, Exception ex)
        {
            _sysLogger.Error(message, ex);
        }


        public static void BizInfoFormat(string format, params object[] args)
        {
            _bizLogger.InfoFormat(format, args);
        }
        public static void BizErrorFormat(string format, params object[] args)
        {
            _bizLogger.ErrorFormat(format, args);
        }
        public static void BizError(object message)
        {
            _bizLogger.Error(message);
        }
        public static void BizError(object message, Exception ex)
        {
            _bizLogger.Error(message, ex);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public class AccessTokenNotInitializedException : Exception
    {
        public string _msg;
        public AccessTokenNotInitializedException(string clientId, string prefix)
        {
            _msg = string.Format("Access Token not been initilazied. Client ID = {0}, Retailer Prefix = {1}.", clientId, prefix);
        }

        public override string Message
        {
            get
            {
                return _msg;
            }
        }
    }

    public class NullApplicationException : Exception
    {
        public override string Message
        {
            get
            {
                return "Application can not be NULL.";
            }
        }
    }
}

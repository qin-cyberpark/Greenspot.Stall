using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public interface IAccessTokenStore
    {
        AccessTokenBag ReadAccessToken(string clientId, string prefix);
        bool WriteAccessToken(string clientId, string prefix, AccessTokenBag accessTokenInfo);
    }
}

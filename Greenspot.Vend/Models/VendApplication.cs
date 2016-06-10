using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public class VendApplication
    {
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public string RedirectUri { get; private set; }

        public VendApplication(string clientId, string clientSecret, string redirectUri)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
        }

        public string GetAuthorisationCodeUri(string state)
        {
            return string.Format(@"https://secure.vendhq.com/connect?response_type=code&client_id={0}&redirect_uri={1}&state={2}",
                                ClientId, RedirectUri, state);
        }
    }
}

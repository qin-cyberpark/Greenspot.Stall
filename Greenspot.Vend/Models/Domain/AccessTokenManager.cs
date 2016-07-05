using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Greenspot.SDK.Vend
{
    public class AccessTokenManager
    {
        public VendApplication VendApplication { get; set; }
        private Dictionary<string, AccessTokenBag> _accessTokens;
        private IAccessTokenStore _accessTokenStore;

    
        public AccessTokenManager(IAccessTokenStore accessTokenStore,  VendApplication defaultApp)
        {
            VendApplication = defaultApp;
            _accessTokens = new Dictionary<string, AccessTokenBag>();
            _accessTokenStore = accessTokenStore;

        }

        public async Task<string> GetAccessTokenAsync(string prefix)
        {
            AccessTokenBag token = null;
            if (_accessTokens.ContainsKey(prefix))
            {
                token = _accessTokens[prefix];
            }
            else
            {
                token = _accessTokenStore.ReadAccessToken(VendApplication?.ClientId, prefix);
            }

            if (token == null)
            {
                //not exist - need init
                throw new AccessTokenNotInitializedException(VendApplication?.ClientId, prefix);
            }
            else
            {
                if (token.HasExpired)
                {
                    //expired
                    var newToken = await RefreshAccessTokenAsync(prefix, token.RefreshToken);
                    if (newToken != null)
                    {
                        token.AccessToken = newToken.AccessToken;
                        token.Expires = newToken.Expires;
                        token.ExpiresIn = newToken.ExpiresIn;

                        //save token
                        _accessTokens[prefix] = token;
                        _accessTokenStore.WriteAccessToken(VendApplication?.ClientId, prefix, token);
                    }
                }
                return token.AccessToken;
            }
        }

        public async Task<string> GetAccessTokenAsync(string prefix, string authCode)
        {
            if(string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(authCode))
            {
                return null;
            }

            if (VendApplication == null)
            {
                throw new NullApplicationException();   
            }

            var data = new KeyValuePair<string, string>[] {
                            new KeyValuePair<string, string>("code",authCode),
                            new KeyValuePair<string, string>("client_id",VendApplication.ClientId),
                            new KeyValuePair<string, string>("client_secret",VendApplication.ClientSecret),
                            new KeyValuePair<string, string>("grant_type","authorization_code"),
                            new KeyValuePair<string, string>("redirect_uri",VendApplication.RedirectUri)};

            var token = await HttpUtility.PostUrlencodedFormAsync<AccessTokenBag>(GetRequestUri(prefix, "1.0/token"), data);

            //save token
            _accessTokens[prefix] = token;
            _accessTokenStore.WriteAccessToken(VendApplication?.ClientId, prefix, token);

            return token?.AccessToken;
        }
     
        private async Task<AccessTokenBag> RefreshAccessTokenAsync(string prefix, string refreshToken)
        {
            if (VendApplication == null)
            {
                throw new NullApplicationException();
            }

            var data = new KeyValuePair<string, string>[] {
                            new KeyValuePair<string, string>("refresh_token",refreshToken),
                            new KeyValuePair<string, string>("client_id",VendApplication.ClientId),
                            new KeyValuePair<string, string>("client_secret",VendApplication.ClientSecret),
                            new KeyValuePair<string, string>("grant_type","authorization_code")};

            return await HttpUtility.PostUrlencodedFormAsync<AccessTokenBag>(GetRequestUri(prefix, "1.0/token"), data);
        }

        private string GetRequestUri(string prefix, string requestUri) {
            return HttpUtility.GetRequestUri(prefix, requestUri);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greenspot.SDK.Vend;
using Greenspot.Configuration;
using Greenspot.Stall.Models;

namespace Greenspot.Stall
{
    public class StallApplication
    {
        private VendApplication _vendApplication;
        private AccessTokenManager _vendAccessTokenManager;
        private StallApplication(VendApplication vendApp, AccessTokenManager tokenManager)
        {
            _vendApplication = vendApp;
            _vendAccessTokenManager = tokenManager;
        }

        private static StallApplication _instance;

        #region static
        static StallApplication()
        {
            var vendApp = new VendApplication(GreenspotConfiguration.AccessAccounts["vend"].Id,
                                              GreenspotConfiguration.AccessAccounts["vend"].Secret,
                                              GreenspotConfiguration.AccessAccounts["vend"].RedirectUri);

            var tokenManager = new AccessTokenManager(new VendAccessTokenStore(), vendApp);
            _instance = new StallApplication(vendApp, tokenManager);
        }

        public static string GetAuthorisationCodeUri(string state)
        {
            return _instance._vendApplication.GetAuthorisationCodeUri(state);
        }
        public static string GetAuthorisationCodeUri(string prefix, string state)
        {
            return _instance._vendApplication.GetAuthorisationCodeUri(prefix, state);
        }

        public static async Task<string> GetAccessTokenAsync(string prefix)
        {
            return await _instance._vendAccessTokenManager.GetAccessTokenAsync(prefix);
        }

        public static async Task<string> GetAccessTokenAsync(string prefix, string authCode)
        {
            return await _instance._vendAccessTokenManager.GetAccessTokenAsync(prefix, authCode);
        }
        public static string VendClientId
        {
            get
            {
                return _instance._vendApplication.ClientId;
            }
        }
        #endregion
    }
}

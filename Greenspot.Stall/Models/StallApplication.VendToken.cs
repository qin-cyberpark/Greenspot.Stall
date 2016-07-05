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

        private Dictionary<string, Area> _areas;

        private static StallApplication _instance;

        #region static
        static StallApplication()
        {
            var vendApp = new VendApplication(GreenspotConfiguration.AccessAccounts["vend"].Id,
                                              GreenspotConfiguration.AccessAccounts["vend"].Secret,
                                              GreenspotConfiguration.AccessAccounts["vend"].RedirectUri);

            var tokenManager = new AccessTokenManager(new VendAccessTokenStore(), vendApp);
            _instance = new StallApplication();
            _instance._vendApplication = vendApp;
            _instance._vendAccessTokenManager = tokenManager;

            using (var db = new StallEntities())
            {
                _instance._areas = new Dictionary<string, Area>();
                foreach (var a in db.Areas.ToList())
                {
                    _instance._areas.Add(a.ID, a);
                }
            }
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
        public static Area GetArea(string id)
        {
            if (_instance._areas.ContainsKey(id))
            {
                return _instance._areas[id];
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Greenspot.SDK.Vend;
namespace Greenspot.Stall.Models
{
    public class VendAccessTokenStore : IAccessTokenStore
    {
        private StallEntities _db = new StallEntities();
        public AccessTokenBag ReadAccessToken(string clientId, string prefix)
        {
            var token = _db.VendAccessTokens.FirstOrDefault(x => x.Prefix.Equals(prefix));
            if (token != null)
            {
                return new AccessTokenBag()
                {
                    AccessToken = token.AccessToken,
                    Expires = token.Expires,
                    ExpiresIn = token.ExpiresIn,
                    RefreshToken = token.RefreshToken,
                    DomainPrefix = token.Prefix,
                    TokenType = token.TokenType
                };
            }
            else
            {
                return null;
            }
        }

        public bool WriteAccessToken(string clientId, string prefix, AccessTokenBag accessTokenInfo)
        {
            var token = _db.VendAccessTokens.FirstOrDefault(x => x.Prefix.Equals(prefix));
            if (token != null)
            {
                //update
                token.ExpiresIn = accessTokenInfo.ExpiresIn;
                token.Expires = accessTokenInfo.Expires;
                token.AccessToken = accessTokenInfo.AccessToken;
                token.RefreshToken = accessTokenInfo.RefreshToken;
                token.TokenType = accessTokenInfo.TokenType;
                _db.Entry(token).State = System.Data.Entity.EntityState.Modified;

            }
            else
            {
                //new
                token = new VendAccessToken()
                {
                    Prefix = prefix,
                    ExpiresIn = accessTokenInfo.ExpiresIn,
                    Expires = accessTokenInfo.Expires,
                    AccessToken = accessTokenInfo.AccessToken,
                    RefreshToken = accessTokenInfo.RefreshToken,
                    TokenType = accessTokenInfo.TokenType
                };
                _db.VendAccessTokens.Add(token);

            }

            return _db.SaveChanges() == 1;
        }
    }
}
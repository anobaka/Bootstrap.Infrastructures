using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Bootstrap.Infrastructures.Extensions
{
    public abstract class SessionManager<TUser>
    {
        private readonly IHttpContextAccessor _ctxAccessor;
        protected abstract string Scheme { get; }

        protected SessionManager(IHttpContextAccessor ctxAccessor)
        {
            _ctxAccessor = ctxAccessor;
        }

        protected HttpContext HttpContext => _ctxAccessor.HttpContext;

        protected abstract List<Claim> BuildClaims(TUser user);

        public virtual async Task SignInAsync(TUser user, string authorizationType, DateTimeOffset? expireDt = null)
        {
            var claims = BuildClaims(user);
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, authorizationType) {Label = Scheme});
            await HttpContext.SignInAsync(Scheme, principal,
                expireDt.HasValue ? new AuthenticationProperties {IsPersistent = true, ExpiresUtc = expireDt} : null);
        }

        public virtual async Task SignOutAsync()
        {
            await HttpContext.SignOutAsync(Scheme);
        }

        protected virtual string GetClaim(string claim)
        {
            var str = HttpContext.User.Identities.FirstOrDefault(t => t.Label == Scheme)?.FindFirst(claim).Value;
            return str;
        }
    }
}
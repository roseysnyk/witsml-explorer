using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using WitsmlExplorer.Api.Configuration;
using WitsmlExplorer.Api.Extensions;
using WitsmlExplorer.Api.Services;

namespace WitsmlExplorer.Api.HttpHandlers
{
    public static class AuthorizeHandler
    {
        public static async Task<IResult> Authorize([FromQuery(Name = "keep")] bool keep, [FromServices] ICredentialsService credentialsService, HttpContext httpContext, IConfiguration configuration)
        {
            EssentialHeaders eh = new(httpContext?.Request);
            bool useOAuth2 = StringHelpers.ToBoolean(configuration[ConfigConstants.OAuth2Enabled]);
            string clientId = useOAuth2 ? credentialsService.GetClientId(eh) : httpContext.GetOrCreateWitsmlExplorerCookie();
            bool success = await credentialsService.VerifyAndCacheCredentials(eh, keep, clientId);
            if (success)
            {
                return TypedResults.Ok();
            }
            return TypedResults.Unauthorized();
        }

        public static IResult Deauthorize(IConfiguration configuration, HttpContext httpContext, [FromServices] ICredentialsService credentialsService)
        {
            bool useOAuth2 = StringHelpers.ToBoolean(configuration[ConfigConstants.OAuth2Enabled]);
            EssentialHeaders eh = new(httpContext?.Request);
            if (!useOAuth2)
            {
                httpContext.Response.Cookies.Delete(EssentialHeaders.CookieName);
            }
            string cacheClientId = credentialsService.GetClientId(eh);
            credentialsService.RemoveCachedCredentials(cacheClientId);

            return TypedResults.Ok();
        }
    }
}

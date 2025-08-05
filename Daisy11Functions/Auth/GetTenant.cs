using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using NewWorldFunctions.Helpers;
using Daisy11Functions.Helpers;

namespace Daisy11Functions;

public static class GetTenant
{
    public static string Value(HttpRequestData req)
    {
        IHttpCookie? cookieTenant = req.Cookies.FirstOrDefault(x => x.Name == "active_tenant");
        return cookieTenant == null ? "nice-beach-erikson" : cookieTenant.Value;
    }

}
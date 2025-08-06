using Microsoft.Azure.Functions.Worker.Http;

namespace Daisy11Functions;

public static class GetSubdomain
{
    public static string Value(HttpRequestData req)
    {
        IHttpCookie? cookieTenant = req.Cookies.FirstOrDefault(x => x.Name == "active_tenant");
        return cookieTenant == null ? "admin" : cookieTenant.Value;
    }
}
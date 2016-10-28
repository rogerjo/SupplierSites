using Microsoft.SharePoint.Client;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

public class SPHttpClientHandler : HttpClientHandler
{
    public SPHttpClientHandler(Uri webUri, string userName, string password)
    {
        CookieContainer = GetAuthCookies(webUri, userName, password);
        FormatType = FormatType.Json;
    }


    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
        if(FormatType == FormatType.Json)
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return base.SendAsync(request, cancellationToken);
    }


    /// <summary>
    /// Retrieve SPO Auth Cookies 
    /// </summary>
    /// <param name="webUri"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private static CookieContainer GetAuthCookies(Uri webUri, string userName, string password)
    {
        var securePassword = new SecureString();
        foreach (var c in password) { securePassword.AppendChar(c); }
        var credentials = new SharePointOnlineCredentials("rogerjn", securePassword);
        var authCookie = credentials.GetAuthenticationCookie(webUri);
        var cookieContainer = new CookieContainer();
        cookieContainer.SetCookies(webUri, authCookie);
        return cookieContainer;
    }


    public FormatType FormatType { get; set; }
}

public enum FormatType
{
    Json,
    Xml
}

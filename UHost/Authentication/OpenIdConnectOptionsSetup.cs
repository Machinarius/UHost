using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace UHost.Authentication {
  internal class OpenIdConnectOptionsSetup : IConfigureOptions<OpenIdConnectOptions> {
    public OpenIdConnectOptionsSetup(IOptions<AzureADB2COptions> b2cOptions) {
      B2COptions = b2cOptions.Value;
    }

    public AzureADB2COptions B2COptions { get; set; }

    public void Configure(OpenIdConnectOptions options) {
      options.ClientId = B2COptions.ClientId;
      options.Authority = B2COptions.Authority;
      options.UseTokenLifetime = true;
      options.TokenValidationParameters = new TokenValidationParameters() { NameClaimType = "name" };

      options.Events = new OpenIdConnectEvents() {
        OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
        OnRemoteFailure = OnRemoteFailure,
        OnAuthorizationCodeReceived = OnAuthorizationCodeReceived
      };
    }

    public Task OnRedirectToIdentityProvider(RedirectContext context) {
      var defaultPolicy = B2COptions.DefaultPolicy;
      if (context.Properties.Items.TryGetValue(AzureADB2COptions.PolicyAuthenticationProperty, out var policy) &&
          !policy.Equals(defaultPolicy)) {
        context.ProtocolMessage.Scope = OpenIdConnectScope.OpenIdProfile;
        context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.IdToken;
        context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower().Replace(defaultPolicy.ToLower(), policy.ToLower());
        context.Properties.Items.Remove(AzureADB2COptions.PolicyAuthenticationProperty);
      } else if (!string.IsNullOrEmpty(B2COptions.ApiUrl)) {
        context.ProtocolMessage.Scope += $" offline_access {B2COptions.ApiScopes}";
        context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
      }
      return Task.FromResult(0);
    }

    public Task OnRemoteFailure(FailureContext context) {
      context.HandleResponse();
      // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page 
      // because password reset is not supported by a "sign-up or sign-in policy"
      if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("AADB2C90118")) {
        // If the user clicked the reset password link, redirect to the reset password route
        context.Response.Redirect("/Session/ResetPassword");
      } else if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied")) {
        context.Response.Redirect("/");
      } else {
        context.Response.Redirect("/Home/Error?message=" + context.Failure.Message);
      }
      return Task.FromResult(0);
    }

    public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context) {
      // Use MSAL to swap the code for an access token
      // Extract the code from the response notification
      var code = context.ProtocolMessage.Code;

      var signedInUserID = context.Ticket.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
      var userTokenCache = new MSALSessionCache(signedInUserID, context.HttpContext).GetMsalCacheInstance();
      var cca = new ConfidentialClientApplication(B2COptions.ClientId, B2COptions.Authority, B2COptions.RedirectUri, new ClientCredential(B2COptions.ClientSecret), userTokenCache, null);

      var result = await cca.AcquireTokenByAuthorizationCodeAsync(code, B2COptions.ApiScopes.Split(' '));
      context.HandleCodeRedemption(result.AccessToken, result.IdToken);
    }
  }
}
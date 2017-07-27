using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace UHost.Controllers {
  public class SessionController : Controller {
    public SessionController(IOptions<AzureADB2COptions> b2cOptions) {
      B2COptions = b2cOptions.Value;
    }

    public AzureADB2COptions B2COptions { get; set; }

    [HttpGet]
    public async Task SignIn() {
      await HttpContext.Authentication.ChallengeAsync(
          OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
    }

    [HttpGet]
    public async Task ResetPassword() {
      var properties = new AuthenticationProperties() { RedirectUri = "/" };
      properties.Items[AzureADB2COptions.PolicyAuthenticationProperty] = B2COptions.ResetPasswordPolicyId;
      await HttpContext.Authentication.ChallengeAsync(
          OpenIdConnectDefaults.AuthenticationScheme, properties, ChallengeBehavior.Unauthorized);
    }

    [HttpGet]
    public async Task EditProfile() {
      var properties = new AuthenticationProperties() { RedirectUri = "/" };
      properties.Items[AzureADB2COptions.PolicyAuthenticationProperty] = B2COptions.EditProfilePolicyId;
      await HttpContext.Authentication.ChallengeAsync(
          OpenIdConnectDefaults.AuthenticationScheme, properties, ChallengeBehavior.Unauthorized);
    }

    [HttpGet]
    public IActionResult SignOut() {
      return SignOut(
          new AuthenticationProperties { RedirectUri = Url.Action(nameof(SignedOut)) },
          CookieAuthenticationDefaults.AuthenticationScheme,
          OpenIdConnectDefaults.AuthenticationScheme);
    }

    [HttpGet]
    public IActionResult SignedOut() {
      return View();
    }
  }
}

using System.Linq;
using System.Security.Claims;

namespace UHost.Controllers.Utility {
  public static class UserExtensions {
    public static string GetId(this ClaimsPrincipal principal) {
      return principal.Claims.First(cl => cl.Type == ClaimTypes.NameIdentifier).Value;
    }
  }
}

using System;
using Microsoft.AspNetCore.Mvc;

namespace UHost.Controllers {
  public class FilesController : Controller {
    public IActionResult Mine() {
      var userAuthenticated = User?.Identity.IsAuthenticated ?? false;
      if (!userAuthenticated) {
        return RedirectToAction(nameof(SignInRequired));
      }

      throw new NotImplementedException();
    }

    public IActionResult SignInRequired() {
      return View();
    }
  }
}

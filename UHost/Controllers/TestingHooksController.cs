using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace UHost.Controllers {
  public class TestingHooksController : Controller {
    private IHostingEnvironment environment;
    private IApplicationLifetime appLifetime;

    public TestingHooksController(IHostingEnvironment environment, IApplicationLifetime appLifetime) {
      this.environment = environment;
      this.appLifetime = appLifetime;
    }

    public IActionResult Shutdown() {
      if (!environment.IsEnvironment("Testing")) {
        return NotFound();
      }

      appLifetime.StopApplication();
      return Ok();
    }
  }
}

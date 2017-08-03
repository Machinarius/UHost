using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UHost.Controllers.Utility;
using UHost.Services;
using UHost.ViewModels;

namespace UHost.Controllers {
  public class FilesController : Controller {
    private IHostedFilesService filesService;
    
    public FilesController(IHostedFilesService filesService) {
      if (filesService == null) {
        throw new ArgumentNullException(nameof(filesService));
      }

      this.filesService = filesService;
    }

    public async Task<IActionResult> Mine() {
      var userAuthenticated = User?.Identity.IsAuthenticated ?? false;
      if (!userAuthenticated) {
        return RedirectToAction(nameof(SignInRequired));
      }

      var userId = User.GetId();
      var userFiles = await filesService.GetUserHostedFilesAsync(userId);
      var viewModels = userFiles.Select(uF => new HostedFileViewModel()).ToArray();
      return View(viewModels);
    }

    public IActionResult SignInRequired() {
      return View();
    }
  }
}

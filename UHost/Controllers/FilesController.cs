using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UHost.Controllers.Utility;
using UHost.Models;
using UHost.Services;
using UHost.ViewModels;

namespace UHost.Controllers {
  [Authorize]
  public class FilesController : Controller {
    private IHostedFilesService filesService;
    private ILogger<FilesController> logger;
    
    public FilesController(IHostedFilesService filesService, ILogger<FilesController> logger) {
      this.logger = logger;
      this.filesService = filesService;
    }
    
    [Route("/")]
    [Route("/Files/Mine")]
    public async Task<IActionResult> Mine() {
      var userId = User.GetId();
      var userFiles = await filesService.GetUserHostedFilesAsync(userId);
      var viewModelTasks = userFiles.Select(async uF => {
        var downloadLink = await filesService.ResolveFileUrlAsync(uF);
        return new HostedFileViewModel(uF, downloadLink);
      }).ToArray();
      var viewModels = await Task.WhenAll(viewModelTasks);

      return View(viewModels.AsEnumerable());
    }

    [HttpPost]
    [Route("/Files/Create")]
    public async Task<IActionResult> CreateFile(FileCreationRequestViewModel request) {
      if (string.IsNullOrEmpty(request.Title) || request == null) {
        return new BadRequestResult();
      }

      HostedFile generatedFile;

      var userId = User.GetId();
      var fileName = request.File.FileName;
      var contentType = request.File.ContentType;
      using (var fileStream = request.File.OpenReadStream()) {
        generatedFile = 
          await filesService.CreateFileAsync(userId, fileName, contentType, fileStream, request.Title, request.Description);
      }

      /*
      var safeId = generatedFile.Id.ToUrlSafeString();
      return RedirectToAction(nameof(FileDetails), new {
        fileIdString = safeId
      });
      */

      // The file details view is not yet ready!
      return Ok();
    }
    
    [Route("/Files/{fileIdString}")]
    public async Task<IActionResult> FileDetails(string fileIdString) {
      Guid fileId;
      try {
        fileId = fileIdString.ToGuid();
      } catch (Exception ex) {
        logger.LogError("Error resolving a hosted file with id: '{0}'.\n{1}", fileIdString, ex.StackTrace);
        return NotFound();
      }

      HostedFile hostedFile;
      try {
        hostedFile = await filesService.GetHostedFileAsync(fileId);
      } catch (Exception ex) {
        logger.LogError("Error resolving a hosted file with id: '{0}'.\n{1}", fileIdString, ex.StackTrace);
        return NotFound();
      }

      var fileUrl = await filesService.ResolveFileUrlAsync(hostedFile);
      var viewModel = new HostedFileViewModel(hostedFile, fileUrl);

      return View(viewModel);
    }

    [AllowAnonymous]
    public IActionResult SignInRequired() {
      return View();
    }
  }
}

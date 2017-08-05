using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UHost.Controllers {
  [Authorize]
  public class FileDownloadController : Controller {
    private const string UserFilesDirectory = @"Data\UserFiles";

    [Route("/FileDownload/{fileSlug}")]
    public IActionResult DownloadFile(string fileSlug) {
      var filePath = Path.Combine(UserFilesDirectory, fileSlug);
      if (!System.IO.File.Exists(filePath)) {
        return NotFound();
      }

      var contentType = "application/octet-stream";
      var fileExtension = Path.GetExtension(fileSlug)?.ToLowerInvariant().TrimStart('.');
      switch(fileExtension) {
        case "png":
        case "jpeg":
        case "bmp":
        case "gif":
        contentType = "image/" + fileExtension;
        break;
        case "jpg":
        contentType = "image/jpeg";
        break;
      }

      var fileStream = System.IO.File.Open(filePath, FileMode.Open);
      return File(fileStream, contentType);
    }
  }
}

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UHost.Services.Results;

namespace UHost.Services.LocalServices {
  public class LocalFileSystemFileStorageService : IFileStorageService {
    private const string TargetDirectory = @"Data\UserFiles";

    private IHttpContextAccessor contextAccessor;

    public LocalFileSystemFileStorageService(IHttpContextAccessor contextAccessor) {
      this.contextAccessor = contextAccessor;
    }

    public Task<string> GetFileUrlAsync(string fileSlug) {
      if (string.IsNullOrEmpty(fileSlug)) {
        throw new ArgumentNullException(nameof(fileSlug));
      }

      var httpContext = contextAccessor.HttpContext;
      var fileUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/FileDownload/{fileSlug}";
      return Task.FromResult(fileUrl);
    }

    public async Task<FileStoreResult> StoreFileAsync(Stream fileStream, string contentType = null, string suggestedName = null) {
      if (fileStream == null) {
        throw new ArgumentNullException(nameof(fileStream));
      }

      var fileName = Guid.NewGuid().ToString();
      if (!string.IsNullOrEmpty(suggestedName)) {
        var extension = Path.GetExtension(suggestedName);
        if (!string.IsNullOrEmpty(extension)) {
          fileName += extension;
        }
      }

      if (!Directory.Exists(TargetDirectory)) {
        Directory.CreateDirectory(TargetDirectory);
      }

      var targetFilePath = Path.Combine(TargetDirectory, fileName);
      using (var targetFile = File.Create(targetFilePath)) {
        await fileStream.CopyToAsync(targetFile);
      }

      return new FileStoreResult(fileName);
    }
  }
}

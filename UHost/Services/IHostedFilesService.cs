using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UHost.Models;

namespace UHost.Services {
  public interface IHostedFilesService {
    Task<IEnumerable<HostedFile>> GetUserHostedFilesAsync(string userId);
    Task<HostedFile> CreateFileAsync(string userId, string fileName, string contentType, Stream fileStream, string title, string description);
    Task<string> ResolveFileUrlAsync(HostedFile hostedFile);
    Task<HostedFile> GetHostedFileAsync(Guid fileId);
  }
}

using System.IO;
using System.Threading.Tasks;
using UHost.Services.Results;

namespace UHost.Services {
  public interface IFileStorageService {
    Task<FileStoreResult> StoreFileAsync(Stream fileStream, string suggestedName = null);
    Task<string> GetFileUrlAsync(string fileSlug);
  }
}

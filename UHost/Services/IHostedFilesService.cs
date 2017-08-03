using System.Collections.Generic;
using System.Threading.Tasks;
using UHost.Models;

namespace UHost.Services {
  public interface IHostedFilesService {
    Task<IEnumerable<HostedFile>> GetUserHostedFilesAsync(string userId);
  }
}

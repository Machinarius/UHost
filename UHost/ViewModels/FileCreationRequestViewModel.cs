using Microsoft.AspNetCore.Http;

namespace UHost.ViewModels {
  public class FileCreationRequestViewModel {
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile File { get; set; }
  }
}

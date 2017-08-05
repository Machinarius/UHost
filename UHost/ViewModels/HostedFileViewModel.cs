using System;
using UHost.Models;

namespace UHost.ViewModels {
  public class HostedFileViewModel {
    public string Title { get; }
    public string Description { get; }
    public DateTime CreationDate { get; }
    public string DownloadUrl { get; }
    
    public HostedFileViewModel(HostedFile hostedFile, string fileUrl) {
      if (hostedFile == null) {
        throw new ArgumentNullException(nameof(hostedFile));
      }

      if (string.IsNullOrEmpty(fileUrl)) {
        throw new ArgumentNullException(nameof(fileUrl));
      }

      Title = hostedFile.Title;
      Description = hostedFile.Description;
      CreationDate = hostedFile.CreationDate;
      DownloadUrl = fileUrl;
    }
  }
}

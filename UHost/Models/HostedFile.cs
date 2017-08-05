using System;

namespace UHost.Models {
  public class HostedFile {
    public Guid Id { get; set; }
    public string OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileSlug { get; set; }
    public string ContentType { get; set; }
    public DateTime CreationDate { get; set; }

    public HostedFile() { }

    public HostedFile(string ownerId, string title, string description, string contentType, string fileSlug) {
      OwnerId = ownerId;
      Title = title;
      Description = description;
      ContentType = contentType;
      FileSlug = fileSlug;
      CreationDate = DateTime.UtcNow;
    }
  }
}

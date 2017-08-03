using System;

namespace UHost.Models {
  public class HostedFile {
    public Guid Id { get; set; }
    public string Slugid { get; set; }
    public string OwnerId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string FileSlug { get; set; }
    public string ContentType { get; set; }
    public DateTime CreationDate { get; set; }
  }
}

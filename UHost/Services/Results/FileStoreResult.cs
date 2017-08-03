namespace UHost.Services.Results {
  public class FileStoreResult {
    public string GeneratedSlug { get; }

    internal FileStoreResult(string generatedSlug) {
      GeneratedSlug = generatedSlug;
    }
  }
}

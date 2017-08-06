using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using UHost.Services.Exceptions;
using UHost.Services.RemoteServices.Configuration;
using UHost.Services.Results;

namespace UHost.Services.RemoteServices {
  public class BlobStorageFileStorageService : IFileStorageService {
    private AzureServicesConfiguration azureConfig;

    private CloudStorageAccount cloudStorage;
    private CloudBlobClient blobClient;
    private CloudBlobContainer blobContainer;

    public BlobStorageFileStorageService(IOptions<AzureServicesConfiguration> azureOptions) {
      if (azureOptions == null) {
        throw new ArgumentNullException(nameof(azureOptions));
      }

      azureConfig = azureOptions.Value;
      cloudStorage = CloudStorageAccount.Parse(azureConfig.BlobStorageConnectionString);
      blobClient = cloudStorage.CreateCloudBlobClient();
      blobContainer = blobClient.GetContainerReference(azureConfig.BlobStorageContainer);

      initSemaphore = new SemaphoreSlim(1, 1);
    }

    private bool initCompleted;
    private SemaphoreSlim initSemaphore;

    private async Task InitializeStorageAsync() {
      if (initCompleted) {
        return;
      }

      await initSemaphore.WaitAsync(); 
      try {
        await blobContainer.CreateIfNotExistsAsync();

        var containerPermissions = await blobContainer.GetPermissionsAsync();
        containerPermissions.PublicAccess = BlobContainerPublicAccessType.Blob;
        await blobContainer.SetPermissionsAsync(containerPermissions);

        initCompleted = true;
      } finally {
        initSemaphore.Release();
      }
    }

    public async Task<string> GetFileUrlAsync(string fileSlug) {
      if (string.IsNullOrEmpty(fileSlug)) {
        throw new ArgumentNullException(nameof(fileSlug));
      }

      await InitializeStorageAsync();

      var blobReference = blobContainer.GetBlockBlobReference(fileSlug);
      if (!await blobReference.ExistsAsync()) {
        throw new ResourceNotFoundException();
      }

      return blobReference.Uri.ToString();
    }

    public async Task<FileStoreResult> StoreFileAsync(Stream fileStream, string contentType = null,
                                                      string suggestedName = null) {
      if (fileStream == null) {
        throw new ArgumentNullException(nameof(fileStream));
      }

      await InitializeStorageAsync();

      if (string.IsNullOrEmpty(contentType)) {
        contentType = "application/octet-stream";
      }

      var fileName = Guid.NewGuid().ToString();
      if (!string.IsNullOrEmpty(suggestedName)) {
        var extension = Path.GetExtension(suggestedName);
        if (!string.IsNullOrEmpty(extension)) {
          fileName += extension;
        }
      }

      var blobReference = blobContainer.GetBlockBlobReference(fileName);
      using (fileStream) {
        await blobReference.UploadFromStreamAsync(fileStream);
        blobReference.Properties.ContentType = contentType;
        await blobReference.SetPropertiesAsync();
      }

      return new FileStoreResult(fileName);
    }
  }
}

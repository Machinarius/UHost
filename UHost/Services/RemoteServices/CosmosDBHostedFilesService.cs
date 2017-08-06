using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using UHost.Models;
using UHost.Services.Exceptions;
using UHost.Services.RemoteServices.Configuration;

namespace UHost.Services.RemoteServices {
  public class CosmosDBHostedFilesService : IHostedFilesService {
    private DocumentClient docClient;
    private AzureServicesConfiguration azureConfig;

    private Database database;
    private DocumentCollection userFilesCollection;

    private IFileStorageService fileStorage;
    
    public CosmosDBHostedFilesService(IOptions<AzureServicesConfiguration> azureOptions,
                                      IFileStorageService fileStorage) {
      if (azureOptions == null) {
        throw new ArgumentNullException(nameof(azureOptions));
      }

      if (fileStorage == null) {
        throw new ArgumentNullException(nameof(fileStorage));
      }

      this.fileStorage = fileStorage;
 
      azureConfig = azureOptions.Value;
      Collections.InitCollections(azureConfig.CosmosDBId);

      database = new Database {
        Id = azureConfig.CosmosDBId
      };

      userFilesCollection = new DocumentCollection {
        Id = Collections.Ids.UserFiles
      };

      var cosmosEndpointUri = new Uri(azureConfig.CosmosDBEndpointUrl);
      docClient = new DocumentClient(cosmosEndpointUri, azureConfig.CosmosDBKey);

      initSemaphore = new SemaphoreSlim(1, 1);
    }

    private bool initComplete;
    private SemaphoreSlim initSemaphore;

    private async Task InitDatabaseAsync() {
      if (initComplete) {
        return;
      }

      await initSemaphore.WaitAsync();
      try {
        await docClient.CreateDatabaseIfNotExistsAsync(database);
        await docClient.CreateDocumentCollectionIfNotExistsAsync(Collections.Database, userFilesCollection);

        initComplete = true;
      } finally {
        initSemaphore.Release();
      }
    }
 
    public async Task<HostedFile> CreateFileAsync(string userId, string fileName, string contentType, 
                                                  Stream fileStream, string title, string description) {
      if (string.IsNullOrEmpty(userId)) {
        throw new ArgumentNullException(nameof(userId));
      }

      if (string.IsNullOrEmpty(title)) {
        throw new ArgumentNullException(nameof(title));
      }

      if (fileStream == null) {
        throw new ArgumentNullException(nameof(fileStream));
      }

      await InitDatabaseAsync();

      if (string.IsNullOrEmpty(contentType)) {
        contentType = "application/octet-stream";
      }

      var storeResult = await fileStorage.StoreFileAsync(fileStream, fileName);
      var generatedFileId = storeResult.GeneratedSlug;

      var hostedFile = new HostedFile(userId, title, description, contentType, generatedFileId);
      await docClient.CreateDocumentAsync(Collections.UserFiles, hostedFile);

      return hostedFile;
    }

    public async Task<HostedFile> GetHostedFileAsync(Guid fileId) {
      await InitDatabaseAsync();

      var fileUrl = UriFactory.CreateDocumentUri(azureConfig.CosmosDBId, Collections.Ids.UserFiles, fileId.ToString());

      HostedFile file;
      try {
        file = await docClient.ReadDocumentAsync<HostedFile>(fileUrl);
      } catch (DocumentClientException ex) {
        if (ex.StatusCode == HttpStatusCode.NotFound) {
          throw new ResourceNotFoundException(ex);
        }

        throw;
      }

      return file;
    }

    public async Task<IEnumerable<HostedFile>> GetUserHostedFilesAsync(string userId) {
      await InitDatabaseAsync();

      var query = docClient
        .CreateDocumentQuery<HostedFile>(Collections.UserFiles)
        .Where(hF => hF.OwnerId == userId)
        .AsDocumentQuery();

      var batches = new List<IEnumerable<HostedFile>>();
      do {
        var batch = await query.ExecuteNextAsync<HostedFile>();
        batches.Add(batch);
      } while (query.HasMoreResults);

      var files = batches.SelectMany(batch => batch).ToList().AsEnumerable();
      return files;
    }

    public Task<string> ResolveFileUrlAsync(HostedFile hostedFile) {
      return fileStorage.GetFileUrlAsync(hostedFile.FileSlug);
    }
  }
}

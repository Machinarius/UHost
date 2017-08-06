using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using UHost.Models;
using UHost.Services.LocalServices;
using UHost.Services.Exceptions;

namespace UHost.Services.LocalServices {
  public class LiteDBHostedFilesService : IHostedFilesService {
    private LiteDatabase database;
    private IFileStorageService fileStorage;

    public LiteDBHostedFilesService(LiteDatabase database, IFileStorageService fileStorage) {
      if (database == null) {
        throw new ArgumentNullException(nameof(database));
      }

      if (fileStorage == null) {
        throw new ArgumentNullException(nameof(fileStorage));
      }

      this.database = database;
      this.fileStorage = fileStorage;
    }

    public Task<IEnumerable<HostedFile>> GetUserHostedFilesAsync(string userId) {
      var userFilesCollection = database.GetCollection<HostedFile>(DatabaseCollections.UserFiles);
      var userFiles = userFilesCollection.Find(hF => hF.OwnerId == userId).ToArray().AsEnumerable();
      return Task.FromResult(userFiles);
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

      if (string.IsNullOrEmpty(contentType)) {
        contentType = "application/octet-stream";
      }

      var storeResult = await fileStorage.StoreFileAsync(fileStream, fileName);
      var generatedFileId = storeResult.GeneratedSlug;

      var hostedFile = new HostedFile(userId, title, description, contentType, generatedFileId);
      var userFilesCollection = database.GetCollection<HostedFile>(DatabaseCollections.UserFiles);
      userFilesCollection.Insert(hostedFile);

      return hostedFile;
    }

    public Task<HostedFile> GetHostedFileAsync(Guid fileId) {
      var userFilesCollection = database.GetCollection<HostedFile>(DatabaseCollections.UserFiles);
      var hostedFile = userFilesCollection.FindOne(hF => hF.Id == fileId);
      if (hostedFile == null) {
        throw new ResourceNotFoundException();
      }

      return Task.FromResult(hostedFile);
    }

    public Task<string> ResolveFileUrlAsync(HostedFile hostedFile) {
      return fileStorage.GetFileUrlAsync(hostedFile.FileSlug);
    }
  }
}

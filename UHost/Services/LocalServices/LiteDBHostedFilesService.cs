using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiteDB;
using UHost.Models;
using UHost.Services.Database;

namespace UHost.Services.LocalServices {
  public class LiteDBHostedFilesService : IHostedFilesService {
    private LiteDatabase database;

    public LiteDBHostedFilesService(LiteDatabase database) {
      if (database == null) {
        throw new ArgumentNullException(nameof(database));
      }

      this.database = database;
    }

    public Task<IEnumerable<HostedFile>> GetUserHostedFilesAsync(string userId) {
      var userFilesCollection = database.GetCollection<HostedFile>(DatabaseCollections.UserFiles);
      var userFiles = userFilesCollection.Find(hF => hF.OwnerId == userId);
      return Task.FromResult(userFiles);
    }
  }
}

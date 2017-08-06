using System;
using Microsoft.Azure.Documents.Client;

namespace UHost.Services.RemoteServices.Configuration {
  public static class Collections {
    public static class Ids {
      public const string UserFiles = "user_files";
    }

    public static Uri Database { get; private set; }
    public static Uri UserFiles { get; private set; }

    public static void InitCollections(string databaseId) {
      if (string.IsNullOrEmpty(databaseId)) {
        throw new ArgumentNullException(nameof(databaseId));
      }

      Database = UriFactory.CreateDatabaseUri(databaseId);
      UserFiles = UriFactory.CreateDocumentCollectionUri(databaseId, Ids.UserFiles);
    }
  }
}

using Microsoft.Extensions.DependencyInjection;

namespace UHost.Services.RemoteServices.Configuration {
  public static class AzureServicesServiceCollectionExtensions {
    public static void UseAzureServices(this IServiceCollection services) {
      services.AddSingleton<IHostedFilesService, CosmosDBHostedFilesService>();
      services.AddSingleton<IFileStorageService, BlobStorageFileStorageService>();
    }
  }
}

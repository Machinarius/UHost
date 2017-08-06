namespace UHost.Services.RemoteServices.Configuration {
  public class AzureServicesConfiguration {
    public string BlobStorageContainer { get; set; }
    public string BlobStorageConnectionString { get; set; }
    public string CosmosDBEndpointUrl { get; set; }
    public string CosmosDBKey { get; set; }
    public string CosmosDBId { get; set; }
  }
}

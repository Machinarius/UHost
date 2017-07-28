using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UHost.UITests.B2CUserManagement {
  public class B2CUser {
    public string EmailAddress { get; }
    public string Password { get; }
    public string Name { get; }
    public string DisplayName { get; }

    private string adUserId;

    private AuthenticationContext authContext;
    private ClientCredential credentials;

    private HttpClient httpClient;

    public B2CUser() : this("testuser@test.com", "testpassword", "Test User", "testUser") {
      InitClient();
    }

    public B2CUser(string emailAddress, string password, string name, string displayName) {
      if (string.IsNullOrEmpty(emailAddress)) {
        throw new ArgumentNullException(nameof(emailAddress));
      }

      if (string.IsNullOrEmpty(password)) {
        throw new ArgumentNullException(nameof(password));
      }

      if (string.IsNullOrEmpty(name)) {
        throw new ArgumentNullException(nameof(name));
      }

      if (string.IsNullOrEmpty(displayName)) {
        throw new ArgumentNullException(nameof(displayName));
      }

      EmailAddress = emailAddress;
      Password = password;
      Name = name;
      DisplayName = displayName;

      InitClient();
    }

    private void InitClient() {
      authContext = new AuthenticationContext("https://login.microsoftonline.com/" + ADConstants.TenantId);
      credentials = new ClientCredential(ADConstants.ClientId, ADConstants.ClientSecret);
      httpClient = new HttpClient();
    }

    public async Task RegisterAsync() {
      var userPayloadObject = new {
        accountEnabled = true,
        creationType = "LocalAccount",
        displayName = Name,
        passwordProfile = new {
          password = Password,
          forceChangePasswordNextLogin = false
        },
        signInNames = new [] {
          new {
            type = "emailAddress",
            value = EmailAddress
          }
        }
      };
      var userRequestPayload = JsonConvert.SerializeObject(userPayloadObject);

      var adToken = await authContext.AcquireTokenAsync(ADConstants.GraphResourceId, credentials);
      var urlAddress = ADConstants.GraphResourceId + ADConstants.TenantId + "/users?" + ADConstants.GraphAPIVersion;

      var request = new HttpRequestMessage(HttpMethod.Post, urlAddress);
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adToken.AccessToken);
      request.Content = new StringContent(userRequestPayload, Encoding.UTF8, "application/json");

      var response = await httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();

      var responseContent = await response.Content.ReadAsStringAsync();
      var responseObject = JsonConvert.DeserializeObject<JObject>(responseContent);
      adUserId = responseObject["objectId"].ToString();
    }

    public async Task CleanupAsync() {
      var adToken = await authContext.AcquireTokenAsync(ADConstants.GraphResourceId, credentials);
      var urlAddress = ADConstants.GraphResourceId + ADConstants.TenantId + "/users/" + adUserId + "?" + ADConstants.GraphAPIVersion;

      var request = new HttpRequestMessage(HttpMethod.Delete, urlAddress);
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adToken.AccessToken);

      var response = await httpClient.SendAsync(request);
      response.EnsureSuccessStatusCode();
    }
  }
}

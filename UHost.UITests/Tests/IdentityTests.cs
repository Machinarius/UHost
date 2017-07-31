using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using UHost.UITests.B2CUserManagement;
using UHost.UITests.Pages;
using UHost.UITests.Support;
using Xunit;

namespace UHost.UITests {
  public class IdentityTests : IDisposable {
    B2CUser testUser;
    AppHandle appHandle;
    IWebDriver driver;

    [Fact]
    public async Task SigningInShouldIdentifyMeAsAUserInThePlatformAsync() {
      testUser = new B2CUser("carlos@test.com", "carlosIsGreat123", "Carlos Quintero");
      await testUser.RegisterAsync();
      
      appHandle = AppSource.GenerateWebDriver();
      driver = appHandle.WebDriver;

      var loginPage = new LoginPage(driver);
      loginPage.Login(testUser);
    }
    
    public void Dispose() {
      testUser?.CleanupAsync().Wait();
      appHandle?.Dispose();
    }
  }
}

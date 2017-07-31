using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using UHost.UITests.B2CUserManagement;
using UHost.UITests.Pages;
using UHost.UITests.Support;
using UHost.UITests.Utility;
using Xunit;

namespace UHost.UITests {
  public class IdentityTests : IDisposable {
    B2CUser testUser;
    AppHandle appHandle;
    IWebDriver webDriver;

    [Fact]
    public async Task SigningInShouldIdentifyMeAsAUserInThePlatformAsync() {
      testUser = new B2CUser("carlos@test.com", "carlosIsGreat123", "Carlos Quintero");
      await testUser.RegisterAsync();
      
      appHandle = AppSource.GenerateWebDriver();
      webDriver = appHandle.WebDriver;

      var loginPage = new LoginPage(webDriver);
      loginPage.Login(testUser);
    }
    
    public void Dispose() {
      webDriver.DumpScreenshot();

      testUser?.CleanupAsync().Wait();
      appHandle?.Dispose();
    }
  }
}

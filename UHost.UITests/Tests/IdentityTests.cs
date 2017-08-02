using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using UHost.UITests.B2CUserManagement;
using UHost.UITests.Pages;
using UHost.UITests.Support;
using UHost.UITests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace UHost.UITests {
  public class IdentityTests : IDisposable {
    private B2CUser testUser;
    private AppHandle appHandle;
    private IWebDriver webDriver;
    private ITestOutputHelper output;

    public IdentityTests(ITestOutputHelper output) {
      this.output = output;

      appHandle = AppSource.GenerateWebDriver();
      webDriver = appHandle.WebDriver;
    }

    [Fact]
    public async Task SigningInShouldIdentifyMeAsAUserInThePlatformAsync() {
      testUser = new B2CUser("carlos@test.com", "carlosIsGreat123", "Carlos Quintero");
      await testUser.RegisterAsync();
      
      var loginPage = new LoginPage(webDriver);
      loginPage.Login(testUser);
    }
    
    public void Dispose() {
      webDriver.DumpScreenshot(output);

      testUser?.CleanupAsync().Wait();
      appHandle?.Dispose();
    }
  }
}

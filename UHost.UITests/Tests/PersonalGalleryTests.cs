using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using UHost.UITests.B2CUserManagement;
using UHost.UITests.Pages;
using UHost.UITests.Support;
using UHost.UITests.Utility;
using Xunit;
using Xunit.Abstractions;

namespace UHost.UITests.Tests {
  public class PersonalGalleryTests : IDisposable {
    private AppHandle appHandle;
    private IWebDriver webDriver;
    private B2CUser testUser;
    private ITestOutputHelper output;

    public PersonalGalleryTests(ITestOutputHelper output) {
      this.output = output;

      appHandle = AppSource.GenerateWebDriver();
      webDriver = appHandle.WebDriver;
    }

    [Fact]
    public void ThePersonalGalleryRequiresTheUserToBeLoggedIn() {
      webDriver.FindElement(By.LinkText("My files")).Click();
      webDriver.FindElementByText("You can use UHost to host and share all your files");
      webDriver.FindElementByText("Please Sign in to continue");
    }

    [Fact]
    public async Task TheGalleryForANewUserMustShowAnEmptyMessage() {
      testUser = new B2CUser("carlos@test.com", "carlosIsGreat123", "Carlos Quintero");
      await testUser.RegisterAsync();

      var loginPage = new LoginPage(webDriver);
      loginPage.Login(testUser);

      webDriver.FindElement(By.LinkText("My files")).Click();
      webDriver.FindElementByText("You have no files right now");
      webDriver.FindElementByText("Get started with the upload button");
    } 

    public void Dispose() {
      webDriver.DumpScreenshot(output);

      testUser?.CleanupAsync().Wait();
      appHandle?.Dispose();
    }
  }
}

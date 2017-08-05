using System;
using System.IO;
using System.Threading.Tasks;
using NFluent;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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

    [Fact]
    public async Task UploadingAPhotoMustShowItInTheGallery() {
      testUser = new B2CUser("carlos@test.com", "carlosIsGreat123", "Carlos Quintero");
      await testUser.RegisterAsync();

      var loginPage = new LoginPage(webDriver);
      loginPage.Login(testUser);

      var sourceFile = Path.Combine("Assets", "EafitLogo.jpg");
      var filePath = Path.GetFullPath(sourceFile);

      webDriver.FindElement(By.LinkText("My files")).Click();
      webDriver.FindElement(By.Id("Upload File")).Click();

      webDriver.FindElement(By.Id("upload-modal"));
      webDriver.FindElement(By.Id("title-input")).SendKeys("Universidad EAFIT");
      webDriver.FindElement(By.Id("description-input")).SendKeys("Abierta al mundo");
      webDriver.FindElement(By.Id("file-picker")).SendKeys(filePath);
      webDriver.FindElementByText("Upload File").Click();
      webDriver.FindElement(By.Id("upload-progress"));

      var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(5));
      wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("upload-modal")));

      Check.That(webDriver.Title).IsEqualTo("Universidad EAFIT - UHost");
      webDriver.FindElementByText("Universidad EAFIT");
      webDriver.FindElementByText("Abierta al mundo");
      
      webDriver.FindElement(By.LinkText("My files")).Click();

      wait.Until(webDriver => webDriver.Title == "My files - UHost");
      var emptyMessageExists =
        !ExpectedConditions.InvisibilityOfElementLocated(By.LinkText("You have no files right now")).Invoke(webDriver) ||
        !ExpectedConditions.InvisibilityOfElementLocated(By.LinkText("Get started with the upload button")).Invoke(webDriver);
      Check.That(emptyMessageExists).IsFalse();

      webDriver.FindElementByText("Universidad EAFIT");
      webDriver.FindElementByText("Abierta al mundo");
    }

    public void Dispose() {
      webDriver.DumpScreenshot(output);

      testUser?.CleanupAsync().Wait();
      appHandle?.Dispose();
    }
  }
}

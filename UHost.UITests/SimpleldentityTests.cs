using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UHost.UITests.B2CUserManagement;
using UHost.UITests.Support;
using Xunit;

namespace UHost.UITests {
  public class SimpleldentityTests : IDisposable {
    B2CUser testUser;
    AppHandle appHandle;
    IWebDriver driver;

    [Fact]
    public async Task SigningInShouldIdentifyMeAsAUserInThePlatformAsync() {
      testUser = new B2CUser("carlos@test.com", "carlosIsGreat123", "Carlos Quintero");
      await testUser.RegisterAsync();
      
      appHandle = AppSource.GenerateWebDriver();
      driver = appHandle.WebDriver;

      var signInLink = driver.FindElement(By.LinkText("Sign in"));
      signInLink.Click();

      var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

      wait.Until(driver => driver.FindElement(By.CssSelector("input#logonIdentifier")));
      var emailInput = driver.FindElement(By.CssSelector("input#logonIdentifier"));
      var passwordInput = driver.FindElement(By.CssSelector("input#password"));
      var submitButton = driver.FindElement(By.CssSelector("button#next"));
      
      emailInput.SendKeys(testUser.EmailAddress);
      passwordInput.SendKeys(testUser.Password);
      submitButton.Click();
      
      wait.Until(driver => driver.FindElement(By.LinkText("Hello " + testUser.Name + "!")));
    }
    
    public void Dispose() {
      testUser?.CleanupAsync().Wait();
      appHandle?.Dispose();
    }
  }
}

using System;
using System.Threading.Tasks;
using NFluent;
using OpenQA.Selenium;
using UHost.UITests.B2CUserManagement;
using UHost.UITests.Support;
using Xunit;

namespace UHost.UITests {
  public class SimpleldentityTests : IDisposable {
    B2CUser testUser;
    IWebDriver driver;

    [Fact]
    public async Task SigningInShouldIdentifyMeAsAUserInThePlatformAsync() {
      testUser = new B2CUser("carlos@test.com", "carlosIsGreat123", "Carlos Quintero", "cQuintero");
      await testUser.RegisterAsync();
      
      driver = AppDriverSource.GenerateWebDriver();

      var signInLink = driver.FindElement(By.LinkText("Sign in"));
      signInLink.Click();

      await Task.Delay(TimeSpan.FromSeconds(5));

      var emailInput = driver.FindElement(By.CssSelector("input#logonIdentifier"));
      var passwordInput = driver.FindElement(By.CssSelector("input#password"));
      var submitButton = driver.FindElement(By.CssSelector("button#next"));

      await Task.Delay(TimeSpan.FromSeconds(2));

      emailInput.SendKeys(testUser.EmailAddress);
      passwordInput.SendKeys(testUser.Password);
      submitButton.Click();

      await Task.Delay(TimeSpan.FromSeconds(2));

      driver.FindElement(By.LinkText("Hello " + testUser.Name + "!"));
    }
    
    public void Dispose() {
      testUser?.CleanupAsync().Wait();
      driver?.Dispose();
    }
  }
}

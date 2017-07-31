using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UHost.UITests.B2CUserManagement;

namespace UHost.UITests.Pages {
  public class LoginPage {
    private IWebDriver webDriver;
    private WebDriverWait wait;

    public LoginPage(IWebDriver webDriver) {
      if (webDriver == null) {
        throw new ArgumentNullException(nameof(webDriver));
      }

      this.webDriver = webDriver;

      var signInLink = webDriver.FindElement(By.LinkText("Sign in"));
      signInLink.Click();

      wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(5));
      wait.Until(driver => driver.FindElement(By.CssSelector("input#logonIdentifier")));
    }

    public void Login(B2CUser user) {
      if (user == null) {
        throw new ArgumentNullException(nameof(user));
      }

      var emailInput = webDriver.FindElement(By.CssSelector("input#logonIdentifier"));
      var passwordInput = webDriver.FindElement(By.CssSelector("input#password"));
      var submitButton = webDriver.FindElement(By.CssSelector("button#next"));

      emailInput.SendKeys(user.EmailAddress);
      passwordInput.SendKeys(user.Password);
      submitButton.Click();

      wait.Until(driver => driver.FindElement(By.LinkText("Hello " + user.Name + "!")));
    }
  }
}

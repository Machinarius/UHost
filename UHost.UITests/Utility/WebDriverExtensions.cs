using System;
using OpenQA.Selenium;

namespace UHost.UITests.Utility {
  public static class WebDriverExtensions {
    public static IWebElement FindElementByText(this IWebDriver webDriver, string elementText) {
      if (webDriver == null) {
        throw new ArgumentNullException(nameof(webDriver));
      }
      
      return webDriver.FindElement(By.XPath($"//*[contains(text(), '{elementText}')]"));
    }
  }
}

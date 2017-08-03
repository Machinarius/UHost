using System;
using System.IO;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace UHost.UITests.Utility {
  public static class WebDriverExtensions {
    public static IWebElement FindElementByText(this IWebDriver webDriver, string elementText) {
      if (webDriver == null) {
        throw new ArgumentNullException(nameof(webDriver));
      }
      
      return webDriver.FindElement(By.XPath($"//*[contains(text(), '{elementText}')]"));
    }

    public static void DumpScreenshot(this IWebDriver webDriver, ITestOutputHelper output) {
      if (webDriver == null) {
        throw new ArgumentNullException(nameof(webDriver));
      }

      var screenshotsFolder = "Screenshots";
      var agentWorkFolder = Environment.GetEnvironmentVariable("AGENT_WORKFOLDER");
      if (!string.IsNullOrEmpty(agentWorkFolder)) {
        screenshotsFolder = Path.Combine(agentWorkFolder, screenshotsFolder);
      }

      if (!Directory.Exists(screenshotsFolder)) {
        Directory.CreateDirectory(screenshotsFolder);
      }

      var takesScreen = (ITakesScreenshot)webDriver;
      var screenshot = takesScreen.GetScreenshot();
      var screenshotPath = Path.Combine(screenshotsFolder, 
        Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".png");

      try {
        screenshot.SaveAsFile(screenshotPath, ScreenshotImageFormat.Png);
        output.WriteLine("Saved final screenshot: " + screenshotPath);
      } catch (Exception) {
        output.WriteLine("Failed to saved final screenshot: " + screenshotPath);
      }
    }
  }
}

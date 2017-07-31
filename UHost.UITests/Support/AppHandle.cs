using System;
using System.Diagnostics;
using OpenQA.Selenium;

namespace UHost.UITests.Support {
  public class AppHandle : IDisposable {
    public IWebDriver WebDriver { get; }

    private Process appProcess;

    internal AppHandle(IWebDriver webDriver, Process appProcess) {
      if (webDriver == null) {
        throw new ArgumentNullException(nameof(webDriver));
      }

      if (appProcess == null) {
        throw new ArgumentNullException(nameof(appProcess));
      }

      this.appProcess = appProcess;
      WebDriver = webDriver;
    }

    public void Dispose() {
      WebDriver.Dispose();

      try {
        appProcess.Kill();
      } catch (InvalidOperationException) {
        Debug.WriteLine("Ignoring an InvalidOperationException");
      }
    }
  }
}

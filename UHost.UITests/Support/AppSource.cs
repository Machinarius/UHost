using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.PhantomJS;

namespace UHost.UITests.Support {
  public static class AppSource {
    public const string AppUrl = "http://localhost:5000/";

    public static AppHandle GenerateWebDriver() {
      IWebDriver webDriver;

      var inCIServer = !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD"));
      if (inCIServer) {
        webDriver = new PhantomJSDriver();
      } else {
        webDriver = new ChromeDriver();
      }

      var appLocation = typeof(AppSource).Assembly.Location;
      appLocation = Path.Combine(appLocation, "..", "..", "..", "..", "..", "UHost");
      appLocation = Path.GetFullPath(appLocation);

      var appPSI = new ProcessStartInfo("dotnet", "run") {
        WorkingDirectory = appLocation,
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        RedirectStandardOutput = true
      };
      appPSI.EnvironmentVariables.Add("ASPNETCORE_URLS", AppUrl);
      appPSI.EnvironmentVariables.Add("ASPNETCORE_ENVIRONMENT", "Testing");

      var appProcess = Process.Start(appPSI);
      Thread.Sleep(TimeSpan.FromSeconds(2));

      webDriver.Navigate().GoToUrl(AppUrl);

      var handle = new AppHandle(webDriver, appProcess);
      return handle;
    }
  }
}

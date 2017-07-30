using System.Diagnostics;
using System.IO;
using OpenQA.Selenium.Chrome;

namespace UHost.UITests.Support {
  public static class AppSource {
    public const string AppUrl = "http://localhost:5000/";

    public static AppHandle GenerateWebDriver() {
      // TODO: Launch the app here
      // ASPNETCORE_URLS="http://localhost:5000/" ASPNETCORE_ENVIRONMENT="Testing" dotnet run

      var appLocation = typeof(AppSource).Assembly.Location;
      appLocation = Path.Combine(appLocation, "..", "..", "..", "..", "..", "UHost");
      appLocation = Path.GetFullPath(appLocation);

      var appPSI = new ProcessStartInfo("dotnet", "run") {
        WorkingDirectory = appLocation,
        UseShellExecute = false,
        RedirectStandardInput = true,
        WindowStyle = ProcessWindowStyle.Normal
      };
      appPSI.EnvironmentVariables.Add("ASPNETCORE_URLS", AppUrl);
      appPSI.EnvironmentVariables.Add("ASPNETCORE_ENVIRONMENT", "Development");

      var appProcess = Process.Start(appPSI);
      var chromeDriver = new ChromeDriver();
      chromeDriver.Navigate().GoToUrl(AppUrl);

      var handle = new AppHandle(chromeDriver, appProcess);
      return handle;
    }
  }
}

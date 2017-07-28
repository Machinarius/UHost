using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace UHost.UITests.Support {
  public static class AppDriverSource {
    public static IWebDriver GenerateWebDriver() {
      // TODO: Launch the app here
      // ASPNETCORE_URLS="http://localhost:5000/" ASPNETCORE_ENVIRONMENT="Testing" dotnet run

      var chromeDriver = new ChromeDriver();
      chromeDriver.Navigate().GoToUrl(AppConfiguration.AppLocation);

      return chromeDriver;
    }
  }
}

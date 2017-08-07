using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UHost.Authentication;
using UHost.Services.LocalServices.Configuration;
using UHost.Services.RemoteServices.Configuration;

namespace UHost {
  public class Startup {
    public Startup(IHostingEnvironment env) {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

      if (env.IsDevelopment()) {
        // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
        builder.AddUserSecrets<Startup>();
      }

      builder.AddEnvironmentVariables();

      Configuration = builder.Build();
      HostingEnvironment = env;
    }

    private IConfigurationRoot Configuration { get; }
    private IHostingEnvironment HostingEnvironment { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.Configure<AzureADB2COptions>(Configuration.GetSection("Authentication:AzureADB2C"));
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      services.AddMvc();

      services.AddDistributedMemoryCache();
      services.AddSession(options => {
        options.IdleTimeout = TimeSpan.FromHours(1);
        options.CookieHttpOnly = true;
      });

      services.AddAuthentication(
          sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

      services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsSetup>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

      if (HostingEnvironment.IsDevelopment() || 
          HostingEnvironment.IsEnvironment("Testing")) {
        services.UseLocalServices();
      } else {
        services.Configure<AzureServicesConfiguration>(Configuration.GetSection("AzureServices"));
        services.UseAzureServices();
      }

      var sslCertFile = Environment.GetEnvironmentVariable("SSL_CERT_FILE");
      var sslCertPassword = Environment.GetEnvironmentVariable("SSL_CERT_PASSWORD");
      if (!string.IsNullOrEmpty(sslCertFile) && !string.IsNullOrEmpty(sslCertPassword)) {
        services.Configure<KestrelServerOptions>(options => {
          options.UseHttps(sslCertFile, sslCertPassword);
        });
      }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
      } else {
        app.UseExceptionHandler("/");
      }

      app.UseStaticFiles();

      app.UseSession();

      app.UseCookieAuthentication();

      app.UseOpenIdConnectAuthentication();

      app.UseMvc(routes => {
        routes.MapRoute(
            name: "default",
            template: "{controller=Files}/{action=Mine}/{id?}");
      });
    }
  }
}

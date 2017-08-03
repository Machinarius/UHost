﻿using System;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;

namespace UHost.Services.LocalServices.Configuration {
  public static class LocalServicesServiceCollectionExtensions {
    private const string DatabaseFilename = @"Data\UHost.db";

    public static void UseLocalServices(this IServiceCollection services) {
      if (services == null) {
        throw new ArgumentNullException(nameof(services));
      }

      var database = new LiteDatabase(DatabaseFilename);
      services.AddSingleton(database);
      services.AddTransient<IHostedFilesService, LiteDBHostedFilesService>();
    }
  }
}

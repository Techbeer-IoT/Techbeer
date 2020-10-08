using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FunctionStoreData {
    public class Startup : FunctionsStartup {
        public override void Configure(IFunctionsHostBuilder builder) {
            //var settings = InitSettings();

            //builder.Services.AddSingleton((s) => settings);

            //builder.Services.AddSingleton<Core.AppSettings>(settings);
            //builder.Services.AddScoped<Core.Adapters.DataCreatorAdapter, Storage.DataCreator>()
        }

        //private Core.AppSettings InitSettings() {
        //    var settings = new Core.AppSettings();

        //    settings.Storage = new Core.AppSettings.StorageSection() {
        //        StorageTableName = this.GetSettings("StorageTableName"),
        //        ConnectionString = GetSettings("StorageConnectionString")
        //    };

        //    return settings;
        //}

        //private string GetSettings(string name) =>
        //    System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }
}

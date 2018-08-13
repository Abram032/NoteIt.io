using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NoteIt.Infrastructure.Identity;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Azure.Services.AppAuthentication;

//TODO: Ability to log in through external provider.

namespace NoteIt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            ApplicationIdentityDbContextSeed.SeedAsync(host.Services).Wait();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    var config = configBuilder.Build();
                    if (config["ASPNETCORE_ENVIRONMENT"].Equals("Production"))
                    {
                        var endpoint = config["AzureKeyVaultEndpoint"];
                        var client = config["AzureKeyVaultClientID"];
                        var secret = config["AzureKeyVaultSecret"];

                        configBuilder.AddAzureKeyVault(endpoint, client, secret);
                    }
                })
                .UseApplicationInsights()
                .UseStartup<Startup>();
    }
}

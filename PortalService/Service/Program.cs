using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
                    WebHost.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration((context, config) =>
                        {
                            var builtConfig = config.Build();
                    //AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                    //var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    //config.AddAzureKeyVault($"https://{builtConfig["Vault"]}.vault.azure.net/", keyVaultClient, new DefaultKeyVaultSecretManager());
                    config.AddAzureKeyVault($"https://{builtConfig["Vault"]}.vault.azure.net/", builtConfig["ClientId"], builtConfig["ClientSecret"]);

                        })
                        .UseStartup<Startup>()
                        .Build();
    }
}

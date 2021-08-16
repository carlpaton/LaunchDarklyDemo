using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using LaunchDarkly.Sdk.Server.Interfaces;
using LaunchDarkly.Sdk.Server;

namespace FeatureClient
{
    internal static class ContainerConfiguration
    {
        public static IServiceProvider Configure()
        {
            var services = new ServiceCollection();

            var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            var sdkKey = configuration["LaunchDarkly:SdkKey"];
            var launchDarklyClient = new LdClient(sdkKey);
            services.AddSingleton<ILdClient>(launchDarklyClient);

            return services.BuildServiceProvider();
        }
    }
}

using System;
using System.IO;
using LoggerDemo;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;

[assembly: FunctionsStartup(typeof(Startup))]
namespace LoggerDemo
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // InitWithLogger(builder);
            InitConfig(builder);
            InitDependencies(builder);
        }

        private static void InitWithLogger(IFunctionsHostBuilder builder)
        {
            var executionContextOptions = builder.Services.BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>().Value;
            var currentDirectory = executionContextOptions.AppDirectory;
            
            LogManager.Setup()
                .SetupExtensions(e => e.AutoLoadAssemblies(false))
                .LoadConfigurationFromFile(currentDirectory + Path.DirectorySeparatorChar + "nlog.config", optional: false)
                .LoadConfiguration(configurationBuilder => configurationBuilder.LogFactory.AutoShutdown = false);
        }

        private static void InitDependencies(IFunctionsHostBuilder builder)
        {
            builder.Services
                .AddHttpClient()
                .AddLogging()
                //     (loggingBuilder) =>
                // {
                //     loggingBuilder.AddNLog(new NLogProviderOptions() { ShutdownOnDispose = true });
                // })
                .AddMvc();
        }

        private static void InitConfig(IFunctionsHostBuilder builder)
        {
            var appEnv = Environment.GetEnvironmentVariable("APP_ENV");

            var isLocal = appEnv == null || appEnv.Equals("local", StringComparison.OrdinalIgnoreCase);

            var appDirectory = isLocal
                ? Environment.CurrentDirectory
                : $"{Environment.GetEnvironmentVariable("SITE_FOLDER")}/site/wwwroot";

            var config = new ConfigurationBuilder()
                .SetBasePath($"{appDirectory}/ConfigFiles")
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .Build();
            
            builder.Services.AddOptions();
        }
    }
}
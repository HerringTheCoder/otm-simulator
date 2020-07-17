using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using otm_simulator.Hubs;
using otm_simulator.Interfaces;
using otm_simulator.Services;
using otm_simulator.Tests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace otm_simulator_tests
{
    [TestClass]
    public class StatesGeneratorTest
    {
        [TestMethod]
        public async Task StateGenerator_Executes_Properly()
        {

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole()
                    .AddEventLog();
            });
            ILogger workerLogger = loggerFactory.CreateLogger<BackgroundWorker>();
            ILogger stateLogger = loggerFactory.CreateLogger<StateGeneratorService>();
            //Add dependencies
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IHostedService, BackgroundWorker>();
            services.AddSingleton<ITimetableProvider, TimetableProviderMock>();
            services.AddOptions();         
            services.AddSingleton(loggerFactory);
            services.AddSingleton(Mock.Of<IStateGenerator>());
            var serviceProvider = services.AddLogging().BuildServiceProvider();


            var stateGeneratorService = serviceProvider.GetRequiredService<IStateGenerator>();

            var mock = Mock.Get(stateGeneratorService);

            mock.Verify(_ => _.CreateStates(), Times.AtLeastOnce);
        }
    }
}

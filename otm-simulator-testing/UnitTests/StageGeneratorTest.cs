
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using otm_simulator.Helpers;
using otm_simulator.Interfaces;
using otm_simulator.Models;
using otm_simulator.Services;
using otm_simulator.Tests;
using otm_simulator_testing.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace otm_simulator_testing
{
    public class StateGeneratorTest
    {
        private IStateGenerator _stateGenerator;

        [Fact]
        public void TestIfStatesAreCreatedProperly()
        {
            InitializeService();
            _stateGenerator.CreateStates();
            IEnumerable<BusState> states = _stateGenerator.GetStates();
            Assert.NotNull(states);
            Assert.True(states.Count() == 10, "Actual count is " + states.Count());
        }

        [Fact]
        public void TestIfStatesAreReleasedProperly()
        {
            InitializeService(true);
            _stateGenerator.CreateStates();
            _stateGenerator.ReleaseStates();
            IEnumerable<BusState> states = _stateGenerator.GetStates();
            Assert.NotNull(states);
            Assert.True(states.Count() == 0, "Actual count is " + states.Count());
        }

        private void InitializeService(bool timeHasPassed = false)
        {
            ITimetableProvider timetableProviderMock = new TimetableProviderMock();
            timetableProviderMock.FetchAsync();
            var appSettings = Options.Create(new AppSettings());
            appSettings.Value.UpdateIntervalInSeconds = 1;
            var loggerMock = new Mock<ILogger<StateGeneratorService>>();
            ITimeProvider timeProvider = new TimeProviderMock
            {
                Now = DateTime.Now
            };
            if (timeHasPassed == true)
            {
                timeProvider.Now = DateTime.Now.AddMinutes(5);
            }

            _stateGenerator = new StateGeneratorService(
            timetableProviderMock,
            appSettings,
            loggerMock.Object,
            timeProvider
            );
            _stateGenerator.SyncDataWithProvider();
        }
    }
}

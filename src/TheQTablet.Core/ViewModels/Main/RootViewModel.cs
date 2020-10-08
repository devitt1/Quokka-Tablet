using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MvvmCross.Logging;
using MvvmCross;
using MvvmCross.Base;

using TheQTablet.Core.Service.Interfaces;
using MvvmCross.Commands;

namespace TheQTablet.Core.ViewModels.Main
{
    public class RootViewModel : BaseViewModel
    {
        private readonly IMvxLog _log;
        private readonly ISimulatorService _simulationService;

        public RootViewModel(IMvxLog log, ISimulatorService simulationService)
        {
            _log = log;

            _simulationService = simulationService;

            _log.Trace("RootViewModel:RootViewModel()");

            StartSimulationCommand = new MvxAsyncCommand(StartSimulationAsync);
        }

        private async Task StartSimulationAsync()
        {
            await _simulationService.Run();
            _log.Trace(" PolarisationSimulatorService: awaited");
        }

        public MvxAsyncCommand StartSimulationCommand { get; private set; }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

        }
    
    }
}

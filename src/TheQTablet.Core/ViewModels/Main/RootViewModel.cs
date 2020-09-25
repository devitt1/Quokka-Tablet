using System;
using System.Collections.Generic;
using System.Text;

using MvvmCross.Logging;

using TheQTablet.Core.Service.Interfaces;

namespace TheQTablet.Core.ViewModels.Main
{
    public class RootViewModel : BaseViewModel
    {
        private readonly IMvxLog _log;

        public RootViewModel(IMvxLog log, ISimulatorService polarisationSimulatorService)
        {
            _log = log;
            _log.Trace("RootViewModel:RootViewModel()");
        }
        /*
        readonly IPolarisationSimulatorService _polarisationSimulatorService;

        public RootViewModel(IPolarisationSimulatorService polarisationSimulatorService)
        {
            _polarisationSimulatorService = polarisationSimulatorService;
        }
        */
    }
}

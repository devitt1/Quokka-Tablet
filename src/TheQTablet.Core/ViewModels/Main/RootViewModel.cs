using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MvvmCross.Logging;
using MvvmCross;
using MvvmCross.Base;

using TheQTablet.Core.Service.Interfaces;

namespace TheQTablet.Core.ViewModels.Main
{
    public class RootViewModel : BaseViewModel
    {
        private readonly IMvxLog _log;
        private readonly ISimulatorService PolarisationSimulatorService;

        public RootViewModel(IMvxLog log, ISimulatorService polarisationSimulatorService)
        {
            _log = log;
            PolarisationSimulatorService = polarisationSimulatorService;
            _log.Trace("RootViewModel:RootViewModel()");

        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            /*
             * Attempt running the task using the dispatcher
             * to avoid the "iOS only allows the code to run every 10s issue
             * NOT WORKING YET
             * 
            var dispatcher = Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>();
            dispatcher.ExecuteOnMainThreadAsync(
                async () =>
                {
                    await PolarisationSimulatorService.RunManySim();
                });
            */

            Task.Run(async () => {
               await PolarisationSimulatorService.RunManySim();
            });

        }
    }
}

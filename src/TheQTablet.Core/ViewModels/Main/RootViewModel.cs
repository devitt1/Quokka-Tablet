using System.Threading.Tasks;

using MvvmCross.Logging;

using TheQTablet.Core.Service.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using TheQTablet.Core.ViewModels.Main.Lesson01;

namespace TheQTablet.Core.ViewModels.Main
{
    public class RootViewModel : BaseViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly IMvxLog _log;
        private readonly ISimulatorService _simulationService;

        public RootViewModel(
            IMvxLog log,
            ISimulatorService simulationService,
            IMvxNavigationService navigationService)
        {
            _log = log;
            _simulationService = simulationService;
            _navigationService = navigationService;

            _log.Trace("RootViewModel:RootViewModel()");

            NavigateToPolarisationExperimentCommand = new MvxAsyncCommand(NavigateToPolarisationExperimentAsync);
        }

        private async Task NavigateToPolarisationExperimentAsync()
        {
            _log.Trace(" Navigation to PolarisationSimulatorService: awaiting");
            var result = await _navigationService.Navigate<Lesson01StartViewModel>();
            _log.Trace(" Navigation to PolarisationSimulatorService: awaited");
        }

        public MvxAsyncCommand NavigateToPolarisationExperimentCommand { get; private set; }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

        }
    
    }
}

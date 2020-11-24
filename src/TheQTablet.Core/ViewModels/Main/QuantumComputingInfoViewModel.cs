using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels.Main
{
    public class QuantumComputingInfoViewModel : MvxNavigationViewModel
    {
        public QuantumComputingInfoViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            StartSimulationCommand = new MvxAsyncCommand(StartSimulationAsync);
        }

        private async Task StartSimulationAsync()
        {
            await NavigationService.Navigate<StoryContainerViewModel>();
        }

        public MvxAsyncCommand StartSimulationCommand { get; }
    }
}

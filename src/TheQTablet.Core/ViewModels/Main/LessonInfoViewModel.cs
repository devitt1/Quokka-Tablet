using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels.Main
{
    public class LessonInfoViewModel : MvxNavigationViewModel
    {
        public LessonInfoViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            NexLessonViewModel = new MvxAsyncCommand(NextLessonViewModelAsync);

        }

        private async Task NextLessonViewModelAsync()
        {
            await NavigationService.Navigate<QuantumComputingInfoViewModel>();
        }

        public MvxAsyncCommand NexLessonViewModel { get; }
    }
}

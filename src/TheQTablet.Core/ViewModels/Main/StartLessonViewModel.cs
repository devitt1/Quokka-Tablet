using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels.Main
{
    public class StartLessonViewModel : MvxNavigationViewModel
    {
        public StartLessonViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            StartLessonCommand = new MvxAsyncCommand(StartLessonAsync);
        }

        private async Task StartLessonAsync()
        {
            await NavigationService.Navigate<LessonInfoViewModel>();
        }

        public MvxAsyncCommand StartLessonCommand { get; }
    }
}

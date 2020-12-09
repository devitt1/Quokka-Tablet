using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels.Main.Lesson01
{
    public abstract class LessonBaseViewModel: MvxNavigationViewModel
    {
        public LessonBaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            ExitCommand = new MvxAsyncCommand(Exit);
        }

        private async Task Exit()
        {
            await NavigationService.Close(this);
        }

        public MvxAsyncCommand ExitCommand { get; }
    }
}

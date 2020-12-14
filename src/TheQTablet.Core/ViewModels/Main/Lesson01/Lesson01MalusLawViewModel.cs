using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels.Main.Lesson01
{
    public class Lesson01MalusLawViewModel : LessonBaseViewModel
    {
        public Lesson01MalusLawViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            CloseCommand = new MvxAsyncCommand(Close);
        }

        private async Task Close()
        {
            await NavigationService.Close(this);
        }

        public MvxAsyncCommand CloseCommand { get; }
    }
}

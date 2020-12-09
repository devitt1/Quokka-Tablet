using System;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels.Main
{
    public class Lesson01StartViewModel : MvxNavigationViewModel
    {
        public Lesson01StartViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            ContinueCommand = new MvxAsyncCommand(NextPage);
            BackCommand = new MvxAsyncCommand(Back);
        }

        private async Task NextPage()
        {
            await NavigationService.Navigate<TelescopeSearchViewModel>();
        }

        private async Task Back()
        {
            await NavigationService.Close(this);
        }

        public MvxAsyncCommand ContinueCommand { get; }
        public MvxAsyncCommand BackCommand { get; }
    }
}

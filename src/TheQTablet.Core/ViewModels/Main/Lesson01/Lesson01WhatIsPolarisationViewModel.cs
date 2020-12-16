using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace TheQTablet.Core.ViewModels.Main.Lesson01
{
    public class Lesson01WhatIsPolarisationViewModel : Lesson01BaseViewModel
    {
        public Lesson01WhatIsPolarisationViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
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

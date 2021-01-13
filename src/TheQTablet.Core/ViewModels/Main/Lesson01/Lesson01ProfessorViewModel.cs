using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace TheQTablet.Core.ViewModels.Main.Lesson01
{
    public class Lesson01ProfessorViewModel : Lesson01BaseViewModel
    {
        public Lesson01ProfessorViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            ContinueCommand = new MvxAsyncCommand(NextPage);
        }

        private async Task NextPage()
        {
            await NavigationService.Navigate<PlotViewModel>();
        }

        public MvxAsyncCommand ContinueCommand { get; }
    }
}

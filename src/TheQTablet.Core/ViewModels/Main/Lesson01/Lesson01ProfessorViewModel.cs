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
        private bool _compleate1Task = false;

        private async Task NextPage()
        {
            if (_compleate1Task)
            {
                await NavigationService.Navigate<SatellitePlotViewModel>();
            }
            else
            {
               var result = await NavigationService.Navigate<PlotViewModel>();
            }

        }
        public override void Prepare()
        {
            base.Prepare();
            _compleate1Task = false;
            NavigationService.AfterClose += NavigationService_AfterClose;
        }

        private void NavigationService_AfterClose(object sender, MvvmCross.Navigation.EventArguments.IMvxNavigateEventArgs e)
        {
            var viewModel = (e.ViewModel as PlotViewModel);
           _compleate1Task = viewModel.Progress == 1;
        }

        public MvxAsyncCommand ContinueCommand { get; }
    }
}

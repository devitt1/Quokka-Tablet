using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

namespace TheQTablet.Core.ViewModels.Main.Lesson01
{
    public class Lesson01BaseViewModel : LessonBaseViewModel
    {
        public Lesson01BaseViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            MalusLawCommand = new MvxAsyncCommand(MalusLaw);
        }

        private async Task MalusLaw()
        {
            await NavigationService.Navigate<Lesson01MalusLawViewModel>();
        }

        public MvxAsyncCommand MalusLawCommand { get; }
    }
}
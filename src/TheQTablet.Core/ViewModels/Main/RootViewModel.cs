using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;

using TheQTablet.Core.Service.Interfaces;
using TheQTablet.Core.ViewModels.Main.Lesson01;

namespace TheQTablet.Core.ViewModels.Main
{
    public class Lesson
    {
        public int Number { get; }
        public string Title { get; }

        public Type _startView;

        public Lesson(int number, string title, Type startView)
        {
            Number = number;
            Title = title;

            _startView = startView;
        }
    }

    public class RootViewModel : BaseViewModel
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly IMvxLog _log;

        private ObservableCollection<Lesson> _lessons;
        public ObservableCollection<Lesson> Lessons {
            get => _lessons;
            set => SetProperty(ref _lessons, value);
        }

        public MvxAsyncCommand<Lesson> NavigateToLessonCommand { get; private set; }
        public MvxAsyncCommand SettingsCommand { get; private set; }

        public RootViewModel(
            IMvxLog log,
            IMvxNavigationService navigationService)
        {
            _log = log;
            _navigationService = navigationService;

            _log.Trace("RootViewModel:RootViewModel()");

            NavigateToLessonCommand = new MvxAsyncCommand<Lesson>(NavigateToLesson);
            SettingsCommand = new MvxAsyncCommand(Settings);

            Lessons = new ObservableCollection<Lesson>
            {
                new Lesson(1, "LIGHT POLARISATION", typeof(Lesson01ProfessorViewModel)),
            };
        }

        private async Task NavigateToLesson(Lesson value)
        {
            await _navigationService.Navigate(value._startView);
        }

        private async Task Settings()
        {
            await _navigationService.Navigate<ConnectivityViewModel>();
        }
    }
}

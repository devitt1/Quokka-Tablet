using System.Threading.Tasks;

using MvvmCross.Logging;

using TheQTablet.Core.Service.Interfaces;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using TheQTablet.Core.ViewModels.Main.Lesson01;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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
        private readonly ISimulatorService _simulationService;

        private ObservableCollection<Lesson> _lessons;
        public ObservableCollection<Lesson> Lessons {
            get => _lessons;
            set => SetProperty(ref _lessons, value);
        }

        public RootViewModel(
            IMvxLog log,
            ISimulatorService simulationService,
            IMvxNavigationService navigationService)
        {
            _log = log;
            _simulationService = simulationService;
            _navigationService = navigationService;

            _log.Trace("RootViewModel:RootViewModel()");

            NavigateToLessonCommand = new MvxAsyncCommand<Lesson>(NavigateToLesson);

            Lessons = new ObservableCollection<Lesson>
            {
                new Lesson(1, "LIGHT POLARISATION", typeof(Lesson01StartViewModel)),
            };
        }

        private async Task NavigateToLesson(Lesson value)
        {
            await _navigationService.Navigate(value._startView);
        }

        public MvxAsyncCommand<Lesson> NavigateToLessonCommand { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels.Main
{
    public class StoryContainerViewModel : MvxNavigationViewModel
    {
        public StoryContainerViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            NextCommand = new MvxAsyncCommand(ShowNextStory);
        }

        private async Task ShowNextStory()
        {
            _currentIndex++;
            if (_currentIndex < _storyArrayText.Count)
            {
                StoryLabel = _storyArrayText[_currentIndex];
            }
            else
            {
                await NavigationService.Navigate<PlotViewModel>();
            }
        }

        private List<string> _storyArrayText = new List<string>(){"Emma has set up her new telescope, the night sky is clear and the stars are bright",
        "The telescope has a polarisation filter and quantum circuit built in, it can record the photons that hit the lens",
        "Tap the telescope to find a star"};

        public MvxAsyncCommand NextCommand { get; }

        private string _storyLabel = "";
        public string StoryLabel { get => _storyLabel; set => SetProperty(ref _storyLabel, value); }
        private int _currentIndex = 0;
        public override void Prepare()
        {
            base.Prepare();
            StoryLabel = _storyArrayText[0];
            _currentIndex = 0;
        }
    }
}

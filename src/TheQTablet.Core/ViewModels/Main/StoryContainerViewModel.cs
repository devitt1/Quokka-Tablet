using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using TheQTablet.Core.ViewModels.Main.Lesson01;

namespace TheQTablet.Core.ViewModels.Main
{
    public class StoryContainerViewModel : Lesson01BaseViewModel
    {
        public StoryContainerViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
            NextCommand = new MvxAsyncCommand(ShowNextStory);
            TelescopeCommand = new MvxAsyncCommand(ShowTelescope);
        }

        private async Task ShowNextStory()
        {
            _currentIndex++;
            if (_currentIndex < _storyArrayText.Count)
            {
                StoryLabel = _storyArrayText[_currentIndex];
            }

            if(_currentIndex ==_storyArrayText.Count -1)
            {
                EnableButton = true;
            }
        }


        

        private async Task ShowTelescope()
        {
            await NavigationService.Navigate<TelescopeSearchViewModel>();
        }

        private List<string> _storyArrayText = new List<string>(){"Emma has set up her new telescope, the night sky is clear and the stars are bright",
        "The telescope has a polarisation filter and quantum circuit built in, it can record the photons that hit the lens",
        "Tap the telescope to find a star"};

        public MvxAsyncCommand NextCommand { get; }
        public MvxAsyncCommand TelescopeCommand { get; }

        private bool _enabled = false;
        public bool EnableButton { get => _enabled; set => SetProperty(ref _enabled, value); }


        private string _storyLabel = "";
        public string StoryLabel { get => _storyLabel; set => SetProperty(ref _storyLabel, value); }
        private int _currentIndex = 0;
        public override void Prepare()
        {
            base.Prepare();
            StoryLabel = _storyArrayText[0];
            _currentIndex = 0;
            EnableButton = false;
        }
    }
}

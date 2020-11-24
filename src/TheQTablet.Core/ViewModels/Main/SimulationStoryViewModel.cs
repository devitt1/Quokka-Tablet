using System;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace TheQTablet.Core.ViewModels.Main
{
    public class StoryContainerVieModel : MvxNavigationService
    {
        public StoryContainerVieModel(IMvxNavigationCache navigationCache, IMvxViewModelLoader viewModelLoader) : base(navigationCache, viewModelLoader)
        {
        }
    }
}

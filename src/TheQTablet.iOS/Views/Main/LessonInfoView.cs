using System;
using Cirrious.FluentLayouts.Touch;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using TheQTablet.Core.ViewModels.Main;
using UIKit;

namespace TheQTablet.iOS
{
    [MvxFromStoryboard("Main")]
    public partial class LessonInfoView : MvxViewController<LessonInfoViewModel>
    {
        private UIButton _nextButton;

        protected internal LessonInfoView(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _nextButton = ButtonGenerator.DarkButton("CONTINUE");
           
            this.View.AddSubview(_nextButton);

            _nextButton.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -46).Active = true;
            _nextButton.RightAnchor.ConstraintEqualTo(View.RightAnchor, -46).Active = true;
           

            var set = CreateBindingSet();
            set.Bind(_nextButton).For("TouchUpInside").To(vm => vm.NexLessonViewModel);
            set.Apply();
        }
    }
}

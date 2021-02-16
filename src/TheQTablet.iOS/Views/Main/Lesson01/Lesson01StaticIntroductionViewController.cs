using System;
using TheQTablet.Core.ViewModels.Main;
using MvvmCross.Binding.BindingContext;
using TheQTablet.Core.ViewModels.Main.Lesson01;
using UIKit;

namespace TheQTablet.iOS.Views.Main.Lesson01
{
    public class Lesson01StaticIntroductionViewController : LessonBaseViewController<StoryContainerViewModel>
    {
        private UIImageView _background;

        private UIView _infoTextContainer;
        private UILabel _infoText;
        private UIButton _telescopeButton;
        private UIButton _refuseButton;

        public Lesson01StaticIntroductionViewController()
        {
        }

        protected override void CreateView()
        {
            base.CreateView();

            _background = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("professor_scene"),
            };
            View.AddSubview(_background);

            _infoTextContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White.ColorWithAlpha(0.5f),
            };
            _infoTextContainer.Layer.CornerRadius = 20;
            View.AddSubview(_infoTextContainer);

            _infoTextContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White.ColorWithAlpha(0.5f),
            };
            _infoTextContainer.Layer.CornerRadius = 20;
            View.AddSubview(_infoTextContainer);

            _infoText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Center,
                TextColor = ColorPalette.PrimaryText,
            };
            _infoTextContainer.AddSubview(_infoText);

            _refuseButton = ButtonGenerator.PrimaryButton("Next", 40);
            _infoTextContainer.AddSubview(_refuseButton);

            _telescopeButton = new UIButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
                ClipsToBounds = true,
            };
 
            View.AddSubview(_telescopeButton);

        }

        protected override void LayoutView()
        {
            base.LayoutView();

            _background.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            _infoTextContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor, 50).Active = true;
            _infoTextContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor,25).Active = true;
            _infoTextContainer.RightAnchor.ConstraintEqualTo(View.RightAnchor, -25).Active = true;

            _infoText.TopAnchor.ConstraintEqualTo(_infoTextContainer.TopAnchor, 20).Active = true;
            _infoText.BottomAnchor.ConstraintEqualTo(_infoTextContainer.BottomAnchor, -20).Active = true;
            _infoText.LeftAnchor.ConstraintEqualTo(_infoTextContainer.LeftAnchor, 20).Active = true;

            _refuseButton.RightAnchor.ConstraintEqualTo(_infoTextContainer.RightAnchor,-20).Active = true;
            _refuseButton.TopAnchor.ConstraintEqualTo(_infoTextContainer.TopAnchor,10).Active = true;
            _refuseButton.WidthAnchor.ConstraintEqualTo(100).Active = true;
            _refuseButton.BottomAnchor.ConstraintEqualTo(_infoTextContainer.BottomAnchor, -10).Active = true;
            _refuseButton.LeftAnchor.ConstraintEqualTo(_infoText.RightAnchor, 10).Active = true;

            _telescopeButton.LeftAnchor.ConstraintEqualTo(_infoTextContainer.LeftAnchor, 250).Active = true;
            _telescopeButton.CenterYAnchor.ConstraintEqualTo(View.CenterYAnchor).Active = true;
            _telescopeButton.WidthAnchor.ConstraintEqualTo(300).Active = true;
            _telescopeButton.HeightAnchor.ConstraintEqualTo(300).Active = true;

        }

        protected override void BindView()
        {
            base.BindView();
            var set = this.CreateBindingSet<Lesson01StaticIntroductionViewController,StoryContainerViewModel>();

            set.Bind(_refuseButton).For("TouchUpInside").To(vm => vm.NextCommand);
            set.Bind(_telescopeButton).For("TouchUpInside").To(vm => vm.TelescopeCommand);
            set.Bind(_telescopeButton).For(btn=>btn.Enabled).To(vm => vm.EnableButton);

            set.Bind(_infoText).To(vm => vm.StoryLabel);
            set.Apply();
        }
    }
}

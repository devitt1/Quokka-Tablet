using System;
using TheQTablet.Core.ViewModels.Main;
using UIKit;

namespace TheQTablet.iOS.Views.Main.Lesson01
{
    public class Lesson01StartViewController : BaseViewController<Lesson01StartViewModel>
    {
        private UIImageView _background;
        private UIView _logoCentringContainer;
        private UIImageView _logo;

        private UIView _textContainer;
        private UILabel _introText;
        private UIButton _backButton;
        private UIButton _continueButton;

        private UIView _headingContainer;
        private UIImageView _headingContainerBackground;
        private UILabel _lessonNumber;
        private UILabel _lessonName;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _background = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("lesson01_background_dark"),
            };
            View.AddSubview(_background);

            _logoCentringContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_logoCentringContainer);

            _logo = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("logo"),
            };
            _logoCentringContainer.AddSubview(_logo);

            _textContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White.ColorWithAlpha(0.3f),
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Leading = 40,
                    Trailing = 40,
                    Bottom = 40,
                }
            };
            _textContainer.Layer.CornerRadius = 10;
            View.AddSubview(_textContainer);

            _introText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "In this module we will learn about polarisation of photons as they pass through a lens. Using the Q we can investigate how photons change through a polarised lens.",
                Font = FontGenerator.GenerateFont(26, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            _textContainer.AddSubview(_introText);

            _backButton = ButtonGenerator.SecondaryButton("BACK");
            _textContainer.AddSubview(_backButton);

            _continueButton = ButtonGenerator.PrimaryButton("Continue");
            _textContainer.AddSubview(_continueButton);

            _headingContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ClipsToBounds = true,
            };
            _headingContainer.Layer.CornerRadius = 10;
            View.AddSubview(_headingContainer);

            _headingContainerBackground = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("banner_gradient"),
            };
            //_headingContainerBackground.Layer.CornerRadius = _headingContainer.Layer.CornerRadius;
            _headingContainer.AddSubview(_headingContainerBackground);

            _lessonNumber = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "LESSON 1",
                Font = FontGenerator.GenerateFont(30, UIFontWeight.Bold),
                TextColor = ColorPalette.SecondaryText,
            };
            _headingContainer.AddSubview(_lessonNumber);

            _lessonName = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "LIGHT POLARISATION",
                Font = FontGenerator.GenerateFont(30, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            _headingContainer.AddSubview(_lessonName);


            _background.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            _logoCentringContainer.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _logoCentringContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            _logoCentringContainer.BottomAnchor.ConstraintEqualTo(_headingContainer.TopAnchor).Active = true;
            _logoCentringContainer.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.5f).Active = true;
            // Magic numbers to centre planet in lopsided logo image
            _logoCentringContainer.WidthAnchor.ConstraintEqualTo(_logo.WidthAnchor, 1.152f).Active = true;
            _logoCentringContainer.HeightAnchor.ConstraintEqualTo(_logo.HeightAnchor, 1.149f).Active = true;

            _logo.LeftAnchor.ConstraintEqualTo(_logoCentringContainer.LeftAnchor).Active = true;
            _logo.BottomAnchor.ConstraintEqualTo(_logoCentringContainer.BottomAnchor).Active = true;
            _logo.HeightAnchor.ConstraintEqualTo(_logo.WidthAnchor, _logo.Image.Size.Height / _logo.Image.Size.Width).Active = true;

            _textContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 30).Active = true;
            _textContainer.RightAnchor.ConstraintEqualTo(View.RightAnchor, -30).Active = true;
            _textContainer.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -30).Active = true;

            _introText.TopAnchor.ConstraintEqualTo(_headingContainer.BottomAnchor, 30).Active = true;
            _introText.CenterXAnchor.ConstraintEqualTo(_textContainer.CenterXAnchor).Active = true;
            _introText.WidthAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.WidthAnchor).Active = true;
            _introText.BottomAnchor.ConstraintLessThanOrEqualTo(_backButton.TopAnchor, -20).Active = true;
            _introText.BottomAnchor.ConstraintLessThanOrEqualTo(_continueButton.TopAnchor, -20).Active = true;

            _backButton.LeftAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _backButton.BottomAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.BottomAnchor).Active = true;

            _continueButton.RightAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.RightAnchor).Active = true;
            _continueButton.BottomAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.BottomAnchor).Active = true;

            _headingContainerBackground.WidthAnchor.ConstraintEqualTo(_headingContainer.WidthAnchor).Active = true;
            _headingContainerBackground.HeightAnchor.ConstraintEqualTo(_headingContainer.HeightAnchor).Active = true;

            _headingContainer.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _headingContainer.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5f).Active = true;
            _headingContainer.BottomAnchor.ConstraintEqualTo(_textContainer.TopAnchor, 30).Active = true;

            _lessonNumber.CenterXAnchor.ConstraintEqualTo(_headingContainer.CenterXAnchor).Active = true;
            _lessonNumber.TopAnchor.ConstraintEqualTo(_headingContainer.TopAnchor, 30).Active = true;
            _lessonNumber.BottomAnchor.ConstraintEqualTo(_lessonName.TopAnchor, -10).Active = true;

            _lessonName.CenterXAnchor.ConstraintEqualTo(_headingContainer.CenterXAnchor).Active = true;
            _lessonName.BottomAnchor.ConstraintEqualTo(_headingContainer.BottomAnchor, -30).Active = true;

            var set = CreateBindingSet();
            set.Bind(_backButton).To(vm => vm.BackCommand);
            set.Bind(_continueButton).To(vm => vm.ContinueCommand);
            set.Apply();
        }
    }
}

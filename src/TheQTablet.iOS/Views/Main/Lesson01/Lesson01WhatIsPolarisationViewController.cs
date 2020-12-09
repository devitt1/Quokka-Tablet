using System;
using TheQTablet.Core.ViewModels.Main;
using TheQTablet.iOS.Views.Custom;
using UIKit;

namespace TheQTablet.iOS.Views.Main.Lesson01
{
    public class Lesson01WhatIsPolarisationViewController : BaseViewController<Lesson01WhatIsPolarisationViewModel>
    {
        private UIImageView _background;

        private UIView _textContainer;
        private UILabel _introText;

        private UIView _headingContainer;
        private UIImageView _headingContainerBackground;
        private UILabel _heading;

        private UIImageView _fieldGraph;
        private KnobContainerView _dial;
        private UILabel _dialSubheader;

        private UIButton _backButton;
        private UIButton _continueButton;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _background = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("lesson01_background_dark"),
            };
            View.AddSubview(_background);

            _textContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.White.ColorWithAlpha(0.3f),
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Leading = 30,
                    Trailing = 30,
                    Bottom = 30,
                }
            };
            _textContainer.Layer.CornerRadius = 10;
            View.AddSubview(_textContainer);

            _introText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text =
                (
                    "A light wave is an electromagnetic wave that travels through the vacuum of outer space. Light waves are produced by vibrating an electric charge.\n" +
                    "It is possible to transform unpolarised light into polarised light. Polarised light waves are light waves in which the vibration occurs in a single plane."
                ),
                Font = FontGenerator.GenerateFont(26, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            _introText.SetContentCompressionResistancePriority((float)UILayoutPriority.Required, UILayoutConstraintAxis.Vertical);
            _textContainer.AddSubview(_introText);

            _backButton = ButtonGenerator.SecondaryButton("BACK");
            View.AddSubview(_backButton);

            _continueButton = ButtonGenerator.PrimaryButton("Continue");
            View.AddSubview(_continueButton);

            _headingContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ClipsToBounds = true,
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Top = 30,
                    Bottom = 30,
                    Leading = 30,
                    Trailing = 30,
                },
                BackgroundColor = UIColor.Red,
            };
            _headingContainer.Layer.CornerRadius = 10;
            View.AddSubview(_headingContainer);

            _headingContainerBackground = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleToFill,
                Image = UIImage.FromBundle("banner_gradient"),
            };
            _headingContainer.AddSubview(_headingContainerBackground);

            _heading = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "WHAT IS POLARISATION",
                Font = FontGenerator.GenerateFont(30, UIFontWeight.Bold),
                TextColor = ColorPalette.SecondaryText,
            };
            _heading.SetContentCompressionResistancePriority((float)UILayoutPriority.Required, UILayoutConstraintAxis.Vertical);
            _headingContainer.AddSubview(_heading);

            _fieldGraph = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("lesson01_field_graph"),
            };
            View.AddSubview(_fieldGraph);

            _dial = new KnobContainerView("THETA ANGLE")
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_dial);

            _dialSubheader = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "TURN THE DIAL TO ADJUST θ ANGLE",
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
                TextColor = ColorPalette.SecondaryText,
            };
            _dialSubheader.SetContentCompressionResistancePriority((float)UILayoutPriority.Required, UILayoutConstraintAxis.Vertical);
            _dial.InsertArrangedSubview(_dialSubheader, 1);

            _background.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            var viewMargin = 30;
            _textContainer.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, viewMargin).Active = true;
            _textContainer.RightAnchor.ConstraintEqualTo(View.RightAnchor, -viewMargin).Active = true;

            _introText.TopAnchor.ConstraintEqualTo(_headingContainer.BottomAnchor, 30).Active = true;
            _introText.BottomAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.BottomAnchor).Active = true;
            _introText.CenterXAnchor.ConstraintEqualTo(_textContainer.CenterXAnchor).Active = true;
            _introText.WidthAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.WidthAnchor).Active = true;

            _backButton.LeftAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _backButton.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -(_textContainer.DirectionalLayoutMargins.Leading + viewMargin)).Active = true;

            _continueButton.RightAnchor.ConstraintEqualTo(_textContainer.LayoutMarginsGuide.RightAnchor).Active = true;
            _continueButton.BottomAnchor.ConstraintEqualTo(_backButton.BottomAnchor).Active = true;

            _headingContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor, viewMargin).Active = true;
            _headingContainer.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _headingContainer.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.5f).Active = true;
            _headingContainer.BottomAnchor.ConstraintEqualTo(_textContainer.TopAnchor, 30).Active = true;

            _heading.CenterXAnchor.ConstraintEqualTo(_headingContainer.CenterXAnchor).Active = true;
            _heading.CenterYAnchor.ConstraintEqualTo(_headingContainer.CenterYAnchor).Active = true;
            _heading.TopAnchor.ConstraintEqualTo(_headingContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _heading.BottomAnchor.ConstraintEqualTo(_headingContainer.LayoutMarginsGuide.BottomAnchor).Active = true;

            _fieldGraph.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 5.0f / 8.0f).Active = true;
            _fieldGraph.LeftAnchor.ConstraintEqualTo(_backButton.LeftAnchor).Active = true;
            _fieldGraph.BottomAnchor.ConstraintEqualTo(_backButton.BottomAnchor).Active = true;
            _fieldGraph.TopAnchor.ConstraintEqualTo(_textContainer.BottomAnchor, 30).Active = true;

            _dial.LeftAnchor.ConstraintEqualTo(_fieldGraph.RightAnchor, 30).Active = true;
            _dial.RightAnchor.ConstraintEqualTo(_continueButton.RightAnchor).Active = true;
            _dial.TopAnchor.ConstraintEqualTo(_fieldGraph.TopAnchor).Active = true;
            _dial.BottomAnchor.ConstraintEqualTo(_continueButton.TopAnchor, -20).Active = true;

            var set = CreateBindingSet();
            set.Bind(_backButton).To(vm => vm.BackCommand);
            set.Bind(_continueButton).To(vm => vm.ContinueCommand);
            set.Apply();
        }
    }
}

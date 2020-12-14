using CoreGraphics;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using TheQTablet.Core.ViewModels.Main.Lesson01;
using UIKit;

namespace TheQTablet.iOS.Views.Main.Lesson01
{
    [MvxModalPresentation(WrapInNavigationController = false, ModalPresentationStyle = UIModalPresentationStyle.FormSheet)]
    public partial class Lesson01MalusLawViewController : BaseViewController<Lesson01MalusLawViewModel>, IUIAdaptivePresentationControllerDelegate
    {
        private UIImageView _backgroundGradient;

        private UIStackView _headingContainer;
        private UILabel _heading;
        private UIImageView _headingIcon;
        private UIImageView _closeCross;

        private UIView _headingDivider;
        private UIView _container;

        private UIView _videoContainer;
        private UIView _video;
        private UILabel _videoText;

        private UILabel _explanationText;
        private UIStackView _equationsContainer;
        private UILabel _leftEquation;
        private UILabel _centreEquation;
        private UILabel _rightEquation;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            PresentationController.Delegate = this;
            PreferredContentSize = new CGSize(UIScreen.MainScreen.Bounds.Width - 80, UIScreen.MainScreen.Bounds.Height);
        }

        protected override void CreateView()
        {
            base.CreateView();

            View.BackgroundColor = ColorPalette.BackgroundDark;
            _backgroundGradient = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("modal_background_gradient"),
            };
            View.AddSubview(_backgroundGradient);

            _headingContainer = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alignment = UIStackViewAlignment.Center,
                Axis = UILayoutConstraintAxis.Horizontal,
                Spacing = 20,
            };
            View.AddSubview(_headingContainer);

            _headingIcon = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("back_logo"),
            };
            _headingContainer.AddArrangedSubview(_headingIcon);

            _heading = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "MALUS' LAW",
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            _headingContainer.AddArrangedSubview(_heading);

            _closeCross = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("cross"),
            };
            View.AddSubview(_closeCross);

            _headingDivider = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = ColorPalette.Border,
            };
            View.AddSubview(_headingDivider);

            _container = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            View.AddSubview(_container);

            _videoContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Leading = 20,
                    Trailing = 20,
                    Top = 20,
                    Bottom = 20,
                }
            };
            _videoContainer.Layer.BorderWidth = 2;
            _videoContainer.Layer.BorderColor = ColorPalette.Border.CGColor;
            _videoContainer.Layer.CornerRadius = 20;
            _container.AddSubview(_videoContainer);

            _video = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Green,
            };
            _videoContainer.AddSubview(_video);

            _videoText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = (
                    "According to Malus, when completely plane polarized light is " +
                    "incident on the analyzer, the intensity I of the light transmitted " +
                    "by the analyzer is directly proportional to the square of the " +
                    "cosine of angle between the transmission axes of the analyzer and the polarizer"
                ),
                Font = FontGenerator.GenerateFont(26, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            _container.AddSubview(_videoText);

            var infoText = new string[] {
                (
                    "Suppose the angle between the transmission axes of the analyzer " +
                    "and the polarizer is θ. The completely plane polarizer light " +
                    "form the polarizer is incident on the analyzer. If E0 is the amplitude " +
                    "of the electric vector transmitted by the polarizer, then intensity " +
                    "I0 of the light incident of the light incident on the analyzer is "
                ),
                "I ∞ E02",
                (
                    "\n\n" +
                    "The electric field vector E0 can be resolved into two rectangular " +
                    "components i.e. E0 cosθ and E0 sinθ. The analyzer will " +
                    "transmit only the component which is parallel to its transmission " +
                    "axis. However the component E0sinθ will be absorbed by the analyzer. " +
                    "Therefore the intensity I of the light transmitted by the analyzer is:"
                )
            };
            var whiteText = new UIStringAttributes
            {
                ForegroundColor = ColorPalette.PrimaryText,
            };
            var attributes = new UIStringAttributes[] {
                whiteText,
                new UIStringAttributes
                {
                    ForegroundColor = ColorPalette.SecondaryText,
                },
                whiteText,
            };
            var attributedInfoText = AttributedStringGenerator.Generate(infoText, attributes);

            _explanationText = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                AttributedText = attributedInfoText,
                Font = FontGenerator.GenerateFont(26, UIFontWeight.Regular),
                //TextColor = ColorPalette.PrimaryText,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            _container.AddSubview(_explanationText);

            _equationsContainer = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Alignment = UIStackViewAlignment.Center,
                Distribution = UIStackViewDistribution.FillEqually,
            };
            _container.AddSubview(_equationsContainer);

            _leftEquation = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "I ∞ (E0 x cosθ)2",
                Font = FontGenerator.GenerateFont(26, UIFontWeight.Regular),
                TextColor = ColorPalette.SecondaryText,
                TextAlignment = UITextAlignment.Center,
            };
            _equationsContainer.AddArrangedSubview(_leftEquation);

            _centreEquation = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "I / 10 = (E0 x cosθ)2 / E02 = cos2θ",
                Font = FontGenerator.GenerateFont(26, UIFontWeight.Regular),
                TextColor = ColorPalette.SecondaryText,
                TextAlignment = UITextAlignment.Center,
            };
            _equationsContainer.AddArrangedSubview(_centreEquation);

            _rightEquation = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "I = I0 x cos2θ",
                Font = FontGenerator.GenerateFont(26, UIFontWeight.Regular),
                TextColor = ColorPalette.SecondaryText,
                TextAlignment = UITextAlignment.Center,
            };
            _equationsContainer.AddArrangedSubview(_rightEquation);
        }

        protected override void LayoutView()
        {
            base.LayoutView();

            _backgroundGradient.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _backgroundGradient.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            _headingContainer.CenterXAnchor.ConstraintEqualTo(View.CenterXAnchor).Active = true;
            _headingContainer.TopAnchor.ConstraintEqualTo(View.TopAnchor, 22).Active = true;

            _headingIcon.HeightAnchor.ConstraintEqualTo(_heading.Font.PointSize * 1.5f).Active = true;
            _headingIcon.WidthAnchor.ConstraintEqualTo(_headingIcon.HeightAnchor).Active = true;

            _closeCross.CenterYAnchor.ConstraintEqualTo(_heading.CenterYAnchor).Active = true;
            _closeCross.RightAnchor.ConstraintEqualTo(View.RightAnchor, -16).Active = true;
            _closeCross.WidthAnchor.ConstraintEqualTo(_closeCross.HeightAnchor).Active = true;
            _closeCross.WidthAnchor.ConstraintEqualTo(View.WidthAnchor, 0.02f).Active = true;

            _headingDivider.TopAnchor.ConstraintEqualTo(_heading.BottomAnchor, 22).Active = true;
            _headingDivider.LeftAnchor.ConstraintEqualTo(_container.LeftAnchor).Active = true;
            _headingDivider.RightAnchor.ConstraintEqualTo(_container.RightAnchor).Active = true;
            _headingDivider.HeightAnchor.ConstraintEqualTo(2).Active = true;

            _container.TopAnchor.ConstraintEqualTo(_headingDivider.BottomAnchor, 22).Active = true;
            _container.BottomAnchor.ConstraintEqualTo(View.BottomAnchor, -22).Active = true;
            _container.LeftAnchor.ConstraintEqualTo(View.LeftAnchor, 40).Active = true;
            _container.RightAnchor.ConstraintEqualTo(View.RightAnchor, -40).Active = true;

            _videoContainer.TopAnchor.ConstraintEqualTo(_container.TopAnchor).Active = true;
            _videoContainer.LeftAnchor.ConstraintEqualTo(_container.LeftAnchor).Active = true;

            _video.TopAnchor.ConstraintEqualTo(_videoContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _video.BottomAnchor.ConstraintEqualTo(_videoContainer.LayoutMarginsGuide.BottomAnchor).Active = true;
            _video.LeftAnchor.ConstraintEqualTo(_videoContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _video.RightAnchor.ConstraintEqualTo(_videoContainer.LayoutMarginsGuide.RightAnchor).Active = true;
            _video.WidthAnchor.ConstraintEqualTo(_container.WidthAnchor, 0.5f).Active = true;
            _video.HeightAnchor.ConstraintEqualTo(_video.WidthAnchor, 0.5f).Active = true;

            _videoText.TopAnchor.ConstraintEqualTo(_videoContainer.TopAnchor).Active = true;
            _videoText.BottomAnchor.ConstraintEqualTo(_videoContainer.BottomAnchor).Active = true;
            _videoText.LeftAnchor.ConstraintEqualTo(_videoContainer.RightAnchor, 20).Active = true;
            _videoText.RightAnchor.ConstraintEqualTo(_container.RightAnchor).Active = true;

            _explanationText.TopAnchor.ConstraintEqualTo(_videoContainer.BottomAnchor, 20).Active = true;
            _explanationText.LeftAnchor.ConstraintEqualTo(_container.LeftAnchor).Active = true;
            _explanationText.RightAnchor.ConstraintEqualTo(_container.RightAnchor).Active = true;

            _equationsContainer.TopAnchor.ConstraintEqualTo(_explanationText.BottomAnchor, 20).Active = true;
            _equationsContainer.LeftAnchor.ConstraintEqualTo(_container.LeftAnchor).Active = true;
            _equationsContainer.RightAnchor.ConstraintEqualTo(_container.RightAnchor).Active = true;
            _equationsContainer.BottomAnchor.ConstraintLessThanOrEqualTo(_container.BottomAnchor).Active = true;
        }

        protected override void BindView()
        {
            base.BindView();

            var set = CreateBindingSet();
            set.Bind(_closeCross).For("Tap").To(vm => vm.CloseCommand);
            set.Apply();
        }

        [Export("presentationControllerDidDismiss:")]
        public void DidDismiss(UIPresentationController presentationController)
        {
            ViewModel.CloseCommand.Execute();
        }
    }
}

using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;

using TheQTablet.Core.ViewModels.Main;
using TheQTablet.iOS.Views.Custom;

using UIKit;

namespace TheQTablet.iOS
{
    [MvxRootPresentation(WrapInNavigationController = true)]
    public partial class RootViewController : BaseViewController<RootViewModel>
    {
        private UIImageView _background;

        private UIStackView _viewSplit;

        private UIView _leftContainer;
        private UIView _logoContainer;
        private UIImageView _logo;
        private UILabel _logoTitle;
        private UILabel _utsLabel;

        private UIView _lessonsContainer;
        private PaddedLabel _lessonTitle;
        private Divider _lessonsTopBorder;
        private UICollectionView _lessons;
        private Divider _lessonsBottomBorder;

        private IconButton _settings;

        private LessonSource _lessonSource;

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController.NavigationBar.Hidden = true;
            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            NavigationController.SetNeedsStatusBarAppearanceUpdate();
        }

        protected override void CreateView()
        {
            View.DirectionalLayoutMargins = new NSDirectionalEdgeInsets
            {
                Top = 20,
                Bottom = 60,
                Leading = 40,
                Trailing = 40,
            };

            _background = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("start_background"),
            };
            View.AddSubview(_background);

            _viewSplit = new UIStackView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = UILayoutConstraintAxis.Horizontal,
                Alignment = UIStackViewAlignment.Center,
                Distribution = UIStackViewDistribution.FillEqually,
            };
            View.AddSubview(_viewSplit);

            _leftContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _viewSplit.AddArrangedSubview(_leftContainer);

            _logoContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = ColorPalette.AccentLight,
                DirectionalLayoutMargins = new NSDirectionalEdgeInsets
                {
                    Leading = 40,
                    Trailing = 40,
                    Top = 40,
                    Bottom = 40,
                },
            };
            _logoContainer.Layer.CornerRadius = 20;
            _leftContainer.AddSubview(_logoContainer);

            _logo = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("dark_logo"),
            };
            _logoContainer.AddSubview(_logo);

            _logoTitle = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "The Q",
                TextAlignment = UITextAlignment.Center,
                Font = FontGenerator.GenerateFont(48, UIFontWeight.Bold),
                TextColor = ColorPalette.TertiaryText,
            };
            _logoContainer.AddSubview(_logoTitle);

            _utsLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "UNIVERSITY OF TECHNOLOGY\nSYDNEY, AUSTRALIA",
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            _leftContainer.AddSubview(_utsLabel);

            _lessonsContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _viewSplit.AddArrangedSubview(_lessonsContainer);

            _lessonTitle = new PaddedLabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "LESSONS",
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Bold),
                TextColor = ColorPalette.PrimaryText,
                BackgroundColor = ColorPalette.AccentLight.ColorWithAlpha(0.3f),
                Insets = new UIEdgeInsets
                {
                    Top = 10,
                    Bottom = 10,
                    Left = 40,
                    Right = 40,
                },
                ClipsToBounds = true,
            };
            _lessonTitle.Layer.CornerRadius = 10;
            _lessonsContainer.AddSubview(_lessonTitle);

            _lessonsTopBorder = new Divider
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = DividerAxis.Horizontal,
            };
            _lessonsContainer.AddSubview(_lessonsTopBorder);

            var layout = new FixedRowCountFlowLayout
            {
                CountPerRow = 2,
                MinimumLineSpacing = 20,
                MinimumInteritemSpacing = 20,
                SectionInset = new UIEdgeInsets(20, 10, 20, 10),
                ScrollDirection = UICollectionViewScrollDirection.Vertical,
            };
            _lessons = new UICollectionView(View.Bounds, layout)
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Clear,
            };
            _lessonsContainer.AddSubview(_lessons);

            _lessonSource = new LessonSource(_lessons);
            _lessons.Source = _lessonSource;

            _lessonsBottomBorder = new Divider
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = DividerAxis.Horizontal,
            };
            _lessonsContainer.AddSubview(_lessonsBottomBorder);

            _settings = new IconButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "SETTINGS",
                Icon = UIImage.FromBundle("settings_gear"),
            };
            View.AddSubview(_settings);
        }

        protected override void LayoutView()
        {
            _background.WidthAnchor.ConstraintEqualTo(View.WidthAnchor).Active = true;
            _background.HeightAnchor.ConstraintEqualTo(View.HeightAnchor).Active = true;

            _viewSplit.LeftAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.LeftAnchor).Active = true;
            _viewSplit.RightAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.RightAnchor).Active = true;
            _viewSplit.TopAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.TopAnchor).Active = true;
            _viewSplit.BottomAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.BottomAnchor).Active = true;

            _leftContainer.HeightAnchor.ConstraintEqualTo(_viewSplit.HeightAnchor).Active = true;

            _logoContainer.CenterXAnchor.ConstraintEqualTo(_leftContainer.CenterXAnchor).Active = true;
            _logoContainer.CenterYAnchor.ConstraintEqualTo(_leftContainer.CenterYAnchor).Active = true;

            _logo.HeightAnchor.ConstraintEqualTo(View.HeightAnchor, 0.1f).Active = true;
            _logo.AspectRatioConstraint().Active = true;
            _logo.LeftAnchor.ConstraintEqualTo(_logoContainer.LayoutMarginsGuide.LeftAnchor).Active = true;
            _logo.TopAnchor.ConstraintEqualTo(_logoContainer.LayoutMarginsGuide.TopAnchor).Active = true;
            _logo.BottomAnchor.ConstraintEqualTo(_logoContainer.LayoutMarginsGuide.BottomAnchor).Active = true;

            _logoTitle.LeftAnchor.ConstraintEqualTo(_logo.RightAnchor, 40).Active = true;
            _logoTitle.RightAnchor.ConstraintEqualTo(_logoContainer.LayoutMarginsGuide.RightAnchor).Active = true;
            _logoTitle.CenterYAnchor.ConstraintEqualTo(_logoContainer.CenterYAnchor).Active = true;

            _utsLabel.TopAnchor.ConstraintEqualTo(_logoContainer.BottomAnchor, 20).Active = true;
            _utsLabel.CenterXAnchor.ConstraintEqualTo(_logoContainer.CenterXAnchor).Active = true;

            _settings.LeftAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.LeftAnchor).Active = true;
            _settings.BottomAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.BottomAnchor).Active = true;


            _lessonsContainer.TopAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.TopAnchor).Active = true;
            _lessonsContainer.BottomAnchor.ConstraintEqualTo(View.LayoutMarginsGuide.BottomAnchor).Active = true;

            _lessonTitle.TopAnchor.ConstraintEqualTo(_lessonsContainer.TopAnchor, 20).Active = true;
            _lessonTitle.CenterXAnchor.ConstraintEqualTo(_lessonsContainer.CenterXAnchor).Active = true;

            _lessonsTopBorder.TopAnchor.ConstraintEqualTo(_lessonTitle.BottomAnchor, 20).Active = true;
            _lessonsTopBorder.WidthAnchor.ConstraintEqualTo(_lessonsContainer.WidthAnchor).Active = true;

            _lessons.TopAnchor.ConstraintEqualTo(_lessonsTopBorder.BottomAnchor).Active = true;
            _lessons.WidthAnchor.ConstraintEqualTo(_lessonsContainer.WidthAnchor).Active = true;
            _lessons.CenterXAnchor.ConstraintEqualTo(_lessonsContainer.CenterXAnchor).Active = true;

            _lessonsBottomBorder.TopAnchor.ConstraintEqualTo(_lessons.BottomAnchor).Active = true;
            _lessonsBottomBorder.BottomAnchor.ConstraintEqualTo(_lessonsContainer.BottomAnchor).Active = true;
            _lessonsBottomBorder.WidthAnchor.ConstraintEqualTo(_lessonsContainer.WidthAnchor).Active = true;
        }

        protected override void BindView()
        {
            base.BindView();

            var set = this.CreateBindingSet<RootViewController, RootViewModel>();
            set.Bind(_lessonSource).For(v => v.ItemsSource).To(vm => vm.Lessons);
            set.Bind(_lessonSource).For(v => v.SelectionChangedCommand).To(vm => vm.NavigateToLessonCommand);
            set.Bind(_settings).To(vm => vm.SettingsCommand);
            set.Apply();
        }
    }
}

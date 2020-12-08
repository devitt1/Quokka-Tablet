using Cirrious.FluentLayouts.Touch;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using TheQTablet.iOS;
using UIKit;

namespace TheQTablet.iOS
{
    public class ExitLessonButton : UIView
    {
        private UIImageView _logo;
        private UILabel _text;

        public ExitLessonButton()
        {
            _logo = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Image = UIImage.FromBundle("back_logo"),
            };
            AddSubview(_logo);

            _text = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "EXIT LESSON 1",
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular),
            };
            AddSubview(_text);

            _logo.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _text.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;

            _logo.RightAnchor.ConstraintEqualTo(_text.LeftAnchor, -14).Active = true;
            _logo.WidthAnchor.ConstraintEqualTo(_logo.HeightAnchor).Active = true;

            TopAnchor.ConstraintLessThanOrEqualTo(_logo.TopAnchor).Active = true;
            TopAnchor.ConstraintLessThanOrEqualTo(_text.TopAnchor).Active = true;
            BottomAnchor.ConstraintGreaterThanOrEqualTo(_logo.BottomAnchor).Active = true;
            BottomAnchor.ConstraintGreaterThanOrEqualTo(_text.BottomAnchor).Active = true;

            _text.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            _logo.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
        }
    }

    public abstract class BaseViewController<TViewModel> : MvxViewController<TViewModel>
        where TViewModel : class, IMvxViewModel
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            NavigationController.NavigationBar.Translucent = false;
            NavigationController.NavigationBar.Hidden = false;
            NavigationController.NavigationBar.BarTintColor = ColorPalette.Primary;
            NavigationController.NavigationBar.TintColor = UIColor.White;
            NavigationController.NavigationBar.SetBackgroundImage(UIImage.FromBundle("banner_gradient"), UIBarMetrics.Default);

            NavigationItem.HidesBackButton = true;
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(new ExitLessonButton());
            NavigationItem.TitleView = new UILabel
            {
                Text = "LESSON 1 : LIGHT POLARISATION",
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
            };

            NavigationController.SetNeedsStatusBarAppearanceUpdate();

            CreateView();

            LayoutView();

            BindView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
        }

        protected virtual void CreateView()
        {
        }

        protected virtual void LayoutView()
        {
        }

        protected virtual void BindView()
        {
        }
    }
}

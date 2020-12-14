using TheQTablet.Core.ViewModels.Main.Lesson01;
using TheQTablet.iOS.Views.Custom;
using UIKit;

namespace TheQTablet.iOS
{
    public abstract class LessonBaseViewController<TViewModel> : BaseNavbarViewController<TViewModel>
        where TViewModel : LessonBaseViewModel
    {
        protected string _title = "LESSON";
        protected string _exitLabel = "EXIT LESSON";

        private ExitLessonButton _exitButton;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _exitButton = new ExitLessonButton
            {
                Text = _exitLabel
            };
            View.AddSubview(_exitButton);

            NavigationItem.HidesBackButton = true;
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(_exitButton);
            NavigationItem.TitleView = new UILabel
            {
                Text = _title,
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Regular),
            };

            var set = CreateBindingSet();
            set.Bind(_exitButton).For("Tap").To(vm => vm.ExitCommand);
            set.Apply();
        }
    }

    public abstract class Lesson01BaseViewController<TViewModel> : LessonBaseViewController<TViewModel>
        where TViewModel : LessonBaseViewModel
    {
        public Lesson01BaseViewController()
        {
            _title = "LESSON 1 : LIGHT POLARISATION";
            _exitLabel = "EXIT LESSON 1";
        }
    }
}

using System;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class ExitLessonButton : MvxView
    {
        private UIImageView _logo;
        private UILabel _label;

        private UITapGestureRecognizer _tapRecognizer;
        public event EventHandler Tap;

        private string _text = "EXIT LESSON 1";
        public string Text
        {
            get => _text;
            set {
                _text = value;
                SetText();
            }
        }

        public ExitLessonButton()
        {
            _tapRecognizer = new UITapGestureRecognizer();
            _tapRecognizer.AddTarget(() => Tapped());
            AddGestureRecognizer(_tapRecognizer);

            _logo = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                Image = UIImage.FromBundle("back_logo"),
            };
            AddSubview(_logo);

            _label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = Text,
                TextColor = ColorPalette.PrimaryText,
                Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular),
            };
            AddSubview(_label);

            _logo.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _logo.HeightAnchor.ConstraintEqualTo(30).Active = true;
            _label.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;

            _logo.RightAnchor.ConstraintEqualTo(_label.LeftAnchor, -14).Active = true;
            _logo.WidthAnchor.ConstraintEqualTo(_logo.HeightAnchor, _logo.Image.Size.Width / _logo.Image.Size.Height).Active = true;

            TopAnchor.ConstraintLessThanOrEqualTo(_logo.TopAnchor).Active = true;
            TopAnchor.ConstraintLessThanOrEqualTo(_label.TopAnchor).Active = true;
            BottomAnchor.ConstraintGreaterThanOrEqualTo(_logo.BottomAnchor).Active = true;
            BottomAnchor.ConstraintGreaterThanOrEqualTo(_label.BottomAnchor).Active = true;

            _label.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
            _logo.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
        }

        private void SetText()
        {
            if(_label != null)
            {
                _label.Text = Text;
            }
        }

        private void Tapped()
        {
            Tap?.Invoke(this, EventArgs.Empty);
        }
    }
}

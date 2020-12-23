using System;

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class TextbookButton : UIView
    {
        private UIImageView _icon;
        private UILabel _label;

        private UITapGestureRecognizer _tapRecognizer;
        public event EventHandler Tap;

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                SetText();
            }
        }

        private NSLayoutConstraint _imageSizeConstraint;
        private UIImage _image;
        public UIImage Image
        {
            get => _image;
            set
            {
                _image = value;
                SetIcon();
            }
        }

        public TextbookButton()
        {
            _tapRecognizer = new UITapGestureRecognizer();
            _tapRecognizer.AddTarget(() => Tap?.Invoke(this, EventArgs.Empty));

            _icon = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = Image,
            };
            AddSubview(_icon);

            _label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = Text,
                Font = FontGenerator.GenerateFont(12, UIFontWeight.Regular),
                TextColor = ColorPalette.PrimaryText,
            };
            AddSubview(_label);

            _icon.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _icon.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            RecalculateImageAspect();

            _label.TopAnchor.ConstraintEqualTo(_icon.BottomAnchor, 5).Active = true;
            _label.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
            _label.HeightAnchor.ConstraintEqualTo(_label.Font.PointSize).Active = true;
            _label.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;

            LeftAnchor.ConstraintLessThanOrEqualTo(_icon.LeftAnchor).Active = true;
            LeftAnchor.ConstraintLessThanOrEqualTo(_label.LeftAnchor).Active = true;

            RightAnchor.ConstraintGreaterThanOrEqualTo(_icon.RightAnchor).Active = true;
            RightAnchor.ConstraintGreaterThanOrEqualTo(_label.RightAnchor).Active = true;
        }

        private void SetText()
        {
            if (_label != null)
            {
                _label.Text = Text;
            }
        }

        private void SetIcon()
        {
            if (_icon != null)
            {
                _icon.Image = Image;
                RecalculateImageAspect();
            }
        }

        private void RecalculateImageAspect()
        {
            if (_imageSizeConstraint != null)
            {
                _imageSizeConstraint.Active = false;
            }
            if (_icon.Image != null)
            {
                _imageSizeConstraint = _icon.WidthAnchor.ConstraintEqualTo(_icon.HeightAnchor, _icon.Image.Size.Width / _icon.Image.Size.Height);
                _imageSizeConstraint.Active = true;
            }
        }
    }

    public class TextbookButtons : UIView
    {
        private Divider _divider;
        private TextbookButton _malusLawButton;

        public TextbookButton MalusLawButton => _malusLawButton;

        public TextbookButtons()
        {
            _divider = new Divider
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Axis = DividerAxis.Vertical,
            };
            AddSubview(_divider);

            _malusLawButton = new TextbookButton
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("back_logo"),
                Text = "MALUS' LAW"
            };
            AddSubview(_malusLawButton);

            _divider.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _divider.HeightAnchor.ConstraintEqualTo(HeightAnchor).Active = true;

            _malusLawButton.LeftAnchor.ConstraintEqualTo(_divider.RightAnchor, 40).Active = true;
            _malusLawButton.RightAnchor.ConstraintEqualTo(RightAnchor, -30).Active = true;
            _malusLawButton.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _malusLawButton.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;
        }
    }
}

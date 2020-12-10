using System;
using Foundation;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class ToggleButton : UIView
    {
        private bool _active;
        public bool Active
        {
            get => _active;
            set {
                _active = value;

                UpdateBackground();
                UpdateTextColor();
                UpdateBorder();

                ActiveChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _label.Text = Text;
            }
        }

        private UIColor _inactiveBackgroundColor;
        public UIColor InactiveBackgroundColor
        {
            get => _inactiveBackgroundColor;
            set {
                _inactiveBackgroundColor = value;
                UpdateBackground();
            }
        }
        private UIColor _activeBackgroundColor;
        public UIColor ActiveBackgroundColor
        {
            get => _activeBackgroundColor;
            set
            {
                _activeBackgroundColor = value;
                UpdateBackground();
            }
        }

        private UIColor _inactiveTextColor;
        public UIColor InactiveTextColor
        {
            get => _inactiveTextColor;
            set {
                _inactiveTextColor = value;
                UpdateTextColor();
            }
        }
        private UIColor _activeTextColor;
        public UIColor ActiveTextColor
        {
            get => _activeTextColor;
            set
            {
                _activeTextColor = value;
                UpdateTextColor();
            }
        }

        private UIColor _inactiveBorderColor;
        public UIColor InactiveBorderColor
        {
            get => _inactiveBorderColor;
            set
            {
                _inactiveBorderColor = value;
                UpdateBorder();
            }
        }
        private UIColor _activeBorderColor;
        public UIColor ActiveBorderColor
        {
            get => _activeBorderColor;
            set
            {
                _activeBorderColor = value;
                UpdateBorder();
            }
        }

        private UILabel _label;

        public event EventHandler ActiveChanged;

        public ToggleButton()
        {
            ActiveBackgroundColor = ColorPalette.AccentDark;
            InactiveBackgroundColor = UIColor.Clear;
            ActiveBorderColor = UIColor.Clear;
            InactiveBorderColor = ColorPalette.Border;
            Active = false;

            Layer.CornerRadius = 10;
            Layer.BorderWidth = 2;

            _label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(18, UIFontWeight.Regular),
            };
            AddSubview(_label);

            ActiveTextColor = ColorPalette.PrimaryText;
            InactiveTextColor = ColorPalette.SecondaryText;

            _label.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            _label.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;

            HeightAnchor.ConstraintGreaterThanOrEqualTo(_label.HeightAnchor, 2f).Active = true;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            Active = !Active;
        }

        private void UpdateBackground()
        {
            BackgroundColor = Active ? ActiveBackgroundColor : InactiveBackgroundColor;
        }
        private void UpdateTextColor()
        {
            if (_label != null)
            {
                _label.TextColor = Active ? ActiveTextColor : InactiveTextColor;
            }
        }
        private void UpdateBorder()
        {
            if (ActiveBorderColor != null && InactiveBorderColor != null)
            {
                Layer.BorderColor = Active ? ActiveBorderColor.CGColor : InactiveBorderColor.CGColor;
            }
        }
    }
}

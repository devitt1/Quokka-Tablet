using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class IconButton : UIButton
    {
        private UIImageView _icon;
        private UILabel _label;

        private NSLayoutConstraint _spacingConstraint;
        private NSLayoutConstraint _imageAspectConstraint;
        private NSLayoutConstraint _imageHeightConstraint;

        public UIImage Icon
        {
            get => _icon.Image;
            set
            {
                _icon.Image = value;
                RecalculateImageAspect();
            }
        }

        public string Text
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        public UIColor TextColor
        {
            get => _label.TextColor;
            set => _label.TextColor = value;
        }

        public override UIFont Font
        {
            get => _label.Font;
            set
            {
                _label.Font = value;
                RecalculateImageHeight();
            }
        }

        private float _spacing;
        public float Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                RecalculateSpacing();
            }
        }

        public IconButton()
        {
            _icon = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            AddSubview(_icon);

            _label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = ColorPalette.SecondaryText,
            };
            AddSubview(_label);

            DirectionalLayoutMargins = new NSDirectionalEdgeInsets(10, 10, 10, 10);
            Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular);
            Spacing = 10;

            LayoutMarginsGuide.HeightAnchor.ConstraintGreaterThanOrEqualTo(_icon.HeightAnchor).Active = true;
            LayoutMarginsGuide.HeightAnchor.ConstraintGreaterThanOrEqualTo(_label.HeightAnchor).Active = true;

            _icon.LeftAnchor.ConstraintEqualTo(LayoutMarginsGuide.LeftAnchor).Active = true;
            _icon.CenterYAnchor.ConstraintEqualTo(LayoutMarginsGuide.CenterYAnchor).Active = true;

            _label.RightAnchor.ConstraintEqualTo(LayoutMarginsGuide.RightAnchor).Active = true;
            _label.CenterYAnchor.ConstraintEqualTo(LayoutMarginsGuide.CenterYAnchor).Active = true;

            RecalculateImageHeight();
            RecalculateSpacing();
        }

        private void RecalculateImageAspect()
        {
            if (_imageAspectConstraint != null)
            {
                _imageAspectConstraint.Active = false;
            }
            if (_icon.Image != null)
            {
                _imageAspectConstraint = _icon.AspectRatioConstraint();
                _imageAspectConstraint.Active = true;
            }
        }

        private void RecalculateImageHeight()
        {
            if (_imageHeightConstraint != null)
            {
                _imageHeightConstraint.Active = false;
            }

            _imageHeightConstraint = _icon.HeightAnchor.ConstraintEqualTo(Font.PointSize * 2);
            _imageHeightConstraint.Active = true;
        }

        private void RecalculateSpacing()
        {
            if (_spacingConstraint != null)
            {
                _spacingConstraint.Active = false;
            }

            _spacingConstraint = _label.LeftAnchor.ConstraintEqualTo(_icon.RightAnchor, Spacing);
            _spacingConstraint.Active = true;
        }
    }
}

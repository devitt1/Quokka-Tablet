using System;
using UIKit;

namespace TheQTablet.iOS
{
    public static class ButtonGenerator
    {
        private const float _defaultHeight = 70;
        private const string _defaultTitle = "";

        public static UIButton UnstyledButton(string title = _defaultTitle, float height = _defaultHeight)
        {
            var button = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = ColorPalette.AccentLight,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular)
            };

            button.SetTitle(title, UIControlState.Normal);
            button.SetTitle(title, UIControlState.Highlighted);
            button.SetTitle(title, UIControlState.Selected);

            button.HeightAnchor.ConstraintEqualTo(height).Active = true;
            button.WidthAnchor.ConstraintEqualTo(200).Active = true;

            button.Layer.CornerRadius = 14;

            return button;
        }

        public static UIButton PrimaryButton(string title = _defaultTitle, float height = _defaultHeight)
        {
            var button = UnstyledButton(title, height);

            button.BackgroundColor = ColorPalette.AccentDark;
            button.SetTitleColor(ColorPalette.PrimaryText, UIControlState.Normal);

            return button;
        }

        public static UIButton SecondaryButton(string title = _defaultTitle, float height = _defaultHeight)
        {
            var button = UnstyledButton(title, height);

            button.BackgroundColor = ColorPalette.AccentLight;
            button.SetTitleColor(ColorPalette.TertiaryText, UIControlState.Normal);

            return button;
        }
    }
}

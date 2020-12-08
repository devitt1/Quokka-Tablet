using System;
using UIKit;

namespace TheQTablet.iOS
{
    public static class ButtonGenerator
    {
        public static UIButton DarkButton(string title = "", float height = 70)
        {
            var button = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = ColorPalette.AccentLight,
                Font = FontGenerator.GenerateFont(24, UIFontWeight.Regular)
            };

            button.SetTitleColor(ColorPalette.PrimaryText, UIControlState.Normal);
            button.SetTitle(title, UIControlState.Normal);
            button.SetTitle(title, UIControlState.Highlighted);
            button.SetTitle(title, UIControlState.Selected);
            button.HeightAnchor.ConstraintEqualTo(height).Active = true;
            button.WidthAnchor.ConstraintEqualTo(200).Active = true;

            //button.Layer.BorderWidth = 2;
            button.Layer.CornerRadius = 6;
            //button.Layer.AddSublayer(gradient);
            return button;
        }
    }
}

using System;
using UIKit;

namespace TheQTablet.iOS
{
    public static class ButtonGenerator
    {
        public static UIButton DarkButton(string title = "", float height = 58)
        {
            var button = new UIButton()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = ColorPalette.Accent,
                Font = FontGenerator.GenerateFont(17, UIFontWeight.Bold)
            };

            button.SetTitleColor(ColorPalette.AccentLight, UIControlState.Normal);
            button.SetTitle(title, UIControlState.Normal);
            button.SetTitle(title, UIControlState.Highlighted);
            button.SetTitle(title, UIControlState.Selected);
            button.HeightAnchor.ConstraintEqualTo(height).Active = true;
            button.WidthAnchor.ConstraintEqualTo(176).Active = true;

            //button.Layer.BorderWidth = 2;
            button.Layer.CornerRadius = 6;
            //button.Layer.AddSublayer(gradient);
            return button;
        }
    }
}

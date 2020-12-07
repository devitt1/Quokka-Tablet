using System;
using UIKit;

namespace TheQTablet.iOS
{
    public static class ColorPalette
    {
        public static UIColor Primary => FromHexString("#1C3351");
        public static UIColor PrimaryDark => FromHexString("#008653");
        public static UIColor PrimaryLight => FromHexString("#64eab0");

        public static UIColor Accent => FromHexString("#E6AF2E");
        public static UIColor AccentDark => FromHexString("#1C3351");
        public static UIColor AccentLight => FromHexString("#F47527");

        public static UIColor PrimaryText => FromHexString("#FFFFFF");
        public static UIColor SecondaryText => FromHexString("#E69975");

        public static UIColor Border => FromHexString("#E69975");

        public static UIColor BackgroundLight => FromHexString("#5e395a");
        public static UIColor BackgroundDark => FromHexString("#30283F");

        public static UIColor PlotBlue => FromHexString("#5B9CEF");

        public static UIColor ProgressStart => FromHexString("#238ce8");

        private static UIColor FromHexString(string hexValue)
        {
            var colorString = hexValue.Replace("#", "");
            float red, green, blue;

            switch (colorString.Length)
            {
                case 3: // #RGB
                    {
                        red = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(0, 1)), 16) / 255f;
                        green = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(1, 1)), 16) / 255f;
                        blue = Convert.ToInt32(string.Format("{0}{0}", colorString.Substring(2, 1)), 16) / 255f;
                        return UIColor.FromRGB(red, green, blue);
                    }
                case 6: // #RRGGBB
                    {
                        red = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        return UIColor.FromRGB(red, green, blue);
                    }
                case 8: // #AARRGGBB
                    {
                        var alpha = Convert.ToInt32(colorString.Substring(0, 2), 16) / 255f;
                        red = Convert.ToInt32(colorString.Substring(2, 2), 16) / 255f;
                        green = Convert.ToInt32(colorString.Substring(4, 2), 16) / 255f;
                        blue = Convert.ToInt32(colorString.Substring(6, 2), 16) / 255f;
                        return UIColor.FromRGBA(red, green, blue, alpha);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(hexValue), $"Color value {hexValue} is invalid. Expected format #RBG, #RRGGBB, or #AARRGGBB");
            }
        }
    }
}

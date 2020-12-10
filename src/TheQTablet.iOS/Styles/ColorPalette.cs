using System;
using UIKit;

namespace TheQTablet.iOS
{
    public static class ColorPalette
    {
        public static UIColor Primary => FromHexString("#1C3351"); // dark blue
        public static UIColor PrimaryDark => FromHexString("#008653"); // greeny
        public static UIColor PrimaryLight => FromHexString("#64eab0"); // light green

        public static UIColor Accent => FromHexString("#E6AF2E"); // sepia
        public static UIColor AccentDark => FromHexString("#F47527"); // orange
        public static UIColor AccentLight => FromHexString("#b3c7d4"); // light blue

        public static UIColor PrimaryText => FromHexString("#FFFFFF"); // white
        public static UIColor SecondaryText => FromHexString("#E69975"); // light salmon
        public static UIColor TertiaryText => FromHexString("#0e4763"); // dark blue

        public static UIColor Border => FromHexString("#E69975"); // light salmon

        public static UIColor BackgroundLight => FromHexString("#5e395a"); // dark purple
        public static UIColor BackgroundDark => FromHexString("#30283F"); // darker purple

        public static UIColor PlotBlue => FromHexString("#5B9CEF"); // light blue
        //
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

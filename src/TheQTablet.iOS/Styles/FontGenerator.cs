using System;
using UIKit;

namespace TheQTablet.iOS
{
   
    public static class FontGenerator
    {
        public static UIFont GenerateFont(nfloat size, UIFontWeight weight)
        {
            var fontName = "";
            switch (weight)
            {

                case UIFontWeight.Light:
                    fontName = "Roboto-Thin";
                    break;
                case UIFontWeight.Regular:
                    fontName = "Roboto-Regular";
                    break;
                case UIFontWeight.Medium:

                case UIFontWeight.Semibold:
                    fontName = "Roboto-Bold";
                    break;
                case UIFontWeight.Bold:
                    fontName = "Roboto-Black";
                    break;
                    ;
            }
            // This will load default system font with the dynamic size
            //var descriptor = UIFont.GetPreferredFontForTextStyle(style);
            // Then you'll load your custom font with the dynamic size
            return UIFont.FromName(fontName, size);
        }
    }
    
}

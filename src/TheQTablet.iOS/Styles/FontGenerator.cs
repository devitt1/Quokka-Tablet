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
                    fontName = "Akrobat-Thin";
                    break;
                case UIFontWeight.Regular:
                    fontName = "Akrobat-Regular";
                    break;
                case UIFontWeight.Medium:

                case UIFontWeight.Semibold:
                    fontName = "Akrobat-Bold";
                    break;
                case UIFontWeight.Bold:
                    fontName = "Akrobat-Black";
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

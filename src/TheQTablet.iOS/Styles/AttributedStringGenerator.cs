using System;
using Foundation;
using UIKit;

namespace TheQTablet.iOS
{
    public static class AttributedStringGenerator
    {
        public static NSMutableAttributedString Generate(string[] text, UIStringAttributes[] attributes)
        {
            var attributedText = new NSMutableAttributedString(String.Join(null, text));
            var total = 0;
            for(int i = 0; i < text.Length; i++)
            {
                // default to attribute 0 if not enough
                var attributeIndex = i < attributes.Length ? i : 0;
                var attribute = attributes[attributeIndex];
                attributedText.AddAttributes(attribute, new NSRange(total, text[i].Length));
                total += text[i].Length;
            }
            return attributedText;
        }
    }
}

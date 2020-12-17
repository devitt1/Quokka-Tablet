using CoreGraphics;

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class PaddedLabel : UILabel
    {
        public UIEdgeInsets Insets;

        public override void DrawText(CGRect rect)
        {
            var newRect = rect;
            newRect.Width -= Insets.Left;
            newRect.Width -= Insets.Right;
            newRect.Height -= Insets.Top;
            newRect.Height -= Insets.Bottom;
            newRect.X += Insets.Left;
            newRect.Y += Insets.Top;
            base.DrawText(newRect);
        }

        public override CGSize IntrinsicContentSize => CGSize.Add(
            base.IntrinsicContentSize,
            new CGSize(Insets.Left + Insets.Right, Insets.Top + Insets.Bottom)
        );
    }
}

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public static class UIImageViewExtensions
    {
        public static NSLayoutConstraint AspectRatioConstraint(this UIImageView imageView)
        {
            return imageView.WidthAnchor.ConstraintEqualTo(imageView.HeightAnchor, imageView.Image.Size.Width / imageView.Image.Size.Height);
        }
    }
}

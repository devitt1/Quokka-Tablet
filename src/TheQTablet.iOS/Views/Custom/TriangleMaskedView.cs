using CoreAnimation;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class TriangleMaskedView: UIView
    {
        private CAShapeLayer _maskLayer;

        public TriangleMaskedView()
        {
            _maskLayer = new CAShapeLayer();
            Layer.Mask = _maskLayer;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _maskLayer.Frame = Bounds;

            var trianglePath = new UIBezierPath();
            trianglePath.MoveTo(Bounds.Location);
            trianglePath.AddLineTo(new CoreGraphics.CGPoint(Bounds.Right, Bounds.Top));
            trianglePath.AddLineTo(new CoreGraphics.CGPoint(Bounds.Right, Bounds.Bottom));
            trianglePath.AddLineTo(Bounds.Location);
            _maskLayer.Path = trianglePath.CGPath;
        }
    }
}

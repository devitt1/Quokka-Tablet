using CoreAnimation;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class OvalMaskedView: UIView
    {
        private CAShapeLayer _maskLayer;

        public OvalMaskedView()
        {
            _maskLayer = new CAShapeLayer();
            Layer.Mask = _maskLayer;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _maskLayer.Frame = Bounds;
            _maskLayer.Path = UIBezierPath.FromOval(Bounds).CGPath;
        }
    }
}

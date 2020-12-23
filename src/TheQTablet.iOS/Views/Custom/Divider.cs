using System;

using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public enum DividerAxis
    {
        Vertical,
        Horizontal,
    }

    public class Divider : UIView
    {
        private DividerAxis _axis;
        public DividerAxis Axis
        {
            get => _axis;
            set {
                _axis = value;
                SetConstraints();
            }
        }

        private NSLayoutConstraint _constraint;

        public Divider()
        {
            Axis = DividerAxis.Horizontal;

            BackgroundColor = ColorPalette.Border;
        }

        private void SetConstraints()
        {
            if(_constraint != null)
            {
                _constraint.Active = false;
            }
            if(Axis == DividerAxis.Horizontal)
            {
                _constraint = HeightAnchor.ConstraintEqualTo(1);
            }
            else
            {
                _constraint = WidthAnchor.ConstraintEqualTo(1);
            }
            _constraint.Active = true;
        }
    }
}

using System;
using CoreGraphics;
using TheQTablet.Core.Utils;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class KnobView : UIView
    {
        private UIImageView _outerRing;
        private UIView _innerCircleContainer;
        private UIImageView _innerCirclePointer;
        private UILabel _angleLabel;

        private UIPanGestureRecognizer _gestureRecognizer;

        private float _startAngle;
        private int _angle;
        private int SmoothAngle
        {
            get => _angle;
        }
        public int SteppedAngle
        {
            get => AsAngle((int) Round(_angle, Step));
            set
            {
                _angle = AsAngle(value);
                _innerCircleContainer.Transform = CGAffineTransform.MakeRotation(MathHelpers.ToRadF(SmoothAngle));
                _innerCircleContainer.Layer.ShadowOffset = AngleCorrectedOffset(SmoothAngle, _shadowOffset);
                _angleLabel.Text = AngleText(SteppedAngle);
                SteppedAngleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int Step;
        private CGSize _shadowOffset;

        public event EventHandler SteppedAngleChanged;
        public event EventHandler TouchUp;

        public KnobView()
        {
            _gestureRecognizer = new UIPanGestureRecognizer();
            _gestureRecognizer.AddTarget(() => Pan(_gestureRecognizer));
            AddGestureRecognizer(_gestureRecognizer);

            Step = 10;
            _shadowOffset = new CGSize(0, 4);

            _outerRing = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = UIImage.FromBundle("dial_background"),
            };
            AddSubview(_outerRing);

            _innerCircleContainer = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            AddSubview(_innerCircleContainer);

            _innerCirclePointer = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ContentMode = UIViewContentMode.ScaleAspectFill,
                Image = UIImage.FromBundle("dial_top"),
            };
            _innerCircleContainer.AddSubview(_innerCirclePointer);

            _angleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(28, UIFontWeight.Regular),
                TextColor = ColorPalette.SecondaryText,
                Text = AngleText(SmoothAngle),
            };
            AddSubview(_angleLabel);

            WidthAnchor.ConstraintEqualTo(HeightAnchor).Active = true;

            _outerRing.WidthAnchor.ConstraintEqualTo(WidthAnchor).Active = true;
            _outerRing.HeightAnchor.ConstraintEqualTo(HeightAnchor).Active = true;

            _innerCircleContainer.WidthAnchor.ConstraintEqualTo(WidthAnchor, 0.6f).Active = true;
            _innerCircleContainer.HeightAnchor.ConstraintEqualTo(_innerCircleContainer.WidthAnchor).Active = true;
            _innerCircleContainer.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            _innerCircleContainer.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;

            _innerCirclePointer.BottomAnchor.ConstraintEqualTo(_innerCircleContainer.BottomAnchor).Active = true;
            _innerCirclePointer.LeftAnchor.ConstraintEqualTo(_innerCircleContainer.LeftAnchor).Active = true;
            _innerCirclePointer.RightAnchor.ConstraintEqualTo(_innerCircleContainer.RightAnchor).Active = true;
            _innerCirclePointer.HeightAnchor.ConstraintEqualTo(_innerCircleContainer.HeightAnchor, _innerCirclePointer.Image.Size.Height / _innerCirclePointer.Image.Size.Width).Active = true;

            _angleLabel.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            _angleLabel.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
        }

        public void Pan(UIPanGestureRecognizer _gesture)
        {
            if(_gesture.State == UIGestureRecognizerState.Began)
            {
                _startAngle = MathHelpers.ToRadF(SmoothAngle) - GetTouchAngle(_gesture);
            }
            else if(_gesture.State == UIGestureRecognizerState.Changed)
            {
                var angle = GetTouchAngle(_gesture) + _startAngle;
                SteppedAngle = (int) MathHelpers.ToDegF(angle);
            }
            else if(_gesture.State == UIGestureRecognizerState.Ended)
            {
                SteppedAngle = SteppedAngle;
                TouchUp?.Invoke(this, EventArgs.Empty);
            }
        }

        private float GetTouchAngle(UIPanGestureRecognizer _gesture)
        {
            var touchLocation = _gesture.LocationInView(this);
            var centre = new CGPoint(Frame.Width / 2.0f, Frame.Height / 2.0f);

            return (float)Math.Atan2((touchLocation.Y - centre.Y), (touchLocation.X - centre.X));
        }

        private static CGSize AngleCorrectedOffset(float angleDeg, CGSize offset)
        {
            var angleRad = MathHelpers.ToRad(angleDeg);
            return new CGSize(
                offset.Height * (float) Math.Sin(angleRad) + offset.Width * (float)Math.Cos(angleRad),
                offset.Height * (float) Math.Cos(angleRad) + offset.Width * (float)Math.Sin(angleRad)
            );
        }

        private string AngleText(int angle)
        {
            return angle + "°";
        }

        private static int AsAngle(int value)
        {
            var remainder = value % 360;
            if (remainder < 0)
            {
                remainder += 360;
            }
            return remainder;
        }

        private double Round(double number, double increment, double offset = 0)
        {
            return Math.Round((number - offset) / increment) * increment + offset;
        }
    }
}

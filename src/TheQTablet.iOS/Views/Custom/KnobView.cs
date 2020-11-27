using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class KnobView : UIView
    {
        private UIImageView _outerRing;
        private UIImageView _innerCircle;
        private UILabel _angleLabel;

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
                _innerCircle.Transform = CGAffineTransform.MakeRotation((float) ToRad(SmoothAngle));
                _innerCircle.Layer.ShadowOffset = AngleCorrectedOffset(SmoothAngle, _shadowOffset);
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
            Step = 5;
            _shadowOffset = new CGSize(0, 4);

            _outerRing = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = new UIImage("dial.png"),
            };
            AddSubview(_outerRing);

            _innerCircle = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = new UIImage("dial_pointer.png"),
            };
            _innerCircle.Layer.ShadowOpacity = 0.5f;
            _innerCircle.Layer.ShadowRadius = 7;
            _innerCircle.Layer.ShadowColor = UIColor.Black.CGColor;
            _innerCircle.Layer.ShadowOffset = _shadowOffset;
            AddSubview(_innerCircle);

            _angleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular),
                TextColor = ColorPalette.AngleText,
                Text = AngleText(SmoothAngle),
            };
            AddSubview(_angleLabel);

            WidthAnchor.ConstraintEqualTo(HeightAnchor).Active = true;

            _outerRing.WidthAnchor.ConstraintEqualTo(WidthAnchor).Active = true;
            _outerRing.HeightAnchor.ConstraintEqualTo(HeightAnchor).Active = true;

            _innerCircle.WidthAnchor.ConstraintEqualTo(WidthAnchor, 0.9f).Active = true;
            _innerCircle.HeightAnchor.ConstraintEqualTo(_innerCircle.WidthAnchor).Active = true;
            _innerCircle.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            _innerCircle.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;

            _angleLabel.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
            _angleLabel.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            var touch = touches.AnyObject as UITouch;
            _startAngle = (float) ToRad(SmoothAngle) - GetTouchAngle(touch);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            var touch = touches.AnyObject as UITouch;
            var angle = GetTouchAngle(touch) + _startAngle;
            SteppedAngle = (int) ToDeg(angle);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            SteppedAngle = SteppedAngle;
            TouchUp?.Invoke(this, EventArgs.Empty);
        }

        private float GetTouchAngle(UITouch touch)
        {
            var touchLocation = touch.LocationInView(this);
            var centre = new CGPoint(Frame.Width / 2.0f, Frame.Height / 2.0f);

            return (float)Math.Atan2((touchLocation.Y - centre.Y), (touchLocation.X - centre.X));
        }

        private static CGSize AngleCorrectedOffset(float angleDeg, CGSize offset)
        {
            var angleRad = ToRad(angleDeg);
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

        private static double ToRad(double deg)
        {
            return deg * (Math.PI / 180.0);
        }

        private static double ToDeg(double deg)
        {
            return deg * (180.0 / Math.PI);
        }
    }
}

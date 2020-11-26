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
        private float _angle;
        public float Angle
        {
            get => _angle;
            set
            {
                _angle = AsAngle(value);
                _innerCircle.Transform = CGAffineTransform.MakeRotation(_angle);
                _innerCircle.Layer.ShadowOffset = AngleCorrectedOffset(_angle, new CGSize(0, 2));
                _angleLabel.Text = AngleText(_angle);
                AngleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler AngleChanged;
        public event EventHandler TouchUp;

        public KnobView()
        {
            _outerRing = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = new UIImage("dial.png")
            };
            AddSubview(_outerRing);

            _innerCircle = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Image = new UIImage("pointer.png"),
            };
            _innerCircle.Layer.ShadowOpacity = 0.5f;
            _innerCircle.Layer.ShadowRadius = 7;
            _innerCircle.Layer.ShadowColor = UIColor.Black.CGColor;
            AddSubview(_innerCircle);

            _angleLabel = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = FontGenerator.GenerateFont(20, UIFontWeight.Regular),
                TextColor = ColorPalette.AngleText,
                Text = AngleText(Angle),
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
            _startAngle = Angle - GetTouchAngle(touch);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            var touch = touches.AnyObject as UITouch;
            var angle = GetTouchAngle(touch) + _startAngle;
            Angle = (float)angle;
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            TouchUp?.Invoke(this, EventArgs.Empty);
        }

        private float GetTouchAngle(UITouch touch)
        {
            var touchLocation = touch.LocationInView(this);
            var centre = new CGPoint(Frame.Width / 2.0f, Frame.Height / 2.0f);

            return (float)Math.Atan2((touchLocation.Y - centre.Y), (touchLocation.X - centre.X));
        }

        private static float AsAngle(float value)
        {
            var remainder = value % ((float) Math.PI * 2.0f);
            if (remainder < 0)
            {
                remainder += (float) Math.PI * 2.0f;
            }
            return remainder;
        }

        private static CGSize AngleCorrectedOffset(float angle, CGSize offset)
        {
            return new CGSize(
                offset.Height * (float) Math.Sin(angle) + offset.Width * (float)Math.Cos(angle),
                offset.Height * (float) Math.Cos(angle) + offset.Width * (float)Math.Sin(angle)
            );
        }

        private string AngleText(float angle)
        {
            return ((int) (angle * (180 / Math.PI))) + "°";
        }
    }
}

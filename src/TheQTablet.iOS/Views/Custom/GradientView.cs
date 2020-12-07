using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public static class GradientHelpers
    {
        public static void SetGradient<T>(T view) where T: UIView, IGradient
        {
            var layer = view.Layer as CAGradientLayer;
            layer.Colors = new CGColor[] { view.StartColor.CGColor, view.EndColor.CGColor };
            if (view.Angle != null)
            {
                layer.EndPoint = new CGPoint(Math.Cos((double)view.Angle), Math.Sin((double)view.Angle));
                layer.StartPoint = new CGPoint(-layer.EndPoint.X, -layer.EndPoint.Y);
            }
            else
            {
                if (view.Horizontal)
                {
                    layer.StartPoint = new CGPoint(0, 0);
                    layer.EndPoint = new CGPoint(1, 0);
                }
                else
                {
                    layer.StartPoint = new CGPoint(0, 0);
                    layer.EndPoint = new CGPoint(0, 1);
                }
            }
        }
    }

    public interface IGradient
    {
        UIColor StartColor { get; set; }
        UIColor EndColor { get; set; }

        bool Horizontal { get; set; }
        float? Angle { get; set; }
    }

    public class GradientView: UIView, IGradient
    {
        private UIColor _startColor = UIColor.White;
        public UIColor StartColor { get => _startColor; set => _startColor = value; }

        private UIColor _endColor = UIColor.Black;
        public UIColor EndColor { get => _endColor; set => _endColor = value; }

        private bool _horizontal = false;
        public bool Horizontal { get => _horizontal; set => _horizontal = value; }

        private float? _angle = null;
        public float? Angle { get => _angle; set => _angle = value; }

        [Export("layerClass")]
        public static Class LayerClass()
        {
            return new Class(typeof(CAGradientLayer));
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            GradientHelpers.SetGradient(this);
        }
    }

    public class GradientStackView: UIStackView, IGradient
    {
        private UIColor _startColor = UIColor.White;
        public UIColor StartColor { get => _startColor; set => _startColor = value; }

        private UIColor _endColor = UIColor.Black;
        public UIColor EndColor { get => _endColor; set => _endColor = value; }

        private bool _horizontal = false;
        public bool Horizontal { get => _horizontal; set => _horizontal = value; }

        private float? _angle = null;
        public float? Angle { get => _angle; set => _angle = value; }

        [Export("layerClass")]
        public static Class LayerClass()
        {
            return new Class(typeof(CAGradientLayer));
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            GradientHelpers.SetGradient(this);
        }
    }
}

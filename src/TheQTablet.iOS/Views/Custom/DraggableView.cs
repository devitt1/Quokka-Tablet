using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class DraggableView: UIView
    {
        private UIView _dragItem;
        private UIPanGestureRecognizer _recognizer;

        private CGPoint _panStartPosition;
        private NSLayoutConstraint XPositionConstraint;
        private NSLayoutConstraint YPositionConstraint;

        private CGPoint _position;
        public CGPoint Position
        {
            get => _position;
            set
            {
                _position = ClampToBounds(value);
                SetPositionConstraints();
                PositionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler PositionChanged;

        public DraggableView()
        {
            _dragItem = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            base.AddSubview(_dragItem);

            XPositionConstraint = _dragItem.CenterXAnchor.ConstraintEqualTo(LeftAnchor);
            YPositionConstraint = _dragItem.CenterYAnchor.ConstraintEqualTo(TopAnchor);
            XPositionConstraint.Active = true;
            YPositionConstraint.Active = true;

            _recognizer = new UIPanGestureRecognizer();
            _recognizer.AddTarget(() => Pan(_recognizer));
            _dragItem.AddGestureRecognizer(_recognizer);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            SetPositionConstraints();
        }

        private void SetPositionConstraints()
        {
            XPositionConstraint.Constant = Bounds.Width * Position.X;
            YPositionConstraint.Constant = Bounds.Height * Position.Y;
        }

        private void Pan(UIPanGestureRecognizer gesture)
        {
            if (gesture.State == UIGestureRecognizerState.Began)
            {
                _panStartPosition = ToAbsolute(Position);
            }
            var translation = gesture.TranslationInView(this);
            Position = ToProportionOfBounds(CGPoint.Add(_panStartPosition, new CGSize(translation)));
        }

        // Add subviews to drag item
        public override void AddSubview(UIView view)
        {
            _dragItem.AddSubview(view);

            _dragItem.LeftAnchor.ConstraintLessThanOrEqualTo(view.LeftAnchor).Active = true;
            _dragItem.RightAnchor.ConstraintGreaterThanOrEqualTo(view.RightAnchor).Active = true;
            _dragItem.TopAnchor.ConstraintLessThanOrEqualTo(view.TopAnchor).Active = true;
            _dragItem.BottomAnchor.ConstraintGreaterThanOrEqualTo(view.BottomAnchor).Active = true;
        }

        private CGPoint ClampToBounds(CGPoint position)
        {
            var tempX = position.X;
            var tempY = position.Y;
            if (tempX < 0)
            {
                tempX = 0;
            }
            else if (tempX > 1)
            {
                tempX = 1;
            }
            if (tempY < 0)
            {
                tempY = 0;
            }
            else if (tempY > 1)
            {
                tempY = 1;
            }
            return new CGPoint(tempX, tempY);
        }

        private CGPoint ToProportionOfBounds(CGPoint value)
        {
            return new CGPoint(value.X / Bounds.Width, value.Y / Bounds.Height);
        }

        private CGPoint ToAbsolute(CGPoint value)
        {
            return new CGPoint(value.X * Bounds.Width, value.Y * Bounds.Height);
        }
    }
}

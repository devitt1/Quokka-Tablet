using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class InsetProgressBar : UIView
    {
        private float _progress;
        public float Progress
        {
            get => _progress;
            set {
                _progress = value;
                SetProgress();
                ProgressChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler ProgressChanged;

        private UIImage _trackImage;
        public UIImage TrackImage
        {
            get => _trackImage;
            set {
                _trackImage = value;
                _trackView.Image = TrackImage;
            }
        }

        private CAShapeLayer _progressMaskLayer;
        private UIImage _progressImage;
        public UIImage ProgressImage
        {
            get => _progressImage;
            set {
                _progressImage = value;
                _progressView.Image = ProgressImage;
            }
        }

        private float _inset = 5;
        public float Inset
        {
            get => _inset;
            set {
                _inset = value;
                SetLayout();
            }
        }

        private bool _horizontal = true;
        public bool Horizontal
        {
            get => _horizontal;
            set {
                _horizontal = value;
                SetProgress();
            }
        }

        private NSLayoutConstraint[] _insetConstraints;

        private UIImageView _trackView;
        private UIImageView _progressView;

        public InsetProgressBar()
        {
            _trackView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            AddSubview(_trackView);

            _progressView = new UIImageView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                ClipsToBounds = true,
            };
            AddSubview(_progressView);

            _trackView.LeftAnchor.ConstraintEqualTo(LeftAnchor).Active = true;
            _trackView.RightAnchor.ConstraintEqualTo(RightAnchor).Active = true;
            _trackView.TopAnchor.ConstraintEqualTo(TopAnchor).Active = true;
            _trackView.BottomAnchor.ConstraintEqualTo(BottomAnchor).Active = true;

            _progressMaskLayer = new CAShapeLayer();
            _progressView.Layer.Mask = _progressMaskLayer;

            SetLayout();
        }

        private void SetLayout()
        {
            if (_insetConstraints != null && _insetConstraints.Length > 0)
            {
                NSLayoutConstraint.DeactivateConstraints(_insetConstraints);
            }

            _insetConstraints = new NSLayoutConstraint[] {
                _progressView.LeftAnchor.ConstraintEqualTo(LeftAnchor, Inset),
                _progressView.RightAnchor.ConstraintEqualTo(RightAnchor, -Inset),
                _progressView.TopAnchor.ConstraintEqualTo(TopAnchor, Inset),
                _progressView.BottomAnchor.ConstraintEqualTo(BottomAnchor, -Inset),
            };

            NSLayoutConstraint.ActivateConstraints(_insetConstraints);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            SetProgress();
        }

        public void SetProgress()
        {
            if (Horizontal)
            {
                _progressMaskLayer.Path = UIBezierPath.FromRect(new CGRect(
                    0,
                    0,
                    (Bounds.Width - (Inset * 2)) * Progress,
                    Bounds.Height - (Inset * 2)
                )).CGPath;
            }
            else
            {
                _progressMaskLayer.Path = UIBezierPath.FromRect(new CGRect(
                    0,
                    (Bounds.Height - (Inset * 2)) - ((Bounds.Height - (Inset * 2)) * Progress),
                    Bounds.Width - (Inset * 2),
                    Bounds.Height - (Inset * 2)
                )).CGPath;
            }
        }
    }
}

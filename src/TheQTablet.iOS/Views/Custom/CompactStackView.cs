using System;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class CompactStackView: UIStackView
    {
        public bool PadStart = false;

        private readonly UIView _filler;

        public CompactStackView()
        {
            Distribution = UIStackViewDistribution.Fill;

            _filler = new UIView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _filler.SetContentHuggingPriority(0, UILayoutConstraintAxis.Horizontal);
            _filler.SetContentHuggingPriority(0, UILayoutConstraintAxis.Vertical);

            _filler.HeightAnchor.ConstraintEqualTo(10).Active = true;
        }

        public override void AddArrangedSubview(UIView view)
        {
            if (PadStart)
            {
                if (ArrangedSubviews.Length == 0)
                {
                    base.AddArrangedSubview(_filler);
                }
            }
            else if (ArrangedSubviews.Length >= 1)
            {
                base.RemoveArrangedSubview(_filler);
            }

            base.AddArrangedSubview(view);

            if (!PadStart)
            {
                base.AddArrangedSubview(_filler);
            }
        }
    }
}

using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace TheQTablet.iOS.Views.Custom
{
    public class KnobContainerView : UIStackView
    {
        private UILabel _knobHeader;
        public KnobView KnobControl;

        private CAGradientLayer _knobContainerGradient;

        public int Step
        {
            get => KnobControl.Step;
            set => KnobControl.Step = value;
        }

        public KnobContainerView()
        {
            BackgroundColor = UIColor.White;
            Axis = UILayoutConstraintAxis.Vertical;
            Alignment = UIStackViewAlignment.Center;
            Distribution = UIStackViewDistribution.FillProportionally;
            LayoutMarginsRelativeArrangement = true;
            LayoutMargins = new UIEdgeInsets
            {
                Top = 20,
                Bottom = 4,
                Left = 4,
                Right = 4,
            };
            Spacing = 20;

            Layer.ShadowColor = UIColor.Black.CGColor;
            Layer.ShadowOpacity = 0.25f;
            Layer.ShadowOffset = new CGSize(5, 0);
            Layer.ShadowRadius = 0;

            _knobContainerGradient = new CAGradientLayer
            {
                Colors = new CGColor[] {
                    ColorPalette.PlotBackgroundLight.CGColor,
                    ColorPalette.PlotBackgroundDark.CGColor
                }
            };
            Layer.AddSublayer(_knobContainerGradient);

            _knobHeader = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "θ TELESCOPE LENS",
                Font = FontGenerator.GenerateFont(16, UIFontWeight.Bold),
                TextColor = ColorPalette.SecondaryText,
            };
            _knobHeader.SetContentCompressionResistancePriority((float)UILayoutPriority.Required, UILayoutConstraintAxis.Vertical);
            AddArrangedSubview(_knobHeader);

            KnobControl = new KnobView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            AddArrangedSubview(KnobControl);
        }

        public override void LayoutSublayersOfLayer(CALayer layer)
        {
            base.LayoutSublayersOfLayer(layer);

            _knobContainerGradient.Frame = Bounds;
        }
    }
}

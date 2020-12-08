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

        public int Step
        {
            get => KnobControl.Step;
            set => KnobControl.Step = value;
        }

        public KnobContainerView()
        {
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

            _knobHeader = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Text = "TELESCOPE LENS",
                Font = FontGenerator.GenerateFont(22, UIFontWeight.Bold),
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
    }
}
